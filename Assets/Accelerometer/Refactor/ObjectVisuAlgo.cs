using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectVisuAlgo : CalculationAlgo
{
    private GlobalFrame currFrame;

    private Quaternion _origin = Quaternion.identity;

    [SerializeField] private float increaseAmplitude = 1.0f;

    [SerializeField] private bool trackPos = true;
    [SerializeField] private bool trackRot = true;

    void Start()
    {
        base.Start();
    }

    public override void UpdateData(float deltaTime)
    {
        if (trackPos)
            transform.position = calculationFarm.currComputeFrame.computePos * increaseAmplitude;
        if (trackRot)
            transform.rotation = ConvertRightHandedToLeftHandedQuaternion(Quaternion.Inverse(_origin) * calculationFarm.currRawGyrFrame.attitude);
        base.UpdateData(deltaTime);
    }
    
    public override void ResetValue()
    {
        getOrigin();
        currFrame.globalAcc = Vector3.zero;
        currFrame.globalVel = Vector3.zero;
        currFrame.globalPos = Vector3.zero;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
    
    private void getOrigin()
    {
        _origin = calculationFarm.currRawGyrFrame.attitude;
    }

    public static Quaternion ConvertRightHandedToLeftHandedQuaternion(Quaternion rightHandedQuaternion)
    {
        return new Quaternion(-rightHandedQuaternion.x,
            -rightHandedQuaternion.z,
            -rightHandedQuaternion.y,
            rightHandedQuaternion.w);
    }
}
