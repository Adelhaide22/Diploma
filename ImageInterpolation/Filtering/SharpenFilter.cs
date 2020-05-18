using System;
using System.Drawing;
using System.Linq;

namespace ImageInterpolation.Filtering
{
    public static class SharpenFilter
    {
        public static int SharpSize { get; set; } = 3;
        public static double SharpPower { get; set; }

        public static Bitmap Sharpen(Bitmap initialImage, int sharpSize)
        {
            SharpPower = Convert.ToDouble(sharpSize);

            var extendedImage = ImageHelper.GetExtended(initialImage, SharpSize);

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
                return Sharp(fi);
            }).ToArray();

            for (int i = 0; i < extendedImage.Width; i++)
            {
                for (int j = 0; j < extendedImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, Color.FromArgb((int)g[0][i, j], (int)g[1][i, j], (int)g[2][i, j]));
                }
            }

            return ImageHelper.Crop(resultImage, initialImage, SharpSize);
        }

        public static double[,] GetCore()
        {
            var sharp = SharpPower;

            var sharpMatrix = new double[SharpSize, SharpSize];
            for (int l = 0; l < SharpSize; l++)
            {
                for (int k = 0; k < SharpSize; k++)
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

            for (int i = SharpSize / 2; i < f.GetLength(0) - SharpSize / 2; i++)
            {
                for (int j = SharpSize / 2; j < f.GetLength(1) - SharpSize / 2; j++)
                {
                    var temp = 0.0;
                    for (int l = -SharpSize / 2; l <= SharpSize / 2; l++)
                    {
                        for (int k = -SharpSize / 2; k <= SharpSize / 2; k++)
                        {
                            temp += f[i - l, j - k] * sharpMatrix[SharpSize / 2 + l, SharpSize / 2 + k];
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
