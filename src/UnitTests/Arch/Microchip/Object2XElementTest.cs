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
            public int Prop1 { get; set; }
            public bool Test;
        }

        public class WrongSerializableClass
        {
            public ArrayList List { get; set; }

            public WrongSerializableClass()
            {
                List = new ArrayList()
                {
                    1,
                    new SerializableClass()
                };
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
            XElement xmlTree = new XElement("SerializableClass", new XElement("Prop1", 1), new XElement("Test", true));
            XElement xmlTree2 = new XElement("SerializableClass", new XElement("prop2", 1), new XElement("testA", true));
            XElement xmlTree3 = new XElement("WrongSerializableClass", new XElement("propW", 1), new XElement("testW", true));
            object res;
            SerializableClass scl;

            res = xmlTree.FromXElement<SerializableClass>();
            Assert.IsNotNull(res);
            Assert.That(res, Is.InstanceOf(typeof(SerializableClass)));
            scl = res as SerializableClass;
            Assert.IsNotNull(scl);
            Assert.AreEqual(1, scl.Prop1);
            Assert.AreEqual(true, scl.Test);

            res = xmlTree2.FromXElement<SerializableClass>();
            Assert.IsNotNull(res);
            Assert.That(res, Is.InstanceOf(typeof(SerializableClass)));
            scl = res as SerializableClass;
            Assert.IsNotNull(scl);
            Assert.AreEqual(0, scl.Prop1);
            Assert.AreEqual(false, scl.Test);

            res = xmlTree3.FromXElement<SerializableClass>();
            Assert.IsNull(res);
        }

    }
}
