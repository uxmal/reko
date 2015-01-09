#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core.Types;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Controls;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Gui
{
    [TestFixture]
    public class DisassemblyFormatterTests
    {
        private Program program;

        [SetUp]
        public void Setup()
        {
            program = new Program();
        }

        [Test]
        public void Df_FormatString()
        {
            var list = new List<TextSpan>();
            var df = new DisassemblyFormatter(program, list);
            df.Write("Hello");
            df.NewLine();

            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void Df_FormatAddress()
        {
            var list = new List<TextSpan>();
            var df = new DisassemblyFormatter(program, list);
            df.WriteAddress("foo", new Address(0x1234));
            df.NewLine();

            Assert.AreEqual("foo", list[0].GetText());
            Assert.IsAssignableFrom<Address>(list[0].Tag);
        }

        [Test]
        public void Df_FormatAddress_ScannedProcedure()
        {
            var list = new List<TextSpan>();
            program.Procedures.Add(new Address(0x1234), new Procedure("fn_renamed", new Frame(PrimitiveType.Word32)));
            var df = new DisassemblyFormatter(program, list);
            df.WriteAddress("foo", new Address(0x1234));
            df.NewLine();

            Assert.AreEqual("fn_renamed", list[0].GetText());
            Assert.IsAssignableFrom<Address>(list[0].Tag);
        }
    }
}
