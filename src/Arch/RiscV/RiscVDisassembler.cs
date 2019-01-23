#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Types;

namespace Reko.Arch.RiscV
{
    public class RiscVDisassembler : DisassemblerBase<RiscVInstruction>
    {
        private static readonly Decoder[] opRecs;
        private static readonly Decoder[] w32decoders;
        private static readonly Decoder[] compressed0;
        private static readonly Decoder[] compressed1;
        private static readonly Decoder[] compressed2;
        private static readonly int[] compressedRegs;
        private static readonly Decoder invalid;

        private RiscVArchitecture arch;
        private EndianImageReader rdr;
        private Address addrInstr;
        private State state;

        public RiscVDisassembler(RiscVArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = new State();
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

        private static HashSet<uint> seen = new HashSet<uint>();

        private RiscVInstruction NotYetImplemented(uint instr, string message)
        {
            if (!seen.Contains(instr))
            {
                seen.Add(instr);
                base.EmitUnitTest("RiscV", instr.ToString("X8"), message, "RiscV_dasm", Address.Ptr32(0x00100000), w =>
                {
                    w.WriteLine("    AssertCode(\"@@@\", 0x{0:X8});", instr);
                });
            }
            return MakeInvalid();
        }

        public RiscVInstruction MakeInstruction()
        {
            var i = state.instr;
            i.Address = this.addrInstr;
            var ops = this.state.ops;
            if (ops.Count > 0)
            {
                i.op1 = ops[0];
                if (ops.Count > 1)
                {
                    i.op2 = ops[1];
                    if (ops.Count > 2)
                    {
                        i.op3 = ops[2];
                        if (ops.Count > 3)
                        {
                            i.op4 = ops[4];
                        }
                    }
                }
            }
            state.instr = new RiscVInstruction();
            return i;
        }

        internal RiscVInstruction MakeInvalid()
        {
            return new RiscVInstruction
            {
                Address = addrInstr,
                iclass = InstrClass.Invalid,
                opcode = Opcode.invalid,
            };
        }


        internal class State
        {
            public RiscVInstruction instr = new RiscVInstruction();
            public List<MachineOperand> ops = new List<MachineOperand>();
        }

        #region Decoders

        public abstract class Decoder
        {
            public abstract RiscVInstruction Decode(RiscVDisassembler dasm, uint hInstr);
        }

        public class NyiDecoder : Decoder
        {
            private readonly string message; 

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint hInstr)
            {
                return dasm.NotYetImplemented(hInstr, message);
            }
        }

        public class CDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Opcode opcode;
            private readonly Mutator[] mutators;

            internal CDecoder(InstrClass iclass, Opcode opcode, params Mutator[] mutators)
            {
                this.iclass = iclass;
                this.opcode = opcode;
                this.mutators = mutators;
            }

            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint wInstr)
            {
                dasm.state.ops.Clear();
                dasm.state.instr.opcode = opcode;
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                        return dasm.MakeInvalid();
                }
                return dasm.MakeInstruction();
            }
        }

        public class WInstrDecoderOld : Decoder
        {
            private readonly Opcode opcode;
            private readonly InstrClass iclass;
            private readonly string fmt;

            public WInstrDecoderOld(Opcode opcode, string fmt) : this(opcode, InstrClass.Linear, fmt)
            {
            }

            public WInstrDecoderOld(Opcode opcode, InstrClass iclass, string fmt)
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

        public class FpuOpRec : Decoder
        {
            private readonly string fmt;
            private readonly Opcode opcode;

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

        public class MaskDecoder : Decoder
        {
            private readonly int mask;
            private readonly int shift;
            private readonly Decoder[] subcodes;

            public MaskDecoder(int shift, int mask, params Decoder[] subcodes)
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

        public class SparseMaskOpRec : Decoder
        {
            private readonly int mask;
            private readonly int shift;
            private readonly Dictionary<int, Decoder> subcodes;

            public SparseMaskOpRec(int shift, int mask, Dictionary<int, Decoder> subcodes)
            {
                this.mask = mask;
                this.shift = shift;
                this.subcodes = subcodes;
            }

            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint wInstr)
            {
                var slot = (int)((wInstr >> shift) & mask);
                if (!subcodes.TryGetValue(slot, out Decoder decoder))
                {
                    return dasm.MakeInvalid();
                }
                return decoder.Decode(dasm, wInstr);
            }
        }

        public class W32Decoder : Decoder
        {
            public W32Decoder()
            {
            }

            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint hInstr)
            {
                if (!dasm.rdr.TryReadUInt16(out ushort hiword))
                {
                    return dasm.MakeInvalid();
                }
                uint wInstr = (uint)hiword << 16;
                wInstr |= hInstr;
                var slot = (wInstr >> 2) & 0x1F;
                return w32decoders[slot].Decode(dasm, wInstr);
            }
        }

        public class ShiftOpRec : Decoder
        {
            private readonly Opcode[] rl_ra;
            private readonly string fmt;

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

        public class CondDecoder : Decoder
        {
            private readonly Bitfield mask;
            private readonly Func<uint, bool> predicate;
            private readonly Decoder trueDecoder;
            private readonly Decoder falseDecoder;

            public CondDecoder(int bitPos, int len, Func<uint, bool> pred, Decoder trueDecoder, Decoder falseDecoder)
            {
                this.mask = new Bitfield(bitPos, len);
                this.predicate = pred;
                this.trueDecoder = trueDecoder;
                this.falseDecoder = falseDecoder;
            }

            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint hInstr)
            {
                var value = mask.Read(hInstr);
                if (predicate(value))
                    return trueDecoder.Decode(dasm, hInstr);
                else
                    return falseDecoder.Decode(dasm, hInstr);
            }
        }

        /// <summary>
        /// Handle instructions that are encoded differently depending on the word size
        /// of the CPU architecture.
        /// </summary>
        internal class WordSizeDecoder : Decoder
        {
            private readonly Decoder rv32;
            private readonly Decoder rv64;
            private readonly Decoder rv128;

            public WordSizeDecoder(
                Decoder rv32 = null,
                Decoder rv64 = null,
                Decoder rv128 = null)
            {
                this.rv32 = rv32 ?? invalid;
                this.rv64 = rv64 ?? invalid;
                this.rv128 = rv128 ?? invalid;
            }
            public override RiscVInstruction Decode(RiscVDisassembler dasm, uint hInstr)
            {
                switch (dasm.arch.WordWidth.Size)
                {
                case 4: return rv32.Decode(dasm, hInstr);
                case 8: return rv64.Decode(dasm, hInstr);
                case 16: return rv128.Decode(dasm, hInstr);
                }
                throw new NotSupportedException($"{dasm.arch.WordWidth.Size}-bit Risc-V instructions not supported.");
            }
        }

        #endregion

        private static WInstrDecoderOld Instr(Opcode opcode, string format)
        {
            return new WInstrDecoderOld(opcode, InstrClass.Linear, format);
        }

        private static WInstrDecoderOld Instr(InstrClass iclass, Opcode opcode, string format)
        {
            return new WInstrDecoderOld(opcode, iclass, format);
        }

        // Compact instruction decoder

        private static CDecoder CInstr(Opcode opcode, params Mutator[] mutators)
        {
            return new CDecoder(InstrClass.Linear, opcode, mutators);
        }

        private static CDecoder CInstr(InstrClass iclass, Opcode opcode, params Mutator[] mutator)
        {
            return new CDecoder(iclass, opcode, mutator);
        }

        // Conditional decoder

        private static CondDecoder Cond(int bitPos, int length, Func<uint, bool> predicate, Decoder t, Decoder f)
        {
            return new CondDecoder(bitPos, length, predicate, t, f);
        }

        private static WordSizeDecoder WordSize(
            Decoder rv32 = null,
            Decoder rv64 = null,
            Decoder rv128 = null)
        {
            return new WordSizeDecoder(rv32, rv64, rv128);
        }

        private static NyiDecoder Nyi(string message)
        {
            return new NyiDecoder(message);
        }

        #region Mutators
        // Integer register
        private static Mutator R(int bitPos)
        {
            var regMask = new Bitfield(bitPos, 5);
            return (u, d) =>
            {
                var iReg = (int) regMask.Read(u);
                var reg = new RegisterOperand(d.arch.GetRegister(iReg));
                d.state.ops.Add(reg);
                return true;
            };
        }

        // Registers specified at fixed positions.

        private static readonly Mutator Rd = R(7);
        private static readonly Mutator R1 = R(15);
        private static readonly Mutator R2 = R(20);

        // Floating point register
        private static Mutator F(int bitPos)
        {
            var regMask = new Bitfield(bitPos, 5);
            return (u, d) =>
            {
                var iReg = (int) regMask.Read(u);
                var reg = new RegisterOperand(d.arch.FpRegs[iReg]);
                d.state.ops.Add(reg);
                return true;
            };
        }

        // Compressed format register (r')
        private static Mutator Rc(int bitPos)
        {
            var regMask = new Bitfield(bitPos, 3);
            return (u, d) =>
            {
                var iReg = compressedRegs[regMask.Read(u)];
                var reg = new RegisterOperand(d.arch.GetRegister(iReg));
                d.state.ops.Add(reg);
                return true;
            };
        }

        // Compressed format floating point register (fr')
        private static Mutator Fc(int bitPos)
        {
            var regMask = new Bitfield(bitPos, 3);
            return (u, d) =>
            {
                var iReg = compressedRegs[regMask.Read(u)];
                var reg = new RegisterOperand(d.arch.FpRegs[iReg]);
                d.state.ops.Add(reg);
                return true;
            };
        }

        // Unsigned immediate
        private static Mutator Imm(int bitPos1, int length1)
        {
            var mask = new Bitfield(bitPos1, length1);
            return (u, d) =>
            {
                var imm = Constant.Create(d.arch.WordWidth, mask.Read(u));
                d.state.ops.Add(new ImmediateOperand(imm));
                return true;
            };
        }

        private static Mutator Imm(params (int pos, int len) [] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();
            return (u, d) =>
            {
                var uImm = Bitfield.ReadFields(masks, u);
                d.state.ops.Add(new ImmediateOperand(
                    Constant.Create(d.arch.WordWidth, uImm)));
                return true;
            };
        }

        // Immediate with a shift
        private static Mutator ImmSh(int sh, params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();
            return (u, d) =>
            {
                var uImm = Bitfield.ReadFields(masks, u) << sh;
                d.state.ops.Add(new ImmediateOperand(
                    Constant.Create(d.arch.WordWidth, uImm)));
                return true;
            };
        }

        private static Mutator ImmB(params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();
            return (u, d) =>
            {
                var uImm = Bitfield.ReadFields(masks, u);
                d.state.ops.Add(new ImmediateOperand(
                    Constant.Create(PrimitiveType.Byte, uImm)));
                return true;
            };
        }

        private static Mutator ImmS(params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();
            return (u, d) =>
            {
                var sImm = Bitfield.ReadSignedFields(masks, u);
                d.state.ops.Add(new ImmediateOperand(
                    Constant.Create(d.arch.WordWidth, sImm)));
                return true;
            };
        }

        // Signed immediate with a shift
        private static Mutator ImmShS(int sh, params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();
            return (u, d) =>
            {
                var uImm = Bitfield.ReadSignedFields(masks, u) << sh;
                d.state.ops.Add(new ImmediateOperand(
                    Constant.Create(d.arch.WordWidth, uImm)));
                return true;
            };
        }

        // PC-relative displacement with shift.
        private static Mutator PcRel(int sh, params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();
            return (u, d) =>
            {
                var sImm = Bitfield.ReadSignedFields(masks, u) << sh;
                var addr = d.addrInstr + sImm;
                d.state.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }

        // Memory operand format used for compressed instructions
        private static Mutator Memc(PrimitiveType dt, int regOffset, params (int pos, int len)[] fields)
        {
            var baseRegMask = new Bitfield(regOffset, 3);
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();
            return (u, d) =>
            {
                var uOffset = (int) Bitfield.ReadFields(masks, u) * dt.Size;
                var iBase = compressedRegs[baseRegMask.Read(u)];

                d.state.ops.Add(new MemoryOperand(dt)
                {
                    Base = d.arch.GetRegister(iBase),
                    Offset = uOffset
                });
                return true;
            };
        }
        #endregion

        static RiscVDisassembler()
        {
            invalid = new WInstrDecoderOld(Opcode.invalid, "");

            var loads = new Decoder[]
            {
                new WInstrDecoderOld(Opcode.lb, "d,1,Ls"),
                new WInstrDecoderOld(Opcode.lh, "d,1,Ls"),
                new WInstrDecoderOld(Opcode.lw, "d,1,Ls"),
                new WInstrDecoderOld(Opcode.ld, "d,1,Ls"),

                new WInstrDecoderOld(Opcode.lbu, "d,1,Ls"),
                new WInstrDecoderOld(Opcode.lhu, "d,1,Ls"),
                new WInstrDecoderOld(Opcode.lwu, "d,1,Ls"),    // 64
                Nyi(""),
            };

            var fploads = new Decoder[]
            {
                invalid,
                invalid,
                new WInstrDecoderOld(Opcode.flw, "Fd,1,Ls"),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
            };

            var stores = new Decoder[]
            {
                new WInstrDecoderOld(Opcode.sb, "2,1,Ss"),
                new WInstrDecoderOld(Opcode.sh, "2,1,Ss"),
                new WInstrDecoderOld(Opcode.sw, "2,1,Ss"),
                new WInstrDecoderOld(Opcode.sd, "2,1,Ss"),

                invalid,
                invalid,
                invalid,
                invalid,
            };

            var fpstores = new Decoder[]
            {
                invalid,
                invalid,
                new WInstrDecoderOld(Opcode.fsw, "Fd,1,Ls"),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
            };


            var op = new Decoder[]
            {
                new ShiftOpRec( "d,1,2", Opcode.add, Opcode.sub),
                new WInstrDecoderOld(Opcode.sll, "d,1,2"),
                new WInstrDecoderOld(Opcode.slt, "d,1,2"),
                new WInstrDecoderOld(Opcode.sltu, "d,1,2"),

                new WInstrDecoderOld(Opcode.xor, "d,1,2"),
                new ShiftOpRec("d,1,2", Opcode.srl, Opcode.sra),
                new WInstrDecoderOld(Opcode.or, "d,1,2"),
                new WInstrDecoderOld(Opcode.and, "d,1,2"),
            };

            var opimm = new Decoder[]
            {
                new WInstrDecoderOld(Opcode.addi, "d,1,i"),
                new ShiftOpRec("d,1,z", Opcode.slli, Opcode.invalid),
                new WInstrDecoderOld(Opcode.slti, "d,1,i"),
                new WInstrDecoderOld(Opcode.sltiu, "d,1,i"),

                new WInstrDecoderOld(Opcode.xori, "d,1,i"),
                new ShiftOpRec("d,1,z", Opcode.srli, Opcode.srai),
                new WInstrDecoderOld(Opcode.ori, "d,1,i"),
                new WInstrDecoderOld(Opcode.andi, "d,1,i"),
            };

            var opimm32 = new Decoder[]
            {
                new WInstrDecoderOld(Opcode.addiw, "d,1,i"),
                new ShiftOpRec("d,1,Z", Opcode.slliw, Opcode.invalid),
                Nyi(""),
                Nyi(""),
                                           
                Nyi(""),
                new ShiftOpRec("d,1,Z", Opcode.srliw, Opcode.sraiw),
                Nyi(""),
                Nyi(""),
            };

            var op32 = new Decoder[]
            {
                new ShiftOpRec("d,1,2", Opcode.addw, Opcode.subw),
                new ShiftOpRec("d,1,2", Opcode.sllw, Opcode.invalid),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                new ShiftOpRec("d,1,2", Opcode.srlw, Opcode.sraw),
                Nyi(""),
                Nyi(""),
            };

            var opfp = new Dictionary<int, Decoder>
            {
                { 0x00, new FpuOpRec(Opcode.fadd_s, "Fd,F1,F2") },
                { 0x01, new FpuOpRec(Opcode.fadd_d, "Fd,F1,F2") },
                { 0x21, new FpuOpRec(Opcode.fcvt_d_s, "Fd,F1") },
                { 0x50, new SparseMaskOpRec(12, 7, new Dictionary<int, Decoder>
                    {
                        { 2, new WInstrDecoderOld(Opcode.feq_s, "d,F1,F2") }
                    })
                },
                { 0x71, new FpuOpRec(Opcode.fmv_d_x, "Fd,1") },
                { 0x78, new FpuOpRec(Opcode.fmv_s_x, "Fd,1") },
            };

            var branches = new Decoder[]
            {
                new WInstrDecoderOld(Opcode.beq, InstrClass.ConditionalTransfer, "1,2,B"),
                new WInstrDecoderOld(Opcode.bne, InstrClass.ConditionalTransfer, "1,2,B"),
                Nyi(""),
                Nyi(""),

                new WInstrDecoderOld(Opcode.blt, InstrClass.ConditionalTransfer, "1,2,B"),
                new WInstrDecoderOld(Opcode.bge, InstrClass.ConditionalTransfer, "1,2,B"),
                new WInstrDecoderOld(Opcode.bltu, InstrClass.ConditionalTransfer, "1,2,B"),
                new WInstrDecoderOld(Opcode.bgeu, InstrClass.ConditionalTransfer, "1,2,B"),
            };

            w32decoders = new Decoder[]
            {
                // 00
                new MaskDecoder(12, 7, loads),
                new MaskDecoder(12, 7, fploads),
                Nyi("custom-0"),
                Nyi("misc-mem"),

                new MaskDecoder(12, 7, opimm),
                new WInstrDecoderOld(Opcode.auipc, "d,Iu"),
                new MaskDecoder(12, 7, opimm32),
                Nyi("48-bit instruction"),

                new MaskDecoder(12, 7, stores),
                Nyi("fp-stores"),
                Nyi("custom-1"),
                Nyi("amo"),

                new MaskDecoder(12, 7, op),
                new WInstrDecoderOld(Opcode.lui, "d,Iu"),
                new SparseMaskOpRec(25, 0x7F, new Dictionary<int, Decoder>
                {
                    { 1, new MaskDecoder(12, 7,
                        CInstr(Opcode.mulw, Rd,R1,R2),
                        invalid,
                        invalid,
                        invalid,
                        
                        CInstr(Opcode.divw, Rd,R1,R2),
                        Nyi("divuw"),
                        Nyi("remw"),
                        CInstr(Opcode.remuw, Rd,R1,R2))
                    },
                    { 0x20, new MaskDecoder(12, 7,
                        CInstr(Opcode.subw, Rd,R1,R2),
                        Nyi("20 - 001"),
                        Nyi("20 - 010"),
                        Nyi("20 - 011"),

                        Nyi("20 - 100"),
                        Nyi("20 - 101"),
                        Nyi("20 - 110"),
                        Nyi("20 - 111"))}
                }),
                Nyi("64-bit instruction"),

                // 10
                new FpuOpRec(Opcode.fmadd_s, "Fd,F1,F2,F3"),
                Nyi("msub"),
                Nyi("nmsub"),
                Nyi("nmadd"),

                new SparseMaskOpRec(25, 0x7F, opfp),
                Nyi("Reserved"),
                Nyi("custom-2"),
                Nyi("48-bit instruction"),

                new MaskDecoder(12, 7, branches),
                new WInstrDecoderOld(Opcode.jalr, InstrClass.Transfer, "d,1,i"),
                Nyi("Reserved"),
                new WInstrDecoderOld(Opcode.jal, InstrClass.Transfer|InstrClass.Call, "d,J"),

                Nyi("system"),
                Nyi("Reserved"),
                Nyi("custom-3"),
                Nyi(">= 80-bit instruction"),
            };

            compressedRegs = new int[8]
            {
                8, 9, 10, 11, 12, 13, 14, 15
            };

            compressed0 = new Decoder[8]
            {
                Cond(0, 16, u => u != 0,
                    CInstr(Opcode.c_addi4spn, Rc(2), Imm((7,4), (11,2), (5, 1),(6, 1), (0,2))),
                    CInstr(InstrClass.Invalid|InstrClass.Zero, Opcode.invalid)),
                WordSize(
                    rv32: CInstr(Opcode.c_fld, Fc(7), Memc(PrimitiveType.Word64, 2, (5,2), (10, 3))),
                    rv64: CInstr(Opcode.c_fld, Fc(7), Memc(PrimitiveType.Word64, 2, (5,2), (10, 3))),
                    rv128: Nyi("lq")),
                CInstr(Opcode.c_lw, Rc(7), Memc(PrimitiveType.Word32, 2, (5,1), (10,3), (6,1))),
                WordSize(
                    rv32: CInstr(Opcode.c_flw, Fc(7), Memc(PrimitiveType.Word32, 2, (5,1), (10,3), (6,1))),
                    rv64: CInstr(Opcode.c_ld, Rc(7), Memc(PrimitiveType.Word64, 2, (5,2), (10, 3))),
                    rv128: CInstr(Opcode.c_ld, Rc(7), Memc(PrimitiveType.Word64, 2, (5,2), (10, 3)))),

                Nyi("reserved"),
                WordSize(
                    rv32: Nyi("fsd"),
                    rv64: Nyi("fsd"),
                    rv128: Nyi("sq")),
                CInstr(Opcode.c_sw, Rc(7), Memc(PrimitiveType.Word32, 2, (5,1), (10,3), (6,1))),
                WordSize(
                    rv32: Nyi("fsw"),
                    rv64: CInstr(Opcode.c_sd, Rc(7), Memc(PrimitiveType.Word64, 2, (5,2), (10, 3))),
                    rv128: CInstr(Opcode.c_sd, Rc(7), Memc(PrimitiveType.Word64, 2, (5,2), (10, 3)))),
            };

            compressed1 = new Decoder[8]
            {
                CInstr(Opcode.c_addi, R(7), ImmS((12, 1), (2, 5))),
                WordSize(
                    rv32: Nyi("c.jal"),
                    rv64: CInstr(Opcode.c_addiw, R(7), ImmS((12, 1), (2, 5))),
                    rv128: CInstr(Opcode.c_addiw, R(7), ImmS((12, 1), (2, 5)))),
                CInstr(Opcode.c_li, R(7), ImmS((12,1), (2, 5))),
                Cond(7, 5, u => u == 2,
                    CInstr(Opcode.c_addi16sp, ImmShS(4, (12,1), (3,2), (5,1), (2,1), (6, 1))),
                    CInstr(Opcode.c_lui, R(7), ImmShS(12, (12,1), (2, 5)))),

                new MaskDecoder(10, 3, 
                    CInstr(Opcode.c_srli, Rc(7), Imm((12,1), (2,5))),
                    CInstr(Opcode.c_srai, Rc(7), Imm((12,1), (2,5))),
                    CInstr(Opcode.c_andi, Rc(7), ImmS((12,1), (2,5))),
                    new MaskDecoder(12, 1,
                        new MaskDecoder(5, 3,
                            CInstr(Opcode.c_sub, Rc(7), Rc(2)),
                            CInstr(Opcode.c_xor, Rc(7), Rc(2)),
                            CInstr(Opcode.c_or, Rc(7), Rc(2)),
                            CInstr(Opcode.c_and, Rc(7), Rc(2))),
                        new MaskDecoder(5, 3,
                            WordSize(
                                rv64: CInstr(Opcode.c_subw, Rc(7),Rc(2)),
                                rv128: CInstr(Opcode.c_subw, Rc(7),Rc(2))),
                            WordSize(
                                rv64: CInstr(Opcode.c_addw, Rc(7),Rc(2)),
                                rv128: CInstr(Opcode.c_addw, Rc(7),Rc(2))),
                            invalid,
                            invalid))),

// imm[11|4|9:8|10|6|7|3:1|5]
     //11 10  9  8 7 6   3 2
                CInstr(Opcode.c_j, PcRel(1, (11,1), (8,1), (9,2), (6,1), (7,1), (2,1), (10,1), (3,2))),
                CInstr(Opcode.c_beqz, Rc(7), PcRel(1, (12,1), (5,2), (2,1), (10,2), (3, 2))),
                CInstr(Opcode.c_bnez, Rc(7), PcRel(1, (12,1), (5,2), (2,1), (10,2), (3, 2))),
            };

            compressed2 = new Decoder[8]
            {
                CInstr(Opcode.c_slli, R(7), ImmB((12, 1), (2, 5))),
                WordSize(
                    rv32: CInstr(Opcode.c_fldsp, F(2), ImmSh(3, (12,1),(7,3),(10,3))),
                    rv64: CInstr(Opcode.c_fldsp, F(2), ImmSh(3, (12,1),(7,3),(10,3)))),
                CInstr(Opcode.c_lwsp, R(2), ImmSh(2, (12,1),(7,3),(10,3))),
                CInstr(Opcode.c_ldsp, R(2), ImmSh(3, (12,1),(7,3),(10,3))),

                new MaskDecoder(12, 1, 
                    Cond(2, 5, u => u == 0,
                        CInstr(Opcode.c_jr, R(7)),
                        CInstr(Opcode.c_mv, R(7), R(2))),
                    Cond(2, 5, u => u == 0,
                        Cond(7, 5, u => u == 0,
                            Nyi("c.ebreak"),
                            CInstr(Opcode.c_jalr, R(7))),
                        CInstr(Opcode.c_add, R(7), R(2)))),
                Nyi("fsdsp"),
                CInstr(Opcode.c_swsp, R(2), ImmSh(2, (7,3),(10,3))),
                CInstr(Opcode.c_sdsp, R(2), ImmSh(3, (7,3),(10,3))),
            };

            opRecs = new Decoder[] 
            {
                new MaskDecoder(13, 7, compressed0),
                new MaskDecoder(13, 7, compressed1),
                new MaskDecoder(13, 7, compressed2),
                new W32Decoder()
            };
        }
    }

    internal delegate bool Mutator(uint wInstr, RiscVDisassembler dasm);
}
