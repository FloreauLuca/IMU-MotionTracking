using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAlgo : CalculationAlgo
{
    [SerializeField] private float thresholdAcc = 0.1f;

    private ComputeFrame currFrame;

    [SerializeField] private bool removeNoise = true;
    [SerializeField] private bool resetVelocity = true;
    
    public override void UpdateData(float deltaTime)
    {
        if (!calculationFarm) return;
        //Remove init delta
        currFrame.computeAcc = calculationFarm.usedAcceleration;

        if (removeNoise)
        {
            currFrame.computeAcc = RemoveBaseNoise(currFrame.computeAcc, thresholdAcc);
        }

        currFrame.computeVel += currFrame.computeAcc * calculationFarm.deltaTime;
        if (resetVelocity)
            currFrame.computeVel = ResetVelocity(currFrame.computeVel, currFrame.computeAcc);

        currFrame.computePos += currFrame.computeVel * calculationFarm.deltaTime;

        base.UpdateData(deltaTime);
    }

    protected override void Save()
    {
        calculationFarm.currComputeFrame = currFrame;
        calculationFarm.currComputeFrame.time = calculationFarm.time;
    }

    protected override void Display()
    {
        dataParent.acc = currFrame.computeAcc;
        dataParent.vel = currFrame.computeVel;
        dataParent.pos = currFrame.computePos;
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
        for (int i = time.Count-1; i >= 0; i--)
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

    public override void ResetValue()
    {
        currFrame.computeAcc = Vector3.zero;
        currFrame.computeVel = Vector3.zero;
        currFrame.computePos = Vector3.zero;
    }
}
