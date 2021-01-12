using NAudio.CoreAudioApi;
using OnAirSign.detection;
using OnAirSign.display;
using OnAirSign.infra.logging;
using OnAirSign.state;
using System;
using System.Windows.Forms;

namespace OnAirSign
{
    static class Program
    {
        static OnAirForm form;
        static LedDisplay display;
        static AudioStatusDetector audioPlayingDetector;
        static AudioStatusDetector audioCapturingDetector;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            audioPlayingDetector = new AudioStatusDetector(DataFlow.Render);
            audioCapturingDetector = new AudioStatusDetector(DataFlow.Capture);
            form = new OnAirForm();
            form.OnTick(Timer);
            var logger = new ConsoleLogger(LogLevel.Debug);
            display = new LedDisplay(logger);
            Application.Run(form);
        }

        private static OnAirStatus GetOnAirStatus()
        {
            return new OnAirStatus(audioPlayingDetector.IsDeviceStreaming, audioCapturingDetector.IsDeviceStreaming, false);
        }

        private static void RefreshDisplay(OnAirStatus onAirStatus)
        {
            form.UpdateOnAirStatus(onAirStatus);
            var displayUpdateResult = display.UpdateDisplay(onAirStatus);
            var communicationStatus = display.GetConnectionStatus();
            var errorMessage = displayUpdateResult != 0 ? "Communication Error" : communicationStatus; 

            form.UpdateCommunicationStatusError(errorMessage);
        }

        private static void Timer()
        {
            var onAirStatus = GetOnAirStatus();
            RefreshDisplay(onAirStatus);
        }
    }
}