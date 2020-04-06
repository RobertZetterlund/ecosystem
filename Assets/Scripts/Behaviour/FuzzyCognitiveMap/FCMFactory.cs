
using System;

public static class FCMFactory
{
    public static FCM RabbitFCM()
    {
        EntityInput[] inputs = (EntityInput[])Enum.GetValues(typeof(EntityInput));
        EntityAction[] actions = (EntityAction[])Enum.GetValues(typeof(EntityAction));

        FCM fcm = new FCM(inputs, actions);

        // FOOD AND HUNGER
        fcm.SetWeight(EntityField.FoodClose, EntityField.GoingToFood, 0.05);
        fcm.SetWeight(EntityField.FoodFar, EntityField.GoingToFood, -0.05);
        fcm.SetWeight(EntityField.FoodPresenceHigh, EntityField.GoingToFood, 0.02);
        fcm.SetWeight(EntityField.FoodPresenceLow, EntityField.GoingToFood, -0.02);

        fcm.SetWeight(EntityField.Hungry, EntityField.GoingToFood, 0.3);
        fcm.SetWeight(EntityField.NotHungry, EntityField.GoingToFood, -0.2);


        fcm.SetWeight(EntityField.Hungry, EntityField.SearchingForMate, -0.1);
        fcm.SetWeight(EntityField.NotHungry, EntityField.SearchingForMate, 0.1);


        // WATER AND THIRST
        fcm.SetWeight(EntityField.WaterClose, EntityField.GoingToWater, 0.05);
        fcm.SetWeight(EntityField.WaterFar, EntityField.GoingToWater, -0.05);
        fcm.SetWeight(EntityField.WaterPresenceHigh, EntityField.GoingToWater, 0.02);
        fcm.SetWeight(EntityField.WaterPresenceLow, EntityField.GoingToWater, -0.02);

        fcm.SetWeight(EntityField.Thirsty, EntityField.GoingToWater, 0.3);
        fcm.SetWeight(EntityField.NotThirsty, EntityField.GoingToWater, -0.2);


        // MATE AND FERTILITY
        fcm.SetWeight(EntityField.MateClose, EntityField.SearchingForMate, 0.08);
        fcm.SetWeight(EntityField.MateFar, EntityField.SearchingForMate, -0.05);
        fcm.SetWeight(EntityField.MatePresenceHigh, EntityField.SearchingForMate, 0.02);
        fcm.SetWeight(EntityField.MatePresenceLow, EntityField.SearchingForMate, -0.02);

        fcm.SetWeight(EntityField.Fertile, EntityField.SearchingForMate, 0.2);
        fcm.SetWeight(EntityField.NotFertile, EntityField.SearchingForMate, -1);


        // FEAR AND ESCAPING
        fcm.SetWeight(EntityField.FoeClose, EntityField.Fear, 0.3);
        fcm.SetWeight(EntityField.FoeFar, EntityField.Fear, -0.1);
        fcm.SetWeight(EntityField.FoePresenceHigh, EntityField.Fear, 0.15);
        fcm.SetWeight(EntityField.FoePresenceLow, EntityField.Fear, -0.05);

        fcm.SetWeight(EntityField.Fear, EntityField.Fear, -0.07);

        fcm.SetWeight(EntityField.Fear, EntityField.Hungry, -0.1);
        fcm.SetWeight(EntityField.Fear, EntityField.NotHungry, 0.1);
        fcm.SetWeight(EntityField.Escaping, EntityField.GoingToFood, -0.15);

        fcm.SetWeight(EntityField.Fear, EntityField.SearchingForMate, -0.05);
        fcm.SetWeight(EntityField.Fear, EntityField.GoingToWater, -0.05);
        fcm.SetWeight(EntityField.Fear, EntityField.GoingToFood, -0.05);

        fcm.SetWeight(EntityField.Escaping, EntityField.GoingToWater, -0.15);
        fcm.SetWeight(EntityField.Escaping, EntityField.SearchingForMate, -0.15);

        return fcm;
    }

    // same as rabbit for now
    public static FCM FoxFCM()
    {
        EntityInput[] inputs = (EntityInput[])Enum.GetValues(typeof(EntityInput));
        EntityAction[] actions = (EntityAction[])Enum.GetValues(typeof(EntityAction));

        FCM fcm = new FCM(inputs, actions);

        fcm.SetWeight(EntityField.FoodClose, EntityField.GoingToFood, 0.05);
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

        //This should start of has being far away, unless anything else has been sensed
        /*fcm.SetState(EntityField.FoodFar, 1);
        fcm.SetState(EntityField.WaterFar, 1);
        fcm.SetState(EntityField.MateFar, 1);*/

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

