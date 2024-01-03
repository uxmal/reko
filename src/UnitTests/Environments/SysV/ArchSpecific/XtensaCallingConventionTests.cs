#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Arch.Xtensa;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Environments.SysV.ArchSpecific;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Environments.SysV.ArchSpecific
{
    [TestFixture]
    public class XtensaCallingConventionTests
    {
        private void AssertSignature(string sExp, DataType dtRet, params DataType[] args)
        {
            var arch = new XtensaArchitecture(new ServiceContainer(), "xtensa", new Dictionary<string, object>());
            var cc = new XtensaCallingConvention(arch);
            var ccr = new CallingConventionEmitter();
            cc.Generate(ccr, 0, dtRet, null, args.ToList());
            Assert.AreEqual(sExp.Trim(), ccr.ToString());
        }

        [Test]
        public void XtensaCc_void_no_args()
        {
            AssertSignature("Stk: 0 void ()", null);
        }

        [Test]
        public void XtensaCc_returns_int_no_args()
        {
            AssertSignature("Stk: 0 a2 ()", PrimitiveType.Int32);
        }

        [Test]
        public void XtensaCc_returns_int_two_args()
        {
            AssertSignature("Stk: 0 a2 (a2, a3)",
                PrimitiveType.Int32,
                PrimitiveType.Char, PrimitiveType.Ptr32);
        }

    }
}
