using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using old;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace test
{
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

    public class InputRecorder : MonoBehaviour
    {
        private CalculationFarm calculationFarm;

        public RawAccGraph rawGraph;
        public RawGyrGraph rawGyrGraph;
        public GlobalGraph globalGraph;
        public ComputeGraph computeGraph;
        public KalmanGraph kalmanGraph;
        public RCGraph rcGraph;

        public PhaseGraph phaseGraph = new PhaseGraph();

        void Start()
        {
            calculationFarm = FindObjectOfType<CalculationFarm>();
        }
        
        private float dt;

        void LateUpdate()
        {
            RawAccFrame rawAccFrame = new RawAccFrame();
            rawAccFrame.time = calculationFarm.time;
            rawAccFrame.acceleration = calculationFarm.currRawAccFrame.acceleration;
            rawAccFrame.gravity = calculationFarm.currRawAccFrame.gravity;
            rawAccFrame.userAcceleration = calculationFarm.currRawAccFrame.userAcceleration;
            rawAccFrame.rawVel = calculationFarm.currRawAccFrame.rawVelocity;
            rawAccFrame.rawPos = calculationFarm.currRawAccFrame.rawPosition;
            rawGraph.frames.Add(rawAccFrame);

            RawGyrFrame rawGyrFrame = new RawGyrFrame();
            rawGyrFrame.time = calculationFarm.time;
            rawGyrFrame.attitude = calculationFarm.currRawGyrFrame.attitude;
            rawGyrFrame.rotationRate = calculationFarm.currRawGyrFrame.rotationRate;
            rawGyrGraph.frames.Add(rawGyrFrame);

            GlobalFrame globalFrame = new GlobalFrame();
            globalFrame.time = calculationFarm.time;
            globalFrame.globalAcc = calculationFarm.currGlobalAccFrame.globalAcc;
            globalFrame.globalVel = calculationFarm.currGlobalAccFrame.globalVelocity;
            globalFrame.globalPos = calculationFarm.currGlobalAccFrame.globalPos;
            globalGraph.frames.Add(globalFrame);

            ComputeFrame computeFrame = new ComputeFrame();
            computeFrame.time = calculationFarm.time;
            computeFrame.computeAcc = calculationFarm.currProcessAccFrame.computeResetAcceleration;
            computeFrame.computeVel = calculationFarm.currProcessAccFrame.computeResetVelocity;
            computeFrame.computePos = calculationFarm.currProcessAccFrame.computeResetPosition;
            computeGraph.frames.Add(computeFrame);

            RCFrame rcFrame = new RCFrame();
            rcFrame.time = calculationFarm.time;
            rcFrame.rcAcc = calculationFarm.currRCFrame.rcAcc;
            rcFrame.rcVel = calculationFarm.currRCFrame.rcVel;
            rcFrame.rcPos = calculationFarm.currRCFrame.rcPos;
            rcGraph.frames.Add(rcFrame);

            KalmanFrame kalmanFrame = new KalmanFrame();
            kalmanFrame.time = calculationFarm.time;
            kalmanFrame.kalmanAcc = calculationFarm.currKalmanFrame.kalmanRawAcc;
            kalmanFrame.kalmanVel = calculationFarm.currKalmanFrame.kalmanRawVel;
            kalmanFrame.kalmanPos = calculationFarm.currKalmanFrame.kalmanRawPos;
            kalmanGraph.frames.Add(kalmanFrame);
        }

        public void PhaseGraph()
        {
            phaseGraph.phases.Add(Time.time);
        }

        private string path;
        private string prefix;

        public void Save()
        {
            path = Application.persistentDataPath;
            prefix = "/" + System.DateTime.Now.ToString("dd-MM-yy_HH-mm-ss");
            Directory.CreateDirectory(path + prefix);
            CreateJson(rawGraph, path + prefix + "/rawGraph" + ".graph");
            CreateJson(rawGyrGraph, path + prefix + "/rawGyrGraph" + ".graph");
            CreateJson(computeGraph, path + prefix + "/computeGraph" + ".graph");
            CreateJson(kalmanGraph, path + prefix + "/kalmanGraph" + ".graph");
            CreateJson(phaseGraph, path + prefix + "/phaseGraph" + ".graph");
            CreateJson(globalGraph, path + prefix + "/globalGraph" + ".graph");
            CreateJson(rcGraph, path + prefix + "/rcGraph" + ".graph");
            Debug.Log(path + prefix);

            rawGraph = new RawAccGraph();
            rawGyrGraph = new RawGyrGraph();
            computeGraph = new ComputeGraph();
            kalmanGraph = new KalmanGraph();
            phaseGraph = new PhaseGraph();
            globalGraph = new GlobalGraph();
            rcGraph = new RCGraph();
        }

        private void CreateJson(object obj, string path)
        {
            string json = JsonUtility.ToJson(obj, true);
            WriteToFile(json, path);
            //Debug.Log("<color=orange> JsonBuild</color>");
        }

        private void WriteToFile(string json, string path)
        {
            File.Delete(path);
            FileStream fileStream = new FileStream(path, FileMode.Create);

            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(json);
            }
        }
    }
}
