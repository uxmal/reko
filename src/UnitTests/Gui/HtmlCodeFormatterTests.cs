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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Gui;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Reko.UnitTests.Gui
{
    [TestFixture]
    public class HtmlCodeFormatterTests
    {
        private StringWriter sb;
        private HtmlCodeFormatter hcf;
        private Mock<IProcessorArchitecture> arch;
        private Dictionary<Address, Procedure> map;

        [SetUp]
        public void Setup()
        {
            sb = new StringWriter();
            map = new Dictionary<Address,Procedure>();
            hcf = new HtmlCodeFormatter(sb, map);
            arch = new Mock<IProcessorArchitecture>();
        }

        [Test]
        public void WriteProcedureConstant()
        {
            var addr = Address.Ptr32(0x42);
            var proc = Procedure.Create(arch.Object, "proc", addr, new Frame(PrimitiveType.Word32));
            var pc = new ProcedureConstant(PrimitiveType.Word32, proc);
            map.Add(addr, proc);

            pc.Accept(hcf);

            Assert.AreEqual("<a href=\"00000042\">proc</a>", sb.ToString());
        }

        [Test]
        public void WriteProcedure_Max()
        {
            var m = new ProcedureBuilder("proc");
            var r1 = m.Register("r1");
            var r2 = m.Register("r2");
            var r3 = m.Register("r3");
            m.BranchIf(m.Gt(r1, r2), "greaterthan");
            m.Assign(r3,r2);
            m.Assign(r2, r1);
            m.Assign(r1, r3);
            m.Label("greaterthan");
            m.Return(r1);

            hcf.Write(m.Procedure);

            var sExp = @"<span class=""kw"">define</span>&nbsp;proc<br />
{<br />
proc_entry:<br />
&nbsp;&nbsp;&nbsp;&nbsp;<span class=""kw"">goto</span>&nbsp;l1<br />
greaterthan:<br />
&nbsp;&nbsp;&nbsp;&nbsp;<span class=""kw"">return</span>&nbsp;r1<br />
l1:<br />
&nbsp;&nbsp;&nbsp;&nbsp;<span class=""kw"">branch</span>&nbsp;r1&nbsp;&gt;&nbsp;r2&nbsp;greaterthan<br />
l2:<br />
&nbsp;&nbsp;&nbsp;&nbsp;r3&nbsp;=&nbsp;r2<br />
&nbsp;&nbsp;&nbsp;&nbsp;r2&nbsp;=&nbsp;r1<br />
&nbsp;&nbsp;&nbsp;&nbsp;r1&nbsp;=&nbsp;r3<br />
&nbsp;&nbsp;&nbsp;&nbsp;<span class=""kw"">goto</span>&nbsp;greaterthan<br />
proc_exit:<br />
}<br />
";
            Debug.Write(sb.ToString());
            Assert.AreEqual(sExp, sb.ToString());
        }
    }
}
