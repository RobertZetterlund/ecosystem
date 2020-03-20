/*
using System;
using System.Collections;
using UnityEngine;

public class SenseProcessor
{
    
    Animal self;
    GameObject closestFoodObj;
    GameObject closestFoe;
    GameObject closestWater;

    double closestFoodDist;
    double closestFoeDist;
    double closestWaterDist;

    string[] diet;

    public SenseProcessor(string[] diet, Animal self)
    {
        this.self = self;
        this.diet = diet;
    }


    private double DistanceBetweenTwoGameObjects(GameObject obj1, GameObject obj2)
    {
        return Vector3.Distance(obj1.transform.position, obj2.transform.position);
    }


    public void Process(ArrayList sensedGameObjects)
    {
        Debug.Log("iM PROCESSING!");

        foreach (GameObject gameObject in sensedGameObjects)
        {

            Debug.Log("state of IF : " + Array.Exists(diet, foodsource => foodsource.Equals(gameObject.tag)));


            if (Array.Exists(diet, foodsource => foodsource.Equals(gameObject.tag)))
            {
                Debug.Log("in diet!");
                //gameObject is food
                if (closestFoodObj is null)
                {
                    closestFoodObj = gameObject;
                    closestFoodDist = DistanceBetweenTwoGameObjects(self.gameObject, gameObject);
                    continue;
                }

                double distanceBetween = DistanceBetweenTwoGameObjects(self.gameObject, gameObject);

                if (closestFoodDist > distanceBetween)
                {
                    closestFoodObj = gameObject;
                    closestFoodDist = distanceBetween;
                }
            }

        }
    }


    public GameObject GetClosestFoodObj()
    {
        //GameObject tempCopy = GameObject.Instantiate(closestFoodObj);
        //closestFoodObj = null;

        return closestFoodObj;
    }
    
}
*/