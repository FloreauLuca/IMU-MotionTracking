using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataChildren : MonoBehaviour
{
    private enum AxesType
    {
        ACC,
        VEL,
        POS
    }

    [SerializeField] private AxesType displayAxes;

    public Vector3 GetData()
    {
        if (transform.parent.GetComponent<DataParent>())
        {
            switch (displayAxes)
            {
                case AxesType.ACC:
                    return transform.parent.GetComponent<DataParent>().acc;
                    break;
                case AxesType.VEL:
                    return transform.parent.GetComponent<DataParent>().vel;
                    break;
                case AxesType.POS:
                    return transform.parent.GetComponent<DataParent>().pos;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            Debug.Log("Parent not found");
        }

        return  Vector3.zero;
    }
}
