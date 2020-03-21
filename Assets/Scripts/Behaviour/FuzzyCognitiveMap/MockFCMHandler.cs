using System;
using System.Collections;

class MockFCMHandler : FCMHandler
{
    EntityAction action = EntityAction.Idle;
    FCMHandler fcmHandler;
    public MockFCMHandler(FCMHandler fcmHandler) : base(fcmHandler.GetFCM())
    {
        this.fcmHandler = fcmHandler;
    }

    public override EntityAction GetAction()
    {
        return action;
    }

    public override void ProcessSensedObjects(Animal animal, ArrayList gameObjects)
    {
        fcmHandler.ProcessSensedObjects(animal, gameObjects);
    }

    public override FCMHandler Reproduce(FCMHandler mateHandler)
    {
        return fcmHandler.Reproduce(mateHandler);
    }

    public void SetAction(EntityAction action)
    {
        this.action = action;
    }
}

