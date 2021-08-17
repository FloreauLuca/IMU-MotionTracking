using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace record
{
    [Serializable]
    public struct RawAccFrame
    {
        public float time;
        public Vector3 acceleration;
        public Vector3 gravity;
        public Vector3 userAcceleration;
    }

    [Serializable]
    public struct RawGyrFrame
    {
        public float time;
        public Vector3 rotationRate;
        public Vector3 rotationRateUnbiased;
        public Quaternion attitude;
    }
    
    [Serializable]
    public struct RawMagnetoFrame
    {
        public float time;
        public double timestamp;
        public float trueHeading;
        public float magneticHeading;
        public Vector3 rawVector;
        public float headingAccuracy;
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
    public class RawMagnetoGraph
    {
        public List<RawMagnetoFrame> frames = new List<RawMagnetoFrame>();
    }

    [Serializable]
    public class PhaseGraph
    {
        public List<float> phases = new List<float>();
    }

    public class RawValueRecord : MonoBehaviour
    {
        private RawAccGraph rawAccGraph = new RawAccGraph();
        private RawGyrGraph rawGyrGraph = new RawGyrGraph();
        private RawMagnetoGraph rawMagnetoGraph = new RawMagnetoGraph();
        private PhaseGraph phaseGraph = new PhaseGraph();
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log(Application.persistentDataPath);
            Input.gyro.enabled = true;
            Input.compass.enabled = true;
        }

        // Update is called once per frame
        void Update()
        {
            RawAccFrame rawAccFrame = new RawAccFrame();
            rawAccFrame.time = Time.time;
            rawAccFrame.acceleration = Input.acceleration;
            rawAccFrame.userAcceleration = Input.gyro.userAcceleration;
            rawAccFrame.gravity = Input.gyro.gravity;

            RawGyrFrame rawGyrFrame = new RawGyrFrame();

            rawGyrFrame.time = Time.time;
            rawGyrFrame.attitude = Input.gyro.attitude;
            rawGyrFrame.rotationRate = Input.gyro.rotationRate;
            rawGyrFrame.rotationRateUnbiased = Input.gyro.rotationRateUnbiased;

            RawMagnetoFrame rawMagnetoFrame = new RawMagnetoFrame();
            rawMagnetoFrame.time = Time.time;
            rawMagnetoFrame.timestamp = Input.compass.timestamp;
            rawMagnetoFrame.trueHeading = Input.compass.trueHeading;
            rawMagnetoFrame.magneticHeading = Input.compass.magneticHeading;
            rawMagnetoFrame.rawVector = Input.compass.rawVector;
            rawMagnetoFrame.headingAccuracy = Input.compass.headingAccuracy;

            rawAccGraph.frames.Add(rawAccFrame);
            rawGyrGraph.frames.Add(rawGyrFrame);
            rawMagnetoGraph.frames.Add(rawMagnetoFrame);
        }

        public void PhaseGraph()
        {
            phaseGraph.phases.Add(Time.time);
        }

        public void WriteFiles()
        {
            CreateJson(rawAccGraph, Application.persistentDataPath + "/rawAccGraph" + System.DateTime.Now.ToString("-dd-MM-yy_HH-mm-ss") + ".graph");
            CreateJson(rawGyrGraph, Application.persistentDataPath + "/rawGyrGraph" + System.DateTime.Now.ToString("-dd-MM-yy_HH-mm-ss") + ".graph");
            CreateJson(rawMagnetoGraph, Application.persistentDataPath + "/rawMagnetoGraph" + System.DateTime.Now.ToString("-dd-MM-yy_HH-mm-ss") + ".graph");
            CreateJson(phaseGraph, Application.persistentDataPath + "/phaseGraph" + System.DateTime.Now.ToString("-dd-MM-yy_HH-mm-ss") + ".graph");

            rawAccGraph = new RawAccGraph();
            rawGyrGraph = new RawGyrGraph();
            rawMagnetoGraph = new RawMagnetoGraph();
        }

        string ToCSV()
        {
            var sb = new StringBuilder("Time,Value");
            //foreach (var frame in keyFrames)
            //{
            //    sb.Append('\n').Append(frame.Time.ToString()).Append(',').Append(frame.Value.ToString());
            //}

            return sb.ToString();
        }

        protected void CreateJson(object obj, string path)
        {
            string json = JsonUtility.ToJson(obj, true);
            WriteToFile(json, path);
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
