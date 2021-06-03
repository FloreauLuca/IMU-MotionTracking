using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//https://forum.unity.com/threads/input-acceleration-calibration.317121/
public class CalibratedAccelerometer : MonoBehaviour
{
    [SerializeField] public Vector3 speed = Vector3.zero;
    [SerializeField] private float speedModifier = 0.001f;
    [SerializeField] private Quaternion calibrationQuaternion;

    // Used to calibrate the Input.acceleration
    void CalibrateAccelerometer()
    {
        Vector3 accelerationSnapshot = Input.acceleration;

        Quaternion rotateQuaternion = Quaternion.FromToRotation(
            new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);

        calibrationQuaternion = Quaternion.Inverse(rotateQuaternion);
    }

    void Start()
    {
        CalibrateAccelerometer();
    }

    void Update()
    {
        Vector3 theAcceleration = Input.acceleration;
        Vector3 fixedAcceleration = calibrationQuaternion * theAcceleration;
        //Debug.Log("[Calibrated] fixedAcceleration : " + Input.acceleration.ToString("#.000"));
        // Use fixedAcceleration for any logic that follows
        speed += fixedAcceleration - Input.gyro.gravity - Input.acceleration;
        transform.position += speed * speedModifier;

    }

}
