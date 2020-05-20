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
        public static double[,] Core;

        public static Bitmap Filter(Bitmap initialImage, Bitmap broken)
        {
            var g = ComplexImage.FromBitmap(broken);
            var G = ImageHelper.GetComplexImageFromMatrix(ImageHelper.FFT2(ImageHelper.ToVector(g.Data)));
            var snr = ImageHelper.GetSNR(g.Data);

            var filters = new ImageHelper.Filter[]
            {
                ImageHelper.Filter.Gauss,                
                ImageHelper.Filter.MotionLeftToRight,
                ImageHelper.Filter.MotionRightToLeft,
            };

            var bestImages = new Bitmap[filters.Length];
            var cores = new List<double[,]>();

            for (int i = 0; i < filters.Length; i++)
            {
                var previousf = broken;
                var nextf = broken;

                var prevQuality = 0.0;
                var nextQuality = 0.0;
                var k = 0;
                var matrixSize = 1;
                var eps = 0.1;
                var core = new double[matrixSize, matrixSize];

                do
                {
                    matrixSize += 2;
                    snr = snrs[matrixSize];

                    previousf = nextf;

                    core = GetCore(g, filters[i], matrixSize);

                    var h = ImageHelper.GetComplexImageFromMatrix(core);
                    var H = ImageHelper.GetComplexImageFromMatrix(ImageHelper.FFT2(ImageHelper.ToVector(h.Data)));

                    var F = WienerFilter.GetF(H, G, snr);
                    var f = ImageHelper.GetComplexImageFromMatrix(ImageHelper.BFT2(ImageHelper.ToVector(F.Data)));
                    ImageHelper.Rotate(f);

                    nextf = f.ToBitmap();

                    prevQuality = ImageHelper.GetPSNR(initialImage, previousf);
                    nextQuality = ImageHelper.GetPSNR(initialImage, nextf);                    

                    //Console.WriteLine($"{nextQuality} {filters[i]} {matrixSize}");
                    k++;
                } while (nextQuality - prevQuality > eps || k < 2);

                bestImages[i] = previousf;
                cores.Add(core);
            }

            var psnrs = new List<double>();

            for (int i = 0; i < bestImages.Length; i++)
            {
                psnrs.Add(ImageHelper.GetPSNR(initialImage, bestImages[i]));
            }

            var bestIndex = psnrs.IndexOf(psnrs.Max());
            Core = cores[bestIndex];
            return bestImages[bestIndex];
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
                    SharpenFilter.SharpPower = matrixSize;
                    matrix = SharpenFilter.GetCore();
                    matrixSize = SharpenFilter.SharpSize;
                    break;
                case ImageHelper.Filter.MotionLeftToRight:
                    MotionFilter.MotionSize = matrixSize;
                    MotionFilter.Direction = Direction.LeftToRight;
                    matrix = MotionFilter.GetCore();
                    break;
                case ImageHelper.Filter.MotionRightToLeft:
                    MotionFilter.MotionSize = matrixSize;
                    MotionFilter.Direction = Direction.RightToLeft;
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

        private static double[] snrs = new double[]
        {
            1,
            1,
            0.7,
            0.7,
            0.05,
            0.05,
            0.028,
            0.028,
            0.022,
            0.022,
            0.015,
            0.015,
            0.0132,
            0.0132,
            0.0115,
            0.0115,
            0.009,
            0.009,
            0.0075,
            0.0075,
            0.006,
            0.006,
            0.0055,
            0.0055,
            0.005,
            0.005,
            0.004,
            0.004,
            0.0035,
            0.0035,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.0017,
            0.0017,
            0.0015,
            0.0015,
            0.0013,
            0.0013,
            0.001,
            0.001,
            0.008,
            0.008,
            0.006,
            0.006,
            0.005,
            0.005,
            0.004,
            0.004,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,



            0.005,
            0.005,
            0.004,
            0.004,
            0.0035,
            0.0035,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.0017,
            0.0017,
            0.0015,
            0.0015,
            0.0013,
            0.0013,
            0.001,
            0.001,
            0.008,
            0.008,
            0.006,
            0.006,
            0.005,
            0.005,
            0.004,
            0.004,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.005,
            0.005,
            0.004,
            0.004,
            0.0035,
            0.0035,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.0017,
            0.0017,
            0.0015,
            0.0015,
            0.0013,
            0.0013,
            0.001,
            0.001,
            0.008,
            0.008,
            0.006,
            0.006,
            0.005,
            0.005,
            0.004,
            0.004,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.005,
            0.005,
            0.004,
            0.004,
            0.0035,
            0.0035,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.0017,
            0.0017,
            0.0015,
            0.0015,
            0.0013,
            0.0013,
            0.001,
            0.001,
            0.008,
            0.008,
            0.006,
            0.006,
            0.005,
            0.005,
            0.004,
            0.004,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.005,
            0.005,
            0.004,
            0.004,
            0.0035,
            0.0035,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.0017,
            0.0017,
            0.0015,
            0.0015,
            0.0013,
            0.0013,
            0.001,
            0.001,
            0.008,
            0.008,
            0.006,
            0.006,
            0.005,
            0.005,
            0.004,
            0.004,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.005,
            0.005,
            0.004,
            0.004,
            0.0035,
            0.0035,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.0017,
            0.0017,
            0.0015,
            0.0015,
            0.0013,
            0.0013,
            0.001,
            0.001,
            0.008,
            0.008,
            0.006,
            0.006,
            0.005,
            0.005,
            0.004,
            0.004,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.005,
            0.005,
            0.004,
            0.004,
            0.0035,
            0.0035,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.0017,
            0.0017,
            0.0015,
            0.0015,
            0.0013,
            0.0013,
            0.001,
            0.001,
            0.008,
            0.008,
            0.006,
            0.006,
            0.005,
            0.005,
            0.004,
            0.004,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.005,
            0.005,
            0.004,
            0.004,
            0.0035,
            0.0035,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.0017,
            0.0017,
            0.0015,
            0.0015,
            0.0013,
            0.0013,
            0.001,
            0.001,
            0.008,
            0.008,
            0.006,
            0.006,
            0.005,
            0.005,
            0.004,
            0.004,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.005,
            0.005,
            0.004,
            0.004,
            0.0035,
            0.0035,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.0017,
            0.0017,
            0.0015,
            0.0015,
            0.0013,
            0.0013,
            0.001,
            0.001,
            0.008,
            0.008,
            0.006,
            0.006,
            0.005,
            0.005,
            0.004,
            0.004,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.005,
            0.005,
            0.004,
            0.004,
            0.0035,
            0.0035,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002,
            0.0017,
            0.0017,
            0.0015,
            0.0015,
            0.0013,
            0.0013,
            0.001,
            0.001,
            0.008,
            0.008,
            0.006,
            0.006,
            0.005,
            0.005,
            0.004,
            0.004,
            0.003,
            0.003,
            0.0025,
            0.0025,
            0.002,
            0.002


        };
    }
}
