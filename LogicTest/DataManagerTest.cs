using Data;

using Logic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicTest
{
    [TestClass]
    public class DataManagerTest
    {
        private class InvalidDataType1 : IUpdatable<InvalidDataType1>
        {
            public void Update(InvalidDataType1 t)
            {
                throw new NotImplementedException();
            }
        }

        private class InvalidDataType2 : IUpdatable<InvalidDataType2>
        {
            [Id]
            public uint Id
            {
                get;
                set;
            }
            [Id]
            public string StringData
            {
                get;
                set;
            }

            public void Update(InvalidDataType2 t)
            {
                throw new NotImplementedException();
            }
        }

        private class TestDataType : IUpdatable<TestDataType>
        {
            [Id]
            public uint Id
            {
                get;
                set;
            }

            public string StringData
            {
                get;
                set;
            }

            public void Update(TestDataType t)
            {
                throw new NotImplementedException();
            }
        }
        private class InvalidDataManager1 : DataManager<InvalidDataType1, uint> { }
        private class InvalidDataManager2 : DataManager<InvalidDataType2, uint> { }
        private class InvalidTestDataManager : DataManager<TestDataType, double> { }

        private class TestDataManager : DataManager<TestDataType, uint> { }

        private class TestDataObserver : IObserver<DataChanged<TestDataType>>
        {
            public int CompleteCount { get; private set; }
            public Queue<Exception> Errors { get; } = new Queue<Exception>();
            public Queue<DataChanged<TestDataType>> Next { get; } = new Queue<DataChanged<TestDataType>>();

            public void OnCompleted()
            {
                ++CompleteCount;
            }

            public void OnError(Exception error)
            {
                Errors.Enqueue(error);
            }

            public void OnNext(DataChanged<TestDataType> value)
            {
                Next.Enqueue(value);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void Construction_NoIdProperty_Throws()
        {
            new InvalidDataManager1();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void Construction_MoreThanOneIdProperty_Throws()
        {
            new InvalidDataManager2();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void Construction_WrongIdPropertyType_Throws()
        {
            new InvalidTestDataManager();
        }

        [TestMethod]
        public void Construction_ValidTypesAndId_Successful()
        {
            new TestDataManager();
        }

        [TestMethod]
        public void Add_AddedOnceAndEventRaised()
        {
            TestDataManager dm = new TestDataManager();
            TestDataType data = new TestDataType();
            TestDataObserver obs = new TestDataObserver();
            using IDisposable unsubscriber = dm.Subscribe(obs);
            try
            {
                dm.Add(data);
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            try
            {
                dm.Add(data);
                Assert.Fail();
            }
            catch (Exception)
            {
                // ignored
            }

            Assert.AreEqual(0, obs.CompleteCount);
            Assert.AreEqual(0, obs.Errors.Count);
            Assert.AreEqual(1, obs.Next.Count);
            DataChanged<TestDataType> change = obs.Next.Dequeue();
            Assert.IsTrue(change.Action == DataChangedAction.Add && change.NewItems?.FirstOrDefault() == data);
        }

        [TestMethod]
        public void GetWithoutAdd_ReturnsNull()
        {
            TestDataManager dm = new TestDataManager();
            Assert.IsNull(dm.Get(0U));
        }

        [TestMethod]
        public void GetAdded_ReturnsNotNull()
        {
            TestDataManager dm = new TestDataManager();
            dm.Add(new TestDataType { Id = 1U });
            Assert.IsNotNull(dm.Get(1U));
        }

        [TestMethod]
        public void Remove_RemovedOnceAndEventRaised()
        {
            TestDataManager dm = new TestDataManager();
            TestDataType data = new TestDataType { Id = 1U };
            dm.Add(data);
            TestDataObserver obs = new TestDataObserver();
            using IDisposable unsubscriber = dm.Subscribe(obs);
            Assert.IsTrue(dm.Remove(data.Id));
            Assert.IsFalse(dm.Remove(data.Id));
            Assert.AreEqual(0, obs.CompleteCount);
            Assert.AreEqual(0, obs.Errors.Count);
            Assert.AreEqual(1, obs.Next.Count);
            DataChanged<TestDataType> change = obs.Next.Dequeue();
            Assert.IsTrue(change.Action == DataChangedAction.Remove && change.OldItems?.FirstOrDefault() == data);
        }

        [TestMethod]
        public void Reset_ResetAndEventRaised()
        {
            TestDataManager dm = new TestDataManager();
            TestDataType data = new TestDataType();
            dm.Add(data);
            TestDataObserver obs = new TestDataObserver();
            using IDisposable unsubscriber = dm.Subscribe(obs);
            dm.Reset();
            Assert.IsFalse(dm.GetAll().Any());
            Assert.AreEqual(0, obs.CompleteCount);
            Assert.AreEqual(0, obs.Errors.Count);
            Assert.AreEqual(1, obs.Next.Count);
            DataChanged<TestDataType> change = obs.Next.Dequeue();
            Assert.IsTrue(change.Action == DataChangedAction.Reset && change.OldItems?.FirstOrDefault() == data);
        }

        [TestMethod]
        public void Replace_ReplacedAndEventRaised()
        {
            TestDataManager dm = new TestDataManager();
            TestDataType data1 = new TestDataType { Id = 1U }, data2 = new TestDataType { Id = 2U };
            dm.Add(data1);

            TestDataObserver obs = new TestDataObserver();
            using IDisposable unsubscriber = dm.Subscribe(obs);
            Assert.AreEqual(dm.Get(1U), data1);
            dm.ReplaceData(new HashSet<TestDataType> { data2 });
            Assert.AreEqual(dm.Get(2U), data2);
            Assert.AreEqual(0, obs.CompleteCount);
            Assert.AreEqual(0, obs.Errors.Count);
            Assert.AreEqual(1, obs.Next.Count);
            DataChanged<TestDataType> change = obs.Next.Dequeue();
            Assert.IsTrue(change.Action == DataChangedAction.Replace
                          && change.OldItems?.FirstOrDefault() == data1
                          && change.NewItems?.FirstOrDefault() == data2);
        }
    }
}
