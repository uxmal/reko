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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmRewriter : IEnumerable<RtlInstructionCluster>
    {
        private WasmArchitecture arch;
        private IEnumerator<WasmInstruction> dasm;
        private WasmInstruction instr;
        private Frame frame;
        private RtlEmitter m;

        public WasmRewriter(WasmArchitecture arch, LeImageReader rdr, Frame frame)
        {
            this.arch = arch;
            this.dasm = new WasmDisassembler(rdr).GetEnumerator();
            this.frame = frame;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var rtlc = new RtlInstructionCluster(instr.Address, instr.Length);
                rtlc.Class = RtlClass.Linear;
                m = new RtlEmitter(rtlc.Instructions);
                switch (instr.Opcode)
                {
                case Opcode.i32_const: Const(PrimitiveType.Word32); break;
                default: m.Invalid(); break;
                }
                yield return rtlc;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Push(Expression exp)
        {
            var sp = frame.EnsureRegister(arch.StackRegister);
            m.Assign(sp, m.ISub(sp, m.Int32(8)));
            m.Assign(m.Load(exp.DataType, sp), exp);
        }

        private void Const(DataType dt)
        {
            var tmp = frame.CreateTemporary(dt);
            m.Assign(tmp, ((ImmediateOperand)instr.Operands[0]).Value);
            Push(tmp);
        }

    }
}
