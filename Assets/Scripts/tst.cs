using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class tst : MonoBehaviour
{
    private KinectSensor _Sensor;
    private BodyFrameReader _Reader;
    private Body[] _Data = null;
    private ulong _trackingId = 0;

    public DetectJoins GestureManager;

    public Body[] GetData()
    {
        return _Data;
    }

    void Start()
    {
        _Sensor = KinectSensor.GetDefault();

        if (_Sensor != null)
        {
            _Reader = _Sensor.BodyFrameSource.OpenReader();
            
            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }
    }

    void Update()
    {
        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                if (_Data == null)
                {
                    _Data = new Body[_Sensor.BodyFrameSource.BodyCount];
                }

                frame.GetAndRefreshBodyData(_Data);

                frame.Dispose();
                frame = null;

                foreach (var body in _Data)
                {
                    if (body != null && body.IsTracked)
                    {
                        if(_trackingId != body.TrackingId) { 
                        _trackingId = body.TrackingId;
                        if (GestureManager != null)
                        {
                            GestureManager.SetTrackingId(body.TrackingId);
                        }
                        }
                        break;
                    }
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }
}
