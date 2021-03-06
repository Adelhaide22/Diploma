﻿using Accord.Imaging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;

namespace ImageInterpolation.Filtering
{
    public static class ImageHelper
    {
        public static Bitmap Crop(Bitmap resultImage, Bitmap initialImage, int matrixSize)
        {
            var croped = new Bitmap(initialImage.Width, initialImage.Height);

            for (int i = 0; i < croped.Height; i++)
            {
                for (int j = 0; j < croped.Width; j++)
                {
                    croped.SetPixel(i, j, resultImage.GetPixel(i + matrixSize / 2, j + matrixSize / 2));
                }
            }

            return croped;
        }

        public static Bitmap GetExtended(Bitmap initialImage, int matrixSize)
        {
            var extended = new Bitmap(initialImage.Width + matrixSize, initialImage.Height + matrixSize);

            //copy to center
            for (int i = matrixSize / 2; i < initialImage.Height + matrixSize / 2 - 1; i++)
            {
                for (int j = matrixSize / 2; j < initialImage.Width + matrixSize / 2 - 1; j++)
                {
                    extended.SetPixel(i, j, initialImage.GetPixel(i - matrixSize / 2, j - matrixSize / 2));
                }
            }

            //left right
            for (int i = matrixSize / 2; i < initialImage.Height + matrixSize / 2 - 1; i++)
            {
                for (int j = 0; j < matrixSize / 2; j++)
                {
                    extended.SetPixel(i, j, initialImage.GetPixel(i - matrixSize / 2, 0));
                }

                for (int j = initialImage.Width + matrixSize / 2 - 1; j < initialImage.Width + matrixSize; j++)
                {
                    extended.SetPixel(i, j, initialImage.GetPixel(i - matrixSize / 2, initialImage.Height - 1));
                }
            }

            //up down
            for (int i = 0; i < matrixSize / 2; i++)
            {
                for (int j = matrixSize / 2; j < initialImage.Width + matrixSize / 2 - 1; j++)
                {
                    extended.SetPixel(i, j, initialImage.GetPixel(0, j - matrixSize / 2));
                }
            }
            for (int i = initialImage.Width + matrixSize / 2 - 1; i < extended.Width; i++)
            {
                for (int j = matrixSize / 2; j < initialImage.Width + matrixSize / 2 - 1; j++)
                {
                    extended.SetPixel(i, j, initialImage.GetPixel(initialImage.Height - 1, j - matrixSize / 2));
                }
            }

            //corners
            for (int i = matrixSize / 2 + 1; i >= 0; i--)
            {
                for (int j = matrixSize / 2 + 1; j >= 0; j--)
                {
                    extended.SetPixel(i, j, Color.FromArgb(
                        (extended.GetPixel(i, j + 1).R + extended.GetPixel(i + 1, j).R) / 2,
                        (extended.GetPixel(i, j + 1).G + extended.GetPixel(i + 1, j).G) / 2,
                        (extended.GetPixel(i, j + 1).B + extended.GetPixel(i + 1, j).B) / 2));
                }
                for (int j = initialImage.Width + matrixSize / 2 - 1; j < initialImage.Width + matrixSize; j++)
                {
                    extended.SetPixel(i, j, Color.FromArgb(
                        (extended.GetPixel(i, j - 1).R + extended.GetPixel(i + 1, j).R) / 2,
                        (extended.GetPixel(i, j - 1).G + extended.GetPixel(i + 1, j).G) / 2,
                        (extended.GetPixel(i, j - 1).B + extended.GetPixel(i + 1, j).B) / 2));
                }
            }
            for (int i = initialImage.Height + matrixSize / 2 - 1; i < initialImage.Height + matrixSize; i++)
            {
                for (int j = initialImage.Width + matrixSize / 2 - 1; j < initialImage.Width + matrixSize; j++)
                {
                    extended.SetPixel(i, j, Color.FromArgb(
                        (extended.GetPixel(i, j - 1).R + extended.GetPixel(i - 1, j).R) / 2,
                        (extended.GetPixel(i, j - 1).G + extended.GetPixel(i - 1, j).G) / 2,
                        (extended.GetPixel(i, j - 1).B + extended.GetPixel(i - 1, j).B) / 2));
                }
                for (int j = matrixSize / 2 + 1; j >= 0; j--)
                {
                    extended.SetPixel(i, j, Color.FromArgb(
                        (extended.GetPixel(i, j + 1).R + extended.GetPixel(i - 1, j).R) / 2,
                        (extended.GetPixel(i, j + 1).G + extended.GetPixel(i - 1, j).G) / 2,
                        (extended.GetPixel(i, j + 1).B + extended.GetPixel(i - 1, j).B) / 2));
                }
            }

            return extended;
        }

        internal static double[,] Normalize(double[,] initialImage)
        {
            var max = GetMax(initialImage);
            var min = GetMin(initialImage);

            var result = new double[initialImage.GetLength(1), initialImage.GetLength(1)];
            for (int i = 0; i < initialImage.GetLength(1); i++)
            {
                for (int j = 0; j < initialImage.GetLength(1); j++)
                {
                    result[i, j] = (initialImage[i, j] - min)/(max-min) * 255;
                }
            }
            return result;
        }

        internal static Bitmap ToBitmap(double[,] initialImage)
        {
            var result = new Bitmap(initialImage.GetLength(1), initialImage.GetLength(1));
            for (int i = 0; i < initialImage.GetLength(1); i++)
            {
                for (int j = 0; j < initialImage.GetLength(1); j++)
                {
                    result.SetPixel(i, j, Color.FromArgb((int)initialImage[i, j], (int)initialImage[i, j], (int)initialImage[i, j]));
                }
            }
            return result;
        }

        internal static double GetMin(double[,] initialImage)
        {
            var max = initialImage[0, 0];
            var temp = initialImage[0, 0];
            for (int i = 0; i < initialImage.GetLength(1); i++)
            {
                for (int j = 0; j < initialImage.GetLength(1); j++)
                {
                    temp = initialImage[i, j];
                    if (initialImage[i, j] <= max)
                    {
                        max = temp;
                    }
                }
            }
            return max;
        }

        internal static double GetMax(double[,] initialImage)
        {
            var min = initialImage[0,0];
            var temp = initialImage[0, 0];
            for (int i = 0; i < initialImage.GetLength(1); i++)
            {
                for (int j = 0; j < initialImage.GetLength(1); j++)
                {
                    temp = initialImage[i, j];
                    if (initialImage[i, j] >= min)
                    {
                        min = temp;
                    }
                }
            }
            return min;
        }

        internal static double[,] BitmapToMatrix(Bitmap initialImage)
        {
            var result = new double[initialImage.Width, initialImage.Height];
            for (int i = 0; i < initialImage.Width; i++)
            {
                for (int j = 0; j < initialImage.Height; j++)
                {
                    result[i, j] = initialImage.GetPixel(i, j).R;
                }
            }
            return result;
        }

        internal static void Rotate(ComplexImage f)
        {
            for (int i = 0; i < f.Height / 2; i++)
            {
                for (int j = 0; j < f.Width / 2; j++)
                {
                    var t = f.Data[i, j];
                    f.Data[i, j] = f.Data[i + f.Height / 2, j + f.Width / 2];
                    f.Data[i + f.Height / 2, j + f.Width / 2] = t;

                    t = f.Data[i + f.Height / 2, j];
                    f.Data[i + f.Height / 2, j] = f.Data[i, j + f.Width / 2];
                    f.Data[i, j + f.Width / 2] = t;
                }
            }
        }

        public static double GetMSE(Bitmap origin, Bitmap result)
        {
            var n = origin.Width;
            var m = origin.Height;

            var f = 0.0;

            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    f += Math.Pow((result.GetPixel(i, j).R - origin.GetPixel(i, j).R), 2);
                }
            }

            return f/(n*m);
        }

        public static double GetPSNR(Bitmap origin, Bitmap result)
        {
            var n = origin.Width;
            var m = origin.Height;

            var f = 0.0;

            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    f += Math.Pow((result.GetPixel(i, j).R - origin.GetPixel(i, j).R), 2) +
                        Math.Pow((result.GetPixel(i, j).B - origin.GetPixel(i, j).B), 2) +
                        Math.Pow((result.GetPixel(i, j).G - origin.GetPixel(i, j).G), 2);
                }
            }

            var s = 10 * Math.Log((255 * 255 / (f / (3 * n * m))), 10);

            return s;
        }


        public static Bitmap ConvertTo8bpp(this Bitmap oldbmp)
        {
            using (var ms = new MemoryStream())
            {
                oldbmp.Save(ms, ImageFormat.Gif);
                ms.Position = 0;
                return (Bitmap)System.Drawing.Image.FromStream(ms);
            }
        }

        public static Bitmap ToGray(Bitmap image)
        {
            var initialImage = image.ConvertTo8bpp();
            initialImage.ConvertColor8bppToGrayscale8bpp();
            return initialImage;
        }

        private static Complex w(int k, int N)
        {
            if (k % N == 0) return 1;
            double arg = -2 * Math.PI * k / N;
            return new Complex(Math.Cos(arg), Math.Sin(arg));
        }
        
        public static Complex[] FFT(Complex[] x)
        {
            Complex[] X;
            int N = x.Length;
            if (N == 2)
            {
                X = new Complex[2];
                X[0] = x[0] + x[1];
                X[1] = x[0] - x[1];
            }
            else
            {
                Complex[] x_even = new Complex[N / 2];
                Complex[] x_odd = new Complex[N / 2];
                for (int i = 0; i < N / 2; i++)
                {
                    x_even[i] = x[2 * i];
                    x_odd[i] = x[2 * i + 1];
                }
                Complex[] X_even = FFT(x_even);
                Complex[] X_odd = FFT(x_odd);
                X = new Complex[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + w(i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - w(i, N) * X_odd[i];
                }
            }
            return X;
        }

        public static Complex[,] FFT2(Complex[] X)
        {
            var trans = FFT(X);
            var res = new Complex[(int)Math.Sqrt(trans.Length), (int)Math.Sqrt(trans.Length)];
            for (int i = 0; i < res.GetLength(1); i++)
            {
                for (int j = 0; j < res.GetLength(0); j++)
                {
                    res[i, j] = trans[i * res.GetLength(0) + j];
                }
            }
            return res;
        }

        public static Complex[] BFT(Complex[] f)
        {
            Complex[] F = new Complex[f.Length];
            for (int i = 0; i < f.Length; i++)
            {
                F[i] = Complex.Conjugate(f[i]);
            }
            FT(F.Length, ref F);
            float scaling = (float)(1.0 / F.Length);
            for (int i = 0; i < F.Length; i++)
            {
                F[i] = scaling * Complex.Conjugate(F[i]);
            }

            return F;
        }

        public static Complex[,] BFT2(Complex[] X)
        {
            var trans = BFT(X);
            var res = new Complex[(int)Math.Sqrt(trans.Length), (int)Math.Sqrt(trans.Length)];
            for (int i = 0; i < res.GetLength(1); i++)
            {
                for (int j = 0; j < res.GetLength(0); j++)
                {
                    res[i, j] = trans[i * res.GetLength(0) + j];
                }
            }
            return res;
        }

        static void FT(float n, ref Complex[] f)
        {
            if (n > 1)
            {
                Complex[] g = new Complex[(int)n / 2];
                Complex[] u = new Complex[(int)n / 2];

                for (int i = 0; i < n / 2; i++)
                {
                    g[i] = f[i * 2];
                    u[i] = f[i * 2 + 1];
                }

                FT(n / 2, ref g);
                FT(n / 2, ref u);

                for (int i = 0; i < n / 2; i++)
                {
                    float a = i;
                    a = -2.0f * (float)Math.PI * a / n;
                    float cos = (float)Math.Cos(a);
                    float sin = (float)Math.Sin(a);
                    Complex c1 = new Complex(cos, sin);
                    c1 = Complex.Multiply(u[i], c1);
                    f[i] = Complex.Add(g[i], c1);

                    f[i + (int)n / 2] = Complex.Subtract(g[i], c1);
                }
            }
        }

        public static Bitmap GetScaledImage(Bitmap initialImage)
        {
            var scaledImage = new Bitmap(initialImage.Width * 2, initialImage.Height * 2);

            for (int i = 0; i < scaledImage.Height - 2; i += 2)
            {
                for (int j = 0; j < scaledImage.Width - 2; j += 2)
                {
                    scaledImage.SetPixel(i, j, initialImage.GetPixel(i / 2, j / 2));
                }
            }

            return scaledImage;
        }

        public static Complex[] ToVector(Complex[,] data)
        {
            var vec = new Complex[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                vec[i] = data[(i - i % data.GetLength(1)) / data.GetLength(1), i % data.GetLength(1)];
            }

            return vec;
        }
        public static ComplexImage GetComplexImageFromMatrix(Complex[,] core)
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

        public static ComplexImage GetComplexImageFromMatrix(double[,] core)
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


        public static Bitmap GetCoreImage(Bitmap initialImage, Filter filter)
        {
            var g = ComplexImage.FromBitmap(initialImage);
            var core = filter != Filter.Predict ? WienerFilter.GetCore(g, filter) : WienerPredictFilter.Core;
            var h = GetComplexImageFromMatrix(core);

            for (int i = 0; i < h.Height; i++)
            {
                for (int j = 0; j < h.Width; j++)
                {
                    h.Data[i, j] *= 255;
                }
            }

            return h.ToBitmap();
        }
        public static Complex GetSNR(Complex[,] data)
        {
            var average = GetAverage(data);
            var dispersion = GetDispersion(data, average);
            return GetAverage(data) / Complex.Sqrt(dispersion);
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

        public static double GetDispersion(Bitmap layer, double average)
        {
            var sum = 0.0;
            for (int i = 0; i < layer.Height; i++)
            {
                for (int j = 0; j < layer.Width; j++)
                {
                    sum += (layer.GetPixel(i, j).R - average) * (layer.GetPixel(i, j).R - average);
                }
            }

            return sum / (layer.Width * layer.Height);
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

        public static double GetAverage(Bitmap layer)
        {
            var sum = 0.0;
            for (int i = 0; i < layer.Height; i++)
            {
                for (int j = 0; j < layer.Width; j++)
                {
                    sum += layer.GetPixel(i, j).R;
                }
            }
            return sum / (layer.Width * layer.Height);
        }

        public static double GetCov(Bitmap x, Bitmap y, double aX, double aY)
        {
            var sum = 0.0;
            for (int i = 0; i < x.Height; i++)
            {
                for (int j = 0; j < x.Width; j++)
                {
                    sum += (x.GetPixel(i, j).R - aX)*(y.GetPixel(i,j).R - aY);
                }
            }
            return sum / (x.Width * x.Height);
        }

        public static double GetSSIM(Bitmap x, Bitmap y)
        {
            var averageX = GetAverage(x);
            var averageY = GetAverage(y);
            var dispersionX = GetDispersion(x, averageX);
            var dispersionY = GetDispersion(y, averageY);
            var cov = GetCov(x, y, averageX, averageY);
            var c1 = Math.Pow(0.01 * 255, 2);
            var c2 = Math.Pow(0.03 * 255, 2);
            return ((2 * averageX * averageY + c1) * (2 * cov + c2)) /
               ((averageX * averageX + averageY * averageY + c1) * (dispersionX + dispersionY + c2));
        }

        public enum Filter 
        { 
            Gauss,
            Sharpen, 
            MotionLeftToRight,
            MotionRightToLeft,
            Predict
        }
    }
}
