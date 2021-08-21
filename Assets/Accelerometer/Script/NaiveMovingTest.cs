using System.Collections;
using System.Collections.Generic;
using old;
using TMPro;
using UnityEditor;
using UnityEngine;



public class NaiveMovingTest : MonoBehaviour
{
    private CalculationFarm calculationFarm;

    [SerializeField] private float increaseAmplitude = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        calculationFarm = FindObjectOfType<CalculationFarm>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = calculationFarm.currRCFrame.rcPos * increaseAmplitude;
    }
    
}
