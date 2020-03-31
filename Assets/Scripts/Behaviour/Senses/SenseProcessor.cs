
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenseProcessor
{

    private Animal self;
    private GameObject closestFoodObj;
    private GameObject closestFoeObj;
    private GameObject closestWaterObj;
    private GameObject closestMateObj;

    private double closestFoodDist = Int32.MaxValue;
    private double closestFoeDist = Int32.MaxValue;
    private double closestWaterDist = Int32.MaxValue;
    private double closestMateDist = Int32.MaxValue;

    private string[] diet;
    private string[] foes;
    private string[] mates;


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
        if (obj1 == null || obj2 == null)
        {
            return Int32.MaxValue;
        }
        return Vector3.Distance(obj1.transform.position, obj2.transform.position);
    }

    // returns a sensedEvent that can be written to memory
    public SensedEvent Process(ArrayList sensedGameObjects)
    {
        int foodCount = 0;
        int foeCount = 0;
        int mateCount = 0;
        int waterCount = 0;
        closestFoodDist = Int32.MaxValue;
        closestFoeDist = Int32.MaxValue;
        closestMateDist = Int32.MaxValue;
        closestWaterDist = Int32.MaxValue;

        closestFoodObj = null;
        closestFoeObj = null;
        closestWaterObj = null;
        closestMateObj = null;


        foreach (GameObject gameObject in sensedGameObjects)
        {
            string tagOfSensedObject = gameObject.tag;
            double distanceBetween = DistanceBetweenTwoGameObjects(self.gameObject, gameObject);

            // check if in diet
            if (Array.Exists(diet, food => food.Equals(tagOfSensedObject)))
            {
                foodCount++;

                if (closestFoodDist > distanceBetween)
                {
                    closestFoodObj = gameObject;
                    closestFoodDist = distanceBetween;
                }
            }
            // check if water
            else if ((gameObject.tag).Equals("Water"))
            {
                waterCount++;

                if (closestWaterDist > distanceBetween)
                {
                    closestWaterObj = gameObject;
                    closestWaterDist = distanceBetween;
                }

            }
            // check if foe
            else if (Array.Exists(foes, foe => foe.Equals(tagOfSensedObject)))
            {
                foeCount++;
                if (closestFoeDist > distanceBetween)
                {
                    closestFoeObj = gameObject;
                    closestFoeDist = distanceBetween;
                }

            }
            // check if mate
            else if (Array.Exists(mates, mate => mate.Equals(tagOfSensedObject)))
            {
                Animal animal = gameObject.GetComponent<Animal>();
                if (animal.isFertile && (self.isMale ^ animal.isMale)) // closestMateDist > distanceBetween &&
                {
                    mateCount++;
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
        /*
        if(closestMateObj != null)
        {
            Animal animal = closestMateObj.GetComponent<Animal>();
            if (!animal.isFertile)
            {
                closestMateObj = null;
                closestMateDist = Int32.MaxValue;
            }
        }
        */

        // this is the count of sensed objects, it will dictate the strength of which the FCM will input the concept
        // collect all data and combine to a strength of various senses.
        IDictionary<string, int> weightMap = new Dictionary<string, int>();
        weightMap.Add("Foe", foeCount);
        weightMap.Add("Food", foodCount);
        weightMap.Add("Mate", mateCount);

        // return a sensedEvent that can be written to memory.
        return new SensedEvent(weightMap, closestWaterObj, closestFoeObj, closestMateObj, closestFoodObj);
    }

    public GameObject GetClosestFoodObj()
    {
        return closestFoodObj;
    }

}
