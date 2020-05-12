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
            var extendedImage = GetExtended(initialImage);
            return extendedImage;

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

            return Crop(resultImage, initialImage);
        }

        private static Bitmap Crop(Bitmap resultImage, Bitmap initialImage)
        {
            var croped = new Bitmap(initialImage.Width, initialImage.Height);

            for (int i = 0; i < croped.Height; i++)
            {
                for (int j = 0; j < croped.Width; j++)
                {
                    croped.SetPixel(i, j, resultImage.GetPixel(i + BlurSize / 2, j + BlurSize / 2));
                }
            }

            return croped;
        }

        private static Bitmap GetExtended(Bitmap initialImage)
        {
            var extended = new Bitmap(initialImage.Width + BlurSize, initialImage.Height + BlurSize);

            //copy to center
            for (int i = BlurSize / 2; i < initialImage.Height + BlurSize / 2 - 1; i++)
            {
                for (int j = BlurSize / 2; j < initialImage.Width + BlurSize / 2 - 1; j++)
                {
                    extended.SetPixel(i, j, initialImage.GetPixel(i - BlurSize/2, j - BlurSize / 2));
                }
            }

            //left right part
            for (int i = BlurSize/2; i < initialImage.Height + BlurSize/2 - 1; i++)
            {
                for (int j = 0; j < BlurSize / 2; j++)
                {
                    extended.SetPixel(i, j, initialImage.GetPixel(i - BlurSize / 2, 0));
                }

                for (int j = initialImage.Width + BlurSize/2 - 1; j < initialImage.Width + BlurSize; j++)
                {
                    extended.SetPixel(i, j, initialImage.GetPixel(i - BlurSize/2, initialImage.Height - 1));
                }
            }

            //up down
            for (int i = 0; i < BlurSize/2; i++)
            {
                for (int j = BlurSize/2; j < initialImage.Width + BlurSize/2 - 1; j++)
                {
                    extended.SetPixel(i, j, initialImage.GetPixel(0, j - BlurSize / 2));
                }
            }
            for (int i = initialImage.Width + BlurSize/2 - 1; i < extended.Width; i++)
            {
                for (int j = BlurSize / 2; j < initialImage.Width + BlurSize / 2 - 1; j++)
                {
                    extended.SetPixel(i, j, initialImage.GetPixel(initialImage.Height - 1, j - BlurSize/2));
                }
            }

            //corners
            for (int i = BlurSize / 2 + 1; i >= 0; i--)
            {
                for (int j = BlurSize / 2 + 1; j >= 0; j--)
                {
                    extended.SetPixel(i, j, Color.FromArgb(
                        (extended.GetPixel(i, j + 1).R + extended.GetPixel(i + 1, j).R) / 2,
                        (extended.GetPixel(i, j + 1).G + extended.GetPixel(i + 1, j).G) / 2,
                        (extended.GetPixel(i, j + 1).B + extended.GetPixel(i + 1, j).B) / 2));
                }
                for (int j = initialImage.Width + BlurSize / 2 - 1; j < initialImage.Width + BlurSize; j++)
                {
                    extended.SetPixel(i, j, Color.FromArgb(
                        (extended.GetPixel(i, j - 1).R + extended.GetPixel(i + 1, j).R) / 2,
                        (extended.GetPixel(i, j - 1).G + extended.GetPixel(i + 1, j).G) / 2,
                        (extended.GetPixel(i, j - 1).B + extended.GetPixel(i + 1, j).B) / 2));
                }
            }
            for (int i = initialImage.Height + BlurSize / 2 - 1; i < initialImage.Height + BlurSize; i++)
            {
                for (int j = initialImage.Width + BlurSize / 2 - 1; j < initialImage.Width + BlurSize; j++)
                {
                    extended.SetPixel(i, j, Color.FromArgb(
                        (extended.GetPixel(i, j - 1).R + extended.GetPixel(i - 1, j).R) / 2,
                        (extended.GetPixel(i, j - 1).G + extended.GetPixel(i - 1, j).G) / 2,
                        (extended.GetPixel(i, j - 1).B + extended.GetPixel(i - 1, j).B) / 2));
                }
                for (int j = BlurSize / 2 + 1; j >= 0; j--)
                {
                    extended.SetPixel(i, j, Color.FromArgb(
                        (extended.GetPixel(i, j + 1).R + extended.GetPixel(i - 1, j).R) / 2,
                        (extended.GetPixel(i, j + 1).G + extended.GetPixel(i - 1, j).G) / 2,
                        (extended.GetPixel(i, j + 1).B + extended.GetPixel(i - 1, j).B) / 2));
                }
            }

            return extended;
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
