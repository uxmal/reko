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

using Reko.Environments.AmigaOS;
using Reko.Core.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Reko.UnitTests.Environments.AmigaOS
{
    [TestFixture]
    public class FuncsFileParserTests
    {
        private Reko.Arch.M68k.M68kArchitecture arch;

        public FuncsFileParserTests()
        {
            this.arch = new Reko.Arch.M68k.M68kArchitecture("m68k");
        }

        private FuncsFileParser CreateParser(string file)
        {
            return new FuncsFileParser(arch, new StringReader(file));
        }

        [Test]
        public void FuncsEmpty()
        {
            var file = "";
            var ffp = CreateParser(file);
            ffp.Parse();
            Assert.AreEqual(0, ffp.FunctionsByLibBaseOffset.Count);
        }

        [Test]
        public void FuncsSingleLine()
        {
            var file = "#0107    666  0x029a                   CreateMsgPort ()\n";
            var ffp = CreateParser(file);
            ffp.Parse();
            Assert.AreEqual(1, ffp.FunctionsByLibBaseOffset.Count);
            var func = ffp.FunctionsByLibBaseOffset[-666];
            Assert.AreEqual(-666, func.Offset);
            Assert.AreEqual("CreateMsgPort", func.Name);
            Assert.AreEqual(0, func.Signature.Arguments.Length);
        }

        [Test]
        public void FuncsSingleLineWithArgs()
        {
            var file = "#0088    552  0x0228                     OpenLibrary ( libName/a1, version/d0 )";
            var ffp = CreateParser(file);
            ffp.Parse();
            Assert.AreEqual(1, ffp.FunctionsByLibBaseOffset.Count);
            var func = ffp.FunctionsByLibBaseOffset[-552];
            Assert.AreEqual(-552, func.Offset);
            Assert.AreEqual("OpenLibrary", func.Name);
            Assert.AreEqual(2, func.Signature.Arguments.Length);
            var p0 = func.Signature.Arguments[0];
            var p1 = func.Signature.Arguments[1];
            Assert.AreEqual("libName", p0.Name);
            Assert.AreEqual("version", p1.Name);
            Assert.AreEqual("a1", ((Register_v1) p0.Kind).Name);
            Assert.AreEqual("d0", ((Register_v1) p1.Kind).Name);
        }

        [Test]
        public void FuncsExtReturnRegister_d0()
        {
            var file = "#0004 4   4  Frobnitz ( argIn / a1; ret/d0 )";
            var ffp = CreateParser(file);
            ffp.Parse();
            Assert.AreEqual(1, ffp.FunctionsByLibBaseOffset.Count);
            var func = ffp.FunctionsByLibBaseOffset[-4];
            Assert.IsNotNull(func.Signature.ReturnValue);
            Assert.AreEqual("d0", ((Register_v1) func.Signature.ReturnValue.Kind).Name);
        }
    }
}
