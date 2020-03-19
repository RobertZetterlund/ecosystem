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
        public void FCMJSONTest()
        {
            FCM fcm = FCMFactory.RabbitFCM();

            FCMHandler fCMHandler = new RabbitFCMHandler(fcm);

            string s = fCMHandler.GenerateJSON();
            Debug.Log(s);

            fCMHandler.SaveFCM(s, "SavedFCM");

            
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
