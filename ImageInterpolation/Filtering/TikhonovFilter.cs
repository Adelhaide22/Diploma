using Accord.Imaging;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageInterpolation.Filtering
{
    class TikhonovFilter
    {
        private static double alpha = 0.0001;
        private static double delta = 20;

        public static Bitmap Filter(Bitmap initialImage, ImageHelper.Filter filter)
        {
            var n = initialImage.Width;

            var A = GetA(initialImage);
            var E = GenerateEMatrix(n);
            var At = Matrix.Transpose(A);

            var Ea = E.Multiply(alpha);
            var AtA2 = At.Multiply(A);
            var AtA = m(At,A);
            var sum = Ea.Add(AtA);
            var reverseMatrix = sum.Inverse();

            var Wy2 = reverseMatrix.Multiply(At);
            var Wy = m(reverseMatrix, At);
            var b = ImageHelper.BitmapToMatrix(initialImage);
            var a2 = Wy.Multiply(b);
            var a = m(Wy,b);

            return a.ToBitmap();
        }

        private static double[,] GenerateEMatrix(int n)
        {
            var e = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                e[i, i] = 1;
            }
            return e;
        }

        private static double[,] GetA(Bitmap initialImage)
        {
            var n = initialImage.Width;
            var A = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = GetH(i, j);
                }
            }
            return A;
        }

        private static double GetH(int x, int ksi)
        {
            return x- ksi >= 0 && (x - ksi) <= delta
                ? 1 / delta
                : 0;
        }

        private static double[,] m(double[,] a, double[,] b)
        {
            var c = new double[a.GetLength(0), a.GetLength(0)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(0); j++)
                {
                    for (int k = 0; k < a.GetLength(0); k++)
                    {
                        c[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return c;
        }
    }
}
