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
		/*[Test]
        public void FcmCsvTest()
        {
            FCM fcm = FCMFactory.RabbitFCM();

            FCMHandler fCMHandler = new RabbitFCMHandler(fcm);

            string s = fCMHandler.ToCsv().ToString();

            fCMHandler.SaveFCM(s, "SavedFCM");

            
        }*/

		[Test]
		public void CreateFCM()
		{
			FCM fcm = FCMFactory.GetBaseFCM();
			fcm.Randomise();

			Debug.Log("done");
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
