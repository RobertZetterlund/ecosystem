
public static class FCMFactory
{
    public static FCM RabbitFCM()
    {
        EntityAction[] actions = new EntityAction[] { EntityAction.GoingToFood, EntityAction.GoingToWater, EntityAction.Idle, EntityAction.SearchingForMate };
        EntityInput[] inputs = new EntityInput[] { EntityInput.FoodClose, EntityInput.FoodFar, 
                                                    EntityInput.WaterClose, EntityInput.WaterFar,
                                                    EntityInput.RabbitClose, EntityInput.RabbitFar,
                                                    EntityInput.Fertile, EntityInput.NotFertile,
                                                    EntityInput.Hungry, EntityInput.NotHungry};

        FCM fcm = new FCM(inputs, actions);

        fcm.SetWeight(EntityField.FoodClose, EntityField.GoingToFood, 0.05);
        fcm.SetWeight(EntityField.FoodFar, EntityField.GoingToFood, -0.05);
        fcm.SetWeight(EntityField.WaterClose, EntityField.GoingToWater, 0.05);
        fcm.SetWeight(EntityField.WaterFar, EntityField.GoingToWater, -0.05);
        fcm.SetWeight(EntityField.RabbitClose, EntityField.SearchingForMate, 0.08);
        fcm.SetWeight(EntityField.RabbitFar, EntityField.SearchingForMate, -0.05);
        fcm.SetWeight(EntityField.Fertile, EntityField.SearchingForMate, 0.2);
        fcm.SetWeight(EntityField.NotFertile, EntityField.SearchingForMate, -1);
        fcm.SetWeight(EntityField.Hungry, EntityField.GoingToFood, 0.3);
        fcm.SetWeight(EntityField.NotHungry, EntityField.GoingToFood, -0.2);

        fcm.SetWeight(EntityField.Hungry, EntityField.SearchingForMate, -0.1);
        fcm.SetWeight(EntityField.NotHungry, EntityField.SearchingForMate, 0.1);

        //This should start of has being far away, unless anything else has been sensed
        fcm.SetState(EntityField.FoodFar, 1);
        fcm.SetState(EntityField.WaterFar, 1);
        fcm.SetState(EntityField.RabbitFar, 1);

        return fcm;
    }
}

