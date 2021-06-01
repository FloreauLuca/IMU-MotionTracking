using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class RawAccFrame : Frame
{
   public Vector3 acceleration;
   public Vector3 gravity;
   public Vector3 userAcceleration;
   public Vector3 rawVelocity;
}
[Serializable]
public class ProcessAccFrame : Frame
{
    public Vector3 computeAccelerationGrav;
    public Vector3 computeAccelerationInit;
    public Vector3 velocityprocessInit;
}
[Serializable]
public class KalmanFrame : Frame
{
    public Vector3 kalmanRawAcc;
    public Vector3 kalmanComputeAcc;
    public Vector3 kalmanSpeed;
    public Vector3 kalmanProcessSpeed;
}

[Serializable]
public class RawGraph
{
    public List<RawAccFrame> frames = new List<RawAccFrame>();
}

[Serializable]
public class ProcessGraph
{
    public List<ProcessAccFrame> frames = new List<ProcessAccFrame>();
}

[Serializable]
public class KalmanGraph
{
    public List<KalmanFrame> frames = new List<KalmanFrame>();
}

public class InputViewer : MonoBehaviour
{
    public RawGraph rawGraph;
    public ProcessGraph processGraph;
    public KalmanGraph kalmanGraph;


    private Vector3 velocity;
    private Vector3 kalmanVelocity;
    private Vector3 velocityprocessInit;
    private Vector3 acceleration;


    private KalmanFilterVector3 kalmanFilterRawAcc = new KalmanFilterVector3();
    private KalmanFilterVector3 kalmanFilterComputeAcc = new KalmanFilterVector3();
    private KalmanFilterVector3 kalmanFilterSpeed = new KalmanFilterVector3();
    private KalmanFilterVector3 kalmanFilterProcessSpeed = new KalmanFilterVector3();

    [SerializeField] private bool gyro = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.acceleration == Vector3.zero) return;
        if (Input.acceleration != Vector3.zero && acceleration == Vector3.zero)
        {
            acceleration = Input.acceleration;
        }
        RawAccFrame rawAccFrame = new RawAccFrame();
        ProcessAccFrame processAccFrame = new ProcessAccFrame();
        KalmanFrame kalmanFrame = new KalmanFrame();
        rawAccFrame.dt = Time.time;
        processAccFrame.dt = Time.time;
        kalmanFrame.dt = Time.time;

        rawAccFrame.acceleration = Input.acceleration;
        rawAccFrame.gravity = Input.gyro.gravity;
        rawAccFrame.userAcceleration = Input.gyro.userAcceleration;
        velocity += rawAccFrame.userAcceleration;
        rawAccFrame.rawVelocity = velocity;

        processAccFrame.computeAccelerationGrav = (Input.acceleration - Input.gyro.gravity);
        processAccFrame.computeAccelerationInit = (Input.acceleration - acceleration);
        if (processAccFrame.computeAccelerationInit.x > 0.004f ||processAccFrame.computeAccelerationInit.y > 0.004f || processAccFrame.computeAccelerationInit.z > 0.004f )
        {
            velocityprocessInit += processAccFrame.computeAccelerationInit;
        }

        processAccFrame.velocityprocessInit = velocityprocessInit;
            
        Debug.Log("[InputViewer] Input.acceleration : " + Input.acceleration.ToString("#.000"));
        Debug.Log("[InputViewer] Input.accelerationEventCount : " + Input.accelerationEventCount);
        Debug.Log("[InputViewer] Input.gyro.gravity : " + Input.gyro.gravity.ToString("#.000"));
        Debug.Log("[InputViewer] process acceleration : " + (Input.acceleration - acceleration).ToString("#.000"));
        string log = "[InputViewer] Input.accelerationEvents : ";
        for (int i = 0; i < Input.accelerationEventCount; i++)
        {
            log += Input.accelerationEvents[i].acceleration.ToString("#.000") + "\n";
        }
        //Debug.Log(log);

        if (gyro)
        {
            Debug.Log("[InputViewer] Input.gyro.userAcceleration : " + Input.gyro.userAcceleration.ToString("#.000"));
            Debug.Log("[InputViewer] Input.gyro.attitude : " + Input.gyro.attitude.ToString("#.000"));
            Debug.Log("[InputViewer] Input.gyro.rotationRate : " + Input.gyro.rotationRate.ToString("#.000"));
            Debug.Log("[InputViewer] Input.gyro.ToString() : " + Input.gyro.ToString());
        }


        kalmanFrame.kalmanRawAcc = kalmanFilterRawAcc.Update(Input.acceleration);
        kalmanFrame.kalmanComputeAcc = kalmanFilterComputeAcc.Update(processAccFrame.computeAccelerationInit);
        kalmanFrame.kalmanComputeAcc = NaiveMovingTest.RoundVector3(kalmanFrame.kalmanComputeAcc, 2);
        kalmanVelocity += kalmanFrame.kalmanComputeAcc;
        kalmanVelocity = kalmanFilterSpeed.Update(kalmanVelocity);
        kalmanFrame.kalmanSpeed = kalmanVelocity;
        kalmanFrame.kalmanProcessSpeed = kalmanFilterProcessSpeed.Update(processAccFrame.velocityprocessInit);
        

        rawGraph.frames.Add(rawAccFrame);
        processGraph.frames.Add(processAccFrame);
        kalmanGraph.frames.Add(kalmanFrame);

    }

}

[CustomEditor(typeof(InputViewer))] //1
public class InputGraphButton : GraphButton
{
    protected override void ButtonPushed()
    {
        InputViewer inputViewer = (InputViewer)target;
        CreateJson(inputViewer.rawGraph, "Assets/Graph/rawGraph.graph");
        CreateJson(inputViewer.processGraph, "Assets/Graph/processGraph.graph");
        CreateJson(inputViewer.kalmanGraph, "Assets/Graph/kalmanGraph.graph");
    }
}
