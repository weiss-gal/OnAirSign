using OnAirSign.state;
using System;
using System.Windows.Forms;

namespace OnAirSign
{
    public delegate void Callback();
    public partial class OnAirForm : Form
    {
        int counter = 0;
        Callback onTickCB = null;
        OnAirStatus onAirStatus; 

        public OnAirForm()
        {
            InitializeComponent();
            captureTimer.Enabled = true;
            UpdateOnAirStatus(new OnAirStatus());
        }

        public void UpdateOnAirStatus(OnAirStatus status)
        {
            onAirStatus = status;
            playbackStatusOutputLabel.Text = status.IsAudioPlaying ? "Streaming" : "Idle";
            microphoneStatusOutputLabel.Text = status.IsAudioCapturing ? "Streaming" : "Idle";
        }

        public void UpdateCommunicationStatusError(string error)
        {
            // TODO: add to display
            if (error == null)
                connectionStatusOutputLabel.Text = "Connected";
            else
                connectionStatusOutputLabel.Text = $"Disconnected - {error}";
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

        private void label1_Click_2(object sender, EventArgs e)
        {

        }

        private void playbackStatusLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
