using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//https://github.com/xioTechnologies/Gait-Tracking-With-x-IMU/blob/master/Gait%20Tracking%20With%20x-IMU/Script.m

[Serializable]
public class XIMUGaitFrame : Frame
{
    public float deltaTime;
}

[Serializable]
public class XIMUGaitGraph
{
    public List<XIMUGaitFrame> frames = new List<XIMUGaitFrame>();
}

public class XIMUGaitTracking : MonoBehaviour
{
    public XIMUGaitGraph ximuGaitGraph = new XIMUGaitGraph();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        XIMUGaitFrame ximuGaitFrame = new XIMUGaitFrame();
        ximuGaitFrame.dt = Time.time;
        ximuGaitFrame.deltaTime = Time.deltaTime;


        ximuGaitGraph.frames.Add(ximuGaitFrame);

    }
}

[CustomEditor(typeof(XIMUGaitTracking))] //1
public class XIMUGaitGraphButton : GraphButton
{
    protected override void ButtonPushed()
    {
        XIMUGaitTracking ximuGaitTracking = (XIMUGaitTracking)target;
        CreateJson(ximuGaitTracking.ximuGaitGraph, "Assets/Graph/xIMUGait.graph");
    }
}
