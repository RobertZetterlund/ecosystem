using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

namespace Tests
{
    public class ReproductionTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void RangedIntTest()
        {
            // try invalid bounds
            RangedInt a;
            try
            {
                a = new RangedInt(0, 1, 2);
                Assert.Fail();
            } catch (ArgumentException)
            {
            }

            try
            {
                a = new RangedInt(0, -2,-1);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            try
            {
                a = new RangedInt(0, -2, -3);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            try
            {
                a = new RangedInt(0, 2, 1);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            // try valid bound
            try
            {
                a = new RangedInt(0, -1, 1);
            }
            catch (ArgumentException)
            {
                Assert.Fail();
            }

            try
            {
                a = new RangedInt(0, -1);
            }
            catch (ArgumentException)
            {
                Assert.Fail();
            }

            // try add

            a = new RangedInt(0, -2, 3);
            Debug.Assert(a.Add(4) == 3);
        }

        [Test]
        public void RangedDoubleTest()
        {
            // try invalid bounds
            RangedDouble a;
            try
            {
                a = new RangedDouble(0, 1, 2);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            try
            {
                a = new RangedDouble(0, -2, -1);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            try
            {
                a = new RangedDouble(0, -2, -3);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            try
            {
                a = new RangedDouble(0, 2, 1);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            // try valid bound
            try
            {
                a = new RangedDouble(0, -1, 1);
            }
            catch (ArgumentException)
            {
                Assert.Fail();
            }

            try
            {
                a = new RangedDouble(0, -1);
            }
            catch (ArgumentException)
            {
                Assert.Fail();
            }

            // try add

            a = new RangedDouble(0, -2, 3);
            double result = a.Add(4);
            Debug.Log(result + "add");
            Debug.Assert(result == 3);
        }

        [Test]
        public void DoubleMutationDifferentBoundsTest()
        {
            RangedDouble a = new RangedDouble(3, 2, 4);
            RangedDouble b = new RangedDouble(5, 3, 6);
            RangedDouble c;

            try
            {
                c = ReproductionUtility.ReproduceRangedDouble(a, b);
                Assert.Fail();
            }
            catch (Exception)
            {

            }
        }

        [Test]
        public void DoubleMutationTest()
        {

            for (int i = 0; i < 100; i++)
            {
                double lower = MathUtility.RandomUniform(-100, 100);
                double upper = MathUtility.RandomUniform(lower, lower + 100);
                double value1 = MathUtility.RandomUniform(lower, upper);
                double value2 = MathUtility.RandomUniform(lower, upper);
                RangedDouble a = new RangedDouble(value1, lower, upper);
                RangedDouble b = new RangedDouble(value2, lower, upper);
                RangedDouble c = ReproductionUtility.ReproduceRangedDouble(a, b);

                Debug.Assert(c.GetLower() == a.GetLower() && c.GetLower() == b.GetLower());
                Debug.Assert(c.GetUpper() == a.GetUpper() && c.GetUpper() == b.GetUpper());

                double alpha = BlendCrossover.GetInstance().alpha;
                double exploitation = Math.Abs(a.GetValue() - b.GetValue());
                double exploration = exploitation * alpha;
                Debug.Assert(c.GetValue() <= (a.GetUpper() + exploration) && c.GetValue() >= (a.GetLower() - exploration));
                Debug.Log(c.GetValue());

            }
        }

        [Test]
        public void IntMutationTest()
        {
            RangedInt a = new RangedInt(3, 2, 4);
            RangedInt b = new RangedInt(5, 3, 6);

            RangedInt c = ReproductionUtility.ReproduceRangedInt(a, b);

            Debug.Assert(c.GetLower() == a.GetLower() || c.GetLower() == b.GetLower());
            Debug.Assert(c.GetUpper() == a.GetUpper() || c.GetUpper() == b.GetUpper());
            Debug.Assert(c.GetValue() <= a.GetUpper() && c.GetValue() >= a.GetLower() ||
                c.GetValue() <= b.GetUpper() && c.GetValue() >= b.GetLower());
        }

        /*
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator FCMTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
        */
    }
}
