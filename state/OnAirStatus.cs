using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnAirSign.state
{
    public struct OnAirStatus
    {
        public OnAirStatus(bool isAudioPlaying = false, bool isAudioCapturing = false, bool isCameraCapturing = false) 
        {
            IsAudioPlaying = isAudioPlaying;
            IsAudioCapturing = isAudioCapturing;
            IsCameraCapturing = isCameraCapturing;
        }

        public bool IsAudioPlaying { get; }
        public bool IsAudioCapturing { get; }
        public bool IsCameraCapturing { get; }
    }
}
