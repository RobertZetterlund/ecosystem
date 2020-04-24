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
		// using base 7
		float weight = (float)Math.Log(count + 1, 7);
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
			float dist = DistanceBetweenUtility.DistanceBetweenTwoGameObjects(sensedObject, animal);
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

	public static StringBuilder ToCsvRow(double[,] weights, bool isHeader)
	{
		EntityField[] inputs = (EntityField[])Enum.GetValues(typeof(EntityInput));
		EntityField[] middles = (EntityField[])Enum.GetValues(typeof(EntityMiddle));
		EntityField[] actions = (EntityField[])Enum.GetValues(typeof(EntityAction));

		int nbrOfWeights = inputs.Length * middles.Length + middles.Length * middles.Length + middles.Length * actions.Length;
		(EntityField, EntityField, double)[] output = new (EntityField, EntityField, double)[nbrOfWeights];


		int index = 0;
		int size = inputs.Length + middles.Length + actions.Length;

		foreach (EntityField from in inputs)
		{
			foreach (EntityField to in middles)
			{
				output[index] = (from, to, weights[(int)from, (int)to]);
				index++;
			}
		}

		int index2 = index + middles.Length * middles.Length;

		foreach (EntityField from in middles)
		{
			foreach (EntityField to in middles)
			{
				output[index] = (from, to, weights[(int)from, (int)to]);
				index++;
			}
			foreach (EntityField to in actions)
			{
				output[index2] = (from, to, weights[(int)from, (int)to]);
				index2++;
			}
		}

		//

		StringBuilder csv = new StringBuilder("");
		for (int i = 0; i < output.Length; i++)
		{
			if (isHeader)
			{
				csv.Append(output[i].Item1.ToString());
				csv.Append('-');
				csv.Append(output[i].Item1.ToString());
			}
			else
			{
				csv.Append(output[i].Item3.ToString(System.Globalization.CultureInfo.InvariantCulture));
			}
			csv.Append(',');
		}
		csv.Length--;
		return csv;
	}
}