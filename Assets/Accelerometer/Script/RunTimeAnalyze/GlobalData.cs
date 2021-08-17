using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour
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
        dataParent.acc = calculationFarm.currGlobalAccFrame.globalAcc;
        dataParent.vel = calculationFarm.currGlobalAccFrame.globalVelocity;
        dataParent.pos = calculationFarm.currGlobalAccFrame.globalPos;
    }
}
