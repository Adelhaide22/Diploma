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
        public Form1()
        {
            InitializeComponent();

            btn_loadOrigin.Click += btn_openFile_Click;
            btn_loadScaled.Click += button_loadScaled_Click;
            
            openFileDialog1.Filter = "Image files (*.jpg)|*.jpg|Bitmap files (*.bmp)|*.bmp";
            openFileDialog2.Filter = "Image files (*.jpg)|*.jpg|Bitmap files (*.bmp)|*.bmp";

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; 
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        void button_loadScaled_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            
            Bitmap origin = (Bitmap)Image.FromFile(openFileDialog2.FileName);
        }

        void btn_openFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);

            pictureBox1.Image = initialImage;
        }

        void btn_Edge_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);
            var scaledImage = GetScaledImage(initialImage);

            var resampledImage = EdgeSensitiveInterpolator.Resample(scaledImage);

            pictureBox2.Image = resampledImage;

            sw.Stop();
            
            MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
        }

        void btn_Lanczos_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);

            var resampledImage = LanczosInterpolator.Resample(initialImage);

            pictureBox2.Image = resampledImage;      

            sw.Stop();
                    

            MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
        }
        private void btn_Wiener_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var initialImage = (Bitmap)pictureBox1.Image;

            var greyImage = ImageHelper.ToGray(initialImage);
            pictureBox2.Image = greyImage;

            var brokenImage = GaussianFilter.Blur(greyImage);
            pictureBox3.Image = brokenImage;

            var reconstructedImage = WienerFilter.Filter(ImageHelper.ToGray(brokenImage));
            pictureBox4.Image = reconstructedImage;

            sw.Stop();

            //MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
        }

        private void btn_Gaussian_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);
            var brokenImage = GaussianFilter.Blur(initialImage);

            pictureBox2.Image = brokenImage;

            sw.Stop();
            //MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
        }

        private void btn_Sharpen_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);
            var brokenImage = SharpenFilter.Sharpen(initialImage);

            pictureBox2.Image = brokenImage;

            sw.Stop();
            //MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
        }

        private Bitmap GetScaledImage(Bitmap initialImage)
        {
            var scaledImage = new Bitmap(initialImage.Width * 2, initialImage.Height * 2);

            for (int i = 0; i < scaledImage.Height - 2; i += 2)
            {
                for (int j = 0; j < scaledImage.Width - 2; j += 2)
                {
                    scaledImage.SetPixel(i, j, initialImage.GetPixel(i / 2, j / 2));
                }
            }

            return scaledImage;
        }

    }
}
