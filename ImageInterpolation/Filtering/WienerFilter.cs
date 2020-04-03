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
                    var h = GetGaussianCore(gi, 400);

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
            snr = 1;
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

        internal static Bitmap Blur(Bitmap initialImage)
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
                return GaussianBlur(fi);
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

        internal static Bitmap Sharpen(Bitmap initialImage)
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
                return SharpBlur(fi);
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

        private static double[,] SharpBlur(double[,] f)
        {
            var result = new double[f.GetLength(0), f.GetLength(1)];
            var blurSize = 3;
            var sharp = 50d;
            var sigma = 4;

            var blurMatrix = new double[blurSize, blurSize];
            for (int l = 0; l < blurSize; l++)
            {
                for (int k = 0; k < blurSize; k++)
                {
                    blurMatrix[l, k] = l == k && k == 1 ? sharp : (1-sharp)/8;
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
                    result[i, j] = temp > 255 ? 255 : temp;
                    result[i, j] = result[i, j] < 0 ? 0 : result[i, j];
                }
            }

            return result;
        }

        private static double[,] GaussianBlur(double[,] f)
        {
            var result = new double[f.GetLength(0), f.GetLength(1)];
            var blurSize = 3;
            var sigma = 4;

            var sum = 0.0;
            var blurMatrix = new double[blurSize, blurSize];
            for (int l = 0; l < blurSize; l++)
            {
                for (int k = 0; k < blurSize; k++)
                {
                    blurMatrix[l, k] = 1 / Math.Sqrt(2 * Math.PI * sigma*sigma) * Math.Exp(-(l * l + k * k) / (2 * sigma*sigma));
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

            for (int i = blurSize/2; i < f.GetLength(0)-blurSize/2; i++)
            {
                for (int j = blurSize/2; j < f.GetLength(1)-blurSize/2; j++)
                {
                    var temp = 0.0;
                    for (int l = -blurSize/2; l <= blurSize/2; l++)
                    {
                        for (int k = -blurSize/2; k <= blurSize/2; k++)
                        {
                            temp += f[i - l, j - k]  * blurMatrix[blurSize/2+l, blurSize / 2+k]; 
                        } 
                    }
                    result[i, j] = temp; 
                }
            }

            return result;
        }

        private static Complex<double>[,] Multiplicate(Complex<double>[,] f, Complex<double>[,] h)
        {
            var result = new Complex<double>[f.GetLength(0), f.GetLength(1)];

            for (int i = 0; i < f.GetLength(0); i++)
            {
                for (int j = 0; j < f.GetLength(1); j++)
                {
                    result[i, j] = f[i, j] * h[i, j];
                }
            }

            return result;
        }
    }
}
 