using Accord.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImageInterpolation.Filtering
{
    class ComplexImageHelper
    {
        public enum Direction
        {
            /// <summary>
            ///   Forward direction of Fourier transformation.
            /// </summary>
            /// 
            Forward = 1,

            /// <summary>
            ///   Backward direction of Fourier transformation.
            /// </summary>
            /// 
            Backward = -1
        };
        
        public ComplexImage ForwardFourierTransform(ComplexImage image)
        {
            if (!image.FourierTransformed)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        if (((x + y) & 0x1) != 0)
                            image.Data[y, x] *= -1;
                    }
                }

                DFT2(image.Data, Direction.Forward);
            }

            return image;
        }

        /// <summary>
        /// Applies backward fast Fourier transformation to the complex image.
        /// </summary>
        /// 
        public ComplexImage BackwardFourierTransform(ComplexImage image)
        {
            if (image.FourierTransformed)
            {
                DFT2(image.Data, Direction.Backward);

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        if (((x + y) & 0x1) != 0)
                            image.Data[y, x] *= -1;
                    }
                }
            }

            return image;
        }

        public static void DFT2(Complex[,] data, Direction direction)
        {
            int n = data.GetLength(0);	// rows
            int m = data.GetLength(1);	// columns
            double arg, cos, sin;
            var dst = new Complex[System.Math.Max(n, m)];

            // process rows
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < dst.Length; j++)
                {
                    dst[j] = Complex.Zero;

                    arg = -(int)direction * 2.0 * System.Math.PI * (double)j / (double)m;

                    // sum source elements
                    for (int k = 0; k < m; k++)
                    {
                        cos = System.Math.Cos(k * arg);
                        sin = System.Math.Sin(k * arg);

                        double re = data[i, k].Real * cos - data[i, k].Imaginary * sin;
                        double im = data[i, k].Real * sin + data[i, k].Imaginary * cos;

                        dst[j] += new Complex(re, im);
                    }
                }

                // copy elements
                if (direction == Direction.Forward)
                {
                    // devide also for forward transform
                    for (int j = 0; j < dst.Length; j++)
                        data[i, j] = dst[j] / m;
                }
                else
                {
                    for (int j = 0; j < dst.Length; j++)
                        data[i, j] = dst[j];
                }
            }

            // process columns
            for (int j = 0; j < m; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    dst[i] = Complex.Zero;

                    arg = -(int)direction * 2.0 * System.Math.PI * (double)i / (double)n;

                    // sum source elements
                    for (int k = 0; k < n; k++)
                    {
                        cos = System.Math.Cos(k * arg);
                        sin = System.Math.Sin(k * arg);

                        double re = data[k, j].Real * cos - data[k, j].Imaginary * sin;
                        double im = data[k, j].Real * sin + data[k, j].Imaginary * cos;

                        dst[i] += new Complex(re, im);
                    }
                }

                // copy elements
                if (direction == Direction.Forward)
                {
                    // devide also for forward transform
                    for (int i = 0; i < dst.Length; i++)
                        data[i, j] = dst[i] / n;
                }
                else
                {
                    for (int i = 0; i < dst.Length; i++)
                        data[i, j] = dst[i];
                }
            }
        }
    }
}
