using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationVisualization : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Input.gyro.userAcceleration * 5;
    }
}
