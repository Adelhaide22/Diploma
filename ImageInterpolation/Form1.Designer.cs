namespace Lanczos
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
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
        /// Required method for Designer support - do not modify
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
            this.btn_SART = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
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
            this.btn_edge.Location = new System.Drawing.Point(519, 36);
            this.btn_edge.Name = "btn_edge";
            this.btn_edge.Size = new System.Drawing.Size(121, 23);
            this.btn_edge.TabIndex = 1;
            this.btn_edge.Text = "Edge Directed";
            this.btn_edge.UseVisualStyleBackColor = true;
            // 
            // btn_lanzcos
            // 
            this.btn_lanzcos.Location = new System.Drawing.Point(710, 35);
            this.btn_lanzcos.Name = "btn_lanzcos";
            this.btn_lanzcos.Size = new System.Drawing.Size(122, 23);
            this.btn_lanzcos.TabIndex = 2;
            this.btn_lanzcos.Text = "Lanzcos Filter";
            this.btn_lanzcos.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(36, 98);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(600, 600);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(694, 98);
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
            this.btn_loadScaled.Location = new System.Drawing.Point(169, 35);
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
            // btn_SART
            // 
            this.btn_SART.Location = new System.Drawing.Point(907, 36);
            this.btn_SART.Name = "btn_SART";
            this.btn_SART.Size = new System.Drawing.Size(75, 23);
            this.btn_SART.TabIndex = 7;
            this.btn_SART.Text = "SART";
            this.btn_SART.UseVisualStyleBackColor = true;
            this.btn_SART.Click += new System.EventHandler(this.btn_SART_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1432, 761);
            this.Controls.Add(this.btn_SART);
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
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Button btn_SART;
    }
}

