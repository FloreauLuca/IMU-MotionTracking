using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighPassFilter
{
    static public float ComputeRC(float newMeasureData, float prevMeasureData, float prevEstimData, float dt, float RC)
    {
        float alpha = RC / (RC + dt);
        return alpha * prevEstimData + alpha * (newMeasureData - prevMeasureData);
    }

    static public Vector3 ComputeRC(Vector3 newMeasureData, Vector3 prevMeasureData, Vector3 prevEstimData, float dt, float RC)
    {
        float alpha = RC / (RC + dt);
        return alpha * prevEstimData + alpha * (newMeasureData - prevMeasureData);
    }
    
}