using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageInterpolation
{
    static class SARTInterpolator
    {
        public static Bitmap Resample(Bitmap initialImage)
        {
            var k = 0;
            var lambda = 0;

            var A = new List<List<Color>>();
            var b = new List<Color>();
            var x = ToVector(initialImage);

            var N = 0;
            var M = 0;

            var xValues = new List<List<Color>>();

            while (!StopCondition())
            {
                for (int j = 0; j < N*N; j++)
                {
                    var sum = 0;

                    for (int i = 1; i < M; i++)
                    {
                        sum += A[i][j] / GetARowsSum(A, i) * (b[i] - A[i] * xValues[k - 1]);
                    }

                    List<Color> newXValue;
                    if (k == 0)
                    {
                        newXValue = x;
                    }
                    else
                    {
                        newXValue = xValues[k - 1] + lambda * (1 / GetAColsSum(A, j)) * sum;
                    }

                    xValues.Add(newXValue);
                }

                k++;
            }
        }

        private static Color GetARowsSum(List<List<Color>> A, int i)
        {
            var rowSumR = 0;
            var rowSumG = 0;
            var rowSumB = 0;
            var N = A.ToArray().GetLength(1);
            var M = A.ToArray().GetLength(0);
            
            for (int j = 0; j < N; j++)
            {
                rowSumR += A[i][j].R;
                rowSumG += A[i][j].G;
                rowSumB += A[i][j].B;
            }
            return Color.FromArgb(rowSumR, rowSumG, rowSumB);
        }

        private static Color GetAColsSum(List<List<Color>> A, int j)
        {
            var rowSumR = 0;
            var rowSumG = 0;
            var rowSumB = 0;
            var N = A.ToArray().GetLength(1);
            var M = A.ToArray().GetLength(0);            
           
            for (int i = 0; i < M; i++)
            {
                rowSumR += A[i][j].R;
                rowSumG += A[i][j].G;
                rowSumB += A[i][j].B;
            }
            return Color.FromArgb(rowSumR, rowSumG, rowSumB);
        }

        private static List<Color> ToVector(Bitmap initialImage)
        {
            var x = new List<Color>();

            for (int i = 0; i < initialImage.Height; i++)
            {
                for (int j = 0; j < initialImage.Width; j++)
                {
                    x.Add(initialImage.GetPixel(i, j));
                }
            }

            return x;
        }

        private static bool StopCondition()
        {
            throw new NotImplementedException();
        }
    }
}
