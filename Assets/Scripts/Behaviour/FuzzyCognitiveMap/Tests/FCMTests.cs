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

            FCM fcm = CreateFCM();

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

        [Test]
        public void DefuzzificationTest()
        {
            EntityAction[] actions = new EntityAction[] { EntityAction.GoingToFood, EntityAction.Idle };
            EntityInput[] inputs = new EntityInput[] { EntityInput.FoodClose };

            FCM fcm = new FCM(inputs, actions);
            fcm.SetWeight(EntityField.FoodClose, EntityField.GoingToFood, 1);
            fcm.SetState(EntityField.GoingToFood, 1);
            fcm.SetState(EntityField.Idle, 0.5);

            Dictionary<EntityAction, int> dict = new Dictionary<EntityAction, int>();
            for(int i = 0; i < 1000; i++)
            {
                EntityAction action = fcm.GetAction();
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
