using System;
using System.Collections;
using UnityEngine;

/**
 * Controlls actions of the animals. Currently a work in progress and a lot is hard coded just to try things out.
 * It's not abstracted at all since I don't really know in what way we should do this. Also, there is a double dependency between
 * animal <--> ActionController which I'm not super keen on.  
 * 
 * Just works for rabbit atm.
 * 
 * Actions are constructed by these IEnumerator things, and you chan them together to make a series of steps.
 * You start an action by calling StartCoroutine(..some action) on the animal.
 */
public class ActionController : IActionController, IObserver<GameObject>
{
    Animal animal;
    public string targetGametag = "";

    public ActionController(Animal animal)
    {
        this.animal = animal;
    }

    public IEnumerator GoToFood()
    {
        yield return Search("Plant");
    }

    public IEnumerator GoToWater()
    {
        yield return Search("Water");
    }

    public IEnumerator GoToPartner()
    {
        throw new NotImplementedException();
    }

    public IEnumerator ChaseAnimal(Animal animal)
    {
        throw new NotImplementedException();
    }

    public IEnumerator EscapeAnimal(Animal animal)
    {
        throw new NotImplementedException();
    }

    IEnumerator Walk()
    {
        Vector3 pos = ChooseNewDestination();
        animal.SetDestination(pos);
        yield return new WaitForSeconds(0);
    }

    public IEnumerator Search(string gametag)
    {
        targetGametag = gametag;
        while (true)
        {
            yield return new WaitForSeconds(1);
            yield return Walk();
        }
    }


    /**
     * Makes the animal walk to a position 10 steps in front of the animal in a direction that is in the bounderies of an angle
     * of -40 to +40 of the direction that the animal is facing.
     */
    private Vector3 ChooseNewDestination()
    {
        Vector3 dir = animal.transform.forward;
        float angle = Vector3.SignedAngle(dir, Vector3.forward, Vector3.up);
        float angle1 = angle - 40;
        float angle2 = angle + 40;
        float new_angle = UnityEngine.Random.Range(angle1, angle2);
        Vector3 new_directon = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * new_angle), 0, Mathf.Cos(Mathf.Deg2Rad * new_angle));

        Vector3 new_pos = animal.transform.position + new_directon * 10;
        return new_pos;
    }

    /**
     * Now this is obviously trash. Just stops the animal and makes it go to the
     * object found from the senseregistrator if it happens to be what it is searching for.
     * 
     */
    public void OnNext(GameObject value)
    {
        if (!targetGametag.Equals("") && value.CompareTag(targetGametag))
        {
            animal.StopAllCoroutines();
            animal.SetDestination(value.transform.position);
        }
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

}
