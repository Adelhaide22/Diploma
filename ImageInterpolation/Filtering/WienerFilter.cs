using Accord.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;

namespace ImageInterpolation.Filtering
{
    public static class WienerFilter
    {
        public static Bitmap Filter(Bitmap initialImage)
        {
            for (int i = 0; i < initialImage.Height; i++)
            {
                for (int j = 0; j < initialImage.Width; j++)
                {
                    System.Console.WriteLine(initialImage.GetPixel(i,j).R);
                }
            }

            var g = ComplexImage.FromBitmap(initialImage);
            var h = GetComplexImageFromMatrix(GetGaussianCore(g));

            var average = GetAverage(g.Data);
            var dispersion = GetDispersion(g.Data, average);

            var G = GetComplexImageFromMatrix(ComplexImageHelper.fft2(ToVector(g.Data)));
            var H = GetComplexImageFromMatrix(ComplexImageHelper.fft2(ToVector(h.Data)));
            
            var F = GetF(H, G, average, dispersion);

            var f = GetComplexImageFromMatrix(ComplexImageHelper.bft2(ToVector(F.Data)));
            return f.ToBitmap();





            //var g = new double[3][,];
            //g[0] = new double[initialImage.Width, initialImage.Height];
            //g[1] = new double[initialImage.Width, initialImage.Height];
            //g[2] = new double[initialImage.Width, initialImage.Height];

            //for (int i = 0; i < initialImage.Width; i++)
            //{
            //    for (int j = 0; j < initialImage.Height; j++)
            //    {
            //        g[0][i, j] = initialImage.GetPixel(i, j).R;
            //        g[1][i, j] = initialImage.GetPixel(i, j).G;
            //        g[2][i, j] = initialImage.GetPixel(i, j).B;
            //    }
            //}

            //var f = g.AsParallel().Select(gi =>
            //{
            //    var average = GetAverage(gi);
            //    var dispersion = GetDispersion(gi, average);
            //    var h = GetGaussianCore(gi, 16);

            //    var G = FourierTransform.TransformForward(gi);
            //    var H = FourierTransform.TransformForward(h);

            //    var F = GetF(H, G, average, dispersion);
            //    var fi = FourierTransform.TransformBackward(F);
            //    return fi;
            //}).ToArray();

            //var resultImage = new Bitmap(initialImage.Width, initialImage.Height);

            //for (int i = 0; i < initialImage.Width; i++)
            //{
            //    for (int j = 0; j < initialImage.Height; j++)
            //    {
            //        resultImage.SetPixel(i, j, Color.FromArgb((int)f[0][i, j], (int)f[1][i, j], (int)f[2][i, j]));
            //    }
            //}

            //return resultImage;
        }

        private static Complex[] ToVector(Complex[,] data)
        {
            var vec = new Complex[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                vec[i] = data[(i - i%data.GetLength(0)) / data.GetLength(0), i % data.GetLength(0)];
            }

            return vec;
        }
        private static ComplexImage GetComplexImageFromMatrix(Complex[,] core)
        {
            var bitmap = new Bitmap(core.GetLength(0), core.GetLength(1));

            var bitmap8bpp = bitmap.ConvertTo8bpp();
            bitmap8bpp.ConvertColor8bppToGrayscale8bpp();

            var complexImage = ComplexImage.FromBitmap(bitmap8bpp);

            for (int i = 0; i < complexImage.Height; i++)
            {
                for (int j = 0; j < complexImage.Width; j++)
                {
                    complexImage.Data[i, j] = core[i, j];
                }
            }

            return complexImage;
        }

        private static ComplexImage GetComplexImageFromMatrix(double[,] core)
        {
            var bitmap = new Bitmap(core.GetLength(0), core.GetLength(1));

            var bitmap8bpp = bitmap.ConvertTo8bpp();
            bitmap8bpp.ConvertColor8bppToGrayscale8bpp();

            var complexImage = ComplexImage.FromBitmap(bitmap8bpp);

            for (int i = 0; i < complexImage.Height; i++)
            {
                for (int j = 0; j < complexImage.Width; j++)
                {
                    complexImage.Data[i, j] = core[i, j];
                }
            }

            return complexImage;
        }

        private static ComplexImage GetF(ComplexImage H, ComplexImage G, Complex average, Complex dispersion)
        {
            var snr = GetSNR(average, dispersion);
            snr = 0.2;

            var bitmap = new Bitmap(G.Width, G.Height);
            var bitmap8bpp = bitmap.ConvertTo8bpp();
            bitmap8bpp.ConvertColor8bppToGrayscale8bpp();

            var complexImage = ComplexImage.FromBitmap(bitmap8bpp);

            for (int i = 0; i < G.Height; i++)
            {
                for (int j = 0; j < G.Width; j++)
                {
                    complexImage.Data[i, j] = (1 / H.Data[i, j]) * ((H.Data[i, j] * H.Data[i, j]) / (H.Data[i, j] * H.Data[i, j] + snr)) 
                        * G.Data[i, j];
                }
            }
            return complexImage;
        }

        private static Complex GetSNR(Complex average, Complex dispersion)
        {
            return average / Complex.Sqrt(dispersion);
        }

        private static double[,] GetGaussianCore(ComplexImage g)
        {
            var matrixSize = 9;
            var matrix = GaussianFilter.GetCore();

            var result = new double[g.Width, g.Height];            

            for (int l = 0; l < g.Width/2 - matrixSize/2 - 1; l++)
            {
                for (int k = 0; k < g.Height/2 - matrixSize/2 - 1; k++)
                {
                    result[l, k] = 0;
                }
            }

            for (int l = g.Width / 2 + matrixSize / 2; l < g.Width; l++)
            {
                for (int k = g.Height / 2 + matrixSize / 2; k < g.Height; k++)
                {
                    result[l, k] = 0;
                }
            }

            for (int l = g.Width / 2 - matrixSize / 2 - 1; l < g.Width / 2 + matrixSize / 2; l++)
            {
                for (int k = g.Height / 2 - matrixSize / 2 - 1; k < g.Height / 2 + matrixSize / 2; k++)
                {
                    result[l, k] = matrix[l - (g.Width / 2 - matrixSize / 2 - 1), k - (g.Height / 2 - matrixSize / 2 - 1)];
                }
            }

            return result;
        }

        private static Complex GetDispersion(Complex[,] layer, Complex average)
        {
            var sum = new Complex();
            for (int i = 0; i < layer.GetLength(0); i++)
            {
                for (int j = 0; j < layer.GetLength(1); j++)
                {
                    sum += (layer[i, j] - average) * (layer[i, j] - average);
                }
            }

            return sum / layer.Length;
        }

        private static Complex GetAverage(Complex[,] layer)
        {
            var sum = new Complex();
            for (int i = 0; i < layer.GetLength(0); i++)
            {
                for (int j = 0; j < layer.GetLength(1); j++)
                {
                    sum += layer[i, j];
                }
            }
            return sum / layer.Length;
        }

        private static Bitmap ConvertTo8bpp(this Bitmap oldbmp)
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
    }
}
 