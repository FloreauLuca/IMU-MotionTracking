using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculationAlgo : MonoBehaviour
{
    protected NewCalculationFarm calculationFarm;

    protected DataParent dataParent;

    protected void Start()
    {
        calculationFarm = FindObjectOfType<NewCalculationFarm>();
        dataParent = GetComponent<DataParent>();
    }

    [SerializeField] protected bool toSave;
    public virtual void UpdateData(float deltaTime)
    {
        if (toSave)
        {
            Save();
        }
        if (dataParent)
        {
            Display();
        }
    }

    protected virtual void Save()
    {

    }

    protected virtual void Display()
    {

    }

    public virtual void ResetValue()
    {

    }
}
