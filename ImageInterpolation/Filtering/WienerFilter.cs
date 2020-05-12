using Accord.Imaging;
using System;
using System.Drawing;
using System.Numerics;

namespace ImageInterpolation.Filtering
{
    public static class WienerFilter
    {
        public static Bitmap Filter(Bitmap initialImage)
        {
            var g = ComplexImage.FromBitmap(initialImage);
            var h = GetComplexImageFromMatrix(GetCore(g, "gauss"));

            var average = GetAverage(g.Data);
            var dispersion = GetDispersion(g.Data, average);
            var snr = GetSNR(average, dispersion);

            var G = GetComplexImageFromMatrix(ImageHelper.FFT2(ToVector(g.Data)));
            var H = GetComplexImageFromMatrix(ImageHelper.FFT2(ToVector(h.Data)));

            var F = GetF(H, G, 0.015);

            var f = GetComplexImageFromMatrix(ImageHelper.BFT2(ToVector(F.Data)));

            //rotate
            for (int i = 0; i < f.Height/2; i++)
            {
                for (int j = 0; j < f.Width/2; j++)
                {
                    var t = f.Data[i, j];
                    f.Data[i, j] = f.Data[i + f.Height / 2, j + f.Width / 2];
                    f.Data[i + f.Height / 2, j + f.Width / 2] = t;

                    t = f.Data[i + f.Height / 2, j];
                    f.Data[i + f.Height / 2, j] = f.Data[i, j + f.Width / 2];
                    f.Data[i, j + f.Width / 2] = t;
                }
            }

            return f.ToBitmap();
        }

        private static Complex[] ToVector(Complex[,] data)
        {
            var vec = new Complex[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                vec[i] = data[(i - i%data.GetLength(1)) / data.GetLength(1), i % data.GetLength(1)];
            }

            return vec;
        }
        private static ComplexImage GetComplexImageFromMatrix(Complex[,] core)
        {
            var bitmap = new Bitmap(core.GetLength(1), core.GetLength(0));

            var bitmap8bpp = bitmap.ConvertTo8bpp();
            bitmap8bpp.ConvertColor8bppToGrayscale8bpp();

            var complexImage = ComplexImage.FromBitmap(bitmap8bpp);

            for (int i = 0; i < complexImage.Height; i++)
            {
                for (int j = 0; j < complexImage.Width; j++)
                {
                    complexImage.Data[i, j] = core[i, j];
                }
            }

            return complexImage;
        }

        private static ComplexImage GetComplexImageFromMatrix(double[,] core)
        {
            var bitmap = new Bitmap(core.GetLength(1), core.GetLength(0));

            var bitmap8bpp = bitmap.ConvertTo8bpp();
            bitmap8bpp.ConvertColor8bppToGrayscale8bpp();

            var complexImage = ComplexImage.FromBitmap(bitmap8bpp);

            for (int i = 0; i < complexImage.Height; i++)
            {
                for (int j = 0; j < complexImage.Width; j++)
                {
                    complexImage.Data[i, j] = core[i, j];
                }
            }

            return complexImage;
        }

        private static ComplexImage GetF(ComplexImage H, ComplexImage G, Complex snr)
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

        private static Complex GetSNR(Complex average, Complex dispersion)
        {
            return average / Complex.Sqrt(dispersion);
        }

        private static double[,] GetCore(ComplexImage g, string type)
        {
            var matrixSize = 0;
            var matrix = new double[matrixSize, matrixSize];

            if (type == "gauss")
            {
                matrixSize = GaussianFilter.BlurSize;
                matrix = GaussianFilter.GetCore();
            }
            if (type == "sharp")
            {
                matrixSize = SharpenFilter.SharpSize;
                matrix = SharpenFilter.GetCore();
            }
            if (type == "predict")
            {

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

        private static Complex GetDispersion(Complex[,] layer, Complex average)
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

        private static Complex GetAverage(Complex[,] layer)
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
 