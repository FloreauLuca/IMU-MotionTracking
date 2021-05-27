using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerometerAddedToKalmanFilter : MonoBehaviour
{
    private KalmanFilterVector3 kalman;

    private Vector3 speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        speed += Input.acceleration - Input.gyro.gravity;
        speed = kalman.Update(speed);
        transform.position += speed;
    }
}
