using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/**
 * FCMHandler works as an observer to GameObject so that senses can be passed to the handler
 * from a SenseRegistrator
 * 
 */
public abstract class FCMHandler
{

	public IFuzzifier fuzzifier;

	protected FCM fcm;

	public FCMHandler(FCM fcm)
	{
		this.fcm = fcm;
	}

	public void CalculateFCM()
	{
		fcm.Calculate();
	}

	public virtual EntityAction GetAction()
	{
		return fcm.GetAction();
	}

	public string GetFCMData()
	{
		return fcm.ToString();
	}

	public FCM GetFCM()
	{
		return fcm;
	}

	public abstract void ProcessSensedObjects(Animal animal, SensedEvent sE);

	public abstract FCMHandler Reproduce(FCMHandler mateHandler);

	public void ProcessAnimal(double thirst, double energy, double dietFactor,
		bool isMale, double nChildren, double size, double speed, bool isFertile, double maxSize)
	{
		double fertility = isFertile ? 1 : 0;
		fcm.SetState(EntityField.Fertile, fertility);
		fcm.SetState(EntityField.NotFertile, 1 - fertility);

		fcm.SetState(EntityField.Hungry, 1 - size / maxSize);
		fcm.SetState(EntityField.NotHungry, size / maxSize);

		fcm.SetState(EntityField.Thirsty, thirst);
		fcm.SetState(EntityField.NotThirsty, 1 - thirst);
	}

	// Returns the translated weights
	public double[,] GetWeights()
	{
		return fcm.GetTranslatedWeights();
	}

	// Fuzzifier for presence
	private float calculatePresenceInput(int count)
	{
		float weight = (float)Math.Log10(count + 1);
		return weight > 1 ? 1 : weight;
	}

	public void SetInversePresenceInputFields(EntityInput close, EntityInput far, float value)
	{
		fcm.SetState((EntityField)close, value);
		fcm.SetState((EntityField)far, 1 - value);
	}


	public Dictionary<string, float> CorrectWeightMapToInputs(Dictionary<string, int> weightMap)
	{

		Dictionary<string, int>.Enumerator enumerator = weightMap.GetEnumerator();
		Dictionary<string, float> fcmInput = new Dictionary<string, float>();

		while (enumerator.MoveNext())
		{
			KeyValuePair<string, int> curr = enumerator.Current;
			float adjustedvalue = calculatePresenceInput(curr.Value);
			fcmInput.Add(curr.Key, adjustedvalue);
		}

		return fcmInput;
	}

	// Sets the two input fields as "komplement" to eachother in the fcm.
	public void SetInverseDistanceInputFields(EntityInput close, EntityInput far, GameObject sensedObject, Animal animal)
	{
		float standard = 0;
		float inverse = 1;
		if (sensedObject != null)
		{
			float dist = (sensedObject.transform.position - animal.transform.position).magnitude;
			standard = fuzzifier.Fuzzify(0, 100, dist);
			inverse = 1 - standard;
		}
		fcm.SetState((EntityField)close, standard);
		fcm.SetState((EntityField)far, inverse);
	}


	public StringBuilder ToCsv()
	{
		return ToCsv(GetWeights());
	}

	public static StringBuilder ToCsv(double[,] weights)
	{
		StringBuilder csv = new StringBuilder("");
		csv.Append("from,to,weight");
		csv.AppendLine();


		for (int _from = 0; _from < weights.GetLength(0); _from++)
		{
			for (int _to = 0; _to < weights.GetLength(1); _to++)
			{
				double weight = weights[_from, _to];
				csv.Append(((EntityField)_from).ToString() + "," + ((EntityField)_to).ToString() + "," + weight.ToString(System.Globalization.CultureInfo.InvariantCulture));
				csv.AppendLine();
			}
		}

		return csv;

	}
}
