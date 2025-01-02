#region License
/* 
 *
 * Copyrighted (c) 2017-2025 Christian Hostelet.
 *
 * The contents of this file are subject to the terms of the Common Development
 * and Distribution License (the License), or the GPL v2, or (at your option)
 * any later version. 
 * You may not use this file except in compliance with the License.
 *
 * You can obtain a copy of the License at http://www.netbeans.org/cddl.html
 * or http://www.gnu.org/licenses/gpl-2.0.html.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *
 * When distributing Covered Code, include this CDDL Header Notice in each file
 * and include the License file at http://www.netbeans.org/cddl.txt.
 * If applicable, add the following below the CDDL Header, with the fields
 * enclosed by brackets [] replaced by your own identifying information:
 * "Portions Copyrighted (c) [year] [name of copyright owner]"
 *
 */
#endregion

using Reko.Libraries.Microchip;
using Reko.Libraries.Microchip.V1;
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

        private IPICDescriptor getPIC(string sPICName)
        {
            var pic = db.GetPIC(sPICName);
            Assert.That(pic, Is.Not.Null, $"Unable to get PIC descriptor for '{sPICName}' - {PICCrownking.LastErrMsg}.");
            Assert.That(pic.PICName, Is.EqualTo(sPICName));
            return pic;
        }

        private IPICDescriptor getPIC(int iProcID)
        {
            var pic = db.GetPIC(iProcID);
            Assert.That(pic, Is.Not.Null, $"Unable to get PIC descriptor for '0x{iProcID:X}' - {PICCrownking.LastErrMsg}.");
            Assert.That(pic.PICName, Is.EqualTo(iProcID));
            return pic;
        }

        [Test]
        [Description("Decipher XML version 1")]
        public void DecipherXML_V1_Tests()
        {
            var sPICName = "PIC16F84A";
            XElement xpic = db.GetPICAsXML(sPICName);
            Assert.That(xpic, Is.Not.Null, $"Unable to load '{sPICName}' XML - {PICCrownking.LastErrMsg}.");
            Assert.That(xpic.Name.LocalName, Is.EqualTo("PIC"));
            Assert.That(xpic.Attribute("name").Value, Is.EqualTo(sPICName));
            Assert.That(xpic.Attribute("arch").Value, Is.EqualTo("16xxxx"));
            Assert.That(xpic.GetAsBoolean("isextended"), Is.False);
            var procID = 0x6F84;
            xpic = db.GetPICAsXML(procID);
            Assert.That(xpic, Is.Not.Null, $"Unable to load '0x{procID:X}' XML - {PICCrownking.LastErrMsg}.");
            Assert.That(xpic.Name.LocalName, Is.EqualTo("PIC"));

            var pic_v1 = xpic.ToObject<PIC_v1>();
            Assert.That(pic_v1, Is.Not.Null, $"Unable to get PIC descriptor for '{sPICName}' - {PICCrownking.LastErrMsg}..");
            Assert.That(pic_v1.Name, Is.EqualTo(sPICName));
            Assert.That(pic_v1.Arch, Is.EqualTo("16xxxx"));
            Assert.That(pic_v1.ProcID, Is.EqualTo(procID));
            Assert.That(pic_v1.HasExtendedMode, Is.False);
        }

        [Test]
        public void GetPIC_Tests()
        {

            var sPICName = "PIC16F1825";
            var pic = getPIC(sPICName);
            Assert.That(pic.ArchName, Is.EqualTo("16Exxx"));
            Assert.That(pic.HasExtendedMode, Is.False);

            sPICName = "PIC18F25K50";
            pic = getPIC(sPICName);
            Assert.That(pic.ArchName, Is.EqualTo("18xxxx"));
            Assert.That(pic.HasExtendedMode, Is.True);

            sPICName = "PIC16F15313";
            pic = getPIC(sPICName);
            Assert.That(pic.ArchName, Is.EqualTo("16Exxx"));
            Assert.That(pic.HasExtendedMode, Is.False);

            sPICName = "PIC18F24K42";
            pic = getPIC(sPICName);
            Assert.That(pic.ArchName, Is.EqualTo("18xxxx"));

        }

        [Test]
        [Description("Decipher whole XML definitions")]
        [Category(Categories.IntegrationTests)]
        public void WholeDecipherXML_Tests()
        {
            var count = 0;
            foreach (var name in db.EnumPICList())
            {
                Assert.That(name.StartsWith("PIC1"), "Wrong PIC XML");
                var pic = db.GetPIC(name);
                Assert.That(pic, Is.Not.Null, $"Unable to get PIC definition for: '{name}'");
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
            foreach (var sPICName in new string[] { "PIC18F25K50", "PIC16F84A", "PIC16F1825",  "PIC16F15313", "PIC18F24K42" })
            {
                var pic = db.GetPICAsXML(sPICName).ToObject<PIC_v1>();
                var xs = new XmlSerializer(pic.GetType());
                using (var str = new StreamWriter(sPICName + ".XML"))
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
                var pic = db.GetPIC(name);
                Assert.That(pic, Is.Not.Null, $"Unable to get PIC descriptor for '{name}'");
                var xs = new XmlSerializer(pic.GetType());
                var xmlfpath = Path.Combine(xmlsdir, name + ".XML");
                using (var sw = new StreamWriter(xmlfpath, false))
                {
                    xs.Serialize(sw, pic);
                }
            }
        }


#endif

    }
}
