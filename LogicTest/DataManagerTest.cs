using Data;
using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
                throw new System.NotImplementedException();
            }
        }
        private class InvalidDataManager1 : DataManager<InvalidDataType1, uint> { }
        private class InvalidDataManager2 : DataManager<InvalidDataType2, uint> { }
        private class InvalidTestDataManager : DataManager<TestDataType, double> { }

        private class TestDataManager : DataManager<TestDataType, uint> { }

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
            bool raised = false;
            dm.DataChanged += (o, e) =>
            {
                if (e.Action == NotifyDataChangedAction.Add && e.NewItems?.Cast<TestDataType>().FirstOrDefault() == data)
                {
                    raised = true;
                }
            };
            Assert.IsTrue(dm.Add(data));
            Assert.IsFalse(dm.Add(data));
            Assert.IsTrue(raised);
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
            bool raised = false;
            dm.DataChanged += (o, e) =>
            {
                if (e.Action == NotifyDataChangedAction.Remove && e.OldItems?.Cast<TestDataType>().FirstOrDefault() == data)
                {
                    raised = true;
                }
            };
            Assert.IsTrue(dm.Remove(data.Id));
            Assert.IsFalse(dm.Remove(data.Id));
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void Reset_ResetAndEventRaised()
        {
            TestDataManager dm = new TestDataManager();
            TestDataType data = new TestDataType();
            dm.Add(data);
            bool raised = false;
            dm.DataChanged += (o, e) =>
            {
                if (e.Action == NotifyDataChangedAction.Reset && e.OldItems?.Cast<TestDataType>().FirstOrDefault() == data)
                {
                    raised = true;
                }
            };
            dm.Reset();
            Assert.IsFalse(dm.GetAll().Any());
            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void Replace_ReplacedAndEventRaised()
        {
            TestDataManager dm = new TestDataManager();
            TestDataType data1 = new TestDataType { Id = 1U }, data2 = new TestDataType { Id = 2U };
            dm.Add(data1);
            bool raised = false;
            dm.DataChanged += (o, e) =>
            {
                if (e.Action == NotifyDataChangedAction.Replace
                && e.OldItems?.Cast<TestDataType>().FirstOrDefault() == data1
                && e.NewItems?.Cast<TestDataType>().FirstOrDefault() == data2)
                {
                    raised = true;
                }
            };
            Assert.AreEqual(dm.Get(1U), data1);
            dm.ReplaceData(new System.Collections.Generic.HashSet<TestDataType> { data2 });
            Assert.AreEqual(dm.Get(2U), data2);
            Assert.IsTrue(raised);
        }
    }
}
