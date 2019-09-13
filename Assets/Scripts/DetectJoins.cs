using Microsoft.Kinect.VisualGestureBuilder;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Windows.Kinect;


public class DetectJoins : MonoBehaviour
{
    //public GameObject BodySrcManager;
    //public JointType TrackedJoint;
    //private BodySourceManager bodyManager;
    //private Body[] bodies;
    //public float multiplier = 6f;
    //private string dataBasePath;

    public Gesture gesture;
    private readonly string leanDB = "kinectHandSignland.gbd";
    private string landingLeftGesture = "Landing_Left";
    private string landingRightGesture = "Landing_Right";

    private VisualGestureBuilderFrameSource vgbFrameSource = null;
    private VisualGestureBuilderFrameReader _Reader;
    KinectSensor sensor = null;


    VisualGestureBuilderDatabase _gestureDatabase;
    VisualGestureBuilderFrameSource _gestureFrameSource;
    VisualGestureBuilderFrameReader _gestureFrameReader;
    KinectSensor _kinect;
    Gesture _salute;
    Gesture _saluteProgress;
    ParticleSystem _ps;
    public GameObject AttachedObject;

    public void SetTrackingId(ulong id)
    {
        _gestureFrameReader.IsPaused = false;
        _gestureFrameSource.TrackingId = id;
        _gestureFrameReader.FrameArrived += _gestureFrameReader_FrameArrived;
    }

    // Use this for initialization
    void Start()
    {
        if (AttachedObject != null)
        {
            _ps = AttachedObject.GetComponent<ParticleSystem>();
            _ps.emissionRate = 4;
            _ps.startColor = Color.blue;
        }
        _kinect = KinectSensor.GetDefault();

        _gestureDatabase = VisualGestureBuilderDatabase.Create(Path.Combine(Application.streamingAssetsPath, leanDB));
        _gestureFrameSource = VisualGestureBuilderFrameSource.Create(_kinect, 0);

        foreach (var gesture in _gestureDatabase.AvailableGestures)
        {
            _gestureFrameSource.AddGesture(gesture);

            if (gesture.Name == landingLeftGesture)
            {
                _salute = gesture;
            }
            if (gesture.Name == landingRightGesture)
            {
                _saluteProgress = gesture;
            }
        }

        _gestureFrameReader = _gestureFrameSource.OpenReader();
        _gestureFrameReader.IsPaused = true;
    }

    void _gestureFrameReader_FrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
    {
        VisualGestureBuilderFrameReference frameReference = e.FrameReference;
        using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
        {
            if (frame != null && frame.DiscreteGestureResults != null)
            {
                if (AttachedObject == null)
                    return;

                DiscreteGestureResult result = null;

                if (frame.DiscreteGestureResults.Count > 0)
                    result = frame.DiscreteGestureResults[_salute];
                if (result == null)
                    return;

                if (result.Detected == true)
                {
                    var progressResult = frame.ContinuousGestureResults[_saluteProgress];
                    if (AttachedObject != null)
                    {
                        var prog = progressResult.Progress;
                        float scale = 0.5f + prog * 3.0f;
                        AttachedObject.transform.localScale = new Vector3(scale, scale, scale);
                        if (_ps != null)
                        {
                            _ps.emissionRate = 100 * prog;
                            _ps.startColor = Color.red;
                        }
                    }
                }
                else
                {
                    if (_ps != null)
                    {
                        _ps.emissionRate = 4;
                        _ps.startColor = Color.blue;
                    }
                }
            }
        }
    }









    /**************************************________OLD CODE!!!!_________******************************************/











    // Start is called before the first frame update
    //void Start()
    //{
    //    sensor = KinectSensor.GetDefault();
    //    if (sensor != null)
    //    {
    //        if (!sensor.IsOpen)
    //        {
    //            sensor.Open();
    //        }

    //        vgbFrameSource = VisualGestureBuilderFrameSource.Create(sensor, 0);
    //        _Reader = vgbFrameSource.OpenReader();

    //    if (_Reader != null)
    //    {
    //        _Reader.IsPaused = true;
    //        Debug.Log("vgbFrameReader is paused");
    //    }
    //    var databasePath = Path.Combine(Application.streamingAssetsPath, leanDB);
    //    using (VisualGestureBuilderDatabase database = VisualGestureBuilderDatabase.Create(databasePath))
    //    {
    //        foreach (Gesture gesture in database.AvailableGestures)
    //        {
    //            if (gesture.Name.Equals(gestureName))
    //            {
    //                this.vgbFrameSource.AddGesture(gesture);
    //                    Debug.Log("What is gesture: " + gesture);
    //            }
    //        }
    //    }
    //    }
    //    //dataBasePath = Path.Combine(Application.streamingAssetsPath, leanDB);

    //    //Debug.Log("What is the path: " + dataBasePath);

    //    //if (BodySrcManager == null)
    //    //{
    //    //    Debug.Log("Assign Game Object with Body Source Manager");
    //    //}
    //    //else
    //    //{
    //    //    bodyManager = BodySrcManager.GetComponent<BodySourceManager>();
    //    //}


    //}

    //// Update is called once per frame
    //void Update()
    //{

    //    //Debug.Log(vgbFrameSource.Gestures.Count);
    //    //if (bodyManager == null)
    //    //{
    //    //    return;
    //    //}
    //    //bodies = bodyManager.GetData();
    //    //if (bodies == null)
    //    //{
    //    //    return;
    //    //}
    //    //else
    //    //{
    //    //    foreach (var body in bodies)
    //    //    {
    //    //        if (body == null)
    //    //        {
    //    //            continue;
    //    //        }
    //    //        if (body.IsTracked)
    //    //        {
    //    //            var pos = body.Joints[TrackedJoint].Position;
    //    //            gameObject.transform.position = new Vector3(pos.X * multiplier, pos.Y * multiplier);
    //    //        }
    //    //    }
    //    //}
    //}
}
