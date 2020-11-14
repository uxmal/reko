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

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.Arch.Cray.Ymp
{
    using Decoder = Reko.Core.Machine.Decoder<YmpDisassembler, Mnemonic, CrayInstruction>;

    // Based on "Cray Y-MP Computer Systems Function Description Manual (HR-04001-0C)

    public class YmpDisassembler : DisassemblerBase<CrayInstruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;
        
        private readonly CrayYmpArchitecture arch;
        private readonly Word16BeImageReader rdr;
        private readonly List<MachineOperand>  ops;
        private Address addr;

        public YmpDisassembler(CrayYmpArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            // Crays are weird; we can only disassemble areas that have 16-bit granularity.
            this.rdr = rdr as Word16BeImageReader;
            this.ops = new List<MachineOperand>();
        }

        public override CrayInstruction DisassembleInstruction()
        {
            if (rdr is null)
                return null;
            this.addr = rdr.Address;
            if (!rdr.TryReadBeUInt16(out ushort hInstr))
                return null;
            ops.Clear();
            var instr = rootDecoder.Decode(hInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override CrayInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new CrayInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = this.ops.ToArray()
            };
            return instr;
        }

        public override CrayInstruction CreateInvalidInstruction()
        {
            return new CrayInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        public override CrayInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("YmpDis", this.addr, rdr, message, Octize);
            return CreateInvalidInstruction();
        }

        public static string Octize(byte[] bytes)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                var word = ByteMemoryArea.ReadBeUInt16(bytes, i);
                sb.Append(Convert.ToString(word, 8).PadLeft(6, '0'));
            }
            return sb.ToString();
        }

        #region Mutators

        private static Mutator<YmpDisassembler> Reg(int bitOffset, int bitSize, RegisterStorage[] regs)
        {
            var field = new Bitfield(bitOffset, bitSize);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var reg = regs[iReg];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<YmpDisassembler> Si = Reg(6, 3, Registers.SRegs);
        private static readonly Mutator<YmpDisassembler> Sj = Reg(3, 3, Registers.SRegs);
        private static readonly Mutator<YmpDisassembler> Sk = Reg(0, 3, Registers.SRegs);

        private static Mutator<YmpDisassembler> Reg0(int bitOffset, int bitSize, RegisterStorage r0, RegisterStorage[] regs)
        {
            var field = new Bitfield(bitOffset, bitSize);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var reg = iReg != 0 ? regs[iReg] : r0;
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<YmpDisassembler> Sk_SB = Reg0(0, 3, Registers.sb, Registers.SRegs);

        private static readonly Mutator<YmpDisassembler> Ah = Reg(9, 3, Registers.ARegs);
        private static readonly Mutator<YmpDisassembler> Ai = Reg(6, 3, Registers.ARegs);
        private static readonly Mutator<YmpDisassembler> Aj = Reg(3, 3, Registers.ARegs);
        private static readonly Mutator<YmpDisassembler> Ak = Reg(0, 3, Registers.ARegs);

        private static readonly Mutator<YmpDisassembler> Bjk = Reg(0, 6, Registers.BRegs);
        private static readonly Mutator<YmpDisassembler> Tjk = Reg(0, 6, Registers.TRegs);

        private static readonly Mutator<YmpDisassembler> STj = Reg(3, 3, Registers.STRegs);

        private static readonly Mutator<YmpDisassembler> Vi = Reg(6, 3, Registers.VRegs);
        private static readonly Mutator<YmpDisassembler> Vj = Reg(3, 3, Registers.VRegs);
        private static readonly Mutator<YmpDisassembler> Vk = Reg(0, 3, Registers.VRegs);

        private static Mutator<YmpDisassembler> Reg(RegisterStorage reg)
        {
            var r = new RegisterOperand(reg);
            return (u, d) =>
            {
                d.ops.Add(r);
                return true;
            };
        }

        private static readonly Mutator<YmpDisassembler> S0 = Reg(Registers.SRegs[0]);
        private static readonly Mutator<YmpDisassembler> SB = Reg(Registers.sb);

        private static Mutator<YmpDisassembler> Imm(PrimitiveType dt, int bitpos, int bitlen)
        {
            var bitfield = new Bitfield(bitpos, bitlen);
            return (u, d) =>
            {
                var imm = bitfield.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, imm)));
                return true;
            };
        }

        private static readonly Mutator<YmpDisassembler> Ijk = Imm(PrimitiveType.Byte, 0, 6);

        private static Mutator<YmpDisassembler> Imm(PrimitiveType dt, int bitpos, int bitlen, uint nFrom)
        {
            var bitfield = new Bitfield(bitpos, bitlen);
            return (u, d) =>
            {
                var imm = bitfield.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, nFrom - imm)));
                return true;
            };
        }
        private static readonly Mutator<YmpDisassembler> IjkFrom64 = Imm(PrimitiveType.Byte, 0, 6, 64);

        private static bool Inm(uint uInstr, YmpDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt32(out uint nm))
                return false;
            dasm.ops.Add(ImmediateOperand.Word32(nm));
            return true;
        }

        private static bool InmZext(uint uInstr, YmpDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt32(out uint nm))
                return false;
            dasm.ops.Add(ImmediateOperand.Word64(nm));
            return true;
        }

        private static bool Jijkm(uint uInstr, YmpDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out ushort n))
                return false;
            var uAddr = ((uInstr & 0x1FF) << 16) | n;
            dasm.ops.Add(AddressOperand.Ptr32(uAddr));
            return true;
        }

        private static bool Jnm(uint uInstr, YmpDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out ushort m))
                return false;
            if (!dasm.rdr.TryReadBeUInt16(out ushort n))
                return false;
            uint nm = n;
            nm = nm << 16 | m;
            dasm.ops.Add(AddressOperand.Ptr32(nm));
            return true;
        }

        private static bool Imm0(uint word, YmpDisassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Word64(0));
            return true;
        }

        private static bool Imm1(uint word, YmpDisassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Word64(1));
            return true;
        }

        private static bool Imm_1(uint word, YmpDisassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Word64(-1L));
            return true;
        }

        private static Mutator<YmpDisassembler> Imm64(ulong n)
        {
            var imm = ImmediateOperand.Word64(n);
            return (u, d) =>
            {
                d.ops.Add(imm);
                return true;
            };
        }

        private static readonly Mutator<YmpDisassembler> Imm_2_63 = Imm64(1ul << 63);

        #endregion

        private static bool Is0(uint u) => u == 0;

        #region Decoders

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<YmpDisassembler>[] mutators)
        {
            return new InstrDecoder<YmpDisassembler, Mnemonic, CrayInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<YmpDisassembler>[] mutators)
        {
            return new InstrDecoder<YmpDisassembler, Mnemonic, CrayInstruction>(iclass, mnemonic, mutators);
        }

        protected static NyiDecoder<YmpDisassembler, Mnemonic, CrayInstruction> Nyi(string message)
        {
            return new NyiDecoder<YmpDisassembler, Mnemonic, CrayInstruction>(message);
        }

        #endregion

        static YmpDisassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

            var opc_001 = Select((0, 9), Is0, "  001",
                Instr(Mnemonic.pass, InstrClass.Linear | InstrClass.Padding),
                Nyi("001"));

            var opc_005 = Select((6, 3), Is0, "  005x",
                    Instr(Mnemonic.j, InstrClass.Transfer, Bjk),
                    invalid);

            var opc_006 = Mask(8, 1, "  006",
                Select((0, 8), Is0, "  006 0",
                    Instr(Mnemonic.j, InstrClass.Transfer, Jnm),
                    Instr(Mnemonic.j, InstrClass.Transfer, Jijkm)),
                Mask(5, 1, " 0 006 4",
                    Instr(Mnemonic.jts, InstrClass.Transfer, Ijk, Jnm),
                    Instr(Mnemonic.jts, InstrClass.Transfer, Ak, Jnm)));

            var opc_007 = Select((0, 9), Is0, "  007",
                Instr(Mnemonic.r, InstrClass.Call | InstrClass.Transfer, Jnm),
                Instr(Mnemonic.r, InstrClass.Call | InstrClass.Transfer, Jijkm));

            var opc_010 = Select((0, 9), Is0, "  010",
                Instr(Mnemonic.jaz, InstrClass.ConditionalTransfer, Jnm),
                Instr(Mnemonic.jaz, InstrClass.ConditionalTransfer, Jijkm));

            var opc_011 = Select((0, 9), Is0, "  011",
                Instr(Mnemonic.jan, InstrClass.ConditionalTransfer, Jnm),
                Instr(Mnemonic.jan, InstrClass.ConditionalTransfer, Jijkm));

            var opc_012 = Select((0, 9), Is0, "  012",
                Instr(Mnemonic.jap, InstrClass.ConditionalTransfer, Jnm),
                Instr(Mnemonic.jap, InstrClass.ConditionalTransfer, Jijkm));

            var opc_013 = Select((0, 9), Is0, "  013",
                Instr(Mnemonic.jam, InstrClass.ConditionalTransfer, Jnm),
                Instr(Mnemonic.jam, InstrClass.ConditionalTransfer, Jijkm));

            var opc_014 = Select((0, 9), Is0, "  014",
                Instr(Mnemonic.jsz, InstrClass.ConditionalTransfer, Jnm),
                Instr(Mnemonic.jsz, InstrClass.ConditionalTransfer, Jijkm));

            var opc_015 = Select((0, 9), Is0, "  015",
                Instr(Mnemonic.jsn, InstrClass.ConditionalTransfer, Jnm),
                Instr(Mnemonic.jsn, InstrClass.ConditionalTransfer, Jijkm));

            var opc_016 = Select((0, 9), Is0, "  016",
                Instr(Mnemonic.jsp, InstrClass.ConditionalTransfer, Jnm),
                Instr(Mnemonic.jsp, InstrClass.ConditionalTransfer, Jijkm));

            var opc_017 = Select((0, 9), Is0, "  017",
                Instr(Mnemonic.jsm, InstrClass.ConditionalTransfer, Jnm),
                Instr(Mnemonic.jsm, InstrClass.ConditionalTransfer, Jijkm));

            var opc_030 = Select((3, 3), Is0, "  030",
                Select((0, 3), Is0, "  031 x0", 
                    Instr(Mnemonic._mov, Ai, Imm1),
                    Instr(Mnemonic._mov, Ai, Ak)),
                Select((0, 3), Is0, "  030 j!=0)",
                    Instr(Mnemonic._iadd, Ai, Aj, Imm1),
                    Instr(Mnemonic._iadd, Ai, Aj, Ak)));

            var opc_031 = Select((3, 3), Is0, "  031",
                Select((0, 3), Is0, "  031 x0", 
                    Instr(Mnemonic._mov, Ai, Imm_1),
                    Instr(Mnemonic._neg, Ai, Ak)),
                Select((0, 3), Is0, "  031 j!=0)",
                    Instr(Mnemonic._isub, Ai, Aj, Imm1),
                    Instr(Mnemonic._isub, Ai, Aj, Ak)));

            var opc_040 = Sparse(3, 3, "  040", invalid,
                (0, Instr(Mnemonic._mov, Si, InmZext)),
                (2, Instr(Mnemonic._movlo, Si, Si, Inm)),
                (4, Instr(Mnemonic._movhi, Si, Inm, Si)));

            var opc_042 = Select((0, 6), Is0, "  042",
                Instr(Mnemonic._mov, Si, Imm_1),
                Select((0, 6), u => u == 0x3F,
                    Instr(Mnemonic._mov, Si, Imm1),
                    Instr(Mnemonic._lmask, Si, Ijk)));

            var opc_043 = Select((0, 6), Is0, "  043",
                Instr(Mnemonic._mov, Si, Imm0),
                Instr(Mnemonic._rmask, Si, Ijk));

            var opc_056 = Select((3, 3), Is0, "  056",
                Instr(Mnemonic._lsl, Si, Si, Ak),
                Select((0, 3), Is0, "  056 j!=0",
                    Instr(Mnemonic._dlsl, Si, Si, Sj, Imm1),
                    Instr(Mnemonic._dlsl, Si, Si, Sj, Ak)));

            var opc_057 = Select((3, 3), Is0, "  057",
                Instr(Mnemonic._lsr, Si, Si, Ak),
                Select((0, 3), Is0, "  057 j!=0",
                    Instr(Mnemonic._dlsr, Si, Si, Sj, Imm1),
                    Instr(Mnemonic._dlsr, Si, Si, Sj, Ak)));

            var opc_060 = Select((3, 3), Is0, "  060",
                Select((0, 3), Is0, "  060 j=0",
                    Instr(Mnemonic._mov, Si, Imm_2_63),
                    Instr(Mnemonic._mov, Si, Sk)),
                Select((0, 3), Is0, "  060 j!=0",
                    Nyi(" (Si) = (Sj) with bit 263 complemented."),
                    Instr(Mnemonic._iadd, Si, Sj, Sk)));

            var opc_061 = Select((3, 3), Is0, "  061",
                Select((0, 3), Is0, "  060 j=0",
                    Instr(Mnemonic._mov, Si, Imm_2_63),
                    Instr(Mnemonic._neg, Si, Sk)),
                Select((0, 3), Is0, "  060 j!=0",
                    Nyi(" (Sz) = (Sj) with bit 263 complemented."),
                    Instr(Mnemonic._isub, Si, Sj, Sk)));

            var opc_071 = Sparse(3, 3, " 071", Nyi("071"),
                (0, Instr(Mnemonic._movz, Si, Ak)),
                (1, Instr(Mnemonic._movs, Si, Ak)));

            var opc_072 = Sparse(0, 3, " 072", invalid,
                (0, Instr(Mnemonic._mov, Si, Reg(Registers.rt))),
                (2, Instr(Mnemonic._mov, Si, Reg(Registers.sm))),
                (3, Instr(Mnemonic._mov, Si, STj)),
                (6, Instr(Mnemonic._movst, Si, Reg(Registers.st), Aj)));

            var opc_073 = Sparse(0, 3, " 072", Nyi("073"),
                (2, Instr(Mnemonic._mov, Reg(Registers.sm), Si)));

            rootDecoder = Sparse(9, 7, "YMP",
                Nyi("YMP"),
                (0x00, Select((0, 6), Is0,
                    Instr(Mnemonic.err),
                    invalid)),
                (0x01, opc_001),
                (0x05, opc_005),
                (0x06, opc_006),
                (0x07, opc_007),

                (0x08, opc_010),
                (0x09, opc_011),
                (0x0A, opc_012),
                (0x0B, opc_013),

                (0x0C, opc_014),
                (0x0D, opc_015),
                (0x0E, opc_016),
                (0x0F, opc_017),

                (0x10, Select((0,6), Is0, "  020",          // 0o020
                    Instr(Mnemonic._mov, Ai, Inm),
                    invalid)),
                (0x13, Instr(Mnemonic._mov, Ai, Sj)),       // 0o023
                (0x14, Instr(Mnemonic._mov, Ai, Bjk)),      // 0o024
                (0x15, Instr(Mnemonic._mov, Bjk, Ai)),      // 0o025
                (0x18, opc_030),                            // 0o030
                (0x19, opc_031),                            // 0o031
                (0x1A, Instr(Mnemonic._imul, Ai,Aj,Ak)),    // 0o032
                (0x20, opc_040),                            // 0o040
                (0x22, opc_042),                            // 0o042
                (0x23, opc_043),                            // 0o043
                (0x24, Instr(Mnemonic._and, Si,Sj,Sk_SB)),   // 0o044
                (0x25, Instr(Mnemonic._andnot, Si,Sk_SB, Sj)),  // 0o045
                (0x26, Instr(Mnemonic._xor, Si,Sj,Sk_SB)),      // 0o046
                (0x27, Instr(Mnemonic._eqv, Si, Sk_SB, Sj)),    // 0o047
                (0x28, Instr(Mnemonic._and_or, Si, Sj, Sk_SB)), // 0o050
                (0x29, Instr(Mnemonic._or, Si, Sj, Sk_SB)), // 0o051
                (0x2A, Instr(Mnemonic._lsl, S0, Si, Ijk)),          // 0o052
                (0x2B, Instr(Mnemonic._lsr, S0, Si, IjkFrom64)),    // 0o053
                (0x2C, Instr(Mnemonic._lsl, Si, Si, Ijk)),          // 0o054
                (0x2D, Instr(Mnemonic._lsr, Si, Si, IjkFrom64)),    // 0o055
                (0x2E, opc_056),                                    // 0o056
                (0x2F, opc_057),                                    // 0o057

                (0x30, opc_060),                                    // 0o060
                (0x31, opc_061),                                    // 0o061
                (0x34, Instr(Mnemonic._fmul, Si, Sj, Sk)),  // 0o064
                (0x39, opc_071),       // 0o071
                (0x3A, opc_072),       // 0o072
                (0x3B, opc_073),       // 0o073
                (0x3C, Instr(Mnemonic._mov, Si, Tjk)),          // 0o074
                (0x3D, Instr(Mnemonic._mov, Tjk, Si)),          // 0o075
                (0x3E, Instr(Mnemonic._mov, Si, Vj, Ak)),       // 0o076

                (0x40, Instr(Mnemonic._load, Ai, Inm)),         // 0o100
                (0x41, Instr(Mnemonic._load, Ai, Inm, Ah)),     // 0o101
                (0x42, Instr(Mnemonic._load, Ai, Inm, Ah)),     // 0o102
                (0x43, Instr(Mnemonic._load, Ai, Inm, Ah)),     // 0o103
                (0x44, Instr(Mnemonic._load, Ai, Inm, Ah)),     // 0o104
                (0x45, Instr(Mnemonic._load, Ai, Inm, Ah)),     // 0o105
                (0x46, Instr(Mnemonic._load, Ai, Inm, Ah)),     // 0o106
                (0x47, Instr(Mnemonic._load, Ai, Inm, Ah)),     // 0o107
                (0x48, Instr(Mnemonic._store, Inm, Ai)),        // 0o100
                (0x49, Instr(Mnemonic._store, Inm, Ah, Ai)),    // 0o111
                (0x4A, Instr(Mnemonic._store, Inm, Ah, Ai)),    // 0o112
                (0x4B, Instr(Mnemonic._store, Inm, Ah, Ai)),    // 0o113
                (0x4C, Instr(Mnemonic._store, Inm, Ah, Ai)),    // 0o114
                (0x4D, Instr(Mnemonic._store, Inm, Ah, Ai)),    // 0o115
                (0x4E, Instr(Mnemonic._store, Inm, Ah, Ai)),    // 0o116
                (0x4F, Instr(Mnemonic._store, Inm, Ah, Ai)),    // 0o117

                (0x50, Instr(Mnemonic._load, Si, Inm)),         // 0o120
                (0x51, Instr(Mnemonic._load, Si, Inm,Ah)),      // 0o121
                (0x52, Instr(Mnemonic._load, Si, Inm,Ah)),      // 0o122
                (0x53, Instr(Mnemonic._load, Si, Inm,Ah)),      // 0o123
                (0x54, Instr(Mnemonic._load, Si, Inm,Ah)),      // 0o124
                (0x55, Instr(Mnemonic._load, Si, Inm,Ah)),      // 0o125
                (0x56, Instr(Mnemonic._load, Si, Inm,Ah)),      // 0o126
                (0x57, Instr(Mnemonic._load, Si, Inm,Ah)),      // 0o127
                (0x58, Instr(Mnemonic._store, Inm, Si)),        // 0o130
                (0x59, Instr(Mnemonic._store, Inm,Ah, Si)),     // 0o131
                (0x5A, Instr(Mnemonic._store, Inm,Ah, Si)),     // 0o132
                (0x5B, Instr(Mnemonic._store, Inm,Ah, Si)),     // 0o133
                (0x5C, Instr(Mnemonic._store, Inm,Ah, Si)),     // 0o134
                (0x5D, Instr(Mnemonic._store, Inm,Ah, Si)),     // 0o135
                (0x5E, Instr(Mnemonic._store, Inm,Ah, Si)),     // 0o136
                (0x5F, Instr(Mnemonic._store, Inm,Ah, Si))      // 0o137
                );
        }
    }
}
