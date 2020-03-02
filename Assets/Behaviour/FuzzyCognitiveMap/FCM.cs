﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    }

    /**
     * Impact a selected state with a value. Commonly used for
     * sending input to the fcm such as Hunger or FoodClose, but it can be used for all states
     * 
     */
    public void ImpactState(EntityField state, double value)
    {
        int i = translation.Forward[(int)state];
        states[i] += value;
        states[i] = Mathf.Clamp((float)states[i], 0, 1);
    }

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

}