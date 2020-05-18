using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageInterpolation.Filtering
{
    public static class MotionFilter
    {
        public static int MotionSize { get; set; }
        public static Direction Direction { get; set; }

        public static Bitmap Motion(Bitmap initialImage, int motionSize, Direction direction = Direction.LeftToRight)
        {
            MotionSize = motionSize;
            Direction = direction;

            var extendedImage = ImageHelper.GetExtended(initialImage, MotionSize);

            var f = new double[3][,];
            f[0] = new double[extendedImage.Width, extendedImage.Height];
            f[1] = new double[extendedImage.Width, extendedImage.Height];
            f[2] = new double[extendedImage.Width, extendedImage.Height];

            for (int i = 0; i < extendedImage.Width; i++)
            {
                for (int j = 0; j < extendedImage.Height; j++)
                {
                    f[0][i, j] = extendedImage.GetPixel(i, j).R;
                    f[1][i, j] = extendedImage.GetPixel(i, j).G;
                    f[2][i, j] = extendedImage.GetPixel(i, j).B;
                }
            }

            var resultImage = new Bitmap(extendedImage);

            var g = f.AsParallel().Select(fi =>
            {
                return MotionBlur(fi);
            }).ToArray();

            for (int i = 0; i < extendedImage.Width; i++)
            {
                for (int j = 0; j < extendedImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, Color.FromArgb((int)g[0][i, j], (int)g[1][i, j], (int)g[2][i, j]));
                }
            }

            return ImageHelper.Crop(resultImage, initialImage, MotionSize);
        }

        public static double[,] GetCore()
        {
            var motion = 1;
            var sum = 0d;

            var motionMatrix = new double[MotionSize, MotionSize];

            for (int i = 0; i < MotionSize; i++)
            {
                for (int j = 0; j < MotionSize; j++)
                {
                    if ((i == j && Direction == Direction.LeftToRight)
                        || (i == MotionSize - j - 1 && Direction == Direction.RightToLeft)
                        || (j == MotionSize/2 && Direction == Direction.Horizontal)
                        || (i == MotionSize / 2 && Direction == Direction.Vertical))
                    {
                        motionMatrix[i, j] = motion;
                    }
                    else
                    {
                        motionMatrix[i, j] = 0;                               
                    }
                    sum += motionMatrix[i, j];
                }
            }

            for (int l = 0; l < MotionSize; l++)
            {
                for (int k = 0; k < MotionSize; k++)
                {
                    motionMatrix[l, k] /= sum;
                }
            }

            return motionMatrix;
        }

        public static double[,] MotionBlur(double[,] f)
        {
            var result = new double[f.GetLength(1), f.GetLength(0)];
            var motionMatrix = GetCore();

            for (int i = MotionSize / 2; i < f.GetLength(1) - MotionSize / 2; i++)
            {
                for (int j = MotionSize / 2; j < f.GetLength(0) - MotionSize / 2; j++)
                {
                    var temp = 0.0;
                    for (int l = -MotionSize / 2; l <= MotionSize / 2; l++)
                    {
                        for (int k = -MotionSize / 2; k <= MotionSize / 2; k++)
                        {
                            temp += f[i - l, j - k] * motionMatrix[MotionSize / 2 + l, MotionSize / 2 + k];
                        }
                    }
                    result[i, j] = temp > 255 ? 255 : temp;
                    result[i, j] = result[i, j] < 0 ? 0 : result[i, j];
                }
            }

            return result;
        }
    }

    public enum Direction
    {
        RightToLeft,
        LeftToRight,
        Horizontal,
        Vertical
    }
}
