using Accord.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImageInterpolation.Filtering
{
    class WienerPredictFilter
    {
        public static Bitmap Filter(Bitmap initialImage)
        {
            var previousf = initialImage;
            var nextf = initialImage;

            var prevQuality = 0.0; 
            var nextQuality = 0.0;
            var k = 0;
            var epsMax = 1;
            var epsMin = 0.5;

            var matrixSize = 1;
            var filter = ImageHelper.Filter.Gauss;

            var g = ComplexImage.FromBitmap(initialImage);
            var G = ImageHelper.GetComplexImageFromMatrix(ImageHelper.FFT2(ImageHelper.ToVector(g.Data)));
            var snr = WienerFilter.GetSNR(g.Data);

            do
            {
                if (nextQuality - prevQuality < epsMin && k > 1)
                {
                    if (filter == ImageHelper.Filter.Motion)
                    {
                        return previousf;
                    }
                    filter++;
                    matrixSize = 3;
                }
                else
                {
                    matrixSize += 2;
                }

                previousf = nextf;

                var core = GetCore(g, filter, matrixSize);

                var h = ImageHelper.GetComplexImageFromMatrix(core);
                var H = ImageHelper.GetComplexImageFromMatrix(ImageHelper.FFT2(ImageHelper.ToVector(h.Data)));

                var F = WienerFilter.GetF(H, G, 0.015);
                var f = ImageHelper.GetComplexImageFromMatrix(ImageHelper.BFT2(ImageHelper.ToVector(F.Data)));
                ImageHelper.Rotate(f);

                nextf = f.ToBitmap();

                if (k > 0)
                {
                    prevQuality = ImageHelper.GetMSE(initialImage, previousf);
                    nextQuality = ImageHelper.GetMSE(initialImage, nextf);
                }
                Console.WriteLine($"{prevQuality} {filter} {matrixSize}");
                k++;
            } while (nextQuality - prevQuality > epsMax || nextQuality - prevQuality < epsMin || k < 2);
            

            return nextf;
        }

        public static double[,] GetCore(ComplexImage g, ImageHelper.Filter filter, int matrixSize)
        {
            var matrix = new double[matrixSize, matrixSize];

            switch (filter)
            {
                case ImageHelper.Filter.Gauss:
                    GaussianFilter.BlurSize = matrixSize;
                    matrix = GaussianFilter.GetCore();
                    break;
                case ImageHelper.Filter.Sharpen:
                    SharpenFilter.SharpSize = matrixSize;
                    matrix = SharpenFilter.GetCore();
                    break;
                case ImageHelper.Filter.Motion:
                    MotionFilter.MotionSize = matrixSize;
                    matrix = MotionFilter.GetCore();
                    break;
                default:
                    break;
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
    }
}
