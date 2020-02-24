using System;

static class FCMFactory
{
    public static FCM RabbitFCM()
    {
        EntityAction[] actions = new EntityAction[] { EntityAction.GoingToFood, EntityAction.Idle };
        EntityInput[] inputs = new EntityInput[] { EntityInput.FoodClose };

        FCM fcm = new FCM(inputs, actions);

        fcm.SetWeight(EntityField.FoodClose, EntityField.GoingToFood, 0.05);

        return fcm;
    }
}

