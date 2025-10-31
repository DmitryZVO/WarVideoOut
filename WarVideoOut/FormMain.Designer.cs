namespace WarVideoOut
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBoxMain = new PictureBox();
            label1 = new Label();
            trackBarFps = new TrackBar();
            trackBarFrameSize = new TrackBar();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBoxMain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarFps).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarFrameSize).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxMain
            // 
            pictureBoxMain.BackColor = Color.Black;
            pictureBoxMain.Location = new Point(12, 12);
            pictureBoxMain.Name = "pictureBoxMain";
            pictureBoxMain.Size = new Size(800, 600);
            pictureBoxMain.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxMain.TabIndex = 0;
            pictureBoxMain.TabStop = false;
            // 
            // label1
            // 
            label1.Location = new Point(818, 12);
            label1.Name = "label1";
            label1.Size = new Size(131, 23);
            label1.TabIndex = 1;
            label1.Text = "Кадров в секунду:";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // trackBarFps
            // 
            trackBarFps.AutoSize = false;
            trackBarFps.Location = new Point(955, 12);
            trackBarFps.Maximum = 90;
            trackBarFps.Minimum = 1;
            trackBarFps.Name = "trackBarFps";
            trackBarFps.Size = new Size(297, 23);
            trackBarFps.TabIndex = 2;
            trackBarFps.Value = 30;
            // 
            // trackBarFrameSize
            // 
            trackBarFrameSize.AutoSize = false;
            trackBarFrameSize.LargeChange = 30;
            trackBarFrameSize.Location = new Point(955, 41);
            trackBarFrameSize.Maximum = 1200;
            trackBarFrameSize.Minimum = 90;
            trackBarFrameSize.Name = "trackBarFrameSize";
            trackBarFrameSize.Size = new Size(297, 23);
            trackBarFrameSize.SmallChange = 3;
            trackBarFrameSize.TabIndex = 4;
            trackBarFrameSize.Value = 600;
            // 
            // label2
            // 
            label2.Location = new Point(818, 41);
            label2.Name = "label2";
            label2.Size = new Size(131, 23);
            label2.TabIndex = 3;
            label2.Text = "Размер пакета (байт):";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 681);
            Controls.Add(trackBarFrameSize);
            Controls.Add(label2);
            Controls.Add(trackBarFps);
            Controls.Add(label1);
            Controls.Add(pictureBoxMain);
            MaximizeBox = false;
            Name = "FormMain";
            ShowIcon = false;
            Text = "Radio Video OUT";
            ((System.ComponentModel.ISupportInitialize)pictureBoxMain).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarFps).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarFrameSize).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBoxMain;
        private Label label1;
        private TrackBar trackBarFps;
        private TrackBar trackBarFrameSize;
        private Label label2;
    }
}
