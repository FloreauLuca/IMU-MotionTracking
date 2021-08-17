using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CalculationFarm : MonoBehaviour
{
    [SerializeField] private bool useRunTimeData;
    [SerializeField] private string fileName;
    private RawGraph readGraph;
    private int frameIndex = 0;
    [SerializeField] private bool resetCount;

    //Current Frame data
    public float deltaTime;
    public float time;

    public Vector3 initAcceleration;
    public Vector3 usedAcceleration;

    public RawAccFrame currRawAccFrame;

    public RawGyrFrame currRawGyrFrame;

    public GlobalAccFrame currGlobalAccFrame;

    public ProcessAccFrame currProcessAccFrame;

    public ABerkFrame currABerkFrame;
    
    public KalmanFrame currKalmanFrame;

    public RCFrame currRCFrame;

    void ReadFile(string path)
    {
        string json = "";
        FileStream fileStream = new FileStream(path, FileMode.Open);
        using (StreamReader reader = new StreamReader(fileStream))
        {
            string text = reader.ReadLine();
            while (text != null)
            {
                json += text;
                json += '\n';
                text = reader.ReadLine();
            }
        }
        readGraph = JsonUtility.FromJson<RawGraph>(json);
    }

    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;
        if (!useRunTimeData)
        {
            ReadFile("Assets/Graph/SavedGraph/" + fileName + ".graph");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (useRunTimeData)
        {
            deltaTime = Time.deltaTime;
            time = Time.time;
            //Wait acceleration initialisation
            if (Input.acceleration == Vector3.zero) return;
            if (Input.acceleration != Vector3.zero && initAcceleration == Vector3.zero)
            {
                initAcceleration = Input.gyro.userAcceleration;
            }
            if (Input.accelerationEventCount != 1)
                Debug.LogError("Multiple Acceleration during the last frame");
            currRawAccFrame.acceleration = Input.acceleration;
            currRawAccFrame.userAcceleration = Input.gyro.userAcceleration;
            currRawAccFrame.gravity = Input.gyro.gravity;

            currRawGyrFrame.attitude = Input.gyro.attitude;
            currRawGyrFrame.rotationRate = Input.gyro.rotationRate;
            currRawGyrFrame.rotationRateUnbiased = Input.gyro.rotationRateUnbiased;
        }
        else
        {
            if (resetCount)
            {
                frameIndex = 0;
                resetCount = false;
            }
            initAcceleration = readGraph.frames[0].userAcceleration;
            frameIndex++;
            if (frameIndex < readGraph.frames.Count)
            {
                time = readGraph.frames[frameIndex].time;
                deltaTime = readGraph.frames[frameIndex].time - readGraph.frames[frameIndex - 1].time;
                currRawAccFrame.acceleration = readGraph.frames[frameIndex].acceleration;
                currRawAccFrame.gravity = readGraph.frames[frameIndex].gravity;
                currRawAccFrame.userAcceleration = readGraph.frames[frameIndex].userAcceleration;
            }
            else
            {
                ResetVelocity();
                resetCount = true;
            }
        }
        
        currRCFrame.time = time;
        currRawAccFrame.time = time;
        currRawGyrFrame.time = time;
        currGlobalAccFrame.time = time;
        currKalmanFrame.time = time;
        currProcessAccFrame.time = time;
        currABerkFrame.time = time;

        //usedAcceleration = currRawAccFrame.userAcceleration;
        usedAcceleration = currGlobalAccFrame.globalAcc;
        currRawAccFrame.rawVelocity += currRawAccFrame.userAcceleration * deltaTime;
        currRawAccFrame.rawPosition += currRawAccFrame.rawVelocity * deltaTime;

        currRawGyrFrame.angle += currRawGyrFrame.rotationRate * deltaTime;
        currRawGyrFrame.angleUnbiased += currRawGyrFrame.rotationRateUnbiased * deltaTime;

        currGlobalAccFrame.globalVelocity += currGlobalAccFrame.globalAcc * deltaTime;
        currGlobalAccFrame.globalPos += currGlobalAccFrame.globalVelocity * deltaTime;

        ////Remove init delta
        currProcessAccFrame.computeInitAcceleration = currRawAccFrame.acceleration - currRawAccFrame.gravity;
        //Remove Standard noise
        currProcessAccFrame.computeInitAcceleration = RemoveBaseNoise(currProcessAccFrame.computeInitAcceleration, 0.1f);
        currProcessAccFrame.computeInitVelocity += currProcessAccFrame.computeInitAcceleration;
        currProcessAccFrame.computeInitPosition += currProcessAccFrame.computeInitVelocity * deltaTime;

        currProcessAccFrame.computeInitVelocity += currProcessAccFrame.computeInitAcceleration;
        if (currProcessAccFrame.computeInitAcceleration == Vector3.zero)
        {
            currProcessAccFrame.computeInitVelocity = Vector3.zero;
            //Debug.Log("[Calculation] Reset Velocity at : " + Time.time);
        }
        currProcessAccFrame.computeInitPosition += currProcessAccFrame.computeInitVelocity * deltaTime;
    }

    Vector3 RemoveBaseNoise(Vector3 vec, float minValue)
    {
        Vector3 computeVec = Vector3.zero;
        if (Mathf.Abs(currProcessAccFrame.computeInitAcceleration.x) > minValue)
            computeVec.x = currProcessAccFrame.computeInitAcceleration.x;
        if (Mathf.Abs(currProcessAccFrame.computeInitAcceleration.y) > minValue)
            computeVec.y = currProcessAccFrame.computeInitAcceleration.y;
        if (Mathf.Abs(currProcessAccFrame.computeInitAcceleration.z) > minValue)
            computeVec.z = currProcessAccFrame.computeInitAcceleration.z;

        return computeVec;
    }


    public void ResetVelocity()
    {
        currRawAccFrame.rawVelocity = Vector3.zero;
        currRawAccFrame.rawPosition = Vector3.zero;
        currProcessAccFrame.computeInitVelocity = Vector3.zero;
        currProcessAccFrame.computeResetVelocity = Vector3.zero;
        currGlobalAccFrame.globalAcc = Vector3.zero;
        currGlobalAccFrame.globalVelocity = Vector3.zero;
        currGlobalAccFrame.globalPos = Vector3.zero;
        FindObjectOfType<RCPassTester>().Reset();
        FindObjectOfType<AccelerometerAddedToKalmanFilter>().ResetFilter();
        foreach (KalmanData kalman in FindObjectsOfType<KalmanData>())
        {
            kalman.ResetFilter();
        }

        foreach (RCData rcdata in FindObjectsOfType<RCData>())
        {
            rcdata.Reset();
        }

        foreach (ComputeData compute in FindObjectsOfType<ComputeData>())
        {
            compute.Reset();
        }

    }
}
