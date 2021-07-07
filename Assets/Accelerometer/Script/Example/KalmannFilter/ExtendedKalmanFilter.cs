using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VecX = Matrix;

//https://github.com/shazraz/Extended-Kalman-Filter/blob/master/src/kalman_filter.cpp

public class ExtendedKalmanFilter
{
    //state vector
    public VecX x;

    //state convariance matrix
    public Matrix P;

    //state transition matrix
    public Matrix F;

    //process covariance matrix
    public Matrix Q;

    //measurement matrix
    public Matrix H;

    //measurement covariance matrix
    public Matrix R;

    void Init(VecX x_in, Matrix P_in, Matrix F_in, Matrix H_in, Matrix R_in, Matrix Q_in)
    {
        x = x_in;
        P = P_in;
        F = F_in;
        H = H_in;
        R = R_in;
        Q = Q_in;
    }

    public void Predict()
    {
        //Use the state using the state transition matrix
        x = F * x;
        //Update the covariance matrix using the process noise and state transition matrix
        Matrix Ft = Matrix.Transpose(F);
        P = F * P * Ft + Q;
    }

    public void UpdateSimple(VecX z)
    {
        Matrix Ht = Matrix.Transpose(H);
        Matrix PHt = P * Ht;

        VecX y = z - H * x;
        Matrix S = H * PHt + R;
        Matrix K = PHt * S.Invert();

        //Update State
        x = x + (K * y);
        //Update covariance matrix
        int x_size = x.rows;
        Matrix I = Matrix.IdentityMatrix(x_size, x_size);
        P = (I - K * H) * P;
    }

    public void UpdateEKF(VecX z)
    {
        float px = (float)x[0, 0];
        float py = (float)x[1, 0];
        float vx = (float)x[2, 0];
        float vy = (float)x[3, 0];

        //Convert the predictions into polar coordinates
        float rho_p = Mathf.Sqrt(px * px + py * py);
        float theta_p = Mathf.Atan2(py, px);

        if (rho_p < 0.0001f)
        {
            Debug.Log("Small prediction value - reassigning Rho_p to 0.0005 to avoid division by zero");
            rho_p = 0.0001f;
        }

        float rho_dot_p = (px * vx + py * vy) / rho_p;

        VecX z_pred = new VecX(new Vector3(rho_p, theta_p, rho_dot_p));

        VecX y = z - z_pred;

        //Adjust the value of theta if it is outside of [-PI, PI]
        if (y[1, 0] > Mathf.PI)
        {
            y[1, 0] = y[1, 0] - 2 * Mathf.PI;
        }

        else if (y[1, 0] < -Mathf.PI)
        {
            y[1, 0] = y[1, 0] + 2 * Mathf.PI;
        }

        Matrix Ht = Matrix.Transpose(H);
        Matrix PHt = P * Ht;

        Matrix S = H * PHt + R;
        Matrix K = PHt * S.Invert();

        //Update State
        x = x + (K * y);
        //Update covariance matrix
        int x_size = x.rows;
        Matrix I = Matrix.IdentityMatrix(x_size, x_size);
        P = (I - K * H) * P;
    }
}
