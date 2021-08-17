using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeData : MonoBehaviour
{
    private DataParent dataParent;

    private CalculationFarm calculationFarm;

    [SerializeField] private float thresholdAcc = 0.1f;

    void Start()
    {
        calculationFarm = FindObjectOfType<CalculationFarm>();
        dataParent = GetComponent<DataParent>();
    }


    void Update()
    { 
        //Remove init delta
        dataParent.acc = calculationFarm.usedAcceleration;
        //Remove Standard noise
        dataParent.acc = RemoveBaseNoise(dataParent.acc, thresholdAcc);
        dataParent.vel += dataParent.acc * calculationFarm.deltaTime;
        //dataParent.pos += dataParent.vel * calculationFarm.deltaTime;

        if (dataParent.acc == Vector3.zero)
        {
            dataParent.vel = Vector3.zero;
        }
        dataParent.pos += dataParent.vel * calculationFarm.deltaTime;
    }


    Vector3 RemoveBaseNoise(Vector3 vec, float minValue)
    {
        Vector3 computeVec = Vector3.zero;
        if (Mathf.Abs(vec.x) > minValue)
            computeVec.x = vec.x;
        if (Mathf.Abs(vec.y) > minValue)
            computeVec.y = vec.y;
        if (Mathf.Abs(vec.z) > minValue)
            computeVec.z = vec.z;

        return computeVec;
    }

    public void Reset()
    {
        dataParent.acc = Vector3.zero;
        dataParent.vel = Vector3.zero;
        dataParent.pos = Vector3.zero;
    }
}
