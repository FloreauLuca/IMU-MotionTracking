using System.Collections;
using System.Collections.Generic;
using old;
using UnityEngine;

public class RCData : MonoBehaviour
{
    [SerializeField] private float RCHighPassAcc = 1.0f;
    [SerializeField] private float RCLowPassAcc = 1.0f;
    [SerializeField] private float RCHighPassVel = 1.0f;
    [SerializeField] private float RCLowPassVel = 1.0f;
    [SerializeField] private float RCHighPassPos = 1.0f;
    [SerializeField] private float RCLowPassPos = 1.0f;

    [SerializeField] private float thresholdAcc = 0.1f;
    [SerializeField] private float thresholdVel = 0.1f;

    private CalculationFarm calculationFarm;
    private DataParent dataParent;

    private Vector3 prevRcAcc;
    private Vector3 prevRcVel;
    private Vector3 prevRcPos;

    private Vector3 rcAcc;
    private Vector3 rcVel;
    private Vector3 rcPos;

    private Vector3 prevRawAcc;
    private Vector3 prevRawVel;
    private Vector3 prevRawPos;

    private Vector3 rawAcc;
    private Vector3 rawVel;
    private Vector3 rawPos;

    void Start()
    {
        calculationFarm = FindObjectOfType<CalculationFarm>();
        dataParent = GetComponent<DataParent>();
    }

    void FixedUpdate()
    {
        rawAcc = calculationFarm.usedAcceleration;

        //rcAcc = HighPassFilter.ComputeRC(rawAcc, prevRawAcc, prevRcAcc, Time.fixedDeltaTime, RCHighPassAcc);
        rcAcc = LowPassFilter.ComputeRC(rawAcc, prevRcAcc, calculationFarm.deltaTime, RCLowPassAcc);
        if (Mathf.Abs(rawAcc.magnitude) > thresholdAcc)
        {
            rawVel = rcAcc * calculationFarm.deltaTime + rcVel;
        }
        else
        {
            rawVel = Vector3.zero;
        }

        rcVel = rawVel;
        rcVel = HighPassFilter.ComputeRC(rawVel, prevRawVel, prevRcVel, calculationFarm.deltaTime, RCHighPassVel);
        if (Mathf.Abs(rcVel.magnitude) > thresholdVel)
        {
            rawPos = rcVel * calculationFarm.deltaTime + rawPos;
        }

        rcPos = rawPos;
        //rcPos = HighPassFilter.ComputeRC(rawPos, prevRawPos, prevRcPos, calculationFarm.deltaTime, RCHighPassPos);


        prevRawAcc = rawAcc;
        prevRawVel = rawVel;
        prevRawPos = rawPos;

        prevRcAcc = rcAcc;
        prevRcVel = rcVel;
        prevRcPos = rcPos;
        
        dataParent.acc = rawVel;
        dataParent.vel = rcVel;
        dataParent.pos = rcPos;
    }

    public void Reset()
    {
        rcAcc = Vector3.zero;
        rcVel = Vector3.zero;
        rcPos = Vector3.zero;

        prevRcAcc = Vector3.zero;
        prevRcVel = Vector3.zero;
        prevRcPos = Vector3.zero;

        prevRawAcc = Vector3.zero;
        prevRawVel = Vector3.zero;
        prevRawPos = Vector3.zero;

        dataParent.acc = Vector3.zero;
        dataParent.vel = Vector3.zero;
        dataParent.pos = Vector3.zero;
    }
}
