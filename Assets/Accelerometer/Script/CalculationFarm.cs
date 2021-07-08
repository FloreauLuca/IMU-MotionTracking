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

    public GlobalAccFrame currGlobalAccFrame;

    public ProcessAccFrame currProcessAccFrame;

    public ABerkFrame currABerkFrame;
    
    public KalmanFrame currKalmanFrame;

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
    void FixedUpdate()
    {
        
        if (useRunTimeData)
        {
            deltaTime = Time.fixedDeltaTime;
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
                time = readGraph.frames[frameIndex].dt;
                deltaTime = readGraph.frames[frameIndex].dt - readGraph.frames[frameIndex - 1].dt;
                currRawAccFrame.acceleration = readGraph.frames[frameIndex].acceleration;
                currRawAccFrame.gravity = readGraph.frames[frameIndex].gravity;
                currRawAccFrame.userAcceleration = readGraph.frames[frameIndex].userAcceleration;
            }
        }
        
        currRawAccFrame.dt = time;
        currGlobalAccFrame.dt = time;
        currKalmanFrame.dt = time;
        currProcessAccFrame.dt = time;
        currABerkFrame.dt = time;

        usedAcceleration = currGlobalAccFrame.globalAcc;
        currRawAccFrame.rawVelocity += currRawAccFrame.userAcceleration * deltaTime;
        currRawAccFrame.rawPosition += currRawAccFrame.rawVelocity * deltaTime;

        currGlobalAccFrame.globalVelocity += currGlobalAccFrame.globalAcc * deltaTime;
        currGlobalAccFrame.globalPos += currGlobalAccFrame.globalVelocity * deltaTime;

        //Remove init delta
        currProcessAccFrame.computeInitAcceleration = currRawAccFrame.acceleration - currRawAccFrame.gravity;
        //Remove Standard noise
        currProcessAccFrame.computeInitAcceleration = RemoveBaseNoise(currProcessAccFrame.computeInitAcceleration, 0.05f);
        currProcessAccFrame.computeInitVelocity += currProcessAccFrame.computeInitAcceleration;
        currProcessAccFrame.computeInitPosition += currProcessAccFrame.computeInitVelocity * deltaTime;

        currProcessAccFrame.computeResetAcceleration = currProcessAccFrame.computeInitAcceleration;
        currProcessAccFrame.computeResetVelocity += currProcessAccFrame.computeResetAcceleration;
        if (currProcessAccFrame.computeResetAcceleration == Vector3.zero && currProcessAccFrame.computeResetVelocity != Vector3.zero)
        {
            currProcessAccFrame.computeResetVelocity = Vector3.zero;
            //Debug.Log("[Calculation] Reset Velocity at : " + Time.time);
        }
        currProcessAccFrame.computeResetPosition += currProcessAccFrame.computeResetVelocity * deltaTime;
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
        currProcessAccFrame.computeInitVelocity = Vector3.zero;
        currProcessAccFrame.computeResetVelocity = Vector3.zero;
    }
}
