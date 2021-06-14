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
        public static Bitmap Filter(Bitmap initialImage, ImageHelper.Filter filter, double alpha, double delta)
        {
            var n = initialImage.Width;

            var A = GetA(initialImage, delta);
            var E = GenerateEMatrix(n);
            var At = Matrix.Transpose(A);

            var Ea = E.Multiply(alpha);
            //var AtA2 = At.Multiply(A);
            var AtA = m(At,A);
            var sum = Ea.Add(AtA);
            var reverseMatrix = sum.Inverse();

           //var Wy2 = reverseMatrix.Multiply(At);
            var Wy = m(reverseMatrix, At);
            var b = ImageHelper.BitmapToMatrix(initialImage);
            //var a2 = Wy.Multiply(b);
            var a = m(Wy,b);

            var norm = ImageHelper.Normalize(a);
            return ImageHelper.ToBitmap(norm);
            //return a.ToBitmap();
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

        private static double[,] GetA(Bitmap initialImage, double delta)
        {
            var n = initialImage.Width;
            var A = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = GetH(i, j, delta);
                }
            }
            return A;
        }

        private static double GetH(int x, int ksi, double delta)
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
