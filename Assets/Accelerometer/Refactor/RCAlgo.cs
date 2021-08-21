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
        rawAcc = calculationFarm.usedAcceleration;

        //rcAcc = HighPassFilter.ComputeRC(rawAcc, prevRawAcc, prevRcAcc, Time.fixedDeltaTime, RCHighPassAcc);
        rcAcc = LowPassFilter.ComputeRC(rawAcc, prevRcAcc, deltaTime, RCLowPassAcc);
        if (Mathf.Abs(rawAcc.y) > thresholdAcc)
        {
            rawVel = rcAcc * deltaTime + rcVel;
        }
        else
        {
            rawVel = Vector3.zero;
        }

        rcVel = HighPassFilter.ComputeRC(rawVel, prevRawVel, prevRcVel, deltaTime, RCHighPassVel);
        if (Mathf.Abs(rcVel.y) > thresholdVel)
        {
            rcPos = rcVel * deltaTime + rcPos;
        }
        //rcPos = HighPassFilter.ComputeRC(rawPos, prevRawPos, prevRcPos, Time.fixedDeltaTime, RCHighPassPos);


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

    protected override void Save()
    {
        calculationFarm.currRCFrame = currFrame;
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
