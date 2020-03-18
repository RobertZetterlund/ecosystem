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
        public void DoubleMutationTest()
        {
            RangedDouble a = new RangedDouble(3, 2, 4);
            RangedDouble b = new RangedDouble(5, 3, 6);

            RangedDouble c = ReproductionUtility.ReproduceRangedDouble(a, b);

            Debug.Assert(c.GetLower() == a.GetLower() || c.GetLower() == b.GetLower());
            Debug.Assert(c.GetUpper() == a.GetUpper() || c.GetUpper() == b.GetUpper());
            Debug.Assert(c.GetValue() <= a.GetUpper() && c.GetValue() >= a.GetLower() ||
                c.GetValue() <= b.GetUpper() && c.GetValue() >= b.GetLower());
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
