using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tetryds.Reumpr;
using tetryds.Reumpr.Collections;
using tetryds.Reumpr.Tests.Tools;

namespace tetryds.Reumpr.Tests
{
    [TestClass]
    public class ConcurrentHashTests
    {
        [TestMethod]
        public void AddItem()
        {
            const int Count = 50;

            ConcurrentHash<int> hash = new ConcurrentHash<int>();

            var values = Enumerable.Range(0, Count).ToArray();

            Task[] tasks = new Task[Count];

            for (int i = 0; i < values.Length; i++)
            {
                int value = values[i];
                tasks[i] = Task.Run(() => hash.TryAdd(value));
            }

            Task.WaitAll(tasks);

            Assert.AreEqual(Count, hash.Count);
            CollectionAssert.AreEqual(values, hash.ToList());
        }

        [TestMethod]
        public void ContainsItem()
        {
            const int Count = 50;

            ConcurrentHash<int> hash = new ConcurrentHash<int>();

            var values = Enumerable.Range(0, Count).ToArray();

            Task[] tasks = new Task[Count];

            for (int i = 0; i < values.Length; i++)
            {
                int value = values[i];
                tasks[i] = Task.Run(() => hash.TryAdd(value));
            }

            Task.WaitAll(tasks);

            Assert.AreEqual(Count, hash.Count);
            for (int i = 0; i < values.Length; i++)
            {
                Assert.IsTrue(hash.Contains(i));
            }
        }

        [TestMethod]
        public void RemoveItem()
        {
            const int Count = 50;

            ConcurrentHash<int> hash = new ConcurrentHash<int>();

            var values = Enumerable.Range(0, Count).ToArray();

            Task[] tasks = new Task[Count];

            for (int i = 0; i < values.Length; i++)
            {
                int value = values[i];
                tasks[i] = Task.Run(() => hash.TryAdd(value));
            }

            Task.WaitAll(tasks);

            for (int i = 0; i < values.Length; i++)
            {
                int value = values[i];
                tasks[i] = Task.Run(() => hash.Remove(value));
            }

            Task.WaitAll(tasks);

            Assert.AreEqual(0, hash.Count);
            CollectionAssert.AreEqual(new int[0], hash.ToArray());
        }

        [TestMethod]
        public void Clear()
        {
            const int Count = 50;

            ConcurrentHash<int> hash = new ConcurrentHash<int>();

            var values = Enumerable.Range(0, Count).ToArray();

            Task[] tasks = new Task[Count];

            for (int i = 0; i < values.Length; i++)
            {
                int value = values[i];
                tasks[i] = Task.Run(() => hash.TryAdd(value));
            }

            Task.WaitAll(tasks);

            hash.Clear();

            Assert.AreEqual(0, hash.Count);
            CollectionAssert.AreEqual(new int[0], hash.ToArray());
        }
    }
}
