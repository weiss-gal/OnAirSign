using OnAirSign.detection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnAirSign
{
    public partial class Form1 : Form
    {
        int counter = 0;
        public Form1()
        {
            InitializeComponent();
            captureTimer.Enabled = true;
        }
        
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            counter++;
            counterLabel.Text = counter.ToString();

            var audioStatus = CallStatusDetection.IsAudioPlaying() ? "on" : "off";
            var captureStatus = CallStatusDetection.IsAudioCapturing() ? "on" : "off";

            playbackStatusLabel.Text = $"Audio Status: {audioStatus}";
            captureStatusLabel.Text = $"Capture Status: {captureStatus}";
        }

    }
}
