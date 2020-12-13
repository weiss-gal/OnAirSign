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
            this.microphoneStatusLabel = new System.Windows.Forms.Label();
            this.connectionStatusLabel = new System.Windows.Forms.Label();
            this.connectionStatusOutputLabel = new System.Windows.Forms.Label();
            this.playbackStatusOutputLabel = new System.Windows.Forms.Label();
            this.microphoneStatusOutputLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // playbackStatusLabel
            // 
            this.playbackStatusLabel.AutoSize = true;
            this.playbackStatusLabel.Location = new System.Drawing.Point(21, 41);
            this.playbackStatusLabel.Name = "playbackStatusLabel";
            this.playbackStatusLabel.Size = new System.Drawing.Size(87, 13);
            this.playbackStatusLabel.TabIndex = 1;
            this.playbackStatusLabel.Text = "Playback Status:";
            this.playbackStatusLabel.Click += new System.EventHandler(this.playbackStatusLabel_Click);
            // 
            // captureTimer
            // 
            this.captureTimer.Interval = 1000;
            this.captureTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // microphoneStatusLabel
            // 
            this.microphoneStatusLabel.AutoSize = true;
            this.microphoneStatusLabel.Location = new System.Drawing.Point(21, 64);
            this.microphoneStatusLabel.Name = "microphoneStatusLabel";
            this.microphoneStatusLabel.Size = new System.Drawing.Size(99, 13);
            this.microphoneStatusLabel.TabIndex = 2;
            this.microphoneStatusLabel.Text = "Microphone Status:";
            // 
            // connectionStatusLabel
            // 
            this.connectionStatusLabel.AutoSize = true;
            this.connectionStatusLabel.Location = new System.Drawing.Point(21, 19);
            this.connectionStatusLabel.Name = "connectionStatusLabel";
            this.connectionStatusLabel.Size = new System.Drawing.Size(66, 13);
            this.connectionStatusLabel.TabIndex = 3;
            this.connectionStatusLabel.Text = "LED Dispay:";
            this.connectionStatusLabel.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // connectionStatusOutputLabel
            // 
            this.connectionStatusOutputLabel.AutoSize = true;
            this.connectionStatusOutputLabel.Location = new System.Drawing.Point(124, 19);
            this.connectionStatusOutputLabel.Name = "connectionStatusOutputLabel";
            this.connectionStatusOutputLabel.Size = new System.Drawing.Size(35, 13);
            this.connectionStatusOutputLabel.TabIndex = 4;
            this.connectionStatusOutputLabel.Text = "label1";
            // 
            // playbackStatusOutputLabel
            // 
            this.playbackStatusOutputLabel.AutoSize = true;
            this.playbackStatusOutputLabel.Location = new System.Drawing.Point(124, 41);
            this.playbackStatusOutputLabel.Name = "playbackStatusOutputLabel";
            this.playbackStatusOutputLabel.Size = new System.Drawing.Size(35, 13);
            this.playbackStatusOutputLabel.TabIndex = 5;
            this.playbackStatusOutputLabel.Text = "label2";
            // 
            // microphoneStatusOutputLabel
            // 
            this.microphoneStatusOutputLabel.AutoSize = true;
            this.microphoneStatusOutputLabel.Location = new System.Drawing.Point(124, 64);
            this.microphoneStatusOutputLabel.Name = "microphoneStatusOutputLabel";
            this.microphoneStatusOutputLabel.Size = new System.Drawing.Size(35, 13);
            this.microphoneStatusOutputLabel.TabIndex = 6;
            this.microphoneStatusOutputLabel.Text = "label3";
            // 
            // OnAirForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 108);
            this.Controls.Add(this.microphoneStatusOutputLabel);
            this.Controls.Add(this.playbackStatusOutputLabel);
            this.Controls.Add(this.connectionStatusOutputLabel);
            this.Controls.Add(this.connectionStatusLabel);
            this.Controls.Add(this.microphoneStatusLabel);
            this.Controls.Add(this.playbackStatusLabel);
            this.Name = "OnAirForm";
            this.Text = "On Air Sign Controller";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label playbackStatusLabel;
        private System.Windows.Forms.Timer captureTimer;
        private System.Windows.Forms.Label microphoneStatusLabel;
        private System.Windows.Forms.Label connectionStatusLabel;
        private System.Windows.Forms.Label connectionStatusOutputLabel;
        private System.Windows.Forms.Label playbackStatusOutputLabel;
        private System.Windows.Forms.Label microphoneStatusOutputLabel;
    }
}

