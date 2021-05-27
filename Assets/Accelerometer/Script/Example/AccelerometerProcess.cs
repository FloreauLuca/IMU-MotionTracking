using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerometerProcess : MonoBehaviour
{
    //The lower this value, the less smooth the value is and faster Accel is updated. 30 seems fine for this
    const float updateSpeed = 30.0f;

    float AccelerometerUpdateInterval = 1.0f / updateSpeed;
    float LowPassKernelWidthInSeconds = 1.0f;
    float LowPassFilterFactor = 0;
    Vector3 lowPassValue = Vector3.zero;

    void Start()
    {
        //Filter Accelerometer
        LowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds;
        lowPassValue = Input.acceleration;
    }

    void Update()
    {

        //Get Raw Accelerometer values (pass in false to get raw Accelerometer values)
        Vector3 rawAccelValue = filterAccelValue(false);
        //Debug.Log("[AccelerometerProcess] RAW X: " + rawAccelValue.x + "  Y: " + rawAccelValue.y + "  Z: " + rawAccelValue.z);

        //Get smoothed Accelerometer values (pass in true to get Filtered Accelerometer values)
        Vector3 filteredAccelValue = filterAccelValue(true);
        //Debug.Log("[AccelerometerProcess] FILTERED X: " + filteredAccelValue.x + "  Y: " + filteredAccelValue.y + "  Z: " + filteredAccelValue.z);
    }

    //Filter Accelerometer
    Vector3 filterAccelValue(bool smooth)
    {
        if (smooth)
            lowPassValue = Vector3.Lerp(lowPassValue, Input.acceleration, LowPassFilterFactor);
        else
            lowPassValue = Input.acceleration;

        return lowPassValue;
    }
}
