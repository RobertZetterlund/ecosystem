using UnityEngine;
using System.Collections;

public class MockSensor : MonoBehaviour
{

    AbstractSensor sensor;
    float hFOV = 360;
    float vFOV = 360;
    float radius = 20;

    // Use this for initialization
    void Start()
    {
        sensor = new AreaSensor(radius, hFOV, vFOV, true, SensorType.SIGHT);
    }

    // Update is called once per frame
    void Update()
    {
        sensor.Sense(transform);
    }

    void OnDrawGizmos()
    {

        var pos1 = transform.position + Quaternion.AngleAxis(hFOV / 2, transform.up) * transform.forward * radius;
        //var pos2 = transform.position + Quaternion.AngleAxis(vFOV / 2, transform.right) * transform.forward * radius;
        var pos2 = transform.position + Quaternion.AngleAxis(-hFOV / 2, transform.up) * transform.forward * radius;
        //var pos4 = transform.position + Quaternion.AngleAxis(-vFOV / 2, transform.right) * transform.forward * radius;

        Gizmos.DrawLine(transform.position, pos1);
        Gizmos.DrawLine(transform.position, pos2);

        var prev = pos2;
        int start = (int)(-hFOV / 2);
        int end = (int)(hFOV / 2);
        for (int i = start; i <= end; i += 10)
        {
            var newpos = transform.position + Quaternion.AngleAxis(i, transform.up) * transform.forward * radius;
            Gizmos.DrawLine(prev, newpos);
            prev = newpos;
        }
        Gizmos.color = new Color(0, 0, 1, 0.2f);
        Gizmos.DrawSphere(transform.position, radius);

    }
}
