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
        public static Bitmap FilterMPI(Bitmap initialImage, ImageHelper.Filter filter, double alpha, double delta)
        {
            //int size = initialImage.Height;

            //// Будем хранить матрицу в векторе, состоящем из 
            //// векторов вещественных чисел
            //vector <vector<long double>> matrix;

            //// Матрица будет иметь размер (size) x (size + 1),
            //// c учетом столбца свободных членов    
            //matrix.resize(size);
            //for (int i = 0; i < size; i++)
            //{
            //    matrix[i].resize(size + 1);

            //    for (int j = 0; j < size + 1; j++)
            //    {
            //        cin >> matrix[i][j];
            //    }
            //}

            //// Считываем необходимую точность решения
            //double eps = delta;

            //// Введем вектор значений неизвестных на предыдущей итерации,
            //// размер которого равен числу строк в матрице, т.е. size,
            //// причем согласно методу изначально заполняем его нулями
            //vector < long double> previousVariableValues(size, 0.0);

            //// Будем выполнять итерационный процесс до тех пор, 
            //// пока не будет достигнута необходимая точность    
            //while (true)
            //{
            //    // Введем вектор значений неизвестных на текущем шаге       
            //    vector < long double> currentVariableValues(size);

            //    // Посчитаем значения неизвестных на текущей итерации
            //    // в соответствии с теоретическими формулами
            //    for (int i = 0; i < size; i++)
            //    {
            //        // Инициализируем i-ую неизвестную значением 
            //        // свободного члена i-ой строки матрицы
            //        currentVariableValues[i] = matrix[i][size];

            //        // Вычитаем сумму по всем отличным от i-ой неизвестным
            //        for (int j = 0; j < size; j++)
            //        {
            //            if (i != j)
            //            {
            //                currentVariableValues[i] -= matrix[i][j] * previousVariableValues[j];
            //            }
            //        }

            //        // Делим на коэффициент при i-ой неизвестной
            //        currentVariableValues[i] /= matrix[i][i];
            //    }

            //    // Посчитаем текущую погрешность относительно предыдущей итерации
            //    long double error = 0.0;

            //    for (int i = 0; i < size; i++)
            //    {
            //        error += abs(currentVariableValues[i] - previousVariableValues[i]);
            //    }

            //    // Если необходимая точность достигнута, то завершаем процесс
            //    if (error < eps)
            //    {
            //        break;
            //    }

            //    // Переходим к следующей итерации, так 
            //    // что текущие значения неизвестных 
            //    // становятся значениями на предыдущей итерации
            //    previousVariableValues = currentVariableValues;
            //}

            //// Выводим найденные значения неизвестных с 8 знаками точности
            //for (int i = 0; i < size; i++)
            //{
            //    printf("%.8llf ", previousVariableValues[i]);
            //}

            //return 0;
        }





        public static Bitmap FilterGauss(Bitmap initialImage, ImageHelper.Filter filter, double alpha, double delta)
        {
            return initialImage;
        }

        public static Bitmap FilterMatrix(Bitmap initialImage, ImageHelper.Filter filter, double alpha, double delta)
        {
            var n = initialImage.Width;

            var A = GetA(initialImage, delta);
            var E = GenerateEMatrix(n);
            var At = Matrix.Transpose(A);

            var Ea = E.Multiply(alpha);
            var AtA = m(At,A);
            var sum = Ea.Add(AtA);
            var reverseMatrix = sum.Inverse();

            var Wy = m(reverseMatrix, At);
            var b = ImageHelper.BitmapToMatrix(initialImage);
            var a = m(Wy,b);

            var norm = ImageHelper.Normalize(a);
            return ImageHelper.ToBitmap(norm);
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
