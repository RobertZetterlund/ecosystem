using System;
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


    // This is for fields such as "fear"
    public enum Intermediates
    {

    }

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

    private void MapStates(EntityField[] fields)
    {
        int i = 0;
        foreach (EntityField field in fields)
        {
            translation.Add((int)field, i);
            i++;
        }
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


    public void ImpactState(EntityField state, double impact)
    {
        states[translation.Forward[(int)state]] += impact;
    }

    public double[] GetStates()
    {
        return states;
    }

    public void SetWeight(EntityField _from, EntityField _to, double weight)
    {
        weights[translation.Forward[(int)_from], translation.Forward[(int)_to]] = weight;
    }

}
