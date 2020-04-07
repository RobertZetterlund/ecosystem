using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class EvolutionTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void RouletteSelectionTest()
        {
            ISelection selection = RouletteSelection.Instance;

            double[] fitness = new double[] {0.1, 0.4, 0.3, 0.2};
            int[] results = new int[fitness.Length];

            for(int i = 0; i < 1000; i++)
            {
                results[selection.Select(fitness)]++; ;
            }

            Assert.IsTrue(results[1] > results[2]);
            Assert.IsTrue(results[3] > results[0]);
            Assert.IsTrue(results[2] > results[3]);
        }

        [Test]
        public void UniformCrossoverTest()
        {
            ICrossover crossover = UniformCrossover.Instance;

            int[] results = new int[2];
            for(int i = 0; i < 1000; i++)
            {
                RangedDouble geneA = new RangedDouble(1, 0);
                RangedDouble geneB = new RangedDouble(0, 0);
                RangedDouble resultingGene = crossover.Crossover(geneA, geneB);
                results[(int)resultingGene.GetValue()]++;
            }

            //They shouldnt differ by much if uniform. Maybe 100 isnt the most scientific pick but whatever.
            Assert.IsTrue(Math.Abs(results[0] - results[1]) < 100);
        }
    }
}
