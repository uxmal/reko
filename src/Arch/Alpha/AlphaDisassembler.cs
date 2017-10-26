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
            var instr = Oprecs[op].Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = 4;
            return instr;
        }

        private RegisterOperand AluRegister(uint n)
        {
            return new RegisterOperand(Registers.AluRegisters[n & 0x1F]);
        }
        private AlphaInstruction Invalid()
        {
            return new AlphaInstruction { Opcode = Opcode.invalid };
        }

        private AlphaInstruction Nyi(uint uInstr)
        {
#if DEBUG
            if (!seen.Contains(uInstr))
            {
                seen.Add(uInstr);
                Debug.Print(
@"        [Test]
        public void AlphaDis_{0:X8}()
        {{
            var instr = DisassembleWord(0x{0:X8});
            Assert.AreEqual(""@@@"", instr.ToString());
        }}
", uInstr);
#endif
            }
            return Invalid();
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
            private Opcode opcode;

            public BranchOpRec(Opcode opcode)
            {
                this.opcode = opcode;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                int offset = ((int)uInstr << 11) >> 9;
                var op1 = dasm.AluRegister(uInstr >> 21);
                var op2 = AddressOperand.Create(dasm.rdr.Address + offset);
                return new AlphaInstruction
                {
                    Opcode = this.opcode,
                    op1 = op1,
                    op2 = op2,
                };
            }
        }

        private class RegOpRec : OpRecBase
        {
            private Opcode opcode;

            public RegOpRec(Opcode opcode)
            {
                this.opcode = opcode;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                var op1 = dasm.AluRegister(uInstr >> 21);
                var op2 = (uInstr & (1 << 12)) != 0
                    ? ImmediateOperand.Byte((byte)(uInstr >> 13))
                    : (MachineOperand) dasm.AluRegister(uInstr >> 13);
                var op3 = dasm.AluRegister(uInstr);
                return new AlphaInstruction
                {
                    Opcode = this.opcode,
                    op1 = op1,
                    op2 = op2,
                    op3 = op3,
                };
            }
        }

        private class OperateOpRec : OpRecBase
        {
            private Dictionary<int, RegOpRec> decoders;

            public OperateOpRec(Dictionary<int, RegOpRec> decoders)
            {
                this.decoders = decoders;
            }

            public override AlphaInstruction Decode(uint uInstr, AlphaDisassembler dasm)
            {
                RegOpRec decoder;
                var functionCode = ((int)uInstr >> 5) & 0x7F;
                if (!decoders.TryGetValue(functionCode, out decoder))
                    return dasm.Invalid();
                else
                    return decoder.Decode(uInstr, dasm);
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

        private static OpRecBase[] oprecs = new OpRecBase[64]
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
            new OperateOpRec(new Dictionary<int, RegOpRec>
            {
                { 0x00, new RegOpRec(Opcode.addl) },
                { 0x02, new RegOpRec(Opcode.s4addl) },
                { 0x09, new RegOpRec(Opcode.subl) },
                { 0x0B, new RegOpRec(Opcode.s4subl) },
                { 0x0F, new RegOpRec(Opcode.cmpbge) },
                { 0x12, new RegOpRec(Opcode.s8addl) },
                { 0x1B, new RegOpRec(Opcode.s8subl) },
                { 0x1D, new RegOpRec(Opcode.cmpult) },
                { 0x20, new RegOpRec(Opcode.addq) },
                { 0x22, new RegOpRec(Opcode.s4addq) },
                { 0x29, new RegOpRec(Opcode.subq) },
                { 0x2B, new RegOpRec(Opcode.s4subq) },
                { 0x2D, new RegOpRec(Opcode.cmpeq) },
                { 0x32, new RegOpRec(Opcode.s8addq) },
                { 0x3B, new RegOpRec(Opcode.s8subq) },
                { 0x3D, new RegOpRec(Opcode.cmpule) },
                { 0x40, new RegOpRec(Opcode.addl_v) },
                { 0x49, new RegOpRec(Opcode.subl_v) },
                { 0x4D, new RegOpRec(Opcode.cmplt) },
                { 0x60, new RegOpRec(Opcode.addq_v) },
                { 0x69, new RegOpRec(Opcode.subq_v) },
                { 0x6D, new RegOpRec(Opcode.cmple) },
            }),
            new OperateOpRec(new Dictionary<int, RegOpRec>
            {
                { 0x00, new RegOpRec(Opcode.and) },
                { 0x08, new RegOpRec(Opcode.bic) },
                { 0x14, new RegOpRec(Opcode.cmovlbs) },
                { 0x16, new RegOpRec(Opcode.cmovlbc) },
                { 0x20, new RegOpRec(Opcode.bis) },
                { 0x24, new RegOpRec(Opcode.cmovne) },
                { 0x26, new RegOpRec(Opcode.cmovne) },
                { 0x28, new RegOpRec(Opcode.ornot) },
                { 0x40, new RegOpRec(Opcode.xor) },
                { 0x44, new RegOpRec(Opcode.cmovlt) },
                { 0x46, new RegOpRec(Opcode.cmovge) },
                { 0x66, new RegOpRec(Opcode.cmovgt) },
                { 0x6C, new RegOpRec(Opcode.implver) },
            }),
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

            new MemOpRec(Opcode.ldl),
            new MemOpRec(Opcode.ldq),
            new MemOpRec(Opcode.ldl_l),
            new MemOpRec(Opcode.ldq_l),

            new MemOpRec(Opcode.stl),
            new MemOpRec(Opcode.stq),
            new MemOpRec(Opcode.stl_c),
            new MemOpRec(Opcode.stq_c),
            // 30
            new BranchOpRec(Opcode.br),
            new BranchOpRec(Opcode.fbeq),
            new BranchOpRec(Opcode.fblt),
            new BranchOpRec(Opcode.fble),

            new BranchOpRec(Opcode.bsr),
            new BranchOpRec(Opcode.fbne),
            new BranchOpRec(Opcode.fbge),
            new BranchOpRec(Opcode.fbgt),

            new BranchOpRec(Opcode.blbc),
            new BranchOpRec(Opcode.beq),
            new BranchOpRec(Opcode.blt),
            new BranchOpRec(Opcode.ble),

            new BranchOpRec(Opcode.blbs),
            new BranchOpRec(Opcode.bne),
            new BranchOpRec(Opcode.bge),
            new BranchOpRec(Opcode.bgt),
        };

        private static OpRecBase[] Oprecs
        {
            get
            {
                return oprecs;
            }

            set
            {
                oprecs = value;
            }
        }
    }
}