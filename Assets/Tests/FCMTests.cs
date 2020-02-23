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
        [Test]
        public void FCMTest()
        {
            FCM fcm = new FCM();
            fcm.SetWeight(FCM.Field.FoodClose, FCM.Field.GoToFood, 1);
            fcm.ImpactState(FCM.Field.FoodClose, 1);
            fcm.ImpactState(FCM.Field.Idle, 0.5);

            fcm.Calculate();
            foreach(double d in fcm.GetStates()) 
            {
                Debug.Log(d);
            }

            Debug.Log(" ");

            fcm.Calculate();

            foreach (double d in fcm.GetStates())
            {
                Debug.Log(d);
            }

            Debug.Log(" ");
        }

        [Test]
        public void DefuzzificationTest()
        {
            FCM fcm = new FCM();
            fcm.SetWeight(FCM.Field.FoodClose, FCM.Field.GoToFood, 1);
            fcm.ImpactState(FCM.Field.GoToFood, 1);
            fcm.ImpactState(FCM.Field.Idle, 0.5);

            Dictionary<FCM.Action, int> dict = new Dictionary<FCM.Action, int>();
            for(int i = 0; i < 1000; i++)
            {
                FCM.Action action = fcm.GetAction();
                if (dict.ContainsKey(action)) {
                    dict[action] += 1;
                }
                else
                {
                    dict.Add(action, 1);
                }
            }

            Debug.Log(dict.ToString());
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
