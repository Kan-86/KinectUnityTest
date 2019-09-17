using Microsoft.Kinect.VisualGestureBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class Paddle : MonoBehaviour
{
    public struct EventArgs
    {
        public string name;
        public float confidence;
        
        public EventArgs(string _name, float _confidence)
        {
            name = _name;
            confidence = _confidence;
        }
    }

    private readonly string databasePath = "test2.gbd";
    private string landingLeftGesture = "Move_Horizontally_Left_Left";
    private string landingRightGesture = "Move_Horizontally_Right_Right";
    private string hover = "Hover";
    private string landing = "Land";
    private string moveUpwards = "Move_Upwards";
    private string moveDownWards = "Move_Downwards";
    private List<int> land = new List<int>();
    private List<int> right = new List<int>();
    private List<int> left = new List<int>();
    private List<int> up = new List<int>();
    private List<int> down = new List<int>();
    private List<int> hov = new List<int>();

    public BodySourceManager _BodySource;
    private KinectSensor _Sensor;
    private VisualGestureBuilderFrameSource _Source;
    private VisualGestureBuilderFrameReader _Reader;
    private VisualGestureBuilderDatabase _Database;

    // Gesture Detection Events
    public delegate void GestureAction(EventArgs e);
    public event GestureAction OnGesture;

    // Use this for initialization
    void Start()
    {
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }

            // Set up Gesture Source
            _Source = VisualGestureBuilderFrameSource.Create(_Sensor, 0);

            // open the reader for the vgb frames
            _Reader = _Source.OpenReader();
            if (_Reader != null)
            {
                _Reader.IsPaused = true;
                _Reader.FrameArrived += GestureFrameArrived;
            }

            // load the ‘Squat’ gesture from the gesture database
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, databasePath);
            _Database = VisualGestureBuilderDatabase.Create(path);

            // Load all gestures
            IList<Gesture> gesturesList = _Database.AvailableGestures;

            for (int x = 0; x < gesturesList.Count; x++)
            {
                Gesture g = gesturesList[x];
                
                if (g.Name.Equals(landingLeftGesture))
                {
                    _Source.AddGesture(g);
                }
                if (g.Name.Equals(landingRightGesture))
                {
                    _Source.AddGesture(g);
                }
                if (g.Name.Equals(landing))
                {
                    _Source.AddGesture(g);
                }
                if (g.Name.Equals(moveUpwards))
                {
                    _Source.AddGesture(g);
                }
                if (g.Name.Equals(hover))
                {
                    _Source.AddGesture(g);
                }
                if (g.Name.Equals(moveDownWards))
                {
                    _Source.AddGesture(g);
                }
            }

            //for (int g = 0; g < gesturesList.Count; g++)
            // {
            // Gesture gesture = gesturesList[g];
            // _Source.AddGesture(gesture);
            //}

        }
    }

    // Public setter for Body ID to track
    public void SetBody(ulong id)
    {
        if (id > 0)
        {
            _Source.TrackingId = id;
            _Reader.IsPaused = false;
        }
        else
        {
            _Source.TrackingId = 0;
            _Reader.IsPaused = true;
        }
    }

    // Update Loop, set body if we need one
    void Update()
    {

        if (!_Source.IsTrackingIdValid)
        {
            //print (“found”);
            FindValidBody();
        }
    }

    // Check Body Manager, grab first valid body
    void FindValidBody()
    {

        if (_BodySource != null)
        {

            Body[] bodies = _BodySource.GetData();
            if (bodies != null)
            {
                foreach (Body body in bodies)
                {
                    if (body.IsTracked)
                    {
                        SetBody(body.TrackingId);
                        break;
                    }
                }
            }
        }

    }
    /// Handles gesture detection results arriving from the sensor for the associated body tracking Id
    private void GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
    {
        //Debug.Log (“GestureFrameArrived CALLED!”);
        VisualGestureBuilderFrameReference frameReference = e.FrameReference;
        using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
        {
            if (frame != null)
            {
                // get the discrete gesture results which arrived with the latest frame
                IDictionary<Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;

                if (discreteResults != null)
                {
                    foreach (Gesture gesture in _Source.Gestures)
                    {

                        if (gesture.GestureType == GestureType.Discrete)
                        {
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                               
                                if ((int)(result.Confidence * 100) > 55)
                                {
                                 
                                    if (gesture.Name.Equals(landingLeftGesture))
                                    {
                                        left.Add((int)(result.Confidence * 100));
                                     }
                                    if (gesture.Name.Equals(landingRightGesture))
                                    {
                                        right.Add((int)(result.Confidence * 100));
                                    }
                                    if (gesture.Name.Equals(landing))
                                    {
                                        land.Add((int)(result.Confidence * 100));
                                    }
                                    if (gesture.Name.Equals(moveUpwards))
                                    {
                                        up.Add((int)(result.Confidence * 100));
                                    }
                                    if (gesture.Name.Equals(moveDownWards))
                                    {
                                        down.Add((int)(result.Confidence * 100));
                                    }
                                    if (gesture.Name.Equals(hover))
                                    {
                                        hov.Add((int)(result.Confidence * 100));
                                    }
                                    // Fire Event
                                    //OnGesture (new EventArgs (gesture.Name, result.Confidence));
                                    Debug.Log("Detected Gesture " + gesture.Name + " with Confidence " + (int)(result.Confidence * 100));
                                }
                                }
                            else
                            {
                                Debug.Log("result is NULL@: " + result);
                            }
                        }
                    }
                }
            }
        }
    }
    void OnApplicationQuit()
    {
        int sumRight = 0 , sumLeft = 0, sumLand = 0, sumUP = 0, sumDown = 0, sumHov = 0;
        foreach (var item in right)
        {
           sumRight =  sumRight + item;
        }
        if ( right.Count != 0)
        Debug.Log("Average right: " + sumRight / right.Count);

        foreach (var item in left)
        {
            sumLeft = sumLeft + item;
        }
        if (left.Count != 0)
            Debug.Log("Average left: " + sumLeft / left.Count);

        foreach (var item in land)
        {
            sumLand = sumLand + item;
        }
        if (land.Count != 0)
            Debug.Log("Average land: " + sumLand / land.Count);
        foreach (var item in up)
        {
            sumUP = sumUP + item;
        }
        if (up.Count != 0)
            Debug.Log("Average up: " + sumUP / up.Count);

        foreach (var item in down)
        {
            sumDown = sumDown + item;
        }
        if (down.Count != 0)
            Debug.Log("Average down: " + sumDown / down.Count);

        foreach (var item in hov)
        {
            sumHov = sumHov + item;
        }
        if (down.Count != 0)
            Debug.Log("Average down: " + sumHov / hov.Count);


    }
}
