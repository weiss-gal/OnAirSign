using OnAirSign.detection;
using OnAirSign.state;
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
    public delegate void Callback();
    public partial class Form1 : Form
    {
        int counter = 0;
        Callback onTickCB = null;
        OnAirStatus onAirStatus = new OnAirStatus();

        public Form1()
        {
            InitializeComponent();
            captureTimer.Enabled = true;
        }

        public void UpdateOnAirStatus(OnAirStatus status)
        {
            onAirStatus = status;
        }

        public void UpdateCommunicationStatusError(string error)
        {
            // TODO: add to display
            if (error == null)
                connectionStatusLabel.Text = "Connection Status: OK";
            else
                connectionStatusLabel.Text = $"Connection Status: {error}";
        }

        // Sets a callback for the tick timer
        public void OnTick(Callback cb)
        {
            onTickCB = cb;
        }
        
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            counter++;
            //counterLabel.Text = counter.ToString();

            //var audioStatus = AudioStatusDetection.IsAudioPlaying() ? "on" : "off";
            //var captureStatus = AudioStatusDetection.IsAudioCapturing() ? "on" : "off";

            //playbackStatusLabel.Text = $"Audio Status: {audioStatus}";
            //captureStatusLabel.Text = $"Capture Status: {captureStatus}";

            //Do whatever we were asked
            onTickCB();
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
