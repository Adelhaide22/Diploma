using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageInterpolation.Filtering
{
    public static class SharpenFilter
    {
        private static int sharpSize = 9;

        public static Bitmap Sharpen(Bitmap initialImage)
        {
            var f = new double[3][,];
            f[0] = new double[initialImage.Width, initialImage.Height];
            f[1] = new double[initialImage.Width, initialImage.Height];
            f[2] = new double[initialImage.Width, initialImage.Height];

            for (int i = 0; i < initialImage.Width; i++)
            {
                for (int j = 0; j < initialImage.Height; j++)
                {
                    f[0][i, j] = initialImage.GetPixel(i, j).R;
                    f[1][i, j] = initialImage.GetPixel(i, j).G;
                    f[2][i, j] = initialImage.GetPixel(i, j).B;
                }
            }

            var resultImage = new Bitmap(initialImage.Width, initialImage.Height);

            var g = f.AsParallel().Select(fi =>
            {
                return Sharp(fi);
            }).ToArray();

            for (int i = 0; i < initialImage.Width; i++)
            {
                for (int j = 0; j < initialImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, Color.FromArgb((int)g[0][i, j], (int)g[1][i, j], (int)g[2][i, j]));
                }
            }

            return resultImage;
        }

        public static double[,] GetCore()
        {
            var sharp = 5d;

            var sharpMatrix = new double[sharpSize, sharpSize];
            for (int l = 0; l < sharpSize; l++)
            {
                for (int k = 0; k < sharpSize; k++)
                {
                    sharpMatrix[l, k] = l == k && k == 1 ? sharp : (1 - sharp) / 8;
                }
            }

            return sharpMatrix;
        }

        private static double[,] Sharp(double[,] f)
        {
            var result = new double[f.GetLength(0), f.GetLength(1)];
            var sharpMatrix = GetCore();

            for (int i = sharpSize / 2; i < f.GetLength(0) - sharpSize / 2; i++)
            {
                for (int j = sharpSize / 2; j < f.GetLength(1) - sharpSize / 2; j++)
                {
                    var temp = 0.0;
                    for (int l = -sharpSize / 2; l <= sharpSize / 2; l++)
                    {
                        for (int k = -sharpSize / 2; k <= sharpSize / 2; k++)
                        {
                            temp += f[i - l, j - k] * sharpMatrix[sharpSize / 2 + l, sharpSize / 2 + k];
                        }
                    }
                    result[i, j] = temp > 255 ? 255 : temp;
                    result[i, j] = result[i, j] < 0 ? 0 : result[i, j];
                }
            }

            return result;
        }
    }
}
