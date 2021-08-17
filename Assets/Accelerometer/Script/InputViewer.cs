using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[Serializable]
public struct RawAccFrame
{
    public float time;
    public Vector3 acceleration;
   public Vector3 gravity;
   public Vector3 userAcceleration;
   public Vector3 rawVelocity;
   public Vector3 rawPosition;
}

[Serializable]
public struct RawGyrFrame
{
    public float time;
    public Vector3 rotationRate;
    public Vector3 rotationRateUnbiased;
    public Quaternion attitude;
    public Vector3 angle;
    public Vector3 angleUnbiased;
}

[Serializable]
public struct GlobalAccFrame
{
    public float time;
    public Vector3 globalAcc;
    public Vector3 globalVelocity;
    public Vector3 globalPos;
}

[Serializable]
public struct ProcessAccFrame
{
    public float time;
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
    public float time;
    public Vector3 kalmanRawAcc;
    public Vector3 kalmanRawVel;
    public Vector3 kalmanRawPos;

    public Vector3 kalmanComputeAcc;
    public Vector3 kalmanComputeVel;
    public Vector3 kalmanComputePos;

    public Vector3 ekfRawAcc;
    public Vector3 ekfRawVel;
    public Vector3 ekfRawPos;

    public Vector3 kalmanK;
    public Vector3 kalmanP;

    public Vector3 kalmanQ;
    public Vector3 kalmanR;
}
[Serializable]
public struct AnalysisFrame
{
    public float time;
    public int rawAccCount;
    public int computeAccCount;
    public Vector3 sumRawAcc;
    public Vector3 averageRawAcc;
    //public Vector3[] boxRawAcc;
    public Vector3 sumComputeAcc;
    public Vector3 averageComputeAcc;
    //public Vector3[] boxComputeAcc;

    public Vector3 sumRawVel;
    public Vector3 averageRawVel;
    public int rawVelCount;

    public Vector3 sumRawPos;
    public Vector3 averageRawPos;
    public int rawPosCount;
}
[Serializable]
public struct ABerkFrame
{
    public float time;
    public Vector3 aBerkAcceleration;
    public Vector3 aBerkVelocity;
    public Vector3 aBerkPosition;
}

[Serializable]
public struct RCFrame
{
    public float time;
    public Vector3 rcAcc;
    public Vector3 rcVel;
    public Vector3 rcPos;
}

[Serializable]
public class RawGraph
{
    public List<RawAccFrame> frames = new List<RawAccFrame>();
}

[Serializable]
public class RawGyrGraph
{
    public List<RawGyrFrame> frames = new List<RawGyrFrame>();
}

[Serializable]
public class GlobalGraph
{
    public List<GlobalAccFrame> frames = new List<GlobalAccFrame>();
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
public class RCGraph
{
    public List<RCFrame> frames = new List<RCFrame>();
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
    public RawGyrGraph rawGyrGraph;
    public GlobalGraph globalGraph;
    public ProcessGraph processGraph;
    public KalmanGraph kalmanGraph;
    public AnalysisGraph analysisGraph;
    public LowValueGraph lowValueGraph;
    public ABerkGraph aBerkGraph;
    public RCGraph rcGraph;
    public PhaseGraph phaseGraph = new PhaseGraph();


    [SerializeField] private bool gyro = false;
    [SerializeField] private bool log = false;

    [SerializeField] private Phase currentPhase;
    [SerializeField] private AnalysisFrame currentAnalysisFrame;
    public List<Vector3> currentPhaseRawAccs = new List<Vector3>();
    public List<Vector3> currentPhaseRawVels = new List<Vector3>();
    public List<Vector3> currentPhaseRawPoss = new List<Vector3>();
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
        //if (calculationFarm.currRawAccFrame.acceleration == Vector3.zero) return;
        //if (Math.Abs(calculationFarm.currRawAccFrame.time - rawGraph.frames[rawGraph.frames.Count - 1].time) < 0.0001f) return;
        UpdateRawGraph();
        globalGraph.frames.Add(calculationFarm.currGlobalAccFrame);
        rcGraph.frames.Add(calculationFarm.currRCFrame);
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
        rawGyrGraph.frames.Add(calculationFarm.currRawGyrFrame);
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
        currentPhaseRawVels.Add(calculationFarm.currRawAccFrame.rawVelocity);
        currentPhaseRawPoss.Add(calculationFarm.currRawAccFrame.rawPosition);
        currentPhaseComputeAccs.Add(calculationFarm.currProcessAccFrame.computeInitAcceleration * calculationFarm.currProcessAccFrame.time);
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
        //currentAnalysisFrame.boxRawAcc[0].x = currentPhaseRawAccs.Aggregate((smallest, next) => next.x < smallest.x ? next : smallest).x;
        //currentAnalysisFrame.boxRawAcc[0].y = currentPhaseRawAccs.Aggregate((smallest, next) => next.y < smallest.y ? next : smallest).y;
        //currentAnalysisFrame.boxRawAcc[0].z = currentPhaseRawAccs.Aggregate((smallest, next) => next.z < smallest.z ? next : smallest).z;
        //currentAnalysisFrame.boxRawAcc[1] = currentPhaseRawAccs[(1/4)*(currentPhaseRawAccs.Count+1)];
        //currentAnalysisFrame.boxRawAcc[3] = currentPhaseRawAccs[(3 / 4) * (currentPhaseRawAccs.Count + 1)];
        //currentAnalysisFrame.boxRawAcc[2] = currentAnalysisFrame.boxRawAcc[3] - currentAnalysisFrame.boxRawAcc[1];
        //currentAnalysisFrame.boxRawAcc[4].x = currentPhaseRawAccs.Aggregate((longest, next) => next.x > longest.x ? next : longest).x;
        //currentAnalysisFrame.boxRawAcc[4].y = currentPhaseRawAccs.Aggregate((longest, next) => next.y > longest.y ? next : longest).y;
        //currentAnalysisFrame.boxRawAcc[4].z = currentPhaseRawAccs.Aggregate((longest, next) => next.z > longest.z ? next : longest).z;
        currentAnalysisFrame.rawAccCount = currentPhaseRawAccs.Count;

        currentAnalysisFrame.sumComputeAcc = currentPhaseComputeAccs.Aggregate((vec1, vec2) => vec1 + vec2);
        currentAnalysisFrame.averageComputeAcc = currentAnalysisFrame.sumComputeAcc / (currentPhaseComputeAccs.Count);
        //currentAnalysisFrame.boxComputeAcc[0].x = currentPhaseComputeAccs.Aggregate((smallest, next) => next.x < smallest.x ? next : smallest).x;
        //currentAnalysisFrame.boxComputeAcc[0].y = currentPhaseComputeAccs.Aggregate((smallest, next) => next.y < smallest.y ? next : smallest).y;
        //currentAnalysisFrame.boxComputeAcc[0].z = currentPhaseComputeAccs.Aggregate((smallest, next) => next.z < smallest.z ? next : smallest).z;
        //currentAnalysisFrame.boxComputeAcc[1] = currentPhaseComputeAccs[(1 / 4) * (currentPhaseComputeAccs.Count + 1)];
        //currentAnalysisFrame.boxComputeAcc[3] = currentPhaseComputeAccs[(3 / 4) * (currentPhaseComputeAccs.Count + 1)];
        //currentAnalysisFrame.boxComputeAcc[2] = currentAnalysisFrame.boxComputeAcc[3] - currentAnalysisFrame.boxComputeAcc[1];
        //currentAnalysisFrame.boxComputeAcc[4].x = currentPhaseComputeAccs.Aggregate((longest, next) => next.x > longest.x ? next : longest).x;
        //currentAnalysisFrame.boxComputeAcc[4].y = currentPhaseComputeAccs.Aggregate((longest, next) => next.y > longest.y ? next : longest).y;
        //currentAnalysisFrame.boxComputeAcc[4].z = currentPhaseComputeAccs.Aggregate((longest, next) => next.z > longest.z ? next : longest).z;
        currentAnalysisFrame.computeAccCount = currentPhaseComputeAccs.Count;


        currentAnalysisFrame.sumRawVel = currentPhaseRawVels.Aggregate((vec1, vec2) => vec1 + vec2);
        currentAnalysisFrame.averageRawVel = currentAnalysisFrame.sumRawVel / (currentPhaseRawVels.Count);
        currentAnalysisFrame.rawVelCount = currentPhaseRawVels.Count;

        currentAnalysisFrame.sumRawPos = currentPhaseRawPoss.Aggregate((vec1, vec2) => vec1 + vec2);
        currentAnalysisFrame.averageRawPos = currentAnalysisFrame.sumRawPos / (currentPhaseRawPoss.Count);
        currentAnalysisFrame.rawPosCount = currentPhaseRawPoss.Count;



        currentAnalysisFrame.time = (currentPhase.startValue + currentPhase.endValue)/2;
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
#if UNITY_EDITOR
[CustomEditor(typeof(InputViewer))] //1
public class InputGraphButton : GraphButton
{
    private string prefix;
    private string suffix;
    private bool saveRawGraph = true;
    private bool saveProcessGraph = true;
    private bool saveKalmanGraph = true;
    private bool savePhaseGraph = true;
    private bool saveAnalysisGraph = true;
    private bool saveLowValueGraph = true;
    private bool saveABerkGraph = true;
    private bool saveGlobalGraph = true;

    private bool foldoutOpen = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        foldoutOpen = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutOpen, "Graph Export");
        if (foldoutOpen)
        {
            prefix = EditorGUILayout.TextField("Prefix", prefix);
            suffix = EditorGUILayout.TextField("Suffix", suffix);
            saveRawGraph = EditorGUILayout.Toggle("saveRawGraph", saveRawGraph);
            saveProcessGraph = EditorGUILayout.Toggle("saveProcessGraph", saveProcessGraph);
            saveKalmanGraph = EditorGUILayout.Toggle("saveKalmanGraph", saveKalmanGraph);
            savePhaseGraph = EditorGUILayout.Toggle("savePhaseGraph", savePhaseGraph);
            saveAnalysisGraph = EditorGUILayout.Toggle("saveAnalysisGraph", saveAnalysisGraph);
            saveLowValueGraph = EditorGUILayout.Toggle("saveLowValueGraph", saveLowValueGraph);
            saveABerkGraph = EditorGUILayout.Toggle("saveABerkGraph", saveABerkGraph);
            saveGlobalGraph = EditorGUILayout.Toggle("saveGlobalGraph", saveGlobalGraph);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    protected override void ButtonPushed()
    {
        InputViewer inputViewer = (InputViewer)target;
        if (saveRawGraph)
            CreateJson(inputViewer.rawGraph, "Assets/Graph/" + prefix + "rawGraph" + suffix + ".graph");
        if (saveRawGraph)
            CreateJson(inputViewer.rawGyrGraph, "Assets/Graph/" + prefix + "rawGyrGraph" + suffix + ".graph");
        if (saveProcessGraph)
            CreateJson(inputViewer.processGraph, "Assets/Graph/" + prefix + "processGraph" + suffix + ".graph");
        if (saveKalmanGraph)
            CreateJson(inputViewer.kalmanGraph, "Assets/Graph/" + prefix + "kalmanGraph" + suffix + ".graph");
        if (savePhaseGraph)
            CreateJson(inputViewer.phaseGraph, "Assets/Graph/" + prefix + "phaseGraph" + suffix + ".graph");
        if (saveAnalysisGraph)
            CreateJson(inputViewer.analysisGraph, "Assets/Graph/" + prefix + "analysisGraph" + suffix + ".graph");
        if (saveLowValueGraph)
            CreateJson(inputViewer.lowValueGraph, "Assets/Graph/" + prefix + "lowValueGraph" + suffix + ".graph");
        if (saveABerkGraph)
            CreateJson(inputViewer.aBerkGraph, "Assets/Graph/" + prefix + "aBerkGraph" + suffix + ".graph");
        if (saveGlobalGraph)
            CreateJson(inputViewer.globalGraph, "Assets/Graph/" + prefix + "globalGraph" + suffix + ".graph");

        CreateJson(inputViewer.rcGraph, "Assets/Graph/" + prefix + "rcGraph" + suffix + ".graph");
    }
}
#endif
