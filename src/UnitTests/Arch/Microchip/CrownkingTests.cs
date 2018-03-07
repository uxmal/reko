#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Libraries.Microchip;
using NUnit.Framework;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Reko.UnitTests.Arch.Microchip.Crownking
{
    [TestFixture]
    public class MicrochipPICCrowningTests
    {
        private PICCrownking db;

        [SetUp]
        public void LoadDB()
        {
            db = PICCrownking.GetDB();
            Assert.That(db, Is.Not.Null, $"No accessible PIC XML database - {PICCrownking.LastErrMsg}.");
        }

        [Test]
        [Description("List PICs names")]
        public void ListPICs_Tests()
        {
            foreach (var name in db.EnumPICList())
            {
                Assert.That(name.StartsWith("PIC1"), "Wrong PIC XML");
            }
        }

        [Test]
        [Description("List PIC16s names")]
        public void ListPIC16Fs_Tests()
        {
            foreach (var name in db.EnumPICList((s) => s.StartsWith("PIC16F", true, CultureInfo.InvariantCulture)))
            {
                Assert.That(name.StartsWith("PIC16F"), "Wrong PIC16 XML");
            }
        }

        [Test]
        [Description("List PIC18s names")]
        public void ListPIC18Fs_Tests()
        {
            foreach (var name in db.EnumPICList((s) => s.StartsWith("PIC18F", true, CultureInfo.InvariantCulture)))
            {
                Assert.That(name.StartsWith("PIC18F"), "Wrong PIC18 XML");
            }
        }

        private PIC _getPIC(string sPICName)
        {
            PIC pic = db.GetPIC(sPICName);
            Assert.That(pic, Is.Not.Null, $"Unable to get PIC object for '{sPICName}' - {PICCrownking.LastErrMsg}.");
            Assert.That(pic.Name, Is.EqualTo(sPICName));
            return pic;
        }

        [Test]
        [Description("Decipher XML")]
        public void DecipherXML_Tests()
        {
            string sPICName = "PIC16F84A";
            XElement xpic = db.GetPICAsXML(sPICName);
            Assert.That(xpic, Is.Not.Null, $"Unable to load '{sPICName}' XML - {PICCrownking.LastErrMsg}.");
            Assert.That(xpic.Name.LocalName, Is.EqualTo("PIC"));
            Assert.That(xpic.Attribute("name").Value, Is.EqualTo(sPICName));
            Assert.That(xpic.Attribute("arch").Value, Is.EqualTo("16xxxx"));
            Assert.That(xpic.GetAsBoolean("isextended"), Is.False);

            PIC pic = xpic.ToObject<PIC>();
            Assert.That(pic, Is.Not.Null, $"Unable to get PIC object for '{sPICName}' - {PICCrownking.LastErrMsg}..");
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
        public void WholeDecipherXML_Tests()
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

#if DEBUG

        [Test]
        [Explicit("For debugging purpose only")]
        [Ignore("For debugging purpose only")]
        public void PruneXML_Tests()
        {
            foreach (string sPICName in new string[] { "PIC18F25K50", "PIC16F84A", "PIC16F1825",  "PIC16F15313", "PIC18F24K42" })
            {
                PIC pic = db.GetPICAsXML(sPICName).ToObject<PIC>();
                XmlSerializer xs = new XmlSerializer(typeof(PIC));
                using (StreamWriter str = new StreamWriter(sPICName + ".XML"))
                {
                    xs.Serialize(str, pic);
                }
            }
        }

        [Test]
        [Explicit("For debugging purpose only")]
        [Description("Dump PIC local database to XML files. Takes some time.")]
        [Ignore("For debugging purpose only")]
        public void PICDB2XML_Tests()
        {
            const string xmlsdir = "XMLs";  // local folder where XMLS files are written to.

            if (!Directory.Exists(xmlsdir))
                Directory.CreateDirectory(xmlsdir);

            foreach (var name in db.EnumPICList())
            {
                Assert.That(name.StartsWith("PIC1"), "Wrong PIC XML");
                PIC pic = db.GetPIC(name);
                Assert.That(pic, Is.Not.Null, $"Unable to get PIC object: {name}");
                XmlSerializer xs = new XmlSerializer(typeof(PIC));
                string xmlfpath = Path.Combine(xmlsdir, name + ".XML");
                using (StreamWriter sw = new StreamWriter(xmlfpath, false))
                {
                    xs.Serialize(sw, pic);
                }
            }
        }


#endif

    }
}
