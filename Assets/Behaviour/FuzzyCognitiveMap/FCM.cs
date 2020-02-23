using System;
using UnityEngine;


public class FCM
{
    double[,] weights;
    double[] states;
    int NOFields;
    int NOActions;
    System.Random r = new System.Random();

    public enum Field
    {
        FoodClose,
        GoToFood,
        Idle
    }

    public enum Action
    {
        GoToFood = Field.GoToFood,
        Idle = Field.Idle
    }

    public enum Input
    {
        FoodClose = Field.FoodClose
    }

    // This is for fields such as "fear"
    public enum Intermediates
    {

    }

    public FCM()
    {
        NOFields = Enum.GetNames(typeof(Field)).Length;
        NOActions = Enum.GetNames(typeof(Action)).Length;
        weights = new double[NOFields, NOFields];
        states = new double[NOFields];
        Debug.developerConsoleVisible = true;
    }


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

    
    public Action GetAction()
    {
        Action a = Action.GoToFood;

        double sum = 0;
        for (int i = (int)a; i < NOFields; i++)
        {
            sum += states[i];
        }

        if (sum > 0)
        {
            for (int i = (int)a; i < NOFields; i++)
            {
                double prob = states[i] / sum;
                double roll = r.NextDouble();
                if (roll < prob)
                {
                    return (Action)i;
                }
            }
        }
        return Action.Idle;
    }


    public void ImpactState(Field state, double impact)
    {
        states[(int)state] += impact;
    }

    public double[] GetStates()
    {
        return states;
    }

    public void SetWeight(Field _from, Field _to, double weight)
    {
        weights[(int)_from, (int)_to] = weight;
    }

}
