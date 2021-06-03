using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

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
public class AnalysisFrame : Frame
{
    public int rawAccCount;
    public int computeAccCount;
    public Vector3 sumRawAcc;
    public Vector3 averageRawAcc;
    public Vector3[] boxRawAcc = new Vector3[5];
    public Vector3 sumComputeAcc;
    public Vector3 averageComputeAcc;
    public Vector3[] boxComputeAcc = new Vector3[5];
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

[Serializable]
public class AnalysisGraph
{
    public List<AnalysisFrame> frames = new List<AnalysisFrame>();
}

[Serializable]
public class Phase
{
    public float startValue;
    public float endValue;
    public int phase;
    public int valueCount;
}

[Serializable]
public class PhaseGraph
{
    public List<Phase> phases = new List<Phase>();
}


public class InputViewer : MonoBehaviour
{
    public RawGraph rawGraph;
    public ProcessGraph processGraph;
    public KalmanGraph kalmanGraph;
    public AnalysisGraph analysisGraph;
    public PhaseGraph phaseGraph = new PhaseGraph();


    private Vector3 velocity;
    private Vector3 kalmanVelocity;
    private Vector3 velocityprocessInit;
    private Vector3 acceleration;


    private KalmanFilterVector3 kalmanFilterRawAcc = new KalmanFilterVector3();
    private KalmanFilterVector3 kalmanFilterComputeAcc = new KalmanFilterVector3();
    private KalmanFilterVector3 kalmanFilterSpeed = new KalmanFilterVector3();
    private KalmanFilterVector3 kalmanFilterProcessSpeed = new KalmanFilterVector3();

    [SerializeField] private bool gyro = false;

    [SerializeField] private Phase currentPhase;
    [SerializeField] private AnalysisFrame currentAnalysisFrame;
    public List<Vector3> currentPhaseRawAccs = new List<Vector3>();
    public List<Vector3> currentPhaseComputeAccs = new List<Vector3>();


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
        UpdateRawGraph();
        UpdateProcessAccGraph();
        UpdateKalmanGraph();
        UpdateAnalysisGraph();
        currentPhase.valueCount++;

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
        
    }

    private RawAccFrame rawAccFrame;
    void UpdateRawGraph()
    {

        rawAccFrame = new RawAccFrame();
        rawAccFrame.dt = Time.time;
        rawAccFrame.acceleration = Input.acceleration;
        rawAccFrame.gravity = Input.gyro.gravity;
        rawAccFrame.userAcceleration = Input.gyro.userAcceleration;
        velocity += rawAccFrame.userAcceleration;
        rawAccFrame.rawVelocity = velocity;
        rawGraph.frames.Add(rawAccFrame);
    }

    private ProcessAccFrame processAccFrame;
    void UpdateProcessAccGraph()
    {
        processAccFrame = new ProcessAccFrame();
        processAccFrame.dt = Time.time;
        //Remove init noise
        processAccFrame.computeAccelerationInit = (Input.acceleration - acceleration);
        //Remove Standard noise
        if (Mathf.Abs(processAccFrame.computeAccelerationInit.x) > 0.05f)
            velocityprocessInit += processAccFrame.computeAccelerationInit.x * Vector3.right;
        if (Mathf.Abs(processAccFrame.computeAccelerationInit.y) > 0.05f)
            velocityprocessInit += processAccFrame.computeAccelerationInit.y * Vector3.up;
        if (Mathf.Abs(processAccFrame.computeAccelerationInit.z) > 0.05f)
            velocityprocessInit += processAccFrame.computeAccelerationInit.z * Vector3.forward;

        processAccFrame.velocityprocessInit = velocityprocessInit;
        processGraph.frames.Add(processAccFrame);
    }

    private KalmanFrame kalmanFrame;
    void UpdateKalmanGraph()
    {
        kalmanFrame = new KalmanFrame();
        kalmanFrame.dt = Time.time;
        kalmanFrame.kalmanRawAcc = kalmanFilterRawAcc.Update(Input.acceleration);
        kalmanFrame.kalmanComputeAcc = kalmanFilterComputeAcc.Update(processAccFrame.computeAccelerationInit);
        //kalmanFrame.kalmanComputeAcc = NaiveMovingTest.RoundVector3(kalmanFrame.kalmanComputeAcc, 2);
        kalmanVelocity += kalmanFrame.kalmanComputeAcc;
        //kalmanVelocity = kalmanFilterSpeed.Update(kalmanVelocity);
        kalmanFrame.kalmanSpeed = kalmanVelocity;
        kalmanFrame.kalmanProcessSpeed = kalmanFilterProcessSpeed.Update(kalmanVelocity);
        kalmanGraph.frames.Add(kalmanFrame);

    }
    
    void UpdateAnalysisGraph()
    {
        currentPhaseRawAccs.Add(rawAccFrame.userAcceleration);
        currentPhaseComputeAccs.Add(processAccFrame.computeAccelerationInit);
    }

    void EnterAnalysisPhase()
    {
        currentAnalysisFrame.sumRawAcc = currentPhaseRawAccs.Aggregate((vec1, vec2) => vec1 + vec2);
        currentAnalysisFrame.averageRawAcc = currentAnalysisFrame.sumRawAcc / (currentPhaseRawAccs.Count);
        currentAnalysisFrame.boxRawAcc[0].x = currentPhaseRawAccs.Aggregate((smallest, next) => next.x < smallest.x ? next : smallest).x;
        currentAnalysisFrame.boxRawAcc[0].y = currentPhaseRawAccs.Aggregate((smallest, next) => next.y < smallest.y ? next : smallest).y;
        currentAnalysisFrame.boxRawAcc[0].z = currentPhaseRawAccs.Aggregate((smallest, next) => next.z < smallest.z ? next : smallest).z;
        currentAnalysisFrame.boxRawAcc[1] = currentPhaseRawAccs[(1/4)*(currentPhaseRawAccs.Count+1)];
        currentAnalysisFrame.boxRawAcc[3] = currentPhaseRawAccs[(3 / 4) * (currentPhaseRawAccs.Count + 1)];
        currentAnalysisFrame.boxRawAcc[2] = currentAnalysisFrame.boxRawAcc[3] - currentAnalysisFrame.boxRawAcc[1];
        currentAnalysisFrame.boxRawAcc[4].x = currentPhaseRawAccs.Aggregate((longest, next) => next.x > longest.x ? next : longest).x;
        currentAnalysisFrame.boxRawAcc[4].y = currentPhaseRawAccs.Aggregate((longest, next) => next.y > longest.y ? next : longest).y;
        currentAnalysisFrame.boxRawAcc[4].z = currentPhaseRawAccs.Aggregate((longest, next) => next.z > longest.z ? next : longest).z;
        currentAnalysisFrame.rawAccCount = currentPhaseRawAccs.Count;

        currentAnalysisFrame.sumComputeAcc = currentPhaseComputeAccs.Aggregate((vec1, vec2) => vec1 + vec2);
        currentAnalysisFrame.averageComputeAcc = currentAnalysisFrame.sumComputeAcc / (currentPhaseComputeAccs.Count);
        currentAnalysisFrame.boxComputeAcc[0].x = currentPhaseComputeAccs.Aggregate((smallest, next) => next.x < smallest.x ? next : smallest).x;
        currentAnalysisFrame.boxComputeAcc[0].y = currentPhaseComputeAccs.Aggregate((smallest, next) => next.y < smallest.y ? next : smallest).y;
        currentAnalysisFrame.boxComputeAcc[0].z = currentPhaseComputeAccs.Aggregate((smallest, next) => next.z < smallest.z ? next : smallest).z;
        currentAnalysisFrame.boxComputeAcc[1] = currentPhaseComputeAccs[(1 / 4) * (currentPhaseComputeAccs.Count + 1)];
        currentAnalysisFrame.boxComputeAcc[3] = currentPhaseComputeAccs[(3 / 4) * (currentPhaseComputeAccs.Count + 1)];
        currentAnalysisFrame.boxComputeAcc[2] = currentAnalysisFrame.boxComputeAcc[3] - currentAnalysisFrame.boxComputeAcc[1];
        currentAnalysisFrame.boxComputeAcc[4].x = currentPhaseComputeAccs.Aggregate((longest, next) => next.x > longest.x ? next : longest).x;
        currentAnalysisFrame.boxComputeAcc[4].y = currentPhaseComputeAccs.Aggregate((longest, next) => next.y > longest.y ? next : longest).y;
        currentAnalysisFrame.boxComputeAcc[4].z = currentPhaseComputeAccs.Aggregate((longest, next) => next.z > longest.z ? next : longest).z;
        currentAnalysisFrame.computeAccCount = currentPhaseComputeAccs.Count;

        currentAnalysisFrame.dt = (currentPhase.startValue + currentPhase.endValue)/2;
        analysisGraph.frames.Add(currentAnalysisFrame);
    }

    public void SetPhase(int phase)
    {
        if (currentPhase.startValue == 0)
        {
            currentPhase = new Phase();
            currentPhaseRawAccs = new List<Vector3>();
            currentPhaseComputeAccs = new List<Vector3>();
            currentPhase.startValue = Time.time;
            currentPhase.phase = phase;
        }
        else
        {
            EndPhase();
            currentPhase = new Phase();
            currentPhaseRawAccs = new List<Vector3>();
            currentPhaseComputeAccs = new List<Vector3>();
            currentPhase.startValue = Time.time;
            currentPhase.phase = phase;
        }
    }

    public void EndPhase()
    {
        if (currentPhase.startValue != 0)
        {
            currentPhase.endValue = Time.time;
            EnterAnalysisPhase();
            phaseGraph.phases.Add(currentPhase);
            currentPhase = new Phase();
            currentAnalysisFrame = new AnalysisFrame();

        }
    }

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
        velocityprocessInit = Vector3.zero;
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
        CreateJson(inputViewer.phaseGraph, "Assets/Graph/phaseGraph.graph");
        CreateJson(inputViewer.analysisGraph, "Assets/Graph/analysisGraph.graph");
    }
}
