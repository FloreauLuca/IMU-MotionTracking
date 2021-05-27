using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerometerRaw : MonoBehaviour
{
    void Update()
    {
        Vector3 accelerationRaw = Input.gyro.userAcceleration;
        transform.Translate(accelerationRaw);
    }
}
