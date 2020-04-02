using ImageInterpolation;
using ImageInterpolation.Filtering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lanczos
{
    public partial class Form1 : Form
    {
        Bitmap origin;

        public Form1()
        {
            InitializeComponent();

            btn_loadOrigin.Click += btn_openFile_Click;
            btn_loadScaled.Click += button_loadScaled_Click;

            btn_edge.Click += btn_Edge_Click;
            btn_lanzcos.Click += btn_Lanczos_Click;
            //btn_SART.Click += btn_SART_Click;
            btn_Wiener.Click += btn_Wiener_Click;

            openFileDialog1.Filter = "Image files (*.jpg)|*.jpg|Bitmap files (*.bmp)|*.bmp";
            openFileDialog2.Filter = "Image files (*.jpg)|*.jpg|Bitmap files (*.bmp)|*.bmp";
        }

        void button_loadScaled_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            
            origin = (Bitmap)Image.FromFile(openFileDialog2.FileName);
        }

        void btn_openFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);

            pictureBox1.Image = initialImage;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        void btn_Edge_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);
            var scaledImage = GetScaledImage(initialImage);

            var resampledImage = EdgeSensitiveInterpolator.Resample(scaledImage);

            pictureBox2.Image = resampledImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

            sw.Stop();

            //var n = origin.Width;
            //var m = origin.Height;

            //var f = 0.0;

            //for (int i = 0; i < m - 1; i++)
            //{
            //    for (int j = 0; j < n - 1; j++)
            //    {
            //        f += Math.Pow((resampledImage.GetPixel(i, j).R - origin.GetPixel(i, j).R), 2) +
            //            Math.Pow((resampledImage.GetPixel(i, j).B - origin.GetPixel(i, j).B), 2) +
            //            Math.Pow((resampledImage.GetPixel(i, j).G - origin.GetPixel(i, j).G), 2);
            //    }
            //}

            //string s = 10 * Math.Log((255*255/(f/ (3 * n * m))), 10) + "\n";

            //MessageBox.Show(s);

            MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
        }

        private void btn_SART_Click(object sender, EventArgs e)
        {
            //var sw = new Stopwatch();
            //sw.Start();

            //var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);
            //var scaledImage = GetScaledImage(initialImage);

            //var resampledImage = SARTInterpolator.resample(scaledImage);

            //pictureBox2.Image = resampledImage;
            //pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

            //sw.Stop();
        }

        void btn_Lanczos_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);

            var resampledImage = LanczosInterpolator.Resample(initialImage);

            pictureBox2.Image = resampledImage;      
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

            sw.Stop();
            //var n = origin.Width;
            //var m = origin.Height;

            //var f = 0.0;

            //for (int i = 0; i < m - 1; i++)
            //{
            //    for (int j = 0; j < n - 1; j++)
            //    {
            //        f += Math.Pow((resampledImage.GetPixel(i, j).R - origin.GetPixel(i, j).R), 2) +
            //            Math.Pow((resampledImage.GetPixel(i, j).B - origin.GetPixel(i, j).B), 2) +
            //            Math.Pow((resampledImage.GetPixel(i, j).G - origin.GetPixel(i, j).G), 2);
            //    }
            //}

            //string s =  10 * Math.Log((255 * 255 / (f / (3 * n * m))), 10) + "\n";            

            MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
        }
        private  void  btn_Wiener_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);
            var filteredImage =  WienerFilter.Filter(initialImage);

            pictureBox2.Image = filteredImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            sw.Stop();

            MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
        }

        private Bitmap GetScaledImage(Bitmap initialImage)
        {
            var scaledImage = new Bitmap(initialImage.Width * 2, initialImage.Height * 2);

            for (int i = 0; i < scaledImage.Height - 2; i+=2)
            {
                for (int j = 0; j < scaledImage.Width - 2; j+=2)
                {
                    scaledImage.SetPixel(i, j, initialImage.GetPixel(i / 2, j / 2));
                }
            }

            return scaledImage;
        }        
    }
}
