using System.Collections;
using System.Collections.Generic;
using UE.Math;
using UnityEngine;

//https://github.com/shazraz/Extended-Kalman-Filter/blob/master/src/kalman_filter.cpp

public class ExtendedKalmanFilter
{
    //state vector
    private Vector3 x;

    //state convariance matrix
    private Matrix3x3 P;

    //state transition matrix
    private Matrix3x3 F;

    //process covariance matrix
    private Matrix3x3 Q;

    //measurement matrix
    private Matrix3x3 H;

    //measurement covariance matrix
    private Matrix3x3 R;

    void Init(Vector3 x_in, Matrix3x3 P_in, Matrix3x3 F_in, Matrix3x3 H_in, Matrix3x3 R_in, Matrix3x3 Q_in)
    {
        x = x_in;
        P = P_in;
        F = F_in;
        H = H_in;
        R = R_in;
        Q = Q_in;
    }

    private void Predict()
    {
        //Use the state using the state transition matrix
        x = F * x;
        //Update the covariance matrix using the process noise and state transition matrix
        Matrix3x3 Ft = F.transpose;
        P = F * P * Ft + Q;
    }

    private void UpdateSimple(Vector3 z)
    {
        Matrix3x3 Ht = H.transpose;
        Matrix3x3 PHt = P * Ht;

        Vector3 y = z - H * x;
        Matrix3x3 S = H * PHt + R;
        Matrix3x3 K = PHt * S.inverse;

        //Update State
        x = x + (K * y);
        //Update covariance matrix
        Matrix3x3 I = Matrix3x3.identity;
        P = (I - K * H) * P;
    }

    private void UpdateEKF(Vector3 z)
    {
        float px = x.x;
        float py = x.y;
        float vx = x.z;
        float vy = 1;

        //Convert the predictions into polar coordinates
        float rho_p = Mathf.Sqrt(px * px + py * py);
        float theta_p = Mathf.Atan2(py, px);

        if (rho_p < 0.0001f)
        {
            Debug.Log("Small prediction value - reassigning Rho_p to 0.0005 to avoid division by zero");
            rho_p = 0.0001f;
        }

        float rho_dot_p = (px * vx + py * vy) / rho_p;

        Vector3 z_pred = new Vector3(rho_p, theta_p, rho_dot_p);

        Vector3 y = z - z_pred;

        //Adjust the value of theta if it is outside of [-PI, PI]
        if (y.y > Mathf.PI)
        {
            y.y = y.y - 2 * Mathf.PI;
        }

        else if (y.y < -Mathf.PI)
        {
            y.y = y.y + 2 * Mathf.PI;
        }

        Matrix3x3 Ht = H.transpose;
        Matrix3x3 PHt = P * Ht;

        Matrix3x3 S = H * PHt + R;
        Matrix3x3 K = PHt * S.inverse;

        //Update State
        x = x + (K * y);
        //Update covariance matrix
        Matrix3x3 I = Matrix3x3.identity;
        P = (I - K * H) * P;
    }
}
