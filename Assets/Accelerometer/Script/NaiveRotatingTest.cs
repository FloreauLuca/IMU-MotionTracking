using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//https://answers.unity.com/questions/1552608/unity-c-android-gyro-wrong-orientation.html

public class NaiveRotatingTest : MonoBehaviour
{
    private Quaternion _origin = Quaternion.identity;

    private void getOrigin()
    {
        _origin = Input.gyro.attitude;
    }

    private void Start()
    {
        Input.gyro.enabled = true;
        getOrigin();
    }

    void Update()
    {
        transform.rotation = ConvertRightHandedToLeftHandedQuaternion(Quaternion.Inverse(_origin) * Input.gyro.attitude);
    }

    private Quaternion ConvertRightHandedToLeftHandedQuaternion(Quaternion rightHandedQuaternion)
    {
        return new Quaternion(-rightHandedQuaternion.x,
            -rightHandedQuaternion.z,
            -rightHandedQuaternion.y,
            rightHandedQuaternion.w);
    }
}
