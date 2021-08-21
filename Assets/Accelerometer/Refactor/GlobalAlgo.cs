using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAlgo : CalculationAlgo
{
    private GlobalFrame currFrame;

    private Quaternion _origin = Quaternion.identity;

    void Start()
    {
        base.Start();
        getOrigin();
    }

    public override void UpdateData(float deltaTime)
    {
        Vector3 newAcc = FromMEMSpaceToUnitySpace(calculationFarm.currRawAccFrame.userAcceleration);
        Quaternion newQuat =
            ConvertRightHandedToLeftHandedQuaternion(Quaternion.Inverse(_origin) * calculationFarm.currRawGyrFrame.attitude);
        newAcc = FromLocalToGlobal(newAcc, newQuat);
        newAcc *= 9.81f;
        currFrame.globalAcc = newAcc;

        currFrame.globalVel += currFrame.globalAcc * deltaTime;
        currFrame.globalPos += currFrame.globalVel * deltaTime;

        base.UpdateData(deltaTime);
    }

    protected override void Save()
    {
        calculationFarm.currGlobalAccFrame = currFrame;
    }

    protected override void Display()
    {
        dataParent.acc = currFrame.globalAcc;
        dataParent.vel = currFrame.globalVel;
        dataParent.pos = currFrame.globalPos;
    }
    
    public override void ResetValue()
    {
        currFrame.globalAcc = Vector3.zero;
        currFrame.globalVel = Vector3.zero;
        currFrame.globalPos = Vector3.zero;
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
