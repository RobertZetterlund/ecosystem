﻿
using System;
using System.Collections;
using System.IO;



public static class FCMFactory
{

	public static FCM parsedFCM = null;

	public static FCM GetBaseFCM()
	{
		EntityInput[] inputs = (EntityInput[])Enum.GetValues(typeof(EntityInput));
		EntityMiddle[] middles = (EntityMiddle[])Enum.GetValues(typeof(EntityMiddle));
		EntityAction[] actions = (EntityAction[])Enum.GetValues(typeof(EntityAction));

		FCM fcm = new FCM(inputs, middles, actions);

		return fcm;
	}
	public static FCM RabbitFCM()
	{
		EntityInput[] inputs = (EntityInput[])Enum.GetValues(typeof(EntityInput));
		EntityMiddle[] middles = (EntityMiddle[])Enum.GetValues(typeof(EntityMiddle));
		EntityAction[] actions = (EntityAction[])Enum.GetValues(typeof(EntityAction));



		FCM fcm = new FCM(inputs, middles, actions);

		if (!SimulationController.Instance().settings.rabbit.randomiseFCM)
		{
			// FOOD AND HUNGER
			fcm.SetWeight(EntityField.FoodClose, EntityField.GoingToFood, 0.05);
			fcm.SetWeight(EntityField.FoodFar, EntityField.GoingToFood, -0.05);
			fcm.SetWeight(EntityField.FoodPresenceHigh, EntityField.GoingToFood, 0.02);
			fcm.SetWeight(EntityField.FoodPresenceLow, EntityField.GoingToFood, -0.02);

			fcm.SetWeight(EntityField.Hungry, EntityField.GoingToFood, 0.3);
			fcm.SetWeight(EntityField.NotHungry, EntityField.GoingToFood, -0.2);


			fcm.SetWeight(EntityField.Hungry, EntityField.SearchingForMate, -0.1);
			fcm.SetWeight(EntityField.NotHungry, EntityField.SearchingForMate, 0.1);

			fcm.SetWeight(EntityField.Hungry, EntityField.Escaping, -0.2);


			// WATER AND THIRST
			fcm.SetWeight(EntityField.WaterClose, EntityField.GoingToWater, 0.05);
			fcm.SetWeight(EntityField.WaterFar, EntityField.GoingToWater, -0.05);
			fcm.SetWeight(EntityField.WaterPresenceHigh, EntityField.GoingToWater, 0.02);
			fcm.SetWeight(EntityField.WaterPresenceLow, EntityField.GoingToWater, -0.02);

			fcm.SetWeight(EntityField.Thirsty, EntityField.GoingToWater, 0.3);
			fcm.SetWeight(EntityField.NotThirsty, EntityField.GoingToWater, -0.2);


			fcm.SetWeight(EntityField.Thirsty, EntityField.Escaping, -0.2);




			// MATE AND FERTILITY
			fcm.SetWeight(EntityField.MateClose, EntityField.SearchingForMate, 0.08);
			fcm.SetWeight(EntityField.MateFar, EntityField.SearchingForMate, -0.05);
			fcm.SetWeight(EntityField.MatePresenceHigh, EntityField.SearchingForMate, 0.02);
			fcm.SetWeight(EntityField.MatePresenceLow, EntityField.SearchingForMate, -0.02);

			fcm.SetWeight(EntityField.Fertile, EntityField.SearchingForMate, 0.2);
			fcm.SetWeight(EntityField.NotFertile, EntityField.SearchingForMate, -1);

			fcm.SetWeight(EntityField.Hungry, EntityField.SearchingForMate, -0.1);
			fcm.SetWeight(EntityField.NotHungry, EntityField.SearchingForMate, 0.1);
			//This should start of has being far away, unless anything else has been sensed
			fcm.SetState(EntityField.FoodFar, 1);
			fcm.SetState(EntityField.WaterFar, 1);
			fcm.SetState(EntityField.MateFar, 1);


			// FEAR AND ESCAPING
			
			fcm.SetWeight(EntityField.Escaping, EntityField.GoingToWater, -0.15);
			fcm.SetWeight(EntityField.Escaping, EntityField.SearchingForMate, -0.15);
			fcm.SetWeight(EntityField.Escaping, EntityField.GoingToFood, -0.15);


		}
		else
		{
			fcm.Randomise();
		}

		return fcm;
	}

	// same as rabbit for now
	public static FCM FoxFCM()
	{
		EntityInput[] inputs = (EntityInput[])Enum.GetValues(typeof(EntityInput));
		EntityMiddle[] middles = (EntityMiddle[])Enum.GetValues(typeof(EntityMiddle));
		EntityAction[] actions = (EntityAction[])Enum.GetValues(typeof(EntityAction));

		FCM fcm = new FCM(inputs, middles, actions);

		if (!SimulationController.Instance().settings.fox.randomiseFCM)
		{
			/*fcm.SetWeight(EntityField.FoodClose, EntityField.GoingToFood, 0.05);
			fcm.SetWeight(EntityField.FoodFar, EntityField.GoingToFood, -0.05);
			fcm.SetWeight(EntityField.FoodPresenceHigh, EntityField.GoingToFood, 0.02);
			fcm.SetWeight(EntityField.FoodPresenceLow, EntityField.GoingToFood, -0.02);

			fcm.SetWeight(EntityField.WaterClose, EntityField.GoingToWater, 0.05);
			fcm.SetWeight(EntityField.WaterFar, EntityField.GoingToWater, -0.05);
			fcm.SetWeight(EntityField.WaterPresenceHigh, EntityField.GoingToWater, 0.02);
			fcm.SetWeight(EntityField.WaterPresenceLow, EntityField.GoingToWater, -0.02);

			fcm.SetWeight(EntityField.MateClose, EntityField.SearchingForMate, 0.08);
			fcm.SetWeight(EntityField.MateFar, EntityField.SearchingForMate, -0.05);
			fcm.SetWeight(EntityField.MatePresenceHigh, EntityField.SearchingForMate, 0.02);
			fcm.SetWeight(EntityField.MatePresenceLow, EntityField.SearchingForMate, -0.02);

			fcm.SetWeight(EntityField.Fertile, EntityField.SearchingForMate, 0.2);
			fcm.SetWeight(EntityField.NotFertile, EntityField.SearchingForMate, -1);
			fcm.SetWeight(EntityField.Hungry, EntityField.GoingToFood, 0.3);
			fcm.SetWeight(EntityField.NotHungry, EntityField.GoingToFood, -0.2);
			fcm.SetWeight(EntityField.Thirsty, EntityField.GoingToWater, 0.3);
			fcm.SetWeight(EntityField.NotThirsty, EntityField.GoingToWater, -0.2);

			fcm.SetWeight(EntityField.Hungry, EntityField.SearchingForMate, -0.1);
			fcm.SetWeight(EntityField.NotHungry, EntityField.SearchingForMate, 0.1);

			fcm.SetState(EntityField.pFear, 0);

			//This should start of has being far away, unless anything else has been sensed*/
			fcm.SetState(EntityField.FoodFar, 1);
            fcm.SetState(EntityField.WaterFar, 1);
            fcm.SetState(EntityField.MateFar, 1);

			fcm.SetState(EntityField.GoingToFood, 1);

		}
		else
		{
			fcm.Randomise();
		}

		return fcm;
	}


    // given a filepath, creates a single fcm of that log
    public static FCM ParseFCMFilePath(string filepath)
    {


        if(!(parsedFCM is null))
        {
			return parsedFCM;
        }

		ArrayList fcmWeightLines = new ArrayList();

		using (var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
        {
			using (StreamReader sr = new StreamReader(fileStream))
			{
                // skip the first categorization line.
				sr.ReadLine();
				while (!sr.EndOfStream)
				{
					fcmWeightLines.Add(sr.ReadLine());
				}
			}
		}


        string[] arr = (string[]) fcmWeightLines.ToArray(typeof(string));

			return ParseFCMStringArray(arr);
    }


    public static FCM ParseFCMStringArray(string[] fcmWeightArray)
    {
		FCM fcm = GetBaseFCM();

        foreach(string line in fcmWeightArray)
        {

			string[] lineValues = line.Split(',');
			Enum.TryParse(lineValues[0], out EntityField _to);
			Enum.TryParse(lineValues[1], out EntityField _from);
			double weight = Convert.ToDouble(lineValues[2]);
			fcm.SetWeight(_to, _from, weight);
        }

		parsedFCM = fcm;

		return fcm;
    }



	public static FCM getSpeciesFCM(Species species)
	{

		if (species == Species.Rabbit)
		{

			return RabbitFCM();
		}
		else if (species == Species.Fox)
		{
			return FoxFCM();
		}
		else
		{
			return null;
		}

	}
}

