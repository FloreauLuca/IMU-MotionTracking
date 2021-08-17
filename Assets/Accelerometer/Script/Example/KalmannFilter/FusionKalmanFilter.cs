using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UE.Math;
using UnityEngine;

using VecX = Matrix;

public class FusionKalmanFilter
{

    private ExtendedKalmanFilter ekf;
    private bool isInitialized;
    private float previous_timestamp;
    private Matrix r_mesure;
    private Matrix hj_;
    private float noise_ax;
    private float noise_ay;

    public void Start(float covariance)
    {
        ekf = new ExtendedKalmanFilter();
        isInitialized = false;
        previous_timestamp = 0;

        r_mesure = new Matrix(4, 4);

        hj_ = new Matrix(3, 4);

        r_mesure.mat = new double[,]{ { covariance, 0, 0, 0 }, { 0, covariance, 0 , 0},{ 0, 0, covariance, 0 }, { 0, 0, 0, 0 } };
        ekf.F = new Matrix(4, 4);
        ekf.P = new Matrix(4, 4);
        ekf.Q = new Matrix(4, 4);

        noise_ax = 9;
        noise_ay = 9;
    }

    public Vector3 ProcessMesurement(Vector3 acc, float time)
    {
        if (!isInitialized)
        {
            ekf.x = new VecX(new Vector4(1, 1, 0.5f, 0.5f));

            float rho = acc.x;
            float theta = acc.y;
            float rho_dot = acc.z;

            ekf.x[0, 0] = rho * Mathf.Cos(theta);
            ekf.x[1, 0] = rho * Mathf.Sin(theta);

            previous_timestamp = time;

            ekf.F.mat = new double[,] { { 1, 0, 1, 0 }, { 0, 1, 0, 1 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            ekf.P.mat = new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 500, 0 }, { 0, 0, 0, 500 } };
            isInitialized = true;
            Debug.Log("Completed initialization of FusionEKF.");
            return Vector3.zero;
        }

        float dt = (time - previous_timestamp);
        float dt2 = dt * dt;
        float dt3 = dt2 * dt;
        float dt4 = dt3 * dt;

        previous_timestamp = time;

        ekf.F[0, 2] = dt;
        ekf.F[1, 3] = dt;

        ekf.Q.mat = new double[,]
        {
            {dt4/4*noise_ax, 0, dt3/2*noise_ax, 0 },
            { 0, dt4/4*noise_ay, 0, dt3/2*noise_ay },
            {  dt3/2*noise_ax, 0, dt2*noise_ax, 0 }, 
            { 0, dt3/2*noise_ay, 0, dt2*noise_ay }
        };

        ekf.Predict();

        hj_ = CalculateJacobian(ekf.x);
        ekf.H = hj_;

        ekf.R = new Matrix(4, 4);
        ekf.R = r_mesure;
        ekf.UpdateEKF(new Matrix(acc));

        return new Vector3((float)ekf.x[0, 0], (float)ekf.x[1, 0], (float)ekf.x[2, 0]);
    }

    private Matrix CalculateJacobian(VecX x_state)
    {
        Matrix Hj = new Matrix(3, 4);

        float px = (float)x_state[0, 0];
        float py = (float)x_state[1, 0];
        float vx = (float)x_state[2, 0];
        float vy = (float)x_state[3, 0];

        Hj = new Matrix(4, 4);

        float rho = Mathf.Pow((px * px + py * py), 0.5f);
        if (rho < 0.0001f)
        {
            Debug.Log("Value of rho too small - possible div by 0. Reassigning rho = 0.0005");
            rho = 0.0001f;
        }

        float inv_rho = 1 / rho;

        Hj[0, 0] = px * inv_rho;
        Hj[1, 0] = -py * Mathf.Pow(inv_rho, 2);
        Hj[2, 0] = py * (vx * py - vy * px) * Mathf.Pow(inv_rho, 3);
        Hj[0, 1] = py * inv_rho;
        Hj[1, 1] = px * Mathf.Pow(inv_rho, 2);
        Hj[2, 1] = px * (vy * px - vx * py) * Mathf.Pow(inv_rho, 3);
        Hj[2, 2] = Hj[0, 0];
        Hj[2, 3] = Hj[0, 1];
        return Hj;
    }
}

