using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Extreme.Mathematics;
using Accord.Imaging;

namespace ImageInterpolation.Filtering
{
	public class FourierTransform
	{	
		public static Complex[,] TransformForward(double[,] f)
		{
			var height = f.GetLength(1);
			var F = new Complex[f.GetLength(0), f.GetLength(1)];

			for (int k = 0; k < f.Length; k++)
			{
				Complex temp = 0;
				for (int n = 0; n < f.Length; n++)
				{
					temp += f[n/height, n%height] * (Math.Cos(2 * Math.PI * n * k / f.Length)
						- /*Complex<double>.I * */Math.Sin(2 * Math.PI * n * k / f.Length));
				}
				F[k / height, k % height] = temp;
			}

			return F;
		}

		public static double[,] TransformBackward(Complex[,] F)
		{
			var height = F.GetLength(1);
			var f = new double[F.GetLength(0), F.GetLength(1)];

			for (int k = 0; k < F.Length; k++)
			{
				Complex temp = 0;
				for (int n = 0; n < F.Length; n++)
				{
					temp += (F[n / height, n % height] * (Math.Cos(2 * Math.PI * n * k / F.Length)
						+ /*Complex<double>.I **/ Math.Sin(2 * Math.PI * n * k / F.Length)))/F.Length;
				}
				f[k / height, k % height] = temp.Real;
			}

			//var result = new ComplexImage();
			return f;
		}
	}
}

