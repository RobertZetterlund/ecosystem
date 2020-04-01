using UnityEngine;
using System.Collections.Generic;
using System;


public class FCM
{
    double[,] weights;
    double[] states;
    int NOFields;
    int NOActions;
    int NOInputs;
    System.Random r = new System.Random();
    TwoWayMap<int, int> translation = new TwoWayMap<int, int>();


    public FCM(EntityInput[] inputs, EntityAction[] actions)
    {
        NOInputs = inputs.Length;
        NOActions = actions.Length;
        NOFields = NOInputs + NOActions;

        weights = new double[NOFields, NOFields];
        states = new double[NOFields];


        EntityField[] fields = new EntityField[NOFields];
        inputs.CopyTo(fields, 0);
        actions.CopyTo(fields, NOInputs);


        MapStates(fields);
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
     */
    public void Calculate()
    {
        double[] new_states = (double[])states.Clone();

        for(int _from = 0; _from < NOFields; _from++)
        {
            for (int _to = 0; _to < NOFields; _to++)
            {
                new_states[_to] += weights[_from, _to] * states[_from];
                new_states[_to] = Mathf.Clamp((float)new_states[_to], 0, 1);
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
        int best_action = 0;
        for (int i = NOInputs; i < NOFields; i++)
        {
            if (states[i] >= best)
            {
                best = states[i];
                best_action = i;
            }
                
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

    private EntityInput[] GetInputs()
    {
        EntityInput[] inputs = new EntityInput[NOInputs];
        for (int i = 0; i < NOInputs; i++)
        {
            inputs[i] = (EntityInput)translation.Reverse[i];
        }
        return inputs;
    }

    private EntityAction[] GetActions()
    {
        // kinda dumb cause the first half (noINput) is gonna be empty
        EntityAction[] actions = new EntityAction[NOActions];
        for (int i = NOInputs; i < NOFields; i++)
        {
            actions[i-NOInputs] = (EntityAction)translation.Reverse[i];
        }
        return actions;
    }



    internal FCM Reproduce(FCM mateFCM)
    {
        // make entity parameters for child
        // basically take union of both parents
        HashSet<EntityInput> inputs = new HashSet<EntityInput>(GetInputs());
        HashSet<EntityAction> actions = new HashSet<EntityAction>(GetActions());

        HashSet<EntityInput> mateInputs = new HashSet<EntityInput>(mateFCM.GetInputs());
        HashSet<EntityAction> mateActions = new HashSet<EntityAction>(mateFCM.GetActions());

        inputs.UnionWith(mateInputs);
        actions.UnionWith(mateActions);

        EntityInput[] childInputs = new EntityInput[inputs.Count];
        EntityAction[] childActions = new EntityAction[actions.Count];

        inputs.CopyTo(childInputs);
        actions.CopyTo(childActions);


        FCM child = new FCM(childInputs, childActions);

        int childFields = childInputs.Length + childActions.Length;
        double[,] childWeights = new double[childFields, childFields];
        Dictionary<(EntityInput, EntityAction), double> childWeights2 = new Dictionary<(EntityInput, EntityAction), double>();
        // set flag for weights that dont exist
        for (int i = 0; i < childWeights.GetLength(0); i++)
        {
            for (int j = 0; j < childWeights.GetLength(1); j++)
            {
                childWeights[i, j] = double.MinValue;
            }
        }

        foreach (EntityInput ei in childInputs)
        {
            foreach(EntityAction ea in childActions)
            {
                EntityField _from = (EntityField)Enum.Parse(typeof(EntityField), ei.ToString());
                EntityField _to = (EntityField)Enum.Parse(typeof(EntityField), ea.ToString());

                try
                {
                    int i_from = translation.Forward[(int)_from];
                    int i_to = translation.Forward[(int)_to];
                    // childweights[ei,ea]=
                    childWeights2.Add((ei,ea), weights[i_from, i_to]);
                } catch (Exception)
                {
                    // ignore if field doesnt exist
                }
                try
                {
                    int i_from = mateFCM.translation.Forward[(int)_from];
                    int i_to = mateFCM.translation.Forward[(int)_to];

                    if (!childWeights2.ContainsKey((ei, ea)))
                    {
                        childWeights2.Add((ei, ea), weights[i_from, i_to]);
                    }
                    else
                    {
                        RangedDouble geneA = new RangedDouble(childWeights2[(ei, ea)], -1, 1);
                        RangedDouble geneB = new RangedDouble(mateFCM.weights[i_from, i_to], -1, 1);
                        childWeights2[(ei, ea)] = ReproductionUtility.ReproduceRangedDouble(geneA, geneB).GetValue();
                    }
                } catch (Exception)
                {
                    // ignore if field doesnt exist
                }
                double geneC = childWeights2[(ei, ea)];
                child.SetWeight(_from, _to, geneC);
            }
        }

        return child;
        // mutate and add to child
        /*for (int i = 0; i < maxWeightedInputs; i++)
        {
            for (int j = 0; j < maxWeightedActions; j++)
            {
                child.SetWeight((EntityField)i, (EntityField)j,
                    ReproductionUtility.ReproduceRangedDouble(new RangedDouble(childWeights[i, j], -1, 1)).GetValue());
            }
        }
        */

        // dont set states for now, maybe kid shouldnt know anything? idk
        /*
        // set states
        int states = this.states.Length;
        int mateStates = mateFCM.states.Length;

        int maxStates = Math.Max(states, mateStates);
        double[] childstates = new double[maxStates];

        foreach (double state in states)
        {
            fcm.SetState(EntityField.WaterFar, 1);
        }
        */
    }

}
