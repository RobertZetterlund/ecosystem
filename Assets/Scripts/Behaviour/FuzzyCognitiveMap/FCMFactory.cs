
public static class FCMFactory
{
    public static FCM RabbitFCM()
    {
        EntityAction[] actions = new EntityAction[] { EntityAction.GoingToFood, EntityAction.GoingToWater, EntityAction.Idle, EntityAction.SearchingForMate };
        EntityInput[] inputs = new EntityInput[] { EntityInput.FoodClose, EntityInput.FoodFar, 
                                                    EntityInput.WaterClose, EntityInput.WaterFar,
                                                    EntityInput.RabbitClose, EntityInput.RabbitFar};

        FCM fcm = new FCM(inputs, actions);

        fcm.SetWeight(EntityField.FoodClose, EntityField.GoingToFood, 0.05);
        fcm.SetWeight(EntityField.FoodFar, EntityField.GoingToFood, -0.05);
        fcm.SetWeight(EntityField.WaterClose, EntityField.GoingToWater, 0.05);
        fcm.SetWeight(EntityField.WaterFar, EntityField.GoingToWater, -0.05);
        fcm.SetWeight(EntityField.RabbitClose, EntityField.SearchingForMate, 0.05);
        fcm.SetWeight(EntityField.RabbitFar, EntityField.SearchingForMate, -0.05);

        //This should start of has being far away, unless anything else has been sensed
        fcm.SetState(EntityField.FoodFar, 1);
        fcm.SetState(EntityField.WaterFar, 1);
        fcm.SetState(EntityField.RabbitFar, 1);

        return fcm;
    }
}

