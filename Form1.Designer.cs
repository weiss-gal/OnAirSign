namespace OnAirSign
{
    partial class OnAirForm
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
            this.components = new System.ComponentModel.Container();
            this.playbackStatusLabel = new System.Windows.Forms.Label();
            this.captureTimer = new System.Windows.Forms.Timer(this.components);
            this.captureStatusLabel = new System.Windows.Forms.Label();
            this.connectionStatusLabel = new System.Windows.Forms.Label();
            this.cameraStatusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // playbackStatusLabel
            // 
            this.playbackStatusLabel.AutoSize = true;
            this.playbackStatusLabel.Location = new System.Drawing.Point(129, 53);
            this.playbackStatusLabel.Name = "playbackStatusLabel";
            this.playbackStatusLabel.Size = new System.Drawing.Size(87, 13);
            this.playbackStatusLabel.TabIndex = 1;
            this.playbackStatusLabel.Text = "Playback Status:";
            // 
            // captureTimer
            // 
            this.captureTimer.Interval = 1000;
            this.captureTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // captureStatusLabel
            // 
            this.captureStatusLabel.AutoSize = true;
            this.captureStatusLabel.Location = new System.Drawing.Point(129, 76);
            this.captureStatusLabel.Name = "captureStatusLabel";
            this.captureStatusLabel.Size = new System.Drawing.Size(80, 13);
            this.captureStatusLabel.TabIndex = 2;
            this.captureStatusLabel.Text = "Capture Status:";
            // 
            // connectionStatusLabel
            // 
            this.connectionStatusLabel.AutoSize = true;
            this.connectionStatusLabel.Location = new System.Drawing.Point(129, 31);
            this.connectionStatusLabel.Name = "connectionStatusLabel";
            this.connectionStatusLabel.Size = new System.Drawing.Size(97, 13);
            this.connectionStatusLabel.TabIndex = 3;
            this.connectionStatusLabel.Text = "Connection Status:";
            this.connectionStatusLabel.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // cameraStatusLabel
            // 
            this.cameraStatusLabel.AutoSize = true;
            this.cameraStatusLabel.Location = new System.Drawing.Point(129, 99);
            this.cameraStatusLabel.Name = "cameraStatusLabel";
            this.cameraStatusLabel.Size = new System.Drawing.Size(79, 13);
            this.cameraStatusLabel.TabIndex = 4;
            this.cameraStatusLabel.Text = "Camera Status:";
            this.cameraStatusLabel.Click += new System.EventHandler(this.label1_Click_2);
            // 
            // OnAirForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cameraStatusLabel);
            this.Controls.Add(this.connectionStatusLabel);
            this.Controls.Add(this.captureStatusLabel);
            this.Controls.Add(this.playbackStatusLabel);
            this.Name = "OnAirForm";
            this.Text = "On Air Sign Controller";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label playbackStatusLabel;
        private System.Windows.Forms.Timer captureTimer;
        private System.Windows.Forms.Label captureStatusLabel;
        private System.Windows.Forms.Label connectionStatusLabel;
        private System.Windows.Forms.Label cameraStatusLabel;
    }
}

