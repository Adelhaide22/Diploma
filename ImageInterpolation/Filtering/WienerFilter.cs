﻿using Accord.Imaging;
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
        public static Bitmap Filter(Bitmap i)
        {
            var ini = new Bitmap(i.Width, i.Height, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);
            var initialImage = ini.Convert16bppTo8bpp();
            initialImage.ConvertColor8bppToGrayscale8bpp();

            return initialImage;
            var g = ComplexImage.FromBitmap(initialImage);
            var h = ComplexImage.FromBitmap(initialImage);
            var f = ComplexImage.FromBitmap(initialImage);
            var average = GetAverage(g.Data);
            var dispersion = GetDispersion(g.Data, average);
            var H = GetGaussianCore(h, g, 16);

            g.ForwardFourierTransform();
            H.ForwardFourierTransform();

            var F = GetF(f, h, g, average, dispersion);
            F.BackwardFourierTransform();

            return F.ToBitmap();

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

        private static ComplexImage GetGaussianCore(ComplexImage h, ComplexImage g, int dispersion)
        {
            var core = new double[g.Width, g.Height];

            for (int i = 0; i < g.Width; i++)
            {
                for (int j = 0; j < g.Height; j++)
                {
                    core[i, j] = 1 / Math.Sqrt(2 * Math.PI * dispersion) * Math.Exp((i * i + j * j) / (2 * dispersion));
                }
            }

            for (int i = 0; i < h.Width; i++)
            {
                for (int j = 0; j < h.Height; j++)
                {
                    h.Data[i, j] = core[i, j];
                }
            }

            return h;
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
 