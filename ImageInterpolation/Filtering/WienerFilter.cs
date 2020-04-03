using Extreme.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImageInterpolation.Filtering
{
    public static class WienerFilter
    {
        public static Bitmap Filter(Bitmap initialImage)
        {
                var g = new double[3][,];
                g[0] = new double[initialImage.Width, initialImage.Height];
                g[1] = new double[initialImage.Width, initialImage.Height];
                g[2] = new double[initialImage.Width, initialImage.Height];

                for (int i = 0; i < initialImage.Width; i++)
                {
                    for (int j = 0; j < initialImage.Height; j++)
                    {
                        g[0][i, j] = initialImage.GetPixel(i, j).R;
                        g[1][i, j] = initialImage.GetPixel(i, j).G;
                        g[2][i, j] = initialImage.GetPixel(i, j).B;
                    }
                }

                var f = g.AsParallel().Select(gi =>
                {
                    var average = GetAverage(gi);
                    var dispersion = GetDispersion(gi, average);
                    var h = GetGaussianCore(gi, 16);

                    var G = FourierTransform.TransformForward(gi);
                    var H = FourierTransform.TransformForward(h);

                    var F = GetF(H, G, average, dispersion);
                    var fi = FourierTransform.TransformBackward(F);
                    return fi;
                }).ToArray();

                var resultImage = new Bitmap(initialImage.Width, initialImage.Height);

                for (int i = 0; i < initialImage.Width; i++)
                {
                    for (int j = 0; j < initialImage.Height; j++)
                    {
                        resultImage.SetPixel(i, j, Color.FromArgb((int)f[0][i, j], (int)f[1][i, j], (int)f[2][i, j]));
                    }
                }

                return resultImage;
        }

        private static Complex<double>[,] GetF(Complex<double>[,] H, Complex<double>[,] G, double average, double dispersion)
        {
            var snr = GetSNR(average, dispersion);
            //snr = 1;
            var F = new Complex<double>[G.GetLength(0), G.GetLength(1)];

            for (int i = 0; i < G.GetLength(0); i++)
            {
                for (int j = 0; j < G.GetLength(1); j++)
                {
                    F[i, j] = (1 / H[i, j]) * ((H[i, j] * H[i, j]) / (H[i, j] * H[i, j] + snr)) * G[i, j];
                }
            }

            return F;
        }

        private static double[,] GetGaussianCore(double[,] g, double dispersion)
        {
            var core = new double[g.GetLength(0), g.GetLength(1)];

            for (int i = 0; i < g.GetLength(0); i++)
            {
                for (int j = 0; j < g.GetLength(1); j++)
                {
                    core[i, j] = 1 / Math.Sqrt(2 * Math.PI * dispersion) * Math.Exp((i * i + j * j) / (2 * dispersion));
                }
            }
            return core;
        }

        private static double GetDispersion(double[,] layer, double average)
        {
            var sum = 0.0;
            for (int i = 0; i < layer.GetLength(0); i++)
            {
                for (int j = 0; j < layer.GetLength(1); j++)
                {
                    sum += Math.Pow((layer[i, j] - average), 2);
                }
            }

            return sum / layer.Length;
        }

        private static double GetAverage(double[,] layer)
        {
            var sum = 0.0;
            for (int i = 0; i < layer.GetLength(0); i++)
            {
                for (int j = 0; j < layer.GetLength(1); j++)
                {
                    sum += layer[i, j];
                }
            }
            return sum / layer.Length;
        }
       
        private static double GetSNR(double average, double dispersion)
        {
            return average / Math.Sqrt(dispersion);
        }
    }
}
 