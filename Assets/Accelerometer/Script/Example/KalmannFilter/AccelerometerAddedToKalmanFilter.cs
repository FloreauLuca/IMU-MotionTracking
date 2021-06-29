using System.Collections;
using System.Collections.Generic;
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
    private CalculationFarm calculationFarm;

    [SerializeField] private float Q = 0.001f;
    [SerializeField] private float R = 0.01f;

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
    }

    // Update is called once per frame
    void Update()
    {
        KalmanFilterFloat kalman = kalmanX[kalmanIndex];
        calculationFarm.kalmanAcceleration.x = kalman.Update(calculationFarm.rawAcceleration.x);
        calculationFarm.kalmanAcceleration.y = kalmanY.Update(calculationFarm.rawAcceleration.y, Q, R);
        calculationFarm.kalmanAcceleration.z = kalmanZ.Update(calculationFarm.rawAcceleration.z, Q, R);
        calculationFarm.kalmanVelocity += calculationFarm.kalmanAcceleration * Time.deltaTime;
        calculationFarm.kalmanK.x = kalman.K;
        calculationFarm.kalmanK.y = kalmanY.K;
        calculationFarm.kalmanK.z = kalmanZ.K;
        calculationFarm.kalmanP.x = kalman.P;
        calculationFarm.kalmanP.y = kalmanY.P;
        calculationFarm.kalmanP.z = kalmanZ.P;
        K.x = kalman.K;
        P.x = kalman.P;
        calculationFarm.kalmanQ.x = kalman.Q;
        calculationFarm.kalmanR.x = kalman.R;
        KalmanFilterFloat kalmanCompute = kalmanComputeX[kalmanIndex];
        if (calculationFarm.computeResetAcceleration.x == 0)
        {
            calculationFarm.kalmanComputeVelocity.x = 0;
            kalmanCompute.Reset();
        }
        else
        {
            calculationFarm.kalmanComputeAcceleration.x = kalmanCompute.Update(calculationFarm.computeResetAcceleration.x);
            calculationFarm.kalmanComputeVelocity.x += calculationFarm.kalmanComputeAcceleration.x * Time.deltaTime;
        }
        calculationFarm.kalmanK.x = kalmanCompute.K;
    }

    public void ResetFilter()
    {
        kalmanX[kalmanIndex].Reset();
        kalmanY.Reset();
        kalmanZ.Reset();
        calculationFarm.kalmanVelocity = Vector3.zero;
    }

    public void Switch()
    {
        ResetFilter();
        kalmanIndex++;
        ResetFilter();
        kalmanIndex %= kalmanX.Length;
    }
}
