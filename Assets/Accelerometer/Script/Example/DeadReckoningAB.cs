using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://os.mbed.com/users/aberk/notebook/dead-reckoning/

public class DeadReckoningAB : MonoBehaviour
{
    [SerializeField] int CALIBRATION_COUNT = 5;
    [SerializeField] int SAMPLE_COUNT = 2;
    [SerializeField] int NO_MOVEMENT = 2;
    [SerializeField] Vector3 WINDOW_MIN_POS = Vector3.one * 0.005f;
    [SerializeField] Vector3 WINDOW_MIN_NEG = Vector3.one * -0.005f;
    [SerializeField] Vector3 GAIN = Vector3.one;
    private CalculationFarm calculationFarm;

    private Vector3 accumulator= Vector3.zero;
    private Vector3 accelerometerReadings = Vector3.zero;
    private Vector3 bias = Vector3.zero;
    private Vector3 acceleration = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 position = Vector3.zero;

    private Vector3 prevAcc = Vector3.zero;
    private Vector3 prevVel = Vector3.zero;
    private Vector3 prevPos = Vector3.zero;

    private int calibrationCount = 0;
    private int sampleCount = 0;
    private int noAccelerationCount = 0;

    private bool biasCalculated;
    // Start is called before the first frame update
    void Start()
    {
        calculationFarm = FindObjectOfType<CalculationFarm>();
        accumulator = Vector3.zero;
        sampleCount = 0;
        calibrationCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!biasCalculated)
        {
            CalculateBias();
            return;
        }
        //SampleAcceleration();
        acceleration = calculationFarm.rawAcceleration;
        acceleration -= bias;
        RemoveWindowDiscrimination();
        DoubleIntegration();
        CheckingEndMovement();

        prevAcc = acceleration;
        prevVel = velocity;
        prevPos = position;
        calculationFarm.aBerkAcceleration = acceleration;
        calculationFarm.aBerkVelocity = velocity;
        calculationFarm.aBerkPosition = position;
    }

    void CalculateBias()
    {
        if (calibrationCount < CALIBRATION_COUNT)
        {
            accelerometerReadings = calculationFarm.rawAcceleration;
            accumulator += accelerometerReadings;
            calibrationCount++;
        }
        else
        {
            bias = accumulator / CALIBRATION_COUNT;
            accumulator = Vector3.zero;
            sampleCount = 0;
            biasCalculated = true;
        }
    }

    void SampleAcceleration()
    {
        if (sampleCount < SAMPLE_COUNT)
        {
            accelerometerReadings = calculationFarm.rawAcceleration;
            accumulator += accelerometerReadings;
            sampleCount++;
        }
        else
        {
            acceleration = accumulator / sampleCount;
            accumulator = Vector3.zero;
            sampleCount = 0;
            biasCalculated = true;
        }
    }

    void RemoveWindowDiscrimination()
    {
        //There will be a certain amount of noise around 0 which would give
        //false acceleration readings. If the acceleration value falls within
        //a certain band, we can assume it's just noise and set the actual
        //acceleration to 0.
        if (acceleration.x >= WINDOW_MIN_NEG.x && acceleration.x <= WINDOW_MIN_POS.x)
        {
            acceleration.x = 0;
        }
        //If we're sure we're seeing actual acceleration, then we'll convert it
        //to m/s^2.
        else
        {
            acceleration.x *= GAIN.x;
        }
        if (acceleration.y >= WINDOW_MIN_NEG.y && acceleration.y <= WINDOW_MIN_POS.y)
        {
            acceleration.y = 0;
        }
        else
        {
            acceleration.y *= GAIN.y;
        }
        if (acceleration.z >= WINDOW_MIN_NEG.z && acceleration.z <= WINDOW_MIN_POS.z)
        {
            acceleration.z = 0;
        }
        else
        {
            acceleration.z *= GAIN.z;
        }
    }

    void DoubleIntegration()
    {
        //First x-axis integration.
        velocity = prevVel +
                    (prevAcc +
                     ((acceleration - prevAcc) / 2.0f)) * Time.deltaTime;

        //Second x-axis integration.
        position = prevPos +
                    (prevVel +
                     ((velocity - prevVel) / 2.0f)) * Time.deltaTime;
    }

    void CheckingEndMovement()
    {
        //If we've observed no acceleration in the past N readings, we can assume
        //movement has stopped, and can reset the velocity variables, greatly
        //reducing position errors.            
        if (acceleration == Vector3.zero)
        {
            noAccelerationCount++;
        }
        else
        {
            noAccelerationCount = 0;
        }

        if (noAccelerationCount > NO_MOVEMENT)
        {
            velocity = Vector3.zero;
            prevVel = Vector3.zero;
            noAccelerationCount = 0;
        }
    }
}
