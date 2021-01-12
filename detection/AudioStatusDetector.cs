using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace OnAirSign.detection
{
    //TODO: optimize by making a non-static class and reduce number of enumarations
    public class AudioStatusDetector : IAudioChangeListener
    {
        private readonly DataFlow _type;
        private MMDeviceCollection _collection;
        private bool _collectionShouldChange = true;
        private readonly MMDeviceEnumerator _enumerator;
        private readonly NotificationClient _notificationClient;

        public AudioStatusDetector(DataFlow type)
        {
            
            _type = type;
            _enumerator = new MMDeviceEnumerator();
            _notificationClient = new NotificationClient(this);
            _enumerator.RegisterEndpointNotificationCallback(_notificationClient);

        }

        ~AudioStatusDetector()
        {
            _enumerator.UnregisterEndpointNotificationCallback(_notificationClient);
        }

        public bool IsDeviceStreaming
        {
            get
            {
                // COM is not working very well with threads, so we defer the refresh to happen here
                // can't get more lazy than that
                if (_collectionShouldChange)
                {
                    _collection = _enumerator.EnumerateAudioEndPoints(_type, DeviceState.Active);
                    _collectionShouldChange = false;
                }

                foreach (var device in _collection)
                {
                    var values = device.AudioMeterInformation.PeakValues;
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
                    
                }

                return false;
            }
        }

        public void OnAudioChange()
        {
            // No reason to be to subtle here, we just get a new enumarator
            _collectionShouldChange = true;
        }

        class NotificationClient : IMMNotificationClient
        {
            private readonly IAudioChangeListener _listener;
            internal NotificationClient(IAudioChangeListener listener)
            {
                _listener = listener;
            }

            public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
            {
            }

            public void OnDeviceAdded(string pwstrDeviceId)
            {
                _listener.OnAudioChange();
            }

            public void OnDeviceRemoved(string deviceId)
            {
                _listener.OnAudioChange();
            }

            public void OnDeviceStateChanged(string deviceId, DeviceState newState)
            {
                _listener.OnAudioChange();
            }

            public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
            {
            }
        }
    }
}