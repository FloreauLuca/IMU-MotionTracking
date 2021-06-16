using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccCallibrationJL : MonoBehaviour
{
    public Vector3 rawAcc;
    public Vector3 computeAcc;
    public Vector3 biasAcc;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //Remove Wihte Noise
    void RemoveWhiteNoise()
    {

    }

    //Remove Gravity
    void RemoveGravity()
    {
        computeAcc = rawAcc + Input.gyro.gravity;
    }

    //Remove Bias
    void RemoveBias()
    {
        computeAcc -= biasAcc;
    }
}
