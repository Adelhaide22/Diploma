using System;
using System.Drawing;
using System.Linq;

namespace ImageInterpolation.Filtering
{
    public static class GaussianFilter
    {
        public static int BlurSize = 29;

        public static Bitmap Blur(Bitmap initialImage)
        {
            var extendedImage = ImageHelper.GetExtended(initialImage, BlurSize);

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
                return GaussianBlur(fi);
            }).ToArray();

            for (int i = 0; i < extendedImage.Width; i++)
            {
                for (int j = 0; j < extendedImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, Color.FromArgb((int)g[0][i, j], (int)g[1][i, j], (int)g[2][i, j]));
                }
            }

            return ImageHelper.Crop(resultImage, initialImage, BlurSize);
        }
        
        public static double[,] GetCore()
        {
            var sigma = 7;

            var sum = 0.0;
            var blurMatrix = new double[BlurSize, BlurSize];
            for (int l = 0; l < BlurSize; l++)
            {
                for (int k = 0; k < BlurSize; k++)
                {
                    blurMatrix[l, k] = 1 / Math.Sqrt(2 * Math.PI * sigma * sigma) * Math.Exp(-(l * l + k * k) / (2 * sigma * sigma));
                    sum += blurMatrix[l, k];
                }
            }

            for (int l = 0; l < BlurSize; l++)
            {
                for (int k = 0; k < BlurSize; k++)
                {
                    blurMatrix[l, k] /= sum;
                }
            }

            return blurMatrix;
        }

        public static double[,] GaussianBlur(double[,] f)
        {
            var result = new double[f.GetLength(1), f.GetLength(0)];
            var blurMatrix = GetCore();

            for (int i = BlurSize / 2; i < f.GetLength(1) - BlurSize / 2; i++)
            {
                for (int j = BlurSize / 2; j < f.GetLength(0) - BlurSize / 2; j++)
                {
                    var temp = 0.0;
                    for (int l = -BlurSize / 2; l <= BlurSize / 2; l++)
                    {
                        for (int k = -BlurSize / 2; k <= BlurSize / 2; k++)
                        {
                            temp += f[i - l, j - k] * blurMatrix[BlurSize / 2 + l, BlurSize / 2 + k];
                        }
                    }
                    result[i, j] = temp;
                }
            }

            return result;
        }
    }
}
