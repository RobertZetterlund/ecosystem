using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTestRabbit : MonoBehaviour
{
    public Species specie = Species.Rabbit;
    private Color spherecolor = new Color(0, 1, 0, 0.2f);
    public float senseRadius = 5;
    double Hunger = 1;
    bool isMale = true;
    double speed = 5;

    // Start is called before the first frame update
    void Start()
    {
        // use Köres senses to locate a food source
        // go to food source
        // else
        // walk and loiter about
    }

    // Update is called once per frame
    void Update()
    {
        // look for enemy
        //transform.Translate(0.000000001f * Time.deltaTime, 0f, 0f);
        Collider[] colliders = EnvironmentController.CheckSurroundings(transform.position, senseRadius);
        //Debug.Log(colliders);
        foreach(Collider c in colliders)
        {
            //Debug.Log(c.name);

            string objName = c.name;


           /* Animal animal;
            try
            {
                animal = GetComponent<Animal>();
            } catch (Exception e)
            {

            }*/
            
            if(objName.Equals("MyTestPlant"))
            {
                //Debug.Log("I FOUND A TESTPLANT");
                Debug.Log(c.gameObject);
                Debug.Log(typeof(ISensable));
                var interfaceType = typeof(ISensable);

                if (interfaceType.IsInstanceOfType(c.gameObject.GetType()))
                {
                    Debug.Log("THE TESTPLANT IS A SENSABLE");
                }
                transform.position = c.gameObject.transform.position;
            }

            

            /*if(Vector3.Distance(transform.position, c.gameObject.transform.position) < 1 ) {
                if (!c.gameObject.Equals(this))
                {
                Destroy(c.gameObject);

                }
            }*/


        }
            
    }

    void OnDrawGizmos()
    {
        Gizmos.color = spherecolor;
        Gizmos.DrawSphere(transform.position, senseRadius);
    }

}
