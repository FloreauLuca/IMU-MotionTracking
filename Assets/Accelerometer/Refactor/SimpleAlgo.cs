using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAlgo : CalculationAlgo
{
    [SerializeField] private float thresholdAcc = 0.1f;

    private ComputeFrame currFrame;
    
    public override void UpdateData(float deltaTime)
    {
        //Remove init delta
        currFrame.computeAcc = calculationFarm.usedAcceleration;

        //Remove Standard noise
        currFrame.computeAcc = RemoveBaseNoise(currFrame.computeAcc, thresholdAcc);
        currFrame.computeVel += currFrame.computeAcc * calculationFarm.deltaTime;
        //currFrame.pos += currFrame.vel * calculationFarm.deltaTime;

        if (currFrame.computeAcc == Vector3.zero)
        {
            currFrame.computeVel = Vector3.zero;
        }
        currFrame.computePos += currFrame.computeVel * calculationFarm.deltaTime;

        base.UpdateData(deltaTime);
    }

    protected override void Save()
    {
        calculationFarm.currComputeFrame = currFrame;
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

    public override void ResetValue()
    {
        currFrame.computeAcc = Vector3.zero;
        currFrame.computeVel = Vector3.zero;
        currFrame.computePos = Vector3.zero;
    }
}
