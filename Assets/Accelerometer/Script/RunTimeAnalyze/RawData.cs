using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawData : MonoBehaviour
{
    private DataParent dataParent;

    private CalculationFarm calculationFarm;

    void Start()
    {
        calculationFarm = FindObjectOfType<CalculationFarm>();
        dataParent = GetComponent<DataParent>();
    }

    void Update()
    {
        dataParent.acc = calculationFarm.currRawAccFrame.userAcceleration;
        dataParent.vel = calculationFarm.currRawAccFrame.rawVelocity;
        dataParent.pos = calculationFarm.currRawAccFrame.rawPosition;
    }
}
