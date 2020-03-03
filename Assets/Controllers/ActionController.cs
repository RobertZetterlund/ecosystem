using System;
using System.Collections;
using UnityEngine;

public class ActionController
{
    Animal animal;
    public Vector3 draw1, draw2 = new Vector3(0, 0, 0);

    public ActionController(Animal animal)
    {
        this.animal = animal;
    }

    public IEnumerator GoToFood()
    {

        // do something here
        while(true)
        {
            yield return new WaitForSeconds(2);
            yield return Walk();
            yield return Draw();
        }
        
        // do whatever else here

    }

    IEnumerator Walk()
    {
        Vector3 pos = ChooseNewDestination();
        animal.SetDestination(pos);
        yield return new WaitForSeconds(0);
    }

    IEnumerator Draw()
    {
        Vector3 dir = animal.transform.forward;
        float angle = Vector3.SignedAngle(dir, Vector3.forward, Vector3.up);
        float angle1 = angle - 20;
        float angle2 = angle + 20;
        Vector3 direction1 = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * angle1), 0, Mathf.Cos(Mathf.Deg2Rad * angle1));
        draw1 = direction1;
        Vector3 direction2 = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * angle2), 0, Mathf.Cos(Mathf.Deg2Rad * angle2));
        draw2 = direction2;
        yield return new WaitForSeconds(0);
    }

    private Vector3 ChooseNewDestination()
    {
        Vector3 dir = animal.transform.forward;
        float angle = Vector3.SignedAngle(dir, Vector3.forward, Vector3.up);
        float angle1 = angle - 20;
        float angle2 = angle + 20;
        Debug.Log("Angle1: " + angle1 + "    Angle2: " + angle2);
        float new_angle = UnityEngine.Random.Range(angle1, angle2);
        Vector3 new_directon = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * new_angle), 0, Mathf.Cos(Mathf.Deg2Rad * new_angle));
        Debug.Log("new_angle: " + new_angle);

        Vector3 new_pos = animal.transform.position + new_directon * 10;
        return new_pos;
    }

}
