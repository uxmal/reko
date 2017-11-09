#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Types;
using Reko.Environments.MacOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Environments.MacOS
{
    [TestFixture]
    public class StackBasedConventionTests
    {
        private M68kArchitecture arch;
        private CallingConventionEmitter emitter;

        public StackBasedConventionTests()
        {
            this.arch = new M68kArchitecture();
        }

        [SetUp]
        public void Setup()
        {
            this.emitter = new CallingConventionEmitter();
        }

        [Test]
        public void Sbcc_VoidFn()
        {
            var sbcc = new StackBasedConvention(arch);
            sbcc.Generate(emitter, VoidType.Instance, null, new List<DataType> { PrimitiveType.Word32 });
            Assert.AreEqual("Stk: 0 void (Stack +0004)", emitter.ToString());
        }
    }
}
