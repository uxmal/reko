#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Diagnostics;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Reko.Arch.M68k.Machine;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Arch.M68k.Disassembler
{
    using Decoder = Decoder<M68kDisassembler, Mnemonic, M68kInstruction>;
    // M68k opcode map in http://www.freescale.com/files/archives/doc/ref_manual/M68000PRM.pdf
    using Mutator = Mutator<M68kDisassembler>;

#pragma warning disable IDE1006 // Naming Styles

    public partial class M68kDisassembler : DisassemblerBase<M68kInstruction, Mnemonic>
    {
        public const string HexStringFormat = "{0}${1}";

        private const InstrClass LinPriv = InstrClass.Linear | InstrClass.Privileged;

        private static readonly TraceSwitch trace = new TraceSwitch("m68dasm", "Detailed tracing of M68k disassembler");

        private readonly IServiceProvider services;
        private readonly EndianImageReader rdr;          // program counter 
        private readonly List<MachineOperand> ops;       // Operand list being built
        private string g_dasm_str = "";              //string to hold disassembly: OBSOLETE
        internal Address addr;                   // Start address of current instruction
        private InstrClass iclass;
        private Mnemonic mnemonic;
        private PrimitiveType? dataWidth;       // width of data.
        private ushort bitSet;                  // Bit set.
        private ushort fpWord;                  // Word for 68040 FPU instructions.
        private uint cpuType = 0;

        private M68kDisassembler(IServiceProvider services, EndianImageReader rdr, uint cpuType)
        {
            this.services = services;
            this.rdr = rdr;
            addr = rdr.Address;
            ops = new List<MachineOperand>();
            this.cpuType = cpuType;
        }

        public static M68kDisassembler Create68000(IServiceProvider services, EndianImageReader rdr) { return new M68kDisassembler(services, rdr, TYPE_68000); }
        public static M68kDisassembler Create68010(IServiceProvider services, EndianImageReader rdr) { return new M68kDisassembler(services, rdr, TYPE_68010); }
        public static M68kDisassembler Create68020(IServiceProvider services, EndianImageReader rdr) { return new M68kDisassembler(services, rdr, TYPE_68020); }

        static M68kDisassembler()
        {
            g_opcode_info = GenTable();
            BuildDecoderTable();
        }

        public override M68kInstruction? DisassembleInstruction()
        {
            if (!rdr.TryReadBeUInt16(out ushort uInstr))
                return null;
            InstrDecoder handler = g_instruction_table[uInstr];
            dataWidth = null;
            ops.Clear();
            var instr = handler.Decode(uInstr, this);
            instr.Address = addr;
            addr = rdr.Address;
            instr.Length = (int) (addr - instr.Address);
            instr.InstructionClass |= uInstr == 0 ? InstrClass.Zero : 0;
            return instr;
        }

        public override M68kInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new M68kInstruction
            {
                InstructionClass = this.iclass,
                Mnemonic = this.mnemonic,
                DataWidth = dataWidth,
                Operands = ops.ToArray(),
            };
            return instr;
        }


        /* ======================================================================== */
        /* ========================= LICENSING & COPYRIGHT ======================== */
        /* ======================================================================== */
        /*
         *                                  MUSASHI
         *                                Version 3.32
         *
         * A portable Motorola M680x0 processor emulation engine.
         * Copyright Karl Stenerud.  All rights reserved.
         *
         * This code may be freely used for non-commercial purposes as long as this
         * copyright notice remains unaltered in the source code and any binary files
         * containing this code in compiled form.
         *
         * All other licensing terms must be negotiated with the author
         * (Karl Stenerud).
         *
         * The latest version of this code can be obtained at:
         * http://kstenerud.cjb.net
         */

        /* Bit Isolation Functions */
        internal static bool BIT_0(uint A) { return (A & 0x00000001) != 0; }
        internal static bool BIT_1(uint A) { return (A & 0x00000002) != 0; }
        internal static bool BIT_2(uint A) { return (A & 0x00000004) != 0; }
        internal static bool BIT_3(uint A) { return (A & 0x00000008) != 0; }
        internal static bool BIT_4(uint A) { return (A & 0x00000010) != 0; }
        internal static bool BIT_5(uint A) { return (A & 0x00000020) != 0; }
        internal static bool BIT_6(uint A) { return (A & 0x00000040) != 0; }
        internal static bool BIT_7(uint A) { return (A & 0x00000080) != 0; }
        internal static bool BIT_8(uint A) { return (A & 0x00000100) != 0; }
        internal static bool BIT_9(uint A) { return (A & 0x00000200) != 0; }
        internal static bool BIT_A(uint A) { return (A & 0x00000400) != 0; }
        internal static bool BIT_B(uint A) { return (A & 0x00000800) != 0; }
        internal static bool BIT_C(uint A) { return (A & 0x00001000) != 0; }
        internal static bool BIT_D(uint A) { return (A & 0x00002000) != 0; }
        internal static bool BIT_E(uint A) { return (A & 0x00004000) != 0; }
        internal static bool BIT_F(uint A) { return (A & 0x00008000) != 0; }
        internal static bool BIT_10(uint A) { return (A & 0x00010000) != 0; }
        internal static bool BIT_11(uint A) { return (A & 0x00020000) != 0; }
        internal static bool BIT_12(uint A) { return (A & 0x00040000) != 0; }
        internal static bool BIT_13(uint A) { return (A & 0x00080000) != 0; }
        internal static bool BIT_14(uint A) { return (A & 0x00100000) != 0; }
        internal static bool BIT_15(uint A) { return (A & 0x00200000) != 0; }
        internal static bool BIT_16(uint A) { return (A & 0x00400000) != 0; }
        internal static bool BIT_17(uint A) { return (A & 0x00800000) != 0; }
        internal static bool BIT_18(uint A) { return (A & 0x01000000) != 0; }
        internal static bool BIT_19(uint A) { return (A & 0x02000000) != 0; }
        internal static bool BIT_1A(uint A) { return (A & 0x04000000) != 0; }
        internal static bool BIT_1B(uint A) { return (A & 0x08000000) != 0; }
        internal static bool BIT_1C(uint A) { return (A & 0x10000000) != 0; }
        internal static bool BIT_1D(uint A) { return (A & 0x20000000) != 0; }
        internal static bool BIT_1E(uint A) { return (A & 0x40000000) != 0; }
        internal static bool BIT_1F(uint A) { return (A & 0x80000000) != 0; }

        /* These are the CPU types understood by this disassembler */
        private const uint TYPE_68000 = 1;
        private const uint TYPE_68008 = 2;
        private const uint TYPE_68010 = 4;
        private const uint TYPE_68020 = 8;
        private const uint TYPE_68030 = 16;
        private const uint TYPE_68040 = 32;

        private const uint M68000_ONLY = TYPE_68000 | TYPE_68008;

        private const uint M68010_ONLY = TYPE_68010;
        private const uint M68010_LESS = TYPE_68000 | TYPE_68008 | TYPE_68010;
        private const uint M68010_PLUS = TYPE_68010 | TYPE_68020 | TYPE_68030 | TYPE_68040;

        private const uint M68020_ONLY = TYPE_68020;
        private const uint M68020_LESS = TYPE_68010 | TYPE_68020;
        private const uint M68020_PLUS = TYPE_68020 | TYPE_68030 | TYPE_68040;

        private const uint M68030_ONLY = TYPE_68030;
        private const uint M68030_LESS = TYPE_68010 | TYPE_68020 | TYPE_68030;
        private const uint M68030_PLUS = TYPE_68030 | TYPE_68040;

        private const uint M68040_PLUS = TYPE_68040;

        private static readonly Bitfield bf3L3 = new Bitfield(3, 3);

        /* Extension word formats */
        private static sbyte EXT_8BIT_DISPLACEMENT(uint A) { return (sbyte) (A & 0xff); }
        internal static bool EXT_FULL(uint A) { return BIT_8(A); }
        internal static bool EXT_EFFECTIVE_ZERO(uint A) { return (A & 0xe4) == 0xc4 || (A & 0xe2) == 0xc0; }
        private static bool EXT_BASE_REGISTER_PRESENT(uint A) { return !BIT_7(A); }
        private static bool EXT_INDEX_REGISTER_PRESENT(uint A) { return !BIT_6(A); }
        private static uint EXT_INDEX_REGISTER(uint A) { return A >> 12 & 7; }
        private static bool EXT_INDEX_PRE_POST(uint A) { return EXT_INDEX_REGISTER_PRESENT(A) && (A & 3) != 0; }
        private static bool EXT_INDEX_PRE(uint A) { return EXT_INDEX_REGISTER_PRESENT(A) && (A & 7) < 4 && (A & 7) != 0; }
        private static bool EXT_INDEX_POST(uint A) { return EXT_INDEX_REGISTER_PRESENT(A) && (A & 7) > 4; }
        internal static int EXT_INDEX_SCALE(uint A) { return (int) (A >> 9 & 3); }
        private static bool EXT_INDEX_LONG(uint A) { return BIT_B(A); }
        private static bool EXT_INDEX_AR(uint A) { return BIT_F(A); }
        private static bool EXT_BASE_DISPLACEMENT_VALID(uint A) { return (A & 0x30) > 0x00; }
        private static bool EXT_BASE_DISPLACEMENT_PRESENT(uint A) { return (A & 0x30) > 0x10; }
        private static bool EXT_BASE_DISPLACEMENT_WORD(uint A) { return (A & 0x30) == 0x20; }
        private static bool EXT_BASE_DISPLACEMENT_LONG(uint A) { return (A & 0x30) == 0x30; }
        private static bool EXT_OUTER_DISPLACEMENT_PRESENT(uint A) { return (A & 3) > 1 && (A & 0x47) < 0x44; }
        private static bool EXT_OUTER_DISPLACEMENT_WORD(uint A) { return (A & 3) == 2 && (A & 0x47) < 0x44; }
        private static bool EXT_OUTER_DISPLACEMENT_LONG(uint A) { return (A & 3) == 3 && (A & 0x47) < 0x44; }

        public override M68kInstruction CreateInvalidInstruction()
        {
            return new M68kInstruction
            {
                Mnemonic = Mnemonic.illegal,
                InstructionClass = InstrClass.Invalid,
                Operands = Array.Empty<MachineOperand>(),
            };
        }

        public override M68kInstruction NotYetImplemented(string message)
        {
            var testGenSvc = services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("M68kdis", addr, rdr, message);
            return CreateInvalidInstruction();
        }

        /// <summary>
        /// Decoders provide the knowledge for how to decode a M68k instruction.
        /// </summary>
        /// <remarks>
        /// Decoders follow the flyweight pattern; they must never have modifiable state. Therefore,
        /// all the fields are readonly.
        /// </remarks>
        private abstract class BaseDecoder : Decoder
        {
            public readonly uint mask;                  // opcode mask
            public readonly uint match;                 // opcode bit pattern (after mask)
            public readonly uint ea_mask;               // Permitted ea modes are allowed 

            public BaseDecoder(uint mask, uint match, uint ea_mask)
            {
                this.mask = mask;
                this.match = match;
                this.ea_mask = ea_mask;
            }
        }

        private class InstrDecoder : BaseDecoder
        {
            public readonly Mnemonic mnemonic;          // The decoded mnemonic.
            public readonly InstrClass iclass;          // Instruction class of the decoded instruction.
            public readonly Mutator[] mutators;         // Mutators used to modify the disassembler state while decoding.

            /// <summary>
            /// Builds an instance of an decoder record that uses one or more <see cref="Mutator{TDasm}"/> 
            /// to decode the operands. 
            /// </summary>
            /// <remarks>
            /// This is the preferred constructor. Contributors are kindly asked to translate calls to the 
            /// other, deprecated, constructor to this one.
            /// </remarks>
            /// <param name="opFormat">string to be passed to OperandFormatDecoder</param>
            /// <param name="mask"></param>
            /// <param name="match"></param>
            /// <param name="ea_mask"></param>
            /// <param name="mnemonic"></param>
            /// <param name="iclass"></param>
            public InstrDecoder(Mutator[] mutators, uint mask, uint match, uint ea_mask, Mnemonic mnemonic = Mnemonic.illegal, InstrClass iclass = InstrClass.Linear)
                : base(mask, match, ea_mask)
            {
                this.mutators = mutators;
                Debug.Assert(mutators.All(m => m is not null));
                this.mnemonic = mnemonic;
                this.iclass = iclass;
            }

            public override M68kInstruction Decode(uint uInstr, M68kDisassembler dasm)
            {
                dasm.mnemonic = mnemonic;
                dasm.iclass = iclass;
                foreach (var m in mutators)
                {
                    if (!m(uInstr, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                return dasm.MakeInstruction(iclass, mnemonic);
            }
        }

        private static InstrDecoder Instr(uint mask, uint match, uint ea_mask, Mnemonic mnemonic, InstrClass iclass = InstrClass.Linear)
        {
            return new InstrDecoder(
                Array.Empty<Mutator>(),
                mask,
                match,
                ea_mask,
                mnemonic,
                iclass);
        }

        private static InstrDecoder Instr(Mutator m1, uint mask, uint match, uint ea_mask, Mnemonic mnemonic = Mnemonic.illegal, InstrClass iclass = InstrClass.Linear)
        {
            return new InstrDecoder(
                new Mutator[] { m1 },
                mask,
                match,
                ea_mask,
                mnemonic,
                iclass);
        }

        private static InstrDecoder Instr(Mutator m1, Mutator m2, uint mask, uint match, uint ea_mask, Mnemonic mnemonic, InstrClass iclass = InstrClass.Linear)
        {
            return new InstrDecoder(
                new Mutator[] { m1, m2 },
                mask,
                match,
                ea_mask,
                mnemonic,
                iclass);
        }

        private static InstrDecoder Instr(Mutator m1, Mutator m2, Mutator m3, uint mask, uint match, uint ea_mask, Mnemonic mnemonic, InstrClass iclass = InstrClass.Linear)
        {
            return new InstrDecoder(
                new Mutator[] { m1, m2, m3 },
                mask,
                match,
                ea_mask,
                mnemonic,
                iclass);
        }

        private static InstrDecoder Instr(Mutator m1, Mutator m2, Mutator m3, Mutator m4, uint mask, uint match, uint ea_mask, Mnemonic mnemonic, InstrClass iclass = InstrClass.Linear)
        {
            return new InstrDecoder(
                new Mutator[] { m1, m2, m3, m4 },
                mask,
                match,
                ea_mask,
                mnemonic,
                iclass);
        }

        private class MaskFpDecoder : Decoder
        {
            private readonly Bitfield bitfield;
            private readonly string message;
            private readonly Decoder[] decoders;

            public MaskFpDecoder(Bitfield bitfield, string message, params Decoder[] decoders)
            {
                this.bitfield = bitfield;
                this.message = message;
                this.decoders = decoders;
            }

            public override M68kInstruction Decode(uint uInstr, M68kDisassembler dasm)
            {
                return decoders[bitfield.Read(dasm.fpWord)].Decode(uInstr, dasm);
            }
        }

        private static MaskFpDecoder MaskFp(int bitPos, int bitLen, string message, params Decoder[] decoders)
        {
            Debug.Assert(decoders.Length == 1 << bitLen);
            return new MaskFpDecoder(new Bitfield(bitPos, bitLen), message, decoders);
        }

        private static NyiDecoder<M68kDisassembler, Mnemonic, M68kInstruction> Nyi(string message)
        {
            return new NyiDecoder<M68kDisassembler, Mnemonic, M68kInstruction>(message);
        }

        // One decoder for each possible 16-bit opcode.
        private static readonly InstrDecoder[] g_instruction_table = new InstrDecoder[0x10000];

        // 'Q'uick dasm.instructions contain patterns that map to integers.
        private static readonly uint[] g_3bit_qdata_table = new uint[8]
        {
            8, 1, 2, 3, 4, 5, 6, 7
        };

        private static readonly uint[] g_5bit_data_table = new uint[32]
        {
            32,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31
        };

        internal static readonly Mnemonic[] g_bcc = new Mnemonic[16] {
            Mnemonic.bt,  Mnemonic.bf,  Mnemonic.bhi, Mnemonic.bls, Mnemonic.bcc, Mnemonic.bcs, Mnemonic.bne, Mnemonic.beq,
            Mnemonic.bvc, Mnemonic.bvs, Mnemonic.bpl, Mnemonic.bmi, Mnemonic.bge, Mnemonic.blt, Mnemonic.bgt, Mnemonic.ble };
        private static readonly Mnemonic[] g_dbcc = new Mnemonic[16] {
            Mnemonic.dbt,  Mnemonic.dbf,  Mnemonic.dbhi, Mnemonic.dbls, Mnemonic.dbcc, Mnemonic.dbcs, Mnemonic.dbne, Mnemonic.dbeq,
            Mnemonic.dbvc, Mnemonic.dbvs, Mnemonic.dbpl, Mnemonic.dbmi, Mnemonic.dbge, Mnemonic.dblt, Mnemonic.dbgt, Mnemonic.dble };
        private static readonly Mnemonic[] g_scc = new Mnemonic[16] {
            Mnemonic.st,  Mnemonic.sf,  Mnemonic.shi, Mnemonic.sls, Mnemonic.scc, Mnemonic.scs, Mnemonic.sne, Mnemonic.seq,
            Mnemonic.svc, Mnemonic.svs, Mnemonic.spl, Mnemonic.smi, Mnemonic.sge, Mnemonic.slt, Mnemonic.sgt, Mnemonic.sle };
        private static readonly Mnemonic[] g_trapcc = new Mnemonic[16] {
            Mnemonic.trapt,  Mnemonic.trapf,  Mnemonic.traphi, Mnemonic.trapls, Mnemonic.trapcc, Mnemonic.trapcs, Mnemonic.trapne, Mnemonic.trapeq,
            Mnemonic.trapvc, Mnemonic.trapvs, Mnemonic.trappl, Mnemonic.trapmi, Mnemonic.trapge, Mnemonic.traplt, Mnemonic.trapgt, Mnemonic.traple
        };

        private static readonly Mnemonic[] g_cpcc = new Mnemonic[64]
        {
            /* 000            001            010            011    100    101    110    111 */
	          Mnemonic.fbf,     Mnemonic.fbeq,   Mnemonic.fbogt,  Mnemonic.fboge,  Mnemonic.fbolt,  Mnemonic.fbole,  Mnemonic.fbogl,  Mnemonic.fbor, /* 000 */
	          Mnemonic.fbun,    Mnemonic.fbueq,  Mnemonic.fbugt,  Mnemonic.fbuge,  Mnemonic.fbult,  Mnemonic.fbule,  Mnemonic.fbne,   Mnemonic.fbt, /* 001 */
	          Mnemonic.fbsf,    Mnemonic.fbseq,  Mnemonic.fbgt,   Mnemonic.fbge,   Mnemonic.fblt,   Mnemonic.fble,   Mnemonic.fbgl,   Mnemonic.fbgle, /* 010 */
              Mnemonic.fbngle,  Mnemonic.fbngl,  Mnemonic.fbnle,  Mnemonic.fbnlt,  Mnemonic.fbnge,  Mnemonic.fbngt,  Mnemonic.fbsne,  Mnemonic.fbst, /* 011 */
	          Mnemonic.illegal, Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,/* 100 */
	          Mnemonic.illegal, Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal, /* 101 */
	          Mnemonic.illegal, Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal, /* 110 */
	          Mnemonic.illegal, Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,Mnemonic.illegal,/* 111 */
        };

        private static readonly string[] g_caches =
        {
            "", "dc", "ic", "bc"
        };

        private static readonly Mnemonic[] g_mmucond =
        {
            Mnemonic.pbbs, Mnemonic.pbbc, Mnemonic.pbls, Mnemonic.pblc,
            Mnemonic.pbss, Mnemonic.pbsc, Mnemonic.pbas, Mnemonic.pbac,
            Mnemonic.pbws, Mnemonic.pbwc, Mnemonic.pbis, Mnemonic.pbic,
            Mnemonic.pbgs, Mnemonic.pbgc, Mnemonic.pbcs, Mnemonic.pbcc
        };


        private static readonly Mnemonic[] g_mmudcond =
        {
            Mnemonic.pdbbs, Mnemonic.pdbbc, Mnemonic.pdbls, Mnemonic.pdblc,
            Mnemonic.pdbss, Mnemonic.pdbsc, Mnemonic.pdbas, Mnemonic.pdbac,
            Mnemonic.pdbws, Mnemonic.pdbwc, Mnemonic.pdbis, Mnemonic.pdbic,
            Mnemonic.pdbgs, Mnemonic.pdbgc, Mnemonic.pdbcs, Mnemonic.pdbcc
        };

        // Utility functions 

        private bool LIMIT_CPU_TYPES(uint uInstr, uint ALLOWED_CPU_TYPES)
        {
            if ((cpuType & ALLOWED_CPU_TYPES) == 0)
            {
                if ((uInstr & 0xf000) == 0xf000)
                    d68000_1111(uInstr, this);
                else
                    d68000_illegal(uInstr, this);
                return false;
            }
            return true;
        }

        private MachineOperand? DecodeEa8(uint instruction) { return DecodeEffectiveAddress(instruction, PrimitiveType.Byte); }
        private MachineOperand? DecodeEa16(uint instruction) { return DecodeEffectiveAddress(instruction, PrimitiveType.Word16); }
        private MachineOperand? DecodeEa32(uint instruction) { return DecodeEffectiveAddress(instruction, PrimitiveType.Word32); }

        private bool get_imm_str_s8([MaybeNullWhen(false)] out Constant imm) { return TryGetSignedImmediate(0, out imm); }
        private bool get_imm_str_s16([MaybeNullWhen(false)] out Constant imm) { return TryGetSignedImmediate(1, out imm); }
        private bool get_imm_str_s32([MaybeNullWhen(false)] out Constant imm) { return TryGetSignedImmediate(2, out imm); }

        private bool get_imm_str_u8([MaybeNullWhen(false)] out Constant imm) { return get_imm_str_u(PrimitiveType.Byte, out imm); }

        private static RegisterStorage get_data_reg(uint d) { return Registers.DataRegister(d); }
        private static RegisterStorage get_addr_reg(uint a) { return Registers.AddressRegister(a); }
        private static RegisterStorage get_addr_or_data_reg(bool addrReg, uint bits)
        {
            var reg = addrReg ? Registers.AddressRegister(bits) : Registers.DataRegister(bits);
            return reg;
        }

        private RegisterStorage get_fp_reg(int fp) { return Registers.FpRegister(fp); }

        private PredecrementMemoryOperand get_pre_dec(uint uInstr, int offset, PrimitiveType dataWidth)
        {
            return new PredecrementMemoryOperand(dataWidth, Registers.AddressRegister(uInstr >> offset & 7));
        }

        private PostIncrementMemoryOperand get_post_inc(int a, PrimitiveType dataWidth)
        {
            return new PostIncrementMemoryOperand(dataWidth, Registers.AddressRegister(a & 7));
        }

        private RegisterStorage get_ctrl_reg(string regName, uint number)
        {
            return RegisterStorage.Reg16(regName, (int) number);
        }

        private DoubleRegisterOperand get_double_data_reg(uint d1, uint d2)
        {
            return new DoubleRegisterOperand(
                Registers.DataRegister((int) d1 & 7),
                Registers.DataRegister((int) d2 & 7));
        }

        private bool TryReadImm8(out byte b)
        {
            if (!rdr.TryReadBeInt16(out short s))
            {
                b = 0;
                return false;
            }
            else
            {
                b = (byte) s;
                return true;
            }
        }

        /// <summary>
        /// Build a signed immediate operand from the instruction stream.
        /// </summary>
        private bool TryGetSignedImmediate(uint size, [MaybeNullWhen(false)] out Constant imm)
        {
            imm = default!;
            if (size == 0)
            {
                if (!TryReadImm8(out byte b))
                    return false;
                imm = Constant.SByte((sbyte) b);
            }
            else if (size == 1)
            {
                if (!rdr.TryReadBeInt16(out short s))
                    return false;
                imm = Constant.Int16(s);
            }
            else
            {
                if (!rdr.TryReadBeInt32(out int i))
                    return false;
                imm = Constant.Int32(i);
            }
            return true;
        }

        private bool get_imm_str_u(PrimitiveType dt, [MaybeNullWhen(false)] out Constant imm)
        {
            Constant? c;
            imm = default!;
            if (dt.Domain == Domain.Real)
            {
                if (!rdr.TryReadBe(dt, out c))
                    return false;
            }
            else
            {
                if (dt.Size == 1)
                {
                    if (!TryReadImm8(out byte b))
                        return false;
                    c = Constant.Byte(b);
                }
                else if (dt.Size == 2)
                {
                    if (!rdr.TryReadBeUInt16(out ushort us))
                        return false;
                    c = Constant.Word16(us);
                }
                else
                {
                    if (!rdr.TryReadBeUInt32(out uint ui))
                        return false;
                    c = Constant.Word32(ui);
                }
            }
            imm = c;
            return true;
        }

        /// <summary>
        /// Build an effective address.
        /// </summary>
        /// <param name="uInstr">Bits encoding the effective address. Only the 6 least
        /// significant are actually used.</param>
        /// <param name="size"></param>
        /// <returns></returns>
        private MachineOperand? DecodeEffectiveAddress(uint uInstr, PrimitiveType dataWidth)
        {
            switch (uInstr & 0x3f)
            {
            case 0x00:
            case 0x01:
            case 0x02:
            case 0x03:
            case 0x04:
            case 0x05:
            case 0x06:
            case 0x07:
                // data register direct 
                return Registers.DataRegister(uInstr & 7);
            case 0x08:
            case 0x09:
            case 0x0a:
            case 0x0b:
            case 0x0c:
            case 0x0d:
            case 0x0e:
            case 0x0f:
                // address register direct 
                return Registers.AddressRegister(uInstr & 7);
            case 0x10:
            case 0x11:
            case 0x12:
            case 0x13:
            case 0x14:
            case 0x15:
            case 0x16:
            case 0x17:
                // address register indirect
                return MemoryOperand.Indirect(dataWidth, Registers.AddressRegister((int) uInstr & 7));
            case 0x18:
            case 0x19:
            case 0x1a:
            case 0x1b:
            case 0x1c:
            case 0x1d:
            case 0x1e:
            case 0x1f:
                // address register indirect with postincrement
                return new PostIncrementMemoryOperand(dataWidth, Registers.AddressRegister((int) uInstr & 7));
            case 0x20:
            case 0x21:
            case 0x22:
            case 0x23:
            case 0x24:
            case 0x25:
            case 0x26:
            case 0x27:
                // address register indirect with predecrement
                return new PredecrementMemoryOperand(dataWidth, Registers.AddressRegister(uInstr & 7));
            case 0x28:
            case 0x29:
            case 0x2a:
            case 0x2b:
            case 0x2c:
            case 0x2d:
            case 0x2e:
            case 0x2f:
                // address register indirect with displacement
                if (!rdr.TryReadBeInt16(out short sDisplacement))
                    return null;
                return MemoryOperand.Indirect(
                    dataWidth,
                    Registers.AddressRegister(uInstr & 7),
                    Constant.Int16(sDisplacement));
            case 0x30:
            case 0x31:
            case 0x32:
            case 0x33:
            case 0x34:
            case 0x35:
            case 0x36:
            case 0x37:
                // address register indirect with index
                if (!rdr.TryReadBeUInt16(out ushort extension))
                {
                    return null;
                }
                if (EXT_FULL(extension))
                {
                    if (EXT_EFFECTIVE_ZERO(extension))
                    {
                        return Address.Ptr32(0);
                    }

                    RegisterStorage? base_reg = null;
                    RegisterStorage? index_reg = null;
                    PrimitiveType? index_reg_width = null;
                    int index_scale = 1;
                    Constant? @base = null;
                    if (!EXT_BASE_DISPLACEMENT_VALID(extension))
                        return null;
                    if (EXT_BASE_DISPLACEMENT_PRESENT(extension))
                    {
                        if (!rdr.TryReadBe(EXT_BASE_DISPLACEMENT_LONG(extension) ? PrimitiveType.Word32 : PrimitiveType.Int16, out @base))
                            return null;
                    }
                    Constant? outer = null;
                    if (EXT_OUTER_DISPLACEMENT_PRESENT(extension))
                    {
                        if (!rdr.TryReadBe(EXT_OUTER_DISPLACEMENT_LONG(extension) ? PrimitiveType.Word32 : PrimitiveType.Int16, out outer))
                            return null;
                    }
                    if (EXT_BASE_REGISTER_PRESENT(extension))
                    {
                        base_reg = Registers.AddressRegister((int) uInstr & 7);
                    }
                    else if (!EXT_BASE_DISPLACEMENT_PRESENT(extension))
                        return null;    // Neither a base reg nor a base displacement present.
                    if (EXT_INDEX_REGISTER_PRESENT(extension))
                    {
                        index_reg = EXT_INDEX_AR(extension)
                            ? Registers.AddressRegister((int) EXT_INDEX_REGISTER(extension))
                            : Registers.DataRegister((int) EXT_INDEX_REGISTER(extension));
                        index_reg_width = EXT_INDEX_LONG(extension) ? PrimitiveType.Word32 : PrimitiveType.Word16;
                        if (EXT_INDEX_SCALE(extension) != 0)
                            index_scale = 1 << EXT_INDEX_SCALE(extension);
                    }
                    var preindex = (extension & 7) > 0 && (extension & 7) < 4;
                    var postindex = (extension & 7) > 4;
                    return new IndexedOperand(dataWidth, @base, outer, base_reg, index_reg, index_reg_width, index_scale, preindex, postindex);
                }
                else
                {
                    return new IndirectIndexedOperand(
                        dataWidth,
                        EXT_8BIT_DISPLACEMENT(extension),
                        Registers.AddressRegister((int) uInstr & 7),
                        EXT_INDEX_AR(extension)
                            ? Registers.AddressRegister((int) EXT_INDEX_REGISTER(extension))
                            : Registers.DataRegister((int) EXT_INDEX_REGISTER(extension)),
                        EXT_INDEX_LONG(extension) ? PrimitiveType.Word32 : PrimitiveType.Int16,
                        1 << EXT_INDEX_SCALE(extension));
                }
            case 0x38:
                // Absolute short address
                if (!rdr.TryReadBeUInt16(out ushort usAddr))
                    return null;
                return Address.Ptr32(usAddr);
            case 0x39:
                // Absolute long address
                if (!rdr.TryReadBeUInt32(out uint uAddr))
                    return null;
                return Address.Ptr32(uAddr);
            case 0x3A:
                // Program counter with displacement
                var off = rdr.Address - addr;
                short sOffset;
                if (!rdr.TryReadBeInt16(out sOffset))
                {
                    return null;
                }
                off += sOffset;
                return new MemoryOperand(dataWidth, Registers.pc, Constant.Int16((short) off));
            case 0x3B:
                // Program counter with index
                var extension_offset = (short) (rdr.Address - addr);
                var addrExt = rdr.Address;
                if (!rdr.TryReadBeUInt16(out extension))
                {
                    return null;
                }

                if (EXT_FULL(extension))
                {
                    if (EXT_EFFECTIVE_ZERO(extension))
                    {
                        return null;
                    }
                    Constant? @base = null;
                    Constant? outer = null;
                    if (!EXT_BASE_DISPLACEMENT_VALID(extension))
                        return null;
                    if (EXT_BASE_DISPLACEMENT_PRESENT(extension))
                    {
                        if (!rdr.TryReadBe(
                                EXT_BASE_DISPLACEMENT_LONG(extension)
                                    ? PrimitiveType.Word32
                                    : PrimitiveType.Int16,
                                out @base))
                            return null;
                    }
                    if (EXT_OUTER_DISPLACEMENT_PRESENT(extension))
                    {
                        if (!rdr.TryReadBe(
                                EXT_OUTER_DISPLACEMENT_LONG(extension)
                                    ? PrimitiveType.Word32
                                    : PrimitiveType.Int16,
                                out outer))
                            return null;
                    }
                    RegisterStorage? base_reg = EXT_BASE_REGISTER_PRESENT(extension)
                        ? Registers.pc
                        : null;
                    if (@base is null && base_reg is null)
                        return null;
                    RegisterStorage? index_reg = null;
                    PrimitiveType? index_width = null;
                    int index_scale = 0;
                    if (EXT_INDEX_REGISTER_PRESENT(extension))
                    {
                        index_reg = EXT_INDEX_AR(extension)
                            ? Registers.AddressRegister((int) EXT_INDEX_REGISTER(extension))
                            : Registers.DataRegister((int) EXT_INDEX_REGISTER(extension));
                        index_width = EXT_INDEX_LONG(extension) ? PrimitiveType.Word32 : PrimitiveType.Int16;
                        index_scale = EXT_INDEX_SCALE(extension) != 0
                            ? 1 << EXT_INDEX_SCALE(extension)
                            : 0;
                    }
                    return new IndexedOperand(dataWidth, @base, outer, base_reg, index_reg, index_width, index_scale,
                        (extension & 7) > 0 && (extension & 7) < 4,
                        (extension & 7) > 4);
                }
                return new IndirectIndexedOperand(
                    dataWidth,
                    (sbyte) (extension_offset + EXT_8BIT_DISPLACEMENT(extension)),
                    Registers.pc,
                    EXT_INDEX_AR(extension)
                        ? Registers.AddressRegister((int) EXT_INDEX_REGISTER(extension))
                        : Registers.DataRegister((int) EXT_INDEX_REGISTER(extension)),
                    EXT_INDEX_LONG(extension)
                        ? PrimitiveType.Word32
                        : PrimitiveType.Int16,
                    1 << EXT_INDEX_SCALE(extension));
            case 0x3C:
                // Immediate 
                if (dataWidth.Size == 1)        // don't want the instruction stream to get misaligned!
                {
                    rdr.Offset += 1;
                }
                if (!rdr.TryReadBe(dataWidth, out Constant? coff))
                {
                    return null;
                }
                return coff;
            }
            return null;
        }

        /* ======================================================================== */
        /* ========================= INSTRUCTION HANDLERS ========================= */
        /* ======================================================================== */
        /* Instruction handler function names follow this convention:
         *
         * d68000_NAME_EXTENSIONS()
         * where NAME is the name of the opcode it handles and EXTENSIONS are any
         * extensions for special instances of that opcode.
         *
         * Examples:
         *   d68000_add_er_8(): add opcode, from effective address to register,
         *                      size = byte
         *
         *   d68000_asr_s_8(): arithmetic shift right, static count, size = byte
         *
         *
         * Common extensions:
         * 8   : size = byte
         * 16  : size = word
         * 32  : size = long
         * rr  : register to register
         * mm  : memory to memory
         * r   : register
         * s   : static
         * er  : effective address -> register
         * re  : register -> effective address
         * ea  : using effective address mode of operation
         * d   : data register direct
         * a   : address register direct
         * ai  : address register indirect
         * pi  : address register indirect with postincrement
         * pd  : address register indirect with predecrement
         * di  : address register indirect with displacement
         * ix  : address register indirect with index
         * aw  : absolute word
         * al  : absolute long
         */

        private static bool d68000_illegal(uint uInstr, M68kDisassembler dasm)
        {
            return false;
            //throw new NotSupportedException(string.Format("dc.w    ${0:X}; ILLEGAL", dasm.instruction));
        }

        private static bool d68000_1010(uint uInstr, M68kDisassembler dasm)
        {
            trace.Verbose("dc.w    ${0:X4}; opcode 1010", uInstr);
            dasm.iclass = InstrClass.Invalid;
            dasm.mnemonic = Mnemonic.illegal;
            dasm.ops.Add(Constant.Word16((ushort) uInstr));
            return true;
        }

        private static bool d68000_1111(uint uInstr, M68kDisassembler dasm)
        {
            trace.Verbose("dc.w    ${0:X4}; opcode 1111", uInstr);
            return false;
        }

        private static bool d68010_bkpt(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68010_PLUS);
            dasm.mnemonic = Mnemonic.bkpt;
            dasm.iclass = LinPriv;
            dasm.ops.Add(Constant.Byte((byte) (uInstr & 7)));
            return true;
        }

        private static bool d68020_bfchg(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            if ((extension & 0xF000) != 0)
                return false;
            MachineOperand offset;
            if (BIT_B(extension))
                offset = get_data_reg((uint) (extension >> 6) & 7);
            else
                offset = Constant.Byte((byte) (extension >> 6 & 31));

            MachineOperand srcOp;
            if (BIT_5(extension))
                srcOp = Registers.DataRegister(extension & 7);
            else
                srcOp = Constant.UInt32(g_5bit_data_table[extension & 31]);

            dasm.mnemonic = Mnemonic.bfchg;
            var op = dasm.DecodeEa8(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(new BitfieldOperand(PrimitiveType.Word32, offset, srcOp));
            return true;
        }

        private static bool d68020_bfclr(uint uInstr, M68kDisassembler dasm)
        {
            MachineOperand offset;
            MachineOperand width;

            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            if (BIT_B(extension))
                offset = get_data_reg((uint) (extension >> 6) & 7);
            else
                offset = Constant.Byte((byte) (extension >> 6 & 31));
            if (BIT_5(extension))
                width = Registers.DataRegister(extension & 7);
            else
                width = Constant.UInt32(g_5bit_data_table[extension & 31]);
            dasm.mnemonic = Mnemonic.bfclr;
            var op = dasm.DecodeEa8(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(new BitfieldOperand(PrimitiveType.Word32, offset, width));
            return true;
        }

        private static bool d68020_bfexts(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            MachineOperand offset;
            if (BIT_B(extension))
                offset = Registers.DataRegister((uint) (extension >> 6) & 7);
            else
                offset = Constant.Byte((byte) (extension >> 6 & 31));
            MachineOperand width;
            if (BIT_5(extension))
                width = Registers.DataRegister(extension & 7);
            else
                width = Constant.UInt32(g_5bit_data_table[extension & 31]);
            dasm.mnemonic = Mnemonic.bfexts;
            dasm.ops.Add(get_data_reg((uint) (extension >> 12) & 7));
            var op = dasm.DecodeEa8(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(new BitfieldOperand(PrimitiveType.Word32, offset, width));
            return true;
        }

        private static bool d68020_bfextu(uint uInstr, M68kDisassembler dasm)
        {
            return false;
            //uint extension;
            //string offset; ;
            //string width; ;

            //dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);

            //extension = dasm.read_imm_16();

            //if (BIT_B(extension))
            //    offset = string.Format("D{0}", (extension >> 6) & 7);
            //else
            //    offset = string.Format("{0}", (extension >> 6) & 31);
            //if (BIT_5(extension))
            //    width = string.Format("D{0}", extension & 7);
            //else
            //    width = string.Format("{0}", g_5bit_data_table[extension & 31]);
            //dasm.g_dasm_str = string.Format("bfextu  D{0},{1} {{{2}:{3}}}; (2+)", (extension >> 12) & 7, dasm.get_ea_mode_str_8(dasm.instruction), offset, width);
            //throw new NotImplementedException();
        }

        private static bool d68020_bfffo(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            var offset = BIT_B(extension)
                ? Registers.DataRegister(extension >> 6 & 7)
                : (MachineOperand) Constant.Int32(extension >> 6 & 31);
            var width = BIT_5(extension)
                ? Registers.DataRegister(extension & 7)
                : (MachineOperand) Constant.Int32((int) g_5bit_data_table[extension & 31]);
            dasm.mnemonic = Mnemonic.bfffo;
            dasm.ops.Add(get_data_reg((uint) (extension >> 12) & 7));
            var op = dasm.DecodeEa8(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(new BitfieldOperand(PrimitiveType.Word32, offset, width));
            return true;
        }

        private static bool d68020_bfins(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            var offset = BIT_B(extension)
                ? Registers.DataRegister(extension >> 6 & 7)
                : (MachineOperand) Constant.Int32(extension >> 6 & 31);
            var width = BIT_5(extension)
                ? Registers.DataRegister(extension & 7)
                : (MachineOperand) Constant.Int32((int) g_5bit_data_table[extension & 31]);
            dasm.mnemonic = Mnemonic.bfins;
            dasm.ops.Add(Registers.DataRegister(extension >> 12 & 7));
            var op = dasm.DecodeEa8(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(new BitfieldOperand(PrimitiveType.Word32, offset, width));
            return true;
        }

        private static bool d68020_bfset(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            var offset = BIT_B(extension)
                ? Registers.DataRegister(extension >> 6 & 7)
                : (MachineOperand) Constant.Int32(extension >> 6 & 31);
            var width = BIT_5(extension)
                ? Registers.DataRegister(extension & 7)
                : (MachineOperand) Constant.Int32((int) g_5bit_data_table[extension & 31]);
            dasm.mnemonic = Mnemonic.bfins;
            var op = dasm.DecodeEa8(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(new BitfieldOperand(PrimitiveType.Word32, offset, width));
            return true;
        }

        private static bool d68020_bftst(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            var offset = BIT_B(extension)
                ? Registers.DataRegister(extension >> 6 & 7)
                : (MachineOperand) Constant.Int32(extension >> 6 & 31);
            var width = BIT_5(extension)
                ? Registers.DataRegister(extension & 7)
                : (MachineOperand) Constant.Int32((int) g_5bit_data_table[extension & 31]);
            dasm.mnemonic = Mnemonic.bftst;
            var op = dasm.DecodeEa8(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(new BitfieldOperand(PrimitiveType.Word32, offset, width));
            return true;
        }

        private static bool d68020_callm(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_ONLY);
            if (!dasm.TryReadImm8(out byte b))
                return false;
            dasm.mnemonic = Mnemonic.callm;
            dasm.iclass = InstrClass.Transfer | InstrClass.Call;
            dasm.ops.Add(Constant.Byte(b));
            var op = dasm.DecodeEa8(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        private static bool d68020_cas_8(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            dasm.mnemonic = Mnemonic.cas;
            dasm.iclass = InstrClass.Linear;
            dasm.dataWidth = PrimitiveType.Byte;
            dasm.ops.Add(get_data_reg(extension & 7u));
            dasm.ops.Add(get_data_reg((uint) (extension >> 8) & 7));
            var op = dasm.DecodeEa8(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        private static bool d68020_cas_16(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            dasm.mnemonic = Mnemonic.cas;
            dasm.iclass = InstrClass.Linear;
            dasm.dataWidth = PrimitiveType.Word16;
            dasm.ops.Add(get_data_reg(extension & 7u));
            dasm.ops.Add(get_data_reg((uint) (extension >> 8) & 7));
            var op = dasm.DecodeEa16(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        private static bool d68020_cas_32(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            dasm.mnemonic = Mnemonic.cas;
            dasm.iclass = InstrClass.Linear;
            dasm.dataWidth = PrimitiveType.Word16;
            dasm.ops.Add(get_data_reg(extension & 7u));
            dasm.ops.Add(get_data_reg((uint) (extension >> 8) & 7));
            var op = dasm.DecodeEa32(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        private static bool d68020_cas2_16(uint uInstr, M68kDisassembler dasm)
        {
            /* CAS2 Dc1:Dc2,Du1:Dc2:(Rn1):(Rn2)
            f e d c b a 9 8 7 6 5 4 3 2 1 0
             DARn1  0 0 0  Du1  0 0 0  Dc1
             DARn2  0 0 0  Du2  0 0 0  Dc2
            */

            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt32(out uint extension))
                return false;
            dasm.g_dasm_str = string.Format("cas2.w  D{0}:D%d:D%d:D%d, (%c%d):(%c%d); (2+)",
                extension >> 16 & 7, extension & 7, extension >> 22 & 7, extension >> 6 & 7,
                BIT_1F(extension) ? 'A' : 'D', extension >> 28 & 7,
                BIT_F(extension) ? 'A' : 'D', extension >> 12 & 7);
            return false;
        }

        private static bool d68020_cas2_32(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (dasm.rdr.TryReadBeUInt32(out uint extension))
                return false;
            dasm.g_dasm_str = string.Format("cas2.l  D{0}:D%d:D%d:D%d, (%c%d):(%c%d); (2+)",
                extension >> 16 & 7, extension & 7, extension >> 22 & 7, extension >> 6 & 7,
                BIT_1F(extension) ? 'A' : 'D', extension >> 28 & 7,
                BIT_F(extension) ? 'A' : 'D', extension >> 12 & 7);
            return false;
        }

        private static bool d68000_chk_16(uint uInstr, M68kDisassembler dasm)
        {
            dasm.mnemonic = Mnemonic.chk;
            dasm.iclass = InstrClass.Linear;
            dasm.dataWidth = PrimitiveType.Word16;
            var op = dasm.DecodeEa16(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(get_data_reg(uInstr >> 9 & 7));
            return true;
        }

        private static bool d68020_chk_32(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            dasm.mnemonic = Mnemonic.chk;
            dasm.iclass = InstrClass.Linear;
            dasm.dataWidth = PrimitiveType.Word32;
            var op = dasm.DecodeEa32(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(get_data_reg(uInstr >> 9 & 7));
            return true;
        }

        private static bool d68020_chk2_cmp2_8(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            dasm.mnemonic = BIT_B(extension) ? Mnemonic.chk2 : Mnemonic.cmp2;
            dasm.iclass = InstrClass.Invalid;
            dasm.dataWidth = PrimitiveType.Byte;
            var op = dasm.DecodeEa8(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(get_addr_or_data_reg(BIT_F(extension), (uint) (extension >> 12) & 7));
            return true;
        }

        private static bool d68020_chk2_cmp2_16(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            dasm.mnemonic = BIT_B(extension) ? Mnemonic.chk2 : Mnemonic.cmp2;
            dasm.iclass = InstrClass.Linear;
            dasm.dataWidth = PrimitiveType.Word16;
            var op = dasm.DecodeEa16(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(get_addr_or_data_reg(BIT_F(extension), (uint) (extension >> 12) & 7));
            return true;
        }

        private static bool d68020_chk2_cmp2_32(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            dasm.mnemonic = BIT_B(extension) ? Mnemonic.chk2 : Mnemonic.cmp2;
            dasm.iclass = InstrClass.Linear;
            dasm.dataWidth = PrimitiveType.Word32;
            var op = dasm.DecodeEa32(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(get_addr_or_data_reg(BIT_F(extension), (uint) (extension >> 12) & 7));
            return true;
        }

        private static bool d68040_cinv(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68040_PLUS);
            var cache = RegisterStorage.Reg32(
                g_caches[uInstr >> 6 & 3],
                0);
            if (cache.Name == "")
                return false;
            dasm.iclass = LinPriv;
            switch (uInstr >> 3 & 3)
            {
            case 0:
                dasm.iclass = InstrClass.Invalid;
                dasm.mnemonic = Mnemonic.illegal;
                break;
            case 1:
                dasm.mnemonic = Mnemonic.cinvl;
                dasm.ops.Add(cache);
                dasm.ops.Add(MemoryOperand.Indirect(PrimitiveType.Word32, Registers.AddressRegister((int) uInstr & 7)));
                break;
            case 2:
                dasm.mnemonic = Mnemonic.cinvp;
                dasm.ops.Add(cache);
                dasm.ops.Add(MemoryOperand.Indirect(PrimitiveType.Word32, Registers.AddressRegister((int) uInstr & 7)));
                break;
            case 3:
                dasm.mnemonic = Mnemonic.cinva;
                dasm.ops.Add(cache);
                break;
            }
            return true;
        }

        private static bool d68020_cpbcc_16(uint uInstr, M68kDisassembler dasm)
        {
            var new_pc = dasm.rdr.Address;
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            Mnemonic mnemonic = g_cpcc[uInstr & 0x3f];
            if (mnemonic == Mnemonic.illegal)
                return false;
            if (!dasm.rdr.TryReadBeInt16(out var displacement))
                return false;
            dasm.mnemonic = mnemonic;
            dasm.iclass = InstrClass.ConditionalTransfer;
            dasm.ops.Add(new_pc + displacement);
            return true;
        }

        private static bool d68020_cpbcc_32(uint uInstr, M68kDisassembler dasm)
        {
            var new_pc = dasm.rdr.Address;
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            if (!dasm.rdr.TryReadBeInt32(out int sDisplacement))
                return false;
            new_pc += sDisplacement;
            if (!dasm.get_imm_str_s16(out var imm))
                return false;

            dasm.g_dasm_str = string.Format("%db%-4s  %s; %x (extension = %x) (2-3)", uInstr >> 9 & 7, g_cpcc[uInstr & 0x3f], imm, new_pc, extension);
            return true;
        }

        private static bool d68020_cpdbcc(uint uInstr, M68kDisassembler dasm)
        {
            var new_pc = dasm.rdr.Address;
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension1))
                return false;
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension2))
                return false;
            if (!dasm.rdr.TryReadBeInt16(out short sDisplacement))
                return false;
            new_pc += sDisplacement;
            if (!dasm.get_imm_str_s16(out var imm))
                return false;
            dasm.g_dasm_str = string.Format("%ddb%-4s D%d,%s; %x (extension = %x) (2-3)", uInstr >> 9 & 7, g_cpcc[extension1 & 0x3f], uInstr & 7, imm, new_pc, extension2);
            return true;
        }

        private static bool d68020_cpgen(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt32(out uint u))
                return false;
            dasm.g_dasm_str = string.Format("%dgen    %s; (2-3)", uInstr >> 9 & 7, u);
            return false;
        }

        private static bool d68020_cprestore(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if ((uInstr >> 9 & 7) == 1)
            {
                dasm.mnemonic = Mnemonic.frestore;
                var op = dasm.DecodeEa8(uInstr);
                if (op is null)
                    return false;
                dasm.ops.Add(op);
                return true;
            }
            else
            {
                dasm.g_dasm_str = string.Format("%drestore %s; (2-3)", uInstr >> 9 & 7, dasm.DecodeEa8(uInstr));
            }
            return false;
        }

        private static bool d68020_cpsave(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if ((uInstr >> 9 & 7) == 1)
            {
                dasm.mnemonic = Mnemonic.fsave;
                var op = dasm.DecodeEa8(uInstr);
                if (op is null)
                    return false;
                dasm.ops.Add(op);
                return true;
            }
            else
            {
                dasm.g_dasm_str = string.Format("{0}save   {1}; (2-3)", uInstr >> 9 & 7, dasm.DecodeEa8(uInstr));
            }
            return false;
        }

        private static bool d68020_cpscc(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            // 0 - MMU
            // 1 - MC68881/2 FPU
            int cooprocessor_id = (int) (uInstr >> 9) & 7;
            if (dasm.rdr.TryReadBeUInt16(out var extension1))
                return false;
            dasm.g_dasm_str = string.Format("{0}cpS{1}  %s; (extension = %x) (2-3)", cooprocessor_id, g_cpcc[extension1 & 0x3f], dasm.DecodeEa8(uInstr));
            return false;
        }

        private static bool d68020_cptrapcc_0(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (dasm.rdr.TryReadBeUInt16(out var extension1))
                return false;
            if (dasm.rdr.TryReadBeUInt16(out var extension2))
                return false;
            dasm.g_dasm_str = string.Format("{0}cptrap{1,4}; (extension = {2:X}) (2-3)", uInstr >> 9 & 7, g_cpcc[extension1 & 0x3f], extension2);
            return false;
        }

        private static bool d68020_cptrapcc_16(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out var extension1))
                return false;
            if (!dasm.rdr.TryReadBeUInt16(out var extension2))
                return false;
            if (!dasm.rdr.TryReadBeInt16(out var s))
                return false;
            dasm.g_dasm_str = string.Format("{0}trap{1,4} {2}; (extension = {3}) (2-3)", uInstr >> 9 & 7, g_cpcc[extension1 & 0x3f], s, extension2);
            return false;
        }

        private static bool d68020_cptrapcc_32(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out var extension1))
                return false;
            if (!dasm.rdr.TryReadBeUInt16(out var extension2))
                return false;
            if (!dasm.rdr.TryReadBeUInt32(out uint u))
                return false;
            dasm.g_dasm_str = string.Format("%dtrap%-4s %s; (extension = %x) (2-3)",
                uInstr >> 9 & 7, g_cpcc[extension1 & 0x3f],
                Constant.UInt32(u),
                extension2);
            return false;
        }

        private static bool d68040_cpush(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68040_PLUS);
            switch (uInstr >> 3 & 3)
            {
            case 0:
                dasm.g_dasm_str = string.Format("cpush (illegal scope); (4)");
                break;
            case 1:
                dasm.g_dasm_str = string.Format("cpushl  {0}, (A{1}); (4)", uInstr >> 6 & 3, uInstr & 7);
                break;
            case 2:
                dasm.g_dasm_str = string.Format("cpushp  {0}, (A{1}); (4)", uInstr >> 6 & 3, uInstr & 7);
                break;
            case 3:
                dasm.g_dasm_str = string.Format("cpusha  {0}; (4)", uInstr >> 6 & 3);
                break;
            }
            return false;
        }

        private static bool d68000_dbcc(uint uInstr, M68kDisassembler dasm)
        {
            Address temp_pc = dasm.rdr.Address;
            if (!dasm.rdr.TryReadBeInt16(out short sDisplacement))
                return false;
            if ((sDisplacement & 1) != 0)
                return false;
            dasm.iclass = InstrClass.ConditionalTransfer;
            dasm.mnemonic = g_dbcc[uInstr >> 8 & 0xf];
            dasm.ops.Add(get_data_reg(uInstr & 7));
            dasm.ops.Add(temp_pc + sDisplacement);
            return true;
        }

        private static bool d68020_divl(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            var op = dasm.DecodeEa32(uInstr);
            if (op is null)
                return false;
            Mnemonic code;
            if (BIT_B(extension))
            {
                code = BIT_A(extension) ? Mnemonic.divs : Mnemonic.divsl;
            }
            else
            {
                code = BIT_A(extension) ? Mnemonic.divu : Mnemonic.divul;
            }
            var dq = (uint) (extension >> 12) & 7;
            var dr = (uint) (extension & 7);
            MachineOperand op2;
            PrimitiveType dataWidth;
            if (BIT_A(extension))
            {
                op2 = dasm.get_double_data_reg(dr, dq);
                dataWidth = PrimitiveType.Int64;
            }
            else if (dr == dq)
            {
                op2 = get_data_reg(dq);
                dataWidth = PrimitiveType.Int32;
            }
            else
            {
                op2 = dasm.get_double_data_reg(dr, dq);
                dataWidth = PrimitiveType.Int32;
            }

            dasm.mnemonic = code;
            dasm.iclass = InstrClass.Linear;
            dasm.dataWidth = dataWidth;
            dasm.ops.Add(op);
            dasm.ops.Add(op2);
            return true;
        }

        private static readonly PrimitiveType?[] float_data_format = new PrimitiveType?[8]
        {
            PrimitiveType.Int32,  // ".l",
            PrimitiveType.Real32, // ".s",
            PrimitiveType.Real96, // ".x",
            null,                 // ".p", 
            PrimitiveType.Int16,  // ".w",
            PrimitiveType.Real64, // ".d", 
            PrimitiveType.Byte,   // ".b",
            null,                 // ".p"
	    };

        private static bool d68040_fpu(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68030_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out dasm.fpWord))
                return false;

            uint src = (uint) (dasm.fpWord >> 10) & 0x7;
            uint dst_reg = (uint) (dasm.fpWord >> 7) & 0x7;

            // special override for FMOVECR
            if ((dasm.fpWord >> 13 & 0x7) == 2 && (dasm.fpWord >> 10 & 0x7) == 7)
            {
                dasm.mnemonic = Mnemonic.fmovecr;
                dasm.ops.Add(Constant.Byte((byte) (dasm.fpWord & 0x7f)));
                dasm.ops.Add(dasm.get_fp_reg((int) dst_reg));
                return true;
            }

            switch (dasm.fpWord >> 13 & 0x7)
            {
            case 0x0:
            case 0x2:
                {
                    Mnemonic mnemonic;
                    switch (dasm.fpWord & 0x7f)
                    {
                    case 0x00: mnemonic = Mnemonic.fmove; break;
                    case 0x01: mnemonic = Mnemonic.fint; break;
                    case 0x02: mnemonic = Mnemonic.fsinh; break;
                    case 0x03: mnemonic = Mnemonic.fintrz; break;
                    case 0x04: mnemonic = Mnemonic.fsqrt; break;
                    case 0x06: mnemonic = Mnemonic.flognp1; break;
                    case 0x08: mnemonic = Mnemonic.fetoxm1; break;
                    case 0x09: mnemonic = Mnemonic.ftanh1; break;
                    case 0x0a: mnemonic = Mnemonic.fatan; break;
                    case 0x0c: mnemonic = Mnemonic.fasin; break;
                    case 0x0d: mnemonic = Mnemonic.fatanh; break;
                    case 0x0e: mnemonic = Mnemonic.fsin; break;
                    case 0x0f: mnemonic = Mnemonic.ftan; break;
                    case 0x10: mnemonic = Mnemonic.fetox; break;
                    case 0x11: mnemonic = Mnemonic.ftwotox; break;
                    case 0x12: mnemonic = Mnemonic.ftentox; break;
                    case 0x14: mnemonic = Mnemonic.flogn; break;
                    case 0x15: mnemonic = Mnemonic.flog10; break;
                    case 0x16: mnemonic = Mnemonic.flog2; break;
                    case 0x18: mnemonic = Mnemonic.fabs; break;
                    case 0x19: mnemonic = Mnemonic.fcosh; break;
                    case 0x1a: mnemonic = Mnemonic.fneg; break;
                    case 0x1c: mnemonic = Mnemonic.facos; break;
                    case 0x1d: mnemonic = Mnemonic.fcos; break;
                    case 0x1e: mnemonic = Mnemonic.fgetexp; break;
                    case 0x1f: mnemonic = Mnemonic.fgetman; break;
                    case 0x20: mnemonic = Mnemonic.fdiv; break;
                    case 0x21: mnemonic = Mnemonic.fmod; break;
                    case 0x22: mnemonic = Mnemonic.fadd; break;
                    case 0x23: mnemonic = Mnemonic.fmul; break;
                    case 0x24: mnemonic = Mnemonic.fsgldiv; break;
                    case 0x25: mnemonic = Mnemonic.frem; break;
                    case 0x26: mnemonic = Mnemonic.fscale; break;
                    case 0x27: mnemonic = Mnemonic.fsglmul; break;
                    case 0x28: mnemonic = Mnemonic.fsub; break;
                    case 0x30:
                    case 0x31:
                    case 0x32:
                    case 0x33:
                    case 0x34:
                    case 0x35:
                    case 0x36:
                    case 0x37:
                        mnemonic = Mnemonic.fsincos; break;
                    case 0x38: mnemonic = Mnemonic.fcmp; break;
                    case 0x3a: mnemonic = Mnemonic.ftst; break;
                    case 0x41: mnemonic = Mnemonic.fssqrt; break;
                    case 0x45: mnemonic = Mnemonic.fdsqrt; break;
                    case 0x58: mnemonic = Mnemonic.fsabs; break;
                    case 0x5a: mnemonic = Mnemonic.fsneg; break;
                    case 0x5c: mnemonic = Mnemonic.fdabs; break;
                    case 0x5e: mnemonic = Mnemonic.fdneg; break;
                    case 0x60: mnemonic = Mnemonic.fsdiv; break;
                    case 0x62: mnemonic = Mnemonic.fsadd; break;
                    case 0x63: mnemonic = Mnemonic.fsmul; break;
                    case 0x64: mnemonic = Mnemonic.fddiv; break;
                    case 0x66: mnemonic = Mnemonic.fdadd; break;
                    case 0x67: mnemonic = Mnemonic.fdmul; break;
                    case 0x68: mnemonic = Mnemonic.fssub; break;
                    case 0x6c: mnemonic = Mnemonic.fdsub; break;

                    default: return false;
                    }

                    if ((dasm.fpWord & 0x4000) != 0)
                    {
                        dasm.mnemonic = mnemonic;
                        dasm.dataWidth = float_data_format[src];
                        if (dasm.dataWidth is null)
                            return false;
                        var op = dasm.DecodeEffectiveAddress(uInstr, dasm.dataWidth);
                        if (op is null)
                            return false;
                        dasm.ops.Add(op);
                        dasm.ops.Add(dasm.get_fp_reg((int) dst_reg));
                    }
                    else
                    {
                        dasm.mnemonic = mnemonic;
                        dasm.dataWidth = PrimitiveType.Real80;
                        dasm.ops.Add(dasm.get_fp_reg((int) src));
                        dasm.ops.Add(dasm.get_fp_reg((int) dst_reg));
                    }
                    return true;
                }

            case 0x3:
                {
                    switch (dasm.fpWord >> 10 & 7)
                    {
                    case 3:		// packed decimal w/fixed k-factor
                        dasm.mnemonic = Mnemonic.fmove;
                        dasm.dataWidth = float_data_format[dasm.fpWord >> 10 & 7];
                        if (dasm.dataWidth is null)
                            return false;
                        dasm.ops.Add(dasm.get_fp_reg((int) dst_reg));
                        var op = dasm.DecodeEa32(uInstr);
                        if (op is null)
                            return false;
                        dasm.ops.Add(op);
                        // sext_7bit_int((int)w2 & 0x7f));
                        return true;
                    case 7:		// packed decimal w/dynamic k-factor (register)
                        dasm.mnemonic = Mnemonic.fmove;
                        dasm.dataWidth = float_data_format[dasm.fpWord >> 10 & 7];
                        if (dasm.dataWidth is null)
                            return false;
                        dasm.ops.Add(dasm.get_fp_reg((int) dst_reg));
                        op = dasm.DecodeEa32(uInstr);
                        if (op is null)
                            return false;
                        dasm.ops.Add(op);
                        dasm.ops.Add(Constant.Byte((byte) (dasm.fpWord >> 4 & 7)));
                        break;

                    default:
                        dasm.mnemonic = Mnemonic.fmove;
                        dasm.dataWidth = float_data_format[dasm.fpWord >> 10 & 7];
                        if (dasm.dataWidth is null)
                            return false;
                        dasm.ops.Add(dasm.get_fp_reg((int) dst_reg));
                        op = dasm.DecodeEa32(uInstr);
                        if (op is null)
                            return false;
                        dasm.ops.Add(op);
                        return true;
                    }
                    break;
                }

            case 0x4:	// ea to control
                {
                    dasm.g_dasm_str = string.Format("fmovem.l   {0}, ", dasm.DecodeEa32(uInstr));
                    if ((dasm.fpWord & 0x1000) != 0) dasm.g_dasm_str += "fpcr";
                    if ((dasm.fpWord & 0x0800) != 0) dasm.g_dasm_str += "/fpsr";
                    if ((dasm.fpWord & 0x0400) != 0) dasm.g_dasm_str += "/fpiar";
                    break;
                }

            case 0x5:	// control to ea
                {
                    dasm.g_dasm_str = "fmovem.l   ";
                    if ((dasm.fpWord & 0x1000) != 0) dasm.g_dasm_str += "fpcr";
                    if ((dasm.fpWord & 0x0800) != 0) dasm.g_dasm_str += "/fpsr";
                    if ((dasm.fpWord & 0x0400) != 0) dasm.g_dasm_str += "/fpiar";
                    dasm.g_dasm_str += ", ";
                    dasm.g_dasm_str += dasm.DecodeEa32(uInstr);
                    break;
                }

            case 0x6:	// memory to FPU, list
                {
                    if ((dasm.fpWord >> 11 & 1) != 0)	// dynamic register list
                    {
                        dasm.mnemonic = Mnemonic.fmovem;
                        dasm.dataWidth = PrimitiveType.Real96;
                        var op = dasm.DecodeEa32(uInstr);
                        if (op is null)
                            return false;
                        dasm.ops.Add(op);
                        dasm.ops.Add(Registers.DataRegister(dasm.fpWord >> 4 & 7));
                    }
                    else	// static register list
                    {
                        dasm.mnemonic = Mnemonic.fmovem;
                        dasm.dataWidth = PrimitiveType.Real96;
                        var op = dasm.DecodeEa32(uInstr);
                        if (op is null)
                            return false;
                        dasm.ops.Add(op);
                        MachineOperand op2;
                        if ((dasm.fpWord >> 12 & 1) == 0)
                        {
                            op2 = RegisterSetOperand.CreateReversed((byte) dasm.fpWord, PrimitiveType.Real96);
                        }
                        else
                        {
                            op2 = new RegisterSetOperand((byte) dasm.fpWord, PrimitiveType.Real96);
                        }
                        dasm.ops.Add(op2);
                        return true;
                    }
                    break;
                }

            case 0x7:   // FPU to memory, list
                if ((dasm.fpWord >> 11 & 1) != 0)  // dynamic register list
                {
                    dasm.mnemonic = Mnemonic.fmovem;
                    dasm.dataWidth = PrimitiveType.Real96;
                    dasm.ops.Add(Registers.GetRegister( dasm.fpWord >> 4 & 7));
                    var op = dasm.DecodeEa32(uInstr);
                    if (op is null)
                        return false;
                    dasm.ops.Add(op);
                }
                else    // static register list
                {
                    dasm.mnemonic = Mnemonic.fmovem;
                    dasm.dataWidth = PrimitiveType.Real96;
                    if ((dasm.fpWord >> 12 & 1) == 0)
                    {
                        dasm.ops.Add(RegisterSetOperand.CreateReversed((ushort) (dasm.fpWord << 8), PrimitiveType.Real96));
                    }
                    else
                    {
                        dasm.ops.Add(new RegisterSetOperand(dasm.fpWord & 0xFFu, PrimitiveType.Real96));
                    }
                    var op = dasm.DecodeEa32(uInstr);
                    if (op is null || op is RegisterStorage)
                        return false;
                    dasm.ops.Add(op);
                }
                return true;
            }
            return false;
        }

        private static bool d68000_move_8(uint uInstr, M68kDisassembler dasm)
        {
            var op = dasm.DecodeEa8(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            op = dasm.DecodeEa8(uInstr >> 9 & 7 | uInstr >> 3 & 0x38);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(Registers.d1);

            dasm.mnemonic = Mnemonic.move;
            dasm.iclass = InstrClass.Linear;
            dasm.dataWidth = PrimitiveType.Byte;
            return true;
        }

        private static bool d68000_move_16(uint uInstr, M68kDisassembler dasm)
        {
            var op = dasm.DecodeEa16(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            op = dasm.DecodeEa16(uInstr >> 9 & 7 | uInstr >> 3 & 0x38);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(Registers.d2);

            dasm.mnemonic = Mnemonic.move;
            dasm.iclass = InstrClass.Linear;
            dasm.dataWidth = PrimitiveType.Word16;
            return true;
        }

        private static bool d68000_move_32(uint uInstr, M68kDisassembler dasm)
        {
            var op = dasm.DecodeEa32(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            op = dasm.DecodeEa32(uInstr >> 9 & 7 | uInstr >> 3 & 0x38);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            dasm.ops.Add(Registers.d4);

            dasm.mnemonic = Mnemonic.move;
            dasm.iclass = InstrClass.Linear;
            dasm.dataWidth = PrimitiveType.Word32;
            return true;
        }

        private static bool d68010_movec(uint uInstr, M68kDisassembler dasm)
        {
            MachineOperand reg_name;
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            uint regNumber = extension & 0xFFFu;
            if (Registers.sregsByCode.TryGetValue(regNumber, out var sreg))
                reg_name = sreg;
            else
                reg_name = Constant.Int16((short) (extension & 0xfff));

            var other_reg = BIT_F(extension)
                ? get_addr_reg((uint) (extension >> 12) & 7)
                : get_data_reg((uint) (extension >> 12) & 7);
            dasm.mnemonic = Mnemonic.movec;
            dasm.iclass = LinPriv;
            if (BIT_0(uInstr))
            {
                dasm.ops.Add(other_reg);
                dasm.ops.Add(reg_name);
            }
            else
            {
                dasm.ops.Add(reg_name);
                dasm.ops.Add(other_reg);
            }
            return true;
        }

        private static bool d68010_moves_8(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68010_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            var reg = get_addr_or_data_reg(BIT_F(extension), (uint) (extension >> 12) & 7);
            var ea = dasm.DecodeEa8(uInstr);
            if (ea is null)
                return false;
            dasm.mnemonic = Mnemonic.moves;
            dasm.dataWidth = PrimitiveType.Word16;
            if (BIT_B(extension))
            {
                dasm.ops.Add(reg);
                dasm.ops.Add(ea);
            }
            else
            {
                dasm.ops.Add(ea);
                dasm.ops.Add(reg);
            }
            return true;
        }

        private static bool d68010_moves_16(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68010_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            var reg = get_addr_or_data_reg(BIT_F(extension), (uint) (extension >> 12) & 7);
            var ea = dasm.DecodeEa16(uInstr);
            if (ea is null)
                return false;
            dasm.mnemonic = Mnemonic.moves;
            dasm.dataWidth = PrimitiveType.Word16;
            if (BIT_B(extension))
            {
                dasm.ops.Add(reg);
                dasm.ops.Add(ea);
            }
            else
            {
                dasm.ops.Add(ea);
                dasm.ops.Add(reg);
            }
            return true;
        }

        private static bool d68010_moves_32(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68010_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            if ((extension & 0b11_1111_1111) != 0)
                return false;
            var reg = get_addr_or_data_reg(BIT_F(extension), (uint) (extension >> 12) & 7);
            var op = dasm.DecodeEa32(uInstr);
            if (op is null)
                return false;
            dasm.mnemonic = Mnemonic.moves;
            dasm.dataWidth = PrimitiveType.Word16;
            if (BIT_B(extension))
            {
                dasm.ops.Add(reg);
                dasm.ops.Add(op);
            }
            else
            {
                dasm.ops.Add(op);
                dasm.ops.Add(reg);
            };
            return true;
        }

        private static bool d68040_move16_pi_pi(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68040_PLUS);
            if (dasm.rdr.TryReadBeUInt16(out ushort us))
                return false;
            dasm.mnemonic = Mnemonic.move16;
            dasm.iclass = InstrClass.Linear;
            dasm.ops.Add(dasm.get_post_inc((int) uInstr & 7, PrimitiveType.Word128));
            dasm.ops.Add(dasm.get_post_inc(us >> 12 & 7, PrimitiveType.Word128));
            return true;
        }

        private static bool d68040_move16_pi_al(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68040_PLUS);
            if (!dasm.rdr.TryReadBeUInt32(out uint uOp2))
                return false;
            dasm.mnemonic = Mnemonic.move16;
            dasm.iclass = InstrClass.Linear;
            dasm.ops.Add(dasm.get_post_inc((int) uInstr & 7, PrimitiveType.Word128));
            dasm.ops.Add(Constant.UInt32(uOp2));
            return true;
        }

        private static bool d68040_move16_al_pi(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68040_PLUS);
            if (!dasm.rdr.TryReadBeUInt32(out uint uOp1))
                return false;
            dasm.mnemonic = Mnemonic.move16;
            dasm.iclass = InstrClass.Linear;
            dasm.ops.Add(Constant.UInt32(uOp1));
            dasm.ops.Add(dasm.get_post_inc((int) uInstr & 7, PrimitiveType.Word128));
            return true;
        }

        private static bool d68040_move16_ai_al(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68040_PLUS);
            if (!dasm.rdr.TryReadBeUInt32(out uint uOp2))
                return false;
            dasm.mnemonic = Mnemonic.move16;
            dasm.ops.Add(new MemoryOperand(PrimitiveType.Word128, Registers.AddressRegister(uInstr & 7)));
            dasm.ops.Add(Constant.UInt32(uOp2));
            return true;
        }

        private static bool d68040_move16_al_ai(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68040_PLUS);
            if (!dasm.rdr.TryReadBeUInt32(out uint uOp1))
                return false;

            dasm.mnemonic = Mnemonic.move16;
            dasm.ops.Add(Constant.UInt32(uOp1));
            dasm.ops.Add(new MemoryOperand(PrimitiveType.Word128, Registers.AddressRegister(uInstr & 7)));
            return true;
        }

        private static bool d68020_mull(uint uInstr, M68kDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out var extension))
                return false;

            MachineOperand op2 = BIT_A(extension)
                ? dasm.get_double_data_reg(extension & 7u, (uint) (extension >> 12) & 7u)
                :  get_data_reg((uint) (extension >> 12) & 7);
            dasm.mnemonic = BIT_B(extension) ? Mnemonic.muls : Mnemonic.mulu;
            dasm.dataWidth = PrimitiveType.Word32;

            var op1 = dasm.DecodeEffectiveAddress(uInstr, PrimitiveType.Word32);
            if (op1 is null)
                return false;
            dasm.ops.Add(op1);
            dasm.ops.Add(op2);
            return true;
        }

        private static bool d68020_pack_rr(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort uImm))
                return false;
            dasm.mnemonic = Mnemonic.pack;
            dasm.iclass = InstrClass.Linear;
            dasm.ops.Add(get_data_reg(uInstr & 7));
            dasm.ops.Add(get_data_reg(uInstr >> 9 & 7));
            dasm.ops.Add(Constant.UInt16(uImm));
            return true;
        }

        private static bool d68020_pack_mm(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort uImm))
                return false;
            dasm.mnemonic = Mnemonic.pack;
            dasm.iclass = InstrClass.Linear;
            dasm.ops.Add(dasm.get_pre_dec(uInstr, 0, PrimitiveType.Word16));
            dasm.ops.Add(dasm.get_pre_dec(uInstr, 9, PrimitiveType.Word16));
            dasm.ops.Add(Constant.UInt16(uImm));
            return true;
        }

        // this is a 68040-specific form of PFLUSH
        private static bool d68040_pflush(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68040_PLUS);

            if ((uInstr & 0x10) != 0)
            {
                dasm.g_dasm_str = string.Format("pflusha{0}", (uInstr & 8) != 0 ? "" : "n");
            }
            else
            {
                dasm.g_dasm_str = string.Format("pflush{0}(A%d)", (uInstr & 8) != 0 ? "" : "n", uInstr & 7);
            }
            return false;
        }

        private static bool d68020_rtm(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_ONLY);
            uint reg = uInstr & 7;
            dasm.mnemonic = Mnemonic.rtm;
            dasm.ops.Add(BIT_3(uInstr)
                ? get_addr_reg(reg)
                : get_data_reg(reg));
            return true;
        }

        private static bool d68000_scc(uint uInstr, M68kDisassembler dasm)
        {
            dasm.mnemonic = g_scc[uInstr >> 8 & 0xf];
            dasm.iclass = InstrClass.Linear;
            var op = dasm.DecodeEa8(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        private static bool d68000_stop(uint uInstr, M68kDisassembler dasm)
        {
            if (!dasm.get_imm_str_s16(out var imm))
                return false;
            dasm.mnemonic = Mnemonic.stop;
            dasm.iclass = InstrClass.Terminates | InstrClass.Privileged;
            dasm.ops.Add(imm);
            return true;
        }

        private static bool d68000_tas(uint uInstr, M68kDisassembler dasm)
        {
            dasm.mnemonic = Mnemonic.tas;
            dasm.iclass = InstrClass.Linear;
            var op = dasm.DecodeEa8(uInstr);
            if (op is null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        private static bool d68000_trap(uint uInstr, M68kDisassembler dasm)
        {
            dasm.mnemonic = Mnemonic.trap;
            dasm.iclass = InstrClass.Call | InstrClass.Transfer;
            dasm.ops.Add(Constant.Byte((byte) (uInstr & 0xf)));
            return true;
        }

        private static bool d68020_trapcc_0(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            dasm.mnemonic = g_trapcc[uInstr >> 8 & 0xf];
            dasm.iclass = dasm.mnemonic != Mnemonic.trapf
                ? InstrClass.Call | InstrClass.Transfer
                : InstrClass.Linear;
            return true;
        }

        private static bool d68020_trapcc_16(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort uImm))
                return false;

            dasm.mnemonic = g_trapcc[uInstr >> 8 & 0xf];
            dasm.iclass = InstrClass.Call | InstrClass.Transfer;
            dasm.ops.Add(Constant.UInt16(uImm));
            return true;
        }

        private static bool d68020_trapcc_32(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt32(out uint uOp1))
                return false;

            dasm.mnemonic = g_trapcc[uInstr >> 8 & 0xf];
            dasm.iclass = InstrClass.Call | InstrClass.Transfer;
            dasm.ops.Add(Constant.UInt32(uOp1));
            return true;
        }

        private static bool d68020_unpk_rr(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort uImm))
                return false;
            dasm.mnemonic = Mnemonic.unpk;
            dasm.iclass = InstrClass.Linear;
            dasm.ops.Add(get_data_reg(uInstr & 7));
            dasm.ops.Add(get_data_reg(uInstr >> 9 & 7));
            dasm.ops.Add(Constant.UInt16(uImm));
            return true;
        }

        private static bool d68020_unpk_mm(uint uInstr, M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(uInstr, M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort uImm))
                return false;
            dasm.mnemonic = Mnemonic.unpk;
            dasm.iclass = InstrClass.Linear;
            dasm.ops.Add(dasm.get_pre_dec(uInstr, 0, PrimitiveType.Word16));
            dasm.ops.Add(dasm.get_pre_dec(uInstr, 9, PrimitiveType.Word16));
            dasm.ops.Add(Constant.UInt16(uImm));
            return true;
        }

        // PFLUSH:  001xxx0xxxxxxxxx
        // PLOAD:   001000x0000xxxxx
        // PVALID1: 0010100000000000
        // PVALID2: 0010110000000xxx
        // PMOVE 1: 010xxxx000000000
        // PMOVE 2: 011xxxx0000xxx00
        // PMOVE 3: 011xxxx000000000
        // PTEST:   100xxxxxxxxxxxxx
        // PFLUSHR: 1010000000000000
        private static bool d68851_p000(uint uInstr, M68kDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out ushort modes))
                return false;

            // do this after fetching the second PMOVE word so we properly get the 3rd if necessary
            var op = dasm.DecodeEa32(uInstr);
            if (op is null)
                return false;

            if ((modes & 0xfde0) == 0x2000)	// PLOAD
            {
                if ((modes & 0x0200) != 0)
                {
                    dasm.mnemonic = Mnemonic.pload;
                    dasm.iclass = InstrClass.Linear;
                    dasm.ops.Add(Constant.Byte((byte) (modes >> 10 & 7)));
                    dasm.ops.Add(op);
                    return true;
                }
                else
                {
                    dasm.mnemonic = Mnemonic.pload;
                    dasm.iclass = InstrClass.Linear;
                    dasm.ops.Add(op);
                    dasm.ops.Add(Constant.Byte((byte) (modes >> 10 & 7)));
                    return true;
                }
            }

            if ((modes & 0xe200) == 0x2000)	// PFLUSH
            {
                dasm.mnemonic = Mnemonic.pflushr;
                dasm.iclass = LinPriv;
                dasm.ops.Add(Constant.Byte((byte) (modes & 0x1f)));
                dasm.ops.Add(Constant.Byte((byte) (modes >> 5 & 0xf)));
                dasm.ops.Add(op);
                return true;
            }

            if (modes == 0xa000)	// PFLUSHR
            {
                dasm.mnemonic = Mnemonic.pflushr;
                dasm.iclass = LinPriv;
                dasm.ops.Add(op);
                return true;
            }

            if (modes == 0x2800)	// PVALID (FORMAT 1)
            {
                dasm.mnemonic = Mnemonic.pvalid;
                dasm.iclass = InstrClass.Linear;
                dasm.ops.Add(dasm.get_ctrl_reg("VAL", 0x2800));
                dasm.ops.Add(op);
                return true;
            }

            if ((modes & 0xfff8) == 0x2c00)	// PVALID (FORMAT 2)
            {
                dasm.mnemonic = Mnemonic.pvalid;
                dasm.iclass = InstrClass.Linear;
                dasm.ops.Add(get_addr_reg(modes & 0xFu));
                dasm.ops.Add(op);
                return true;
            }

            if ((modes & 0xe000) == 0x8000)	// PTEST
            {
                const uint valid_ea_mask = 0xE4;
                if ((1u << ((int) uInstr >> 3 & 7) & valid_ea_mask) == 0)
                    return false;
                if ((uInstr & 0x3F) == 0x3C)    // Immediate
                    return false;
                dasm.mnemonic = Mnemonic.ptest;
                dasm.iclass = LinPriv;
                dasm.ops.Add(Constant.Byte((byte) (modes & 0x1F)));
                dasm.ops.Add(op);
                return true;
            }

            switch (modes >> 13 & 0x7)
            {
            case 0:	// MC68030/040 form with FD bit
            case 2:	// MC68881 form, FD never set
                var mmureg = Registers.mmuregs[modes >> 10 & 7];
                dasm.iclass = LinPriv;
                dasm.mnemonic = (modes & 0x0100) != 0
                    ? dasm.mnemonic = Mnemonic.pmovefd
                    : dasm.mnemonic = Mnemonic.pmove;

                if ((modes & 0x0200) != 0)
                {
                    dasm.ops.Add(mmureg);
                    dasm.ops.Add(op);
                }
                else
                {
                    dasm.ops.Add(op);
                    dasm.ops.Add(mmureg);
                }
                return true;

            case 3:	// MC68030 to/from status reg
                dasm.iclass = LinPriv;
                dasm.mnemonic = Mnemonic.pmove;
                if ((modes & 0x0200) != 0)
                {
                    dasm.ops.Add(Registers.MMUSR);
                    dasm.ops.Add(op);
                }
                else
                {
                    dasm.ops.Add(op);
                    dasm.ops.Add(Registers.MMUSR);
                }
                return true;

            default:
                // dasm.g_dasm_str = string.Format("pmove [unknown form] {0}", op);
                return false;
            }
        }

        private static bool d68851_pbcc16(uint uInstr, M68kDisassembler dasm)
        {
            var temp_pc = dasm.rdr.Address;
            if (!dasm.rdr.TryReadBeInt16(out short sDisplacement))
                return false;
            dasm.mnemonic = g_mmucond[uInstr & 0xf];
            dasm.ops.Add(temp_pc + sDisplacement);
            return true;
        }

        private static bool d68851_pbcc32(uint uInstr, M68kDisassembler dasm)
        {
            var temp_pc = dasm.rdr.Address;
            if (!dasm.rdr.TryReadBeInt16(out short sDisplacement))
                return false;
            dasm.mnemonic = g_mmucond[uInstr & 0xf];
            dasm.ops.Add(temp_pc + sDisplacement);
            return false;
        }

        private static bool d68851_pdbcc(uint uInstr, M68kDisassembler dasm)
        {
            var temp_pc = dasm.rdr.Address;
            if (!dasm.rdr.TryReadBeUInt16(out ushort modes))
                return false;
            if (!dasm.rdr.TryReadBeInt16(out short displacement))
                return false;
            dasm.mnemonic = g_mmudcond[uInstr & 0xf];
            dasm.ops.Add(temp_pc + displacement);
            return false;
        }

        // PScc:  0000000000xxxxxx
        private static bool d68851_p001(uint uInstr, M68kDisassembler dasm)
        {
            //dasm.g_dasm_str = string.Format("MMU 001 group");
            return false;
        }

        #region Mutators

        /// <summary>
        /// Operand size as two bits.
        /// </summary>
        private static Mutator s(int bitOffset)
        {
            return (u, d) =>
            {
                switch (u >> bitOffset & 3)
                {
                case 0: d.dataWidth = PrimitiveType.Byte; return true;
                case 1: d.dataWidth = PrimitiveType.Word16; return true;
                case 2: d.dataWidth = PrimitiveType.Word32; return true;
                default: return false;
                }
            };
        }

        private static readonly Mutator s6 = s(6);
        private static readonly Mutator s9 = s(9);

        private static bool sb(uint u, M68kDisassembler d) { d.dataWidth = PrimitiveType.Byte; return true; }
        private static bool sw(uint u, M68kDisassembler d) { d.dataWidth = PrimitiveType.Word16; return true; }
        private static bool sl(uint u, M68kDisassembler d) { d.dataWidth = PrimitiveType.Word32; return true; }
        private static bool su(uint u, M68kDisassembler d) { d.dataWidth = PrimitiveType.UInt16; return true; }
        private static bool sr(uint u, M68kDisassembler d)
        {
            // If EA is register, 32 bits, else 8.
            var dt = (u & 0x30) == 0
                ? PrimitiveType.Word32
                : PrimitiveType.Byte;
            d.dataWidth = dt;
            return true;
        }

        /// <summary>
        /// Data register D0-D7 encoded in instruction
        /// </summary>
        private static Mutator D(int bitOffset)
        {
            return (u, d) =>
            {
                d.ops.Add(Registers.DataRegister((int) (u >> bitOffset) & 0x7));
                return true;
            };
        }

        private static readonly Mutator D0 = D(0);
        private static readonly Mutator D9 = D(9);
        private static bool ccr(uint uInstr, M68kDisassembler d) { d.ops.Add(Registers.ccr); return true; }
        private static bool SR(uint uInstr, M68kDisassembler d) { d.ops.Add(Registers.sr); return true; }
        private static bool USP(uint uInstr, M68kDisassembler d) { d.ops.Add(Registers.usp); return true; }

        /// <summary>
        /// Address register A0-A7 encoded in in instrution
        /// </summary>
        private static Mutator A(int bitOffset)
        {
            return (u, d) =>
            {
                d.ops.Add(Registers.AddressRegister((int) (u >> bitOffset) & 0x7));
                return true;
            };
        }

        private static readonly Mutator A0 = A(0);
        private static readonly Mutator A9 = A(9);

        /// <summary>
        /// Address register with 16-bit displacement
        /// </summary>
        private static Mutator Ad(int bitOffset)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadBeInt16(out short sDisplacement))
                    return false;
                var aReg = Registers.AddressRegister((int) (u >> bitOffset) & 0x7);
                d.ops.Add(new MemoryOperand(
                    PrimitiveType.Word16,
                    Registers.AddressRegister((int) u & 7),
                    Constant.Int16(sDisplacement)));
                return true;
            };
        }

        private static readonly Mutator Ad0 = Ad(0);

        /// <summary>
        /// Effective address (EA)
        /// </summary>
        private static Mutator E(int bitOffset)
        {
            return (u, d) =>
            {
                var opcode = u >> bitOffset;
                var op = d.DecodeEffectiveAddress(opcode, d.dataWidth!);
                if (op is null)
                    return false;
                d.ops.Add(op);
                return true;
            };
        }

        /// <summary>
        /// Effective address with 3-bit halves swapped
        /// </summary>
        private static Mutator e(int bitOffset)
        {
            return (u, d) =>
            {
                var o = u >> bitOffset;
                var opcode = o >> 3 & 0x07u | (o & 0x7u) << 3;
                var op = d.DecodeEffectiveAddress(opcode, d.dataWidth!);
                if (op is null)
                    return false;
                d.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator E0 = E(0);
        //private static readonly Mutator e0 = e(0);
        private static readonly Mutator e6 = e(6);

        /// <summary>
        /// Immediate operand from instruction stream
        /// </summary>
        private static bool Ib(uint uInstr, M68kDisassembler d)
        {
            d.rdr.Offset += 1;    // skip a byte so we get the appropriate lsb byte and align the word stream.
            if (!d.rdr.TryRead(PrimitiveType.Byte, out var c))
                return false;
            d.ops.Add(c);
            return true;
        }

        private static bool Iw(uint uInstr, M68kDisassembler d)
        {
            if (!d.rdr.TryRead(PrimitiveType.Word16, out var c))
                return false;
            d.ops.Add(c);
            return true;
        }

        private static bool Il(uint uInstr, M68kDisassembler d)
        {
            if (!d.rdr.TryRead(PrimitiveType.Word32, out var c))
                return false;
            d.ops.Add(c);
            return true;
        }

        private static bool Iv(uint uInstr, M68kDisassembler d)
        {
            if (d.dataWidth is null)
                return false;
            if (d.dataWidth.Size == 1)
                return Ib(uInstr, d);
            if (!d.rdr.TryRead(d.dataWidth, out var c))
                return false;
            d.ops.Add(c);
            return true;
        }

        /// <summary>
        /// Quick constant: the immediate is part of the opcode
        /// </summary>
        private static Mutator Q(int bitOffset, int mask, int zeroValue, PrimitiveType dt)
        {
            return (u, d) =>
            {
                int v = (int) u >> bitOffset & mask;
                if (v == 0)
                    v = zeroValue;
                d.ops.Add(Constant.Create(dt, v));
                return true;
            };
        }

        private static readonly Mutator q9 = Q(9, 0x07, 8, PrimitiveType.Byte);
        private static readonly Mutator Q0 = Q(0, 0xFF, 0, PrimitiveType.SByte);
        private static readonly Mutator Qinstr = Q(0, 0xFFFF, 0, PrimitiveType.Word16);

        /// <summary>
        /// Predecrement operator
        /// </summary>
        private static Mutator Pre(int bitOffset)
        {
            return (u, d) =>
            {
                d.ops.Add(new PredecrementMemoryOperand(
                    d.dataWidth!,
                    Registers.AddressRegister(u >> bitOffset & 0x7)));
                return true;
            };
        }

        private static readonly Mutator Pre0 = Pre(0);
        private static readonly Mutator Pre9 = Pre(9);


        // Postdecrement operator
        private static Mutator Post(int bitOffset)
        {
            return (u, d) =>
            {
                d.ops.Add(new PostIncrementMemoryOperand(
                    d.dataWidth!,
                    Registers.AddressRegister((int) (u >> bitOffset) & 0x7)));
                return true;
            };
        }

        private static readonly Mutator Post0 = Post(0);
        private static readonly Mutator Post9 = Post(9);


        // PC Relative jump 
        private static bool J(uint uInstr, M68kDisassembler dasm)
        {
            var addr = dasm.rdr.Address;
            int offset = (int) uInstr & 0xFF;
            if (offset == 0xFF)
            {
                if (!dasm.rdr.TryReadBeInt32(out offset))
                {
                    return false;
                }
            }
            else if (offset == 0x00)
            {
                if (!dasm.rdr.TryReadBeInt16(out short sOffset))
                {
                    return false;
                }
                offset = sOffset;
            }
            else
                offset = (sbyte) offset;
            dasm.ops.Add(addr + offset);
            return true;
        }


        // relative
        private static bool Rw(uint uInstr, M68kDisassembler dasm)
        {
            var addr = dasm.rdr.Address;
            if (!dasm.rdr.TryReadBeInt16(out short relative))
                return false;
            dasm.ops.Add(addr + relative);
            return true;
        }

        /// <summary>
        /// Register bitset.
        /// </summary>
        private static Mutator M(PrimitiveType size)
        {
            return (u, d) =>
            {
                // Predecrement addressing mode reverses bits.
                if (bf3L3.Read(u) != 0b100) 
                    d.ops.Add(RegisterSetOperand.CreateReversed(d.bitSet, size));
                else
                    d.ops.Add(new RegisterSetOperand(d.bitSet, size));
                return true;
            };
        }

        private static bool n(uint uInstr, M68kDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out dasm.bitSet))
            {
                return false;
            }
            return true;
        }

        private static readonly Mutator Mw = M(PrimitiveType.Word16);
        private static readonly Mutator Ml = M(PrimitiveType.Word32);


        /// <summary>
        /// Mnemonic determined from bitfield.
        /// </summary>
        private static Mutator MnemonicField(int bitpos, int length, Mnemonic[] mnemonics)
        {
            var field = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var code = field.Read(u);
                d.mnemonic = mnemonics[code];
                return true;
            };
        }
        private static readonly Mutator MnBcc = MnemonicField(8, 4, g_bcc);

        #endregion

        /* ======================================================================== */
        /* ======================= INSTRUCTION TABLE BUILDER ====================== */
        /* ======================================================================== */

        /* EA Masks:
        800 = data register direct
        400 = address register direct
        200 = address register indirect
        100 = ARI postincrement
         80 = ARI pre-decrement
         40 = ARI displacement
         20 = ARI index
         10 = absolute short
          8 = absolute long
          4 = immediate / sr
          2 = pc displacement
          1 = pc idx
        */

        private static readonly InstrDecoder[] g_opcode_info;

        /// <summary>
        /// Generates the table of opcode decoders. Should only be called once
        /// per execution, as the table is expensive to build. Fortunately,
        /// Decoders have no mutable state, so the table is reused for all
        /// disassembler instances.
        /// </summary>
        private static InstrDecoder[] GenTable()
        {
            return new InstrDecoder[]
            {
//  opcode handler             mask    match   ea mask 
	Instr(Qinstr, 0xf000, 0xa000, 0x000, Mnemonic.illegal, InstrClass.Invalid), // d68000_1010
    Instr(0xf000, 0xf000, 0x000, Mnemonic.illegal, InstrClass.Invalid),  // d68000_1111
	Instr(D0,D9, 0xf1f8, 0xc100, 0x000, Mnemonic.abcd),             // d68000_abcd_rr
	Instr(Pre0,Pre9, 0xf1f8, 0xc108, 0x000, Mnemonic.abcd),         // d68000_abcd_mm
	Instr(s6,E0,D9, 0xf1c0, 0xd000, 0xbff, Mnemonic.add),           // d68000_add_er_8
	Instr(sw,E0,D9, 0xf1c0, 0xd040, 0xfff, Mnemonic.add),           // d68000_add_er_16
	Instr(sl,E0,D9, 0xf1c0, 0xd080, 0xfff, Mnemonic.add),           // d68000_add_er_32
	Instr(s6,D9,E0, 0xf1c0, 0xd100, 0x3f8, Mnemonic.add),           // d68000_add_re_8
	Instr(sw,D9,E0, 0xf1c0, 0xd140, 0x3f8, Mnemonic.add),           // d68000_add_re_16
	Instr(sl,D9,E0, 0xf1c0, 0xd180, 0x3f8, Mnemonic.add),           // d68000_add_re_32
	Instr(sw,E0,A9, 0xf1c0, 0xd0c0, 0xfff, Mnemonic.adda),          // d68000_adda_16
	Instr(sl,E0,A9, 0xf1c0, 0xd1c0, 0xfff, Mnemonic.adda),          // d68000_adda_32
	Instr(sb,Ib,E0, 0xffc0, 0x0600, 0xbf8, Mnemonic.addi),          // d68000_addi_8
	Instr(sw,Iw,E0, 0xffc0, 0x0640, 0xbf8, Mnemonic.addi),          // d68000_addi_16
	Instr(sl,Il,E0, 0xffc0, 0x0680, 0xbf8, Mnemonic.addi),          // d68000_addi_32
	Instr(s6,q9,E0, 0xf1c0, 0x5000, 0xbf8, Mnemonic.addq),          // d68000_addq_8 
	Instr(s6,q9,E0, 0xf1c0, 0x5040, 0xff8, Mnemonic.addq),          // d68000_addq_16
	Instr(s6,q9,E0, 0xf1c0, 0x5080, 0xff8, Mnemonic.addq),          // d68000_addq_32
	Instr(sb,D0,D9, 0xf1f8, 0xd100, 0x000, Mnemonic.addx),          // d68000_addx_rr_8    
	Instr(sw,D0,D9, 0xf1f8, 0xd140, 0x000, Mnemonic.addx),          // d68000_addx_rr_16   
	Instr(sl,D0,D9, 0xf1f8, 0xd180, 0x000, Mnemonic.addx),          // d68000_addx_rr_32   
	Instr(sb,Pre0,Pre9, 0xf1f8, 0xd108, 0x000, Mnemonic.addx),      // d68000_addx_mm_8    
	Instr(sw,Pre0,Pre9, 0xf1f8, 0xd148, 0x000, Mnemonic.addx),      // d68000_addx_mm_16   
	Instr(sl,Pre0,Pre9, 0xf1f8, 0xd188, 0x000, Mnemonic.addx),      // d68000_addx_mm_32   
	Instr(sb,E0,D9, 0xf1c0, 0xc000, 0xbff, Mnemonic.and),           // d68000_and_er_8
	Instr(sw,E0,D9, 0xf1c0, 0xc040, 0xbff, Mnemonic.and),           // d68000_and_er_16
	Instr(sl,E0,D9, 0xf1c0, 0xc080, 0xbff, Mnemonic.and),           // d68000_and_er_32
	Instr(sb,D9,E0, 0xf1c0, 0xc100, 0x3f8, Mnemonic.and),           // d68000_and_re_8
	Instr(sw,D9,E0, 0xf1c0, 0xc140, 0x3f8, Mnemonic.and),           // d68000_and_re_16
	Instr(sl,D9,E0, 0xf1c0, 0xc180, 0x3f8, Mnemonic.and),           // d68000_and_re_32
	Instr(Iw,ccr,   0xffff, 0x023c, 0x000, Mnemonic.andi),          // d68000_andi_to_ccr
	Instr(Iw,SR,    0xffff, 0x027c, 0x000, Mnemonic.andi, LinPriv), // d68000_andi_to_sr
	Instr(sb,Ib,E0, 0xffc0, 0x0200, 0xbf8, Mnemonic.andi),          // d68000_andi_8
	Instr(sw,Iw,E0, 0xffc0, 0x0240, 0xbf8, Mnemonic.andi),          // d68000_andi_16
	Instr(sl,Il,E0, 0xffc0, 0x0280, 0xbf8, Mnemonic.andi),          // d68000_andi_32
	Instr(sb,q9,D0, 0xf1f8, 0xe000, 0x000, Mnemonic.asr),           // d68000_asr_s_8
	Instr(sw,q9,D0, 0xf1f8, 0xe040, 0x000, Mnemonic.asr),           // d68000_asr_s_16
	Instr(sl,q9,D0, 0xf1f8, 0xe080, 0x000, Mnemonic.asr),           // d68000_asr_s_32
	Instr(sb,D9,D0, 0xf1f8, 0xe020, 0x000, Mnemonic.asr),           // d68000_asr_r_8
    Instr(sw,D9,D0, 0xf1f8, 0xe060, 0x000, Mnemonic.asr),           // d68000_asr_r_16
	Instr(sl,D9,D0, 0xf1f8, 0xe0a0, 0x000, Mnemonic.asr),           // d68000_asr_r_32
	Instr(sw,E0,    0xffc0, 0xe0c0, 0x3f8, Mnemonic.asr),           // d68000_asr_ea
	Instr(sb,q9,D0, 0xf1f8, 0xe100, 0x000, Mnemonic.asl),           // d68000_asl_s_8 
	Instr(sw,q9,D0, 0xf1f8, 0xe140, 0x000, Mnemonic.asl),           // d68000_asl_s_16
	Instr(sl,q9,D0, 0xf1f8, 0xe180, 0x000, Mnemonic.asl),           // d68000_asl_s_32
	Instr(sb,D9,D0, 0xf1f8, 0xe120, 0x000, Mnemonic.asl),           // d68000_asl_r_8 
	Instr(sw,D9,D0, 0xf1f8, 0xe160, 0x000, Mnemonic.asl),           // d68000_asl_r_16
	Instr(sl,D9,D0, 0xf1f8, 0xe1a0, 0x000, Mnemonic.asl),           // d68000_asl_r_32
	Instr(sw,E0,    0xffc0, 0xe1c0, 0x3f8, Mnemonic.asl),           // d68000_asl_ea  
	Instr(MnBcc,J,  0xf000, 0x6000, 0x000, 0, iclass:InstrClass.ConditionalTransfer),  // d68000_bcc_8
    Instr(MnBcc,J,  0xf0ff, 0x6000, 0x000, 0, iclass:InstrClass.ConditionalTransfer),  // d68000_bcc_16
    Instr(MnBcc,J,  0xf0ff, 0x60ff, 0x000, 0, iclass:InstrClass.ConditionalTransfer),  // d68020_bcc_32
    Instr(sr,D9,E0, 0xf1c0, 0x0140, 0xbf8, Mnemonic.bchg),          // d68000_bchg_r 
	Instr(sr,Ib,E0, 0xffc0, 0x0840, 0xbf8, Mnemonic.bchg),          // d68000_bchg_s 
	Instr(sr,D9,E0, 0xf1c0, 0x0180, 0xbf8, Mnemonic.bclr),          // d68000_bclr_r 
	Instr(sr,Ib,E0, 0xffc0, 0x0880, 0xbf8, Mnemonic.bclr),          // d68000_bclr_s 
	Instr(d68020_bfchg        , 0xffc0, 0xeac0, 0xa78),
    Instr(d68020_bfclr        , 0xffc0, 0xecc0, 0xa78),
    Instr(d68020_bfexts       , 0xffc0, 0xebc0, 0xa7b),
    Instr(d68020_bfextu       , 0xffc0, 0xe9c0, 0xa7b),
    Instr(d68020_bfffo        , 0xffc0, 0xedc0, 0xa7b),
    Instr(d68020_bfins        , 0xffc0, 0xefc0, 0xa78),
    Instr(d68020_bfset        , 0xffc0, 0xeec0, 0xa78),
    Instr(d68020_bftst        , 0xffc0, 0xe8c0, 0xa7b),
    Instr(d68010_bkpt         , 0xfff8, 0x4848, 0x000),
    Instr(J, 0xff00, 0x6000, 0x000, Mnemonic.bra, InstrClass.Transfer),              // d68000_bra_8
	Instr(J, 0xffff, 0x6000, 0x000, Mnemonic.bra, InstrClass.Transfer),              // d68000_bra_16
	Instr(J, 0xffff, 0x60ff, 0x000, Mnemonic.bra, InstrClass.Transfer),              // d68020_bra_32
	Instr(D9,E0, 0xf1c0, 0x01c0, 0xbf8, Mnemonic.bset),         // d68000_bset_r
	Instr(Iw,E0, 0xffc0, 0x08c0, 0xbf8, Mnemonic.bset),         // d68000_bset_s
	Instr(J, 0xff00, 0x6100, 0x000, Mnemonic.bsr, InstrClass.Transfer|InstrClass.Call),   // d68000_bsr_8 
	Instr(J, 0xffff, 0x6100, 0x000, Mnemonic.bsr, InstrClass.Transfer|InstrClass.Call),   // d68000_bsr_16
	Instr(J, 0xffff, 0x61ff, 0x000, Mnemonic.bsr, InstrClass.Transfer|InstrClass.Call),   // d68020_bsr_32
	Instr(sl,D9,E0, 0xf1c0, 0x0100, 0xbff, Mnemonic.btst),      // d68000_btst_r 
	Instr(sw,Iw,E0, 0xffc0, 0x0800, 0xbfb, Mnemonic.btst),      // d68000_btst_s
	Instr(d68020_callm        , 0xffc0, 0x06c0, 0x27b, iclass:InstrClass.Transfer|InstrClass.Call),
    Instr(d68020_cas_8        , 0xffc0, 0x0ac0, 0x3f8),
    Instr(d68020_cas_16       , 0xffc0, 0x0cc0, 0x3f8),
    Instr(d68020_cas_32       , 0xffc0, 0x0ec0, 0x3f8),
    Instr(d68020_cas2_16      , 0xffff, 0x0cfc, 0x000),
    Instr(d68020_cas2_32      , 0xffff, 0x0efc, 0x000),
    Instr(d68000_chk_16       , 0xf1c0, 0x4180, 0xbff),
    Instr(d68020_chk_32       , 0xf1c0, 0x4100, 0xbff),
    Instr(d68020_chk2_cmp2_8  , 0xffc0, 0x00c0, 0x27b),
    Instr(d68020_chk2_cmp2_16 , 0xffc0, 0x02c0, 0x27b),
    Instr(d68020_chk2_cmp2_32 , 0xffc0, 0x04c0, 0x27b),
    Instr(d68040_cinv         , 0xff20, 0xf400, 0x000),
    Instr(sb,E0, 0xffc0, 0x4200, 0xbf8, Mnemonic.clr),      // d68000_clr_8
	Instr(sw,E0, 0xffc0, 0x4240, 0xbf8, Mnemonic.clr),      // d68000_clr_16
	Instr(sl,E0, 0xffc0, 0x4280, 0xbf8, Mnemonic.clr),      // d68000_clr_32
	Instr(sb,E0,D9, 0xf1c0, 0xb000, 0xbff, Mnemonic.cmp),   // d68000_cmp_8
	Instr(sw,E0,D9, 0xf1c0, 0xb040, 0xfff, Mnemonic.cmp),   // d68000_cmp_16
	Instr(sl,E0,D9, 0xf1c0, 0xb080, 0xfff, Mnemonic.cmp),   // d68000_cmp_32
	Instr(sw,E0,A9, 0xf1c0, 0xb0c0, 0xfff, Mnemonic.cmpa),  // d68000_cmpa_16
	Instr(sl,E0,A9, 0xf1c0, 0xb1c0, 0xfff, Mnemonic.cmpa),  // d68000_cmpa_32
	Instr(sb,Ib,E0, 0xffc0, 0x0c00, 0xbf8, Mnemonic.cmpi),  // d68000_cmpi_8
	Instr(sb,Ib,E0, 0xffff, 0x0c3a, 0x000, Mnemonic.cmpi),              // d68020_cmpi_pcdi_8 
    Instr(sb,Ib,E0, 0xffff, 0x0c3b, 0x000, Mnemonic.cmpi),              // d68020_cmpi_pcix_8 
    Instr(sw,Iw,E0, 0xffc0, 0x0c40, 0xbf8, Mnemonic.cmpi),              // d68000_cmpi_16
	Instr(sw,Iw,E0, 0xffff, 0x0c7a, 0x000, Mnemonic.cmpi),              // d68020_cmpi_pcdi_16
    Instr(sw,Iw,E0, 0xffff, 0x0c7b, 0x000, Mnemonic.cmpi),              // d68020_cmpi_pcix_16
    Instr(sl,Il,E0, 0xffc0, 0x0c80, 0xbf8, Mnemonic.cmpi),              // d68000_cmpi_32
	Instr(sl,Il,E0, 0xffff, 0x0cba, 0x000, Mnemonic.cmpi),              // d68020_cmpi_pcdi_32
    Instr(sl,Il,E0, 0xffff, 0x0cbb, 0x000, Mnemonic.cmpi),              // d68020_cmpi_pcix_32 
    Instr(sb,Post0,Post9, 0xf1f8, 0xb108, 0x000, Mnemonic.cmpm),        // d68000_cmpm_8
	Instr(sw,Post0,Post9 , 0xf1f8, 0xb148, 0x000, Mnemonic.cmpm),       // d68000_cmpm_16     
	Instr(sl,Post0,Post9 , 0xf1f8, 0xb188, 0x000, Mnemonic.cmpm),       // d68000_cmpm_32     
	Instr(d68020_cpbcc_16     , 0xf1c0, 0xf080, 0x000),
    Instr(d68020_cpbcc_32     , 0xf1c0, 0xf0c0, 0x000),
    Instr(d68020_cpdbcc       , 0xf1f8, 0xf048, 0x000),
    Instr(d68020_cpgen        , 0xf1c0, 0xf000, 0x000),
    Instr(d68020_cprestore    , 0xf1c0, 0xf140, 0x37f),
    Instr(d68020_cpsave       , 0xf1c0, 0xf100, 0x2f8),
    Instr(d68020_cpscc        , 0xf1c0, 0xf040, 0xbf8),
    Instr(d68020_cptrapcc_0   , 0xf1ff, 0xf07c, 0x000),
    Instr(d68020_cptrapcc_16  , 0xf1ff, 0xf07a, 0x000),
    Instr(d68020_cptrapcc_32  , 0xf1ff, 0xf07b, 0x000),
    Instr(d68040_cpush        , 0xff20, 0xf420, 0x000),
    Instr(d68000_dbcc         , 0xf0f8, 0x50c8, 0x000),
    Instr(D0,Rw, 0xfff8, 0x51c8, 0x000, Mnemonic.dbra),             // d68000_dbra
	Instr(sw,E0,D9, 0xf1c0, 0x81c0, 0xbff, Mnemonic.divs),          // d68000_divs
	Instr(su,E0,D9, 0xf1c0, 0x80c0, 0xbff, Mnemonic.divu),          // d68000_divu
	Instr(d68020_divl, 0xffc0, 0x4c40, 0xbff),
    Instr(sb,D9,E0, 0xf1c0, 0xb100, 0xbf8, Mnemonic.eor),           // d68000_eor_8
	Instr(sw,D9,E0, 0xf1c0, 0xb140, 0xbf8, Mnemonic.eor),           // d68000_eor_16
	Instr(sl,D9,E0, 0xf1c0, 0xb180, 0xbf8, Mnemonic.eor),           // d68000_eor_32
	Instr(sb,Ib,ccr, 0xffff, 0x0a3c, 0x000, Mnemonic.eori),         // d68000_eori_to_ccr
    Instr(sw,Iw,SR, 0xffff, 0x0a7c, 0x000, Mnemonic.eori, LinPriv), // d68000_eori_to_sr
    Instr(sb,Ib,E0, 0xffc0, 0x0a00, 0xbf8, Mnemonic.eori),          // d68000_eori_8
	Instr(sw,Iw,E0, 0xffc0, 0x0a40, 0xbf8, Mnemonic.eori),          // d68000_eori_16
	Instr(sl,Il,E0, 0xffc0, 0x0a80, 0xbf8, Mnemonic.eori),          // d68000_eori_32
	Instr(D9,D0, 0xf1f8, 0xc140, 0x000, Mnemonic.exg),              // d68000_exg_dd 
	Instr(A9,A0, 0xf1f8, 0xc148, 0x000, Mnemonic.exg),              // d68000_exg_aa
	Instr(D9,A0, 0xf1f8, 0xc188, 0x000, Mnemonic.exg),              // d68000_exg_da
	Instr(sl,D0, 0xfff8, 0x49c0, 0x000, Mnemonic.extb),             // d68020_extb_32
	Instr(sw,D0, 0xfff8, 0x4880, 0x000, Mnemonic.ext),              // d68000_ext_16
	Instr(sl,D0, 0xfff8, 0x48c0, 0x000, Mnemonic.ext),              // d68000_ext_32
    Instr(d68040_fpu         , 0xffc0, 0xf200, 0x000),
    Instr(d68000_illegal      , 0xffff, 0x4afc, 0x000, iclass:InstrClass.Invalid),
    Instr(sl,E0, 0xffc0, 0x4ec0, 0x27b, Mnemonic.jmp, InstrClass.Transfer),   // d68000_jmp
	Instr(sl,E0, 0xffc0, 0x4e80, 0x27b, Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call),   // d68000_jsr
	Instr(E0,A9, 0xf1c0, 0x41c0, 0x27b, Mnemonic.lea),              // d68000_lea
	Instr(A0,Iw, 0xfff8, 0x4e50, 0x000, Mnemonic.link),             // d68000_link_16 
	Instr(A0,Il, 0xfff8, 0x4808, 0x000, Mnemonic.link),             // d68020_link_32
	Instr(s6,q9,D0, 0xf1f8, 0xe008, 0x000, Mnemonic.lsr),           // d68000_lsr_s_8
	Instr(s6,q9,D0, 0xf1f8, 0xe048, 0x000, Mnemonic.lsr),           // d68000_lsr_s_16
	Instr(s6,q9,D0, 0xf1f8, 0xe088, 0x000, Mnemonic.lsr),           // d68000_lsr_s_32
	Instr(sb,D9,D0, 0xf1f8, 0xe028, 0x000, Mnemonic.lsr),           // d68000_lsr_r_8
	Instr(sw,D9,D0, 0xf1f8, 0xe068, 0x000, Mnemonic.lsr),           // d68000_lsr_r_16
	Instr(sl,D9,D0, 0xf1f8, 0xe0a8, 0x000, Mnemonic.lsr),           // d68000_lsr_r_32
	Instr(sw,E0,    0xffc0, 0xe2c0, 0x3f8, Mnemonic.lsr),           // d68000_lsr_ea
	Instr(s6,q9,D0, 0xf1f8, 0xe108, 0x000, Mnemonic.lsl),           // d68000_lsl_s_8
	Instr(s6,q9,D0, 0xf1f8, 0xe148, 0x000, Mnemonic.lsl),           // d68000_lsl_s_16
	Instr(s6,q9,D0, 0xf1f8, 0xe188, 0x000, Mnemonic.lsl),           // d68000_lsl_s_32
	Instr(sb,D9,D0, 0xf1f8, 0xe128, 0x000, Mnemonic.lsl),           // d68000_lsl_r_8 
	Instr(sw,D9,D0, 0xf1f8, 0xe168, 0x000, Mnemonic.lsl),           // d68000_lsl_r_16
	Instr(sl,D9,D0, 0xf1f8, 0xe1a8, 0x000, Mnemonic.lsl),           // d68000_lsl_r_32
	Instr(sw,E0,    0xffc0, 0xe3c0, 0x3f8, Mnemonic.lsl),           // d68000_lsl_ea
	Instr(sb,E0,e6, 0xf000, 0x1000, 0xbff, Mnemonic.move),          // d68000_move_8
	Instr(sw,E0,e6, 0xf000, 0x3000, 0xfff, Mnemonic.move),          // d68000_move_16
	Instr(sl,E0,e6, 0xf000, 0x2000, 0xfff, Mnemonic.move),          // d68000_move_32
	Instr(sw,E0,A9, 0xf1c0, 0x3040, 0xfff, Mnemonic.movea),         // d68000_movea_16
	Instr(sl,E0,A9, 0xf1c0, 0x2040, 0xfff, Mnemonic.movea),         // d68000_movea_32
	Instr(sb,E0,ccr,    0xffc0, 0x44c0, 0xbff, Mnemonic.move),      // d68000_move_to_ccr
	Instr(sb,ccr,E0,    0xffc0, 0x42c0, 0xbf8, Mnemonic.move),      // d68010_move_fr_ccr
	Instr(sw,E0,SR, 0xffc0, 0x46c0, 0xbff, Mnemonic.move, LinPriv), // d68000_move_to_sr
    Instr(sw,SR,E0, 0xffc0, 0x40c0, 0xbf8, Mnemonic.move, LinPriv), // d68000_move_fr_sr
    Instr(A0,USP,   0xfff8, 0x4e60, 0x000, Mnemonic.move),          // d68000_move_to_usp
    Instr(USP,A0,   0xfff8, 0x4e68, 0x000, Mnemonic.move),          // d68000_move_fr_usp
    Instr(d68010_movec        , 0xfffe, 0x4e7a, 0x000),
    Instr(sw,n,Mw,E0, 0xfff8, 0x48a0, 0x000, Mnemonic.movem),         // d68000_movem_pd_16
	Instr(sl,n,Ml,E0, 0xfff8, 0x48e0, 0x000, Mnemonic.movem),         // d68000_movem_pd_32
	Instr(sw,n,Mw,E0, 0xffc0, 0x4880, 0x2f8, Mnemonic.movem),         // d68000_movem_re_16
	Instr(sl,n,Ml,E0, 0xffc0, 0x48c0, 0x2f8, Mnemonic.movem),         // d68000_movem_re_32
	Instr(sw,n,E0,Mw, 0xffc0, 0x4c80, 0x37b, Mnemonic.movem),       // d68000_movem_er_16
	Instr(sl,n,E0,Ml, 0xffc0, 0x4cc0, 0x37b, Mnemonic.movem),       // d68000_movem_er_32
	Instr(sw,Ad0,D9, 0xf1f8, 0x0108, 0x000, Mnemonic.movep),        // d68000_movep_er_16
    Instr(sl,Ad0,D9, 0xf1f8, 0x0148, 0x000, Mnemonic.movep),        // 68000_movep_er_32
    Instr(sw,D9,Ad0, 0xf1f8, 0x0188, 0x000, Mnemonic.movep),        // d68000_movep_re_16),
	Instr(sl,D9,Ad0, 0xf1f8, 0x01c8, 0x000, Mnemonic.movep),        // d68000_movep_re_32
	Instr(d68010_moves_8      , 0xffc0, 0x0e00, 0x3f8, iclass:LinPriv),
    Instr(d68010_moves_16     , 0xffc0, 0x0e40, 0x3f8, iclass:LinPriv),
    Instr(d68010_moves_32     , 0xffc0, 0x0e80, 0x3f8, iclass:LinPriv),
    Instr(Q0,D9, 0xf100, 0x7000, 0x000, Mnemonic.moveq),        // d68000_moveq
	Instr(d68040_move16_pi_pi , 0xfff8, 0xf620, 0x000),
    Instr(d68040_move16_pi_al , 0xfff8, 0xf600, 0x000),
    Instr(d68040_move16_al_pi , 0xfff8, 0xf608, 0x000),
    Instr(d68040_move16_ai_al , 0xfff8, 0xf610, 0x000),
    Instr(d68040_move16_al_ai , 0xfff8, 0xf618, 0x000),
    Instr(sw,E0,D9, 0xf1c0, 0xc1c0, 0xbff, Mnemonic.muls),      // d68000_muls
	Instr(sw,E0,D9, 0xf1c0, 0xc0c0, 0xbff, Mnemonic.mulu),      // d68000_mulu
	Instr(d68020_mull         , 0xffc0, 0x4c00, 0xbff),
    Instr(sb,E0, 0xffc0, 0x4800, 0xbf8, Mnemonic.nbcd),         // d68000_nbcd
    Instr(sb,E0, 0xffc0, 0x4400, 0xbf8, Mnemonic.neg),          // d68000_neg_8
	Instr(sw,E0, 0xffc0, 0x4440, 0xbf8, Mnemonic.neg),          // d68000_neg_16
	Instr(sl,E0, 0xffc0, 0x4480, 0xbf8, Mnemonic.neg),          // d68000_neg_32
	Instr(sb,E0, 0xffc0, 0x4000, 0xbf8, Mnemonic.negx),         // d68000_negx_8
	Instr(sw,E0, 0xffc0, 0x4040, 0xbf8, Mnemonic.negx),         // d68000_negx_16
	Instr(sl,E0, 0xffc0, 0x4080, 0xbf8, Mnemonic.negx),         // d68000_negx_32,
	Instr(0xffff, 0x4e71, 0x000, Mnemonic.nop),                 // d68000_nop
	Instr(sb,E0, 0xffc0, 0x4600, 0xbf8, Mnemonic.not),          // d68000_not_8
	Instr(sw,E0, 0xffc0, 0x4640, 0xbf8, Mnemonic.not),          // d68000_not_16
	Instr(sl,E0, 0xffc0, 0x4680, 0xbf8, Mnemonic.not),          // d68000_not_32       
	Instr(sb,E0,D9, 0xf1c0, 0x8000, 0xbff, Mnemonic.or),        // d68000_or_er_8      
	Instr(sw,E0,D9, 0xf1c0, 0x8040, 0xbff, Mnemonic.or),        // d68000_or_er_16     
	Instr(sl,E0,D9, 0xf1c0, 0x8080, 0xbff, Mnemonic.or),        // d68000_or_er_32   
	Instr(sb,D9,E0, 0xf1c0, 0x8100, 0x3f8, Mnemonic.or),        // d68000_or_re_8     
	Instr(sw,D9,E0, 0xf1c0, 0x8140, 0x3f8, Mnemonic.or),        // d68000_or_re_16    
	Instr(sl,D9,E0, 0xf1c0, 0x8180, 0x3f8, Mnemonic.or),        // d68000_or_re_32
	Instr(sb,Ib,ccr, 0xffff, 0x003c, 0x000, Mnemonic.ori),      // d68000_ori_to_ccr   
	Instr(sw,Iw,SR, 0xffff, 0x007c, 0x000, Mnemonic.ori, LinPriv),       // d68000_ori_to_sr    
	Instr(s6,Iv,E0, 0xffc0, 0x0000, 0xbf8, Mnemonic.ori),       // d68000_ori_8        
	Instr(s6,Iv,E0, 0xffc0, 0x0040, 0xbf8, Mnemonic.ori),       // d68000_ori_16        
	Instr(s6,Iv,E0, 0xffc0, 0x0080, 0xbf8, Mnemonic.ori),       // d68000_ori_32       
	Instr(d68020_pack_rr      , 0xf1f8, 0x8140, 0x000),
    Instr(d68020_pack_mm      , 0xf1f8, 0x8148, 0x000),
    Instr(E0, 0xffc0, 0x4840, 0x27b, Mnemonic.pea),             // d68000_pea
	Instr(d68040_pflush       , 0xffe0, 0xf500, 0x000),
    Instr(0xffff, 0x4e70, 0x000, Mnemonic.reset, LinPriv),      // d68000_reset
	Instr(sb,q9,D0, 0xf1f8, 0xe018, 0x000, Mnemonic.ror),       // d68000_ror_s_8
	Instr(sw,q9,D0, 0xf1f8, 0xe058, 0x000, Mnemonic.ror),       // d68000_ror_s_16
	Instr(sl,q9,D0, 0xf1f8, 0xe098, 0x000, Mnemonic.ror),       // d68000_ror_s_32
	Instr(sb,D9,D0, 0xf1f8, 0xe038, 0x000, Mnemonic.ror),       // d68000_ror_r_8 
	Instr(sw,D9,D0, 0xf1f8, 0xe078, 0x000, Mnemonic.ror),       // d68000_ror_r_16
	Instr(sl,D9,D0, 0xf1f8, 0xe0b8, 0x000, Mnemonic.ror),       // d68000_ror_r_32
	Instr(sl,E0, 0xffc0, 0xe6c0, 0x3f8, Mnemonic.ror),          // d68000_ror_ea
	Instr(sb,q9,D0, 0xf1f8, 0xe118, 0x000, Mnemonic.rol),       // d68000_rol_s_8
	Instr(sw,q9,D0, 0xf1f8, 0xe158, 0x000, Mnemonic.rol),       // d68000_rol_s_16
	Instr(sl,q9,D0, 0xf1f8, 0xe198, 0x000, Mnemonic.rol),       // d68000_rol_s_32
	Instr(sb,D9,D0, 0xf1f8, 0xe138, 0x000, Mnemonic.rol),       // d68000_rol_r_8
	Instr(sw,D9,D0, 0xf1f8, 0xe178, 0x000, Mnemonic.rol),       // d68000_rol_r_16
	Instr(sl,D9,D0, 0xf1f8, 0xe1b8, 0x000, Mnemonic.rol),       // d68000_rol_r_32
	Instr(sl,E0,    0xffc0, 0xe7c0, 0x3f8, Mnemonic.rol),       // d68000_rol_ea
	Instr(sb,q9,D0, 0xf1f8, 0xe010, 0x000, Mnemonic.roxr),      // d68000_roxr_s_8 
	Instr(sw,q9,D0, 0xf1f8, 0xe050, 0x000, Mnemonic.roxr),      // d68000_roxr_s_16
	Instr(sl,q9,D0, 0xf1f8, 0xe090, 0x000, Mnemonic.roxr),      // d68000_roxr_s_32
	Instr(sb,D9,D0, 0xf1f8, 0xe030, 0x000, Mnemonic.roxr),      // d68000_roxr_r_8 
	Instr(sw,D9,D0, 0xf1f8, 0xe070, 0x000, Mnemonic.roxr),      // d68000_roxr_r_16
	Instr(sl,D9,D0, 0xf1f8, 0xe0b0, 0x000, Mnemonic.roxr),      // d68000_roxr_r_32
	Instr(sl,E0, 0xffc0, 0xe4c0, 0x3f8, Mnemonic.roxr),         // d68000_roxr_ea  
	Instr(sb,q9,D0, 0xf1f8, 0xe110, 0x000, Mnemonic.roxl),      // d68000_roxl_s_8 
	Instr(sw,q9,D0, 0xf1f8, 0xe150, 0x000, Mnemonic.roxl),      // d68000_roxl_s_16
	Instr(sl,q9,D0, 0xf1f8, 0xe190, 0x000, Mnemonic.roxl),      // d68000_roxl_s_32
	Instr(sb,D9,D0, 0xf1f8, 0xe130, 0x000, Mnemonic.roxl),      // d68000_roxl_r_8 
	Instr(sw,D9,D0, 0xf1f8, 0xe170, 0x000, Mnemonic.roxl),      // d68000_roxl_r_16
	Instr(sl,D9,D0, 0xf1f8, 0xe1b0, 0x000, Mnemonic.roxl),      // d68000_roxl_r_32
	Instr(sl,E0, 0xffc0, 0xe5c0, 0x3f8, Mnemonic.roxl),         // d68000_roxl_ea 
	Instr(Iw, 0xffff, 0x4e74, 0x000, Mnemonic.rtd, InstrClass.Transfer|InstrClass.Return),      // d68010_rtd
	Instr(0xffff, 0x4e73, 0x000, Mnemonic.rte, InstrClass.Transfer|InstrClass.Privileged),      // d68000_rte
	Instr(d68020_rtm, 0xfff0, 0x06c0, 0x000, iclass:InstrClass.Transfer|InstrClass.Return),
    Instr(0xffff, 0x4e77, 0x000, Mnemonic.rtr, InstrClass.Transfer|InstrClass.Return),          // d68000_rtr
	Instr(0xffff, 0x4e75, 0x000, Mnemonic.rts, InstrClass.Transfer|InstrClass.Return),          // d68000_rts
	Instr(D0,D9, 0xf1f8, 0x8100, 0x000, Mnemonic.sbcd),         // d68000_sbcd_rr
	Instr(Pre0,Pre9, 0xf1f8, 0x8108, 0x000, Mnemonic.sbcd),     // d68000_sbcd_mm
	Instr(d68000_scc          , 0xf0c0, 0x50c0, 0xbf8),
    Instr(d68000_stop         , 0xffff, 0x4e72, 0x000),
    Instr(sb,E0,D9, 0xf1c0, 0x9000, 0xbff, Mnemonic.sub),       // d68000_sub_er_8
	Instr(sw,E0,D9, 0xf1c0, 0x9040, 0xfff, Mnemonic.sub),       // d68000_sub_er_16
	Instr(sl,E0,D9, 0xf1c0, 0x9080, 0xfff, Mnemonic.sub),       // d68000_sub_er_32
	Instr(sb,D9,E0, 0xf1c0, 0x9100, 0x3f8, Mnemonic.sub),       // d68000_sub_re_8
	Instr(sw,D9,E0, 0xf1c0, 0x9140, 0x3f8, Mnemonic.sub),       // d68000_sub_re_16
	Instr(sl,D9,E0, 0xf1c0, 0x9180, 0x3f8, Mnemonic.sub),       // d68000_sub_re_32
	Instr(sw,E0,A9, 0xf1c0, 0x90c0, 0xfff, Mnemonic.suba),      // d68000_suba_16
	Instr(sl,E0,A9, 0xf1c0, 0x91c0, 0xfff, Mnemonic.suba),      // d68000_suba_32
	Instr(sb,Ib,E0,   0xffc0, 0x0400, 0xbf8, Mnemonic.subi),    // d68000_subi_8
	Instr(sw,Iw,E0,   0xffc0, 0x0440, 0xbf8, Mnemonic.subi),    // d68000_subi_16
	Instr(sl,Il,E0,   0xffc0, 0x0480, 0xbf8, Mnemonic.subi),    // d68000_subi_32
	Instr(sb,q9,E0,   0xf1c0, 0x5100, 0xbf8, Mnemonic.subq),    // d68000_subq_8
	Instr(sw,q9,E0,   0xf1c0, 0x5140, 0xff8, Mnemonic.subq),    // d68000_subq_16
	Instr(sl,q9,E0,   0xf1c0, 0x5180, 0xff8, Mnemonic.subq),    // d68000_subq_32
	Instr(sb,D0,D9,   0xf1f8, 0x9100, 0x000, Mnemonic.subx),    // d68000_subx_rr_8
	Instr(sw,D0,D9,   0xf1f8, 0x9140, 0x000, Mnemonic.subx),    // d68000_subx_rr_16
	Instr(sl,D0,D9,   0xf1f8, 0x9180, 0x000, Mnemonic.subx),    // d68000_subx_rr_32
	Instr(sb,Pre0,Pre9, 0xf1f8, 0x9108, 0x000, Mnemonic.subx),  // d68000_subx_mm_8
	Instr(sw,Pre0,Pre9, 0xf1f8, 0x9148, 0x000, Mnemonic.subx),  // d68000_subx_mm_16
	Instr(sl,Pre0,Pre9, 0xf1f8, 0x9188, 0x000, Mnemonic.subx),  // d68000_subx_mm_32
	Instr(sl,D0,      0xfff8, 0x4840, 0x000, Mnemonic.swap),    // d68000_swap
	Instr(d68000_tas          , 0xffc0, 0x4ac0, 0xbf8),
    Instr(d68000_trap         , 0xfff0, 0x4e40, 0x000, iclass:InstrClass.Transfer|InstrClass.Call),
    Instr(d68020_trapcc_0     , 0xf0ff, 0x50fc, 0x000, iclass:InstrClass.Transfer|InstrClass.Call),
    Instr(d68020_trapcc_16    , 0xf0ff, 0x50fa, 0x000, iclass:InstrClass.Transfer|InstrClass.Call),
    Instr(d68020_trapcc_32    , 0xf0ff, 0x50fb, 0x000, iclass:InstrClass.Transfer|InstrClass.Call),
    Instr(0xffff, 0x4e76, 0x000, Mnemonic.trapv, InstrClass.Transfer|InstrClass.Call),  // d68000_trapv
	Instr(sb,E0, 0xffc0, 0x4a00, 0xbf8, Mnemonic.tst),              // d68000_tst_8
	Instr(sb,E0, 0xffff, 0x4a3a, 0x000, Mnemonic.tst),              // d68020_tst_pcdi_8
	Instr(sb,E0, 0xffff, 0x4a3b, 0x000, Mnemonic.tst),              // d68020_tst_pcix_8
	Instr(sb,E0, 0xffff, 0x4a3c, 0x000, Mnemonic.tst),              // d68020_tst_i_8
	Instr(sw,E0, 0xffc0, 0x4a40, 0xbf8, Mnemonic.tst),              // d68000_tst_16
	Instr(sw,E0, 0xfff8, 0x4a48, 0x000, Mnemonic.tst),              // d68020_tst_a_16
	Instr(sw,E0, 0xffff, 0x4a7a, 0x000, Mnemonic.tst),              // d68020_tst_pcdi_16
	Instr(sw,E0, 0xffff, 0x4a7b, 0x000, Mnemonic.tst),              // d68020_tst_pcix_16
	Instr(sw,E0, 0xffff, 0x4a7c, 0x000, Mnemonic.tst),              // d68020_tst_i_16
	Instr(sl,E0, 0xffc0, 0x4a80, 0xbf8, Mnemonic.tst),              // d68000_tst_32
	Instr(sl,E0, 0xfff8, 0x4a88, 0x000, Mnemonic.tst),              // d68020_tst_a_32
	Instr(sl,E0, 0xffff, 0x4aba, 0x000, Mnemonic.tst),              // d68020_tst_pcdi_32
	Instr(sl,E0, 0xffff, 0x4abb, 0x000, Mnemonic.tst),              // d68020_tst_pcix_32
	Instr(sl,E0, 0xffff, 0x4abc, 0x000, Mnemonic.tst),              // d68020_tst_i_32
	Instr(A0, 0xfff8, 0x4e58, 0x000, Mnemonic.unlk),                // d68000_unlk
	Instr(d68020_unpk_rr      , 0xf1f8, 0x8180, 0x000),
    Instr(d68020_unpk_mm      , 0xf1f8, 0x8188, 0x000),
    Instr(d68851_p000         , 0xffc0, 0xf000, 0x000),
    Instr(d68851_pbcc16       , 0xffc0, 0xf080, 0x000),
    Instr(d68851_pbcc32       , 0xffc0, 0xf0c0, 0x000),
    Instr(d68851_pdbcc        , 0xfff8, 0xf048, 0x000),
    Instr(d68851_p001         , 0xffc0, 0xf040, 0x000),
    Instr((u, d) => false, 0, 0, 0),
};
        }

        private static readonly InstrDecoder illegal = new InstrDecoder(Array.Empty<Mutator>(), 0, 0, 0, Mnemonic.illegal);

        // Check if opcode is using a valid ea mode
        static bool valid_ea(uint opcode, uint mask)
        {
            if (mask == 0)
                return true;

            switch (opcode & 0x3f)
            {
            case 0x00:
            case 0x01:
            case 0x02:
            case 0x03:
            case 0x04:
            case 0x05:
            case 0x06:
            case 0x07:
                return (mask & 0x800) != 0;
            case 0x08:
            case 0x09:
            case 0x0a:
            case 0x0b:
            case 0x0c:
            case 0x0d:
            case 0x0e:
            case 0x0f:
                return (mask & 0x400) != 0;
            case 0x10:
            case 0x11:
            case 0x12:
            case 0x13:
            case 0x14:
            case 0x15:
            case 0x16:
            case 0x17:
                return (mask & 0x200) != 0;
            case 0x18:
            case 0x19:
            case 0x1a:
            case 0x1b:
            case 0x1c:
            case 0x1d:
            case 0x1e:
            case 0x1f:
                return (mask & 0x100) != 0;
            case 0x20:
            case 0x21:
            case 0x22:
            case 0x23:
            case 0x24:
            case 0x25:
            case 0x26:
            case 0x27:
                return (mask & 0x080) != 0;
            case 0x28:
            case 0x29:
            case 0x2a:
            case 0x2b:
            case 0x2c:
            case 0x2d:
            case 0x2e:
            case 0x2f:
                return (mask & 0x040) != 0;
            case 0x30:
            case 0x31:
            case 0x32:
            case 0x33:
            case 0x34:
            case 0x35:
            case 0x36:
            case 0x37:
                return (mask & 0x020) != 0;
            case 0x38:
                return (mask & 0x010) != 0;
            case 0x39:
                return (mask & 0x008) != 0;
            case 0x3a:
                return (mask & 0x002) != 0;
            case 0x3b:
                return (mask & 0x001) != 0;
            case 0x3c:
                return (mask & 0x004) != 0;
            }
            return false;
        }

        static int compare_nof_true_bits(InstrDecoder aptr, InstrDecoder bptr)
        {
            int a = (int) aptr.mask;
            int b = (int) bptr.mask;

            a = ((a & 0xAAAA) >> 1) + (a & 0x5555);
            a = ((a & 0xCCCC) >> 2) + (a & 0x3333);
            a = ((a & 0xF0F0) >> 4) + (a & 0x0F0F);
            a = ((a & 0xFF00) >> 8) + (a & 0x00FF);

            b = ((b & 0xAAAA) >> 1) + (b & 0x5555);
            b = ((b & 0xCCCC) >> 2) + (b & 0x3333);
            b = ((b & 0xF0F0) >> 4) + (b & 0x0F0F);
            b = ((b & 0xFF00) >> 8) + (b & 0x00FF);

            return b - a; // reversed to get greatest to least sorting 
        }

        private static void BuildDecoderTable()
        {
            int ostruct;
            InstrDecoder[] opcode_info = (InstrDecoder[]) g_opcode_info.Clone();
            Array.Sort(opcode_info, compare_nof_true_bits);

            for (uint i = 0; i < 0x10000; i++)
            {
                g_instruction_table[i] = illegal;     //default to illegal
                uint opcode = i;
                // search through opcode info for a match
                for (ostruct = 0; opcode_info[ostruct].mutators is not null; ostruct++)
                {
                    // match opcode mask and allowed ea modes
                    if ((opcode & opcode_info[ostruct].mask) == opcode_info[ostruct].match)
                    {
                        if (opcode_info[ostruct].mutators.Length != 0)
                        {
                            // Handle destination ea for move dasm.instructions 
                            if ((opcode_info[ostruct].mutators[0] == d68000_move_8 ||
                                 opcode_info[ostruct].mutators[0] == d68000_move_16 ||
                                 opcode_info[ostruct].mutators[0] == d68000_move_32) &&
                                 !valid_ea(opcode >> 9 & 7 | opcode >> 3 & 0x38, 0xbf8))
                                continue;
                        }
                        if (valid_ea(opcode, opcode_info[ostruct].ea_mask))
                        {
                            g_instruction_table[i] = opcode_info[ostruct];
                            break;
                        }
                    }
                }
            }
        }
    }
}