using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class NaiveMovingTest : MonoBehaviour
{
    [SerializeField] public Vector3 speed = Vector3.zero;
    [SerializeField] private Vector3 rawAcceleration = Vector3.zero;
    [SerializeField] private Vector3 acceleration = Vector3.zero;
    [SerializeField] private Vector3 initialAcceleration = Vector3.zero;
    [SerializeField] private Vector3 gravity = Vector3.zero;
    [SerializeField] private Vector3 initialGravity = Vector3.zero;
    [SerializeField] private Vector3 initialRotation = Vector3.zero;
    [SerializeField] private Quaternion initialRotationQuat;
    [SerializeField] private Vector3 rawRotation = Vector3.zero;
    [SerializeField] private Vector3 rotation = Vector3.zero;
    [SerializeField] private Quaternion rotationQuat;
    [SerializeField] private Quaternion calibrationQuaternion;
    private bool started = false;
    [SerializeField] private float speedModifier = 0.001f;

    public static Vector3 RoundVector3(Vector3 vec, int decimals)
    {
        float pow = Mathf.Pow(10, decimals);
        return new Vector3(Mathf.Round(vec.x * pow) / pow,
            Mathf.Round(vec.y * pow) / pow,
            Mathf.Round(vec.z * pow) / pow);
    }

    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.acceleration == Vector3.zero)
        {
            started = true;
            initialRotation = Input.gyro.attitude.eulerAngles;
            initialRotation = new Vector3(initialRotation.x, initialRotation.z, initialRotation.y);
            initialRotationQuat = Input.gyro.attitude;
            initialGravity = Input.gyro.gravity;
            CalibrateAccelerometer();
            return;
        }
        else if (initialAcceleration == Vector3.zero)
        {
            initialAcceleration = Input.acceleration;
        }
        gravity = Input.gyro.gravity;

        rawAcceleration = Input.acceleration;
        Vector3 theAcceleration = Input.acceleration;
        Vector3 fixedAcceleration = calibrationQuaternion * theAcceleration;
        acceleration = fixedAcceleration;

        acceleration = Input.acceleration;
        acceleration -= initialAcceleration;
        //acceleration = RoundVector3(acceleration, 5);
        speed += acceleration;
        transform.position += speed * speedModifier;

        rawRotation = Input.gyro.attitude.eulerAngles;
        rotationQuat = Input.gyro.attitude;
        rotation = new Vector3(rawRotation.x, -rawRotation.z, rawRotation.y);
        //transform.rotation = Quaternion.Euler(rotation);
        //transform.rotation = rotationQuat;
    }
    // Used to calibrate the Input.acceleration
    void CalibrateAccelerometer()
    {
        Vector3 accelerationSnapshot = Input.acceleration;

        Quaternion rotateQuaternion = Quaternion.FromToRotation(
            new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);

        calibrationQuaternion = Quaternion.Inverse(rotateQuaternion);
    }
}
