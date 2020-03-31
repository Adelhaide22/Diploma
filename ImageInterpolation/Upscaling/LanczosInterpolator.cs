using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageInterpolation
{
    static class LanczosInterpolator
    {                       
        static double Scale = 1;
        
        static int Window = 3;

        static double[] CoordX;
        static double[] CoordY;
                
        public static Bitmap Resample(Bitmap Input)
        {
            var Extended = new Bitmap(Input.Width + Window * 2, Input.Height + Window * 2); //!!!206
            FillExtended(Input, Extended);

            var Output = new Bitmap(ToInt(Input.Width * Scale), ToInt(Input.Height * Scale));

            CoordX = new double[ToInt(Scale * Input.Width)];
            CoordY = new double[ToInt(Scale * Input.Height)];

            for (int i = 0; i < ToInt(Scale * Input.Width); i++)
            {
                CoordX[i] = i / Scale;
            }
            for (int j = 0; j < ToInt(Scale * Input.Height); j++)
            {
                CoordY[j] = j / Scale;
            }

            Filtering(Extended, Output);

            return Output;
        }

        public static void FillExtended(Bitmap Input, Bitmap Extended)
        {
            for (int i = 0; i < Input.Width; i++)
            {
                for (int j = 0; j < Input.Height; j++)
                {
                    Extended.SetPixel(i + Window, j + Window, Input.GetPixel(i, j));
                }
            }
            for (int i = 0; i < Extended.Width; i++)
            {
                for (int j = 0; j < Window; j++)
                {
                    Extended.SetPixel(i, j, Extended.GetPixel(i, Window));
                }
                for (int j = Extended.Height - Window; j < Extended.Height; j++)
                {
                    Extended.SetPixel(i, j, Extended.GetPixel(i, Extended.Height - Window - 1));
                }
            }
            for (int i = 0; i < Window; i++)
            {
                for (int j = 0; j < Extended.Height; j++)
                {
                    Extended.SetPixel(i, j, Extended.GetPixel(Window, j));
                }
            }
            for (int i = Extended.Width - Window; i < Extended.Width; i++)
            {
                for (int j = 0; j < Extended.Height; j++)
                {
                    Extended.SetPixel(i, j, Extended.GetPixel(Extended.Width - Window - 1, j));
                }
            }
        }

        public static void Filtering(Bitmap Extended, Bitmap Output)
        {
            for (int i = 0; i < Output.Width; i++)
            {
                for (int j = 0; j < Output.Height; j++)
                {
                    double ColR = Calc(Extended, CoordX[i], CoordY[j], "red");
                    double ColG = Calc(Extended, CoordX[i], CoordY[j], "green");
                    double ColB = Calc(Extended, CoordX[i], CoordY[j], "blue");

                    if (ColR < 0) ColR = 0;
                    if (ColG < 0) ColG = 0;
                    if (ColB < 0) ColB = 0;

                    if (ColR > 255) ColR = 255;
                    if (ColG > 255) ColG = 255;
                    if (ColB > 255) ColB = 255;
                    Console.WriteLine($"{i},{j}");
                    Output.SetPixel(i, j, Color.FromArgb(Convert.ToInt32(ColR), Convert.ToInt32(ColG), Convert.ToInt32(ColB)));
                }
            }
        }

        public static double Calc(Bitmap Extended, double x, double y, string color)
        {
            double x0 = x;
            double y0 = y;

            int u0 = Convert.ToInt32(Math.Floor(x0));
            int v0 = Convert.ToInt32(Math.Floor(y0));

            double q = 0;

            for (int j = 0; j < 2 * Window; j++)
            {
                int v = v0 + j - Window + 1;

                double p = 0;
                for (int i = 0; i < 2 * Window; i++)
                {
                    int u = u0 + i - Window + 1;

                    if (color == "red")
                    {
                        p += Extended.GetPixel(u + Window, v + Window).R * Core(x0 - u);
                    }
                    if (color == "blue")
                    {
                        p += Extended.GetPixel(u + Window, v + Window).B * Core(x0 - u);
                    }
                    if (color == "green")
                    {
                        p += Extended.GetPixel(u + Window, v + Window).G * Core(x0 - u);
                    }
                }
                q += p * Core(y0 - v);
            }
            return q;
        }

        static double Core(double t)
        {
            if (t < 0.0f) t = -t;
            if (t == 0.0f) return 1;
            if (t < Window) return SinC(t) * SinC(t / Window);
            else return 0;
        }

        static double SinC(double Arg)
        {
            return Convert.ToSingle(Math.Sin(Convert.ToDouble(Arg * Math.PI)) / Convert.ToDouble(Arg * Math.PI));
        }

        static int ToInt(double t)
        {
            return Convert.ToInt32(Math.Floor(t));
        }
    }
}

