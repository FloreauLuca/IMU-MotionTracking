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
    public Vector3 rawVel;
    public Vector3 rawPos;
}

[Serializable]
public struct RawGyrFrame
{
    public float time;
    public Vector3 rotationRate;
    public Quaternion attitude;
}

[Serializable]
public class RawAccGraph
{
    public List<RawAccFrame> frames = new List<RawAccFrame>();
}

[Serializable]
public class RawGyrGraph
{
    public List<RawGyrFrame> frames = new List<RawGyrFrame>();
}

[Serializable]
public struct GlobalFrame
{
    public float time;
    public Vector3 globalAcc;
    public Vector3 globalVel;
    public Vector3 globalPos;
}

[Serializable]
public class GlobalGraph
{
    public List<GlobalFrame> frames = new List<GlobalFrame>();
}

[Serializable]
public struct ComputeFrame
{
    public float time;
    public Vector3 computeAcc;
    public Vector3 computeVel;
    public Vector3 computePos;
}

[Serializable]
public class ComputeGraph
{
    public List<ComputeFrame> frames = new List<ComputeFrame>();
}

[Serializable]
public struct KalmanFrame
{
    public float time;
    public Vector3 kalmanAcc;
    public Vector3 kalmanVel;
    public Vector3 kalmanPos;
}

[Serializable]
public class KalmanGraph
{
    public List<KalmanFrame> frames = new List<KalmanFrame>();
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
public class RCGraph
{
    public List<RCFrame> frames = new List<RCFrame>();
}

[Serializable]
public class PhaseGraph
{
    public List<float> phases = new List<float>();
}

public class OutputManager : MonoBehaviour
{
    [SerializeField] private bool save = false;

    private NewCalculationFarm calculationFarm;

    public RawAccGraph rawGraph;
    public RawGyrGraph rawGyrGraph;
    public GlobalGraph globalGraph;
    public ComputeGraph computeGraph;
    public KalmanGraph kalmanGraph;
    public RCGraph rcGraph;

    public PhaseGraph phaseGraph = new PhaseGraph();

    private void Start()
    {
        calculationFarm = FindObjectOfType<NewCalculationFarm>();
    }

    private float dt;

    public void UpdateGraph()
    {
        rawGraph.frames.Add(calculationFarm.currRawAccFrame);
        
        rawGyrGraph.frames.Add(calculationFarm.currRawGyrFrame);
        
        globalGraph.frames.Add(calculationFarm.currGlobalAccFrame);
        
        computeGraph.frames.Add(calculationFarm.currComputeFrame);
        
        rcGraph.frames.Add(calculationFarm.currRCFrame);
        
        kalmanGraph.frames.Add(calculationFarm.currKalmanFrame);
        if (save)
        {
            Save();
            save = false;
        }
    }

    public void PhaseGraph()
    {
        phaseGraph.phases.Add(Time.time);
    }

    [SerializeField] private bool local;
    private string path;
    private string prefix;

    public void Save()
    {
        if (local)
        {
            path = "Assets/Graph";
            prefix = "/" + DateTime.Now.ToString("dd-MM-yy_HH-mm-ss");
            Directory.CreateDirectory(path + prefix);
            CreateJson(rawGraph, path + prefix + "/rawGraph" + ".graph");
            CreateJson(rawGyrGraph, path + prefix + "/rawGyrGraph" + ".graph");
            CreateJson(computeGraph, path + prefix + "/computeGraph" + ".graph");
            CreateJson(kalmanGraph, path + prefix + "/kalmanGraph" + ".graph");
            CreateJson(phaseGraph, path + prefix + "/phaseGraph" + ".graph");
            CreateJson(globalGraph, path + prefix + "/globalGraph" + ".graph");
            CreateJson(rcGraph, path + prefix + "/rcGraph" + ".graph");
            Debug.Log(path + prefix);
        }
        else
        {
            path = Application.persistentDataPath;
            prefix = "/" + DateTime.Now.ToString("dd-MM-yy_HH-mm-ss");
            Directory.CreateDirectory(path + prefix);
            CreateJson(rawGraph, path + prefix + "/rawGraph" + ".graph");
            CreateJson(rawGyrGraph, path + prefix + "/rawGyrGraph" + ".graph");
            CreateJson(computeGraph, path + prefix + "/computeGraph" + ".graph");
            CreateJson(kalmanGraph, path + prefix + "/kalmanGraph" + ".graph");
            CreateJson(phaseGraph, path + prefix + "/phaseGraph" + ".graph");
            CreateJson(globalGraph, path + prefix + "/globalGraph" + ".graph");
            CreateJson(rcGraph, path + prefix + "/rcGraph" + ".graph");
            Debug.Log(path + prefix);
        }


        rawGraph = new RawAccGraph();
        rawGyrGraph = new RawGyrGraph();
        computeGraph = new ComputeGraph();
        kalmanGraph = new KalmanGraph();
        phaseGraph = new PhaseGraph();
        globalGraph = new GlobalGraph();
        rcGraph = new RCGraph();
        calculationFarm.ResetVelocity();
    }

    private void CreateJson(object obj, string path)
    {
        var json = JsonUtility.ToJson(obj, true);
        WriteToFile(json, path);
        //Debug.Log("<color=orange> JsonBuild</color>");
    }

    private void WriteToFile(string json, string path)
    {
        File.Delete(path);
        var fileStream = new FileStream(path, FileMode.Create);

        using (var writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }
    }
}
