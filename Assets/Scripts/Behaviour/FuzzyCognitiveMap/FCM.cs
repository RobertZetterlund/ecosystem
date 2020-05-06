using UnityEngine;
using System.Collections.Generic;
using System;


public class FCM
{
	public double[,] weights;
	double[] states;
	double[] stateIncreases;
	public int NOFields;
	int NOMiddles;
	int NOActions;
	int NOInputs;
	System.Random r = new System.Random();
	TwoWayMap<int, int> translation = new TwoWayMap<int, int>();


	public FCM(EntityInput[] inputs, EntityMiddle[] middles, EntityAction[] actions)
	{
		NOInputs = inputs.Length;
		NOMiddles = middles.Length;
		NOActions = actions.Length;
		NOFields = NOInputs + NOMiddles + NOActions;

		weights = new double[NOFields, NOFields];
		states = new double[NOFields];
		stateIncreases = new double[NOFields];


		EntityField[] fields = new EntityField[NOFields];
		inputs.CopyTo(fields, 0);
		middles.CopyTo(fields, NOInputs);
		actions.CopyTo(fields, NOInputs + NOMiddles);


		MapStates(fields);
	}

	private FCM(EntityInput[] inputs, EntityMiddle[] middles, EntityAction[] actions, double[,] weights) : this(inputs, middles, actions)
	{
		this.weights = weights;
	}


	/**
	 * Maps each EntityField value to a unique index in the fcm.
	 * 
	 * For instance, say that GoingToFood has the value 7, as defined
	 * in EntityFields. The translator will map GoingToFood to a new index specific to the
	 * instance of the fcm. This is done beacuse it makes it very easy to use EntityFields in order to adress
	 * states in the fcm, and the total number of indexes varies depending on what fcm is implemented.
	 * 
	 */
	private void MapStates(EntityField[] fields)
	{
		int i = 0;
		foreach (EntityField field in fields)
		{
			translation.Add((int)field, i);
			i++;
		}
	}

	/**
	 * Updates the FCM one time. This new value of each new state is the sum of all
	 * states multiplied with the weight from that state to the new state. The new values are not being used in
	 * the calculation of the remaining states, as this would create a bias to what states are calculated first vs last.
	 * The new values replaces the old values at the end of the function.
	 *
	 *
	 * Only inputs and middles affect middles and outputs. So outer loop is 0 to (NOFields - NOOutputs), which is equal to NOInputs+NOMiddles
	 * Innter loop is only the affected concepts, being middle and outputs. so it goes from middles to the end.
	 */
	public void Calculate()
	{
		double[] new_states = (double[])states.Clone();
		double action_Constant = 0.2;

		for (int _from = 0; _from < NOInputs+NOMiddles; _from++)
		{
			// if _from is zero, it has effectively no change, and we can skip calculations for this particular loop
			if(states[_from] == 0)
			{
				continue;
			}            
			for (int _to = NOMiddles; _to < NOFields; _to++)
			{
				// we're affecting actions
				if (_to >= NOActions)
				{
					double addition = weights[_from, _to] * states[_from] * action_Constant;
					new_states[_to] += addition;
					stateIncreases[_to] += addition;
					new_states[_to] = Mathf.Clamp((float)new_states[_to], 0, 1);
				}
				// we're affecting middles
				else
				{
				new_states[_to] += weights[_from, _to] * states[_from];
				new_states[_to] = Mathf.Clamp((float)new_states[_to], 0, 1);
				}
			}
		}

		states = new_states;
	}

	/**
	 * Returns an action from the fcm (currently done through Roulette Wheel Selection) where
	 * actions are selected based on the value that the action has in the state array.
	 * 
	 * For instance if GoingToFood = 0.5 and Idle = 1, Idle will have a twice as big chance at
	 * being selected as the returned action. If an action is equal to zero, it does not have any chance
	 * of being selected, and if all actions are equal to zero, the idle action is returned (for now) 
	 * 
	 */
	public EntityAction GetAction()
	{
		/*
		double sum = 0;
		for (int i = NOInputs; i < NOFields; i++)
		{
			sum += states[i];
		}

		if (sum > 0)
		{
			for (int i = NOInputs; i < NOFields; i++)
			{
				double prob = states[i] / sum;
				double roll = r.NextDouble();
				if (roll < prob)
				{
					return (EntityAction)translation.Reverse[i];
				}
			}
		}
		return EntityAction.Idle;
		*/

		double best = 0;
		double bests_increase = 0;
		int best_action = 0;
		for (int i = NOInputs + NOMiddles; i < NOFields; i++)
		{
			if (states[i] > best)
			{
				best = states[i];
				bests_increase = stateIncreases[i];
				best_action = i;
			}
			else if (states[i] == best && stateIncreases[i] > bests_increase)
			{

				best = states[i];
				bests_increase = stateIncreases[i];
				best_action = i;
			}
			stateIncreases[i] = 0;
		}

		return (EntityAction)translation.Reverse[best_action];

	}

	/**
	 * Impact a selected state with a value. Commonly used for
	 * sending input to the fcm such as Hunger or FoodClose, but it can be used for all states
	 * 
	 */
	/*
	public void ImpactState(EntityField state, double value)
	{
		int i = translation.Forward[(int)state];
		states[i] += value;
		states[i] = Mathf.Clamp((float)states[i], 0, 1);
	}
	*/
	public void SetState(EntityField state, double value)
	{
		int i = translation.Forward[(int)state];
		states[i] = value;
		states[i] = Mathf.Clamp((float)states[i], 0, 1);
	}

	public double[] GetStates()
	{
		return states;
	}

	// Getting these weights does not really mean anything since you need the translation table to see for instance what
	// _to and _from is in the case of weights[_to,_from]. It's different for different fcms
	public double[,] GetRawWeights()
	{
		return weights;
	}

	// Translates the weights and represents them in an array containing all possible fields. In this case we know that
	// if _from = 1, then that must mean the _from = FoodFar since FoodFar enum has the value 1
	public double[,] GetTranslatedWeights()
	{
		EntityField[] fields = (EntityField[])Enum.GetValues(typeof(EntityField));
		double[,] convertedWeights = new double[fields.Length, fields.Length];

		for (int _from = 0; _from < weights.GetLength(0); _from++)
		{
			for (int _to = 0; _to < weights.GetLength(1); _to++)
			{
				double weight = weights[_from, _to];
				convertedWeights[translation.Reverse[_from], translation.Reverse[_to]] = weight;
			}
		}

		return convertedWeights;
	}

	public TwoWayMap<int, int> GetTranslation()
	{
		return translation;
	}

	public void SetWeight(EntityField _from, EntityField _to, double weight)
	{
		int i_from = translation.Forward[(int)_from];
		int i_to = translation.Forward[(int)_to];
		weights[i_from, i_to] = weight;
	}

	public override string ToString()
	{
		string s = "";
		for (int i = 0; i < NOFields; i++)
		{
			s += (EntityField)translation.Reverse[i] + ": " + states[i] + "\n";
		}

		return s;
	}

	/*

	private EntityInput[] GetInputs()
	{
		EntityInput[] inputs = new EntityInput[NOInputs];
		for (int i = 0; i < NOInputs; i++)
		{
			inputs[i] = (EntityInput)translation.Reverse[i];
		}
		return inputs;
	}

	private EntityMiddle[] GetMiddles()
	{
		// kinda dumb cause the first half (noINput) is gonna be empty
		EntityMiddle[] middles = new EntityMiddle[NOMiddles];
		for (int i = NOInputs; i < NOFields; i++)
		{
			middles[i - NOInputs] = (EntityMiddle)translation.Reverse[i];
		}
		return middles;
	}

	private EntityAction[] GetActions()
	{
		// kinda dumb cause the first half (noINput) is gonna be empty
		EntityAction[] actions = new EntityAction[NOActions];
		for (int i = NOInputs + NOMiddles; i < NOFields; i++)
		{
			actions[i - (NOInputs + NOMiddles)] = (EntityAction)translation.Reverse[i];
		}
		return actions;
	}

	*/
	internal FCM Reproduce(FCM mateFCM)
	{
		// assume both mates have the same fields
		EntityField[] inputs = (EntityField[])Enum.GetValues(typeof(EntityInput));
		EntityField[] middles = (EntityField[])Enum.GetValues(typeof(EntityMiddle));
		EntityField[] actions = (EntityField[])Enum.GetValues(typeof(EntityAction));

		FCM child = FCMFactory.GetBaseFCM();

		BreedFields(inputs, middles, mateFCM, child);
		BreedFields(middles, middles, mateFCM, child);
		BreedFields(middles, actions, mateFCM, child);



		child.SetState(EntityField.FoodFar, 1);
		child.SetState(EntityField.WaterFar, 1);
		child.SetState(EntityField.MateFar, 1);

		return child;
	}

	internal void BreedFields(EntityField [] from, EntityField [] to, FCM mateFCM, FCM childFCM)
	{
		foreach (EntityField _from in from)
		{
			foreach (EntityField _to in to)
			{
				int i_from = translation.Forward[(int)_from];
				int i_to = translation.Forward[(int)_to];

				// get weights and mutate

				RangedDouble geneA = new RangedDouble(weights[i_from, i_to], -1, 1);
				RangedDouble geneB = new RangedDouble(mateFCM.weights[i_from, i_to], -1, 1);
				childFCM.SetWeight(_from, _to, ReproductionUtility.ReproduceRangedDouble(geneA, geneB).GetValue());
			}
		}
	}

	public void Randomise()
	{
		EntityField[] inputs = (EntityField[])Enum.GetValues(typeof(EntityInput));
		EntityField[] middles = (EntityField[])Enum.GetValues(typeof(EntityMiddle));
		EntityField[] actions = (EntityField[])Enum.GetValues(typeof(EntityAction));

		RandomiseFields(inputs, middles);
		RandomiseFields(middles, middles);
		RandomiseFields(middles, actions);

	}

	internal void RandomiseFields(EntityField[] from, EntityField[] to)
	{
		foreach (EntityField ei in from)
		{
			foreach (EntityField ea in to)
			{
				EntityField _from = (EntityField)Enum.Parse(typeof(EntityField), ei.ToString());
				EntityField _to = (EntityField)Enum.Parse(typeof(EntityField), ea.ToString());

				// randomise weights
				SetWeight(_from, _to, MathUtility.RandomUniform(-1, 1));
			}
		}
	}

	public FCM Duplicate()
	{
		EntityInput[] inputs = new EntityInput[NOInputs];
		EntityMiddle[] middles = new EntityMiddle[NOMiddles];
		EntityAction[] actions = new EntityAction[NOActions];

		for (int i = 0; i < NOInputs; i++)
		{
			inputs[i] = (EntityInput)translation.Reverse[i];
		}

		for (int i = NOInputs; i < NOFields; i++)
		{
			actions[i - NOInputs] = (EntityAction)translation.Reverse[i];
		}

		return new FCM(inputs, middles, actions, weights);
	}
}

