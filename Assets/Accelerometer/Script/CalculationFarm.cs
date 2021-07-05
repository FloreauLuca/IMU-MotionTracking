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
    public Vector3 rawAcceleration;
    public Vector3 userAcceleration;
    public Vector3 gravity;
    public Vector3 rawVelocity;
    public Vector3 computeInitAcceleration;
    public Vector3 computeInitVelocity;
    public Vector3 computeInitPosition;
    public Vector3 computeResetAcceleration;
    public Vector3 computeResetVelocity;
    public Vector3 computeResetPosition;
    
    public Vector3 aBerkAcceleration;
    public Vector3 aBerkVelocity;
    public Vector3 aBerkPosition;


    public KalmanFilterVector3 kalmanFilterRawAcc = new KalmanFilterVector3();
    public KalmanFilterVector3 kalmanFilterComputeAcc = new KalmanFilterVector3();
    public KalmanFilterVector3 kalmanFilterSpeed = new KalmanFilterVector3();
    public KalmanFilterVector3 kalmanFilterProcessSpeed = new KalmanFilterVector3();

    public Vector3 kalmanAcceleration;
    public Vector3 kalmanVelocity;
    public Vector3 kalmanComputeAcceleration;
    public Vector3 kalmanComputeVelocity;
    public Vector3 kalmanK;
    public Vector3 kalmanP;
    public Vector3 kalmanQ;
    public Vector3 kalmanR;

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
        if (resetCount)
        {
            frameIndex = 0;
            resetCount = false;
        }
        //Wait acceleration initialisation
        if (Input.acceleration == Vector3.zero) return;
        if (Input.acceleration != Vector3.zero && initAcceleration == Vector3.zero)
        {
            initAcceleration = Input.gyro.userAcceleration;
        }
        if (Input.accelerationEventCount != 1)
            Debug.LogError("Multiple Acceleration during the last frame");
        if (useRunTimeData)
        {
            time = Time.time;
            deltaTime = Time.fixedDeltaTime;
            rawAcceleration = Input.acceleration;
            userAcceleration = Input.gyro.userAcceleration;
            gravity = Input.gyro.gravity;
        }
        else
        {
            frameIndex++;
            if (frameIndex < readGraph.frames.Count)
            {
                time = readGraph.frames[frameIndex].dt;
                deltaTime = readGraph.frames[frameIndex].dt - readGraph.frames[frameIndex - 1].dt;
                rawAcceleration = readGraph.frames[frameIndex].acceleration;
                gravity = readGraph.frames[frameIndex].gravity;
                userAcceleration = readGraph.frames[frameIndex].userAcceleration;
            }
        }

        rawVelocity += userAcceleration * deltaTime;

        //Remove init delta
        computeInitAcceleration = rawAcceleration - gravity;
        //Remove Standard noise
        computeInitAcceleration = RemoveBaseNoise(computeInitAcceleration, 0.05f);
        computeInitVelocity += computeInitAcceleration; 
        computeInitPosition += computeInitVelocity * deltaTime;

        computeResetAcceleration = computeInitAcceleration;
        computeResetVelocity += computeResetAcceleration;
        if (computeResetAcceleration == Vector3.zero && computeResetVelocity != Vector3.zero)
        {
            computeResetVelocity = Vector3.zero;
            //Debug.Log("[Calculation] Reset Velocity at : " + Time.time);
        }
        computeResetPosition += computeResetVelocity * deltaTime;
    }

    Vector3 RemoveBaseNoise(Vector3 vec, float minValue)
    {
        Vector3 computeVec = Vector3.zero;
        if (Mathf.Abs(computeInitAcceleration.x) > minValue)
            computeVec.x = computeInitAcceleration.x;
        if (Mathf.Abs(computeInitAcceleration.y) > minValue)
            computeVec.y = computeInitAcceleration.y;
        if (Mathf.Abs(computeInitAcceleration.z) > minValue)
            computeVec.z = computeInitAcceleration.z;

        return computeVec;
    }


    public void ResetVelocity()
    {
        rawVelocity = Vector3.zero;
        computeInitVelocity = Vector3.zero;
        computeResetVelocity = Vector3.zero;
    }
}
