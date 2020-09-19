namespace OnAirSign
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
            this.components = new System.ComponentModel.Container();
            this.counterLabel = new System.Windows.Forms.Label();
            this.playbackStatusLabel = new System.Windows.Forms.Label();
            this.captureTimer = new System.Windows.Forms.Timer(this.components);
            this.captureStatusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // counterLabel
            // 
            this.counterLabel.AutoSize = true;
            this.counterLabel.Location = new System.Drawing.Point(129, 53);
            this.counterLabel.Name = "counterLabel";
            this.counterLabel.Size = new System.Drawing.Size(50, 13);
            this.counterLabel.TabIndex = 0;
            this.counterLabel.Text = "Counter: ";
            this.counterLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // playbackStatusLabel
            // 
            this.playbackStatusLabel.AutoSize = true;
            this.playbackStatusLabel.Location = new System.Drawing.Point(129, 97);
            this.playbackStatusLabel.Name = "playbackStatusLabel";
            this.playbackStatusLabel.Size = new System.Drawing.Size(80, 13);
            this.playbackStatusLabel.TabIndex = 1;
            this.playbackStatusLabel.Text = "Capture Status:";
            // 
            // captureTimer
            // 
            this.captureTimer.Interval = 1000;
            this.captureTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // captureStatusLabel
            // 
            this.captureStatusLabel.AutoSize = true;
            this.captureStatusLabel.Location = new System.Drawing.Point(129, 75);
            this.captureStatusLabel.Name = "captureStatusLabel";
            this.captureStatusLabel.Size = new System.Drawing.Size(87, 13);
            this.captureStatusLabel.TabIndex = 2;
            this.captureStatusLabel.Text = "Playback Status:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.captureStatusLabel);
            this.Controls.Add(this.playbackStatusLabel);
            this.Controls.Add(this.counterLabel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label counterLabel;
        private System.Windows.Forms.Label playbackStatusLabel;
        private System.Windows.Forms.Timer captureTimer;
        private System.Windows.Forms.Label captureStatusLabel;
    }
}

