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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Machine;
using System.Diagnostics;
using Reko.Core.Types;

namespace Reko.Arch.Alpha
{
    public class AlphaDisassembler : DisassemblerBase<AlphaInstruction>
    {
#if DEBUG
        private static HashSet<uint> seen = new HashSet<uint>();
#endif
        private AlphaArchitecture arch;
        private EndianImageReader rdr;

        public AlphaDisassembler(AlphaArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override AlphaInstruction DisassembleInstruction()
        {
            var addr = rdr.Address;
            uint uInstr;
            if (!rdr.TryReadUInt32(out uInstr))
                return null;
            var op = uInstr >> 26;
            var instr = oprecs[op].Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = 4;
            return instr;
        }


        private RegisterOperand AluRegister(uint n)
        {
            return new RegisterOperand(Registers.AluRegisters[n & 0x1F]);
        }

        private AlphaInstruction Nyi(uint uInstr)
        {
#if DEBUG
            if (!seen.Contains(uInstr))
            {
                seen.Add(uInstr);
                Debug.Print(
@"        [Test]
        public void AlphaDis_{0}()
        {{
            var instr = DisassembleWord(0x{0:X8});
            Assert.AreEqual(""@@@"", instr.ToString());
        }}
", uInstr);
#endif
            }
            return new AlphaInstruction { Opcode = Opcode.invalid };
        }

        private abstract class OpRecBase
        {
            public abstract AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm);
        }

        private class MemOpRec : OpRecBase
        {
            private Opcode opcode;

            public MemOpRec(Opcode opcode)
            {
                this.opcode = opcode;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return new AlphaInstruction
                {
                    Opcode = this.opcode,
                    op1 = dasm.AluRegister(uInstr >> 21),
                    op2 = new MemoryOperand(
                        PrimitiveType.Word32,    // Dummy value
                        dasm.AluRegister(uInstr >> 16).Register,
                        (short)uInstr)
                };
            }
        }


        private class MemFnOpRec : OpRecBase
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                throw new NotImplementedException();
            }
        }

        private class BranchOpRec : OpRecBase
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                throw new NotImplementedException();
            }
        }

        private class RegOpRec : OpRecBase
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                throw new NotImplementedException();
            }
        }

        private class ImmOpRec : OpRecBase
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                throw new NotImplementedException();
            }
        }

        private class FpuOpRec : OpRecBase
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                throw new NotImplementedException();
            }
        }

        private class PalOpRec : OpRecBase
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                throw new NotImplementedException();
            }
        }

        private class NyiOpRec : OpRecBase
        {
            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                return dasm.Nyi(uInstr);
            }
        }

        private static OpRecBase[] oprecs = new OpRecBase[]
        {
            // 00
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),

            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),

            new MemOpRec(Opcode.lda),
            new MemOpRec(Opcode.ldah),
            new MemOpRec(Opcode.ldbu),
            new MemOpRec(Opcode.ldq_u),

            new MemOpRec(Opcode.ldwu),
            new MemOpRec(Opcode.stw),
            new MemOpRec(Opcode.stb),
            new MemOpRec(Opcode.stq_u),
            // 10
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),

            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),

            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),

            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            // 20
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),

            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),

            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),

            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            // 30
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),

            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),

            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),

            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
            new NyiOpRec(),
        };
    }
}