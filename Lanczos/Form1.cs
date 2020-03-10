using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

            btn_openFile.Click += btn_openFile_Click;
            btn_method1.Click += btn_method1_Click;
            btn_method2.Click += btn_method2_Click;
            btn_method3.Click += btn_method3_Click;
            openFileDialog1.Filter = "Image files (*.jpg)|*.jpg|Bitmap files (*.bmp)|*.bmp";
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

        void btn_method1_Click(object sender, EventArgs e)
        {
            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);
            var scaledImage = GetScaledImage(initialImage);

            var resampledImage = EdgeSensitiveInterpolator.Resample(scaledImage);

            pictureBox2.Image = resampledImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        void btn_method2_Click(object sender, EventArgs e)
        {
            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);
            var scaledImage = GetScaledImage(initialImage);

            var resampledImage = LanczosInterpolator.Resample(scaledImage);

            pictureBox2.Image = resampledImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        void btn_method3_Click(object sender, EventArgs e)
        {
            var initialImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);
            var scaledImage = GetScaledImage(initialImage);

        }
    }
}
