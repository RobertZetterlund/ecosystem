
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SenseProcessor
{
	private Animal self;
	private GameObject bestFoodObj;
	private GameObject closestFoeObj;
	private GameObject closestWaterObj;
	private GameObject closestMateObj;

	private double highestFoodValue = double.MinValue;
	private double shortestTimeBeforeEaten = double.MinValue;
	private double closestWaterDist = Int32.MaxValue;
	private double closestMateDist = Int32.MaxValue;

	private string[] diet;
	private string[] foes;
	private string[] mates;


	public SenseProcessor(Animal self, string[] diet, string[] foes, string[] mates)
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

	private void ProcessFoe(GameObject foe)
	{
		double distanceBetween = DistanceBetweenUtility.DistanceBetweenTwoGameObjects(self.gameObject, foe);
		Animal sensedFoe = foe.GetComponent<Animal>();
		double foeSpeed = sensedFoe.GetSpeed();
		double mySpeed = self.GetMaxSpeed();

		double timeBeforeEaten;
		double speedDifference = (foeSpeed - mySpeed);
		// calculate time before eaten
		if (speedDifference == 0) {
			timeBeforeEaten = double.MaxValue;
		}
		else if (speedDifference < 0) // (distance / -speed) nor (distance * -speed) will work
		{
			timeBeforeEaten = -distanceBetween;
		}
		else
		{
			timeBeforeEaten = distanceBetween / speedDifference;
		}

		// determine most dangerous foe
		if (shortestTimeBeforeEaten  >= 0 && timeBeforeEaten >=  0) // if both positive, take smallest time
		{
			if (shortestTimeBeforeEaten >= timeBeforeEaten)
			{
				closestFoeObj = foe;
				shortestTimeBeforeEaten = timeBeforeEaten;
			}
		}
		else if (shortestTimeBeforeEaten < 0 && timeBeforeEaten < 0) // else if both negative take biggest (smallest distance)
		{
			if (shortestTimeBeforeEaten <= timeBeforeEaten)
			{
				closestFoeObj = foe;
				shortestTimeBeforeEaten = timeBeforeEaten;
			}
		}
		else if (timeBeforeEaten >= 0) // else, one is negative (meaning not dangerous, so take the positive one)
		{
			closestFoeObj = foe;
			shortestTimeBeforeEaten = timeBeforeEaten;
		}
	} 


	private void ProcessWater(GameObject water)
	{
		double distanceBetween = DistanceBetweenUtility.DistanceBetweenTwoGameObjects(self.gameObject, water);
		if (closestWaterDist > distanceBetween)
		{
			closestWaterObj = water;
			closestWaterDist = distanceBetween;
		}
	}

	private void ProcessMate(GameObject mate, ArrayList sensedGameObjects)
	{
		Animal sensedMate = mate.GetComponent<Animal>();
		double distanceBetween = DistanceBetweenUtility.DistanceBetweenTwoGameObjects(self.gameObject, mate);

		if (self.isMale ^ sensedMate.isMale) // closestMateDist > distanceBetween &&
		{
			if (sensedGameObjects.Contains(closestMateObj))
			{
				Animal memoryMate = closestMateObj.GetComponent<Animal>();
				// if same fertility, take closest one
				if (!(memoryMate.isFertile ^ sensedMate.isFertile))
				{
					if (closestMateDist > distanceBetween)
					{
						closestMateDist = distanceBetween;
						closestMateObj = mate;
					}
				}
				else if (sensedMate.isFertile) // if only new mate fertile, take it
				{
					closestMateDist = distanceBetween;
					closestMateObj = mate;
				}
				// else, keep mate in memory
			}
			closestMateObj = mate;
			closestMateDist = distanceBetween;
		}
	}

	private void ProcessFood(GameObject foodObj)
	{
		double distanceBetween = DistanceBetweenUtility.DistanceBetweenTwoGameObjects(self.gameObject, foodObj);
		IConsumable food = foodObj.GetComponent<IConsumable>();
		double foodCurrentSpeed = food.GetSpeed();
		double myMaxSpeed = self.GetMaxSpeed();
		double amount = food.GetAmount();
		if (amount == 0)
			return;
		double chaseTime;
		double speedDifference = myMaxSpeed - foodCurrentSpeed;
		double foodTimeRatio;

		// calculate chase time
		if (speedDifference == 0) 
		{
			chaseTime = double.MaxValue;
		}
		else if (speedDifference < 0) // (distance / -speed) nor (distance * -speed) will work
		{
			chaseTime = -distanceBetween;
		}
		else
		{
			chaseTime = distanceBetween / speedDifference;
		}

		// calculate food time ratio
		if (chaseTime > 0) {
			foodTimeRatio = amount / chaseTime;
		}
		else if (chaseTime < 0)
		{
			foodTimeRatio = chaseTime / amount;
		}
		else
		{
			foodTimeRatio = double.MaxValue;
		}

		// determine best food
		if (highestFoodValue <= foodTimeRatio)
		{
			bestFoodObj = foodObj;
			highestFoodValue = foodTimeRatio;
		} 
	}

	// returns a sensedEvent that can be written to memory
	public SensedEvent Process(ArrayList sensedGameObjects)
	{
		int foodCount = 0;
		int foeCount = 0;
		int mateCount = 0;
		int waterCount = 0;
		highestFoodValue = double.MinValue;
		shortestTimeBeforeEaten = double.MinValue;
		closestMateDist = Int32.MaxValue;
		closestWaterDist = Int32.MaxValue;

		bestFoodObj = null;
		closestFoeObj = null;
		closestWaterObj = null;
		closestMateObj = null;


		foreach (GameObject gameObject in sensedGameObjects)
		{
			string tagOfSensedObject = gameObject.tag;

			// check if in diet
			if (Array.Exists(diet, food => food.Equals(tagOfSensedObject)))
			{
				foodCount++;

				ProcessFood(gameObject);
			}
			// check if water
			else if (tagOfSensedObject == "Water")
			{
				waterCount++;

				ProcessWater(gameObject);

			}
			// check if foe
			else if (Array.Exists(foes, foe => foe.Equals(tagOfSensedObject)))
			{
				foeCount++;
				ProcessFoe(gameObject);

			}
			// check if mate
			else if (Array.Exists(mates, mate => mate.Equals(tagOfSensedObject)))
			{
				mateCount++;
				ProcessMate(gameObject, sensedGameObjects);
			}
			// unknown
			else
			{
				/// ?
			}
		}
		// end of foreach loop

		// this is the count of sensed objects, it will dictate the strength of which the FCM will input the concept
		// collect all data and combine to a strength of various senses.
		Dictionary<string, int> weightMap = new Dictionary<string, int>
		{
			{ "Foe", foeCount },
			{ "Food", foodCount },
			{ "Mate", mateCount },
			{ "Water", waterCount }
		};

		// return a sensedEvent that can be written to memory.
		return new SensedEvent(weightMap, closestWaterObj, closestFoeObj, closestMateObj, bestFoodObj);
	}



	public GameObject GetClosestFoodObj()
	{
		return bestFoodObj;
	}
}
