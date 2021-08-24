using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherAlgo : CalculationAlgo
{
    private List<Vector3> accs = new List<Vector3>();

    private ComputeFrame currFrame;

    public override void UpdateData(float deltaTime)
    {
        //accs.Add(calculationFarm.usedAcceleration *deltaTime);
        //Vector3 avg = Vector3.zero;
        //foreach (Vector3 acc in accs)
        //{
        //    avg += acc;
        //}

        //avg /= accs.Count;
        //currFrame.computeAcc = avg;


        currFrame.computeAcc = calculationFarm.currRawGyrFrame.attitude.eulerAngles;
        base.UpdateData(deltaTime);
    }

    protected override void Save()
    {
        //calculationFarm.currComputeFrame = currFrame;
        //calculationFarm.currComputeFrame.time = calculationFarm.time;
    }

    protected override void Display()
    {
        dataParent.acc = currFrame.computeAcc;
        dataParent.vel = currFrame.computeVel;
        dataParent.pos = currFrame.computePos;
    }
    
    public override void ResetValue()
    {
        currFrame.computeAcc = Vector3.zero;
        currFrame.computeVel = Vector3.zero;
        currFrame.computePos = Vector3.zero;
    }
}
