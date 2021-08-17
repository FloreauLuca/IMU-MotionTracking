using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class LocalToGlobal : MonoBehaviour
{
    public int testNb;

    public Vector3 definedAcc;

    private Vector3 usedAcceleration;

    private CalculationFarm calculationFarm;

    private Quaternion _origin = Quaternion.identity;

    void Start()
    {
        Input.gyro.enabled = true;
        getOrigin();
        calculationFarm = FindObjectOfType<CalculationFarm>();
    }

    void Update()
    {
        Vector3 newAcc = FromMEMSpaceToUnitySpace(calculationFarm.currRawAccFrame.userAcceleration);
        Quaternion newQuat =
            ConvertRightHandedToLeftHandedQuaternion(Quaternion.Inverse(_origin) * Input.gyro.attitude);
        newAcc = FromLocalToGlobal(newAcc, newQuat);
        newAcc *= 9.81f;
        calculationFarm.currGlobalAccFrame.globalAcc = newAcc;
        
        //switch (testNb)
        //{
        //    case 0:
        //        transform.position = Input.acceleration;
        //        break;
        //    case 1:
        //        transform.position = newAcc;
        //        break;
        //    case 2:
        //        transform.position = FromLocalToGlobal(Input.acceleration, Input.gyro.attitude);
        //        break;
        //    case 3:
        //        transform.position = FromLocalToGlobal(Input.acceleration, transform.rotation);
        //        break;
        //    case 4:
        //        transform.position = FromLocalToGlobal(definedAcc, Input.gyro.attitude);
        //        break;
        //    case 5:
        //        transform.position = FromLocalToGlobal(definedAcc, transform.rotation);
        //        break;
        //    case 6:
        //        transform.position = FromLocalToGlobal(newAcc, Input.gyro.attitude);
        //        break;
        //    case 7:
        //        transform.position = FromLocalToGlobal(newAcc, transform.rotation);
        //        break;
        //    default:
        //        break;
        //}
    }

    Vector3 FromMEMSpaceToUnitySpace(Vector3 rawAcc)
    {
        return new Vector3(-rawAcc.x, -rawAcc.z, -rawAcc.y);
    }

    Vector3 FromLocalToGlobal(Vector3 localAcc, Quaternion rotation)
    {
        Matrix4x4 rotMat = Matrix4x4.Rotate(rotation);
        Vector3 globalAcc = rotMat.MultiplyPoint3x4(localAcc);
        return globalAcc;
    }
    
    private void getOrigin()
    {
        _origin = Input.gyro.attitude;
    }
    
    public static Quaternion ConvertRightHandedToLeftHandedQuaternion(Quaternion rightHandedQuaternion)
    {
        return new Quaternion(-rightHandedQuaternion.x,
            -rightHandedQuaternion.z,
            -rightHandedQuaternion.y,
            rightHandedQuaternion.w);
    }
}
