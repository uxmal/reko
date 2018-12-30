#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Reko.Core;
using Reko.Core.Expressions;

namespace Reko.Arch.RiscV
{
    public class RiscVDisassembler : DisassemblerBase<RiscVInstruction>
    {
        private static OpRec[] opRecs;
        private static OpRec[] wideOpRecs;
        private static OpRec[] compressed0;
        private static OpRec[] compressed1;
        private static OpRec[] compressed2;

        private RiscVArchitecture arch;
        private EndianImageReader rdr;
        private Address addrInstr;

        public RiscVDisassembler(RiscVArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override RiscVInstruction DisassembleInstruction()
        {
            this.addrInstr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out ushort hInstr))
            {
                return null;
            }
            var instr = opRecs[hInstr & 0x3].Decode(this, hInstr);
            instr.Address = addrInstr;
            instr.Length = (int) (rdr.Address - addrInstr);
            instr.iclass |= hInstr == 0 ? InstrClass.Zero : 0;
            return instr;
        }

        private RiscVInstruction DecodeCompressedOperands(Opcode opcode, InstrClass iclass, string fmt, uint wInstr)
        {
            var ops = new List<MachineOperand>();
            for (int i = 0; i < fmt.Length; ++i)
            {
                MachineOperand op;
                switch (fmt[i++])
                {
                default: throw new InvalidOperationException(string.Format("Unsupported operand code {0}", fmt[i - 1]));
                case ',': continue;
                case 'd': op = GetRegister(wInstr, 7); break;
                case 'I': op = GetImmediate(wInstr, 12, fmt[i++]); break;
                }
                ops.Add(op);
            }
            return BuildInstruction(opcode, iclass, ops);
        }

        private RiscVInstruction BuildInstruction(Opcode opcode, InstrClass iclass, List<MachineOperand> ops)
        { 
            var instr = new RiscVInstruction
            {
                Address = this.addrInstr,
                opcode = opcode,
                iclass = iclass,
                Length = (int)(this.rdr.Address - addrInstr)
            };
            if (ops.Count > 0)
            {
                instr.op1 = ops[0];
                if (ops.Count > 1)
                {
                    instr.op2 = ops[1];
                    if (ops.Count > 2)
                    {
                        instr.op3 = ops[2];

                        if (ops.Count > 3)
                        {
                            instr.op4 = ops[3];
                        }
                    }
                }
            }
            return instr;
        }

        private RiscVInstruction DecodeWideOperands(Opcode opcode, InstrClass iclass, string fmt, uint wInstr)
        {
            var ops = new List<MachineOperand>();
            for (int i = 0; i < fmt.Length; ++i)
            {
                MachineOperand op;
                switch (fmt[i++])
                {
                default: throw new InvalidOperationException(string.Format("Unsupported operand code {0}", fmt[i - 1]));
                case ',': continue;
                case '1': op = GetRegister(wInstr, 15); break;
                case '2': op = GetRegister(wInstr, 20); break;
                case 'd': op = GetRegister(wInstr, 7); break;
                case 'i': op = GetImmediate(wInstr, 20, 's'); break;
                case 'B': op = GetBranchTarget(wInstr); break;
                case 'F': op = GetFpuRegister(wInstr, fmt[i++]); break;
                case 'J': op = GetJumpTarget(wInstr); break;
                case 'I': op = GetImmediate(wInstr, 12, fmt[i++]); break;
                case 'S': op = GetSImmediate(wInstr); break;
                case 'L': // signed offset used in loads
                    op = GetImmediate(wInstr, 20, 's');
                    break;
                case 'z': op = GetShiftAmount(wInstr, 5); break;
                case 'Z': op = GetShiftAmount(wInstr, 6); break;
                }
                ops.Add(op);
            }
            return BuildInstruction(opcode, iclass, ops);
        }

        private RegisterOperand GetRegister(uint wInstr, int bitPos)
        {
            var reg = arch.GetRegister((int)(wInstr >> bitPos) & 0x1F);
            return new RegisterOperand(reg);
        }

        private RegisterOperand GetFpuRegister(uint wInstr, char bitPos)
        {
            int pos;
            switch (bitPos)
            {
            case '1': pos = 15; break;
            case '2': pos = 20; break;
            case '3': pos = 27; break;
            case 'd': pos = 7; break;
            default: throw new InvalidOperationException();
            }
            var reg = arch.GetRegister(32 + ((int)(wInstr >> pos) & 0x1F));
            return new RegisterOperand(reg);
        }

        private ImmediateOperand GetImmediate(uint wInstr, int bitPos, char sign)
        {
            if (sign == 's')
            {
                int s = ((int)wInstr) >> bitPos;
                return ImmediateOperand.Int32(s);
            }
            else
            {
                uint u = wInstr >> bitPos;
                return ImmediateOperand.Word32(u);
            }
        }

        private ImmediateOperand GetShiftAmount(uint wInstr, int length)
        {
            return ImmediateOperand.UInt32(extract32(wInstr, 20, length));
        }

        private static bool bit(uint wInstr, int bitNo)
        {
            return (wInstr & (1u << bitNo)) != 0;
        }

        private static uint extract32(uint wInstr, int start, int length)
        {
            uint n = (wInstr >> start) & (~0U >> (32 - length));
            return n;
        }

        private static ulong sextract64(ulong value, int start, int length)
        {
            long n = ((long)(value << (64 - length - start))) >> (64 - length);
            return (ulong)n;
        }

        private AddressOperand GetBranchTarget(uint wInstr)
        { 
            long offset = (long)
                  ((extract32(wInstr, 8, 4) << 1)
                | (extract32(wInstr, 25, 6) << 5)
                | (extract32(wInstr, 7, 1) << 11)
                | (sextract64(wInstr, 31, 1) << 12));
            return AddressOperand.Create(addrInstr + offset);
        }

        private AddressOperand GetJumpTarget(uint wInstr)
        {
            long offset = (long)
                  ((extract32(wInstr, 21, 10) << 1)
                | (extract32(wInstr, 20, 1) << 11)
                | (extract32(wInstr, 12, 8) << 12)
                | (sextract64(wInstr, 31, 1) << 20));
            return AddressOperand.Create(addrInstr + offset);
        }

        private ImmediateOperand GetSImmediate(uint wInstr)
        {
            var offset = (int)
                   (extract32(wInstr, 7, 5)
                 | (extract32(wInstr, 25, 7) << 5));
            return ImmediateOperand.Int32(offset);
        }

        public abstract class OpRec
        {
            public abstract RiscVInstruction Decode(RiscVDisassembler dasm, uint hInstr);
        }

        public class COpRec : OpRec
        {
            private Opcode opcode;
            private string fmt;

            public COpRec(Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint wInstr)
            {
                return dasm.DecodeCompressedOperands(opcode, InstrClass.Linear, fmt, wInstr);
            }
        }


        public class WOpRec : OpRec
        {
            private Opcode opcode;
            private InstrClass iclass;
            private string fmt;

            public WOpRec(Opcode opcode, string fmt) : this(opcode, InstrClass.Linear, fmt)
            {
            }

            public WOpRec(Opcode opcode, InstrClass iclass, string fmt)
            {
                this.opcode = opcode;
                this.iclass = iclass;
                this.fmt = fmt;
            }


            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint wInstr)
            {
                return dasm.DecodeWideOperands(opcode, iclass, fmt, wInstr);
            }
        }

        public class FpuOpRec : OpRec
        {
            private string fmt;
            private Opcode opcode;

            public FpuOpRec(Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint wInstr)
            {
                return dasm.DecodeWideOperands(opcode, InstrClass.Linear, fmt, wInstr);
            }
        }

        public class MaskOpRec : OpRec
        {
            private readonly int mask;
            private readonly int shift;
            private readonly OpRec[] subcodes;

            public MaskOpRec(int shift, int mask, OpRec[] subcodes)
            {
                this.mask = mask;
                this.shift = shift;
                this.subcodes = subcodes;
            }

            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint wInstr)
            {
                var slot = (wInstr >> shift) & mask;
                return subcodes[slot].Decode(dasm, wInstr);
            }
        }

        public class SparseMaskOpRec : OpRec
        {
            private readonly int mask;
            private readonly int shift;
            private readonly Dictionary<int, OpRec> subcodes;

            public SparseMaskOpRec(int shift, int mask, Dictionary<int, OpRec> subcodes)
            {
                this.mask = mask;
                this.shift = shift;
                this.subcodes = subcodes;
            }

            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint wInstr)
            {
                var slot = (int)((wInstr >> shift) & mask);
                OpRec oprec;
                if (!subcodes.TryGetValue(slot, out oprec))
                {
                    return new RiscVInstruction
                    {
                        Address = dasm.addrInstr,
                        opcode = Opcode.invalid
                    };
                }
                return oprec.Decode(dasm, wInstr);
            }
        }

        public class WideOpRec : OpRec
        {
            public WideOpRec()
            {
            }

            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint hInstr)
            {
                ushort hiword;
                if (!dasm.rdr.TryReadUInt16(out hiword))
                {
                    return new RiscVInstruction { opcode = Opcode.invalid, Address = dasm.addrInstr };
                }
                uint wInstr = (uint)hiword << 16;
                wInstr |= hInstr;
                var slot = (wInstr >> 2) & 0x1F;
                return wideOpRecs[slot].Decode(dasm, wInstr);
            }
        }

        public class ShiftOpRec : OpRec
        {
            private Opcode[] rl_ra;
            private string fmt;

            public ShiftOpRec(string fmt, params Opcode[] rl_ra)
            {
                this.rl_ra = rl_ra;
                this.fmt = fmt;
            }

            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint wInstr)
            {
                var opcode = rl_ra[bit(wInstr, 30) ? 1 : 0];
                return dasm.DecodeWideOperands(opcode, InstrClass.Linear, fmt, wInstr);
            }
        }


        static RiscVDisassembler()
        {
            var loads = new OpRec[]
            {
                new WOpRec(Opcode.lb, "d,1,Ls"),
                new WOpRec(Opcode.lh, "d,1,Ls"),
                new WOpRec(Opcode.lw, "d,1,Ls"),
                new WOpRec(Opcode.ld, "d,1,Ls"),

                new WOpRec(Opcode.lbu, "d,1,Ls"),
                new WOpRec(Opcode.lhu, "d,1,Ls"),
                new WOpRec(Opcode.lwu, "d,1,Ls"),
                new WOpRec(Opcode.invalid, ""),
            };

            var fploads = new OpRec[]
            {
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.flw, "Fd,1,Ls"),
                new WOpRec(Opcode.invalid, ""),

                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
            };

            var stores = new OpRec[]
            {
                new WOpRec(Opcode.sb, "2,1,Ss"),
                new WOpRec(Opcode.sh, "2,1,Ss"),
                new WOpRec(Opcode.sw, "2,1,Ss"),
                new WOpRec(Opcode.sd, "2,1,Ss"),

                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
            };

            var op = new OpRec[]
            {
                new ShiftOpRec( "d,1,2", Opcode.add, Opcode.sub),
                new WOpRec(Opcode.sll, "d,1,2"),
                new WOpRec(Opcode.slt, "d,1,2"),
                new WOpRec(Opcode.sltu, "d,1,2"),

                new WOpRec(Opcode.xor, "d,1,2"),
                new ShiftOpRec("d,1,2", Opcode.srl, Opcode.sra),
                new WOpRec(Opcode.or, "d,1,2"),
                new WOpRec(Opcode.and, "d,1,2"),
            };

            var opimm = new OpRec[]
            {
                new WOpRec(Opcode.addi, "d,1,i"),
                new ShiftOpRec("d,1,z", Opcode.slli, Opcode.invalid),
                new WOpRec(Opcode.slti, "d,1,i"),
                new WOpRec(Opcode.sltiu, "d,1,i"),

                new WOpRec(Opcode.xori, "d,1,i"),
                new ShiftOpRec("d,1,z", Opcode.srli, Opcode.srai),
                new WOpRec(Opcode.ori, "d,1,i"),
                new WOpRec(Opcode.andi, "d,1,i"),
            };

            var opimm32 = new OpRec[]
            {
                new WOpRec(Opcode.addiw, "d,1,i"),
                new ShiftOpRec("d,1,Z", Opcode.slliw, Opcode.invalid),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                                           
                new WOpRec(Opcode.invalid, ""),
                new ShiftOpRec("d,1,Z", Opcode.srliw, Opcode.sraiw),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
            };

            var op32 = new OpRec[]
            {
                new ShiftOpRec("d,1,2", Opcode.addw, Opcode.subw),
                new ShiftOpRec("d,1,2", Opcode.sllw, Opcode.invalid),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),

                new WOpRec(Opcode.invalid, ""),
                new ShiftOpRec("d,1,2", Opcode.srlw, Opcode.sraw),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
            };

            var opfp = new Dictionary<int, OpRec>
            {
                { 0x00, new FpuOpRec(Opcode.fadd_s, "Fd,F1,F2") },
                { 0x01, new FpuOpRec(Opcode.fadd_d, "Fd,F1,F2") },
                { 0x21, new FpuOpRec(Opcode.fcvt_d_s, "Fd,F1") },
                { 0x50, new SparseMaskOpRec(12, 7, new Dictionary<int, OpRec>
                    {
                        { 2, new WOpRec(Opcode.feq_s, "d,F1,F2") }
                    })
                },
                { 0x71, new FpuOpRec(Opcode.fmv_d_x, "Fd,1") },
                { 0x78, new FpuOpRec(Opcode.fmv_s_x, "Fd,1") },
            };

            var branches = new OpRec[]
            {
                new WOpRec(Opcode.beq, InstrClass.ConditionalTransfer, "1,2,B"),
                new WOpRec(Opcode.bne, InstrClass.ConditionalTransfer, "1,2,B"),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),

                new WOpRec(Opcode.blt, InstrClass.ConditionalTransfer, "1,2,B"),
                new WOpRec(Opcode.bge, InstrClass.ConditionalTransfer, "1,2,B"),
                new WOpRec(Opcode.bltu, InstrClass.ConditionalTransfer, "1,2,B"),
                new WOpRec(Opcode.bgeu, InstrClass.ConditionalTransfer, "1,2,B"),
            };

            wideOpRecs = new OpRec[]
            {
                // 00
                new MaskOpRec(12, 7, loads),
                new MaskOpRec(12, 7, fploads),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),

                new MaskOpRec(12, 7, opimm),
                new WOpRec(Opcode.auipc, "d,Iu"),
                new MaskOpRec(12, 7, opimm32),
                new WOpRec(Opcode.invalid, ""),

                new MaskOpRec(12, 7, stores),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),

                new MaskOpRec(12, 7, op),
                new WOpRec(Opcode.lui, "d,Iu"),
                new MaskOpRec(12, 7, op32),
                new WOpRec(Opcode.invalid, ""),

                // 10
                new FpuOpRec(Opcode.fmadd_s, "Fd,F1,F2,F3"),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),

                new SparseMaskOpRec(25, 0x7F, opfp),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),

                new MaskOpRec(12, 7, branches),
                new WOpRec(Opcode.jalr, InstrClass.Transfer, "d,1,i"),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.jal, InstrClass.Transfer|InstrClass.Call, "d,J"),

                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
                new WOpRec(Opcode.invalid, ""),
            };

            compressed0 = new OpRec[]
            {
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),

                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
            };

            compressed1 = new OpRec[]
            {
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),

                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
            };

            compressed2 = new OpRec[]
            {
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),

                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
                new COpRec(Opcode.invalid, ""),
            };

            opRecs = new OpRec[] 
            {
                new MaskOpRec(0x13, 7, compressed0),
                new MaskOpRec(0x13, 7, compressed1),
                new MaskOpRec(0x13, 7, compressed2),
                new WideOpRec()
            };
        }
    }
}
