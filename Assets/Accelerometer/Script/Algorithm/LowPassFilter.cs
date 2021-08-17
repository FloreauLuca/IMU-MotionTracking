using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowPassFilter
{
    static public float ComputeRC(float newMeasureData, float prevEstimData, float dt, float RC)
    {
        float alpha = dt / (RC + dt);
        return alpha * newMeasureData + (1 - alpha) * prevEstimData;
    }

    static public Vector3 ComputeRC(Vector3 newMeasureData, Vector3 prevEstimData, float dt, float RC)
    {
        float alpha = dt / (RC + dt);
        return alpha * newMeasureData + (1 - alpha) * prevEstimData;
    }
}
