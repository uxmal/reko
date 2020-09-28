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

using Reko;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class InterProceduralJumpTests
    {
        private ProgramBuilder pb;
        private RegisterStorage freg;
        private Identifier reg;
        private Identifier SCZO;

        private void CreateIntraProceduralJumps()
        {
            var regS = new RegisterStorage("reg", 1, 0, PrimitiveType.Word32);
            pb = new ProgramBuilder();
            pb.Add("main", (m) =>
            {
                var reg = m.Frame.EnsureRegister(regS);
                m.Assign(reg, 0);
                m.MStore(m.Word32(0x1234), reg);
                m.Return();
            });
        }

        private void Test()
        {
            freg = new RegisterStorage("freg", 9, 70, PrimitiveType.Word32);
            reg = new Identifier("reg", PrimitiveType.Word32, new RegisterStorage("reg", 1, 0, PrimitiveType.Word32));
            SCZO = new Identifier("SCZO", PrimitiveType.Byte, new FlagGroupStorage(freg, 0xF, "SCZO", PrimitiveType.Byte));
            new RtlTraceBuilder
            {
                { 
                    new RtlTrace(0x1000)
                    {
                        (m) => {
                            m.Assign(reg, m.Word32(0));
                            m.Assign(SCZO, m.Cond(reg));
                        },
                        (m) => {
                            m.Assign(m.Mem(PrimitiveType.Word32, m.Word32(0x1234)), reg);
                        },
                        (m) => {
                            m.Return(0, 0);
                        }
                    }
                },
                {
                    new RtlTrace(0x1010)
                    {
                        (m) => {
                            m.Assign(reg, m.Word32(1));
                        },
                        (m) => {
                            m.Goto(Address.Ptr32(0x1004));
                        }
                    }
                },
            };
        }
    }
}
