using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorVisualizer : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    [SerializeField] private float minDelta;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(Input.gyro.userAcceleration.x) > minDelta || Mathf.Abs(Input.gyro.userAcceleration.y) > minDelta ||
            Mathf.Abs(Input.gyro.userAcceleration.z) > minDelta)
        {
            meshRenderer.material.color = Color.red;
        }
        else
        {
            meshRenderer.material.color = Color.white;
        }
    }
}
