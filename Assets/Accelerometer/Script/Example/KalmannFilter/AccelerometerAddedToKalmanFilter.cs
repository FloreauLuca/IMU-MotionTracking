using System.Collections;
using System.Collections.Generic;
using old;
using UnityEngine;

public class AccelerometerAddedToKalmanFilter : MonoBehaviour
{
    private KalmanFilterFloat[] kalmanX = new KalmanFilterFloat[]
    {
        new KalmanFilterFloat(0.001f, 0.0001f),
        new KalmanFilterFloat(0.001f, 0.01f),
        new KalmanFilterFloat(0.001f, 0.1f),
        new KalmanFilterFloat(0.001f, 1f),
        new KalmanFilterFloat(0.001f, 2f),
        new KalmanFilterFloat(0.001f, 5f),
        new KalmanFilterFloat(0.01f, 0.0001f),
        new KalmanFilterFloat(0.01f, 0.01f),
        new KalmanFilterFloat(0.01f, 0.1f),
        new KalmanFilterFloat(0.01f, 1f),
        new KalmanFilterFloat(0.01f, 2f),
        new KalmanFilterFloat(0.01f, 5f),
        new KalmanFilterFloat(0.1f, 0.0001f),
        new KalmanFilterFloat(0.1f, 0.01f),
        new KalmanFilterFloat(0.1f, 0.1f),
        new KalmanFilterFloat(0.1f, 1f),
        new KalmanFilterFloat(0.1f, 2f),
        new KalmanFilterFloat(0.1f, 5f),
        new KalmanFilterFloat(1f, 0.0001f),
        new KalmanFilterFloat(1f, 0.01f),
        new KalmanFilterFloat(1f, 0.1f),
        new KalmanFilterFloat(1f, 1f),
        new KalmanFilterFloat(1f, 2f),
        new KalmanFilterFloat(1f, 5f),
        new KalmanFilterFloat(2f, 0.0001f),
        new KalmanFilterFloat(2f, 0.01f),
        new KalmanFilterFloat(2f, 0.1f),
        new KalmanFilterFloat(2f, 1f),
        new KalmanFilterFloat(2f, 2f),
        new KalmanFilterFloat(2f, 5f),
        new KalmanFilterFloat(5f, 0.0001f),
        new KalmanFilterFloat(5f, 0.01f),
        new KalmanFilterFloat(5f, 0.1f),
        new KalmanFilterFloat(5f, 1f),
        new KalmanFilterFloat(5f, 2f),
        new KalmanFilterFloat(5f, 5f),
    };
    private KalmanFilterFloat[] kalmanComputeX = new KalmanFilterFloat[]
    {
        new KalmanFilterFloat(0.001f, 0.0001f),
        new KalmanFilterFloat(0.001f, 0.01f),
        new KalmanFilterFloat(0.001f, 0.1f),
        new KalmanFilterFloat(0.001f, 1f),
        new KalmanFilterFloat(0.001f, 2f),
        new KalmanFilterFloat(0.001f, 5f),
        new KalmanFilterFloat(0.01f, 0.0001f),
        new KalmanFilterFloat(0.01f, 0.01f),
        new KalmanFilterFloat(0.01f, 0.1f),
        new KalmanFilterFloat(0.01f, 1f),
        new KalmanFilterFloat(0.01f, 2f),
        new KalmanFilterFloat(0.01f, 5f),
        new KalmanFilterFloat(0.1f, 0.0001f),
        new KalmanFilterFloat(0.1f, 0.01f),
        new KalmanFilterFloat(0.1f, 0.1f),
        new KalmanFilterFloat(0.1f, 1f),
        new KalmanFilterFloat(0.1f, 2f),
        new KalmanFilterFloat(0.1f, 5f),
        new KalmanFilterFloat(1f, 0.0001f),
        new KalmanFilterFloat(1f, 0.01f),
        new KalmanFilterFloat(1f, 0.1f),
        new KalmanFilterFloat(1f, 1f),
        new KalmanFilterFloat(1f, 2f),
        new KalmanFilterFloat(1f, 5f),
        new KalmanFilterFloat(2f, 0.0001f),
        new KalmanFilterFloat(2f, 0.01f),
        new KalmanFilterFloat(2f, 0.1f),
        new KalmanFilterFloat(2f, 1f),
        new KalmanFilterFloat(2f, 2f),
        new KalmanFilterFloat(2f, 5f),
        new KalmanFilterFloat(5f, 0.0001f),
        new KalmanFilterFloat(5f, 0.01f),
        new KalmanFilterFloat(5f, 0.1f),
        new KalmanFilterFloat(5f, 1f),
        new KalmanFilterFloat(5f, 2f),
        new KalmanFilterFloat(5f, 5f),
    };
    private KalmanFilterFloat kalmanY;
    private KalmanFilterFloat kalmanZ;

    private FusionKalmanFilter ekf = new FusionKalmanFilter();

    private CalculationFarm calculationFarm;

    [SerializeField] private float Q = 0.001f;
    [SerializeField] private float R = 0.01f;
    [SerializeField] private bool useKalmanIndex = false;

    private Vector3 K;
    private Vector3 P;

    [SerializeField][Range(0,36)]private int kalmanIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        calculationFarm = FindObjectOfType<CalculationFarm>();
        //kalmanX = new KalmanFilterFloat(Q, R);
        kalmanY = new KalmanFilterFloat(Q, R);
        kalmanZ = new KalmanFilterFloat(Q, R);

        ekf.Start(0.09f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        KalmanFilterFloat kalman = kalmanX[kalmanIndex];
        if (useKalmanIndex)
        {
            calculationFarm.currKalmanFrame.kalmanRawAcc.x = kalman.Update(calculationFarm.usedAcceleration.x);
        }
        else
        {
            calculationFarm.currKalmanFrame.kalmanRawAcc.x = kalman.Update(calculationFarm.usedAcceleration.x, Q, R);
        }
        calculationFarm.currKalmanFrame.kalmanRawAcc.y = kalmanY.Update(calculationFarm.usedAcceleration.y, Q, R);
        calculationFarm.currKalmanFrame.kalmanRawAcc.z = kalmanZ.Update(calculationFarm.usedAcceleration.z, Q, R);
        calculationFarm.currKalmanFrame.kalmanRawVel += calculationFarm.currKalmanFrame.kalmanRawAcc * calculationFarm.deltaTime;
        calculationFarm.currKalmanFrame.kalmanRawPos += calculationFarm.currKalmanFrame.kalmanRawVel * calculationFarm.deltaTime;
        calculationFarm.currKalmanFrame.kalmanK.x = kalman.K;
        calculationFarm.currKalmanFrame.kalmanK.y = kalmanY.K;
        calculationFarm.currKalmanFrame.kalmanK.z = kalmanZ.K;
        calculationFarm.currKalmanFrame.kalmanP.x = kalman.P;
        calculationFarm.currKalmanFrame.kalmanP.y = kalmanY.P;
        calculationFarm.currKalmanFrame.kalmanP.z = kalmanZ.P;
        K.x = kalman.K;
        P.x = kalman.P;
        calculationFarm.currKalmanFrame.kalmanQ.x = kalman.Q;
        calculationFarm.currKalmanFrame.kalmanR.x = kalman.R;
        KalmanFilterFloat kalmanCompute = kalmanComputeX[kalmanIndex];
        if (calculationFarm.currProcessAccFrame.computeResetAcceleration.x == 0)
        {
            calculationFarm.currKalmanFrame.kalmanComputeVel.x = 0;
            kalmanCompute.Reset();
        }
        else
        {
            calculationFarm.currKalmanFrame.kalmanComputeAcc.x = kalmanCompute.Update(calculationFarm.currProcessAccFrame.computeResetAcceleration.x);
            calculationFarm.currKalmanFrame.kalmanComputeVel.x += calculationFarm.currKalmanFrame.kalmanComputeAcc.x * calculationFarm.deltaTime;
        }
        calculationFarm.currKalmanFrame.kalmanK.x = kalmanCompute.K;

        calculationFarm.currKalmanFrame.ekfRawAcc = ekf.ProcessMesurement(calculationFarm.usedAcceleration, calculationFarm.time);
        calculationFarm.currKalmanFrame.ekfRawVel += calculationFarm.currKalmanFrame.ekfRawAcc * calculationFarm.deltaTime;
        calculationFarm.currKalmanFrame.ekfRawPos += calculationFarm.currKalmanFrame.ekfRawVel * calculationFarm.deltaTime;
    }

    public void ResetFilter()
    {
        kalmanX[kalmanIndex].Reset();
        kalmanY.Reset();
        kalmanZ.Reset();
        calculationFarm.currKalmanFrame.kalmanRawVel = Vector3.zero;
        calculationFarm.currKalmanFrame.kalmanRawPos = Vector3.zero;
    }

    public void Switch()
    {
        ResetFilter();
        kalmanIndex++;
        ResetFilter();
        kalmanIndex %= kalmanX.Length;
    }
}
