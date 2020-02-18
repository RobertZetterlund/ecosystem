using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTestRabbit : Animal
{
    public Species specie = Species.Rabbit;
    private Color spherecolor = new Color(0, 1, 0, 0.2f);
    public float senseRadius = 5;
    double Hunger = 1;
    bool isMale = true;

    public MyTestRabbit(GameController controller) : base(controller) 
    {
     
    }

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        // use Köres senses to locate a food source
        // go to food source
        // else
        // walk and loiter about
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        Collider[] colliders = EnvironmentController.CheckSurroundings(transform.position, senseRadius);
        foreach(Collider c in colliders)
        {
            GameObject sensedGameObject = c.gameObject;
            SenseRegistrator.Register(gameObject, sensedGameObject);
            if (gameObject.tag == "Plant")
            {
                Move(sensedGameObject.transform.position);
            } 
        }
            
    }


    void OnDrawGizmos()
    {
        Gizmos.color = spherecolor;
        Gizmos.DrawSphere(transform.position, senseRadius);
    }

}
