﻿using Accord.Imaging;
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
    static class ImageHelper
    {
        public static double GetQuality(Bitmap origin, Bitmap result)
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
    }
}