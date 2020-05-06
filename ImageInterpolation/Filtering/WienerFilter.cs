using Accord.Imaging;
using Accord.Imaging.Filters;
using Extreme.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImageInterpolation.Filtering
{
    public static class WienerFilter
    {
        public static Bitmap ToGray(Bitmap image)
        {
            var initialImage = image.ConvertTo8bpp();
            initialImage.ConvertColor8bppToGrayscale8bpp();
            return initialImage;
        }

        public static Bitmap Filter(Bitmap image)
        {
            var initialImage = ToGray(image);

            var g = ComplexImage.FromBitmap(initialImage);
            var f = ComplexImage.FromBitmap(initialImage);

            for (int i = 0; i < g.Width; i++)
            {
                for (int j = 0; j < g.Height; j++)
                {
                    g.Data[i, j] = initialImage.GetPixel(i, j).R;
                    f.Data[i, j] = initialImage.GetPixel(i, j).R;
                }
            }

            var H = GetComplexImageFromMatrix(GetGaussianCore(g));

            var average = GetAverage(g.Data);
            var dispersion = GetDispersion(g.Data, average);
            
            g.ForwardFourierTransform();
            g.BackwardFourierTransform();
            H.ForwardFourierTransform();
            
            var F = GetF(f, H, g, average, dispersion);
            F.BackwardFourierTransform();

            var result = F.ToBitmap();
            return result;

            //var g = new double[3][,];
            //g[0] = new double[initialImage.Width, initialImage.Height];
            //g[1] = new double[initialImage.Width, initialImage.Height];
            //g[2] = new double[initialImage.Width, initialImage.Height];

            //for (int i = 0; i < initialImage.Width; i++)
            //{
            //    for (int j = 0; j < initialImage.Height; j++)
            //    {
            //        g[0][i, j] = initialImage.GetPixel(i, j).R;
            //        g[1][i, j] = initialImage.GetPixel(i, j).G;
            //        g[2][i, j] = initialImage.GetPixel(i, j).B;
            //    }
            //}

            //var f = g.AsParallel().Select(gi =>
            //{
            //    var average = GetAverage(gi);
            //    var dispersion = GetDispersion(gi, average);
            //    var h = GetGaussianCore(gi, 16);

            //    var G = FourierTransform.TransformForward(gi);
            //    var H = FourierTransform.TransformForward(h);

            //    var F = GetF(H, G, average, dispersion);
            //    var fi = FourierTransform.TransformBackward(F);
            //    return fi;
            //}).ToArray();

            //var resultImage = new Bitmap(initialImage.Width, initialImage.Height);

            //for (int i = 0; i < initialImage.Width; i++)
            //{
            //    for (int j = 0; j < initialImage.Height; j++)
            //    {
            //        resultImage.SetPixel(i, j, Color.FromArgb((int)f[0][i, j], (int)f[1][i, j], (int)f[2][i, j]));
            //    }
            //}

            //return resultImage;
        }

        private static ComplexImage GetComplexImageFromMatrix(double[,] core)
        {
            var bitmap = new Bitmap(core.GetLength(0), core.GetLength(1));

            var bitmap8bpp = bitmap.ConvertTo8bpp();
            bitmap8bpp.ConvertColor8bppToGrayscale8bpp();

            var complexImage = ComplexImage.FromBitmap(bitmap8bpp);

            for (int i = 0; i < complexImage.Width; i++)
            {
                for (int j = 0; j < complexImage.Height; j++)
                {
                    complexImage.Data[i, j] = core[i, j];
                }
            }

            return complexImage;
        }

        private static ComplexImage GetF(ComplexImage F, ComplexImage H, ComplexImage G, Complex average, Complex dispersion)
        {
            var snr = GetSNR(average, dispersion);
            snr = 1;

            for (int i = 0; i < G.Width; i++)
            {
                for (int j = 0; j < G.Height; j++)
                {
                    F.Data[i, j] = (1 / H.Data[i, j]) * ((H.Data[i, j] * H.Data[i, j]) / (H.Data[i, j] * H.Data[i, j] + snr)) 
                        * G.Data[i, j];
                }
            }
            return F;
        }

        private static Complex GetSNR(Complex average, Complex dispersion)
        {
            return average / Complex.Sqrt(dispersion);
        }

        private static double[,] GetGaussianCore(ComplexImage g)
        {
            var result = new double[g.Width, g.Height];
            var blurSize = 5;
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

            for (int l = 0; l < g.Width/2 - blurSize/2 - 1; l++)
            {
                for (int k = 0; k < g.Height/2 - blurSize/2 - 1; k++)
                {
                    result[l, k] = 0;
                }
            }

            for (int l = g.Width / 2 + blurSize / 2; l < g.Width; l++)
            {
                for (int k = g.Height / 2 + blurSize / 2; k < g.Height; k++)
                {
                    result[l, k] = 0;
                }
            }

            for (int l = g.Width / 2 - blurSize / 2 - 1; l < g.Width / 2 + blurSize / 2; l++)
            {
                for (int k = g.Height / 2 - blurSize / 2 - 1; k < g.Height / 2 + blurSize / 2; k++)
                {
                    result[l, k] = blurMatrix[l - (g.Width / 2 - blurSize / 2 - 1), k - (g.Height / 2 - blurSize / 2 - 1)];
                }
            }

            return result;
        }

        private static Complex GetDispersion(Complex[,] layer, Complex average)
        {
            var sum = new Complex();
            for (int i = 0; i < layer.GetLength(0); i++)
            {
                for (int j = 0; j < layer.GetLength(1); j++)
                {
                    sum += (layer[i, j] - average) * (layer[i, j] - average);
                }
            }

            return sum / layer.Length;
        }

        private static Complex GetAverage(Complex[,] layer)
        {
            var sum = new Complex();
            for (int i = 0; i < layer.GetLength(0); i++)
            {
                for (int j = 0; j < layer.GetLength(1); j++)
                {
                    sum += layer[i, j];
                }
            }
            return sum / layer.Length;
        }

        private static Bitmap ConvertTo8bpp(this Bitmap oldbmp)
        {
            using (var ms = new MemoryStream())
            {
                oldbmp.Save(ms, ImageFormat.Gif);
                ms.Position = 0;
                return (Bitmap)System.Drawing.Image.FromStream(ms);
            }
        }





        private static Complex[,] GetF(Complex[,] H, Complex[,] G, double average, double dispersion)
        {
            var snr = GetSNR(average, dispersion);
            snr = 1;
            var F = new Complex[G.GetLength(0), G.GetLength(1)];

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
 