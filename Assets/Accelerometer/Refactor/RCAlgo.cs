using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCAlgo : CalculationAlgo
{
    [SerializeField] private float RCHighPassAcc = 1.0f;
    [SerializeField] private float RCLowPassAcc = 1.0f;
    [SerializeField] private float RCHighPassVel = 1.0f;
    [SerializeField] private float RCLowPassVel = 1.0f;
    [SerializeField] private float RCHighPassPos = 1.0f;
    [SerializeField] private float RCLowPassPos = 1.0f;

    [SerializeField] private bool highPassAcc = false;
    [SerializeField] private bool highPassVel = false;
    [SerializeField] private bool highPassPos = false;
    [SerializeField] private bool lowPassAcc = false;
    [SerializeField] private bool lowPassVel = false;
    [SerializeField] private bool lowPassPos = false;

    [SerializeField] private bool resetVelocity = true;

    [SerializeField] private float thresholdAcc = 0.1f;
    [SerializeField] private float thresholdVel = 0.1f;
    private RCFrame currFrame;

    private Vector3 prevRcAcc;
    private Vector3 prevRcVel;
    private Vector3 prevRcPos;

    private Vector3 rcAcc;
    private Vector3 rcVel;
    private Vector3 rcPos;

    private Vector3 prevRawAcc;
    private Vector3 prevRawVel;
    private Vector3 prevRawPos;

    private Vector3 rawAcc;
    private Vector3 rawVel;
    private Vector3 rawPos;

    public override void UpdateData(float deltaTime)
    {
        if (!calculationFarm) return;
        rawAcc = calculationFarm.usedAcceleration;
        rcAcc = rawAcc;
        if (highPassAcc)
        {
            rcAcc = HighPassFilter.ComputeRC(rawAcc, prevRawAcc, prevRcAcc, deltaTime, RCHighPassAcc);
        }

        if (lowPassAcc)
        {
            rcAcc = LowPassFilter.ComputeRC(rawAcc, prevRcAcc, deltaTime, RCLowPassAcc);
        }

        rcAcc = RemoveBaseNoise(rcAcc, thresholdAcc);

        rawVel = rcAcc * deltaTime + rcVel;
        if (resetVelocity)
            rawVel = ResetVelocity(rawVel, rcAcc);

        rcVel = rawVel;
        if (highPassVel)
        {
            rcVel = HighPassFilter.ComputeRC(rawVel, prevRawVel, prevRcVel, deltaTime, RCHighPassVel);
        }

        if (lowPassVel)
        {
            rcVel = LowPassFilter.ComputeRC(rawVel, prevRcVel, deltaTime, RCLowPassVel);
        }

        rcVel = RemoveBaseNoise(rcVel, thresholdVel);
        rawPos = rcVel * deltaTime + rcPos;

        rcPos = rawPos;
        if (highPassPos)
        {
            rcPos = HighPassFilter.ComputeRC(rawPos, prevRawPos, prevRcPos, deltaTime, RCHighPassPos);
        }

        if (lowPassPos)
        {
            rcPos = LowPassFilter.ComputeRC(rawPos, prevRcPos, deltaTime, RCLowPassPos);
        }


        prevRawAcc = rawAcc;
        prevRawVel = rawVel;
        prevRawPos = rawPos;

        prevRcAcc = rcAcc;
        prevRcVel = rcVel;
        prevRcPos = rcPos;

        currFrame.rcAcc = rcAcc;
        currFrame.rcVel = rcVel;
        currFrame.rcPos = rcPos;

        base.UpdateData(deltaTime);
    }

    Vector3 RemoveBaseNoise(Vector3 vec, float minValue)
    {
        Vector3 computeVec = Vector3.zero;
        if (Mathf.Abs(vec.x) > minValue)
            computeVec.x = vec.x;
        if (Mathf.Abs(vec.y) > minValue)
            computeVec.y = vec.y;
        if (Mathf.Abs(vec.z) > minValue)
            computeVec.z = vec.z;

        return computeVec;
    }

    [SerializeField] private float zeroDelta = 0.1f;
    private List<Vector3> window = new List<Vector3>();
    private List<float> time = new List<float>();

    Vector3 ResetVelocity(Vector3 vel, Vector3 acc)
    {
        Vector3 sum = Vector3.zero;
        for (int i = time.Count - 1; i >= 0; i--)
        {
            if (calculationFarm.time - zeroDelta > time[i])
            {
                window.RemoveAt(i);
                time.RemoveAt(i);
            }
            else
            {
                sum += window[i];
            }
        }
        Vector3 computeVec = Vector3.zero;
        if (sum.x != 0)
            computeVec.x = vel.x;
        if (sum.y != 0)
            computeVec.y = vel.y;
        if (sum.z != 0)
            computeVec.z = vel.z;

        //if (acc != Vector3.zero)
        //    computeVec = vel;

        window.Add(acc);
        time.Add(calculationFarm.time);
        return computeVec;
    }

    protected override void Save()
    {
        calculationFarm.currRCFrame = currFrame;
        calculationFarm.currRCFrame.time = calculationFarm.time;
    }

    protected override void Display()
    {
        dataParent.acc = currFrame.rcAcc;
        dataParent.vel = currFrame.rcVel;
        dataParent.pos = currFrame.rcPos;
    }

    public override void ResetValue()
    {
        currFrame.rcAcc = Vector3.zero;
        currFrame.rcVel = Vector3.zero;
        currFrame.rcPos = Vector3.zero;

        rawAcc = Vector3.zero;
        rawVel = Vector3.zero;
        rawPos = Vector3.zero;

        rcAcc = Vector3.zero;
        rcVel = Vector3.zero;
        rcPos = Vector3.zero;

        prevRcAcc = Vector3.zero;
        prevRcVel = Vector3.zero;
        prevRcPos = Vector3.zero;

        prevRawAcc = Vector3.zero;
        prevRawVel = Vector3.zero;
        prevRawPos = Vector3.zero;
    }
}
