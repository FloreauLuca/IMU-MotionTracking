using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KalmanData : MonoBehaviour
{
    private KalmanFilterFloat kalmanX;
    private KalmanFilterFloat kalmanY;
    private KalmanFilterFloat kalmanZ;

    private FusionKalmanFilter ekf = new FusionKalmanFilter();

    private CalculationFarm calculationFarm;
    private DataParent dataParent;


    [SerializeField] private float Q = 0.001f;
    [SerializeField] private float R = 0.01f;

    private Vector3 K;
    private Vector3 P;

    [SerializeField] private int applyOn = 0;

    // Start is called before the first frame update
    void Start()
    {
        calculationFarm = FindObjectOfType<CalculationFarm>();
        dataParent = GetComponent<DataParent>();
        kalmanX = new KalmanFilterFloat(Q, R);
        kalmanY = new KalmanFilterFloat(Q, R);
        kalmanZ = new KalmanFilterFloat(Q, R);

        ekf.Start(0.09f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //calculationFarm.currKalmanFrame.kalmanRawAcc.x = kalmanX.Update(calculationFarm.usedAcceleration.x);
        
        //calculationFarm.currKalmanFrame.kalmanRawAcc.y = kalmanY.Update(calculationFarm.usedAcceleration.y, Q, R);
        //calculationFarm.currKalmanFrame.kalmanRawAcc.z = kalmanZ.Update(calculationFarm.usedAcceleration.z, Q, R);
        //calculationFarm.currKalmanFrame.kalmanRawVel += calculationFarm.currKalmanFrame.kalmanRawAcc * calculationFarm.deltaTime;
        //calculationFarm.currKalmanFrame.kalmanRawPos += calculationFarm.currKalmanFrame.kalmanRawVel * calculationFarm.deltaTime;
        //calculationFarm.currKalmanFrame.kalmanK.x = kalmanX.K;
        //calculationFarm.currKalmanFrame.kalmanK.y = kalmanY.K;
        //calculationFarm.currKalmanFrame.kalmanK.z = kalmanZ.K;
        //calculationFarm.currKalmanFrame.kalmanP.x = kalmanX.P;
        //calculationFarm.currKalmanFrame.kalmanP.y = kalmanY.P;
        //calculationFarm.currKalmanFrame.kalmanP.z = kalmanZ.P;
        //K.x = kalmanX.K;
        //P.x = kalmanX.P;
        //calculationFarm.currKalmanFrame.kalmanQ.x = kalmanX.Q;
        //calculationFarm.currKalmanFrame.kalmanR.x = kalmanX.R;
        ////KalmanFilterFloat kalmanCompute = kalmanComputeX[kalmanIndex];
        ////if (calculationFarm.currProcessAccFrame.computeResetAcceleration.x == 0)
        ////{
        ////    calculationFarm.currKalmanFrame.kalmanComputeVel.x = 0;
        ////    kalmanCompute.Reset();
        ////}
        ////else
        ////{
        ////    calculationFarm.currKalmanFrame.kalmanComputeAcc.x = kalmanCompute.Update(calculationFarm.currProcessAccFrame.computeResetAcceleration.x);
        ////    calculationFarm.currKalmanFrame.kalmanComputeVel.x += calculationFarm.currKalmanFrame.kalmanComputeAcc.x * calculationFarm.deltaTime;
        ////}
        ////calculationFarm.currKalmanFrame.kalmanK.x = kalmanCompute.K;

        //calculationFarm.currKalmanFrame.ekfRawAcc = ekf.ProcessMesurement(calculationFarm.usedAcceleration, calculationFarm.time);
        //calculationFarm.currKalmanFrame.ekfRawVel += calculationFarm.currKalmanFrame.ekfRawAcc * calculationFarm.deltaTime;
        //calculationFarm.currKalmanFrame.ekfRawPos += calculationFarm.currKalmanFrame.ekfRawVel * calculationFarm.deltaTime;

        if (applyOn == 0)
        {
            dataParent.acc.x = kalmanX.Update(calculationFarm.usedAcceleration.x, Q, R);
            dataParent.acc.y = kalmanY.Update(calculationFarm.usedAcceleration.y, Q, R);
            dataParent.acc.z = kalmanZ.Update(calculationFarm.usedAcceleration.z, Q, R);
            dataParent.vel += dataParent.acc * calculationFarm.deltaTime;
            dataParent.pos += dataParent.vel * calculationFarm.deltaTime;
        } else if (applyOn == 1)
        {
            dataParent.acc = calculationFarm.usedAcceleration;
            dataParent.vel += dataParent.acc * calculationFarm.deltaTime;
            dataParent.vel.x = kalmanX.Update(dataParent.vel.x, Q, R);
            dataParent.vel.y = kalmanY.Update(dataParent.vel.y, Q, R);
            dataParent.vel.z = kalmanZ.Update(dataParent.vel.z, Q, R);
            dataParent.pos += dataParent.vel * calculationFarm.deltaTime;
        }
        else if (applyOn == 2)
        {
            dataParent.acc = calculationFarm.usedAcceleration;
            dataParent.vel += dataParent.acc * calculationFarm.deltaTime;
            dataParent.pos += dataParent.vel * calculationFarm.deltaTime;
            dataParent.pos.x = kalmanX.Update(dataParent.pos.x, Q, R);
            dataParent.pos.y = kalmanY.Update(dataParent.pos.y, Q, R);
            dataParent.pos.z = kalmanZ.Update(dataParent.pos.z, Q, R);
        }
    }

    public void ResetFilter()
    {
        kalmanX.Reset();
        kalmanY.Reset();
        kalmanZ.Reset();
        dataParent.acc = Vector3.zero;
        dataParent.vel = Vector3.zero;
        dataParent.pos = Vector3.zero;
    }
}
