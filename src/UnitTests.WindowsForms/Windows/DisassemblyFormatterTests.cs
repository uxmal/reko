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

using Reko.Core;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.UserInterfaces.WindowsForms.Controls;
using Reko.UserInterfaces.WindowsForms;
using Reko.UnitTests.Mocks;

namespace Reko.UnitTests.Gui
{
    [TestFixture]
    public class DisassemblyFormatterTests
    {
        private Program program;

        [SetUp]
        public void Setup()
        {
            program = new Program
            {
                Architecture = new FakeArchitecture()
            };
        }

        [Test]
        public void Df_FormatString()
        {
            var list = new List<TextSpan>();
            var df = new DisassemblyFormatter(program, program.Architecture, null, list);
            df.WriteString("Hello");
            df.NewLine();

            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void Df_FormatAddress()
        {
            var list = new List<TextSpan>();
            var df = new DisassemblyFormatter(program, program.Architecture, null, list);
            df.WriteAddress("foo", Address.Ptr32(0x1234));
            df.NewLine();

            Assert.AreEqual("foo", list[0].GetText());
            Assert.IsInstanceOf<Address>(list[0].Tag);
        }

        [Test]
        public void Df_FormatAddress_ScannedProcedure()
        {
            var list = new List<TextSpan>();
            var addr = Address.Ptr32(0x1234);
            program.Procedures.Add(addr, new Procedure(program.Architecture, "fn_renamed", addr, new Frame(PrimitiveType.Word32)));
            var df = new DisassemblyFormatter(program, program.Architecture,  null, list);
            df.WriteAddress("foo", Address.Ptr32(0x1234));
            df.NewLine();

            Assert.AreEqual("fn_renamed", list[0].GetText());
            Assert.IsInstanceOf<Address>(list[0].Tag);
        }
    }
}
