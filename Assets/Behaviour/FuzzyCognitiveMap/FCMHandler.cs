using UnityEngine;
using System.Collections;

public class FCMHandler
{

    private FCM fcm;

    public FCMHandler(FCM fcm)
    {
        this.fcm = fcm;
    }

    public void Calculate()
    {
        for (int i = 0; i < 20; i++)
        {
            fcm.Calculate();
        } 
    }
}
