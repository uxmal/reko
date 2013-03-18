#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Decompiler.Tools.C2Xml.UnitTests
{
    [TestFixture]
    public class XmlConverterTests
    {
        void RunTest(string c_code, string expectedXml)
        {
            StringReader reader = null;
            StringWriter writer = null;
            try
            {
                reader = new StringReader(c_code);
                writer = new StringWriter();
                var xWriter = new XmlnsHidingWriter(writer)
                {
                    Formatting = Formatting.Indented
                };
                var xc = new XmlConverter(reader, xWriter);
                xc.Convert();
                writer.Flush();
                Assert.AreEqual(expectedXml, writer.ToString());
            }
            catch {
                Debug.WriteLine(writer.ToString());
                throw;
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
                if (reader != null)
                    reader.Dispose();
            }
        }

        [Test]
        public void C2X_Typedef()
        {
            var sExp =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<library xmlns=""http://schemata.jklnet.org/Decompiler"">
  <Types>
    <typedef name=""INT"">
      <prim domain=""SignedInt"" size=""4"" />
    </typedef>
  </Types>
</library>";
            RunTest("typedef int INT;", sExp);
        }
    }
}
