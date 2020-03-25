using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class FCMTests
    {
        // A Test behaves as an ordinary method
        //[Ignore("dont want these mixed with real logs")]
        [Test]
        public void FcmCsvTest()
        {
            FCM fcm = FCMFactory.RabbitFCM();

            FCMHandler fCMHandler = new RabbitFCMHandler(fcm);

            string s = fCMHandler.ToCsv().ToString();

            fCMHandler.SaveFCM(s, "SavedFCM");

            
        }
        [Test]
        public void NoFieldFoundTest()
        {
            FCM fcm = CreateFCM();
            fcm.SetState(EntityField.Reproducing, 1);
        }

        private FCM CreateFCM()
        {
            EntityAction[] actions = new EntityAction[] { EntityAction.GoingToFood, EntityAction.Idle };
            EntityInput[] inputs = new EntityInput[] { EntityInput.FoodClose };

            FCM fcm = new FCM(inputs, actions);
            fcm.SetWeight(EntityField.FoodClose, EntityField.GoingToFood, 1);
            fcm.SetState(EntityField.FoodClose, 1);
            fcm.SetState(EntityField.Idle, 0.5);

            return fcm;
        }


        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator FCMTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
