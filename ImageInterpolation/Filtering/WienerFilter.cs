using Accord.Imaging;
using System;
using System.Drawing;
using System.Numerics;

namespace ImageInterpolation.Filtering
{
    public static class WienerFilter
    {
        public static Bitmap Filter(Bitmap initialImage, ImageHelper.Filter filter)
        {
            var g = ComplexImage.FromBitmap(initialImage);
            var h = ImageHelper.GetComplexImageFromMatrix(GetCore(g, filter));

            var snr = GetSNR(g.Data);

            var G = ImageHelper.GetComplexImageFromMatrix(ImageHelper.FFT2(ImageHelper.ToVector(g.Data)));
            var H = ImageHelper.GetComplexImageFromMatrix(ImageHelper.FFT2(ImageHelper.ToVector(h.Data)));

            var F = GetF(H, G, 0.015);

            var f = ImageHelper.GetComplexImageFromMatrix(ImageHelper.BFT2(ImageHelper.ToVector(F.Data)));

            ImageHelper.Rotate(f);            

            return f.ToBitmap();
        }

        public static ComplexImage GetF(ComplexImage H, ComplexImage G, Complex snr)
        {            
            var bitmap = new Bitmap(G.Width, G.Height);
            var bitmap8bpp = bitmap.ConvertTo8bpp();
            bitmap8bpp.ConvertColor8bppToGrayscale8bpp();

            var complexImage = ComplexImage.FromBitmap(bitmap8bpp);

            for (int i = 0; i < G.Height; i++)
            {
                for (int j = 0; j < G.Width; j++)
                {
                    complexImage.Data[i, j] = ((1 / H.Data[i, j]) * (Math.Pow(Complex.Abs(H.Data[i, j]), 2)
                        / (Math.Pow(Complex.Abs(H.Data[i, j]), 2) + snr))) * G.Data[i, j];
                }
            }
            return complexImage;
        }

        public static Complex GetSNR(Complex[,] data)
        {
            var average = GetAverage(data);
            var dispersion = GetDispersion(data, average);
            return GetAverage(data) / Complex.Sqrt(dispersion);
        }

        public static double[,] GetCore(ComplexImage g, ImageHelper.Filter filter)
        {
            var matrixSize = 0;
            var matrix = new double[matrixSize, matrixSize];

            switch (filter)
            {
                case ImageHelper.Filter.Gauss:
                    matrixSize = GaussianFilter.BlurSize;
                    matrix = GaussianFilter.GetCore();
                    break;
                case ImageHelper.Filter.Sharpen:
                    matrixSize = SharpenFilter.SharpSize;
                    matrix = SharpenFilter.GetCore();
                    break;
                case ImageHelper.Filter.Motion:
                    matrixSize = MotionFilter.MotionSize;
                    matrix = MotionFilter.GetCore();
                    break;
                default:
                    break;
            }

            var result = new double[g.Width, g.Height];

            for (int l = 0; l < g.Height / 2 - matrixSize / 2 - 1; l++)
            {
                for (int k = 0; k < g.Width / 2 - matrixSize / 2 - 1; k++)
                {
                    result[l, k] = 0;
                }
            }

            for (int l = g.Height / 2 + matrixSize / 2; l < g.Height; l++)
            {
                for (int k = g.Width / 2 + matrixSize / 2; k < g.Width; k++)
                {
                    result[l, k] = 0;
                }
            }

            for (int l = g.Height / 2 - matrixSize / 2 - 1; l < g.Height / 2 + matrixSize / 2; l++)
            {
                for (int k = g.Width / 2 - matrixSize / 2 - 1; k < g.Width / 2 + matrixSize / 2; k++)
                {
                    result[l, k] = matrix[l - (g.Height / 2 - matrixSize / 2 - 1), k - (g.Width / 2 - matrixSize / 2 - 1)];
                }
            }

            return result;
        }

        public static Complex GetDispersion(Complex[,] layer, Complex average)
        {
            var sum = new Complex();
            for (int i = 0; i < layer.GetLength(1); i++)
            {
                for (int j = 0; j < layer.GetLength(0); j++)
                {
                    sum += (layer[i, j] - average) * (layer[i, j] - average);
                }
            }

            return sum / layer.Length;
        }

        public static Complex GetAverage(Complex[,] layer)
        {
            var sum = new Complex();
            for (int i = 0; i < layer.GetLength(1); i++)
            {
                for (int j = 0; j < layer.GetLength(0); j++)
                {
                    sum += layer[i, j];
                }
            }
            return sum / layer.Length;
        }
    }
}
 