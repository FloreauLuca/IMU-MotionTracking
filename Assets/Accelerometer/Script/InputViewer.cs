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
public struct RawAccFrame
{
    public float dt;
    public Vector3 acceleration;
   public Vector3 gravity;
   public Vector3 userAcceleration;
   public Vector3 rawVelocity;
   public Vector3 rawPosition;
}
[Serializable]
public struct ProcessAccFrame
{
    public float dt;
    public Vector3 computeInitAcceleration;
    public Vector3 computeInitVelocity;
    public Vector3 computeInitPosition;
    public Vector3 computeResetAcceleration;
    public Vector3 computeResetVelocity;
    public Vector3 computeResetPosition;
}
[Serializable]
public struct KalmanFrame
{
    public float dt;
    public Vector3 kalmanRawAcc;
    public Vector3 kalmanRawVel;
    public Vector3 kalmanRawPos;

    public Vector3 kalmanComputeAcc;
    public Vector3 kalmanComputeVel;
    public Vector3 kalmanComputePos;

    public Vector3 kalmanK;
    public Vector3 kalmanP;

    public Vector3 kalmanQ;
    public Vector3 kalmanR;
}
[Serializable]
public struct AnalysisFrame
{
    public float dt;
    public int rawAccCount;
    public int computeAccCount;
    public Vector3 sumRawAcc;
    public Vector3 averageRawAcc;
    public Vector3[] boxRawAcc;
    public Vector3 sumComputeAcc;
    public Vector3 averageComputeAcc;
    public Vector3[] boxComputeAcc;
}
[Serializable]
public struct ABerkFrame
{
    public float dt;
    public Vector3 aBerkAcceleration;
    public Vector3 aBerkVelocity;
    public Vector3 aBerkPosition;
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
public class ABerkGraph
{
    public List<ABerkFrame> frames = new List<ABerkFrame>();
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


[Serializable]
public class LowValuePhase
{
    public float startValue;
    public float endValue;
    public int phase;
}

[Serializable]
public class LowValueGraph
{
    public List<LowValuePhase> phases = new List<LowValuePhase>();
}


public class InputViewer : MonoBehaviour
{
    private CalculationFarm calculationFarm;

    public RawGraph rawGraph;
    public ProcessGraph processGraph;
    public KalmanGraph kalmanGraph;
    public AnalysisGraph analysisGraph;
    public LowValueGraph lowValueGraph;
    public ABerkGraph aBerkGraph;
    public PhaseGraph phaseGraph = new PhaseGraph();


    [SerializeField] private bool gyro = false;
    [SerializeField] private bool log = false;

    [SerializeField] private Phase currentPhase;
    [SerializeField] private AnalysisFrame currentAnalysisFrame;
    public List<Vector3> currentPhaseRawAccs = new List<Vector3>();
    public List<Vector3> currentPhaseComputeAccs = new List<Vector3>();


    // Start is called before the first frame update
    void Start()
    {
        calculationFarm = FindObjectOfType<CalculationFarm>();
    }

    void Log()
    {
        Debug.Log("[InputViewer] Input.acceleration : " + Input.acceleration.ToString("#.000"));
        Debug.Log("[InputViewer] Input.accelerationEventCount : " + Input.accelerationEventCount);
        Debug.Log("[InputViewer] Input.gyro.gravity : " + Input.gyro.gravity.ToString("#.000"));
        Debug.Log("[InputViewer] process acceleration : " + (calculationFarm.currProcessAccFrame.computeInitAcceleration).ToString("#.000"));
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

    private float dt;
    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.acceleration == Vector3.zero) return;
        UpdateRawGraph();
        UpdateProcessAccGraph();
        UpdateKalmanGraph();
        UpdateABerkGraph();
        UpdateAnalysisGraph();
        UpdateLowValueGraph();
        currentPhase.valueCount++;
        if (log)
            Log();
    }
    
    void UpdateRawGraph()
    {
        rawGraph.frames.Add(calculationFarm.currRawAccFrame);
        Debug.Log(rawGraph.frames[0].dt + "; " + rawGraph.frames[0].acceleration);
    }
    
    void UpdateProcessAccGraph()
    {
        processGraph.frames.Add(calculationFarm.currProcessAccFrame);
    }
    
    void UpdateKalmanGraph()
    {
        kalmanGraph.frames.Add(calculationFarm.currKalmanFrame);

    }
    void UpdateABerkGraph()
    {
        aBerkGraph.frames.Add(calculationFarm.currABerkFrame);

    }

    void UpdateAnalysisGraph()
    {
        currentPhaseRawAccs.Add(calculationFarm.currRawAccFrame.userAcceleration);
        currentPhaseComputeAccs.Add(calculationFarm.currProcessAccFrame.computeInitAcceleration);
    }

    private LowValuePhase lowValuePhase = new LowValuePhase();

    void UpdateLowValueGraph()
    {
        if (calculationFarm.currProcessAccFrame.computeResetAcceleration == Vector3.zero && lowValuePhase.startValue == 0)
        {
            lowValuePhase = new LowValuePhase();
            lowValuePhase.startValue = calculationFarm.time;
        }
        if (lowValuePhase.startValue != 0 && calculationFarm.currProcessAccFrame.computeResetAcceleration != Vector3.zero)
        {
            lowValuePhase.endValue = calculationFarm.time;
            lowValueGraph.phases.Add(lowValuePhase);
            lowValuePhase = new LowValuePhase();
        }
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
            currentPhase.startValue = calculationFarm.time;
            currentPhase.phase = phase;
        }
        else
        {
            EndPhase();
            currentPhase = new Phase();
            currentPhaseRawAccs = new List<Vector3>();
            currentPhaseComputeAccs = new List<Vector3>();
            currentPhase.startValue = calculationFarm.time;
            currentPhase.phase = phase;
        }
    }

    public void EndPhase()
    {
        if (currentPhase.startValue != 0)
        {
            currentPhase.endValue = calculationFarm.time;
            EnterAnalysisPhase();
            phaseGraph.phases.Add(currentPhase);
            currentPhase = new Phase();
            currentAnalysisFrame = new AnalysisFrame();

        }
    }
}

[CustomEditor(typeof(InputViewer))] //1
public class InputGraphButton : GraphButton
{
    private string suffix = "";
    private bool saveRawGraph = true;
    private bool saveProcessGraph = true;
    private bool saveKalmanGraph = true;
    private bool savePhaseGraph = true;
    private bool saveAnalysisGraph = true;
    private bool saveLowValueGraph = true;
    private bool saveABerkGraph = true;

    private bool foldoutOpen = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        foldoutOpen = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutOpen, "Graph Export");
        if (foldoutOpen)
        {
            suffix = EditorGUILayout.TextField("Suffix", suffix);
            saveRawGraph = EditorGUILayout.Toggle("saveRawGraph", saveRawGraph);
            saveProcessGraph = EditorGUILayout.Toggle("saveProcessGraph", saveProcessGraph);
            saveKalmanGraph = EditorGUILayout.Toggle("saveKalmanGraph", saveKalmanGraph);
            savePhaseGraph = EditorGUILayout.Toggle("savePhaseGraph", savePhaseGraph);
            saveAnalysisGraph = EditorGUILayout.Toggle("saveAnalysisGraph", saveAnalysisGraph);
            saveLowValueGraph = EditorGUILayout.Toggle("saveLowValueGraph", saveLowValueGraph);
            saveABerkGraph = EditorGUILayout.Toggle("saveABerkGraph", saveABerkGraph);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    protected override void ButtonPushed()
    {
        InputViewer inputViewer = (InputViewer)target;
        if (saveRawGraph)
            CreateJson(inputViewer.rawGraph, "Assets/Graph/rawGraph" + suffix + ".graph");
        if (saveProcessGraph)
            CreateJson(inputViewer.processGraph, "Assets/Graph/processGraph" + suffix + ".graph");
        if (saveKalmanGraph)
            CreateJson(inputViewer.kalmanGraph, "Assets/Graph/kalmanGraph" + suffix + ".graph");
        if (savePhaseGraph)
            CreateJson(inputViewer.phaseGraph, "Assets/Graph/phaseGraph" + suffix + ".graph");
        if (saveAnalysisGraph)
            CreateJson(inputViewer.analysisGraph, "Assets/Graph/analysisGraph" + suffix + ".graph");
        if (saveLowValueGraph)
            CreateJson(inputViewer.lowValueGraph, "Assets/Graph/lowValueGraph" + suffix + ".graph");
        if (saveABerkGraph)
            CreateJson(inputViewer.aBerkGraph, "Assets/Graph/aBerkGraph" + suffix + ".graph");
    }
}
