using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MyTestRabbit : Animal
{
    public Species specie = Species.Rabbit;
    double Hunger = 1;
    bool isMale = true;

    public MyTestRabbit(GameController controller) : base(controller) 
    {
     
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        senseRadius = 5;
        fcm = FCMFactory.RabbitFCM();
        senseRegistrator = new RabbitSenseRegistrator(fcm);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    //Draws a sphere corresponding to its sense radius
    void OnDrawGizmos()
    {
        //I tried to make it so that it uses the same color object all the time, but it glitches big time
        Gizmos.color = new Color(1,1,0,0.5f);
        Gizmos.DrawSphere(transform.position, senseRadius);
        Vector3 textOffset = new Vector3(0, 2, 0);
        Handles.Label(transform.position + textOffset, currentAction.ToString());
    }
    

}
