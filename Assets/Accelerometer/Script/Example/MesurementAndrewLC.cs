using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MesurementAndrewLC : MonoBehaviour
{
    [SerializeField] private bool collect = true;
    [SerializeField] private GameObject gravPoint;
    [SerializeField] private GameObject gravOrigin;
    [SerializeField] private int n;

    [SerializeField] private Vector3 I = Vector3.zero;
    [SerializeField] private Vector3 K = Vector3.one;
    [SerializeField] private Vector3 accel;
    [SerializeField] private Vector3 accelMean;
    [SerializeField] private Vector3 c;
    [SerializeField] private Vector3 dv;
    [SerializeField] private Vector3 tdv;
    [SerializeField] private float gforce = 9.14f;
    [SerializeField] private Vector3 gdv;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector3[] vWindow = new Vector3[64];
    [SerializeField] private Vector3 deltad;
    [SerializeField] private Vector3 displacement;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (collect)
        {
            // Find average acceleration
            accel = Vector3.zero;
            accelMean = Vector3.zero;
            int count = 0;
            float dt = 0;
            foreach (AccelerationEvent accEvent in Input.accelerationEvents)
            {
                accel = accEvent.acceleration;
                accelMean += accel; // * gforce;
                count++;
                dt += accEvent.deltaTime;
            }

            accelMean = accelMean / count;
            // c = K(u - I)
            c = Vector3.Scale((accelMean - I), K);
            // Find local dv vector
            dv = RemoveGrav(c) * dt;
            // Apply threshold
            tdv = VelocityThreshold(dv) * gforce;

            // Change local dv vector to global coordinates
            gdv = LocalToGlobal(tdv);
            // add global gdv to velocity window
            velocity -= vWindow[n];
            vWindow[n] = gdv;
            velocity += vWindow[n];
            n++;
            if (n >= vWindow.Length)
                n = 0;
            // integrate global velocity to get global change in position
            deltad = velocity * dt;
            // add change in position to global position
            displacement += deltad;
        }

        // update orientation
        transform.rotation = GetOrientation();
        // Display current orientation and displacement
        //SetDisplacementText(displacement);
        //SetOrientationText(transform.rotation.eulerAngles);
        if (collect)
            LogData();
        // transform.rotation = ConvertRotation(gyro.attitude);
    }

    public void SetDisplacementText(Vector3 displacement)
    {
        Debug.Log("[Andrew] velocity : " + velocity.ToString("#.000"));
        Debug.Log("[Andrew] displacement : " + displacement.ToString("#.000"));
    }

    public void SetOrientationText(Vector3 eulerAngles)
    {
        Debug.Log("[Andrew] eulerAngles : " + eulerAngles.ToString("#.000"));
    }

    public void LogData()
    {

    }

    public Vector3 LocalToGlobal(Vector3 local)
    {
        return local;
        return transform.worldToLocalMatrix * local;
    }

    public Quaternion GetOrientation()
    {
        return Input.gyro.attitude;
    }

    public Vector3 RemoveGrav(Vector3 accelInput)
    {
        Vector3 accel;
        Quaternion pose = GetOrientation();
        // set gravPoint.transform.localPosition to the vector
        // representing the accelerometer input
        gravPoint.transform.localPosition = new Vector3(-accelInput.x, accelInput.z,
            -accelInput.y);
        // rotate the vector around gravOrigin by the pose
        gravOrigin.transform.rotation = pose;
        // gravPoint world position is now equal to the
        // acceleration in world axes
        // subtract gravity from the world y axis
        // note: accelerometer data is measured in G forces,
        // so gravity will exert a force of -1 G forces on the
        // world y axis
        accel = new Vector3(gravPoint.transform.position.x,
            gravPoint.transform.position.y + 1,
            gravPoint.transform.position.z);
        gravPoint.transform.position = accel;
        // gravPoint.transform.localPosition is now the accelerometer
        // data without gravity
        accel = gravPoint.transform.localPosition;
        return accel;
    }

    public Vector3 VelocityThreshold(Vector3 v)
    {
        Vector3 vt;
        if (v.x > -0.001 && v.x < 0.001)
            vt.x = 0;
        else
            vt.x = v.x;
        if (v.y > -0.001 && v.y < 0.001)
            vt.y = 0;
        else
            vt.y = v.y;
        if (v.z > -0.001 && v.z < 0.001)
            vt.z = 0;
        else
            vt.z = v.z;
        return vt;
    }
}
