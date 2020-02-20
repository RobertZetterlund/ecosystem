using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTestRabbit : Animal
{
    public Species specie = Species.Rabbit;
    public float senseRadius = 5f;
    double Hunger = 1;
    bool isMale = true;

    public MyTestRabbit(GameController controller) : base(controller) 
    {
     
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        // use Köres senses to locate a food source
        // go to food source
        // else
        // walk and loiter about
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        Collider[] colliders = EnvironmentController.CheckSurroundings(transform.position, senseRadius);
        foreach(Collider c in colliders)
        {
            GameObject sensedGameObject = c.gameObject;
            SenseRegistrator.Register(gameObject, sensedGameObject);
            if (sensedGameObject.tag == "Plant")
            {
                Debug.Log("We are setting the direction");
                SetTargetDestination(sensedGameObject.transform.position);
            } 
        }
            
    }

    //Draws a sphere corresponding to its sense radius
    void OnDrawGizmos()
    {
        //I tried to make it so that it uses the same color object all the time, but it glitches big time
        Gizmos.color = new Color(1,1,0,0.5f);
        Gizmos.DrawSphere(transform.position, senseRadius);
    }
    

}
