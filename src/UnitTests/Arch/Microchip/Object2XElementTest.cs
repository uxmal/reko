using NUnit.Framework;
using System;
using System.Collections;
using System.Xml.Linq;

namespace Microchip.Utils
{
    [TestFixture]
    public class Object2XElementTest
    {

        [Serializable]
        public class SerializableClass
        {
            public int prop1 { get; set; }
            public bool test;
        }

        public class WrongSerializableClass
        {
            public ArrayList list { get; set; }

            public WrongSerializableClass()
            {
                list = new ArrayList();
                list.Add(1);
                list.Add(new SerializableClass());
            }
        }


        [Test]
        public void ToXElement_Test()
        {
            XElement res;
            WrongSerializableClass empty = new WrongSerializableClass();
            SerializableClass scl = new SerializableClass();

            res = empty.ToXElement();
            Assert.IsNull(res);

            res = scl.ToXElement();
            Assert.IsNotNull(res);
            Assert.IsFalse(res.IsEmpty);
            Assert.IsTrue(res.HasElements);

        }

        [Test]
        public void FromXElement_Test()
        {
            XElement xmlTree = new XElement("SerializableClass", new XElement("prop1", 1), new XElement("test", true));
            XElement xmlTree2 = new XElement("SerializableClass", new XElement("prop2", 1), new XElement("testA", true));
            XElement xmlTree3 = new XElement("WrongSerializableClass", new XElement("propW", 1), new XElement("testW", true));
            object res;
            SerializableClass scl;

            res = xmlTree.FromXElement<SerializableClass>();
            Assert.IsNotNull(res);
            Assert.IsInstanceOfType(typeof(SerializableClass), res);
            scl = res as SerializableClass;
            Assert.IsNotNull(scl);
            Assert.AreEqual(1, scl.prop1);
            Assert.AreEqual(true, scl.test);

            res = xmlTree2.FromXElement<SerializableClass>();
            Assert.IsNotNull(res);
            Assert.IsInstanceOfType(typeof(SerializableClass),res);
            scl = res as SerializableClass;
            Assert.IsNotNull(scl);
            Assert.AreEqual(0, scl.prop1);
            Assert.AreEqual(false, scl.test);

            res = xmlTree3.FromXElement<SerializableClass>();
            Assert.IsNull(res);
        }

    }
}
