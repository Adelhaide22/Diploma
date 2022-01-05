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
        int kernelSize;
        double delta;
        double alpha;
        Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();

            btn_loadOrigin.Click += btn_openFile_Click;
            
            openFileDialog1.Filter = "Image files (*.jpg)|*.jpg|Bitmap files (*.bmp)|*.bmp";
            openFileDialog2.Filter = "Image files (*.jpg)|*.jpg|Bitmap files (*.bmp)|*.bmp";

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; 
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
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

        private void btn_Wiener_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();

            var initialImage = (Bitmap)pictureBox1.Image;

            var greyImage = ImageHelper.ToGray(initialImage);
            pictureBox1.Image = greyImage;

            var brokenImage = new Bitmap(greyImage);
            switch (Filter)
            {
                case ImageHelper.Filter.Gauss:
                    brokenImage = GaussianFilter.Blur(greyImage, kernelSize);
                    break;
                case ImageHelper.Filter.Sharpen:
                    brokenImage = SharpenFilter.Sharpen(greyImage, kernelSize);
                    break;
                case ImageHelper.Filter.MotionLeftToRight:
                    brokenImage = MotionFilter.Motion(greyImage, kernelSize, Direction.LeftToRight);
                    break;
                case ImageHelper.Filter.MotionRightToLeft:
                    brokenImage = MotionFilter.Motion(greyImage, kernelSize, Direction.RightToLeft);
                    break;
                default:
                    break;
            }

            pictureBox2.Image = brokenImage;

            var coreImage = ImageHelper.GetCoreImage(ImageHelper.ToGray(brokenImage), Filter);
            pictureBox3.Image = coreImage;

            sw.Start();
            var reconstructedImage = WienerFilter.Filter(ImageHelper.ToGray(brokenImage), Filter);
            pictureBox4.Image = reconstructedImage;

            sw.Stop();

            MessageBox.Show($"PSNR: {ImageHelper.GetPSNR(greyImage, reconstructedImage)}\nTime: {sw.Elapsed.TotalSeconds}");
        }

        private void btn_Gaussian_Click(object sender, EventArgs e)
        {
            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);
            var brokenImage = GaussianFilter.Blur(initialImage, kernelSize);

            pictureBox2.Image = brokenImage;

            Filter = ImageHelper.Filter.Gauss;
        }

        private void btn_Sharpen_Click(object sender, EventArgs e)
        {
            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);            
            var brokenImage = SharpenFilter.Sharpen(initialImage, kernelSize); 

            pictureBox2.Image = brokenImage;            

            Filter = ImageHelper.Filter.Sharpen;
        }

        private void btn_motion_Click(object sender, EventArgs e)
        {
            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);
            var brokenImage = MotionFilter.Motion(initialImage, kernelSize, Direction.RightToLeft);

            pictureBox2.Image = brokenImage;

            Filter = ImageHelper.Filter.MotionRightToLeft;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length != 0)
            {
                kernelSize = int.Parse(textBox1.Text);
            }
        }

        private void btn_tikh_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();

            var initialImage = (Bitmap)pictureBox1.Image;

            var greyImage = ImageHelper.ToGray(initialImage);
            pictureBox1.Image = greyImage;

            var brokenImage = new Bitmap(greyImage);
            switch (Filter)
            {
                case ImageHelper.Filter.Gauss:
                    brokenImage = GaussianFilter.Blur(greyImage, kernelSize);
                    break;
                case ImageHelper.Filter.Sharpen:
                    brokenImage = SharpenFilter.Sharpen(greyImage, kernelSize);
                    break;
                case ImageHelper.Filter.MotionLeftToRight:
                    brokenImage = MotionFilter.Motion(greyImage, kernelSize, Direction.LeftToRight);
                    break;
                case ImageHelper.Filter.MotionRightToLeft:
                    brokenImage = MotionFilter.Motion(greyImage, kernelSize, Direction.RightToLeft);
                    break;
                default:
                    break;
            }

            pictureBox2.Image = brokenImage;

            var coreImage = ImageHelper.GetCoreImage(ImageHelper.ToGray(brokenImage), Filter);
            pictureBox3.Image = coreImage;

            sw.Start();
            Bitmap reconstructedImage;
            if (checkBox_gauss.Checked)
            {
                reconstructedImage = TikhonovFilter.FilterGauss(ImageHelper.ToGray(brokenImage), Filter, alpha, delta);
            }
            else if (checkBox_mpi.Checked)
            {
                reconstructedImage = TikhonovFilter.FilterMPI(ImageHelper.ToGray(brokenImage), Filter, alpha, delta);
            }
            else
            {
                reconstructedImage = TikhonovFilter.FilterMatrix(ImageHelper.ToGray(brokenImage), Filter, alpha, delta);                
            }

            pictureBox4.Image = reconstructedImage;
            sw.Stop();

            MessageBox.Show($"PSNR: {ImageHelper.GetPSNR(greyImage, reconstructedImage)}\nTime: {sw.Elapsed.TotalSeconds}");
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = alpha.ToString();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox3.Text = delta.ToString();
        }

        private void checkBox_random_CheckedChanged(object sender, EventArgs e)
        {
            delta = 0.01 * rnd.Next(1, 1000);
            alpha = rnd.NextDouble();

            textBox2_TextChanged(sender, e);
            textBox3_TextChanged(sender, e);
            btn_tikh_Click(sender, e);
        }

        private void checkBox_nevyazka_CheckedChanged(object sender, EventArgs e)
        {
           // delta = 0.01 * rnd.Next(1, 1000);
           // alpha = rnd.NextDouble();

            textBox2_TextChanged(sender, e);
            textBox3_TextChanged(sender, e);
            btn_tikh_Click(sender, e);
        }

        private void checkBox_opti_CheckedChanged(object sender, EventArgs e)
        {
           // delta = 0.01 * rnd.Next(1, 1000);
           // alpha = rnd.NextDouble();

            textBox2_TextChanged(sender, e);
            textBox3_TextChanged(sender, e);
            btn_tikh_Click(sender, e);
        }
    }
}
