#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Arch.Mips.Machine;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Mips.Disassembler
{
    using Decoder = Decoder<MipsDisassembler, Mnemonic, MipsInstruction>;

    public partial class MipsDisassembler
    {
        public class DecoderFactory
        {
            protected readonly Decoder<MipsDisassembler, Mnemonic, MipsInstruction> invalid;

            public DecoderFactory()
            {
                this.invalid = Instr(Mnemonic.illegal);
            }

            public static DecoderFactory Create(string? isa)
            {
                switch (isa)
                {
                case "v6": return new V6DecoderFactory();
                case "ps2ee": return new Ps2EeDecoderFactory();
                default: return new DecoderFactory();
                }
            }


            internal static Mutator<MipsDisassembler> R(int offset)
            {
                return (u, d) =>
                {
                    var op = d.Reg(u >> offset);
                    d.ops.Add(op);
                    return true;
                };
            }
            internal static readonly Mutator<MipsDisassembler> R1 = R(21);
            internal static readonly Mutator<MipsDisassembler> R2 = R(16); 
            internal static readonly Mutator<MipsDisassembler> R3 = R(11);
            internal static readonly Mutator<MipsDisassembler> R4 = R(6);

            // Floating point register
            internal static Mutator<MipsDisassembler> F(int offset)
            {
                return (u, d) =>
                {
                    var op = d.FReg(u >> offset);
                    d.ops.Add(op);
                    return true;
                };
            }
            internal static readonly Mutator<MipsDisassembler> F1 = F(21);
            internal static readonly Mutator<MipsDisassembler> F2 = F(16);
            internal static readonly Mutator<MipsDisassembler> F3 = F(11);
            internal static readonly Mutator<MipsDisassembler> F4 = F(6);

            // Floating point register (even part of register pair)
            internal static Mutator<MipsDisassembler> FP(int offset)
            {
                return (u, d) =>
                {
                    var iReg = (u >> offset) & 0x1F;
                    if ((iReg & 1) != 0)
                        return false;
                    var op = d.FReg(iReg);
                    d.ops.Add(op);
                    return true;
                };
            }
            internal static readonly Mutator<MipsDisassembler> FP1 = FP(21);
            internal static readonly Mutator<MipsDisassembler> FP2 = FP(16);
            internal static readonly Mutator<MipsDisassembler> FP3 = FP(11);
            internal static readonly Mutator<MipsDisassembler> FP4 = FP(6);


            // FPU control register
            internal static Mutator<MipsDisassembler> Fcreg(int offset)
            {
                return (u, d) =>
                {
                    if (!d.TryGetFCReg(u >> offset, out RegisterStorage fcreg))
                        return false;
                    d.ops.Add(fcreg);
                    return true;
                };
            }
            internal static readonly Mutator<MipsDisassembler> f1 = Fcreg(21);
            internal static readonly Mutator<MipsDisassembler> f2 = Fcreg(16);
            internal static readonly Mutator<MipsDisassembler> f3 = Fcreg(11);

            /// <summary>
            /// Signed 16-bit immedate from the low 16 bits of the instruction word.
            /// </summary>
            internal static bool SImm(uint wInstr, MipsDisassembler dasm)
            {
                var op = Constant.Create(dasm.signedWord, (short) wInstr);
                dasm.ops.Add(op);
                return true;
            }

            private static Bitfield bf6_5 = new Bitfield(6, 5);
            internal static bool SImm6_5(uint uInstr, MipsDisassembler dasm)
            {
                var imm = bf6_5.ReadSigned(uInstr);
                var op = Constant.Create(dasm.signedWord, imm);
                dasm.ops.Add(op);
                return true;
            }

            /// <summary>
            /// Unsigned 16-bit immedate from the low 16 bits of the instruction word.
            /// </summary>
            internal static bool U(uint wInstr, MipsDisassembler dasm)
            {
                var op = Constant.Create(dasm.arch.WordWidth, (ushort)wInstr);
                dasm.ops.Add(op);
                return true;
            }

            /// <summary>
            /// Signed 16-bit immediate.
            /// </summary>
            internal static bool i(uint wInstr, MipsDisassembler dasm)
            {
                var op = Constant.Int16((short) wInstr);
                dasm.ops.Add(op);
                return true;
            }

            private static Mutator<MipsDisassembler> Imm(PrimitiveType dt, int bitPos, int bitlen)
            {
                var field = new Bitfield(bitPos, bitlen);
                return (u, d) =>
                {
                    var n = field.Read(u);
                    d.ops.Add(Constant.Create(dt, n));
                    return true;
                };
            }

            /// <summary>
            /// 16-bit PC relative jump.
            /// </summary>
            internal static bool j(uint wInstr, MipsDisassembler dasm)
            {
                var op = dasm.RelativeBranch(wInstr);
                dasm.ops.Add(op);
                return true;
            }

            internal static bool J(uint wInstr, MipsDisassembler dasm)
            {
                var op = dasm.LargeBranch(wInstr);
                dasm.ops.Add(op);
                return true;
            }

            internal static bool B(uint wInstr, MipsDisassembler dasm)
            {
                var op = Constant.Word32((wInstr >> 6) & 0xFFFFF);
                dasm.ops.Add(op);
                return true;
            }

            // Shift amount or sync type
            internal static bool s(uint wInstr, MipsDisassembler dasm)
            {
                var op = Constant.Byte((byte) ((wInstr >> 6) & 0x1F));
                dasm.ops.Add(op);
                return true;
            }

            // effective address w 16-bit offset
            internal static Mutator<MipsDisassembler> E(PrimitiveType size)
            {
                return (u, d) =>
                {
                    var op = d.Ea(u, size, 21, (short) u);
                    d.ops.Add(op);
                    return true;
                };
            }
            internal static readonly Mutator<MipsDisassembler> Eb = E(PrimitiveType.Byte);
            internal static readonly Mutator<MipsDisassembler> EB = E(PrimitiveType.SByte);
            internal static readonly Mutator<MipsDisassembler> Eh = E(PrimitiveType.Word16);
            internal static readonly Mutator<MipsDisassembler> EH = E(PrimitiveType.Int16);
            internal static readonly Mutator<MipsDisassembler> Ew = E(PrimitiveType.Word32);
            internal static readonly Mutator<MipsDisassembler> EW = E(PrimitiveType.Int32);
            internal static readonly Mutator<MipsDisassembler> El = E(PrimitiveType.Word64);
            internal static readonly Mutator<MipsDisassembler> EL = E(PrimitiveType.Int64);
            internal static readonly Mutator<MipsDisassembler> Eq = E(PrimitiveType.Word128);

            // effective address w 9-bit offset
            internal static Mutator<MipsDisassembler> e(PrimitiveType size)
            {
                return (u, d) =>
                {
                    var op = d.Ea(u, size, 21, (short) (((short) u) >> 7));
                    d.ops.Add(op);
                    return true;
                };
            }
            internal static readonly Mutator<MipsDisassembler> ew = e(PrimitiveType.Word32);
            internal static readonly Mutator<MipsDisassembler> el = e(PrimitiveType.Word64);

            // effective address w 11-bit offset
            internal static Mutator<MipsDisassembler> E11(PrimitiveType size)
            {
                var offsetField = new Bitfield(0, 11);
                return (u, d) =>
                {
                    var offset = (short) offsetField.ReadSigned(u);
                    var op = d.Ea(u, size, 11, offset);
                    d.ops.Add(op);
                    return true;
                };
            }
            internal static readonly Mutator<MipsDisassembler> E11w = E11(PrimitiveType.Word32);


            // Indexed memory address
            private static Mutator<MipsDisassembler> Mx(PrimitiveType dt, int posBase, int posIdx)
            {
                var baseField = new Bitfield(posBase, 5);
                var idxField = new Bitfield(posIdx, 5);
                return (u, d) =>
                {
                    var iBase = (int) baseField.Read(u);
                    var iIndex = (int) idxField.Read(u);
                    var rBase = d.arch.GetRegister(iBase)!;
                    var rIndex = d.arch.GetRegister(iIndex)!;
                    d.ops.Add(MemoryOperand.Indexed(dt, rBase, rIndex));
                    return true;
                };
            }
            private static readonly Mutator<MipsDisassembler> Mxbu = Mx(PrimitiveType.Byte, 21, 16);
            private static readonly Mutator<MipsDisassembler> Mxh = Mx(PrimitiveType.Word16, 21, 16);
            private static readonly Mutator<MipsDisassembler> Mxw = Mx(PrimitiveType.Word32, 21, 16);
            private static readonly Mutator<MipsDisassembler> Mxd = Mx(PrimitiveType.Word64, 21, 16);

            // trap code
            internal static bool T(uint wInstr, MipsDisassembler dasm)
            {
                var op = Constant.Word16((ushort) ((wInstr >> 6) & 0x03FF));
                dasm.ops.Add(op);
                return true;
            }

            // condition code
            internal static Mutator<MipsDisassembler> c(int bitPos)
            {
                return (u, d) =>
                {
                    var op = d.CCodeFlag(u, bitPos);
                    d.ops.Add(op);
                    return true;
                };
            }
            internal static readonly Mutator<MipsDisassembler> c8 = c(8);
            internal static readonly Mutator<MipsDisassembler> c18 = c(18);

            // FPU condition code
            internal static Mutator<MipsDisassembler> C(int bitPos)
            {
                return (u, d) =>
                {
                    var op = d.FpuCCodeFlag(u, bitPos);
                    d.ops.Add(op);
                    return true;
                };
            }
            internal static Mutator<MipsDisassembler> C18 = C(18);

            // hardware register, see instruction rdhwr
            internal static bool H(uint wInstr, MipsDisassembler dasm)
            {
                var op = Constant.Byte((byte) ((wInstr >> 11) & 0x1f));
                dasm.ops.Add(op);
                return true;
            }

            internal static Mutator<MipsDisassembler> x(string message)
            {
                return (u, d) =>
                {
                    d.NotYetImplemented(message);
                    return false;
                };
            }

            protected static NyiDecoder<MipsDisassembler, Mnemonic, MipsInstruction> Nyi(string message)
            {
                return new NyiDecoder<MipsDisassembler, Mnemonic, MipsInstruction>(message);
            }

            protected static Decoder Instr(Mnemonic mnemonic, params Mutator<MipsDisassembler>[] mutators)
            {
                return new InstrDecoder<MipsDisassembler, Mnemonic, MipsInstruction>(InstrClass.Linear, mnemonic, mutators);
            }

            protected static Decoder Instr(InstrClass iclass, Mnemonic mnemonic, params Mutator<MipsDisassembler>[] mutators)
            {
                return new InstrDecoder<MipsDisassembler, Mnemonic, MipsInstruction>(iclass, mnemonic, mutators);
            }

            protected virtual Decoder Instr64(Mnemonic mnemonic, params Mutator<MipsDisassembler>[] mutators)
            {
                return new A64Decoder(mnemonic, mutators);
            }

            // Model-specific decoders
            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Special3_Decode36()
            {
                return Instr(InstrClass.Invalid, Mnemonic.illegal);
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Special3_Decode37()
            {
                return invalid;
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeCop0()
            {
                var cop0_C0_decoder = Sparse(0, 6, "COP0 C0",
                    invalid,
                    (0x01, Instr(PRIV, Mnemonic.tlbr)),
                    (0x02, Instr(PRIV, Mnemonic.tlbwi)),
                    (0x06, Instr(PRIV, Mnemonic.tlbwr)),
                    (0x08, Instr(PRIV, Mnemonic.tlbp)),
                    (0x18, Instr(InstrClass.Return | InstrClass.Privileged, Mnemonic.eret)),
                    (0x20, Instr(PRIV, Mnemonic.wait)));

                var cop0 = Mask(21, 5, "Coprocessor",
                    Instr(PRIV, Mnemonic.mfc0, R2, R3),
                    Instr(PRIV, Mnemonic.dmfc0, R2, R3),
                    invalid,
                    invalid,
                    Instr(Mnemonic.mtc0, R2, R3),
                    Instr(Mnemonic.dmtc0, R2, R3),
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,

                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder,
                    cop0_C0_decoder);
                return cop0;
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeCop1x()
            {
                var cop1x = Mask(0, 6,
                    Instr(Mnemonic.lwxc1, F3, Mxw),
                    Instr(Mnemonic.ldxc1, F3, Mxd),
                    invalid,
                    invalid,

                    invalid,
                    Instr(Mnemonic.luxc1, F4, Mxw),
                    invalid,
                    invalid,

                    Instr(Mnemonic.swxc1, F3, Mxw),
                    Instr(Mnemonic.sdxc1, F3, Mxd),
                    invalid,
                    invalid,

                    invalid,
                    Instr(Mnemonic.suxc1, F4, Mxw),
                    invalid,
                    Instr(Mnemonic.prefx, Imm(PrimitiveType.Byte, 11, 5), Mxw),

                    // 10
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    Instr(Mnemonic.alnv_ps, F4, F3, F2, R1),
                    invalid,
                    // 20
                    Instr(Mnemonic.madd_s, F4, F1, F3, F2),
                    Instr(Mnemonic.madd_d, F4, F1, F3, F2),
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    Instr(Mnemonic.madd_ps, F4, F1, F3, F2),
                    invalid,

                    Instr(Mnemonic.msub_s, F4, F1, F3, F2),
                    Instr(Mnemonic.msub_d, F4, F1, F3, F2),
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    Instr(Mnemonic.msub_ps, F4, F1, F3, F2),
                    invalid,
                    // 30
                    Instr(Mnemonic.nmadd_s, F4, F1, F3, F2),
                    Instr(Mnemonic.nmadd_d, F4, F1, F3, F2),
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    Instr(Mnemonic.nmadd_ps, F4, F1, F3, F2),
                    invalid,

                    Instr(Mnemonic.nmsub_s, F4, F1, F3, F2),
                    Instr(Mnemonic.nmsub_d, F4, F1, F3, F2),
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    Instr(Mnemonic.nmsub_ps, F4, F1, F3, F2),
                    invalid);
                return cop1x;
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeCop2()
            {
                return invalid;
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode1C()
            {
                var special2 = Sparse(0, 6, "Special2",
                    invalid,
                    (0x0, Instr(Mnemonic.madd, R1, R2)),
                    (0x1, Instr(Mnemonic.maddu, R1, R2)),
                    (0x2, Instr(Mnemonic.mul, R3, R1, R2)),
                    (0x4, Instr(Mnemonic.msub, R1, R2)),
                    (0x5, Instr(Mnemonic.msubu, R1, R2)),

                    (0x20, Instr(Mnemonic.clz, R3, R1)),
                    (0x21, Instr(Mnemonic.clo, R3, R1)),
                    (0x3F, Instr(Mnemonic.sdbbp, Imm(PrimitiveType.UInt32, 6, 20))));
                return special2;
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode1D()
            {
                return Instr(CTD, Mnemonic.jalx, J);
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode1E()
            {
                return invalid;
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode1F()
            {
                var bshfl = Mask(6, 5,
                    invalid,
                    invalid,
                    Instr(Mnemonic.wsbh, R3, R2),
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    // 10
                    Instr(Mnemonic.seb, R3, R2),
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    Instr(Mnemonic.seh, R3, R2),
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid);

                var special3 = Mask(0, 6, "Special3",
                    // 00
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    // 10
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    // 20
                    bshfl,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    // 30
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    this.Special3_Decode36(),
                    this.Special3_Decode37(),

                    invalid,
                    invalid,
                    invalid,
                    Instr(Mnemonic.rdhwr, R2, H),
                    invalid,
                    invalid,
                    invalid,
                    invalid);

                return special3;
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode2F()
            {
                return Instr(PRIV, Mnemonic.cache, Imm(PrimitiveType.Byte, 16, 5), Ew);
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode30()
            {
                return Instr(Mnemonic.ll, R2, Ew);
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode32()
            {
                return Instr(Mnemonic.lwc2, R2, El);
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode34()
            {
                return Instr64(Mnemonic.lld, R2, El);
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode36()
            {
                return Instr(Mnemonic.ldc2, R2, El);
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode38()
            {
                return Instr(Mnemonic.sc, R2, Ew);
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode3A()
            {
                return Instr(Mnemonic.swc2, R2, Ew);
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode3B()
            {
                return invalid;
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode3C()
            {
                return Instr64(Mnemonic.scd, R2, El);
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode3E()
            {
                return Instr(Mnemonic.sdc2, R2, El);
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeCop1_8()
            {
                return Mask(16, 1,
                       Instr(InstrClass.CondJump | InstrClass.Delay, Mnemonic.bc1f, c18, j),
                       Instr(InstrClass.CondJump | InstrClass.Delay, Mnemonic.bc1t, c18, j));
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeCop1_S()
            {
                var cop1_s = Mask(0, 6, "FPU (single)",
                    Instr(Mnemonic.add_s, F4, F3, F2),
                    Instr(Mnemonic.sub_s, F4, F3, F2),
                    Instr(Mnemonic.mul_s, F4, F3, F2),
                    Instr(Mnemonic.div_s, F4, F3, F2),
                    invalid,
                    invalid,
                    Instr(Mnemonic.mov_s, F4, F3),
                    Instr(Mnemonic.neg_s, F4, F3),

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    Instr(Mnemonic.c_eq_s, c8, F3, F2),
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    Instr(Mnemonic.c_lt_s, c8, F3, F2),
                    invalid,
                    Instr(Mnemonic.c_le_s, c8, F3, F2),
                    invalid);
                return cop1_s;
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeWclass()
            {
                var cop1_w = Mask(0, 6, "FPU (word)",
                    // fn 00
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    // fn 10
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    // fn 20
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    // fn 30
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    Instr(Mnemonic.c_lt_d, c8, F3, F2),
                    invalid,
                    invalid,
                    invalid);
                return cop1_w;
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeRegimm18()
            {
                return invalid;
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeRegimm19()
            {
                return invalid;
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial28()
            {
                return invalid;
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial29()
            {
                return invalid;
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial2C()
            {
                return Instr64(Mnemonic.dadd, R3, R1, R2);
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial2D()
            {
                return Instr64(Mnemonic.daddu, R3, R1, R2);
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial2E()
            {
                return Instr64(Mnemonic.dsub, R3, R1, R2);
            }

            protected virtual Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial2F()
            {
                return Instr64(Mnemonic.dsubu, R3, R1, R2);
            }

            public Decoder CreateRootDecoder()
            {
                var cop1_d = Mask(0, 6, "FPU (double)",
                    // fn 00
                    Instr(Mnemonic.add_d, FP4, FP3, FP2),
                    Instr(Mnemonic.sub_d, FP4, FP3, FP2),
                    Instr(Mnemonic.mul_d, FP4, FP3, FP2),
                    Instr(Mnemonic.div_d, FP4, FP3, FP2),
                    invalid,
                    invalid,
                    Instr(Mnemonic.mov_d, F4, F3),
                    Instr(Mnemonic.neg_d, F4, F3),

                    invalid,
                    Instr(Mnemonic.trunc_l_d, F4, F3),
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    // fn 10
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    // fn 20
                    Instr(Mnemonic.cvt_s_d, F4, F3),
                    invalid,
                    invalid,
                    invalid,
                    Instr(Mnemonic.cvt_w_d, F4, F3),
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    // fn 30
                    invalid,
                    invalid,
                    Instr(Mnemonic.c_eq_d, c8, F3, F2),
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    Instr(Mnemonic.c_lt_d, c8, F3, F2),
                    invalid,
                    Instr(Mnemonic.c_le_d, c8, F3, F2),
                    invalid);


                var cop1_l = Mask(0, 6, "FPU (dword)",
                    // fn 00
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    // fn 10
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    // fn 20
                    Instr(Mnemonic.cvt_s_l, F4, F3),
                    Instr(Mnemonic.cvt_d_l, F4, F3),
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    // fn 30
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid);


                var cop1 = Mask(21, 5, "COP1",
                    Instr(Mnemonic.mfc1, R2, F3),
                    Instr64(Mnemonic.dmfc1, R2, F3),
                    Instr(Mnemonic.cfc1, R2, f3),
                    invalid,
                    Instr(Mnemonic.mtc1, R2, F3),
                    Instr64(Mnemonic.dmtc1, R2, F3),
                    Instr(Mnemonic.ctc1, R2, f3),
                    invalid,

                    DecodeCop1_8(),
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    DecodeCop1_S(),
                    cop1_d,
                    invalid,
                    invalid,
                    DecodeWclass(),
                    cop1_l,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid);

                var condDecoders = Mask(16, 5, "CondDecoders",
                    Instr(DCT, Mnemonic.bltz, R1, j),
                    Instr(DCT, Mnemonic.bgez, R1, j),
                    Instr(DCT, Mnemonic.bltzl, R1, j),
                    Instr(DCT, Mnemonic.bgezl, R1, j),

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    Instr(CTD, Mnemonic.tgei, R1, SImm),
                    Instr(CTD, Mnemonic.tgeiu, R1, SImm),
                    Instr(CTD, Mnemonic.tlti, R1, SImm),
                    Instr(CTD, Mnemonic.tltiu, R1, SImm),

                    Instr(CTD, Mnemonic.teqi, R1, SImm),
                    invalid,
                    Instr(CTD, Mnemonic.tnei, R1, SImm),
                    invalid,

                    Instr(CTD, Mnemonic.bltzal, R1, j),
                    Select((21, 5),  u => u == 0,
                        Instr(TD, Mnemonic.bal, j),
                        Instr(CTD, Mnemonic.bgezal, R1, j)),
                    Instr(CTD, Mnemonic.bltzall, R1, j),
                    Instr(CTD, Mnemonic.bgezall, R1, j),

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    DecodeRegimm18(),
                    DecodeRegimm19(),
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid);


                var special = Mask(0, 6, "Special",
                    Select((6, 5), n => n == 0,
                        Instr(InstrClass.Linear | InstrClass.Padding, Mnemonic.nop),
                        Instr(Mnemonic.sll, R3, R2, s)),
                    Mask(16, 1,
                        Instr(Mnemonic.movf, R2, R1, C18),
                        Instr(Mnemonic.movt, R2, R1, C18)),
                    Instr(Mnemonic.srl, R3, R2, s),
                    Instr(Mnemonic.sra, R3, R2, s),

                    Instr(Mnemonic.sllv, R3, R2, R1),
                    Instr(Mnemonic.illegal),
                    Instr(Mnemonic.srlv, R3, R2, R1),
                    Instr(Mnemonic.srav, R3, R2, R1),

                    Select((21, 5), u => u == 0b11111,
                        Instr(RTD, Mnemonic.jr, R1),
                        Instr(TD, Mnemonic.jr, R1)),
                    Instr(CTD, Mnemonic.jalr, R3, R1),
                    Instr(Mnemonic.movz, R3, R1, R2),
                    Instr(Mnemonic.movn, R3, R1, R2),
                    Instr(Mnemonic.syscall, B),
                    Instr(Mnemonic.@break, B),
                    Instr(Mnemonic.illegal),
                    Instr(Mnemonic.sync, s),
                    // 10
                    Instr(Mnemonic.mfhi, R3),
                    Instr(Mnemonic.mthi, R1),
                    Instr(Mnemonic.mflo, R3),
                    Instr(Mnemonic.mtlo, R1),
                    Instr64(Mnemonic.dsllv, R3, R2, R1),
                    Instr(Mnemonic.illegal),
                    Instr64(Mnemonic.dsrlv, R3, R2, R1),
                    Instr64(Mnemonic.dsrav, R3, R2, R1),

                    Instr(Mnemonic.mult, R1, R2),
                    Instr(Mnemonic.multu, R1, R2),
                    Instr(Mnemonic.div, R1, R2),
                    Instr(Mnemonic.divu, R1, R2),
                    Instr64(Mnemonic.dmult, R1, R2),
                    Instr64(Mnemonic.dmultu, R1, R2),
                    Instr64(Mnemonic.ddiv, R1, R2),
                    Instr64(Mnemonic.ddivu, R1, R2),
                    // 20
                    Instr(Mnemonic.add, R3, R1, R2),
                    Instr(Mnemonic.addu, R3, R1, R2),
                    Instr(Mnemonic.sub, R3, R1, R2),
                    Instr(Mnemonic.subu, R3, R1, R2),
                    Instr(Mnemonic.and, R3, R1, R2),
                    Instr(Mnemonic.or, R3, R1, R2),
                    Instr(Mnemonic.xor, R3, R1, R2),
                    Instr(Mnemonic.nor, R3, R1, R2),

                    DecodeSpecial28(),
                    DecodeSpecial29(),
                    Instr(Mnemonic.slt, R3, R1, R2),
                    Instr(Mnemonic.sltu, R3, R1, R2),
                    DecodeSpecial2C(),
                    DecodeSpecial2D(),
                    DecodeSpecial2E(),
                    DecodeSpecial2F(),
                    // 30
                    Instr(CTD, Mnemonic.tge, R1, R2, T),
                    Instr(CTD, Mnemonic.tgeu, R1, R2, T),
                    Instr(CTD, Mnemonic.tlt, R1, R2, T),
                    Instr(CTD, Mnemonic.tltu, R1, R2, T),
                    Instr(CTD, Mnemonic.teq, R1, R2, T),
                    Instr(Mnemonic.illegal),
                    Instr(CTD, Mnemonic.tne, R1, R2, T),
                    Instr(Mnemonic.illegal),

                    Instr64(Mnemonic.dsll, R3, R2, s),
                    Instr(Mnemonic.illegal),
                    Instr64(Mnemonic.dsrl, R3, R2, s),
                    Instr64(Mnemonic.dsra, R3, R2, s),
                    Instr64(Mnemonic.dsll32, R3, R2, s),
                    Instr(Mnemonic.illegal),
                    Instr64(Mnemonic.dsrl32, R3, R2, s),
                    Instr64(Mnemonic.dsra32, R3, R2, s));


                var rootDecoder = Mask(26, 6,
                        special,
                        condDecoders,
                        Instr(TD, Mnemonic.j, J),
                        Instr(CTD, Mnemonic.jal, J),
                        Instr(DCT, Mnemonic.beq, R1, R2, j),
                        Instr(DCT, Mnemonic.bne, R1, R2, j),
                        Instr(DCT, Mnemonic.blez, R1, j),
                        Instr(DCT, Mnemonic.bgtz, R1, j),

                        Instr(Mnemonic.addi, R2, R1, SImm),
                        Instr(Mnemonic.addiu, R2, R1, SImm),
                        Instr(Mnemonic.slti, R2, R1, SImm),
                        Instr(Mnemonic.sltiu, R2, R1, SImm),

                        Instr(Mnemonic.andi, R2, R1, U),
                        Instr(Mnemonic.ori, R2, R1, U),
                        Instr(Mnemonic.xori, R2, R1, U),
                        Instr(Mnemonic.lui, R2, i),
                        // 10
                        DecodeCop0(),
                        cop1,
                        DecodeCop2(),
                        DecodeCop1x(),

                        Instr(DCT, Mnemonic.beql, R1, R2, j),
                        Instr(DCT, Mnemonic.bnel, R1, R2, j),
                        Instr(DCT, Mnemonic.blezl, R1, j),
                        Instr(DCT, Mnemonic.bgtzl, R1, j),

                        Instr64(Mnemonic.daddi, R2, R1, SImm),
                        Instr64(Mnemonic.daddiu, R2, R1, SImm),
                        Instr64(Mnemonic.ldl, R2, El),
                        Instr64(Mnemonic.ldr, R2, El),

                        Decode1C(),
                        Decode1D(),
                        Decode1E(),
                        Decode1F(),

                        // 20
                        Instr(Mnemonic.lb, R2, EB),
                        Instr(Mnemonic.lh, R2, EH),
                        Instr(Mnemonic.lwl, R2, Ew),
                        Instr(Mnemonic.lw, R2, Ew),

                        Instr(Mnemonic.lbu, R2, Eb),
                        Instr(Mnemonic.lhu, R2, Eh),
                        Instr(Mnemonic.lwr, R2, Ew),
                        Instr64(Mnemonic.lwu, R2, Ew),

                        Instr(Mnemonic.sb, R2, Eb),
                        Instr(Mnemonic.sh, R2, Eh),
                        Instr(Mnemonic.swl, R2, Ew),
                        Instr(Mnemonic.sw, R2, Ew),

                        Instr(Mnemonic.sdl, R2, Ew),
                        Instr(Mnemonic.sdr, R2, Ew),
                        Instr(Mnemonic.swr, R2, Ew),
                        Decode2F(),

                        // 30
                        Decode30(),
                        Instr(Mnemonic.lwc1, F2, Ew),
                        Decode32(),
                        Instr(Mnemonic.pref, Imm(PrimitiveType.Byte, 6, 5), Ew),

                        Decode34(),
                        Instr(Mnemonic.ldc1, F2, El),
                        Decode36(),
                        Instr64(Mnemonic.ld, R2, El),

                        Decode38(),
                        Instr(Mnemonic.swc1, F2, Ew),
                        Decode3A(),
                        Decode3B(),

                        Decode3C(),
                        Instr64(Mnemonic.sdc1, F2, El),
                        Decode3E(),
                        Instr64(Mnemonic.sd, R2, El));

                return rootDecoder;
            }
        }
    }
}