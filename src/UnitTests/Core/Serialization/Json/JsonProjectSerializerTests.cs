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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Serialization.Json;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class JsonProjectSerializerTests
    {
        private void RunTest(string sExp, Project project)
        {
            var ser = new JsonProjectSerializer();
            var sw = new StringWriter();
            ser.Serialize(project, sw);
            var sActual = sw.ToString().Replace('\"', '\'');
            if (sExp != sActual)
            {
                Debug.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        [Test]
        public void Jprjs_Programs()
        {
            var arch = new FakeArchitecture();
            var proc1 = new Procedure(arch, "fn00123400", Address.Ptr32(0x00123400), new Frame(PrimitiveType.Ptr32));
            var proc2 = new Procedure(arch, "fn00123500", Address.Ptr32(0x00123500), new Frame(PrimitiveType.Ptr32));
            var project = new Project
            {
                Programs =
                {
                    new Program
                    {
                        Name = "prog1.exe",
                        Procedures =
                        {
                            { Address.Ptr32(0x00123400), proc1 },
                            { Address.Ptr32(0x00123500), proc2 },
                        }
                    }
                }
            };

            var sExp =
                "{" +
                    "'programs':[" +
                        "{'name':'prog1.exe'," +
                         "'procedures':[" +
                            "{'address':'00123400','name':'fn00123400'}," +
                            "{'address':'00123500','name':'fn00123500'}" +
                        "]}" +
                    "]" +
                "}";

            RunTest(sExp, project);
        }
    }
}
