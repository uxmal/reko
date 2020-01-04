#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.UnitTests.Core.Serialization
{
    [TestFixture]
    public class UnpackerSignatureFileTests
    {
        [Test]
        public void Usf_Load()
        {
            var file =
@"<?xml version=""1.0""?>
<SIGNATURES>
  <ENTRY>
    <NAME>!EP (ExE Pack) V1.0 -&gt; Elite Coding Group</NAME>
    <COMMENTS />
    <ENTRYPOINT>6068????????B8????????FF10</ENTRYPOINT>
    <ENTIREPE />
  </ENTRY>
  </SIGNATURES>";
            var rdr = new XmlTextReader(new StringReader(file));
            var serializer = new XmlSerializer(typeof(UnpackerSignatureFile_v1));
            var usf = (UnpackerSignatureFile_v1) serializer.Deserialize(rdr);
            Assert.AreEqual(1, usf.Signatures.Length);
            Assert.AreEqual("!EP (ExE Pack) V1.0 -> Elite Coding Group", usf.Signatures[0].Name);
            Assert.AreEqual("6068????????B8????????FF10", usf.Signatures[0].EntryPoint);
        }
    }
}
