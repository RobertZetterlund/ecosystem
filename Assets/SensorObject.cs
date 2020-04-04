using System.Collections;
using UnityEngine;

// An object with only a sensor I used to debug sences and raycast because the rabbits were to unpredictable 

public class SensorObject : MonoBehaviour
{
    public bool allRaycastHits;
    private bool drawRaycast = true;
    private bool showSightGizmo = true;
    private ArrayList sensedGameObjects;
    private AbstractSensor[] sensors;
    private float sightLength = 25;
    private float horisontalFOV = 120;
    private float verticalFOV = 90;
    private SenseProcessor senseProcessor;
    private Timer senseTimer;

    private void Start() {
        sensors = new AbstractSensor[1];
        sensors[0] = SensorFactory.SightSensor(sightLength, horisontalFOV, verticalFOV);
        senseTimer = new Timer(0.25f);
        senseTimer.Start();

    }

    private void Update() {
        if(senseTimer.IsDone())
        {
            Sense();
            senseTimer.Reset();
            senseTimer.Start();
        }
    }

    void Sense() {
        sensedGameObjects = new ArrayList();
        GameObject[] objectArray = sensors[0].Sense(transform);
        Debug.Log("Sensed" + objectArray);

    }

    void OnDrawGizmos()
    {

        if(drawRaycast)
        {
            foreach(Vector3 vec in ((AreaSensor)sensors[0]).pointList)
            {
                Gizmos.color = UnityEngine.Color.gray;
                Gizmos.DrawSphere(vec, 0.05f);
                Gizmos.color = UnityEngine.Color.white;
                Gizmos.DrawLine(gameObject.transform.position, vec);
            }

            foreach(Vector3 vec in ((AreaSensor)sensors[0]).wrongHitList)
            {
                Gizmos.color = UnityEngine.Color.red;
                Gizmos.DrawLine(gameObject.transform.position, vec);
            }
            foreach(Vector3 vec in ((AreaSensor)sensors[0]).rightHitList)
            {
                Gizmos.color = UnityEngine.Color.blue;
                Gizmos.DrawLine(gameObject.transform.position, vec);

            }

            if(allRaycastHits)
            {
                foreach(Vector3 vec in ((AreaSensor)sensors[0]).hitList)
                {
                    Gizmos.color = UnityEngine.Color.green;
                    Gizmos.DrawLine(gameObject.transform.position, vec);
                }
            }

            Gizmos.color = UnityEngine.Color.white;
        }


        if(showSightGizmo) {
            float hFOV = horisontalFOV;
            //float vFOV = verticalFOV;
            var pos1 = transform.position + Quaternion.AngleAxis(hFOV / 2, transform.up) * transform.forward * sightLength;
            //var pos2 = transform.position + Quaternion.AngleAxis(vFOV / 2, transform.right) * transform.forward * radius;
            var pos2 = transform.position + Quaternion.AngleAxis(-hFOV / 2, transform.up) * transform.forward * sightLength;
            //var pos4 = transform.position + Quaternion.AngleAxis(-vFOV / 2, transform.right) * transform.forward * radius;

            Gizmos.DrawLine(transform.position, pos1);
            Gizmos.DrawLine(transform.position, pos2);

            var prev = pos2;
            int start = (int)(-hFOV / 2);
            int end = (int)(hFOV / 2);
            for(int i = start; i <= end; i += 10) {
                var newpos = transform.position + Quaternion.AngleAxis(i, transform.up) * transform.forward * sightLength;
                Gizmos.DrawLine(prev, newpos);
                prev = newpos;
            }
        }
        
    }
    
}
