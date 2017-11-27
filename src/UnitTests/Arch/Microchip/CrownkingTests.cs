using Microchip.Crownking;
using Microchip.Utils;
using NUnit.Framework;
using System.Globalization;
using System.Xml.Linq;

namespace Reko.UnitTests.Arch.Microchip.Crownking
{
    namespace Microchip_Tests
    {
        [TestFixture]
        public class MicrochipPICCrowningTests
        {
            private PICCrownking db;

            [SetUp]
            public void LoadDB()
            {
                db = PICCrownking.GetDB();
                Assert.That(db, Is.Not.Null, "No accessible PIC database");
            }

            [Test]
            [Explicit]
            [Description("Update PIC local database")]
            public void Test_UpdateDB()
            {
                db.UpdateDB();
                Assert.That(db.Status, Is.EqualTo(DBStatus.DBOK), $"Update failed: status={db.Status}, error={db.LastError}");
            }

            [Test]
            [Description("List PICs names")]
            public void Test_ListPICs()
            {
                foreach (var name in db.EnumPICList())
                {
                    Assert.That(name.StartsWith("PIC1"), "Wrong PIC XML");
                }
            }

            [Test]
            [Description("List PIC16s names")]
            public void Test_ListPIC16Fs()
            {
                foreach (var name in db.EnumPICList((s) => s.StartsWith("PIC16F", true, CultureInfo.InvariantCulture)))
                {
                    Assert.That(name.StartsWith("PIC16F"), "Wrong PIC16 XML");
                }
            }

            [Test]
            [Description("List PIC18s names")]
            public void Test_ListPIC18Fs()
            {
                foreach (var name in db.EnumPICList((s) => s.StartsWith("PIC18F", true, CultureInfo.InvariantCulture)))
                {
                    Assert.That(name.StartsWith("PIC18F"), "Wrong PIC18 XML");
                }
            }

            private PIC _getPIC(string sPICName)
            {
                PIC pic = db.GetPIC(sPICName);
                Assert.That(pic, Is.Not.Null, $"Unable to get PIC object for {sPICName}.");
                Assert.That(pic.Name, Is.EqualTo(sPICName));
                return pic;
            }

            [Test]
            [Description("Decipher XML")]
            public void Test_DecipherXML()
            {
                string sPICName = "PIC16F84A";
                XElement xpic = db.GetPICAsXML(sPICName);
                Assert.That(xpic, Is.Not.Null, $"Unable to load {sPICName} XML");
                Assert.That(xpic.Name.LocalName, Is.EqualTo("PIC"));
                Assert.That(xpic.Attribute("name").Value, Is.EqualTo(sPICName));
                Assert.That(xpic.Attribute("arch").Value, Is.EqualTo("16xxxx"));
                Assert.That(xpic.GetAsBoolean("isextended"), Is.False);

                PIC pic = xpic.ToObject<PIC>();
                Assert.That(pic, Is.Not.Null, $"Unable to get PIC object for {sPICName}.");
                Assert.That(pic.Name, Is.EqualTo(sPICName));
                Assert.That(pic.Arch, Is.EqualTo("16xxxx"));
                Assert.That(pic.IsExtended, Is.False);

                sPICName = "PIC16F1825";
                pic = _getPIC(sPICName);
                Assert.That(pic.Arch, Is.EqualTo("16Exxx"));
                Assert.That(pic.IsExtended, Is.False);

                sPICName = "PIC18F25K50";
                pic = _getPIC(sPICName);
                Assert.That(pic.Arch, Is.EqualTo("18xxxx"));
                Assert.That(pic.IsExtended, Is.True);

                sPICName = "PIC16F15313";
                pic = _getPIC(sPICName);
                Assert.That(pic.Arch, Is.EqualTo("16Exxx"));
                Assert.That(pic.IsExtended, Is.False);

                sPICName = "PIC18F24K42";
                pic = _getPIC(sPICName);
                Assert.That(pic.Arch, Is.EqualTo("18xxxx"));

                xpic = pic.ToXElement();
                Assert.That(xpic, Is.Not.Null, $"Unable to generate XML object for {sPICName}");
                Assert.That(xpic.Name.LocalName, Is.EqualTo("PIC"));
                string spic = xpic.ToString();

            }

            [Test]
            [Description("Decipher whole XML definitions")]
            public void Test_WholeDecipherXML()
            {
                int count = 0;
                foreach (var name in db.EnumPICList())
                {
                    Assert.That(name.StartsWith("PIC1"), "Wrong PIC XML");
                    PIC pic = db.GetPIC(name);
                    Assert.That(pic, Is.Not.Null, $"Unable to get PIC object: {name}");
                    count++;
                }
                Assert.Pass($"{count} PICs loaded");
            }

        }
    }
}
