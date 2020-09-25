using OnAirSign.detection;
using OnAirSign.display;
using OnAirSign.state;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace OnAirSign
{
  


    static class Program
    {
        static Form1 form;
        static LedDisplay display;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new Form1();
            form.OnTick(timer);
            display = new LedDisplay();
            Application.Run(form);
            
        }

        private static OnAirStatus getOnAirStatus()
        {
            return new OnAirStatus(AudioStatusDetection.IsAudioPlaying(), AudioStatusDetection.IsAudioCapturing(), false);
        }

        private static void refreshDisplay(OnAirStatus onAirStatus)
        {
            form.UpdateOnAirStatus(onAirStatus);
            var displayUpdateResult = display.UpdateDisplay(onAirStatus);
            var communicationStatus = display.GetConnectionStatus();
            var errorMessage = displayUpdateResult != 0 ? "Communication Error" : communicationStatus; 

            form.UpdateCommunicationStatusError(errorMessage);
        }

        private static void timer()
        {
            Console.WriteLine("Timer started");
            var onAirStatus = getOnAirStatus();
            refreshDisplay(onAirStatus);
        }
    }
}
