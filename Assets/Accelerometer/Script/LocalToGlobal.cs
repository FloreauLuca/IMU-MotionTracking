using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalToGlobal : MonoBehaviour
{
    public int testNb;

    public Vector3 acc;

    void Update()
    {
        Vector3 newAcc = new Vector3(-Input.acceleration.x, -Input.acceleration.z, -Input.acceleration.y);
        switch (testNb)
        {
            case 0:
                transform.position = Input.acceleration;
                break;
            case 1:
                transform.position = newAcc;
                break;
            case 2:
                transform.position = FromLocalToGlobal(Input.acceleration, Input.gyro.attitude);
                break;
            case 3:
                transform.position = FromLocalToGlobal(Input.acceleration, transform.rotation);
                break;
            case 4:
                transform.position = FromLocalToGlobal(acc, Input.gyro.attitude);
                break;
            case 5:
                transform.position = FromLocalToGlobal(acc, transform.rotation);
                break;
            case 6:
                transform.position = FromLocalToGlobal(newAcc, Input.gyro.attitude);
                break;
            case 7:
                transform.position = FromLocalToGlobal(newAcc, transform.rotation);
                break;
            default:
                break;
        }
    }
    
    Vector3 FromLocalToGlobal(Vector3 localAcc, Quaternion rotation)
    {
        Matrix4x4 rotMat = Matrix4x4.Rotate(rotation);
        Vector3 globalAcc = rotMat.MultiplyPoint3x4(localAcc);
        return globalAcc;
    }
}
