﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageInterpolation.Filtering
{
    public static class GaussianFilter
    {
        private static int blurSize = 5;

        public static Bitmap Blur(Bitmap initialImage)
        {
            var extended = new Bitmap(initialImage.Width + blurSize, initialImage.Height + blurSize);
            LanczosInterpolator.FillExtended(initialImage, extended);

            var f = new double[3][,];
            f[0] = new double[extended.Width, extended.Height];
            f[1] = new double[extended.Width, extended.Height];
            f[2] = new double[extended.Width, extended.Height];

            for (int i = 0; i < extended.Width; i++)
            {
                for (int j = 0; j < extended.Height; j++)
                {
                    f[0][i, j] = extended.GetPixel(i, j).R;
                    f[1][i, j] = extended.GetPixel(i, j).G;
                    f[2][i, j] = extended.GetPixel(i, j).B;
                }
            }

            var resultImage = new Bitmap(initialImage.Width, initialImage.Height);

            var g = f.AsParallel().Select(fi =>
            {
                return GaussianBlur(fi);
            }).ToArray();

            for (int i = blurSize / 2; i < extended.Width - blurSize; i++)
            {
                for (int j = blurSize / 2; j < extended.Height - blurSize; j++)
                {
                    resultImage.SetPixel(i, j, Color.FromArgb((int)g[0][i, j], (int)g[1][i, j], (int)g[2][i, j]));
                }
            }

            return resultImage;
        }

        public static double[,] GaussianBlur(double[,] f)
        {
            var result = new double[f.GetLength(0), f.GetLength(1)];
            var sigma = 5;

            var sum = 0.0;
            var blurMatrix = new double[blurSize, blurSize];
            for (int l = 0; l < blurSize; l++)
            {
                for (int k = 0; k < blurSize; k++)
                {
                    blurMatrix[l, k] = 1 / Math.Sqrt(2 * Math.PI * sigma * sigma) * Math.Exp(-(l * l + k * k) / (2 * sigma * sigma));
                    sum += blurMatrix[l, k];
                }
            }

            for (int l = 0; l < blurSize; l++)
            {
                for (int k = 0; k < blurSize; k++)
                {
                    blurMatrix[l, k] /= sum;
                }
            }

            for (int i = blurSize / 2; i < f.GetLength(0) - blurSize / 2; i++)
            {
                for (int j = blurSize / 2; j < f.GetLength(1) - blurSize / 2; j++)
                {
                    var temp = 0.0;
                    for (int l = -blurSize / 2; l <= blurSize / 2; l++)
                    {
                        for (int k = -blurSize / 2; k <= blurSize / 2; k++)
                        {
                            temp += f[i - l, j - k] * blurMatrix[blurSize / 2 + l, blurSize / 2 + k];
                        }
                    }
                    result[i, j] = temp;
                }
            }

            return result;
        }
    }
}
