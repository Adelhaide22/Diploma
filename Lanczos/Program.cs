using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lanczos
{    
    class Program
    {
        //коэффициент масштабирования изображения
        static float Scale;

        static float Col;

        //изображения
        static Bitmap Input;
        static Bitmap Extended;
        static Bitmap Output;

        //размер окна
        static int Window;

        //массивы вещественных координат, по осям X и Y соответственно
        static float[] CoordX;
        static float[] CoordY;
        static float[] Temp = { 255.0f, 155.0f, 55.0f, 25.0f, 0.0f, 88.0f, 100.0f, 188.0f, 200.0f, 30.0f };

        
        static float SinC(float Arg)
        {
            return Convert.ToSingle(Math.Sin(Convert.ToDouble(Arg * Math.PI)) / Convert.ToDouble(Arg * Math.PI));
        }

        static float Core(float t)
        {
            if (t < 0.0f) t = -t;
            if (t == 0.0f) return 1;
            if (t < Window) return SinC(t) * SinC(t / Window);
            else return 0;
        }


        static float Calc(float x, float y)
        {
            float x0 = x;
            float y0 = y;

            int u0 = Convert.ToInt32(Math.Floor(x0));
            int v0 = Convert.ToInt32(Math.Floor(y0));

            float q = 0;

            for (int j = 0; j <= 2 * Window - 1; j++)
            {
                int v = v0 + j - Window + 1;
                {
                    float p = 0;
                    for (int i = 0; i <= 2 * Window - 1; i++)
                    {

                        int u = u0 + i - Window + 1;
                        {
                            p = p + Extended.GetPixel(u + Window, v + Window).R * Core(x0 - u);
                            //Console.WriteLine(x0 - u);

                        }
                    }
                    q = q + p * Core(y0 - v);
                    //Console.WriteLine(" ");
                }

            }
            return q;
        }

        static void Filtering()
        {
            for (int i = 0; i < Output.Width; i++)
            {
                for (int j = 0; j < Output.Height; j++)
                {

                    Col = Calc(CoordX[i], CoordY[j]);
                    if (Col < 0)
                    {
                        Output.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                    }
                    else
                    {
                        if (Col > 255)
                        {
                            Output.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                        }
                        else
                        {
                            Output.SetPixel(i, j, Color.FromArgb(Convert.ToInt32(Col), Convert.ToInt32(Col), Convert.ToInt32(Col)));
                        }
                    }
                }
            }
        }

        static int Int(double t)
        {
            return Convert.ToInt32(Math.Floor(t));
        }

        static void Main(string[] args)
        {
            Scale = 5.5f;
            Window = 3;
            Col = 0;

            //исходное изображение
            Input = new Bitmap("real.bmp");

            //исходное изображение, обрамленное рамкой, по ширине равной размеру Window
            Extended = new Bitmap(Input.Width + Window * 2, Input.Height + Window * 2);

            //выходное изображение, измененного размера
            Output = new Bitmap(Int(Input.Width * Scale), Int(Input.Height * Scale));

            for (int i = 0; i < Input.Width; i++)
            {
                for (int j = 0; j < Input.Height; j++)
                {
                    Extended.SetPixel(i + Window, j + Window, Input.GetPixel(i, j));
                }
            }

            //верхнее поле
            for (int i = 0; i < Extended.Width; i++)
            {
                for (int j = 0; j < Window; j++)
                {
                    Extended.SetPixel(i, j, Extended.GetPixel(i, Window));
                }
            }
            //нижнее поле
            for (int i = 0; i < Extended.Width; i++)
            {
                for (int j = Extended.Height - Window; j < Extended.Height; j++)
                {
                    Extended.SetPixel(i, j, Extended.GetPixel(i, Extended.Height - Window - 1));
                }
            }
            //левое поле
            for (int i = 0; i < Window; i++)
            {
                for (int j = 0; j < Extended.Height; j++)
                {
                    Extended.SetPixel(i, j, Extended.GetPixel(Window, j));
                }
            }
            //правое поле
            for (int i = Extended.Width - Window; i < Extended.Width; i++)
            {
                for (int j = 0; j < Extended.Height; j++)
                {
                    Extended.SetPixel(i, j, Extended.GetPixel(Extended.Width - Window - 1, j));
                }
            }

            if (Scale >= 1)
            {
                CoordX = new float[Int(Scale * Input.Width)];
                CoordY = new float[Int(Scale * Input.Height)];
                for (int i = 0; i < Int(Scale * Input.Width); i++)
                {
                    CoordX[i] = i / Scale;
                }
                for (int j = 0; j < Int(Scale * Input.Height); j++)
                {
                    CoordY[j] = j / Scale;
                }
            }
            
            Filtering();

            Output.Save("output.bmp", ImageFormat.Bmp);
            Console.Read();
        }
    }
}
