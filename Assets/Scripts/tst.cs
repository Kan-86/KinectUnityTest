using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class tst : MonoBehaviour
{
    private KinectSensor _sensor;
    private BodyFrameReader _bodyFrameReader;
    private Body[] _bodies = null;
    Joint lefthand;
    Joint righthand;
    CameraSpacePoint lefthandPostion;
    CameraSpacePoint righthandPostion;
    ParticleSystem _ps;

    public GameObject AttachedObject;
    private bool IsAvailable;

    public Body[] GetBodies()
    {
        return _bodies;
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
        _sensor = KinectSensor.GetDefault();
        if (_sensor != null)
        {
            IsAvailable = _sensor.IsAvailable;

            _bodyFrameReader = _sensor.BodyFrameSource.OpenReader();

            if (!_sensor.IsOpen)
            {
                _sensor.Open();
            }

            _bodies = new Body[_sensor.BodyFrameSource.BodyCount];
        }
    }

    // Update is called once per frame
    void Update()
    {
        IsAvailable = _sensor.IsAvailable;
         if ( IsAvailable)
        {
        if (_bodyFrameReader != null)
        {
            var frame = _bodyFrameReader.AcquireLatestFrame();

            if (frame != null)
            {
                frame.GetAndRefreshBodyData(_bodies);

                    foreach (var body in _bodies.Where(b => b.IsTracked))
                    {
                        if (body != null)
                        {
                            if (body.IsTracked)
                            {
                                // Find the joints
                                 righthand = body.Joints[JointType.HandRight]; 
                                 lefthand = body.Joints[JointType.HandLeft];
                                 
                                Joint head = body.Joints[JointType.Head];
                                if (MoveUpwards(body))
                                {

                                    if (_ps != null)
                                    {
                                        _ps.emissionRate = 4;
                                        _ps.startColor = Color.red;
                                    }
                                }
                                else if (Hover(body))
                                {

                                    if (_ps != null)
                                    {
                                        _ps.emissionRate = 4;
                                        _ps.startColor = Color.yellow;
                                    }
                                }

                               
                            }
                        }
                    }
                    frame.Dispose();
                frame = null;
            }
        }
        }
    }

    static float RescalingToRangesB(float scaleAStart, float scaleAEnd, float scaleBStart, float scaleBEnd, float valueA)
    {
        return (((valueA - scaleAStart) * (scaleBEnd - scaleBStart)) / (scaleAEnd - scaleAStart)) + scaleBStart;
    }
    float tresHandX;
    float tresHandy;
    private bool Hover(Body body) 
    {
        
        var shoulderRight = body.Joints[JointType.ShoulderRight].Position;
        var shoulderLeft = body.Joints[JointType.ShoulderLeft].Position;
        var ElbowRight = body.Joints[JointType.ElbowRight].Position;
        var ElbowLeft= body.Joints[JointType.ElbowLeft].Position;
        float angleRightSghould = AngleBetweenVector2(new Vector2(shoulderRight.X, shoulderRight.Y),
                new Vector2(ElbowRight.X, ElbowRight.Y));
        float angleLeftghould = AngleBetweenVector2(new Vector2(shoulderLeft.X, shoulderLeft.Y),
                new Vector2(ElbowLeft.X, ElbowLeft.Y));
        if (angleRightSghould > -15 &&
            angleRightSghould < 15  &&
            angleLeftghould > 165 &&
            angleLeftghould < 195
         )
        {
           
            return true;
        }
        return false;
    }
    float trestLeftAngle;
    float trestRightAngle;
    bool allowToMoveUp = true;
    bool allowToMoveDown= true;
    private bool MoveUpwards (Body body)
    {
        var shoulderRight = body.Joints[JointType.ShoulderRight].Position;
        var shoulderLeft = body.Joints[JointType.ShoulderLeft].Position;
        var ElbowRight = body.Joints[JointType.ElbowRight].Position;
        var ElbowLeft = body.Joints[JointType.ElbowLeft].Position;
        var handRight = body.Joints[JointType.HandRight].Position;
        var handLeft = body.Joints[JointType.HandLeft].Position;
        float angleRightSghould = AngleBetweenVector2(new Vector2(shoulderRight.X, shoulderRight.Y),
                new Vector2(ElbowRight.X, ElbowRight.Y));
        float angleLeftghould = AngleBetweenVector2(new Vector2(shoulderLeft.X, shoulderLeft.Y),
                new Vector2(ElbowLeft.X, ElbowLeft.Y));
        if (angleRightSghould > -15 &&
            angleRightSghould < 90 &&
            angleLeftghould > 90 &&
            angleLeftghould < 195 
         )
        {
            if (angleRightSghould > trestRightAngle &&
               angleLeftghould > trestLeftAngle &&
               (allowToMoveUp || (ElbowRight.X - ElbowRight.X/20 < handRight.X + handRight.X/20 
               && ElbowLeft.X - ElbowLeft.X / 20 < handLeft.X + handLeft.X / 20)))
            {
                Debug.Log("up" + true);
                allowToMoveDown = false;
                allowToMoveUp = true;
                return true;
            }
            else if (
                angleRightSghould < trestRightAngle &&
               angleLeftghould < trestLeftAngle &&
               (allowToMoveDown || (trestLeftAngle > 105 && trestRightAngle > 75)))
            {

                Debug.Log("down" + true);
                allowToMoveUp = false;
                allowToMoveDown = true;
                return true;
            }
            return false;
        }
        return false;
    }
    private float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
    {
        Vector2 diference = vec2 - vec1;
        float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
        return Vector2.Angle(Vector2.right, diference) * sign;
    }
    void OnApplicationQuit()
    {
        if (_bodyFrameReader != null)
        {
            _bodyFrameReader.IsPaused = true;
            _bodyFrameReader.Dispose();
            _bodyFrameReader = null;
        }

        if (_sensor != null)
        {
            if (_sensor.IsOpen)
            {
                _sensor.Close();
            }

            _sensor = null;
        }
    }
}
