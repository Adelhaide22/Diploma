using System;
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
        public static Complex[,] GetGausianComplexImage(Size OutImageSize, int HalfWindow, float aSigma)
        {
            Complex[,] lRes = null;

            int lMinHalf = Math.Min(OutImageSize.Width, OutImageSize.Height) / 2;
            HalfWindow = Math.Min(HalfWindow, lMinHalf);

            Complex[,] Gausian = new Complex[2 * HalfWindow + 15, 2 * HalfWindow + 1];
            int lHeight = 2 * HalfWindow + 1;
            int lWidth = 2 * HalfWindow + 1;
            float lCenterX = (float)HalfWindow;
            float lCenterY = (float)HalfWindow;

            float larg = -1 / (2 * aSigma * aSigma);
            float lSumm = 0;
            for (int ly = 0; ly < lHeight; ly++)
                for (int lx = 0; lx < lWidth; lx++)
                {
                    float xdiff = (float)lx - lCenterX;
                    float ydiff = (float)ly - lCenterY;
                    float lVal = (float)(Math.Exp(larg * (xdiff * xdiff + ydiff * ydiff)));
                    lSumm += lVal;
                    Gausian[lx, ly] = new Complex(lVal, 0);
                }

            for (int ly = 0; ly < lHeight; ly++)
                for (int lx = 0; lx < lWidth; lx++)
                {
                    Gausian[lx, ly] = new Complex(Gausian[lx, ly].Real / lSumm, 0);
                }

            lRes = new Complex[OutImageSize.Width, OutImageSize.Height];
            int lNewShiftX = (OutImageSize.Width - lWidth) / 2;
            int lNewShiftY = (OutImageSize.Height - lHeight) / 2;
            for (int ly = 0; ly < lHeight; ly++)
                for (int lx = 0; lx < lWidth; lx++)
                {
                    lRes[lx + lNewShiftX + 1, ly + lNewShiftY + 1] = Gausian[lx, ly];
                }

            return lRes;
        }

        public static Bitmap Filter(Bitmap initialImage)
        {

            for (int i = 0; i < length; i++)
            {
                for (int i = 0; i < length; i++)
                {

                }
            }
        }
    }
}
