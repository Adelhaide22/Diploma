using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Extreme.Mathematics;

namespace ImageInterpolation.Filtering
{
	public class FourierTransform
	{
		public static Bitmap TransformImage(Bitmap initialImage)
		{
			var initialVectorR = ToVector(initialImage, "red");
			var initialVectorG = ToVector(initialImage, "green");
			var initialVectorB = ToVector(initialImage, "blue");

			var resultVectorR = new List<Complex<double>>();
			var resultVectorG = new List<Complex<double>>();
			var resultVectorB = new List<Complex<double>>();

			for (int k = 0; k < initialVectorR.Count - 1; k++)
			{
				Complex<double> temp = 0;
				for (int n = 0; n < initialVectorR.Count - 1; n++)
				{
					temp += initialVectorR[n] * (Math.Cos(-2 * Math.PI * n * k / initialVectorR.Count)
						+ Complex<double>.I * Math.Sin(-2 * Math.PI * n * k / initialVectorR.Count));
				}
				resultVectorR.Add(temp);
			}
			for (int k = 0; k < initialVectorG.Count - 1; k++)
			{
				Complex<double> temp = 0;
				for (int n = 0; n < initialVectorG.Count - 1; n++)
				{
					temp += initialVectorG[n] * (Math.Cos(-2 * Math.PI * n * k / initialVectorG.Count)
					+ Complex<double>.I * Math.Sin(-2 * Math.PI * n * k / initialVectorG.Count));
				}
				resultVectorG.Add(temp);
			}
			for (int k = 0; k < initialVectorG.Count - 1; k++)
			{
				Complex<double> temp = 0;
				for (int n = 0; n < initialVectorB.Count - 1; n++)
				{
					temp += initialVectorB[n] * (Math.Cos(-2 * Math.PI * k * n / initialVectorB.Count)
					+ Complex<double>.I * Math.Sin(-2 * Math.PI * k * n / initialVectorB.Count));
				}
				resultVectorB.Add(temp);
			}

			var resultImage = ToMatrix(resultVectorR, resultVectorG, resultVectorB);

			return resultImage;
		}

		public static double[,] TransformImage(double[,] f)
		{
			var height = f.GetLength(1);
			var F = new double[f.GetLength(0), f.GetLength(1)];

			for (int k = 0; k < f.Length; k++)
			{
				Complex<double> temp = 0;
				for (int n = 0; n < f.Length; n++)
				{
					temp += f[n/height, n*height] * (Math.Cos(-2 * Math.PI * n * k / f.Length)
						+ Complex<double>.I * Math.Sin(-2 * Math.PI * n * k / f.Length));
				}
				F[k / height, k * height] = temp.Re;
			}

			return F;
		}

		private static List<double> ToVector(Bitmap initialImage, string color)
		{
			var result = new List<double>();

			for (int i = 0; i < initialImage.Width; i++)
			{
				for (int j = 0; j < initialImage.Height; j++)
				{
					if (color == "red")
					{
						result.Add(initialImage.GetPixel(i, j).R);
					}
					if (color == "green")
					{
						result.Add(initialImage.GetPixel(i, j).G);
					}
					if (color == "blue")
					{
						result.Add(initialImage.GetPixel(i, j).B);
					}
				}			
			}

			return result;
		}

		private static Bitmap ToMatrix(List<Complex<double>> initialVectorR, List<Complex<double>> initialVectorG, List<Complex<double>> initialVectorB)
		{
			var resultR = new double[initialVectorR.Count, initialVectorR.Count];
			var resultG = new double[initialVectorG.Count, initialVectorG.Count];
			var resultB = new double[initialVectorB.Count, initialVectorB.Count];

			for (int i = 0; i < Math.Sqrt(initialVectorR.Count); i++)
			{
				for (int j = 0; j < Math.Sqrt(initialVectorR.Count); j++)
				{
					resultR[i, j] = initialVectorR[j].Magnitude;
				}
			}
			for (int i = 0; i < Math.Sqrt(initialVectorG.Count); i++)
			{
				for (int j = 0; j < Math.Sqrt(initialVectorG.Count); j++)
				{
					resultG[i, j] = initialVectorG[j].Magnitude;
				}
			}
			for (int i = 0; i < Math.Sqrt(initialVectorB.Count); i++)
			{
				for (int j = 0; j < Math.Sqrt(initialVectorB.Count); j++)
				{
					resultB[i, j] = initialVectorB[j].Magnitude;
				}
			}


			var resultImage = new Bitmap(resultR.Length, resultR.Length);

			for (int i = 0; i < resultImage.Width; i++)
			{
				for (int j = 0; j < resultImage.Height; j++)
				{
					resultImage.SetPixel(i, j, Color.FromArgb(Convert.ToInt32(resultR[i, j]), 
						Convert.ToInt32(resultG[i, j]), Convert.ToInt32(resultB[i, j])));
				}
			}

			return resultImage;
		}
	}
}

