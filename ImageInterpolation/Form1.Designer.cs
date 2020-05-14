namespace Lanczos
{
    partial class Form1
    {
        /// <summary>
        /// required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_loadOrigin = new System.Windows.Forms.Button();
            this.btn_edge = new System.Windows.Forms.Button();
            this.btn_lanzcos = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btn_loadScaled = new System.Windows.Forms.Button();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.btn_Wiener = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.btn_Gaussian = new System.Windows.Forms.Button();
            this.btn_Sharpen = new System.Windows.Forms.Button();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.btn_motion = new System.Windows.Forms.Button();
            this.btn_WienerPredict = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_loadOrigin
            // 
            this.btn_loadOrigin.Location = new System.Drawing.Point(36, 36);
            this.btn_loadOrigin.Name = "btn_loadOrigin";
            this.btn_loadOrigin.Size = new System.Drawing.Size(75, 23);
            this.btn_loadOrigin.TabIndex = 0;
            this.btn_loadOrigin.Text = "Load Origin";
            this.btn_loadOrigin.UseVisualStyleBackColor = true;
            // 
            // btn_edge
            // 
            this.btn_edge.Location = new System.Drawing.Point(35, 130);
            this.btn_edge.Name = "btn_edge";
            this.btn_edge.Size = new System.Drawing.Size(121, 23);
            this.btn_edge.TabIndex = 1;
            this.btn_edge.Text = "Edge Directed";
            this.btn_edge.UseVisualStyleBackColor = true;
            // 
            // btn_lanzcos
            // 
            this.btn_lanzcos.Location = new System.Drawing.Point(35, 178);
            this.btn_lanzcos.Name = "btn_lanzcos";
            this.btn_lanzcos.Size = new System.Drawing.Size(122, 23);
            this.btn_lanzcos.TabIndex = 2;
            this.btn_lanzcos.Text = "Lanzcos Filter";
            this.btn_lanzcos.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(291, 36);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(400, 400);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(714, 36);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(400, 400);
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btn_loadScaled
            // 
            this.btn_loadScaled.Location = new System.Drawing.Point(133, 36);
            this.btn_loadScaled.Name = "btn_loadScaled";
            this.btn_loadScaled.Size = new System.Drawing.Size(75, 23);
            this.btn_loadScaled.TabIndex = 6;
            this.btn_loadScaled.Text = "Load Scaled";
            this.btn_loadScaled.UseVisualStyleBackColor = true;
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            // 
            // btn_Wiener
            // 
            this.btn_Wiener.Location = new System.Drawing.Point(33, 253);
            this.btn_Wiener.Name = "btn_Wiener";
            this.btn_Wiener.Size = new System.Drawing.Size(122, 23);
            this.btn_Wiener.TabIndex = 8;
            this.btn_Wiener.Text = "Wiener Filter";
            this.btn_Wiener.UseVisualStyleBackColor = true;
            this.btn_Wiener.Click += new System.EventHandler(this.btn_Wiener_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(291, 457);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(400, 400);
            this.pictureBox3.TabIndex = 9;
            this.pictureBox3.TabStop = false;
            // 
            // btn_Gaussian
            // 
            this.btn_Gaussian.Location = new System.Drawing.Point(33, 474);
            this.btn_Gaussian.Name = "btn_Gaussian";
            this.btn_Gaussian.Size = new System.Drawing.Size(121, 23);
            this.btn_Gaussian.TabIndex = 10;
            this.btn_Gaussian.Text = "Gaussian Filter";
            this.btn_Gaussian.UseVisualStyleBackColor = true;
            this.btn_Gaussian.Click += new System.EventHandler(this.btn_Gaussian_Click);
            // 
            // btn_Sharpen
            // 
            this.btn_Sharpen.Location = new System.Drawing.Point(33, 519);
            this.btn_Sharpen.Name = "btn_Sharpen";
            this.btn_Sharpen.Size = new System.Drawing.Size(122, 23);
            this.btn_Sharpen.TabIndex = 11;
            this.btn_Sharpen.Text = "Sharpen Filter";
            this.btn_Sharpen.UseVisualStyleBackColor = true;
            this.btn_Sharpen.Click += new System.EventHandler(this.btn_Sharpen_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Location = new System.Drawing.Point(714, 457);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(400, 400);
            this.pictureBox4.TabIndex = 12;
            this.pictureBox4.TabStop = false;
            // 
            // btn_motion
            // 
            this.btn_motion.Location = new System.Drawing.Point(33, 562);
            this.btn_motion.Name = "btn_motion";
            this.btn_motion.Size = new System.Drawing.Size(121, 23);
            this.btn_motion.TabIndex = 13;
            this.btn_motion.Text = "Motion Filter";
            this.btn_motion.UseVisualStyleBackColor = true;
            this.btn_motion.Click += new System.EventHandler(this.btn_motion_Click);
            // 
            // btn_WienerPredict
            // 
            this.btn_WienerPredict.Location = new System.Drawing.Point(33, 297);
            this.btn_WienerPredict.Name = "btn_WienerPredict";
            this.btn_WienerPredict.Size = new System.Drawing.Size(123, 23);
            this.btn_WienerPredict.TabIndex = 14;
            this.btn_WienerPredict.Text = "Wiener Prediction ";
            this.btn_WienerPredict.UseVisualStyleBackColor = true;
            this.btn_WienerPredict.Click += new System.EventHandler(this.btn_WienerPredict_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 366);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Kernel size";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(36, 393);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 16;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1141, 877);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_WienerPredict);
            this.Controls.Add(this.btn_motion);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.btn_Sharpen);
            this.Controls.Add(this.btn_Gaussian);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.btn_Wiener);
            this.Controls.Add(this.btn_loadScaled);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btn_lanzcos);
            this.Controls.Add(this.btn_edge);
            this.Controls.Add(this.btn_loadOrigin);
            this.Name = "Form1";
            this.Text = "Image resampling";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_loadOrigin;
        private System.Windows.Forms.Button btn_edge;
        private System.Windows.Forms.Button btn_lanzcos;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btn_loadScaled;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Button btn_Wiener;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Button btn_Gaussian;
        private System.Windows.Forms.Button btn_Sharpen;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Button btn_motion;
        private System.Windows.Forms.Button btn_WienerPredict;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
    }
}

