#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System.Collections;
using System.Collections.Generic;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly WasmArchitecture arch;
        private readonly IEnumerator<WasmInstruction> dasm;
        private readonly IStorageBinder binder;
        private readonly List<RtlInstruction> instrs;
        private readonly RtlEmitter m;
        private WasmInstruction instr;
        private InstrClass iclass;

        public WasmRewriter(WasmArchitecture arch, EndianImageReader rdr, IStorageBinder binder)
        {
            this.arch = arch;
            this.dasm = new WasmDisassembler(arch, rdr).GetEnumerator();
            this.binder = binder;
            this.instr = default!;
            this.instrs = new List<RtlInstruction>();
            m = new RtlEmitter(instrs);
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = InstrClass.Linear;
                switch (instr.Mnemonic)
                {
                case Mnemonic.i32_const: Const(PrimitiveType.Word32); break;
                case Mnemonic.f32_const: Const(PrimitiveType.Real32); break;
                default: m.Invalid(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, this.iclass);
                instrs.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void Push(Expression exp)
        {
            var sp = binder.EnsureRegister(arch.StackRegister);
            m.Assign(sp, m.ISub(sp, m.Int32(8)));
            m.Assign(m.Mem(exp.DataType, sp), exp);
        }

        private void Const(DataType dt)
        {
            var tmp = binder.CreateTemporary(dt);
            m.Assign(tmp, ((ImmediateOperand)instr.Operands[0]).Value);
            Push(tmp);
        }
    }
}
