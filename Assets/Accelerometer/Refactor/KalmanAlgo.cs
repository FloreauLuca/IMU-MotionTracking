using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KalmanAlgo : CalculationAlgo
{
    private KalmanFrame currFrame;

    [SerializeField] private float Q = 0.001f;
    [SerializeField] private float R = 0.01f;

    private Vector3 K;
    private Vector3 P;

    [SerializeField] private int applyOn = 0;

    private KalmanFilterFloat kalmanX;
    private KalmanFilterFloat kalmanY;
    private KalmanFilterFloat kalmanZ;

    private FusionKalmanFilter ekf = new FusionKalmanFilter();

    [SerializeField] private bool resetVelocity = true;

    [SerializeField] private float thresholdAcc = 0.1f;
    [SerializeField] private float thresholdVel = 0.1f;

    void Start()
    {
        base.Start();
        kalmanX = new KalmanFilterFloat(Q, R);
        kalmanY = new KalmanFilterFloat(Q, R);
        kalmanZ = new KalmanFilterFloat(Q, R);

        ekf.Start(0.09f);
    }

    public override void UpdateData(float deltaTime)
    {
        if (!calculationFarm) return;
        if (applyOn == 0)
        {
            currFrame.kalmanAcc.x = kalmanX.Update(calculationFarm.usedAcceleration.x, Q, R);
            currFrame.kalmanAcc.y = kalmanY.Update(calculationFarm.usedAcceleration.y, Q, R);
            currFrame.kalmanAcc.z = kalmanZ.Update(calculationFarm.usedAcceleration.z, Q, R);
            currFrame.kalmanAcc = RemoveBaseNoise(currFrame.kalmanAcc, thresholdAcc);

            currFrame.kalmanVel += currFrame.kalmanAcc * calculationFarm.deltaTime;
            if (resetVelocity)
                currFrame.kalmanVel = ResetVelocity(currFrame.kalmanVel, currFrame.kalmanAcc);

            currFrame.kalmanPos += currFrame.kalmanVel * calculationFarm.deltaTime;
        }
        else if (applyOn == 1)
        {
            currFrame.kalmanAcc = calculationFarm.usedAcceleration;
            currFrame.kalmanAcc = RemoveBaseNoise(currFrame.kalmanAcc, thresholdAcc);
            if (currFrame.kalmanAcc != Vector3.zero)
            {
                currFrame.kalmanVel += currFrame.kalmanAcc * calculationFarm.deltaTime;
            }
            else if (resetVelocity)
            {
                currFrame.kalmanVel = Vector3.zero;
            }
            currFrame.kalmanVel.x = kalmanX.Update(currFrame.kalmanVel.x, Q, R);
            currFrame.kalmanVel.y = kalmanY.Update(currFrame.kalmanVel.y, Q, R);
            currFrame.kalmanVel.z = kalmanZ.Update(currFrame.kalmanVel.z, Q, R);
            currFrame.kalmanPos += currFrame.kalmanVel * calculationFarm.deltaTime;
        }
        else if (applyOn == 2)
        {
            currFrame.kalmanAcc = calculationFarm.usedAcceleration;
            currFrame.kalmanAcc = RemoveBaseNoise(currFrame.kalmanAcc, thresholdAcc);
            if (currFrame.kalmanAcc != Vector3.zero)
            {
                currFrame.kalmanVel += currFrame.kalmanAcc * calculationFarm.deltaTime;
            }
            else if (resetVelocity)
            {
                currFrame.kalmanVel = Vector3.zero;
            }
            currFrame.kalmanPos += currFrame.kalmanVel * calculationFarm.deltaTime;
            currFrame.kalmanPos.x = kalmanX.Update(currFrame.kalmanPos.x, Q, R);
            currFrame.kalmanPos.y = kalmanY.Update(currFrame.kalmanPos.y, Q, R);
            currFrame.kalmanPos.z = kalmanZ.Update(currFrame.kalmanPos.z, Q, R);
        }

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

    Vector3 ResetVelocity(Vector3 vel, Vector3 acc)
    {
        Vector3 computeVec = Vector3.zero;
        //if (acc.x != 0)
        //    computeVec.x = vel.x;
        //if (acc.y != 0)
        //    computeVec.y = vel.y;
        //if (acc.z != 0)
        //    computeVec.z = vel.z;
        if (acc != Vector3.zero)
            computeVec = vel;

        return computeVec;
    }

    protected override void Save()
    {
        calculationFarm.currKalmanFrame = currFrame;
        calculationFarm.currKalmanFrame.time = calculationFarm.time;
    }

    protected override void Display()
    {
        dataParent.acc = currFrame.kalmanAcc;
        dataParent.vel = currFrame.kalmanVel;
        dataParent.pos = currFrame.kalmanPos;
    }
    
    public override void ResetValue()
    {
        if (!calculationFarm) return;
        kalmanX.Reset();
        kalmanY.Reset();
        kalmanZ.Reset();
        currFrame.kalmanAcc = Vector3.zero;
        currFrame.kalmanVel = Vector3.zero;
        currFrame.kalmanPos = Vector3.zero;
    }
}
