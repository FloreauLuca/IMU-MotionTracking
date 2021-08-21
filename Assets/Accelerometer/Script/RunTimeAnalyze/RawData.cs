using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawData : MonoBehaviour
{
    private DataParent dataParent;

    private NewCalculationFarm calculationFarm;

    void Start()
    {
        calculationFarm = FindObjectOfType<NewCalculationFarm>();
        dataParent = GetComponent<DataParent>();
    }

    void Update()
    {
        dataParent.acc = calculationFarm.currRawAccFrame.userAcceleration;
        dataParent.vel = calculationFarm.currRawAccFrame.rawVel;
        dataParent.pos = calculationFarm.currRawAccFrame.rawPos;
    }
}
