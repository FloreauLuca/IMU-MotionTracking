using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class NaiveMovingTest : MonoBehaviour
{
    private NewCalculationFarm calculationFarm;

    [SerializeField] private float increaseAmplitude = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        calculationFarm = FindObjectOfType<NewCalculationFarm>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = calculationFarm.currComputeFrame.computePos * increaseAmplitude;
    }
    
}
