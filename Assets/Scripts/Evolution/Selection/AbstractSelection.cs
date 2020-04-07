using System;
using System.Collections.Generic;
using System.Linq;

// All denna efterblivna generics för att få in singleton :)
public abstract class AbstractSelection<T> : SingletonBase<T>, ISelection where T : AbstractSelection<T>, new()
{
    public abstract int Select(double[] values);

    public int[] Select(double[] values, int amount)
    {
        int[] newGeneration = new int[amount];

        IList<double> value_list = values.ToList();
        for (int i = 0; i < amount && value_list.Count != 0; i++)
        {
            int selectedIndex = Select(value_list.ToArray());
            newGeneration[i] = selectedIndex;
            value_list.RemoveAt(selectedIndex);
        }
        return newGeneration;
    }

    public T1 Select<T1>(T1[] pool, double[] values)
    {
        return pool[Select(values)];
    }

    public T1[] Select<T1>(T1[] pool, double[] values, int amount)
    {
        T1[] newGeneration = new T1[amount];
        int[] indexes = Select(values, amount);
        for (int i = 0; i < indexes.Length; i++)
        {
            newGeneration[i] = pool[indexes[i]];
        }

        return newGeneration;
    }
}

