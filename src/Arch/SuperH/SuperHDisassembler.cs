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

    using Mutator = Func<ushort, SuperHDisassembler, bool>;

    public class SuperHDisassembler : DisassemblerBase<SuperHInstruction>
    {
        private EndianImageReader rdr;
        private DasmState state;

        public SuperHDisassembler(EndianImageReader rdr)
        {
            this.rdr = rdr;
            this.state = new DasmState();
        }

        public override SuperHInstruction DisassembleInstruction()
        {
            var addr = rdr.Address;
            if (!rdr.TryReadUInt16(out ushort uInstr))
                return null;
            var instr = decoders[uInstr >> 12].Decode(this, uInstr);
            state.Clear();
            instr.Address = addr;
            instr.Length = 2;
            return instr;
        }

        private SuperHInstruction Invalid()
        {
            return new SuperHInstruction
            {
                Opcode = Opcode.invalid
            };
        }

        public class DasmState
        {
            public List<MachineOperand> ops = new List<MachineOperand>();
            public Opcode opcode;
            public RegisterStorage reg;

            public void Clear()
            {
                ops.Clear();
                
            }

            internal SuperHInstruction MakeInstruction()
            {
                var instr =  new SuperHInstruction
                {
                    Opcode = this.opcode,
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
                        }
                    }
                }
                return instr;
            }
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
            return Invalid();
#endif
        }

        // Mutators

        private static bool r1(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> 8) & 0xF;
            dasm.state.ops.Add(new RegisterOperand(Registers.gpregs[reg]));
            return true;
        }

        private static bool r2(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> 4) & 0xF;
            dasm.state.ops.Add(new RegisterOperand(Registers.gpregs[reg]));
            return true;
        }

        private static bool r3(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = uInstr & 0xF;
            dasm.state.ops.Add(new RegisterOperand(Registers.gpregs[reg]));
            return true;
        }

        private static bool R0(ushort uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.r0));
            return true;
        }

        private static bool f1(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> 8) & 0xF;
            dasm.state.ops.Add(new RegisterOperand(Registers.fpregs[reg]));
            return true;
        }
        private static bool f2(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> 4) & 0xF;
            dasm.state.ops.Add(new RegisterOperand(Registers.fpregs[reg]));
            return true;
        }
        private static bool F0(ushort uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.fr0));
            return true;
        }

        private static bool d1(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> (1+8)) & 0x7;
            dasm.state.ops.Add(new RegisterOperand(Registers.dfpregs[reg]));
            return true;
        }
        private static bool d2(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> (1+4)) & 0x7;
            dasm.state.ops.Add(new RegisterOperand(Registers.dfpregs[reg]));
            return true;
        }

        private static bool v1(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> 10) & 0x3;
            dasm.state.ops.Add(new RegisterOperand(Registers.vfpregs[reg]));
            return true;
        }
        private static bool v2(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = (uInstr >> 8) & 0x3;
            dasm.state.ops.Add(new RegisterOperand(Registers.vfpregs[reg]));
            return true;
        }

        private static bool RP(ushort uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.pr));
            return true;
        }

        private static bool RS(ushort uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.sr));
            return true;
        }
        private static bool RG(ushort uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.gbr));
            return true;
        }
        private static bool RK(ushort uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.spc));
            return true;
        }
        private static bool RT(ushort uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.tbr));
            return true;
        }
        private static bool RV(ushort uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.vbr));
            return true;
        }
        private static bool spc(ushort uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.spc));
            return true;
        }

        private static bool mh(ushort uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.mach));
            return true;
        }
        private static bool ml(ushort uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.macl));
            return true;
        }
        private static bool FU(ushort uInstr, SuperHDisassembler dasm)
        { 
            dasm.state.ops.Add(new RegisterOperand(Registers.fpul));
            return true;
        }


        private static bool I(ushort uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(ImmediateOperand.Byte((byte)uInstr));
            return true;
        }

        private static bool j(ushort uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(AddressOperand.Create(dasm.rdr.Address + (2 + 2 * (sbyte)uInstr)));
            return true;
        }
        private static bool J(ushort uInstr, SuperHDisassembler dasm)
        {
            int offset = ((int)uInstr << 20) >> 19;
            dasm.state.ops.Add(AddressOperand.Create(dasm.rdr.Address + (2 + offset)));
            return true;
        }


        private static bool Ind1b(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.Indirect(PrimitiveType.Byte, reg));
            return true;
        }
        private static bool Ind1w(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.Indirect(PrimitiveType.Word16, reg));
            return true;
        }
        private static bool Ind1l(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.Indirect(PrimitiveType.Word32, reg));
            return true;
        }

        private static bool Ind2b(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.Indirect(PrimitiveType.Byte, reg));
            return true;
        }
        private static bool Ind2w(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.Indirect(PrimitiveType.Word16, reg));
            return true;
        }
        private static bool Ind2l(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.Indirect(PrimitiveType.Word32, reg));
            return true;
        }
        private static bool Pre1b(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPreDecr(PrimitiveType.Byte, reg));
            return true;
        }

        private static bool Pre1w(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPreDecr(PrimitiveType.Word16, reg));
            return true;
        }

        private static bool Pre1l(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPreDecr(PrimitiveType.Word32, reg));
            return true;
        }

        private static bool Post1l(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPostIncr(PrimitiveType.Word32, reg));
            return true;
        }
        private static bool Post2b(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPostIncr(PrimitiveType.Byte, reg));
            return true;
        }
        private static bool Post2w(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPostIncr(PrimitiveType.Word16, reg));
            return true;
        }
        private static bool Post2l(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0x0F];
            dasm.state.ops.Add(MemoryOperand.IndirectPostIncr(PrimitiveType.Word32, reg));
            return true;
        }

        // Indirect indexed
        private static bool X1b(ushort uInstr, SuperHDisassembler dasm)
        {
            var iReg = (uInstr >> 8) & 0xF;
            dasm.state.ops.Add(MemoryOperand.IndexedIndirect(PrimitiveType.Byte, Registers.gpregs[iReg]));
            return true;
        }
        private static bool X1w(ushort uInstr, SuperHDisassembler dasm)
        {
            var iReg = (uInstr >> 8) & 0xF;
            dasm.state.ops.Add(MemoryOperand.IndexedIndirect(PrimitiveType.Word16, Registers.gpregs[iReg]));
            return true;
        }
        private static bool X1l(ushort uInstr, SuperHDisassembler dasm)
        {
            var iReg = (uInstr >> 8) & 0xF;
            dasm.state.ops.Add(MemoryOperand.IndexedIndirect(PrimitiveType.Word32, Registers.gpregs[iReg]));
            return true;
        }
        private static bool X2b(ushort uInstr, SuperHDisassembler dasm)
        {
            var iReg = (uInstr >> 4) & 0xF;
            dasm.state.ops.Add(MemoryOperand.IndexedIndirect(PrimitiveType.Byte, Registers.gpregs[iReg]));
            return true;
        }
        private static bool X2w(ushort uInstr, SuperHDisassembler dasm)
        {
            var iReg = (uInstr >> 4) & 0xF;
            dasm.state.ops.Add(MemoryOperand.IndexedIndirect(PrimitiveType.Word16, Registers.gpregs[iReg]));
            return true;
        }
        private static bool X2l(ushort uInstr, SuperHDisassembler dasm)
        {
            var iReg = (uInstr >> 4) & 0xF;
            dasm.state.ops.Add(MemoryOperand.IndexedIndirect(PrimitiveType.Word32, Registers.gpregs[iReg]));
            return true;
        }


        // indirect with displacement
        private static bool D1l(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 8) & 0xF];
            var width = PrimitiveType.Word32;
            dasm.state.ops.Add(MemoryOperand.IndirectDisplacement(width, reg, (uInstr & 0xF) * width.Size));
            return true;
        }
        private static bool D2b(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0xF];
            var width = PrimitiveType.Byte;
            dasm.state.ops.Add(MemoryOperand.IndirectDisplacement(width, reg, (uInstr & 0xF) * width.Size));
            return true;
        }
        private static bool D2w(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0xF];
            var width = PrimitiveType.Word16;
            dasm.state.ops.Add(MemoryOperand.IndirectDisplacement(width, reg, (uInstr & 0xF) * width.Size));
            return true;
        }
        private static bool D2l(ushort uInstr, SuperHDisassembler dasm)
        {
            var reg = Registers.gpregs[(uInstr >> 4) & 0xF];
            var width = PrimitiveType.Word32;
            dasm.state.ops.Add(MemoryOperand.IndirectDisplacement(width, reg, (uInstr & 0xF) * width.Size));
            return true;
        }



        // PC-relative with displacement
        private static bool Pw(ushort uInstr, SuperHDisassembler dasm)
        {
            var width = PrimitiveType.Word16;
            dasm.state.ops.Add(MemoryOperand.PcRelativeDisplacement(width, width.Size * (byte)uInstr));
            return true;
        }
        private static bool Pl(ushort uInstr, SuperHDisassembler dasm)
        {
            var width = PrimitiveType.Word32;
            dasm.state.ops.Add(MemoryOperand.PcRelativeDisplacement(width, width.Size * (byte)uInstr));
            return true;
        }


        private static bool Gb(ushort uInstr, SuperHDisassembler dasm)
        {
            dasm.state.ops.Add(MemoryOperand.GbrIndexedIndirect(PrimitiveType.Byte));
            return true;
        }

        // Factory methods

        private static InstrDecoder Instr(Opcode opcode, params Mutator[] mutators)
        {
            return new InstrDecoder(opcode, mutators);
        }

        // Decoders

        private abstract class Decoder
        {
            public abstract SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr);
        }

        private class InstrDecoder  : Decoder
        {
            private readonly Opcode opcode;
            private readonly Mutator[] mutators;

            public InstrDecoder(Opcode opcode, Mutator[] mutators)
            {
                this.opcode = opcode;
                this.mutators = mutators;
            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                dasm.state.opcode = this.opcode;
                foreach (var mutator in this.mutators)
                {
                    if (!mutator(uInstr, dasm))
                        return dasm.Invalid();
                }
                return dasm.state.MakeInstruction();
            }
        }

        private class Oprec4Bits : Decoder
        {
            private readonly int shift;
            private readonly Decoder[] oprecs;

            public Oprec4Bits(int shift, Decoder[] oprecs)
            {
                this.shift = shift;
                this.oprecs = oprecs;
            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                return oprecs[(uInstr >> shift) & 0xF].Decode(dasm, uInstr);
            }
        }

        private class OprecField : Decoder
        {
            private readonly int shift;
            private readonly int bitcount;
            private Dictionary<int, Decoder> oprecs;

            public OprecField(int shift, int bitcount, Dictionary<int, Decoder> oprecs)
            {
                this.shift = shift;
                this.bitcount = bitcount;
                this.oprecs = oprecs;
            }

            public override SuperHInstruction Decode(SuperHDisassembler dasm, ushort uInstr)
            {
                var mask = (1 << bitcount) - 1;
                if (!oprecs.TryGetValue((uInstr >> shift) & mask, out Decoder or))
                    return dasm.Invalid(uInstr);
                return or.Decode(dasm, uInstr);
            }
        }

        private static Decoder[] decoders = new Decoder[]
        {
            // 0...
            new OprecField(0, 4, new Dictionary<int, Decoder>
            {
                { 0x0, Instr(Opcode.invalid) },
                { 0x1, Instr(Opcode.invalid) },
                { 0x02, new OprecField(4, 4, new Dictionary<int, Decoder>
                    {
                        { 0x0, Instr(Opcode.stc, RS,r2) },
                        { 0x1, Instr(Opcode.stc, RG,r2) },
                        { 0x2, Instr(Opcode.stc, RG,r2) },
                        { 0x4, Instr(Opcode.stc, RK,r2) }
                    })
                },
                { 0x03, new OprecField(4, 4, new Dictionary<int, Decoder>
                    {
                        { 0x0, Instr(Opcode.bsrf, r1) },
                        { 0x2, Instr(Opcode.braf, r1) },
                        { 0x9, Instr(Opcode.ocbi, Ind1b) },
                    })
                },
                { 0x4, Instr(Opcode.mov_b, r2,X1b) },
                { 0x5, Instr(Opcode.mov_w, r2,X1w) },
                { 0x6, Instr(Opcode.mov_l, r2,X1l) },
                { 0x7, Instr(Opcode.mul_l, r2,r1) },
                { 0x8, new OprecField(8, 4, new Dictionary<int, Decoder> {
                        { 0x0, new OprecField(4, 4, new Dictionary<int, Decoder>
                            {
                                { 0x0, Instr(Opcode.clrt) },
                                { 0x1, Instr(Opcode.sett) },
                                { 0x2, Instr(Opcode.clrmac) },
                                { 0x3, Instr(Opcode.ldtlb) },
                                { 0x4, Instr(Opcode.clrs) },
                                { 0xC, Instr(Opcode.invalid) },
                            })
                        }
                    })
                },
                { 0x9, new OprecField(4, 4, new Dictionary<int, Decoder>
                    {
                        { 0x0, Instr(Opcode.nop) },
                        { 0x1, Instr(Opcode.div0u) },
                        { 0x2, Instr(Opcode.movt, r1) },
                    })
                },
                { 0xA, new OprecField(4, 4, new Dictionary<int, Decoder>
                    {
                        { 0x0, Instr(Opcode.sts, mh,r1) },
                        { 0x1, Instr(Opcode.sts, ml,r1) },
                        { 0x4, Instr(Opcode.sts, RT,r1) },
                        { 0x5, Instr(Opcode.sts, FU,r1) },
                    })
                },
                { 0xB, new OprecField(4, 4, new Dictionary<int, Decoder>
                    {
                        { 0x0, Instr(Opcode.rts) },
                        { 0x3, Instr(Opcode.brk) },
                    })
                },
                { 0xC, Instr(Opcode.mov_b, X2b,r1) },
                { 0xD, Instr(Opcode.mov_w, X2w,r1) },
                { 0xE, Instr(Opcode.mov_l, X2l,r1) },
                { 0xF, Instr(Opcode.mac_l, Post2l,Post1l) }
            }),
            Instr(Opcode.mov_l, r2,D1l),
            // 2...
            new Oprec4Bits(0, new Decoder[]
            {
                Instr(Opcode.mov_b, r2,Ind1b),
                Instr(Opcode.mov_w, r2,Ind1w),
                Instr(Opcode.mov_l, r2,Ind1l),
                Instr(Opcode.invalid),

                Instr(Opcode.mov_b, r2,Pre1b),
                Instr(Opcode.mov_w, r2,Pre1w),
                Instr(Opcode.mov_l, r2,Pre1l),
                Instr(Opcode.div0s, r2,r1),

                Instr(Opcode.tst, r2,r1),
                Instr(Opcode.and, r2,r1),
                Instr(Opcode.xor, r2,r1),
                Instr(Opcode.or, r2,r1),

                Instr(Opcode.cmp_str, r2,r1),
                Instr(Opcode.xtrct, r2,r1),
                Instr(Opcode.mulu_w, r2,r1),
                Instr(Opcode.muls_w, r2,r1),
            }),
            // 3...
            new Oprec4Bits(0, new Decoder[]
            {
                Instr(Opcode.cmp_eq, r2,r1),
                Instr(Opcode.invalid),
                Instr(Opcode.cmp_hs, r2,r1),
                Instr(Opcode.cmp_ge, r2,r1),

                Instr(Opcode.div1, r2,r1),
                Instr(Opcode.dmulu_l, r2,r1),
                Instr(Opcode.cmp_hi, r2,r1),
                Instr(Opcode.cmp_gt, r2,r1),

                Instr(Opcode.sub, r2,r1),
                Instr(Opcode.invalid),
                Instr(Opcode.subc, r2,r1),
                Instr(Opcode.subv, r2,r1),

                Instr(Opcode.add, r2,r1),
                Instr(Opcode.dmuls_l, r2,r1),
                Instr(Opcode.addc, r2,r1),
                Instr(Opcode.addv, r2,r1),
            }),

            // 4...
            new OprecField(0, 8, new Dictionary<int, Decoder>
            {
                { 0x00, Instr(Opcode.shll, r1) },
                { 0x01, Instr(Opcode.shlr, r1) },
                { 0x04, Instr(Opcode.rotl, r1) },
                { 0x05, Instr(Opcode.rotr, r1) },
                { 0x06, Instr(Opcode.lds_l, Post1l,mh) },
                { 0x08, Instr(Opcode.shll2, r1) },
                { 0x09, Instr(Opcode.shlr, r1) },
                { 0x0B, Instr(Opcode.jsr, Ind1l) },
                { 0x0C, Instr(Opcode.shad, r2,r1) },
                { 0x0E, Instr(Opcode.ldc, r1,RS) },
                { 0x1C, Instr(Opcode.shad, r2,r1) },
                { 0x2C, Instr(Opcode.shad, r2,r1) },
                { 0x2E, Instr(Opcode.ldc, r1,RV) },
                { 0x3C, Instr(Opcode.shad, r2,r1) },
                { 0x4C, Instr(Opcode.shad, r2,r1) },
                { 0x5C, Instr(Opcode.shad, r2,r1) },
                { 0x6C, Instr(Opcode.shad, r2,r1) },
                { 0x7C, Instr(Opcode.shad, r2,r1) },
                { 0x8C, Instr(Opcode.shad, r2,r1) },
                { 0x9C, Instr(Opcode.shad, r2,r1) },
                { 0xAC, Instr(Opcode.shad, r2,r1) },
                { 0xBC, Instr(Opcode.shad, r2,r1) },
                { 0xCC, Instr(Opcode.shad, r2,r1) },
                { 0xDC, Instr(Opcode.shad, r2,r1) },
                { 0xEC, Instr(Opcode.shad, r2,r1) },
                { 0xFC, Instr(Opcode.shad, r2,r1) },

                { 0x2A, Instr(Opcode.lds, r1,RP) },

                { 0x0D, Instr(Opcode.shld, r2,r1) },
                { 0x1D, Instr(Opcode.shld, r2,r1) },
                { 0x2D, Instr(Opcode.shld, r2,r1) },
                { 0x3D, Instr(Opcode.shld, r2,r1) },
                { 0x4D, Instr(Opcode.shld, r2,r1) },
                { 0x5D, Instr(Opcode.shld, r2,r1) },
                { 0x6D, Instr(Opcode.shld, r2,r1) },
                { 0x7D, Instr(Opcode.shld, r2,r1) },
                { 0x8D, Instr(Opcode.shld, r2,r1) },
                { 0x9D, Instr(Opcode.shld, r2,r1) },
                { 0xAD, Instr(Opcode.shld, r2,r1) },
                { 0xBD, Instr(Opcode.shld, r2,r1) },
                { 0xCD, Instr(Opcode.shld, r2,r1) },
                { 0xDD, Instr(Opcode.shld, r2,r1) },
                { 0xED, Instr(Opcode.shld, r2,r1) },
                { 0xFD, Instr(Opcode.shld, r2,r1) },

                { 0x10, Instr(Opcode.dt, r1) },
                { 0x11, Instr(Opcode.cmp_pz, r1) },
                { 0x15, Instr(Opcode.cmp_pl, r1) },
                { 0x18, Instr(Opcode.shll8, r1) },
                { 0x19, Instr(Opcode.shlr8, r1) },
                { 0x21, Instr(Opcode.shar, r1) },
                { 0x22, Instr(Opcode.sts_l, RP,Pre1l) },
                { 0x24, Instr(Opcode.rotcl, r1) },
                { 0x25, Instr(Opcode.rotcr, r1) },
                { 0x26, Instr(Opcode.lds_l, Post1l,RP) },
                { 0x28, Instr(Opcode.shll16, r1) },
                { 0x29, Instr(Opcode.shlr16, r1) },
                { 0x2B, Instr(Opcode.jmp, Ind1l) },
                { 0x43, Instr(Opcode.stc_l, spc,r1) }
            }),
            Instr(Opcode.mov_l, D2l,r1),
            // 6...
            new Oprec4Bits(0, new Decoder[]
            {
                Instr(Opcode.mov_b, Ind2b,r1),
                Instr(Opcode.mov_w, Ind2w,r1),
                Instr(Opcode.mov_l, Ind2l,r1),
                Instr(Opcode.mov, r2,r1),

                Instr(Opcode.mov_b, Post2b,r1),
                Instr(Opcode.mov_w, Post2w,r1),
                Instr(Opcode.mov_l, Post2l,r1),
                Instr(Opcode.not, r2,r1),

                Instr(Opcode.invalid),
                Instr(Opcode.swap_w, r2,r1),
                Instr(Opcode.negc, r2,r1),
                Instr(Opcode.neg, r2,r1),

                Instr(Opcode.extu_b, r2,r1),
                Instr(Opcode.extu_w, r2,r1),
                Instr(Opcode.exts_b, r2,r1),
                Instr(Opcode.exts_w, r2,r1),
            }),
            Instr(Opcode.add, I,r1),

            // 8...
            new Oprec4Bits(8, new Decoder[] {
                Instr(Opcode.mov_b, R0,D2b),
                Instr(Opcode.mov_w, R0,D2w),
                Instr(Opcode.invalid),
                Instr(Opcode.invalid),

                Instr(Opcode.mov_b, D2b,R0),
                Instr(Opcode.mov_w, D2w,R0),
                Instr(Opcode.invalid),
                Instr(Opcode.invalid),

                Instr(Opcode.cmp_eq, I,R0),
                Instr(Opcode.bt, j),
                Instr(Opcode.invalid),
                Instr(Opcode.bf, j),

                Instr(Opcode.invalid),
                Instr(Opcode.bt_s, j),
                Instr(Opcode.invalid),
                Instr(Opcode.bf_s, j),
            }),
            Instr(Opcode.mov_w, Pw,r1),
            Instr(Opcode.bra, J),
            Instr(Opcode.bsr, J),

            // C...
            new Oprec4Bits(8, new Decoder[]
            {
                Instr(Opcode.invalid),
                Instr(Opcode.invalid),
                Instr(Opcode.invalid),
                Instr(Opcode.invalid),

                Instr(Opcode.invalid),
                Instr(Opcode.invalid),
                Instr(Opcode.invalid),
                Instr(Opcode.mova, Pl,R0),

                Instr(Opcode.tst, I,R0),
                Instr(Opcode.and, I,R0),
                Instr(Opcode.xor, I,R0),
                Instr(Opcode.or, I,R0),

                Instr(Opcode.invalid),
                Instr(Opcode.and_b, I,Gb),
                Instr(Opcode.invalid),
                Instr(Opcode.invalid),
            }),
            Instr(Opcode.mov_l, Pl,r1),
            Instr(Opcode.mov, I,r1),
            // F...
            new OprecField(0, 5, new Dictionary<int, Decoder>
            {
                { 0x0, new OprecField(8, 1, new Dictionary<int, Decoder>
                    {
                        { 0, Instr(Opcode.fadd, d2,d1) },
                        { 1, Instr(Opcode.fadd, f2,f1) }
                    })
                },
                { 0x3, new OprecField(8, 1, new Dictionary<int, Decoder>
                    {
                        { 0, Instr(Opcode.fdiv, d2,d1) },
                        { 1, Instr(Opcode.fdiv, f2,f1) }
                    })
                },
                { 0x4, new OprecField(8, 1, new Dictionary<int, Decoder>
                    {
                        { 0, Instr(Opcode.fcmp_eq, d2,d1) },
                        { 1, Instr(Opcode.fcmp_eq, f2,f1) }
                    })
                },
                { 0x5, new OprecField(8, 1, new Dictionary<int, Decoder>
                    {
                        { 0, Instr(Opcode.fcmp_gt, d2,d1) },
                        { 1, Instr(Opcode.fcmp_gt, f2,f1) }
                    })
                },
                { 0x9, new OprecField(8, 1, new Dictionary<int, Decoder>
                    {
                        { 0, Instr(Opcode.fcmp_gt, d2,d1) },
                        { 1, Instr(Opcode.fcmp_gt, f2,f1) }
                    })
                },

                { 0xD, new OprecField(4, 5, new Dictionary<int, Decoder>
                    {
                        { 0x08, Instr(Opcode.fldi0, f1) },
                        { 0x0A, Instr(Opcode.fcnvsd, FU,d1) },
                        { 0x0E, Instr(Opcode.fipr, v2,v1) },
                        { 0x18, Instr(Opcode.fldi0, f1) },
                        { 0x1E, Instr(Opcode.fipr, v2,v1) },
                    })
                },
                { 0x0E, Instr(Opcode.fmac, F0,f2,f1) },
                { 0x0F, Instr(Opcode.invalid) },
                { 0x14, Instr(Opcode.fcmp_eq, f2,f1) },
                { 0x1D, new OprecField(4, 5, new Dictionary<int, Decoder>
                    {
                        { 0x01, Instr(Opcode.flds, f1,FU) },
                        { 0x05, Instr(Opcode.fabs, d1) },
                        { 0x09, Instr(Opcode.fldi1, f1) },
                        { 0x0A, Instr(Opcode.fcnvsd, FU,d1) },
                        { 0x0B, Instr(Opcode.fcnvds, d1,FU) },
                        { 0x11, Instr(Opcode.flds, f1,FU) },
                        { 0x15, Instr(Opcode.fabs, f1) },
                        { 0x19, Instr(Opcode.fldi1, f1) },
                    })
                },
                { 0x1E, Instr(Opcode.fmac, F0,f2,f1) },
                { 0x1F, Instr(Opcode.invalid) },
            })
        };
    }
}
