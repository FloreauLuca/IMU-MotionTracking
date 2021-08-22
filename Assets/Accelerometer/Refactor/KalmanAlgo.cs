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
            currFrame.kalmanVel += currFrame.kalmanAcc * calculationFarm.deltaTime;
            currFrame.kalmanPos += currFrame.kalmanVel * calculationFarm.deltaTime;
        }
        else if (applyOn == 1)
        {
            currFrame.kalmanAcc = calculationFarm.usedAcceleration;
            currFrame.kalmanVel += currFrame.kalmanAcc * calculationFarm.deltaTime;
            currFrame.kalmanVel.x = kalmanX.Update(currFrame.kalmanVel.x, Q, R);
            currFrame.kalmanVel.y = kalmanY.Update(currFrame.kalmanVel.y, Q, R);
            currFrame.kalmanVel.z = kalmanZ.Update(currFrame.kalmanVel.z, Q, R);
            currFrame.kalmanPos += currFrame.kalmanVel * calculationFarm.deltaTime;
        }
        else if (applyOn == 2)
        {
            currFrame.kalmanAcc = calculationFarm.usedAcceleration;
            currFrame.kalmanVel += currFrame.kalmanAcc * calculationFarm.deltaTime;
            currFrame.kalmanPos += currFrame.kalmanVel * calculationFarm.deltaTime;
            currFrame.kalmanPos.x = kalmanX.Update(currFrame.kalmanPos.x, Q, R);
            currFrame.kalmanPos.y = kalmanY.Update(currFrame.kalmanPos.y, Q, R);
            currFrame.kalmanPos.z = kalmanZ.Update(currFrame.kalmanPos.z, Q, R);
        }

        base.UpdateData(deltaTime);
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
        kalmanX.Reset();
        kalmanY.Reset();
        kalmanZ.Reset();
        currFrame.kalmanAcc = Vector3.zero;
        currFrame.kalmanVel = Vector3.zero;
        currFrame.kalmanPos = Vector3.zero;
    }
}
