using Accord.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImageInterpolation.Filtering
{
    static class ComplexImageHelper
    {
        private static Complex w(int k, int N)
        {
            if (k % N == 0) return 1;
            double arg = -2 * Math.PI * k / N;
            return new Complex(Math.Cos(arg), Math.Sin(arg));
        }
        /// <summary>
        /// Возвращает спектр сигнала
        /// </summary>
        /// <param name="x">Массив значений сигнала. Количество значений должно быть степенью 2</param>
        /// <returns>Массив со значениями спектра сигнала</returns>
        public static Complex[] fft(Complex[] x)
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
                Complex[] X_even = fft(x_even);
                Complex[] X_odd = fft(x_odd);
                X = new Complex[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + w(i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - w(i, N) * X_odd[i];
                }
            }
            return X;
        }
        /// <summary>
        /// Центровка массива значений полученных в fft (спектральная составляющая при нулевой частоте будет в центре массива)
        /// </summary>
        /// <param name="X">Массив значений полученный в fft</param>
        /// <returns></returns>
        public static Complex[] nfft(Complex[] X)
        {
            int N = X.Length;
            Complex[] X_n = new Complex[N];
            for (int i = 0; i < N / 2; i++)
            {
                X_n[i] = X[N / 2 + i];
                X_n[N / 2 + i] = X[i];
            }
            return X_n;
        }

        public static Complex[,] fft2(Complex[] X)
        {
            var trans = fft(X);
            //toWolframAlphaDefinition(ref trans);
            var res = new Complex[(int)Math.Sqrt(trans.Length), (int)Math.Sqrt(trans.Length)];
            for (int i = 0; i < res.GetLength(0); i++)
            {
                for (int j = 0; j < res.GetLength(1); j++)
                {
                    res[i, j] = trans[i * res.GetLength(1) + j];
                }
            }
            return res;
        }

        public static Complex[] bft(Complex[] f)
        {
            Complex[] F = new Complex[f.Length];
            for (int i = 0; i < f.Length; i++)
            {
                F[i] = Complex.Conjugate(f[i]);
            }
            ft(F.Length, ref F);
            float scaling = (float)(1.0 / F.Length);
            for (int i = 0; i < F.Length; i++)
            {
                F[i] = scaling * Complex.Conjugate(F[i]);
            }

            return F;
        }

        public static Complex[,] bft2(Complex[] X)
        {
            var trans = bft(X);
            //toWolframAlphaDefinition(ref trans);
            var res = new Complex[(int)Math.Sqrt(trans.Length), (int)Math.Sqrt(trans.Length)];
            for (int i = 0; i < res.GetLength(0); i++)
            {
                for (int j = 0; j < res.GetLength(1); j++)
                {
                    res[i, j] = trans[i * res.GetLength(1) + j];
                }
            }
            return res;
        }

        static void ft(float n, ref Complex[] f)
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

                ft(n / 2, ref g);
                ft(n / 2, ref u);

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

        static void toWolframAlphaDefinition(ref Complex[] f)
        {
            float scaling = (float)(1.0 / Math.Sqrt(f.Length));
            for (int i = 0; i < f.Length; i++)
            {
                f[i] = scaling * Complex.Conjugate(f[i]);
            }
        }

    }
}
