
using System;
using System.Collections;
using UnityEngine;

public class SenseProcessor
{
    
    Animal self;
    GameObject closestFoodObj;
    GameObject closestFoeObj;
    GameObject closestWaterObj;
    GameObject closestMateObj;

    double closestFoodDist = Int32.MaxValue;
    double closestFoeDist = Int32.MaxValue;
    double closestWaterDist = Int32.MaxValue;
    double closestMateDist = Int32.MaxValue;

    string[] diet;
    string[] foes;
    string[] mates;

    public SenseProcessor(string[] diet, string[] foes, string[] mates, Animal self)
    {
        this.self = self;
        this.diet = diet;
        this.foes = foes;
        this.mates = mates;
    }

    public SenseProcessor(Animal self)
    {
        this.self = self;
        this.diet = new string[] { "Plant" };
        this.foes = new string[] { "Fox" };
        this.mates = new string[] { "Rabbit" };
    }


    private double DistanceBetweenTwoGameObjects(GameObject obj1, GameObject obj2)
    {
        if(obj1 == null || obj2 == null)
        {
            return Int32.MaxValue;
        }
        return Vector3.Distance(obj1.transform.position, obj2.transform.position);
    }

    // returns a sensedEvent that can be written to memory
    public SensedEvent Process(ArrayList sensedGameObjects)
    {
        foreach (GameObject gameObject in sensedGameObjects)
        {
            string tagOfSensedObject = gameObject.tag;
            double distanceBetween = DistanceBetweenTwoGameObjects(self.gameObject, gameObject);

            // check if in diet
            if (Array.Exists(diet, food => food.Equals(tagOfSensedObject)))
            {
                Debug.Log("in diet!");
      
                if (closestFoodDist > distanceBetween)
                {
                    closestFoodObj = gameObject;
                    closestFoodDist = distanceBetween;
                }
            }
            // check if water
            else if ((gameObject.tag).Equals("Water"))
            {
                // TODO: implement
                // hold off until we have water sorted
            }
            // check if foe
            else if(Array.Exists(foes, foe => foe.Equals(tagOfSensedObject)))
            {
                Debug.Log("SENSING A FOE");

                if (closestFoeDist > distanceBetween)
                {
                    closestFoeObj = gameObject;
                    closestFoeDist = distanceBetween;
                }

            }
            // check if mate
            else if(Array.Exists(mates, mate => mate.Equals(tagOfSensedObject)))
            {
                if (closestMateDist > distanceBetween)
                {
                    closestMateObj = gameObject;
                    closestMateDist = distanceBetween;
                }
            }
            // unknown
            else
            {
                /// ?
            }
        }
        // end of foreach loop


        


        // collect all data and combine to a strength of various senses.

        return new SensedEvent(null, closestFoeObj, closestMateObj, closestFoodObj);
    }


    public GameObject GetClosestFoodObj()
    {
        return closestFoodObj;
    }
    
}
