using NAudio.CoreAudioApi;

namespace OnAirSign.detection
{
    //TODO: optimize by making a non-static class and reduce number of enumarations
    public static class AudioStatusDetection
    {
       
        private static bool IsDeviceStreaming(DataFlow type)
        {
            var enumerator = new MMDeviceEnumerator();
            MMDevice speakers;
            try
            {
                speakers = enumerator.GetDefaultAudioEndpoint(type, Role.Multimedia);
            }
            // No default audio device found
            catch (System.Runtime.InteropServices.COMException)
            {
                return false;
            }

            var values = speakers.AudioMeterInformation.PeakValues;

            // this is a bit tricky. 0 is the official "no sound" value
            // but for example, if you open a video and plays/stops with it (w/o killing the app/window/stream),
            // the value will not be zero, but something really small (around 1E-09)
            // so, depending on your context, it is up to you to decide
            // if you want to test for 0 or for a small value
            for (int i = 0; i < values.Count; i++)
            {
                var value = values[i];
                if (value > 1E-08)
                    return true;
            }
            return false;
        }

        public static bool IsAudioPlaying()
        {
            return IsDeviceStreaming(DataFlow.Render);
        }
        
        public static bool IsAudioCapturing()
        {
            return IsDeviceStreaming(DataFlow.Capture);
        }
    }
}