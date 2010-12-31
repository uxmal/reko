#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.Gui;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Gui
{
    [TestFixture]
    public class HtmlCodeFormatterTests
    {
        private StringWriter sb;
        private HtmlCodeFormatter hcf;
        private Dictionary<Address, Procedure> map;

        [SetUp]
        public void Setup()
        {
            sb = new StringWriter();
            map = new Dictionary<Address,Procedure>();
            hcf = new HtmlCodeFormatter(sb, map);
        }

        [Test]
        public void WriteProcedureConstant()
        {
            var proc = new Procedure("proc", new Frame(PrimitiveType.Word32));
            var pc = new ProcedureConstant(PrimitiveType.Word32, proc);
            map.Add(new Address(0x42), proc);

            pc.Accept(hcf);

            Assert.AreEqual("<a href=\"00000042\">proc</a>", sb.ToString());
        }
    }
}
