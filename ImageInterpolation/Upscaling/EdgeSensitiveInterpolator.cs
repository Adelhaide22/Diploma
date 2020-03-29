using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageInterpolation
{
    static class EdgeSensitiveInterpolator
    {
        public static Bitmap Resample(Bitmap initialImage)
        {
            for (int i = 0; i < initialImage.Height - 2; i += 2)
            {
                for (int j = 0; j < initialImage.Width - 2; j += 2)
                {
                    if ((i + j) % 2 == 0)
                    {
                        var a = initialImage.GetPixel(i, j);
                        var b = initialImage.GetPixel(i + 2, j);
                        var c = initialImage.GetPixel(i, j + 2);
                        var d = initialImage.GetPixel(i + 2, j + 2);

                        if (GetDiff(a, c) > GetDiff(b, d))
                        {
                            initialImage.SetPixel(i + 1, j + 1, GetColor(b, d));
                        }
                        else
                        {
                            initialImage.SetPixel(i + 1, j + 1, GetColor(a, c));
                        }
                    }
                }
            }

            for (int i = 1; i < initialImage.Height - 1; i++)
            {
                for (int j = 1; j < initialImage.Width - 1; j++)
                {
                    if ((i + j) % 2 != 0)
                    {
                        var a = initialImage.GetPixel(i, j - 1);
                        var b = initialImage.GetPixel(i, j + 1);
                        var c = initialImage.GetPixel(i + 1, j);
                        var d = initialImage.GetPixel(i - 1, j);

                        if (GetDiff(a, c) > GetDiff(b, d))
                        {
                            initialImage.SetPixel(i, j, GetColor(b, d));
                        }
                        else
                        {
                            initialImage.SetPixel(i, j, GetColor(a, c));
                        }
                    }
                }
            }

            for (int i = 0; i < initialImage.Height; i++)
            {
                var a = initialImage.GetPixel(i, 1);
                initialImage.SetPixel(i, 0, a);

                var b = initialImage.GetPixel(i, initialImage.Width - 1);
                initialImage.SetPixel(i, initialImage.Width - 2, b);
            }

            for (int j = 0; j < initialImage.Width; j++)
            {
                var a = initialImage.GetPixel(1, j);
                initialImage.SetPixel(0, j, a);

                var b = initialImage.GetPixel(initialImage.Height - 1, j);
                initialImage.SetPixel(initialImage.Height - 2, j, b);
            }

            return initialImage;
        }

        private static Color GetColor(Color a, Color b)
        {
            var red = (a.R + b.R) / 2;
            var green = (a.G + b.G) / 2;
            var blue = (a.B + b.B) / 2;
            return Color.FromArgb(red, green, blue);
        }

        private static int GetDiff(Color a, Color b)
        {
            return Math.Abs(a.R - b.R) + Math.Abs(a.B - b.B) + Math.Abs(a.G - b.G) + Math.Abs(a.A - b.A);
        }
    }
}
