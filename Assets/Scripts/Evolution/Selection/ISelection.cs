using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public interface ISelection
{
	int Select(double[] values);
	int[] Select(double[] values, int amount);
	T Select<T>(T[] pool, double[] values);
	T[] Select<T>(T[] pool, double[] values, int amount);

}

