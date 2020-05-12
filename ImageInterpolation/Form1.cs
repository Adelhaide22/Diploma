using ImageInterpolation;
using ImageInterpolation.Filtering;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Lanczos
{
    public partial class Form1 : Form
    {
        ImageHelper.Filter Filter; 

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
            var scaledImage = ImageHelper.GetScaledImage(initialImage);

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
            pictureBox1.Image = greyImage;

            var brokenImage = new Bitmap(greyImage);
            switch (Filter)
            {
                case ImageHelper.Filter.Gauss:
                    brokenImage = GaussianFilter.Blur(greyImage);
                    break;
                case ImageHelper.Filter.Sharpen:
                    brokenImage = SharpenFilter.Sharpen(greyImage);
                    break;
                case ImageHelper.Filter.Motion:
                    brokenImage = MotionFilter.Motion(greyImage);
                    break;
                default:
                    break;
            }

            pictureBox2.Image = brokenImage;

            var coreImage = ImageHelper.GetCoreImage(ImageHelper.ToGray(brokenImage), Filter);
            pictureBox3.Image = coreImage;

            var reconstructedImage = WienerFilter.Filter(ImageHelper.ToGray(brokenImage), Filter);
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

            Filter = ImageHelper.Filter.Gauss;
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

            Filter = ImageHelper.Filter.Sharpen;
            //MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
        }

        private void btn_motion_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);
            var brokenImage = MotionFilter.Motion(initialImage);

            pictureBox2.Image = brokenImage;

            sw.Stop();
            Filter = ImageHelper.Filter.Motion;
        }
    }
}
