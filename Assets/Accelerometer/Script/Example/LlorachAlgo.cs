using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMALowPassFilter
{
    private float previous = 0;

    public float filter(float input,float coefficient)
    {
        previous = previous * (1 - coefficient) + input * coefficient;
        return previous;
    }
}

public class EMAHighPassFilter
{
    private float previous_out = 0;
    private float previous_in = 0;

    public float filter(float input, float cutoff, float dt)
    {
        float RC = 1.0f / (2.0f * 3.14f * cutoff);
        float coeff = RC / (RC + dt);
        previous_out = previous_out * coeff + (input - previous_in) * coeff;
        return previous_out;
    }
}


public class Integration
{
    static public float trapeze(float a, float b, float ta, float tb)
    {
        return ((tb - ta) / 2) * (a + b);
    }

    static public float standard(float prev, float a, float ta, float tb)
    {
        return (ta - tb) * a + prev;
    }
}

public class LlorachAlgo : MonoBehaviour
{
    [SerializeField] private float threshold = 0.5f;
    [SerializeField] private float p = 0.5f;
    [SerializeField] private float range = 0.5f;

    private EMALowPassFilter accLowPassFilter = new EMALowPassFilter();
    private EMAHighPassFilter accHighPassFilter = new EMAHighPassFilter();
    private EMAHighPassFilter velHighPassFilter = new EMAHighPassFilter();

    private Vector3 velocity;
    private Vector3 position;
    private float prev_time;

    private CalculationFarm calculationFarm;

    void Start()
    {
        calculationFarm = FindObjectOfType<CalculationFarm>();
    }

    void Update()
    {
        Vector3 acc = Input.gyro.userAcceleration;
        acc.x = accLowPassFilter.filter(acc.x, 0.1f);
        acc.x = accHighPassFilter.filter(acc.x, 2, Time.deltaTime);
        velocity.x = Integration.standard(velocity.x, acc.x, prev_time, Time.time);
        float cutoff = 2;
        if (Mathf.Abs(acc.x) > 1 / p)
        {
            cutoff = 1.0f / (Mathf.Abs(acc.x) * p);
        }

        velocity.x = velHighPassFilter.filter(velocity.x, cutoff, Time.deltaTime);
        if (Mathf.Abs(acc.x) < threshold)
        {
            velocity.x *= Mathf.Pow(range, threshold - Mathf.Abs(acc.x));
        }

        position.x = Integration.standard(position.x, velocity.x, prev_time, Time.deltaTime);
        prev_time = Time.time;
        calculationFarm.currProcessAccFrame.computeInitAcceleration = acc;
        calculationFarm.currProcessAccFrame.computeInitVelocity = velocity;
        calculationFarm.currProcessAccFrame.computeInitPosition = position;

    }
}