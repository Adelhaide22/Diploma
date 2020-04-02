﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImageInterpolation.Filtering
{
    public static class WienerFilter
    {
        //public static Complex[,] GetGausianComplexImage(Size OutImageSize, int HalfWindow, float aSigma)
        //{
        //    Complex[,] lRes = null;

        //    int lMinHalf = Math.Min(OutImageSize.Width, OutImageSize.Height) / 2;
        //    HalfWindow = Math.Min(HalfWindow, lMinHalf);

        //    Complex[,] Gausian = new Complex[2 * HalfWindow + 15, 2 * HalfWindow + 1];
        //    int lHeight = 2 * HalfWindow + 1;
        //    int lWidth = 2 * HalfWindow + 1;
        //    float lCenterX = (float)HalfWindow;
        //    float lCenterY = (float)HalfWindow;

        //    float larg = -1 / (2 * aSigma * aSigma);
        //    float lSumm = 0;
        //    for (int ly = 0; ly < lHeight; ly++)
        //        for (int lx = 0; lx < lWidth; lx++)
        //        {
        //            float xdiff = (float)lx - lCenterX;
        //            float ydiff = (float)ly - lCenterY;
        //            float lVal = (float)(Math.Exp(larg * (xdiff * xdiff + ydiff * ydiff)));
        //            lSumm += lVal;
        //            Gausian[lx, ly] = new Complex(lVal, 0);
        //        }

        //    for (int ly = 0; ly < lHeight; ly++)
        //        for (int lx = 0; lx < lWidth; lx++)
        //        {
        //            Gausian[lx, ly] = new Complex(Gausian[lx, ly].re / lSumm, 0);
        //        }

        //    lRes = new Complex[OutImageSize.Width, OutImageSize.Height];
        //    int lNewShiftX = (OutImageSize.Width - lWidth) / 2;
        //    int lNewShiftY = (OutImageSize.Height - lHeight) / 2;
        //    for (int ly = 0; ly < lHeight; ly++)
        //        for (int lx = 0; lx < lWidth; lx++)
        //        {
        //            lRes[lx + lNewShiftX + 1, ly + lNewShiftY + 1] = Gausian[lx, ly];
        //        }

        //    return lRes;
        //}

        static double dispersionR, dispersionB, dispersionG, medianR, medianB, medianG;
        public static Bitmap Filter(Bitmap initialImage)
        {
            var g = new double[3][,];
            g[0] = new double[initialImage.Width, initialImage.Height];
            g[1] = new double[initialImage.Width, initialImage.Height];
            g[2] = new double[initialImage.Width, initialImage.Height];

            for (int i = 0; i < initialImage.Width; i++)
            {
                for (int j = 0; j < initialImage.Height; j++)
                {
                    g[0][i, j] = initialImage.GetPixel(i, j).R;
                    g[1][i, j] = initialImage.GetPixel(i, j).G;
                    g[2][i, j] = initialImage.GetPixel(i, j).B;
                }
            }

            var h = GetGaussianCore(g);







            var transformedImage = FourierTransform.TransformImage(initialImage);
            var resultImage = new Bitmap(initialImage.Width, initialImage.Height);

            medianR = GetMedian(transformedImage, "red");
            medianG = GetMedian(transformedImage, "green");
            medianB = GetMedian(transformedImage, "blue");

            dispersionR = GetDispersion(transformedImage, medianR, "red");
            dispersionG = GetDispersion(transformedImage, medianG, "green");
            dispersionB = GetDispersion(transformedImage, medianB, "blue");

            for (int i = 0; i < transformedImage.Width; i++)
            {
                for (int j = 0; j < transformedImage.Height; j++)
                {
                    var coreR = GetGaussianCore(transformedImage, i, j, "red");
                    var coreG = GetGaussianCore(transformedImage, i, j, "green");
                    var coreB = GetGaussianCore(transformedImage, i, j, "blue");

                    //var coreR = LanczosInterpolator.Calc(initialImage, i, j, "red");
                    //var coreG = LanczosInterpolator.Calc(initialImage, i, j, "green");
                    //var coreB = LanczosInterpolator.Calc(initialImage, i, j, "blue");

                    double ColR = GetWiener(coreR, transformedImage, i, j, "red");
                    double ColG = GetWiener(coreG, transformedImage, i, j, "green");
                    double ColB = GetWiener(coreB, transformedImage, i, j, "blue");



                    resultImage.SetPixel(i, j, Color.FromArgb(Convert.ToInt32(ColR), Convert.ToInt32(ColG), Convert.ToInt32(ColB)));
                    Console.WriteLine($"{i},{j}");
                }
            }

            return resultImage;
        }

        private static double[][,] GetGaussianCore(double[][,] g)
        {
            return g.AsParallel().Select(l =>
            {
                var dispersion = GetDispersion(l);
                var core = new double[l.GetLength(0), l.GetLength(1)];

                for (int i = 0; i < l.GetLength(0); i++)
                {
                    for (int j = 0; j < l.GetLength(1); j++)
                    {
                        core[i, j] = 1 / Math.Sqrt(2 * Math.PI * dispersion) * Math.Exp((i * i + j * j) / (2 * dispersion));
                    }
                }
                return core;
            }).ToArray();
        }

        private static double GetDispersion(double[,] layer)
        {
            var sum = 0.0;
            var average = GetAverage(layer);

            for (int i = 0; i < layer.GetLength(0); i++)
            {
                for (int j = 0; j < layer.GetLength(1); j++)
                {
                    sum += Math.Pow((layer[i, j] - average), 2);
                }
            }

            return sum / layer.Length;
        }

        private static double GetAverage(double[,] layer)
        {
            var sum = 0.0;
            for (int i = 0; i < layer.GetLength(0); i++)
            {
                for (int j = 0; j < layer.GetLength(1); j++)
                {
                    sum += layer[i, j];
                }
            }
            return sum / layer.Length;
        }





        private static double GetGaussianCore(Bitmap initialImage, int i, int j, string color)
        {
            if (color == "red")
            {
                return (1 / Math.Sqrt(2 * Math.PI * dispersionR)) * Math.Exp((i * i + j * j) / (2 * dispersionR));
            }
            if (color == "blue")
            {
                return (1 / Math.Sqrt(2 * Math.PI * dispersionB)) * Math.Exp((i * i + j * j) / (2 * dispersionB));
            }
            if (color == "green")
            {
                return (1 / Math.Sqrt(2 * Math.PI * dispersionG)) * Math.Exp((i * i + j * j) / (2 * dispersionG));
            }
            return 0;

            //return (1 / Math.Sqrt(2 * Math.PI * 400)) * Math.Exp((i * i + j * j) / (2 * 400));
        }

        private static double GetWiener(double core, Bitmap initialImage, int i, int j, string color)
        {
            if (color == "red")
            {
                return (1 / core) * ((core * core) / (core * core + GetSNR(color))) * initialImage.GetPixel(i, j).R;
            }
            if (color == "blue")
            {
                return (1 / core) * ((core * core) / (core * core + GetSNR(color))) * initialImage.GetPixel(i, j).B;
            }
            if (color == "green")
            {
                return (1 / core) * ((core * core) / (core * core + GetSNR(color))) * initialImage.GetPixel(i, j).G;
            }
            return 0;
        }

        private static double GetSNR(string color)
        {
            if (color == "red")
            {
                return medianR / Math.Sqrt(dispersionR);
            }
            if (color == "blue")
            {
                return medianB / Math.Sqrt(dispersionB);
            }
            if (color == "green")
            {
                return medianG / Math.Sqrt(dispersionG);
            }
            return 0;
        }

        private static double GetDispersion(Bitmap initialImage, double median, string color)
        {
            var dispersion = 0.0;

            for (int i = 0; i < initialImage.Height; i++)
            {
                for (int j = 0; j < initialImage.Width; j++)
                {
                    if (color == "red")
                    {
                        dispersion += Math.Pow((double)(initialImage.GetPixel(i, j).R - median), 2);
                    }
                    if (color == "blue")
                    {
                        dispersion += Math.Pow((double)(initialImage.GetPixel(i, j).B - median), 2);
                    }
                    if (color == "green")
                    {
                        dispersion += Math.Pow((double)(initialImage.GetPixel(i, j).G - median), 2);
                    }
                }
            }

            return dispersion / (initialImage.Height * initialImage.Width);
        }

        private static double GetMedian(Bitmap initialImage, string color)
        {
            var median = 0.0;
            for (int i = 0; i < initialImage.Height; i++)
            {
                for (int j = 0; j < initialImage.Width; j++)
                {
                    if (color == "red")
                    {
                        median += initialImage.GetPixel(i, j).R;
                    }
                    if (color == "blue")
                    {
                        median += initialImage.GetPixel(i, j).B;
                    }
                    if (color == "green")
                    {
                        median += initialImage.GetPixel(i, j).G;
                    }
                }
            }
            return median / (initialImage.Height * initialImage.Width);
        }
    }
}
 