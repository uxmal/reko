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

using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Types;
using System.Diagnostics;

namespace Reko.Arch.SuperH
{
    // http://www.shared-ptr.com/sh_insns.html
    
    public class SuperHDisassembler : DisassemblerBase<SuperHInstruction>
    {
        private EndianImageReader rdr;

        public SuperHDisassembler(EndianImageReader rdr)
        {
            this.rdr = rdr;
        }

        public override SuperHInstruction DisassembleInstruction()
        {
            var addr = rdr.Address;
            if (!rdr.TryReadUInt16(out ushort uInstr))
                return null;
            var instr = oprecs[uInstr >> 12].Decode(this, uInstr);
            instr.Address = addr;
            instr.Length = 2;
            return instr;
        }

        private SuperHInstruction Decode(ushort uInstr, Opcode opcode, string format)
        {
            var ops = new MachineOperand[2];
            int iop = 0;
            RegisterStorage reg;
            PrimitiveType width;
            for (int i = 0; i < format.Length; ++i)
            {
                switch (format[i])
                {
                case ',':
                    continue;
                case '-': // predecrement
                    reg = Register(format[++i], uInstr);
                    ops[iop] = MemoryOperand.IndirectPreDecr(GetWidth(format[++i]), reg);
                    break;
                case '+': // postdecrement
                    reg = Register(format[++i], uInstr);
                    ops[iop] = MemoryOperand.IndirectPostIncr(GetWidth(format[++i]), reg);
                    break;
                case 'd':
                    ops[iop] = DfpRegister(format[++i], uInstr);
                    break;
                case 'f':
                    ops[iop] = FpRegister(format[++i], uInstr);
                    break;
                case 'r':
                    ops[iop] = new RegisterOperand(Register(format[++i], uInstr));
                    break;
                case 'v':
                    ops[iop] = VfpRegister(format[++i], uInstr);
                    break;
                case 'I':
                    ops[iop] = ImmediateOperand.Byte((byte)uInstr);
                    break;
                case 'F':
                    if (format[++i] == 'U')
                    {
                        ops[iop] = new RegisterOperand(Registers.fpul);
                        break;
                    }
                    goto default;
                case 'G':
                    ops[iop] = MemoryOperand.GbrIndexedIndirect(GetWidth(format[++i]));
                    break;
                case '@':
                    reg = Register(format[++i], uInstr);
                    ops[iop] = MemoryOperand.Indirect(GetWidth(format[++i]), reg);
                    break;
                case 'D':   // indirect with displacement
                    reg = Register(format[++i], uInstr);
                    width = GetWidth(format[++i]);
                    ops[iop] = MemoryOperand.IndirectDisplacement(width, reg, (uInstr & 0xF) * width.Size);
                    break;
                case 'X':   // indirect indexed
                    reg = Register(format[++i], uInstr);
                    ops[iop] = MemoryOperand.IndexedIndirect(GetWidth(format[++i]), reg);
                    break;
                case 'P':   // PC-relative with displacement
                    width = GetWidth(format[++i]);
                    ops[iop] = MemoryOperand.PcRelativeDisplacement(width, width.Size * (byte)uInstr);
                    break;
                case 'R':
                    ++i;
                    if (format[i] == 'P')
                        ops[iop] = new RegisterOperand(Registers.pr);
                    else
                        ops[iop] = new RegisterOperand(Registers.gpregs[HexDigit(format[i]).Value]);
                    break;
                case 'j':
                    ops[iop] = AddressOperand.Create(rdr.Address + (2 + 2 * (sbyte)uInstr));
                    break;
                case 'J':
                    int offset = ((int)uInstr << 20) >> 19;
                    ops[iop] = AddressOperand.Create(rdr.Address + (2 + offset));
                    break;
                case 'm':   // Macl.
                    if (i < format.Length-1)
                    {
                        ++i;
                        if (format[i] == 'l')
                        {
                            ops[iop] = new RegisterOperand(Registers.macl);
                        }
                        else if (format[i] == 'h')
                        {
                            ops[iop] = new RegisterOperand(Registers.mach);
                        }
                        else
                        {
                            throw new NotImplementedException(string.Format("m{0}", format[i]));
                        }
                    }
                    else
                    {
                        throw new NotImplementedException(string.Format("m{0}", format[i]));
                    }
                    break;
                default: throw new NotImplementedException(string.Format("SuperHDisassembler.Decode({0})", format[i]));
                }
                ++iop;
            }
            return new SuperHInstruction
            {
                Opcode = opcode,
                op1 = ops[0],
                op2 = ops[1],
            };
        }

        private static HashSet<ushort> seen = new HashSet<ushort>();

        private SuperHInstruction Invalid(ushort wInstr)
        {
            if (!seen.Contains(wInstr))
            {
                seen.Add(wInstr);
                Debug.WriteLine($"// A SuperH decoder for the instruction {wInstr:X4} has not been implemented yet.");
                Debug.WriteLine("[Test]");
                Debug.WriteLine($"public void ShDis_{wInstr:X4}()");
                Debug.WriteLine("{");
                Debug.WriteLine($"    AssertCode(\"@@@\", \"{wInstr&0xFF:X2}{wInstr>>8:X2}\");");
                Debug.WriteLine("}");
                Debug.WriteLine("");
            }
#if !DEBUG
                throw new NotImplementedException($"A SuperH decoder for the instruction {wInstr:X4} has not been implemented yet.");
#else
            return Decode(wInstr, Opcode.invalid, "");
#endif
        }

        private PrimitiveType GetWidth(char w)
        {
            if (w == 'b')
                return PrimitiveType.Byte;
            else if (w == 'w')
                return PrimitiveType.Word16;
            else if (w == 'l')
                return PrimitiveType.Word32;
            else 
                throw new NotImplementedException(string.Format("{0}", w));
        }

        public static uint? HexDigit(char digit)
        {
            switch (digit)
            {
            case '0': case '1': case '2': case '3': case '4': 
            case '5': case '6': case '7': case '8': case '9':
                return (uint) (digit - '0');
            case 'A': case 'B': case 'C': case 'D': case 'E': case 'F':
                return (uint) ((digit - 'A') + 10);
            case 'a': case 'b': case 'c': case 'd': case 'e': case 'f':
                return (uint) ((digit - 'a') + 10);
            default:
                return null;
            }
        }

        private RegisterStorage Register(char r, ushort uInstr)
        {
            var reg = (uInstr >> 4 * ('3' - r)) & 0xF;
            return Registers.gpregs[reg];
        }

        private RegisterOperand FpRegister(char r, ushort uInstr)
        {
            var reg = (uInstr >> 4 * ('3' - r)) & 0xF;
            return new RegisterOperand(Registers.fpregs[reg]);
        }

        private MachineOperand DfpRegister(char r, ushort uInstr)
        {
            var reg = (uInstr >> (1 + 4 * ('3' - r))) & 0x7;
            return new RegisterOperand(Registers.dfpregs[reg]);
        }

        private MachineOperand VfpRegister(char r, ushort uInstr)
        {
            var reg = (uInstr >> (8 + 2 * ('2' - r))) & 0x3;
            return new RegisterOperand(Registers.vfpregs[reg]);
        }

        private abstract class OprecBase
        {
            public abstract SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr);
        }

        private class Oprec : OprecBase
        {
            private Opcode opcode;
            private string format;

            public Oprec(Opcode op, string format)
            {
                this.opcode = op;
                this.format = format;
            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                return dasm.Decode(uInstr, opcode, format);
            }

            public override string ToString()
            {
                return string.Format("[ {0} {1} ]", opcode, format);
            }
        }

        private class Oprec4Bits : OprecBase
        {
            private int shift;
            private OprecBase[] oprecs;

            public Oprec4Bits(int shift, OprecBase[] oprecs)
            {
                this.shift = shift;
                this.oprecs = oprecs;
            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                return oprecs[(uInstr >> shift) & 0xF].Decode(dasm, uInstr);
            }
        }

        private class OprecField : OprecBase
        {
            private int shift;
            private int bitcount;
            private Dictionary<int, OprecBase> oprecs;

            public OprecField(int shift, int bitcount, Dictionary<int, OprecBase> oprecs)
            {
                this.shift = shift;
                this.bitcount = bitcount;
                this.oprecs = oprecs;
            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                var mask = (1 << bitcount) - 1;
                if (!oprecs.TryGetValue((uInstr >> shift) & mask, out OprecBase or))
                    return dasm.Invalid(uInstr);
                return or.Decode(dasm, uInstr);
            }
        }

        private static OprecBase[] oprecs = new OprecBase[]
        {
            // 0...
            new OprecField(0, 4, new Dictionary<int, OprecBase>
            {
                { 0x03, new OprecField(4, 4, new Dictionary<int, OprecBase>
                    {
                        { 0x0, new Oprec(Opcode.bsrf, "r1") },
                        { 0x2, new Oprec(Opcode.braf, "r1") },
                    })
                },
                { 0x4, new Oprec(Opcode.mov_b, "r2,X1b") },
                { 0x5, new Oprec(Opcode.mov_w, "r2,X1w") },
                { 0x6, new Oprec(Opcode.mov_l, "r2,X1l") },
                { 0x7, new Oprec(Opcode.mul_l, "r2,r1") },
                { 0x8, new OprecField(4, 4, new Dictionary<int, OprecBase>
                    {
                        { 0x0, new Oprec(Opcode.clrt, "") },
                        { 0x1, new Oprec(Opcode.sett, "") },
                        { 0x2, new Oprec(Opcode.clrmac, "") },
                        { 0x4, new Oprec(Opcode.clrs, "") },
                    })
                },
                { 0x9, new OprecField(4, 4, new Dictionary<int, OprecBase>
                    {
                        { 0x0, new Oprec(Opcode.nop, "") },
                        { 0x1, new Oprec(Opcode.div0u, "") },
                        { 0x2, new Oprec(Opcode.movt, "r1") },
                    })
                },
                { 0xA, new OprecField(4, 4, new Dictionary<int, OprecBase>
                    {
                        { 0x0, new Oprec(Opcode.sts, "mh,r1") },
                        { 0x1, new Oprec(Opcode.sts, "ml,r1") },
                    })
                },
                { 0xB, new OprecField(4, 4, new Dictionary<int, OprecBase>
                    {
                        { 0x0, new Oprec(Opcode.rts, "") },
                        { 0x3, new Oprec(Opcode.brk, "") },
                    })
                },
                { 0xC, new Oprec(Opcode.mov_b, "X2b,r1") },
                { 0xD, new Oprec(Opcode.mov_w, "X2w,r1") },
                { 0xE, new Oprec(Opcode.mov_l, "X2l,r1") }
            }),
            new Oprec(Opcode.mov_l, "r2,D1l"),
            // 2...
            new Oprec4Bits(0, new OprecBase[]
            {
                new Oprec(Opcode.mov_b, "r2,@1b"),
                new Oprec(Opcode.mov_w, "r2,@1w"),
                new Oprec(Opcode.mov_l, "r2,@1l"),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.mov_b, "r2,-1b"),
                new Oprec(Opcode.mov_w, "r2,-1w"),
                new Oprec(Opcode.mov_l, "r2,-1l"),
                new Oprec(Opcode.div0s, "r2,r1"),

                new Oprec(Opcode.tst, "r2,r1"),
                new Oprec(Opcode.and, "r2,r1"),
                new Oprec(Opcode.xor, "r2,r1"),
                new Oprec(Opcode.or, "r2,r1"),

                new Oprec(Opcode.cmp_str, "r2,r1"),
                new Oprec(Opcode.xtrct, "r2,r1"),
                new Oprec(Opcode.mulu_w, "r2,r1"),
                new Oprec(Opcode.muls_w, "r2,r1"),
            }),
            // 3...
            new Oprec4Bits(0, new OprecBase[]
            {
                new Oprec(Opcode.cmp_eq, "r2,r1"),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.cmp_hs, "r2,r1"),
                new Oprec(Opcode.cmp_ge, "r2,r1"),

                new Oprec(Opcode.div1, "r2,r1"),
                new Oprec(Opcode.dmulu_l, "r2,r1"),
                new Oprec(Opcode.cmp_hi, "r2,r1"),
                new Oprec(Opcode.cmp_gt, "r2,r1"),

                new Oprec(Opcode.sub, "r2,r1"),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.subc, "r2,r1"),
                new Oprec(Opcode.subv, "r2,r1"),

                new Oprec(Opcode.add, "r2,r1"),
                new Oprec(Opcode.dmuls_l, "r2,r1"),
                new Oprec(Opcode.addc, "r2,r1"),
                new Oprec(Opcode.addv, "r2,r1"),
            }),

            // 4...
            new OprecField(0, 8, new Dictionary<int, OprecBase>
            {
                { 0x00, new Oprec(Opcode.shll, "r1") },
                { 0x01, new Oprec(Opcode.shlr, "r1") },
                { 0x04, new Oprec(Opcode.rotl, "r1") },
                { 0x05, new Oprec(Opcode.rotr, "r1") },
                { 0x08, new Oprec(Opcode.shll2, "r1") },
                { 0x09, new Oprec(Opcode.shlr, "r1") },
                { 0x0B, new Oprec(Opcode.jsr, "@1l") },

                { 0x0C, new Oprec(Opcode.shad, "r2,r1") },
                { 0x1C, new Oprec(Opcode.shad, "r2,r1") },
                { 0x2C, new Oprec(Opcode.shad, "r2,r1") },
                { 0x3C, new Oprec(Opcode.shad, "r2,r1") },
                { 0x4C, new Oprec(Opcode.shad, "r2,r1") },
                { 0x5C, new Oprec(Opcode.shad, "r2,r1") },
                { 0x6C, new Oprec(Opcode.shad, "r2,r1") },
                { 0x7C, new Oprec(Opcode.shad, "r2,r1") },
                { 0x8C, new Oprec(Opcode.shad, "r2,r1") },
                { 0x9C, new Oprec(Opcode.shad, "r2,r1") },
                { 0xAC, new Oprec(Opcode.shad, "r2,r1") },
                { 0xBC, new Oprec(Opcode.shad, "r2,r1") },
                { 0xCC, new Oprec(Opcode.shad, "r2,r1") },
                { 0xDC, new Oprec(Opcode.shad, "r2,r1") },
                { 0xEC, new Oprec(Opcode.shad, "r2,r1") },
                { 0xFC, new Oprec(Opcode.shad, "r2,r1") },

                { 0x2A, new Oprec(Opcode.lds, "r1,RP") },

                { 0x0D, new Oprec(Opcode.shld, "r2,r1") },
                { 0x1D, new Oprec(Opcode.shld, "r2,r1") },
                { 0x2D, new Oprec(Opcode.shld, "r2,r1") },
                { 0x3D, new Oprec(Opcode.shld, "r2,r1") },
                { 0x4D, new Oprec(Opcode.shld, "r2,r1") },
                { 0x5D, new Oprec(Opcode.shld, "r2,r1") },
                { 0x6D, new Oprec(Opcode.shld, "r2,r1") },
                { 0x7D, new Oprec(Opcode.shld, "r2,r1") },
                { 0x8D, new Oprec(Opcode.shld, "r2,r1") },
                { 0x9D, new Oprec(Opcode.shld, "r2,r1") },
                { 0xAD, new Oprec(Opcode.shld, "r2,r1") },
                { 0xBD, new Oprec(Opcode.shld, "r2,r1") },
                { 0xCD, new Oprec(Opcode.shld, "r2,r1") },
                { 0xDD, new Oprec(Opcode.shld, "r2,r1") },
                { 0xED, new Oprec(Opcode.shld, "r2,r1") },
                { 0xFD, new Oprec(Opcode.shld, "r2,r1") },

                { 0x10, new Oprec(Opcode.dt, "r1") },
                { 0x11, new Oprec(Opcode.cmp_pz, "r1") },
                { 0x15, new Oprec(Opcode.cmp_pl, "r1") },
                { 0x18, new Oprec(Opcode.shll8, "r1") },
                { 0x19, new Oprec(Opcode.shlr8, "r1") },
                { 0x21, new Oprec(Opcode.shar, "r1") },
                { 0x22, new Oprec(Opcode.sts_l, "RP,-1l") },
                { 0x24, new Oprec(Opcode.rotcl, "r1") },
                { 0x25, new Oprec(Opcode.rotcr, "r1") },
                { 0x26, new Oprec(Opcode.lds_l, "+1l,RP") },
                { 0x28, new Oprec(Opcode.shll16, "r1") },
                { 0x29, new Oprec(Opcode.shlr16, "r1") },
                { 0x2B, new Oprec(Opcode.jmp, "@1l") },
            }),
            new Oprec(Opcode.mov_l, "D2l,r1"),
            // 6...
            new Oprec4Bits(0, new[]
            {
                new Oprec(Opcode.mov_b, "@2b,r1"),
                new Oprec(Opcode.mov_w, "@2w,r1"),
                new Oprec(Opcode.mov_l, "@2l,r1"),
                new Oprec(Opcode.mov, "r2,r1"),

                new Oprec(Opcode.mov_b, "+2b,r1"),
                new Oprec(Opcode.mov_w, "+2w,r1"),
                new Oprec(Opcode.mov_l, "+2l,r1"),
                new Oprec(Opcode.not, "r2,r1"),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.swap_w, "r2,r1"),
                new Oprec(Opcode.negc, "r2,r1"),
                new Oprec(Opcode.neg, "r2,r1"),

                new Oprec(Opcode.extu_b, "r2,r1"),
                new Oprec(Opcode.extu_w, "r2,r1"),
                new Oprec(Opcode.exts_b, "r2,r1"),
                new Oprec(Opcode.exts_w, "r2,r1"),
            }),
            new Oprec(Opcode.add, "I,r1"),

            // 8...
            new Oprec4Bits(8, new OprecBase[] {
                new Oprec(Opcode.mov_b, "R0,D2b"),
                new Oprec(Opcode.mov_w, "R0,D2w"),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.mov_b, "D2b,R0"),
                new Oprec(Opcode.mov_w, "D2w,R0"),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.cmp_eq, "I,R0"),
                new Oprec(Opcode.bt, "j"),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.bf, "j"),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.bt_s, "j"),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.bf_s, "j"),
            }),
            new Oprec(Opcode.mov_w, "Pw,r1"),
            new Oprec(Opcode.bra, "J"),
            new Oprec(Opcode.bsr, "J"),

            // C...
            new Oprec4Bits(8, new OprecBase[]
            {
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.mova, "Pl,R0"),

                new Oprec(Opcode.tst, "I,R0"),
                new Oprec(Opcode.and, "I,R0"),
                new Oprec(Opcode.xor, "I,R0"),
                new Oprec(Opcode.or, "I,R0"),

                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.and_b, "I,Gb"),
                new Oprec(Opcode.invalid, ""),
                new Oprec(Opcode.invalid, ""),
            }),
            new Oprec(Opcode.mov_l, "Pl,r1"),
            new Oprec(Opcode.mov, "I,r1"),
            // F...
            new OprecField(0, 5, new Dictionary<int, OprecBase>
            {
                { 0x0, new OprecField(8, 1, new Dictionary<int, OprecBase>
                    {
                        { 0, new Oprec(Opcode.fadd, "d2,d1") },
                        { 1, new Oprec(Opcode.fadd, "f2,f1") }
                    })
                },
                { 0x3, new OprecField(8, 1, new Dictionary<int, OprecBase>
                    {
                        { 0, new Oprec(Opcode.fdiv, "d2,d1") },
                        { 1, new Oprec(Opcode.fdiv, "f2,f1") }
                    })
                },
                { 0x4, new OprecField(8, 1, new Dictionary<int, OprecBase>
                    {
                        { 0, new Oprec(Opcode.fcmp_eq, "d2,d1") },
                        { 1, new Oprec(Opcode.fcmp_eq, "f2,f1") }
                    })
                },
                { 0x5, new OprecField(8, 1, new Dictionary<int, OprecBase>
                    {
                        { 0, new Oprec(Opcode.fcmp_gt, "d2,d1") },
                        { 1, new Oprec(Opcode.fcmp_gt, "f2,f1") }
                    })
                },
                { 0x9, new OprecField(8, 1, new Dictionary<int, OprecBase>
                    {
                        { 0, new Oprec(Opcode.fcmp_gt, "d2,d1") },
                        { 1, new Oprec(Opcode.fcmp_gt, "f2,f1") }
                    })
                },

                { 0xD, new OprecField(4, 5, new Dictionary<int, OprecBase>
                    {
                        { 0x08, new Oprec(Opcode.fldi0, "f1") },
                        { 0x0A, new Oprec(Opcode.fcnvsd, "FU,d1") },
                        { 0x0E, new Oprec(Opcode.fipr, "v2,v1") },
                        { 0x18, new Oprec(Opcode.fldi0, "f1") },
                        { 0x1E, new Oprec(Opcode.fipr, "v2,v1") },
                    })
                },
                { 0x14, new Oprec(Opcode.fcmp_eq, "f2,f1") },
                { 0x1D, new OprecField(4, 5, new Dictionary<int, OprecBase>
                    {
                        { 0x01, new Oprec(Opcode.flds, "f1,FU") },
                        { 0x05, new Oprec(Opcode.fabs, "d1") },
                        { 0x09, new Oprec(Opcode.fldi1, "f1") },
                        { 0x0A, new Oprec(Opcode.fcnvsd, "FU,d1") },
                        { 0x0B, new Oprec(Opcode.fcnvds, "d1,FU") },
                        { 0x11, new Oprec(Opcode.flds, "f1,FU") },
                        { 0x15, new Oprec(Opcode.fabs, "f1") },
                        { 0x19, new Oprec(Opcode.fldi1, "f1") },
                    })
                },
            })
        };
    }
}
