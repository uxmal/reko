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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Arch.M68k
{
    // M68k opcode map in http://www.freescale.com/files/archives/doc/ref_manual/M68000PRM.pdf

    public partial class M68kDisassembler : DisassemblerBase<M68kInstruction>
    {
        private static TraceSwitch trace = new TraceSwitch("m68dasm", "Detailed tracing of M68k disassembler");

        private delegate bool Mutator(M68kDisassembler dasm);

        public const string HexStringFormat = "{0}${1}";

        private EndianImageReader rdr;          // program counter 
        private List<MachineOperand> ops;       // Operand list being built
        internal M68kInstruction instr;         // instruction being built
        internal ushort uInstr;                 // 16-bit instruction
        private string g_dasm_str;              //string to hold disassembly: OBSOLETE
        private PrimitiveType dataWidth;        // width of data.
        private ushort bitSet;                  // Bit set.
        private uint g_cpu_type = 0;

        private M68kDisassembler(EndianImageReader rdr, uint cpuType)
        {
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.g_cpu_type = cpuType;
        }

        public static M68kDisassembler Create68000(EndianImageReader rdr) { return new M68kDisassembler(rdr, TYPE_68000); }
        public static M68kDisassembler Create68010(EndianImageReader rdr) { return new M68kDisassembler(rdr, TYPE_68010); }
        public static M68kDisassembler Create68020(EndianImageReader rdr) { return new M68kDisassembler(rdr, TYPE_68020); }

        static M68kDisassembler()
        {
            GenTable();
            build_opcode_table();
        }

        public override M68kInstruction DisassembleInstruction()
        {
            var addr = rdr.Address;
            if (!rdr.TryReadBeUInt16(out uInstr))
                return null;
            var offset = rdr.Offset;
            Decoder handler = g_instruction_table[uInstr];
            try
            {
                instr = new M68kInstruction
                {
                    Address = addr,
                    InstructionClass = handler.iclass
                };
                instr = handler.UseDecodeMethod(this);
                if (instr == null)
                    instr = Invalid();
            }
            catch
            {
                instr = Invalid();
            }
            ops.Clear();
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            instr.InstructionClass |= (uInstr == 0) ? InstrClass.Zero : 0;
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
        internal static bool BIT_0(uint A) { return ((A) & 0x00000001) != 0; }
        internal static bool BIT_1(uint A) { return ((A) & 0x00000002) != 0; }
        internal static bool BIT_2(uint A) { return ((A) & 0x00000004) != 0; }
        internal static bool BIT_3(uint A) { return ((A) & 0x00000008) != 0; }
        internal static bool BIT_4(uint A) { return ((A) & 0x00000010) != 0; }
        internal static bool BIT_5(uint A) { return ((A) & 0x00000020) != 0; }
        internal static bool BIT_6(uint A) { return ((A) & 0x00000040) != 0; }
        internal static bool BIT_7(uint A) { return ((A) & 0x00000080) != 0; }
        internal static bool BIT_8(uint A) { return ((A) & 0x00000100) != 0; }
        internal static bool BIT_9(uint A) { return ((A) & 0x00000200) != 0; }
        internal static bool BIT_A(uint A) { return ((A) & 0x00000400) != 0; }
        internal static bool BIT_B(uint A) { return ((A) & 0x00000800) != 0; }
        internal static bool BIT_C(uint A) { return ((A) & 0x00001000) != 0; }
        internal static bool BIT_D(uint A) { return ((A) & 0x00002000) != 0; }
        internal static bool BIT_E(uint A) { return ((A) & 0x00004000) != 0; }
        internal static bool BIT_F(uint A) { return ((A) & 0x00008000) != 0; }
        internal static bool BIT_10(uint A) { return ((A) & 0x00010000) != 0; }
        internal static bool BIT_11(uint A) { return ((A) & 0x00020000) != 0; }
        internal static bool BIT_12(uint A) { return ((A) & 0x00040000) != 0; }
        internal static bool BIT_13(uint A) { return ((A) & 0x00080000) != 0; }
        internal static bool BIT_14(uint A) { return ((A) & 0x00100000) != 0; }
        internal static bool BIT_15(uint A) { return ((A) & 0x00200000) != 0; }
        internal static bool BIT_16(uint A) { return ((A) & 0x00400000) != 0; }
        internal static bool BIT_17(uint A) { return ((A) & 0x00800000) != 0; }
        internal static bool BIT_18(uint A) { return ((A) & 0x01000000) != 0; }
        internal static bool BIT_19(uint A) { return ((A) & 0x02000000) != 0; }
        internal static bool BIT_1A(uint A) { return ((A) & 0x04000000) != 0; }
        internal static bool BIT_1B(uint A) { return ((A) & 0x08000000) != 0; }
        internal static bool BIT_1C(uint A) { return ((A) & 0x10000000) != 0; }
        internal static bool BIT_1D(uint A) { return ((A) & 0x20000000) != 0; }
        internal static bool BIT_1E(uint A) { return ((A) & 0x40000000) != 0; }
        internal static bool BIT_1F(uint A) { return ((A) & 0x80000000) != 0; }

        /* These are the CPU types understood by this disassembler */
        private const uint TYPE_68000 = 1;
        private const uint TYPE_68008 = 2;
        private const uint TYPE_68010 = 4;
        private const uint TYPE_68020 = 8;
        private const uint TYPE_68030 = 16;
        private const uint TYPE_68040 = 32;

        private const uint M68000_ONLY = (TYPE_68000 | TYPE_68008);

        private const uint M68010_ONLY = TYPE_68010;
        private const uint M68010_LESS = (TYPE_68000 | TYPE_68008 | TYPE_68010);
        private const uint M68010_PLUS = (TYPE_68010 | TYPE_68020 | TYPE_68030 | TYPE_68040);

        private const uint M68020_ONLY = TYPE_68020;
        private const uint M68020_LESS = (TYPE_68010 | TYPE_68020);
        private const uint M68020_PLUS = (TYPE_68020 | TYPE_68030 | TYPE_68040);

        private const uint M68030_ONLY = TYPE_68030;
        private const uint M68030_LESS = (TYPE_68010 | TYPE_68020 | TYPE_68030);
        private const uint M68030_PLUS = (TYPE_68030 | TYPE_68040);

        private const uint M68040_PLUS = TYPE_68040;


        /* Extension word formats */
        private static sbyte EXT_8BIT_DISPLACEMENT(uint A) { return (sbyte)((A) & 0xff); }
        internal static bool EXT_FULL(uint A) { return BIT_8(A); }
        internal static bool EXT_EFFECTIVE_ZERO(uint A) { return (((A) & 0xe4) == 0xc4 || ((A) & 0xe2) == 0xc0); }
        private static bool EXT_BASE_REGISTER_PRESENT(uint A) { return !(BIT_7(A)); }
        private static bool EXT_INDEX_REGISTER_PRESENT(uint A) { return !(BIT_6(A)); }
        private static uint EXT_INDEX_REGISTER(uint A) { return (((A) >> 12) & 7); }
        private static bool EXT_INDEX_PRE_POST(uint A) { return (EXT_INDEX_REGISTER_PRESENT(A) && (A & 3) != 0); }
        private static bool EXT_INDEX_PRE(uint A) { return (EXT_INDEX_REGISTER_PRESENT(A) && ((A) & 7) < 4 && ((A) & 7) != 0); }
        private static bool EXT_INDEX_POST(uint A) { return (EXT_INDEX_REGISTER_PRESENT(A) && ((A) & 7) > 4); }
        internal static int EXT_INDEX_SCALE(uint A) { return (int)(((A) >> 9) & 3); }
        private static bool EXT_INDEX_LONG(uint A) { return BIT_B(A); }
        private static bool EXT_INDEX_AR(uint A) { return BIT_F(A); }
        private static bool EXT_BASE_DISPLACEMENT_PRESENT(uint A) { return (((A) & 0x30) > 0x10); }
        private static bool EXT_BASE_DISPLACEMENT_WORD(uint A) { return (((A) & 0x30) == 0x20); }
        private static bool EXT_BASE_DISPLACEMENT_LONG(uint A) { return (((A) & 0x30) == 0x30); }
        private static bool EXT_OUTER_DISPLACEMENT_PRESENT(uint A) { return (((A) & 3) > 1 && ((A) & 0x47) < 0x44); }
        private static bool EXT_OUTER_DISPLACEMENT_WORD(uint A) { return (((A) & 3) == 2 && ((A) & 0x47) < 0x44); }
        private static bool EXT_OUTER_DISPLACEMENT_LONG(uint A) { return (((A) & 3) == 3 && ((A) & 0x47) < 0x44); }

        private static M68kInstruction Invalid()
        {
            return new M68kInstruction
            {
                code = Opcode.illegal,
                InstructionClass = InstrClass.Invalid,
            };
        }

        /// <summary>
        /// Decoders provide the knowledge for how to decode a M68k instruction.
        /// </summary>
        /// <remarks>
        /// Decoders follow the flyweight pattern; they must never have modifyable state. Therefore,
        /// all the fields are readonly.
        /// </remarks>
        private class Decoder
        {
            public readonly uint mask;                  // opcode mask
            public readonly uint match;                 // opcode bit patter (after mask)
            public readonly uint ea_mask;               // Permitted ea modes are allowed 

            public readonly Opcode opcode;              // The decoded opcode.
            public readonly InstrClass iclass;          // Instruction class of the decoded instruction.
            public readonly Mutator[] mutators;         // Format string which when interpreted generates the operands of the dasm.instruction.

            /// <summary>
            /// Builds an instance of an opcode record that uses <seealso cref="OperandFormatDecoder"/> to decode
            /// the operands. 
            /// </summary>
            /// <remarks>
            /// This is the preferred constructor. Contributors are kindly asked to translate calls to the 
            /// other, deprecated, constructor to this one.
            /// </remarks>
            /// <param name="opFormat">string to be passed to OperandFormatDecoder</param>
            /// <param name="mask"></param>
            /// <param name="match"></param>
            /// <param name="ea_mask"></param>
            /// <param name="opcode"></param>
            /// <param name="iclass"></param>
            public Decoder(Mutator[] mutators, uint mask, uint match, uint ea_mask, Opcode opcode = Opcode.illegal, InstrClass iclass = InstrClass.Linear)
            {
                this.mutators = mutators;
                if (mutators.Any(m => m == null))
                    throw new ArgumentException();
                this.mask = mask;
                this.match = match;
                this.ea_mask = ea_mask;
                this.opcode = opcode;
                this.iclass = iclass;
            }

            public virtual M68kInstruction UseDecodeMethod(M68kDisassembler dasm)
            {
                var instr = dasm.instr;
                instr.code = opcode;
                instr.InstructionClass = iclass;
                foreach (var m in mutators)
                {
                    if (!m(dasm))
                        return Invalid();
                }
                if (dasm.ops.Count > 0)
                {
                    dasm.instr.op1 = dasm.ops[0];
                    if (dasm.ops.Count > 1)
                    {
                        dasm.instr.op2 = dasm.ops[1];
                        if (dasm.ops.Count > 2)
                        {
                            dasm.instr.op2 = dasm.ops[2];
                        }
                    }
                }
                return dasm.instr;
            }
        }

        private static Decoder Instr(uint mask, uint match, uint ea_mask, Opcode opcode, InstrClass iclass = InstrClass.Linear)
        {
            return new Decoder(
                new Mutator[0],
                mask,
                match,
                ea_mask,
                opcode,
                iclass);
        }

        private static Decoder Instr(Mutator m1, uint mask, uint match, uint ea_mask, Opcode opcode = Opcode.illegal, InstrClass iclass = InstrClass.Linear)
        {
            return new Decoder(
                new Mutator[] { m1 },
                mask,
                match,
                ea_mask,
                opcode,
                iclass);
        }

        private static Decoder Instr(Mutator m1, Mutator m2, uint mask, uint match, uint ea_mask, Opcode opcode, InstrClass iclass = InstrClass.Linear)
        {
            return new Decoder(
                new Mutator[] { m1, m2 },
                mask,
                match,
                ea_mask,
                opcode,
                iclass);
        }

        private static Decoder Instr(Mutator m1, Mutator m2, Mutator m3, uint mask, uint match, uint ea_mask, Opcode opcode, InstrClass iclass = InstrClass.Linear)
        {
            return new Decoder(
                new Mutator[] { m1, m2, m3 },
                mask,
                match,
                ea_mask,
                opcode,
                iclass);
        }

        private static Decoder Instr(Mutator m1, Mutator m2, Mutator m3, Mutator m4, uint mask, uint match, uint ea_mask, Opcode opcode, InstrClass iclass = InstrClass.Linear)
        {
            return new Decoder(
                new Mutator[] { m1, m2, m3, m4 },
                mask,
                match,
                ea_mask,
                opcode,
                iclass);
        }

        // Opcode handler jump table 
        private static Decoder[] g_instruction_table = new Decoder[0x10000];

        // 'Q'uick dasm.instructions contain patterns that map to integers.
        private static uint[] g_3bit_qdata_table = new uint[8] 
        {
            8, 1, 2, 3, 4, 5, 6, 7 
        };

        private static uint[] g_5bit_data_table = new uint[32]
        {
	        32,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15,
	        16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31
        };

        private static Opcode[] g_bcc = new Opcode[16] { 
            Opcode.bt,  Opcode.bf,  Opcode.bhi, Opcode.bls, Opcode.bcc, Opcode.bcs, Opcode.bne, Opcode.beq, 
            Opcode.bvc, Opcode.bvs, Opcode.bpl, Opcode.bmi, Opcode.bge, Opcode.blt, Opcode.bgt, Opcode.ble };
        private static Opcode[] g_dbcc = new Opcode[16] { 
            Opcode.dbt,  Opcode.dbf,  Opcode.dbhi, Opcode.dbls, Opcode.dbcc, Opcode.dbcs, Opcode.dbne, Opcode.dbeq, 
            Opcode.dbvc, Opcode.dbvs, Opcode.dbpl, Opcode.dbmi, Opcode.dbge, Opcode.dblt, Opcode.dbgt, Opcode.dble };
        private static Opcode[] g_scc = new Opcode[16] { 
            Opcode.st,  Opcode.sf,  Opcode.shi, Opcode.sls, Opcode.scc, Opcode.scs, Opcode.sne, Opcode.seq, 
            Opcode.svc, Opcode.svs, Opcode.spl, Opcode.smi, Opcode.sge, Opcode.slt, Opcode.sgt, Opcode.sle };
        private static Opcode[] g_trapcc = new Opcode[16] { 
            Opcode.trapt,  Opcode.trapf,  Opcode.traphi, Opcode.trapls, Opcode.trapcc, Opcode.trapcs, Opcode.trapne, Opcode.trapeq, 
            Opcode.trapvc, Opcode.trapvs, Opcode.trappl, Opcode.trapmi, Opcode.trapge, Opcode.traplt, Opcode.trapgt, Opcode.traple
        };

        private static Opcode[] g_cpcc = new Opcode[64] 
        {
            /* 000            001            010            011    100    101    110    111 */
	          Opcode.fbf,     Opcode.fbeq,   Opcode.fbogt,  Opcode.fboge, Opcode.fbolt, Opcode.fbole, Opcode.fbogl,  Opcode.fbor, /* 000 */
	          Opcode.fbun,    Opcode.fbueq,  Opcode.fbugt,  Opcode.fbuge, Opcode.fbult, Opcode.fbule, Opcode.fbne,   Opcode.fbt, /* 001 */
	          Opcode.fbsf,    Opcode.fbseq,  Opcode.fbgt,   Opcode.fbge,  Opcode.fblt,  Opcode.fble,  Opcode.fbgl,   Opcode.fbgle, /* 010 */
              Opcode.fbngle,  Opcode.fbngl,  Opcode.fbnle,  Opcode.fbnlt, Opcode.fbnge, Opcode.fbngt, Opcode.fbsne,  Opcode.fbst, /* 011 */
	          Opcode.illegal, Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal,/* 100 */
	          Opcode.illegal, Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal, /* 101 */
	          Opcode.illegal, Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal, /* 110 */
	          Opcode.illegal, Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal,Opcode.illegal,/* 111 */
        };

        private static string[] g_mmuregs = new string[] 
        {
	        "tc", "drp", "srp", "crp", "cal", "val", "sccr", "acr"
        };

        private static string[] g_caches =
        {
            "", "dc", "ic", "bc"
        };

        private static Opcode[] g_mmucond =
        {
	        Opcode.pbbs, Opcode.pbbc, Opcode.pbls, Opcode.pblc,
            Opcode.pbss, Opcode.pbsc, Opcode.pbas, Opcode.pbac,
	        Opcode.pbws, Opcode.pbwc, Opcode.pbis, Opcode.pbic,
            Opcode.pbgs, Opcode.pbgc, Opcode.pbcs, Opcode.pbcc
        };

        // Utility functions 

        private bool LIMIT_CPU_TYPES(uint ALLOWED_CPU_TYPES)
        {
            if ((g_cpu_type & ALLOWED_CPU_TYPES) == 0)
            {
                if ((uInstr & 0xf000) == 0xf000)
                    d68000_1111(this);
                else d68000_illegal(this);
                return false;
            }
            return true;
        }

        private MachineOperand get_ea_mode_str_8(int instruction) { return get_ea_mode_str((uint) instruction, PrimitiveType.Byte); }
        private MachineOperand get_ea_mode_str_16(int instruction) { return get_ea_mode_str((uint) instruction, PrimitiveType.Word16); }
        private MachineOperand get_ea_mode_str_32(int instruction) { return get_ea_mode_str((uint) instruction, PrimitiveType.Word32); }

        private bool get_imm_str_s8(out M68kImmediateOperand imm) { return TryGetSignedImmediate(0, out imm); }
        private bool get_imm_str_s16(out M68kImmediateOperand imm) { return TryGetSignedImmediate(1, out imm); }
        private bool get_imm_str_s32(out M68kImmediateOperand imm) { return TryGetSignedImmediate(2, out imm); }

        private bool get_imm_str_u8(out M68kImmediateOperand  imm) { return get_imm_str_u(PrimitiveType.Byte, out imm); }

        private static RegisterOperand get_data_reg(int d) { return new RegisterOperand(Registers.DataRegister(d)); }
        private static RegisterOperand get_addr_reg(int a) { return new RegisterOperand(Registers.AddressRegister(a)); }
        private static RegisterOperand get_addr_or_data_reg(bool addrReg, int bits)
        {
            return addrReg ? get_addr_reg(bits) : get_data_reg(bits);
        }

        private RegisterOperand get_fp_reg(int fp) { return new RegisterOperand(Registers.FpRegister(fp)); }

        private PredecrementMemoryOperand get_pre_dec(int a)
        {
            return new PredecrementMemoryOperand(instr.dataWidth, Registers.AddressRegister(a & 7));
        }

        private PostIncrementMemoryOperand get_post_inc(int a)
        {
            return new PostIncrementMemoryOperand(instr.dataWidth, Registers.AddressRegister(a & 7));
        }

        private RegisterOperand get_ctrl_reg(string regName, int number)
        {
            return new RegisterOperand(new RegisterStorage(regName, number, 0, PrimitiveType.Word16));
        }

        private DoubleRegisterOperand get_double_data_reg(uint d1, uint d2)
        {
            return new DoubleRegisterOperand(
                Registers.DataRegister((int)d1&7),
                Registers.DataRegister((int)d2&7));
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
        private bool TryGetSignedImmediate(uint size, out M68kImmediateOperand imm)
        {
            imm = null;
            if (size == 0)
            {
                if (!TryReadImm8(out byte b))
                    return false;
                imm = new M68kImmediateOperand(Constant.SByte((sbyte)b));
            }
            else if (size == 1)
            {
                if (!rdr.TryReadBeInt16(out short s))
                    return false;
                imm = new M68kImmediateOperand(Constant.Int16(s));
            }
            else
            {
                if (!rdr.TryReadBeInt32(out int i))
                    return false;
                imm = new M68kImmediateOperand(Constant.Int32(i));
            }
            return true;
        }

        private bool get_imm_str_u(PrimitiveType dt, out M68kImmediateOperand imm)
        {
            Constant c;
            imm = null;
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
            imm = new M68kImmediateOperand(c);
            return true;
        }

        static string b1 = "";
        static string b2 = "";
        static string mode = b2;

        /// <summary>
        /// Build an effective address.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private MachineOperand get_ea_mode_str(uint instruction, PrimitiveType dataWidth)
        {
            ushort extension;
            bool preindex;
            bool postindex;

            /* Switch buffers so we don't clobber on a double-call to this function */
            mode = mode == b1 ? b2 : b1;

            switch (instruction & 0x3f)
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
                return get_data_reg((int)instruction & 7);
            case 0x08:
            case 0x09:
            case 0x0a:
            case 0x0b:
            case 0x0c:
            case 0x0d:
            case 0x0e:
            case 0x0f:
                // address register direct 
                return get_addr_reg((int)instruction & 7);
            case 0x10:
            case 0x11:
            case 0x12:
            case 0x13:
            case 0x14:
            case 0x15:
            case 0x16:
            case 0x17:
                // address register indirect
                return MemoryOperand.Indirect(dataWidth, Registers.AddressRegister((int)instruction & 7));
            case 0x18:
            case 0x19:
            case 0x1a:
            case 0x1b:
            case 0x1c:
            case 0x1d:
            case 0x1e:
            case 0x1f:
                // address register indirect with postincrement
                return new PostIncrementMemoryOperand(dataWidth, Registers.AddressRegister((int)instruction & 7));
            case 0x20:
            case 0x21:
            case 0x22:
            case 0x23:
            case 0x24:
            case 0x25:
            case 0x26:
            case 0x27:
                // address register indirect with predecrement
                return new PredecrementMemoryOperand(dataWidth, Registers.AddressRegister((int)instruction & 7));
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
                    instr.dataWidth,
                    Registers.AddressRegister((int) instruction & 7),
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
                if (!rdr.TryReadBeUInt16(out extension))
                {
                    return null;
                }
                if (EXT_FULL(extension))
                {
                    if (M68kDisassembler.EXT_EFFECTIVE_ZERO(extension))
                    {
                        return new M68kAddressOperand(Address.Ptr32(0));
                    }

                    RegisterStorage base_reg = null;
                    RegisterStorage index_reg = null;
                    PrimitiveType index_reg_width = null;
                    int index_scale = 1;
                    Constant @base = null;
                    if (EXT_BASE_DISPLACEMENT_PRESENT(extension))
                    {
                        @base = rdr.ReadBe(EXT_BASE_DISPLACEMENT_LONG(extension) ? PrimitiveType.Word32 : PrimitiveType.Int16);
                    }

                    Constant outer = null;
                    if (EXT_OUTER_DISPLACEMENT_PRESENT(extension))
                    {
                        outer = rdr.ReadBe(EXT_OUTER_DISPLACEMENT_LONG(extension) ? PrimitiveType.Word32 : PrimitiveType.Int16);
                    }
                    if (EXT_BASE_REGISTER_PRESENT(extension))
                    {
                        base_reg = Registers.AddressRegister(uInstr & 7);
                    }
                    if (EXT_INDEX_REGISTER_PRESENT(extension))
                    {
                        index_reg = EXT_INDEX_AR(extension)
                            ? Registers.AddressRegister((int) EXT_INDEX_REGISTER(extension))
                            : Registers.DataRegister((int) EXT_INDEX_REGISTER(extension));
                        index_reg_width = EXT_INDEX_LONG(extension) ? PrimitiveType.Word32 : PrimitiveType.Word16;
                        if (EXT_INDEX_SCALE(extension) != 0)
                            index_scale = 1 << EXT_INDEX_SCALE(extension);
                    }
                    preindex = (extension & 7) > 0 && (extension & 7) < 4;
                    postindex = (extension & 7) > 4;
                    return new IndexedOperand(dataWidth, @base, outer, base_reg, index_reg, index_reg_width, index_scale, preindex, postindex);
                }
                else
                {
                    return new IndirectIndexedOperand(
                        dataWidth,
                        EXT_8BIT_DISPLACEMENT(extension),
                        Registers.AddressRegister(uInstr & 7),
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
                return new M68kAddressOperand(usAddr);
            case 0x39:
                // Absolute long address
                if (!rdr.TryReadBeUInt32(out uint uAddr))
                    return null;
                return new M68kAddressOperand(uAddr);
            case 0x3A:
                // Program counter with displacement
                var off = rdr.Address - instr.Address;
                short sOffset;
                if (!rdr.TryReadBeInt16(out sOffset))
                {
                    return null;
                }
                off += sOffset;
                return new MemoryOperand(dataWidth, Registers.pc, Constant.Int16((short)off));
            case 0x3B:
                // Program counter with index
                var extension_offset = (short) (rdr.Address - instr.Address);
                var addrExt = rdr.Address;
                if (!rdr.TryReadBeUInt16(out extension))
                {
                    return null;
                }

                if (EXT_FULL(extension))
                {
                    if (EXT_EFFECTIVE_ZERO(extension))
                    {
                        return new M68kImmediateOperand(Constant.Word32(0));
                    }
                    Constant @base = null;
                    Constant outer = null;
                    if (EXT_BASE_DISPLACEMENT_PRESENT(extension))
                        @base = EXT_BASE_DISPLACEMENT_LONG(extension)
                            ? rdr.ReadBe(PrimitiveType.Word32)
                            : rdr.ReadBe(PrimitiveType.Int16);
                    if (EXT_OUTER_DISPLACEMENT_PRESENT(extension))
                        outer = EXT_OUTER_DISPLACEMENT_LONG(extension)
                            ? rdr.ReadBe(PrimitiveType.Word32)
                            : rdr.ReadBe(PrimitiveType.Int16);
                    RegisterStorage base_reg = EXT_BASE_REGISTER_PRESENT(extension)
                        ? Registers.pc
                        : null;
                    RegisterStorage index_reg = null;
                    PrimitiveType index_width = null;
                    int index_scale = 0;
                    if (EXT_INDEX_REGISTER_PRESENT(extension))
                    {
                        index_reg = EXT_INDEX_AR(extension)
                            ? Registers.AddressRegister((int) EXT_INDEX_REGISTER(extension))
                            : Registers.DataRegister((int) EXT_INDEX_REGISTER(extension));
                        index_width = EXT_INDEX_LONG(extension) ? PrimitiveType.Word32 : PrimitiveType.Int16;
                        index_scale = (EXT_INDEX_SCALE(extension) != 0)
                            ? 1 << EXT_INDEX_SCALE(extension)
                            : 0;
                    }
                    return  new IndexedOperand(dataWidth, @base, outer, base_reg, index_reg, index_width, index_scale,
                        (extension & 7) > 0 && (extension & 7) < 4,
                        (extension & 7) > 4);
                }
                return new IndirectIndexedOperand(
                    dataWidth,
                    (sbyte)(extension_offset + EXT_8BIT_DISPLACEMENT(extension)),
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
                if (!rdr.TryReadBe(dataWidth, out Constant coff))
                {
                    return null;
                }
                return new M68kImmediateOperand(coff);
            }
            this.instr.code = Opcode.illegal;
            return null;
        }

        private static M68kInstruction CreateInstruction(
            Opcode code,
            InstrClass iclass,
            PrimitiveType width,
            MachineOperand op1,
            MachineOperand op2)
        {
            if (op1 == null || op2 == null)
                return Invalid();

            return new M68kInstruction
            {
                code = code,
                InstructionClass = iclass,
                dataWidth = width,
                op1 = op1,
                op2 = op2,
            };
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

        private static bool d68000_illegal(M68kDisassembler dasm)
        {
            return false;
            //throw new NotSupportedException(string.Format("dc.w    ${0:X}; ILLEGAL", dasm.instruction));
        }

        private static bool d68000_1010(M68kDisassembler dasm)
        {
            if (trace.TraceVerbose) Debug.Print("dc.w    ${0:X4}; opcode 1010", dasm.uInstr);
            dasm.instr.code = Opcode.illegal;
            dasm.instr.InstructionClass = InstrClass.Invalid;
            dasm.instr.op1 = new M68kImmediateOperand(Constant.Word16(dasm.uInstr));
            return true;
        }

        private static bool d68000_1111(M68kDisassembler dasm)
        {
            if (trace.TraceVerbose) Debug.Print("dc.w    ${0:X4}; opcode 1111", dasm.uInstr);
            return false;
        }

        private static bool d68000_bcc_8(M68kDisassembler dasm)
        {
            var temp_pc = dasm.rdr.Address + (sbyte) dasm.uInstr;
            dasm.instr.code = g_bcc[(dasm.uInstr >> 8) & 0xf];
            dasm.instr.InstructionClass = InstrClass.ConditionalTransfer;
            dasm.instr.op1 = new M68kAddressOperand(temp_pc);
            return true;
        }

        private static bool d68000_bcc_16(M68kDisassembler dasm)
        {
            var temp_pc = dasm.rdr.Address;
            if (!dasm.rdr.TryReadBeInt16(out short s))
                return false;
            dasm.instr.code = g_bcc[(dasm.uInstr >> 8) & 0xf]; 
            dasm.instr.InstructionClass = InstrClass.ConditionalTransfer;
            dasm.instr.op1 = new M68kAddressOperand(temp_pc + s);
            return true;
        }

        private static bool d68020_bcc_32(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            var temp_pc = dasm.rdr.Address;
            if (!dasm.rdr.TryReadBeInt32(out int s))
                return false;
            dasm.instr.code = g_bcc[(dasm.uInstr >> 8) & 0xf]; 
            dasm.instr.InstructionClass = InstrClass.ConditionalTransfer;
            dasm.instr.op1 = new M68kAddressOperand(temp_pc + s);
            return true;
        }

        private static bool d68010_bkpt(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68010_PLUS);
            dasm.instr.code = Opcode.bkpt;
            dasm.instr.InstructionClass = InstrClass.System;
            dasm.instr.op1 = new M68kImmediateOperand(Constant.Byte((byte) (dasm.uInstr & 7)));
            return true;
        }

        private static bool d68020_bfchg(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            MachineOperand offset;
            if (BIT_B(extension))
                offset = get_data_reg((extension >> 6) & 7);
            else
                offset = new M68kImmediateOperand(Constant.Byte((byte)((extension >> 6) & 31)));

            MachineOperand width;
            if (BIT_5(extension))
                width = new RegisterOperand(Registers.DataRegister(extension & 7));
            else
                width = new M68kImmediateOperand(Constant.UInt32(g_5bit_data_table[extension & 31]));

            dasm.instr.code = Opcode.bfchg;
            dasm.instr.op1 = dasm.get_ea_mode_str_8(dasm.uInstr);
            dasm.instr.op2 = new BitfieldOperand(PrimitiveType.Word32, offset, width);
            return true;
        }

        private static bool d68020_bfclr(M68kDisassembler dasm)
        {
            MachineOperand offset;
            MachineOperand width;

            dasm.LIMIT_CPU_TYPES(M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            if (BIT_B(extension))
                offset = get_data_reg((extension >> 6) & 7);
            else
                offset = new M68kImmediateOperand(Constant.Byte((byte)((extension >> 6) & 31)));
            if (BIT_5(extension))
                width = new RegisterOperand(Registers.DataRegister(extension & 7));
            else
                width = new M68kImmediateOperand(Constant.UInt32(g_5bit_data_table[extension & 31]));
            dasm.instr.code = Opcode.bfclr;
            dasm.instr.op1 = dasm.get_ea_mode_str_8(dasm.uInstr);
            dasm.instr.op2 = new BitfieldOperand(PrimitiveType.Word32, offset, width);
            return true;
        }

        private static bool d68020_bfexts(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            MachineOperand offset;
            if (BIT_B(extension))
                offset = get_data_reg((extension >> 6) & 7);
            else
                offset = new M68kImmediateOperand(Constant.Byte((byte) ((extension >> 6) & 31)));
            MachineOperand width;
            if (BIT_5(extension))
                width = new RegisterOperand(Registers.DataRegister(extension & 7));
            else
                width = new M68kImmediateOperand(Constant.UInt32(g_5bit_data_table[extension & 31]));
            dasm.instr.code = Opcode.bfexts;
            dasm.instr.op1 = get_data_reg((extension >> 12) & 7);
            dasm.instr.op2 = dasm.get_ea_mode_str_8(dasm.uInstr);
            dasm.instr.op3 = new BitfieldOperand(PrimitiveType.Word32, offset, width);
            return true;
        }

        private static bool d68020_bfextu(M68kDisassembler dasm)
        {
            return false;
            //uint extension;
            //string offset; ;
            //string width; ;

            //dasm.LIMIT_CPU_TYPES(M68020_PLUS);

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

        private static bool d68020_bfffo(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            var offset = BIT_B(extension)
                ? new RegisterOperand(Registers.DataRegister((extension >> 6) & 7))
                : (MachineOperand) new M68kImmediateOperand(Constant.Int32((extension >> 6) & 31));
            var width = BIT_5(extension)
                ? new RegisterOperand(Registers.DataRegister(extension & 7))
                : (MachineOperand) new M68kImmediateOperand(Constant.Int32((int) g_5bit_data_table[extension & 31]));
            dasm.instr.code = Opcode.bfffo;
            dasm.instr.op1 = get_data_reg((extension >> 12) & 7);
            dasm.instr.op2 = dasm.get_ea_mode_str_8(dasm.uInstr);
            dasm.instr.op3 = new BitfieldOperand(PrimitiveType.Word32, offset, width);
            return true;
        }

        private static bool d68020_bfins(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            var offset = BIT_B(extension)
                ? new RegisterOperand(Registers.DataRegister((extension >> 6) & 7))
                : (MachineOperand) new M68kImmediateOperand(Constant.Int32((extension >> 6) & 31));
            var width = BIT_5(extension)
                ? new RegisterOperand(Registers.DataRegister(extension & 7))
                : (MachineOperand) new M68kImmediateOperand(Constant.Int32((int)g_5bit_data_table[extension & 31]));
            dasm.instr.code = Opcode.bfins;
            dasm.instr.op1 = new RegisterOperand(Registers.DataRegister((extension >> 12) & 7));
            dasm.instr.op2 = dasm.get_ea_mode_str_8(dasm.uInstr);
            dasm.instr.op3 = new BitfieldOperand(PrimitiveType.Word32, offset, width);
            return true;
        }

        private static bool d68020_bfset(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            var offset = BIT_B(extension)
                ? new RegisterOperand(Registers.DataRegister((extension >> 6) & 7))
                : (MachineOperand) new M68kImmediateOperand(Constant.Int32((extension >> 6) & 31));
            var width = BIT_5(extension)
                ? new RegisterOperand(Registers.DataRegister(extension & 7))
                : (MachineOperand) new M68kImmediateOperand(Constant.Int32((int)g_5bit_data_table[extension & 31]));
            dasm.instr.code = Opcode.bfins;
            dasm.instr.op1 = dasm.get_ea_mode_str_8(dasm.uInstr);
            dasm.instr.op2 = new BitfieldOperand(PrimitiveType.Word32, offset, width);
            return true;
        }

        private static bool d68020_bftst(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            var offset = BIT_B(extension)
                ? new RegisterOperand(Registers.DataRegister((extension >> 6) & 7))
                : (MachineOperand) new M68kImmediateOperand(Constant.Int32((extension >> 6) & 31));
            var width = BIT_5(extension)
                ? new RegisterOperand(Registers.DataRegister(extension & 7))
                : (MachineOperand) new M68kImmediateOperand(Constant.Int32((int) g_5bit_data_table[extension & 31]));
            dasm.instr.code = Opcode.bftst;
            dasm.instr.op1 = dasm.get_ea_mode_str_8(dasm.uInstr);
            dasm.instr.op2 = new BitfieldOperand(PrimitiveType.Word32, offset, width);
            return true;
        }

        private static bool d68020_callm(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_ONLY);
            if (!dasm.TryReadImm8(out byte b))
                return false;
            dasm.instr.code = Opcode.callm;
            dasm.instr.InstructionClass = InstrClass.Transfer|InstrClass.Call;
            dasm.instr.op1 = new M68kImmediateOperand(Constant.Byte(b));
            dasm.instr.op2 = dasm.get_ea_mode_str_8(dasm.uInstr);
            return true;
        }

        private static bool d68020_cas_8(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            dasm.instr.code = Opcode.cas;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Byte;
            dasm.instr.op1 = get_data_reg((int) extension & 7);
            dasm.instr.op2 = get_data_reg((int) (extension >> 8) & 7);
            dasm.instr.op3 = dasm.get_ea_mode_str_8(dasm.uInstr);
            return true;
        }

        private static bool d68020_cas_16(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            dasm.instr.code = Opcode.cas;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            dasm.instr.op1 = get_data_reg((int) extension & 7);
            dasm.instr.op2 = get_data_reg((int) (extension >> 8) & 7);
            dasm.instr.op3 = dasm.get_ea_mode_str_16(dasm.uInstr);
            return true;
        }

        private static bool d68020_cas_32(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            dasm.instr.code = Opcode.cas;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            dasm.instr.op1 = get_data_reg((int) extension & 7);
            dasm.instr.op2 = get_data_reg((int) (extension >> 8) & 7);
            dasm.instr.op3 = dasm.get_ea_mode_str_32(dasm.uInstr);
            return true;
        }

        private static bool d68020_cas2_16(M68kDisassembler dasm)
        {
            /* CAS2 Dc1:Dc2,Du1:Dc2:(Rn1):(Rn2)
            f e d c b a 9 8 7 6 5 4 3 2 1 0
             DARn1  0 0 0  Du1  0 0 0  Dc1
             DARn2  0 0 0  Du2  0 0 0  Dc2
            */

            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt32(out uint extension))
                return false;
            dasm.g_dasm_str = string.Format("cas2.w  D{0}:D%d:D%d:D%d, (%c%d):(%c%d); (2+)",
                (extension >> 16) & 7, extension & 7, (extension >> 22) & 7, (extension >> 6) & 7,
                BIT_1F(extension) ? 'A' : 'D', (extension >> 28) & 7,
                BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7);
            return false;
        }

        private static bool d68020_cas2_32(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (dasm.rdr.TryReadBeUInt32(out uint extension))
                return false;
            dasm.g_dasm_str = string.Format("cas2.l  D{0}:D%d:D%d:D%d, (%c%d):(%c%d); (2+)",
                (extension >> 16) & 7, extension & 7, (extension >> 22) & 7, (extension >> 6) & 7,
                BIT_1F(extension) ? 'A' : 'D', (extension >> 28) & 7,
                BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7);
            return false;
        }

        private static bool d68000_chk_16(M68kDisassembler dasm)
        {
            dasm.instr.code = Opcode.chk;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            dasm.instr.op1 = dasm.get_ea_mode_str_16(dasm.uInstr);
            dasm.instr.op2 = get_data_reg((dasm.uInstr >> 9) & 7);
            return true;
        }

        private static bool d68020_chk_32(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            dasm.instr.code = Opcode.chk;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word32;
            dasm.instr.op1 = dasm.get_ea_mode_str_32(dasm.uInstr);
            dasm.instr.op2 = get_data_reg((dasm.uInstr >> 9) & 7);
            return true;
        }

        private static bool d68020_chk2_cmp2_8(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            dasm.instr.code = BIT_B(extension) ? Opcode.chk2 : Opcode.cmp2;
            dasm.instr.InstructionClass = InstrClass.Invalid;
            dasm.instr.dataWidth = PrimitiveType.Byte;
            dasm.instr.op1 = dasm.get_ea_mode_str_8(dasm.uInstr);
            dasm.instr.op2 = get_addr_or_data_reg(BIT_F(extension), (int) (extension >> 12) & 7);
            return true;
        }

        private static bool d68020_chk2_cmp2_16(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            dasm.instr.code = BIT_B(extension) ? Opcode.chk2 : Opcode.cmp2;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            dasm.instr.op1 = dasm.get_ea_mode_str_16(dasm.uInstr);
            dasm.instr.op2 = get_addr_or_data_reg(BIT_F(extension), (int) (extension >> 12) & 7);
            return true;
        }

        private static bool d68020_chk2_cmp2_32(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            dasm.instr.code = BIT_B(extension) ? Opcode.chk2 : Opcode.cmp2;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word32;
            dasm.instr.op1 = dasm.get_ea_mode_str_32(dasm.uInstr);
            dasm.instr.op2 = get_addr_or_data_reg(BIT_F(extension), (int) (extension >> 12) & 7);
            return true;
        }

        private static bool d68040_cinv(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68040_PLUS);
            var cache = new RegisterStorage(
                g_caches[(dasm.uInstr >> 6) & 3],
                0,
                0,
                PrimitiveType.Word32);

            switch ((dasm.uInstr >> 3) & 3)
            {
            case 0:
                dasm.instr.InstructionClass = InstrClass.Invalid;
                dasm.instr.code = Opcode.cinv; // illegal
                break;
            case 1:
                dasm.instr.code = Opcode.cinvl;
                dasm.instr.op1 = new RegisterOperand(cache);
                dasm.instr.op2 = MemoryOperand.Indirect(PrimitiveType.Word32, Registers.AddressRegister((int) dasm.uInstr & 7));
                break;
            case 2:
                dasm.instr.code = Opcode.cinvp;
                dasm.instr.op1 = new RegisterOperand(cache);
                dasm.instr.op2 = MemoryOperand.Indirect(PrimitiveType.Word32, Registers.AddressRegister((int) dasm.uInstr & 7));
                break;
            case 3:
                dasm.instr.code = Opcode.cinva;
                dasm.instr.op1 = new RegisterOperand(cache);
                break;
            }
            return true;
        }

        private static bool d68020_cmpi_pcdi_8(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68010_PLUS);
            if (!dasm.get_imm_str_s8(out var imm))
                return false;
            dasm.instr.code = Opcode.cmpi;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = imm;
            dasm.instr.op2 = dasm.get_ea_mode_str_8(dasm.uInstr);
            return true;
        }

        private static bool d68020_cmpi_pcix_8(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68010_PLUS);
            if (!dasm.get_imm_str_s8(out var imm))
                return false;
            dasm.instr.code = Opcode.cmpi;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Byte;
            dasm.instr.op1 = imm;
            dasm.instr.op2 = dasm.get_ea_mode_str_8(dasm.uInstr);
            return true;
        }

        private static bool d68020_cmpi_pcdi_16(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68010_PLUS);
            if (!dasm.get_imm_str_s16(out var imm))
                return false;
            dasm.instr.code = Opcode.cmpi;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            dasm.instr.op1 = imm;
            dasm.instr.op2 = dasm.get_ea_mode_str_16(dasm.uInstr);
            return true;
        }

        private static bool d68020_cmpi_pcix_16(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68010_PLUS);
            if (!dasm.get_imm_str_s16(out var imm))
                return false;
            dasm.instr.code = Opcode.cmpi;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            dasm.instr.op1 = imm;
            dasm.instr.op2 = dasm.get_ea_mode_str_16(dasm.uInstr);
            return true;
        }

        private static bool d68020_cmpi_pcdi_32(M68kDisassembler dasm)
        {
            if (!dasm.get_imm_str_s32(out var imm))
                return false;
            dasm.instr.code = Opcode.cmpi;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word32;
            dasm.instr.op1 = imm;
            dasm.instr.op2 = dasm.get_ea_mode_str_32(dasm.uInstr);
            return true;
        }

        private static bool d68020_cmpi_pcix_32(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68010_PLUS);
            if (!dasm.get_imm_str_s32(out var imm))
                return false;
            dasm.instr.code = Opcode.cmpi;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word32;
            dasm.instr.op1 = imm;
            dasm.instr.op2 = dasm.get_ea_mode_str_32(dasm.uInstr);
            return true;
        }

        private static bool d68020_cpbcc_16(M68kDisassembler dasm)
        {
            var new_pc = dasm.rdr.Address;
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            Opcode opcode = g_cpcc[dasm.uInstr & 0x3f];
            if (opcode == Opcode.illegal)
                return false;
            if (!dasm.rdr.TryReadBeInt16(out var displacement))
                return false;
            dasm.instr.code = opcode;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = new M68kAddressOperand(new_pc + displacement);
            return true;
        }

        private static bool d68020_cpbcc_32(M68kDisassembler dasm)
        {
            var new_pc = dasm.rdr.Address;
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            if (!dasm.rdr.TryReadBeInt32(out int sDisplacement))
                return false;
            new_pc += sDisplacement;
            if (!dasm.get_imm_str_s16(out var imm))
                return false;

            dasm.g_dasm_str = string.Format("%db%-4s  %s; %x (extension = %x) (2-3)", (dasm.uInstr >> 9) & 7, g_cpcc[dasm.uInstr & 0x3f], imm, new_pc, extension);
            return true;
        }

        private static bool d68020_cpdbcc(M68kDisassembler dasm)
        {
            var new_pc = dasm.rdr.Address;
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension1))
                return false;
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension2))
                return false;
            if (!dasm.rdr.TryReadBeInt16(out short sDisplacement))
                return false;
            new_pc += sDisplacement;
            if (!dasm.get_imm_str_s16(out var imm))
                return false;
            dasm.g_dasm_str = string.Format("%ddb%-4s D%d,%s; %x (extension = %x) (2-3)", (dasm.uInstr >> 9) & 7, g_cpcc[extension1 & 0x3f], dasm.uInstr & 7, imm, new_pc, extension2);
            return true;
        }

        private static bool d68020_cpgen(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt32(out uint u))
                return false;
            dasm.g_dasm_str = string.Format("%dgen    %s; (2-3)", (dasm.uInstr >> 9) & 7, u);
            return false;
        }

        private static bool d68020_cprestore(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (((dasm.uInstr >> 9) & 7) == 1)
            {
                dasm.instr.code = Opcode.frestore;
                dasm.instr.op1 = dasm.get_ea_mode_str_8(dasm.uInstr);
                return true;
            }
            else
            {
                dasm.g_dasm_str = string.Format("%drestore %s; (2-3)", (dasm.uInstr >> 9) & 7, dasm.get_ea_mode_str_8(dasm.uInstr));
            }
            return false;
        }

        private static bool d68020_cpsave(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (((dasm.uInstr >> 9) & 7) == 1)
            {
                dasm.instr.code = Opcode.fsave;
                dasm.instr.op1 = dasm.get_ea_mode_str_8(dasm.uInstr);
                return true;
            }
            else
            {
                dasm.g_dasm_str = string.Format("{0}save   {1}; (2-3)", (dasm.uInstr >> 9) & 7, dasm.get_ea_mode_str_8(dasm.uInstr));
            }
            return false;
        }

        private static bool d68020_cpscc(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            // 0 - MMU
            // 1 - MC68881/2 FPU
            int cooprocessor_id = (dasm.uInstr >> 9) & 7;
            if (dasm.rdr.TryReadBeUInt16(out var extension1))
                return false;
            dasm.g_dasm_str = string.Format("{0}cpS{1}  %s; (extension = %x) (2-3)", cooprocessor_id, g_cpcc[extension1 & 0x3f], dasm.get_ea_mode_str_8(dasm.uInstr));
            return false;
        }

        private static bool d68020_cptrapcc_0(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (dasm.rdr.TryReadBeUInt16(out var extension1))
                return false;
            if (dasm.rdr.TryReadBeUInt16(out var extension2))
                return false;
            dasm.g_dasm_str = string.Format("{0}cptrap{1,4}; (extension = {2:X}) (2-3)", (dasm.uInstr >> 9) & 7, g_cpcc[extension1 & 0x3f], extension2);
            return false;
        }

        private static bool d68020_cptrapcc_16(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);

            if (!dasm.rdr.TryReadBeUInt16(out var extension1))
                return false;
            if (!dasm.rdr.TryReadBeUInt16(out var extension2))
                return false;
            if (!dasm.rdr.TryReadBeInt16(out var s))
                return false;
            dasm.g_dasm_str = string.Format("{0}trap{1,4} {2}; (extension = {3}) (2-3)", (dasm.uInstr >> 9) & 7, g_cpcc[extension1 & 0x3f], s, extension2);
            return false;
        }

        private static bool d68020_cptrapcc_32(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out var extension1))
                return false;
            if (!dasm.rdr.TryReadBeUInt16(out var extension2))
                return false;
            if (!dasm.rdr.TryReadBeUInt32(out uint u))
                return false;
            dasm.g_dasm_str = string.Format("%dtrap%-4s %s; (extension = %x) (2-3)", (dasm.uInstr >> 9) & 7, g_cpcc[extension1 & 0x3f], new M68kImmediateOperand(Constant.UInt32(u)), extension2);
            return false;
        }

        private static bool d68040_cpush(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68040_PLUS);
            switch ((dasm.uInstr >> 3) & 3)
            {
            case 0:
                dasm.g_dasm_str = string.Format("cpush (illegal scope); (4)");
                break;
            case 1:
                dasm.g_dasm_str = string.Format("cpushl  {0}, (A%d); (4)", (dasm.uInstr >> 6) & 3, dasm.uInstr & 7);
                break;
            case 2:
                dasm.g_dasm_str = string.Format("cpushp  {0}, (A%d); (4)", (dasm.uInstr >> 6) & 3, dasm.uInstr & 7);
                break;
            case 3:
                dasm.g_dasm_str = string.Format("cpusha  {0}; (4)", (dasm.uInstr >> 6) & 3);
                break;
            }
            return false;
        }

        private static bool d68000_dbcc(M68kDisassembler dasm)
        {
            Address temp_pc = dasm.rdr.Address;
            if (!dasm.rdr.TryReadBeInt16(out short sDisplacement))
                return false;
            dasm.instr.InstructionClass = InstrClass.ConditionalTransfer;
            dasm.instr.code = g_dbcc[(dasm.uInstr >> 8) & 0xf];
            dasm.instr.op1 = get_data_reg(dasm.uInstr & 7);
            dasm.instr.op2 = new M68kAddressOperand(temp_pc + sDisplacement);
            return true;
        }

        private static bool d68020_divl(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            var ea = dasm.get_ea_mode_str_32(dasm.uInstr);
            Opcode code;
            if (BIT_B(extension))
            {
                code = BIT_A(extension) ? Opcode.divs : Opcode.divsl;
            }
            else
            {
                code = BIT_A(extension) ? Opcode.divu : Opcode.divul;
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
                op2 = get_data_reg((int) dq);
                dataWidth = PrimitiveType.Int32;
            }
            else
            {
                op2 = dasm.get_double_data_reg(dr, dq);
                dataWidth = PrimitiveType.Int32;
            }

            dasm.instr.code = code;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = dataWidth;
            dasm.instr.op1 = ea;
            dasm.instr.op2 = op2;
            return true;
        }

        private static bool d68000_eori_to_ccr(M68kDisassembler dasm)
        {
            if (!dasm.get_imm_str_u8(out var imm))
                return false;

            dasm.instr.code = Opcode.eori;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = imm;
            dasm.instr.op2 = new RegisterOperand(Registers.ccr);
            return true;
        }

        private static bool d68000_eori_to_sr(M68kDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension1))
                return false;

            dasm.instr.code = Opcode.eori;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            dasm.instr.op1 = new M68kImmediateOperand(Constant.UInt16(extension1));
            dasm.instr.op2 = new RegisterOperand(Registers.sr);
            return true;
        }

        static PrimitiveType[] float_data_format = new PrimitiveType[8] 
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

        private static bool d68040_fpu(M68kDisassembler dasm)
        {
            Opcode mnemonic;
            uint src, dst_reg;
            dasm.LIMIT_CPU_TYPES(M68030_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort w2))
                return false;

            src = (uint)(w2 >> 10) & 0x7;
            dst_reg = (uint)(w2 >> 7) & 0x7;

            var instr = dasm.instr;
            // special override for FMOVECR
            if ((((w2 >> 13) & 0x7) == 2) && (((w2 >> 10) & 0x7) == 7))
            {
                instr.code = Opcode.fmovecr;
                instr.op1 = new M68kImmediateOperand(Constant.Byte((byte) (w2 & 0x7f)));
                instr.op2 = dasm.get_fp_reg((int) dst_reg);
                return true;
            }

            switch ((w2 >> 13) & 0x7)
            {
            case 0x0:
            case 0x2:
                {
                    switch (w2 & 0x7f)
                    {
                    case 0x00: mnemonic = Opcode.fmove; break;
                    case 0x01: mnemonic = Opcode.fint; break;
                    case 0x02: mnemonic = Opcode.fsinh; break;
                    case 0x03: mnemonic = Opcode.fintrz; break;
                    case 0x04: mnemonic = Opcode.fsqrt; break;
                    case 0x06: mnemonic = Opcode.flognp1; break;
                    case 0x08: mnemonic = Opcode.fetoxm1; break;
                    case 0x09: mnemonic = Opcode.ftanh1; break;
                    case 0x0a: mnemonic = Opcode.fatan; break;
                    case 0x0c: mnemonic = Opcode.fasin; break;
                    case 0x0d: mnemonic = Opcode.fatanh; break;
                    case 0x0e: mnemonic = Opcode.fsin; break;
                    case 0x0f: mnemonic = Opcode.ftan; break;
                    case 0x10: mnemonic = Opcode.fetox; break;
                    case 0x11: mnemonic = Opcode.ftwotox; break;
                    case 0x12: mnemonic = Opcode.ftentox; break;
                    case 0x14: mnemonic = Opcode.flogn; break;
                    case 0x15: mnemonic = Opcode.flog10; break;
                    case 0x16: mnemonic = Opcode.flog2; break;
                    case 0x18: mnemonic = Opcode.fabs; break;
                    case 0x19: mnemonic = Opcode.fcosh; break;
                    case 0x1a: mnemonic = Opcode.fneg; break;
                    case 0x1c: mnemonic = Opcode.facos; break;
                    case 0x1d: mnemonic = Opcode.fcos; break;
                    case 0x1e: mnemonic = Opcode.fgetexp; break;
                    case 0x1f: mnemonic = Opcode.fgetman; break;
                    case 0x20: mnemonic = Opcode.fdiv; break;
                    case 0x21: mnemonic = Opcode.fmod; break;
                    case 0x22: mnemonic = Opcode.fadd; break;
                    case 0x23: mnemonic = Opcode.fmul; break;
                    case 0x24: mnemonic = Opcode.fsgldiv; break;
                    case 0x25: mnemonic = Opcode.frem; break;
                    case 0x26: mnemonic = Opcode.fscale; break;
                    case 0x27: mnemonic = Opcode.fsglmul; break;
                    case 0x28: mnemonic = Opcode.fsub; break;
                    case 0x30:
                    case 0x31:
                    case 0x32:
                    case 0x33:
                    case 0x34:
                    case 0x35:
                    case 0x36:
                    case 0x37:
                        mnemonic = Opcode.fsincos; break;
                    case 0x38: mnemonic = Opcode.fcmp; break;
                    case 0x3a: mnemonic = Opcode.ftst; break;
                    case 0x41: mnemonic = Opcode.fssqrt; break;
                    case 0x45: mnemonic = Opcode.fdsqrt; break;
                    case 0x58: mnemonic = Opcode.fsabs; break;
                    case 0x5a: mnemonic = Opcode.fsneg; break;
                    case 0x5c: mnemonic = Opcode.fdabs; break;
                    case 0x5e: mnemonic = Opcode.fdneg; break;
                    case 0x60: mnemonic = Opcode.fsdiv; break;
                    case 0x62: mnemonic = Opcode.fsadd; break;
                    case 0x63: mnemonic = Opcode.fsmul; break;
                    case 0x64: mnemonic = Opcode.fddiv; break;
                    case 0x66: mnemonic = Opcode.fdadd; break;
                    case 0x67: mnemonic = Opcode.fdmul; break;
                    case 0x68: mnemonic = Opcode.fssub; break;
                    case 0x6c: mnemonic = Opcode.fdsub; break;

                    default: return false;
                    }

                    if ((w2 & 0x4000) != 0)
                    {
                        instr.code = mnemonic;
                        instr.dataWidth = float_data_format[src];
                        instr.op1 = dasm.get_ea_mode_str(dasm.uInstr, float_data_format[src]);
                        instr.op2 = dasm.get_fp_reg((int) dst_reg);
                    }
                    else
                    {
                        instr.code = mnemonic;
                        instr.dataWidth = PrimitiveType.Real80;
                        instr.op1 = dasm.get_fp_reg((int) src);
                        instr.op2 = dasm.get_fp_reg((int) dst_reg);
                    }
                    return true;
                }

            case 0x3:
                {
                    switch ((w2 >> 10) & 7)
                    {
                    case 3:		// packed decimal w/fixed k-factor
                        instr.code = Opcode.fmove;
                        instr.dataWidth = float_data_format[(w2 >> 10) & 7];
                        instr.op1 = dasm.get_fp_reg((int) dst_reg);
                        instr.op2 = dasm.get_ea_mode_str_32(dasm.uInstr);
                        // sext_7bit_int((int)w2 & 0x7f));
                        return true;
                    case 7:		// packed decimal w/dynamic k-factor (register)
                        dasm.g_dasm_str = string.Format("fmove{0}   FP%d, %s {{D%d}}", float_data_format[(w2 >> 10) & 7], dst_reg, dasm.get_ea_mode_str_32(dasm.uInstr), (w2 >> 4) & 7);
                        break;

                    default:
                        instr.code = Opcode.fmove;
                        instr.dataWidth = float_data_format[(w2 >> 10) & 7];
                        instr.op1 = dasm.get_fp_reg((int) dst_reg);
                        instr.op2 = dasm.get_ea_mode_str_32(dasm.uInstr);
                        return true;
                    }
                    break;
                }

            case 0x4:	// ea to control
                {
                    dasm.g_dasm_str = string.Format("fmovem.l   {0}, ", dasm.get_ea_mode_str_32(dasm.uInstr));
                    if ((w2 & 0x1000) != 0) dasm.g_dasm_str += "fpcr";
                    if ((w2 & 0x0800) != 0) dasm.g_dasm_str += "/fpsr";
                    if ((w2 & 0x0400) != 0) dasm.g_dasm_str += "/fpiar";
                    break;
                }

            case 0x5:	// control to ea
                {
                    dasm.g_dasm_str = "fmovem.l   ";
                    if ((w2 & 0x1000) != 0) dasm.g_dasm_str += "fpcr";
                    if ((w2 & 0x0800) != 0) dasm.g_dasm_str += "/fpsr";
                    if ((w2 & 0x0400) != 0) dasm.g_dasm_str += "/fpiar";
                    dasm.g_dasm_str += ", ";
                    dasm.g_dasm_str += dasm.get_ea_mode_str_32(dasm.uInstr);
                    break;
                }

            case 0x6:	// memory to FPU, list
                {
                    if (((w2 >> 11) & 1) != 0)	// dynamic register list
                    {
                        dasm.g_dasm_str = string.Format("fmovem.x   {0},D{1}", dasm.get_ea_mode_str_32(dasm.uInstr), (w2 >> 4) & 7);
                    }
                    else	// static register list
                    {
                        instr.code = Opcode.fmovem;
                        instr.dataWidth = PrimitiveType.Real96;
                        if (((w2 >> 12) & 1) == 0)
                        {
                            instr.op2 = RegisterSetOperand.CreateReversed((byte) w2, PrimitiveType.Real96);
                        }
                        else
                        {
                            instr.op2 = new RegisterSetOperand((byte) w2, PrimitiveType.Real96);
                        }
                        instr.op1 = dasm.get_ea_mode_str_32(dasm.uInstr);
                        return true;

                        //int i;

                        //dasm.g_dasm_str = string.Format("fmovem.x   {0}, ", dasm.get_ea_mode_str_32(dasm.instruction));

                        //for (i = 0; i < 8; i++)
                        //{
                        //    if ((w2 & (1 << i)) != 0)
                        //    {
                        //        if (((w2 >> 12) & 1) != 0)	// postincrement or control
                        //        {
                        //            temp = string.Format("FP{0} ", 7 - i);
                        //        }
                        //        else			// predecrement
                        //        {
                        //            temp = string.Format("FP{0} ", i);
                        //        }
                        //        dasm.g_dasm_str += temp;
                        //    }
                        //}
                    }
                    break;
                }

            case 0x7:   // FPU to memory, list
                if (((w2 >> 11) & 1) != 0)  // dynamic register list
                {
                    instr.code = Opcode.fmovem;
                    instr.dataWidth = PrimitiveType.Real96;
                    instr.op1 = new RegisterOperand(Registers.GetRegister((int) (w2 >> 4) & 7));
                    instr.op2 = dasm.get_ea_mode_str_32(dasm.uInstr);
                }
                else    // static register list
                {
                    instr.code = Opcode.fmovem;
                    instr.dataWidth = PrimitiveType.Real96;
                    if (((w2 >> 12) & 1) == 0)
                    {
                        instr.op1 = RegisterSetOperand.CreateReversed((ushort) (w2 << 8), PrimitiveType.Real96);
                    }
                    else
                    {
                        instr.op1 = new RegisterSetOperand(w2 & 0xFFu, PrimitiveType.Real96);
                    }
                    instr.op2 = dasm.get_ea_mode_str_32(dasm.uInstr);
                }
                return true;
            }
            return false;
        }

        private static bool d68000_move_8(M68kDisassembler dasm)
        {
            var ea = dasm.get_ea_mode_str_8(dasm.uInstr);
            dasm.instr.code = Opcode.move;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Byte;
            dasm.instr.op1 = ea;
            dasm.instr.op2 = dasm.get_ea_mode_str_8(((dasm.uInstr >> 9) & 7) | ((dasm.uInstr >> 3) & 0x38));
            return true;
        }

        private static bool d68000_move_16(M68kDisassembler dasm)
        {
            var str = dasm.get_ea_mode_str_16(dasm.uInstr);
            dasm.instr.code = Opcode.move;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            dasm.instr.op1 = str;
            dasm.instr.op2 = dasm.get_ea_mode_str_16(((dasm.uInstr >> 9) & 7) | ((dasm.uInstr >> 3) & 0x38));
            return true;
        }

        private static bool d68000_move_32(M68kDisassembler dasm)
        {
            var str = dasm.get_ea_mode_str_32(dasm.uInstr);
            dasm.instr.code = Opcode.move;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word32;
            dasm.instr.op1 = str;
            dasm.instr.op2 = dasm.get_ea_mode_str_32(((dasm.uInstr >> 9) & 7) | ((dasm.uInstr >> 3) & 0x38));
            return true;
        }

        private static bool d68010_move_fr_ccr(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68010_PLUS);
            dasm.instr.code = Opcode.move;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = new RegisterOperand(Registers.ccr);
            dasm.instr.op2 = dasm.get_ea_mode_str_8(dasm.uInstr);
            return true;
        }

        private static bool d68000_move_fr_sr(M68kDisassembler dasm)
        {
            dasm.instr.code = Opcode.move;
            dasm.instr.InstructionClass = InstrClass.System;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            dasm.instr.op1 = new RegisterOperand(Registers.sr);
            dasm.instr.op2 = dasm.get_ea_mode_str_16(dasm.uInstr);
            return true;
        }

        private static bool d68000_move_to_sr(M68kDisassembler dasm)
        {
            dasm.instr.code = Opcode.move;
            dasm.instr.InstructionClass = InstrClass.System;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            dasm.instr.op1 = dasm.get_ea_mode_str_16(dasm.uInstr);
            dasm.instr.op2 = new RegisterOperand(Registers.sr);
            return true;
        }

        private static bool d68000_move_fr_usp(M68kDisassembler dasm)
        {
            dasm.instr.code = Opcode.move;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = new RegisterOperand(Registers.usp);
            dasm.instr.op2 = get_addr_reg(dasm.uInstr & 7);
            return true;
        }

        private static bool d68000_move_to_usp(M68kDisassembler dasm)
        {
            dasm.instr.code = Opcode.move;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = get_addr_reg(dasm.uInstr & 7);
            dasm.instr.op2 = new RegisterOperand(Registers.usp);
            return true;
        }

        private static bool d68010_movec(M68kDisassembler dasm)
        {
            MachineOperand reg_name;
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;

            int regNumber = (int)extension & 0xfff;
            switch (regNumber)
            {
            case 0x000:
                reg_name = dasm.get_ctrl_reg("SFC", regNumber);
                break;
            case 0x001:
                reg_name = dasm.get_ctrl_reg("DFC", regNumber);
                break;
            case 0x800:
                reg_name = dasm.get_ctrl_reg("USP", regNumber);
                break;
            case 0x801:
                reg_name = dasm.get_ctrl_reg("VBR", regNumber);
                break;
            case 0x002:
                reg_name = dasm.get_ctrl_reg("CACR", regNumber);
                break;
            case 0x802:
                reg_name = dasm.get_ctrl_reg("CAAR", regNumber);
                break;
            case 0x803:
                reg_name = dasm.get_ctrl_reg("MSP", regNumber);
                break;
            case 0x804:
                reg_name = dasm.get_ctrl_reg("ISP", regNumber);
                break;
            case 0x003:
                reg_name = dasm.get_ctrl_reg("TC", regNumber);
                break;
            case 0x004:
                reg_name = dasm.get_ctrl_reg("ITT0", regNumber);
                break;
            case 0x005:
                reg_name = dasm.get_ctrl_reg("ITT1", regNumber);
                break;
            case 0x006:
                reg_name = dasm.get_ctrl_reg("DTT0", regNumber);
                break;
            case 0x007:
                reg_name = dasm.get_ctrl_reg("DTT1", regNumber);
                break;
            case 0x805:
                reg_name = dasm.get_ctrl_reg("MMUSR", regNumber);
                break;
            case 0x806:
                reg_name = dasm.get_ctrl_reg("URP", regNumber);
                break;
            case 0x807:
                reg_name = dasm.get_ctrl_reg("SRP", regNumber);
                break;
            default:
                reg_name = new M68kImmediateOperand(Constant.Int16((short)(extension & 0xfff)));
                break;
            }

            var other_reg = BIT_F(extension)
                ? get_addr_reg((int)(extension >> 12) & 7)
                : get_data_reg((int)(extension >> 12) & 7);
            dasm.instr.code = Opcode.movec;
            dasm.instr.InstructionClass = InstrClass.Linear;
            if (BIT_0(dasm.uInstr))
            {
                dasm.instr.op1 = other_reg;
                dasm.instr.op2 = reg_name;
            }
            else
            {
                dasm.instr.op1 = reg_name;
                dasm.instr.op2 = other_reg;
            }
            return true;
        }

        private static bool d68000_movem_pd_16(M68kDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out ushort data))
                return false;
            StringBuilder buffer = new StringBuilder();
            int first;
            uint run_length;

            for (int i = 0; i < 8; i++)
            {
                if ((data & (1 << (15 - i))) != 0)
                {
                    first = i;
                    run_length = 0;
                    while (i < 7 && (data & (1 << (15 - (i + 1)))) != 0)
                    {
                        i++;
                        run_length++;
                    }
                    if (buffer.Length > 0)
                        buffer.Append("/");
                    buffer.AppendFormat("D{0}", first);
                    if (run_length > 0)
                        buffer.AppendFormat("-D{0}", first + run_length);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if ((data & (1 << (7 - i))) != 0)
                {
                    first = i;
                    run_length = 0;
                    while (i < 7 && (data & (1 << (7 - (i + 1)))) != 0)
                    {
                        i++;
                        run_length++;
                    }
                    if (buffer.Length > 0)
                        buffer.Append("/");
                    buffer.AppendFormat("A{0}", first);
                    if (run_length > 0)
                        buffer.AppendFormat("-A{0}", first + run_length);
                }
            }
            dasm.g_dasm_str = string.Format("movem.w {0},{1}", buffer, dasm.get_ea_mode_str_16(dasm.uInstr));
            return false;
        }

        public void WriteRegisterSet(uint data, int bitPos, int incr, string regType, StringBuilder buffer)
        {
            string sep = "";
            for (int i = 0; i < 8; i++, bitPos += incr)
            {
                if (Bits.IsBitSet(data, bitPos))
                {
                    int first = i;
                    int run_length = 0;
                    while (i < 7 && Bits.IsBitSet(data, bitPos+incr))
                    {
                        bitPos += incr;
                        ++i;
                        ++run_length;
                    }
                    buffer.Append(sep);
                    buffer.AppendFormat("{0}{1}", regType, first);
                    if (run_length > 0)
                        buffer.AppendFormat("-{0}{0}", regType, first + run_length);
                    sep = "/";
                }
            }
        }

        private static bool d68010_moves_8(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68010_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            var reg = get_addr_or_data_reg(BIT_F(extension), (int) (extension >> 12) & 7);
            var ea = dasm.get_ea_mode_str_8(dasm.uInstr);
            if (BIT_B(extension))
            {
                dasm.instr.code = Opcode.moves;
                dasm.instr.InstructionClass = InstrClass.System;
                dasm.instr.dataWidth = PrimitiveType.Word16;
                dasm.instr.op1 = reg;
                dasm.instr.op2 = ea;
            }
            else
            {
                dasm.instr.code = Opcode.moves;
                dasm.instr.InstructionClass = InstrClass.System;
                dasm.instr.dataWidth = PrimitiveType.Word16;
                dasm.instr.op1 = ea;
                dasm.instr.op2 = reg;
            }
            return true;
        }

        private static bool d68010_moves_16(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68010_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            var reg = get_addr_or_data_reg(BIT_F(extension), (int) (extension >> 12) & 7);
            var ea = dasm.get_ea_mode_str_16(dasm.uInstr);
            if (BIT_B(extension))
            {
                dasm.instr.code = Opcode.moves;
                dasm.instr.InstructionClass = InstrClass.System;
                dasm.instr.dataWidth = PrimitiveType.Word16;
                dasm.instr.op1 = reg;
                dasm.instr.op2 = ea;
            }
            else
            {
                dasm.instr.code = Opcode.moves;
                dasm.instr.InstructionClass = InstrClass.System;
                dasm.instr.dataWidth = PrimitiveType.Word16;
                dasm.instr.op1 = ea;
                dasm.instr.op2 = reg;
            }
            return true;
        }

        private static bool d68010_moves_32(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68010_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort extension))
                return false;
            var reg = get_addr_or_data_reg(BIT_F(extension), (int)(extension >> 12) & 7);
            var ea = dasm.get_ea_mode_str_32(dasm.uInstr);
            dasm.instr.code = Opcode.moves;
            dasm.instr.InstructionClass = InstrClass.System;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            if (BIT_B(extension))
            {
                dasm.instr.op1 = reg;
                dasm.instr.op2 = ea;
            }
            else
            {
                dasm.instr.op1 = ea;
                dasm.instr.op2 = reg;
            };
            return true;
        }

        private static bool d68040_move16_pi_pi(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68040_PLUS);
            if (dasm.rdr.TryReadBeUInt16(out ushort us))
                return false;
            dasm.instr.code = Opcode.move16;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = dasm.get_post_inc(dasm.uInstr & 7);
            dasm.instr.op2 = dasm.get_post_inc((us >> 12) & 7);
            return true;
        }

        private static bool d68040_move16_pi_al(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68040_PLUS);
            if (!dasm.rdr.TryReadBeUInt32(out uint uOp2))
                return false;
            dasm.instr.code = Opcode.move16;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = dasm.get_post_inc(dasm.uInstr & 7);
            dasm.instr.op2 = new M68kImmediateOperand(Constant.UInt32(uOp2));
            return true;
        }

        private static bool d68040_move16_al_pi(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68040_PLUS);
            if (!dasm.rdr.TryReadBeUInt32(out uint uOp1))
                return false;
            dasm.instr.code = Opcode.move16;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = new M68kImmediateOperand(Constant.UInt32(uOp1));
            dasm.instr.op2 = dasm.get_post_inc(dasm.uInstr & 7);
            return true;
        }

        private static bool d68040_move16_ai_al(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68040_PLUS);
            if (!dasm.rdr.TryReadBeUInt32(out uint uOp2))
                return false;
            var instr = dasm.instr;
            instr.code = Opcode.move16;
            instr.op1 = new MemoryOperand(dasm.instr.dataWidth, Registers.AddressRegister(dasm.uInstr & 7));
            instr.op2 = new M68kImmediateOperand(Constant.UInt32(uOp2));
            return true;
        }

        private static bool d68040_move16_al_ai(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68040_PLUS);
            if (!dasm.rdr.TryReadBeUInt32(out uint uOp1))
                return false;

            var instr = dasm.instr;
            instr.code = Opcode.move16;
            instr.op1 = new M68kImmediateOperand(Constant.UInt32(uOp1));
            instr.op2 = new MemoryOperand(dasm.instr.dataWidth, Registers.AddressRegister(dasm.uInstr & 7));
            return true;
        }

        private static bool d68020_mull(M68kDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out var extension))
                return false;

            MachineOperand op2 = BIT_A(extension)
                ? dasm.get_double_data_reg(extension & 7u, (uint)(extension >> 12) & 7u)
                : (MachineOperand) get_data_reg((int)(extension >> 12) & 7);
            var opDecoder = new OperandFormatDecoder(dasm, 0);
            dasm.instr.code = BIT_B(extension) ? Opcode.muls : Opcode.mulu;
            dasm.instr.dataWidth = PrimitiveType.Word32;

            if (!opDecoder.TryParseOperand(dasm.uInstr, 0, PrimitiveType.Word32, dasm.rdr, out dasm.instr.op1))
            {
                return false;
            }
            dasm.instr.op2 = op2;
            return true;
        }

        private static bool d68000_nbcd(M68kDisassembler dasm)
        {
            dasm.instr.code = Opcode.nbcd;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = dasm.get_ea_mode_str_8(dasm.uInstr);
            return true;
        }

        private static bool d68020_pack_rr(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort uImm))
                return false;
            dasm.instr.code = Opcode.pack;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = get_data_reg(dasm.uInstr & 7);
            dasm.instr.op2 = get_data_reg((dasm.uInstr >> 9) & 7);
            dasm.instr.op3 = new M68kImmediateOperand(Constant.UInt16(uImm));
            return true;
        }

        private static bool d68020_pack_mm(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort uImm))
                return false;
            dasm.instr.code = Opcode.pack;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = dasm.get_pre_dec(dasm.uInstr & 7);
            dasm.instr.op2 = dasm.get_pre_dec((dasm.uInstr >> 9) & 7);
            dasm.instr.op3 = new M68kImmediateOperand(Constant.UInt16(uImm));
            return true;
        }

        // this is a 68040-specific form of PFLUSH
        private static bool d68040_pflush(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68040_PLUS);

            if ((dasm.uInstr & 0x10) != 0)
            {
                dasm.g_dasm_str = string.Format("pflusha{0}", (dasm.uInstr & 8)!=0 ? "" : "n");
            }
            else
            {
                dasm.g_dasm_str = string.Format("pflush{0}(A%d)", (dasm.uInstr & 8)!=0 ? "" : "n", dasm.uInstr & 7);
            }
            return false;
        }

        private static bool d68020_rtm(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_ONLY);
            int reg = dasm.uInstr & 7;
            dasm.instr.code = Opcode.rtm;
            dasm.instr.InstructionClass = InstrClass.Transfer | InstrClass.Call;
            dasm.instr.op1 = BIT_3(dasm.uInstr)
                ? get_addr_reg(reg)
                : get_data_reg(reg);
            return true;
        }

        private static bool d68000_scc(M68kDisassembler dasm)
        {
            dasm.instr.code = g_scc[(dasm.uInstr >> 8) & 0xf];
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = dasm.get_ea_mode_str_8(dasm.uInstr);
            return true;
        }

        private static bool d68000_stop(M68kDisassembler dasm)
        {
            if (!dasm.get_imm_str_s16(out var imm))
                return false;
            dasm.instr.code = Opcode.stop;
            dasm.instr.InstructionClass = InstrClass.System;
            dasm.instr.op1 = imm;
            return true;
        }

        private static bool d68000_tas(M68kDisassembler dasm)
        {
            dasm.instr.code = Opcode.tas;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = dasm.get_ea_mode_str_8(dasm.uInstr);
            return true;
        }

        private static bool d68000_trap(M68kDisassembler dasm)
        {
            dasm.instr.code = Opcode.trap;
            dasm.instr.InstructionClass = InstrClass.Call | InstrClass.Transfer;
            dasm.instr.op1 = new M68kImmediateOperand(Constant.Byte((byte) (dasm.uInstr & 0xf)));
            return true;
        }

        private static bool d68020_trapcc_0(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            dasm.instr.code = g_trapcc[(dasm.uInstr >> 8) & 0xf];
            dasm.instr.InstructionClass = dasm.instr.code != Opcode.trapf
                ? InstrClass.Call | InstrClass.Transfer
                : InstrClass.Linear;
            return true;
        }

        private static bool d68020_trapcc_16(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort uImm))
                return false;

            dasm.instr.code = g_trapcc[(dasm.uInstr >> 8) & 0xf];
            dasm.instr.InstructionClass = InstrClass.Call | InstrClass.Transfer;
            dasm.instr.op1 = new M68kImmediateOperand(Constant.UInt16(uImm));
            return true;
        }

        private static bool d68020_trapcc_32(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt32(out uint uOp1))
                return false;

            dasm.instr.code = g_trapcc[(dasm.uInstr >> 8) & 0xf];
            dasm.instr.InstructionClass = InstrClass.Call | InstrClass.Transfer;
            dasm.instr.op1 = new M68kImmediateOperand(Constant.UInt32(uOp1));
            return true;
        }

        private static bool d68020_tst_pcdi_8(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            dasm.instr.code = Opcode.tst;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Byte;
            dasm.instr.op1 = dasm.get_ea_mode_str_8(dasm.uInstr);
            return true;
        }

        private static bool d68020_tst_pcix_8(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            dasm.instr.code = Opcode.tst;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Byte;
            dasm.instr.op1 = dasm.get_ea_mode_str_8(dasm.uInstr);
            return true;
        }

        private static bool d68020_tst_i_8(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            dasm.instr.code = Opcode.tst;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Byte;
            dasm.instr.op1 = dasm.get_ea_mode_str_8(dasm.uInstr);
            return true;
        }

        private static bool d68020_tst_a_16(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            dasm.instr.code = Opcode.tst;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            dasm.instr.op1 = dasm.get_ea_mode_str_16(dasm.uInstr);
            return true;
        }

        private static bool d68020_tst_pcdi_16(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            dasm.instr.code = Opcode.tst;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            dasm.instr.op1 = dasm.get_ea_mode_str_16(dasm.uInstr);
            return true;
        }

        private static bool d68020_tst_pcix_16(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            dasm.instr.code = Opcode.tst;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            dasm.instr.op1 = dasm.get_ea_mode_str_16(dasm.uInstr);
            return true;
        }

        private static bool d68020_tst_i_16(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            dasm.instr.code = Opcode.tst;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word16;
            dasm.instr.op1 = dasm.get_ea_mode_str_16(dasm.uInstr);
            return true;
        }

        private static bool d68020_tst_a_32(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            dasm.instr.code = Opcode.tst;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word32;
            dasm.instr.op1 = dasm.get_ea_mode_str_32(dasm.uInstr);
            return true;
        }

        private static bool d68020_tst_pcdi_32(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            dasm.instr.code = Opcode.tst;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word32;
            dasm.instr.op1 = dasm.get_ea_mode_str_32(dasm.uInstr);
            return true;
        }

        private static bool d68020_tst_pcix_32(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            dasm.instr.code = Opcode.tst;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.dataWidth = PrimitiveType.Word32;
            dasm.instr.op1 = dasm.get_ea_mode_str_32(dasm.uInstr);
            return true;
        }

        private static bool d68020_unpk_rr(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort uImm))
                return false;
            dasm.instr.code = Opcode.unpk;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = get_data_reg(dasm.uInstr & 7);
            dasm.instr.op2 = get_data_reg((dasm.uInstr >> 9) & 7);
            dasm.instr.op3 = new M68kImmediateOperand(Constant.UInt16(uImm));
            return true;
        }

        private static bool d68020_unpk_mm(M68kDisassembler dasm)
        {
            dasm.LIMIT_CPU_TYPES(M68020_PLUS);
            if (!dasm.rdr.TryReadBeUInt16(out ushort uImm))
                return false;
            dasm.instr.code = Opcode.unpk;
            dasm.instr.InstructionClass = InstrClass.Linear;
            dasm.instr.op1 = dasm.get_pre_dec(dasm.uInstr & 7);
            dasm.instr.op2 = dasm.get_pre_dec((dasm.uInstr >> 9) & 7);
            dasm.instr.op3 = new M68kImmediateOperand(Constant.UInt16(uImm));
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
        private static bool d68851_p000(M68kDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out ushort modes))
                return false;

            // do this after fetching the second PMOVE word so we properly get the 3rd if necessary
            var str = dasm.get_ea_mode_str_32(dasm.uInstr);

            if ((modes & 0xfde0) == 0x2000)	// PLOAD
            {
                if ((modes & 0x0200) != 0)
                {
                    dasm.instr.code = Opcode.pload;
                    dasm.instr.InstructionClass = InstrClass.Linear;
                    dasm.instr.op1 = new M68kImmediateOperand(Constant.Byte((byte) ((modes >> 10) & 7)));
                    dasm.instr.op2 = str;
                    return true;
                }
                else
                {
                    dasm.instr.code = Opcode.pload;
                    dasm.instr.InstructionClass = InstrClass.Linear;
                    dasm.instr.op1 = str;
                    dasm.instr.op2 = new M68kImmediateOperand(Constant.Byte((byte) ((modes >> 10) & 7)));
                    return true;
                }
            }

            if ((modes & 0xe200) == 0x2000)	// PFLUSH
            {
                dasm.instr.code = Opcode.pflushr;
                dasm.instr.InstructionClass = InstrClass.System;
                dasm.instr.op1 = new M68kImmediateOperand(Constant.Byte((byte) (modes & 0x1f)));
                dasm.instr.op2 = new M68kImmediateOperand(Constant.Byte((byte) ((modes >> 5) & 0xf)));
                dasm.instr.op3 = str;
                return true;
            }

            if (modes == 0xa000)	// PFLUSHR
            {
                dasm.instr.code = Opcode.pflushr;
                dasm.instr.InstructionClass = InstrClass.System;
                dasm.instr.op1 = str;
                return true;
            }

            if (modes == 0x2800)	// PVALID (FORMAT 1)
            {
                dasm.instr.code = Opcode.pvalid;
                dasm.instr.InstructionClass = InstrClass.Linear;
                dasm.instr.op1 = dasm.get_ctrl_reg("VAL", 0x2800);
                dasm.instr.op2 = str;
                return true;
            }

            if ((modes & 0xfff8) == 0x2c00)	// PVALID (FORMAT 2)
            {
                dasm.instr.code = Opcode.pvalid;
                dasm.instr.InstructionClass = InstrClass.Linear;
                dasm.instr.op1 = get_addr_reg(modes & 0xf);
                dasm.instr.op2 = str;
                return true;
            }

            if ((modes & 0xe000) == 0x8000)	// PTEST
            {
                dasm.instr.code = Opcode.ptest;
                dasm.instr.InstructionClass = InstrClass.System;
                dasm.instr.op1 = new M68kImmediateOperand(Constant.Byte((byte) (modes & 0x1f)));
                dasm.instr.op2 = str;
                return true;
            }

            switch ((modes >> 13) & 0x7)
            {
            case 0:	// MC68030/040 form with FD bit
            case 2:	// MC68881 form, FD never set
                if ((modes & 0x0100)!=0)
                {
                    if ((modes & 0x0200)!=0)
                    {
                        dasm.g_dasm_str = string.Format("pmovefd  {0},{1}", g_mmuregs[(modes >> 10) & 7], str);
                    }
                    else
                    {
                        dasm.g_dasm_str = string.Format("pmovefd  {0},{1}", str, g_mmuregs[(modes >> 10) & 7]);
                    }
                }
                else
                {
                    if ((modes & 0x0200)!=0)
                    {
                        dasm.g_dasm_str = string.Format("pmove  {0},{1}", g_mmuregs[(modes >> 10) & 7], str);
                    }
                    else
                    {
                        dasm.g_dasm_str = string.Format("pmove  {0},{1}", str, g_mmuregs[(modes >> 10) & 7]);
                    }
                }
                break;

            case 3:	// MC68030 to/from status reg
                if ((modes & 0x0200)!=0)
                {
                    dasm.g_dasm_str = string.Format("pmove  mmusr, {0}", str);
                }
                else
                {
                    dasm.g_dasm_str = string.Format("pmove  {0}, mmusr", str);
                }
                break;

            default:
                dasm.g_dasm_str = string.Format("pmove [unknown form] {0}", str);
                break;
            }
            return false;
        }

        private static bool d68851_pbcc16(M68kDisassembler dasm)
        {
            var temp_pc = dasm.rdr.Address;
            if (!dasm.rdr.TryReadBeInt16(out short sDisplacement))
                return false;
            dasm.instr.code = g_mmucond[dasm.uInstr & 0xf];
            dasm.instr.op1 = AddressOperand.Create(temp_pc + sDisplacement);
            return true;
        }

        private static bool d68851_pbcc32(M68kDisassembler dasm)
        {
            var temp_pc = dasm.rdr.Address;
            if (!dasm.rdr.TryReadBeInt16(out short sDisplacement))
                return false;
            dasm.g_dasm_str = string.Format("pb{0} %x", g_mmucond[dasm.uInstr & 0xf], temp_pc + sDisplacement);
            return false;
        }

        private static bool d68851_pdbcc(M68kDisassembler dasm)
        {
            var temp_pc = dasm.rdr.Address;
            if (!dasm.rdr.TryReadBeUInt16(out ushort modes))
                return false;
            if (!dasm.rdr.TryReadBeInt16(out short displacement))
                return false;
            dasm.g_dasm_str = string.Format("pb{0} %x", g_mmucond[modes & 0xf], temp_pc + displacement);
            return false;
        }

        // PScc:  0000000000xxxxxx
        private static bool d68851_p001(M68kDisassembler dasm)
        {
            //dasm.g_dasm_str = string.Format("MMU 001 group");
            return false;
        }

        #region Mutators

        private static Mutator s(int bitOffset)
        {
            return d =>
            {
                switch ((d.uInstr >> bitOffset) & 3)
                {
                case 0: d.instr.dataWidth = d.dataWidth = PrimitiveType.Byte; return true;
                case 1: d.instr.dataWidth = d.dataWidth = PrimitiveType.Word16; return true;
                case 2: d.instr.dataWidth = d.dataWidth = PrimitiveType.Word32; return true;
                default: return false;
                }
            };
        }

        private static Mutator s6 = s(6);
        private static bool sb(M68kDisassembler d) { d.instr.dataWidth = d.dataWidth = PrimitiveType.Byte; return true; }
        private static bool sw(M68kDisassembler d) { d.instr.dataWidth = d.dataWidth = PrimitiveType.Word16; return true; }
        private static bool sl(M68kDisassembler d) { d.instr.dataWidth = d.dataWidth = PrimitiveType.Word32; return true; }
        private static bool su(M68kDisassembler d) { d.instr.dataWidth = d.dataWidth = PrimitiveType.UInt16; return true; }
        private static bool sr(M68kDisassembler d)
        {
            // If EA is register, 32 bits, else 8.
            var dt = ((d.uInstr & 0x30) == 0)
                ? PrimitiveType.Word32
                : PrimitiveType.Byte;
            d.instr.dataWidth = d.dataWidth = dt;
            return true;
        }

        // Data register D0-D7 encoded in instruction
        private static Mutator D(int bitOffset)
        {
            return d =>
            {
                d.ops.Add(new RegisterOperand(Registers.DataRegister((d.uInstr >> bitOffset) & 0x7)));
                return true;
            };
        }

        private static readonly Mutator D0 = D(0);
        private static readonly Mutator D9 = D(9);
        private static bool ccr(M68kDisassembler d) { d.ops.Add(new RegisterOperand(Registers.ccr)); return true; }
        private static bool SR(M68kDisassembler d) { d.ops.Add(new RegisterOperand(Registers.sr)); return true; }

        // Address register A0-A7 encoded in in instrution
        private static Mutator A(int bitOffset)
        {
            return d =>
            {
                d.ops.Add(new RegisterOperand(Registers.AddressRegister((d.uInstr >> bitOffset) & 0x7)));
                return true;
            };
        }

        private static readonly Mutator A0 = A(0);
        private static readonly Mutator A9 = A(9);

        // Address register with 16-bit displacement

        private static Mutator Ad(int bitOffset)
        {
            return d =>
            {
                if (!d.rdr.TryReadBeInt16(out short sDisplacement))
                    return false;
                var aReg = Registers.AddressRegister((d.uInstr >> bitOffset) & 0x7);
                d.ops.Add(new MemoryOperand(
                    PrimitiveType.Word16,
                    Registers.AddressRegister(d.uInstr & 7),
                    Constant.Int16(sDisplacement)));
                return true;
            };
        }

        private static readonly Mutator Ad0 = Ad(0);

        // Effective address (EA) 
        private static Mutator E(int bitOffset)
        {
            return d =>
            {
                var opcode = (uint)d.uInstr >> bitOffset;
                var op = d.get_ea_mode_str(opcode, d.dataWidth);
                if (op == null)
                    return false;
                d.ops.Add(op);
                return true;
            };
        }

        // Effective address with 3-bit halves swapped
        private static Mutator e(int bitOffset)
        {
            return d =>
            {
                var opcode = (uint) d.uInstr >> bitOffset;
                opcode = ((opcode >> 3) & 0x07u) | ((opcode & 0x7u) << 3);
                var op = d.get_ea_mode_str(opcode, d.dataWidth);
                if (op == null)
                    return false;
                d.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator E0 = E(0);
        //private static readonly Mutator e0 = e(0);
        private static readonly Mutator e6 = e(6);

        // Immediate operand

        private static bool Ib(M68kDisassembler d)
        {
            if (!d.rdr.IsValidOffset(d.rdr.Offset + 1))
            {
                return false;
            }
            d.rdr.Offset += 1;    // skip a byte so we get the appropriate lsb byte and align the word stream.
            if (!d.rdr.TryRead(PrimitiveType.Byte, out var c))
                return false;
            d.ops.Add(new M68kImmediateOperand(c));
            return true;
        }

        private static bool Iw(M68kDisassembler d)
        {
            if (!d.rdr.TryRead(PrimitiveType.Word16, out var c))
                return false;
            d.ops.Add(new M68kImmediateOperand(c));
            return true;
        }

        private static bool Il(M68kDisassembler d)
        {
            if (!d.rdr.TryRead(PrimitiveType.Word32, out var c))
                return false;
            d.ops.Add(new M68kImmediateOperand(c));
            return true;
        }

        private static bool Iv(M68kDisassembler d)
        {
            if (d.dataWidth.Size == 1)
                return Ib(d);
            if (!d.rdr.TryRead(d.dataWidth, out var c))
                return false;
            d.ops.Add(new M68kImmediateOperand(c));
            return true;
        }

        // Quick constant (3-bit part of the opcode)
        private static Mutator Q(int bitOffset, int mask, int zeroValue, PrimitiveType dt)
        {
            return d =>
            {
                int v = ((int) d.uInstr >> bitOffset) & mask;
                if (v == 0)
                    v = zeroValue;
                d.ops.Add(new M68kImmediateOperand(Constant.Create(dt, v)));
                return true;
            };
        }

        private static Mutator q9 = Q(9, 0x07, 8, PrimitiveType.Byte);
        private static Mutator Q0 = Q(0, 0xFF, 0, PrimitiveType.SByte);

        // Predecrement operator
        private static Mutator Pre(int bitOffset)
        {
            return d =>
            {
                d.ops.Add(new PredecrementMemoryOperand(
                    d.dataWidth,
                    Registers.AddressRegister((d.uInstr >> bitOffset) & 0x7)));
                return true;
            };
        }

        private static readonly Mutator Pre0 = Pre(0);
        private static readonly Mutator Pre9 = Pre(9);


        // Postdecrement operator
        private static Mutator Post(int bitOffset)
        {
            return d =>
            {
                d.ops.Add(new PostIncrementMemoryOperand(
                    d.dataWidth,
                    Registers.AddressRegister((d.uInstr >> bitOffset) & 0x7)));
                return true;
            };
        }

        private static readonly Mutator Post0 = Post(0);
        private static readonly Mutator Post9 = Post(9);


        // PC Relative jump 
        private static bool J(M68kDisassembler dasm)
        {
            var addr = dasm.rdr.Address;
            int offset = dasm.uInstr & 0xFF;
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
            dasm.ops.Add(new M68kAddressOperand(addr + offset));
            return true;
        }


        // relative
        private static bool Rw(M68kDisassembler dasm) {
            var addr = dasm.rdr.Address;
            if (!dasm.rdr.TryReadBeInt16(out short relative))
                return false;
            dasm.ops.Add(new M68kAddressOperand(addr + relative));
            return true;
        }

        private static bool Rl(M68kDisassembler dasm)
        {
            var addr = dasm.rdr.Address;
            if (!dasm.rdr.TryReadBeInt32(out int relative))
                return false;
            dasm.ops.Add(new M68kAddressOperand(addr + relative));
            return true;
        }


        // Register bitset
        private static Mutator M(PrimitiveType size)
        {
            return d =>
            {
                //var size = GetSizeType(0, args[i++], dataWidth);
                if (!d.rdr.TryReadBeUInt16(out ushort memSet))
                {
                    return false;
                }
                d.ops.Add(new RegisterSetOperand(memSet, size));
                return true;
            };
        }


        private static bool n(M68kDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out dasm.bitSet))
            {
                return false;
            }
            return true;
        }

        // Register bitset reversed
        private static Mutator m(PrimitiveType size)
        {
            return d =>
            {
                d.ops.Add(RegisterSetOperand.CreateReversed(d.bitSet, size));
                return true;
            };
        }

        private static readonly Mutator Mw = M(PrimitiveType.Word16);
        private static readonly Mutator Ml = M(PrimitiveType.Word32);
        private static readonly Mutator mw = m(PrimitiveType.Word16);
        private static readonly Mutator ml = m(PrimitiveType.Word32);

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

        static Decoder[] g_opcode_info;

        /// <summary>
        /// Generates the table of opcode decoders. Should only be called once
        /// per execution, as the table is expensive to build. Fortunately,
        /// 
        /// Decoders have no mutable state, so the table is reused for all
        /// disassembler instances.
        /// </summary>
        private static void GenTable()
        {
            g_opcode_info = new Decoder[]
           {
//  opcode handler             mask    match   ea mask 
	Instr(d68000_1010         , 0xf000, 0xa000, 0x000),
    Instr(0xf000, 0xf000, 0x000, Opcode.illegal, InstrClass.Invalid),  // d68000_1111
	Instr(D0,D9, 0xf1f8, 0xc100, 0x000, Opcode.abcd),             // d68000_abcd_rr
	Instr(Pre0,Pre9, 0xf1f8, 0xc108, 0x000, Opcode.abcd),             // d68000_abcd_mm
	Instr(s6,E0,D9, 0xf1c0, 0xd000, 0xbff, Opcode.add),           // d68000_add_er_8
	Instr(sw,E0,D9, 0xf1c0, 0xd040, 0xfff, Opcode.add),           // d68000_add_er_16
	Instr(sl,E0,D9, 0xf1c0, 0xd080, 0xfff, Opcode.add),           // d68000_add_er_32
	Instr(s6,D9,E0, 0xf1c0, 0xd100, 0x3f8, Opcode.add),           // d68000_add_re_8
	Instr(sw,D9,E0, 0xf1c0, 0xd140, 0x3f8, Opcode.add),           // d68000_add_re_16
	Instr(sl,D9,E0, 0xf1c0, 0xd180, 0x3f8, Opcode.add),           // d68000_add_re_32
	Instr(sw,E0,A9, 0xf1c0, 0xd0c0, 0xfff, Opcode.adda),          // d68000_adda_16
	Instr(sl,E0,A9, 0xf1c0, 0xd1c0, 0xfff, Opcode.adda),          // d68000_adda_32
	Instr(sb,Ib,E0, 0xffc0, 0x0600, 0xbf8, Opcode.addi),          // d68000_addi_8
	Instr(sw,Iw,E0, 0xffc0, 0x0640, 0xbf8, Opcode.addi),          // d68000_addi_16
	Instr(sl,Il,E0, 0xffc0, 0x0680, 0xbf8, Opcode.addi),          // d68000_addi_32
	Instr(s6,q9,E0, 0xf1c0, 0x5000, 0xbf8, Opcode.addq),          // d68000_addq_8 
	Instr(s6,q9,E0, 0xf1c0, 0x5040, 0xff8, Opcode.addq),          // d68000_addq_16
	Instr(s6,q9,E0, 0xf1c0, 0x5080, 0xff8, Opcode.addq),          // d68000_addq_32
	Instr(sb,D0,D9, 0xf1f8, 0xd100, 0x000, Opcode.addx),          // d68000_addx_rr_8    
	Instr(sw,D0,D9, 0xf1f8, 0xd140, 0x000, Opcode.addx),          // d68000_addx_rr_16   
	Instr(sl,D0,D9, 0xf1f8, 0xd180, 0x000, Opcode.addx),          // d68000_addx_rr_32   
	Instr(sb,Pre0,Pre9, 0xf1f8, 0xd108, 0x000, Opcode.addx),          // d68000_addx_mm_8    
	Instr(sw,Pre0,Pre9, 0xf1f8, 0xd148, 0x000, Opcode.addx),          // d68000_addx_mm_16   
	Instr(sl,Pre0,Pre9, 0xf1f8, 0xd188, 0x000, Opcode.addx),          // d68000_addx_mm_32   
	Instr(sb,E0,D9, 0xf1c0, 0xc000, 0xbff, Opcode.and),           // d68000_and_er_8
	Instr(sw,E0,D9, 0xf1c0, 0xc040, 0xbff, Opcode.and),           // d68000_and_er_16
	Instr(sl,E0,D9, 0xf1c0, 0xc080, 0xbff, Opcode.and),           // d68000_and_er_32
	Instr(sb,D9,E0, 0xf1c0, 0xc100, 0x3f8, Opcode.and),           // d68000_and_re_8
	Instr(sw,D9,E0, 0xf1c0, 0xc140, 0x3f8, Opcode.and),           // d68000_and_re_16
	Instr(sl,D9,E0, 0xf1c0, 0xc180, 0x3f8, Opcode.and),           // d68000_and_re_32
	Instr(Iw,ccr,   0xffff, 0x023c, 0x000, Opcode.andi),          // d68000_andi_to_ccr
	Instr(Iw,SR,    0xffff, 0x027c, 0x000, Opcode.andi),          // d68000_andi_to_sr
	Instr(sb,Ib,E0, 0xffc0, 0x0200, 0xbf8, Opcode.andi),          // d68000_andi_8
	Instr(sw,Iw,E0, 0xffc0, 0x0240, 0xbf8, Opcode.andi),          // d68000_andi_16
	Instr(sl,Il,E0, 0xffc0, 0x0280, 0xbf8, Opcode.andi),          // d68000_andi_32
	Instr(sb,q9,D0, 0xf1f8, 0xe000, 0x000, Opcode.asr),           // d68000_asr_s_8
	Instr(sw,q9,D0, 0xf1f8, 0xe040, 0x000, Opcode.asr),           // d68000_asr_s_16
	Instr(sl,q9,D0, 0xf1f8, 0xe080, 0x000, Opcode.asr),           // d68000_asr_s_32
	Instr(sb,D9,D0, 0xf1f8, 0xe020, 0x000, Opcode.asr),           // d68000_asr_r_8
    Instr(sw,D9,D0, 0xf1f8, 0xe060, 0x000, Opcode.asr),           // d68000_asr_r_16
	Instr(sl,D9,D0, 0xf1f8, 0xe0a0, 0x000, Opcode.asr),           // d68000_asr_r_32
	Instr(sw,E0,    0xffc0, 0xe0c0, 0x3f8, Opcode.asr),           // d68000_asr_ea
	Instr(sb,q9,D0, 0xf1f8, 0xe100, 0x000, Opcode.asl),           // d68000_asl_s_8     
	Instr(sw,q9,D0, 0xf1f8, 0xe140, 0x000, Opcode.asl),           // d68000_asl_s_16    
	Instr(sl,q9,D0, 0xf1f8, 0xe180, 0x000, Opcode.asl),           // d68000_asl_s_32    
	Instr(sb,D9,D0, 0xf1f8, 0xe120, 0x000, Opcode.asl),           // d68000_asl_r_8     
	Instr(sw,D9,D0, 0xf1f8, 0xe160, 0x000, Opcode.asl),           // d68000_asl_r_16    
	Instr(sl,D9,D0, 0xf1f8, 0xe1a0, 0x000, Opcode.asl),           // d68000_asl_r_32    
	Instr(sw,E0,    0xffc0, 0xe1c0, 0x3f8, Opcode.asl),           // d68000_asl_ea      
	Instr(d68000_bcc_8        , 0xf000, 0x6000, 0x000, iclass:InstrClass.ConditionalTransfer),
    Instr(d68000_bcc_16       , 0xf0ff, 0x6000, 0x000, iclass:InstrClass.ConditionalTransfer),
    Instr(d68020_bcc_32       , 0xf0ff, 0x60ff, 0x000, iclass:InstrClass.ConditionalTransfer),
    Instr(sr,D9,E0, 0xf1c0, 0x0140, 0xbf8, Opcode.bchg),          // d68000_bchg_r 
	Instr(sr,Ib,E0, 0xffc0, 0x0840, 0xbf8, Opcode.bchg),          // d68000_bchg_s 
	Instr(sr,D9,E0, 0xf1c0, 0x0180, 0xbf8, Opcode.bclr),          // d68000_bclr_r 
	Instr(sr,Ib,E0, 0xffc0, 0x0880, 0xbf8, Opcode.bclr),          // d68000_bclr_s 
	Instr(d68020_bfchg        , 0xffc0, 0xeac0, 0xa78),
    Instr(d68020_bfclr        , 0xffc0, 0xecc0, 0xa78),
    Instr(d68020_bfexts       , 0xffc0, 0xebc0, 0xa7b),
    Instr(d68020_bfextu       , 0xffc0, 0xe9c0, 0xa7b),
    Instr(d68020_bfffo        , 0xffc0, 0xedc0, 0xa7b),
    Instr(d68020_bfins        , 0xffc0, 0xefc0, 0xa78),
    Instr(d68020_bfset        , 0xffc0, 0xeec0, 0xa78),
    Instr(d68020_bftst        , 0xffc0, 0xe8c0, 0xa7b),
    Instr(d68010_bkpt         , 0xfff8, 0x4848, 0x000),
    Instr(J, 0xff00, 0x6000, 0x000, Opcode.bra, InstrClass.Transfer),              // d68000_bra_8
	Instr(J, 0xffff, 0x6000, 0x000, Opcode.bra, InstrClass.Transfer),              // d68000_bra_16
	Instr(J, 0xffff, 0x60ff, 0x000, Opcode.bra, InstrClass.Transfer),              // d68020_bra_32
	Instr(D9,E0, 0xf1c0, 0x01c0, 0xbf8, Opcode.bset),         // d68000_bset_r
	Instr(Iw,E0, 0xffc0, 0x08c0, 0xbf8, Opcode.bset),         // d68000_bset_s
	Instr(J, 0xff00, 0x6100, 0x000, Opcode.bsr, InstrClass.Transfer|InstrClass.Call),   // d68000_bsr_8 
	Instr(J, 0xffff, 0x6100, 0x000, Opcode.bsr, InstrClass.Transfer|InstrClass.Call),   // d68000_bsr_16
	Instr(J, 0xffff, 0x61ff, 0x000, Opcode.bsr, InstrClass.Transfer|InstrClass.Call),   // d68020_bsr_32
	Instr(sl,D9,E0, 0xf1c0, 0x0100, 0xbff, Opcode.btst),      // d68000_btst_r 
	Instr(sw,Iw,E0, 0xffc0, 0x0800, 0xbfb, Opcode.btst),      // d68000_btst_s
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
    Instr(sb,E0, 0xffc0, 0x4200, 0xbf8, Opcode.clr),      // d68000_clr_8
	Instr(sw,E0, 0xffc0, 0x4240, 0xbf8, Opcode.clr),      // d68000_clr_16
	Instr(sl,E0, 0xffc0, 0x4280, 0xbf8, Opcode.clr),      // d68000_clr_32
	Instr(sb,E0,D9, 0xf1c0, 0xb000, 0xbff, Opcode.cmp),   // d68000_cmp_8
	Instr(sw,E0,D9, 0xf1c0, 0xb040, 0xfff, Opcode.cmp),   // d68000_cmp_16
	Instr(sl,E0,D9, 0xf1c0, 0xb080, 0xfff, Opcode.cmp),   // d68000_cmp_32
	Instr(sw,E0,A9, 0xf1c0, 0xb0c0, 0xfff, Opcode.cmpa),  // d68000_cmpa_16
	Instr(sl,E0,A9, 0xf1c0, 0xb1c0, 0xfff, Opcode.cmpa),  // d68000_cmpa_32
	Instr(sb,Ib,E0, 0xffc0, 0x0c00, 0xbf8, Opcode.cmpi),  // d68000_cmpi_8
	Instr(d68020_cmpi_pcdi_8  , 0xffff, 0x0c3a, 0x000),
    Instr(d68020_cmpi_pcix_8  , 0xffff, 0x0c3b, 0x000),
    Instr(sw,Iw,E0, 0xffc0, 0x0c40, 0xbf8, Opcode.cmpi),      // d68000_cmpi_16
	Instr(d68020_cmpi_pcdi_16 , 0xffff, 0x0c7a, 0x000),
    Instr(d68020_cmpi_pcix_16 , 0xffff, 0x0c7b, 0x000),
    Instr(sl,Il,E0, 0xffc0, 0x0c80, 0xbf8, Opcode.cmpi),      // d68000_cmpi_32
	Instr(d68020_cmpi_pcdi_32 , 0xffff, 0x0cba, 0x000),
    Instr(d68020_cmpi_pcix_32 , 0xffff, 0x0cbb, 0x000),
    Instr(sb,Post0,Post9, 0xf1f8, 0xb108, 0x000, Opcode.cmpm),      // d68000_cmpm_8
	Instr(sw,Post0,Post9 , 0xf1f8, 0xb148, 0x000, Opcode.cmpm),     // d68000_cmpm_16     
	Instr(sl,Post0,Post9 , 0xf1f8, 0xb188, 0x000, Opcode.cmpm),     // d68000_cmpm_32     
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
    Instr(D0,Rw, 0xfff8, 0x51c8, 0x000, Opcode.dbra),         // d68000_dbra
	Instr(sw,E0,D9, 0xf1c0, 0x81c0, 0xbff, Opcode.divs),      // d68000_divs
	Instr(su,E0,D9, 0xf1c0, 0x80c0, 0xbff, Opcode.divu),      // d68000_divu   
	Instr(d68020_divl         , 0xffc0, 0x4c40, 0xbff),
    Instr(sb,D9,E0, 0xf1c0, 0xb100, 0xbf8, Opcode.eor),          // d68000_eor_8  
	Instr(sw,D9,E0, 0xf1c0, 0xb140, 0xbf8, Opcode.eor),         // d68000_eor_16 
	Instr(sl,D9,E0, 0xf1c0, 0xb180, 0xbf8, Opcode.eor),         // d68000_eor_32 
	Instr(sb,Ib,ccr, 0xffff, 0x0a3c, 0x000, Opcode.eori),       //  d68000_eori_to_ccr
    Instr(d68000_eori_to_sr   , 0xffff, 0x0a7c, 0x000),
    Instr(sb,Ib,E0, 0xffc0, 0x0a00, 0xbf8, Opcode.eori),      // d68000_eori_8
	Instr(sw,Iw,E0, 0xffc0, 0x0a40, 0xbf8, Opcode.eori),      // d68000_eori_16
	Instr(sl,Il,E0, 0xffc0, 0x0a80, 0xbf8, Opcode.eori),      // d68000_eori_32
	Instr(D9,D0, 0xf1f8, 0xc140, 0x000, Opcode.exg),          // d68000_exg_dd 
	Instr(A9,A0, 0xf1f8, 0xc148, 0x000, Opcode.exg),          // d68000_exg_aa
	Instr(D9,A0, 0xf1f8, 0xc188, 0x000, Opcode.exg),          // d68000_exg_da
	Instr(sl,D0, 0xfff8, 0x49c0, 0x000, Opcode.extb),         // d68020_extb_32
	Instr(sw,D0, 0xfff8, 0x4880, 0x000, Opcode.ext),          // d68000_ext_16
	Instr(sl,D0, 0xfff8, 0x48c0, 0x000, Opcode.ext),          // d68000_ext_32
	Instr(d68040_fpu          , 0xffc0, 0xf200, 0x000),
    Instr(d68000_illegal      , 0xffff, 0x4afc, 0x000, iclass:InstrClass.Invalid),
    Instr(sl,E0, 0xffc0, 0x4ec0, 0x27b, Opcode.jmp, InstrClass.Transfer),   // d68000_jmp
	Instr(sl,E0, 0xffc0, 0x4e80, 0x27b, Opcode.jsr, InstrClass.Transfer|InstrClass.Call),   // d68000_jsr
	Instr(E0,A9, 0xf1c0, 0x41c0, 0x27b, Opcode.lea),       // d68000_lea
	Instr(A0,Iw, 0xfff8, 0x4e50, 0x000, Opcode.link),         // d68000_link_16 
	Instr(A0,Il, 0xfff8, 0x4808, 0x000, Opcode.link),         // d68020_link_32
	Instr(s6,q9,D0, 0xf1f8, 0xe008, 0x000, Opcode.lsr),       // d68000_lsr_s_8
	Instr(s6,q9,D0, 0xf1f8, 0xe048, 0x000, Opcode.lsr),       // d68000_lsr_s_16 
	Instr(s6,q9,D0, 0xf1f8, 0xe088, 0x000, Opcode.lsr),       // d68000_lsr_s_32 
	Instr(sb,D9,D0, 0xf1f8, 0xe028, 0x000, Opcode.lsr),       // d68000_lsr_r_8  
	Instr(sw,D9,D0, 0xf1f8, 0xe068, 0x000, Opcode.lsr),       // d68000_lsr_r_16 
	Instr(sl,D9,D0, 0xf1f8, 0xe0a8, 0x000, Opcode.lsr),       // d68000_lsr_r_32 
	Instr(sw,E0,    0xffc0, 0xe2c0, 0x3f8, Opcode.lsr),       // d68000_lsr_ea   
	Instr(s6,q9,D0, 0xf1f8, 0xe108, 0x000, Opcode.lsl),       // d68000_lsl_s_8  
	Instr(s6,q9,D0, 0xf1f8, 0xe148, 0x000, Opcode.lsl),       // d68000_lsl_s_16 
	Instr(s6,q9,D0, 0xf1f8, 0xe188, 0x000, Opcode.lsl),       // d68000_lsl_s_32 
	Instr(sb,D9,D0, 0xf1f8, 0xe128, 0x000, Opcode.lsl),       // d68000_lsl_r_8  
	Instr(sw,D9,D0, 0xf1f8, 0xe168, 0x000, Opcode.lsl),       // d68000_lsl_r_16 
	Instr(sl,D9,D0, 0xf1f8, 0xe1a8, 0x000, Opcode.lsl),       // d68000_lsl_r_32 
	Instr(sw,E0,    0xffc0, 0xe3c0, 0x3f8, Opcode.lsl),       // d68000_lsl_ea       
	Instr(sb,E0,e6, 0xf000, 0x1000, 0xbff, Opcode.move),      // d68000_move_8   
	Instr(sw,E0,e6, 0xf000, 0x3000, 0xfff, Opcode.move),      // d68000_move_16  
	Instr(sl,E0,e6, 0xf000, 0x2000, 0xfff, Opcode.move),      // d68000_move_32  
	Instr(sw,E0,A9, 0xf1c0, 0x3040, 0xfff, Opcode.movea),     // d68000_movea_16 
	Instr(sl,E0,A9, 0xf1c0, 0x2040, 0xfff, Opcode.movea),     // d68000_movea_32
	Instr(sw,E0,ccr,   0xffc0, 0x44c0, 0xbff, Opcode.move),     // d68000_move_to_ccr
	Instr(sw,ccr,E0,   0xffc0, 0x42c0, 0xbf8, Opcode.move),     // d68010_move_fr_ccr
	Instr(d68000_move_to_sr   , 0xffc0, 0x46c0, 0xbff),
    Instr(d68000_move_fr_sr   , 0xffc0, 0x40c0, 0xbf8),
    Instr(d68000_move_to_usp  , 0xfff8, 0x4e60, 0x000),
    Instr(d68000_move_fr_usp  , 0xfff8, 0x4e68, 0x000),
    Instr(d68010_movec        , 0xfffe, 0x4e7a, 0x000),
    Instr(sw,Mw,E0, 0xfff8, 0x48a0, 0x000, Opcode.movem),     // d68000_movem_pd_16
	Instr(sl,Ml,E0, 0xfff8, 0x48e0, 0x000, Opcode.movem),     // d68000_movem_pd_32
	Instr(sw,Mw,E0, 0xffc0, 0x4880, 0x2f8, Opcode.movem),     // d68000_movem_re_16
	Instr(sl,Ml,E0, 0xffc0, 0x48c0, 0x2f8, Opcode.movem),     // d68000_movem_re_32
	Instr(sw,n,E0,mw, 0xffc0, 0x4c80, 0x37b, Opcode.movem),     // d68000_movem_er_16
	Instr(sl,n,E0,ml, 0xffc0, 0x4cc0, 0x37b, Opcode.movem),     // d68000_movem_er_32
	Instr(sw,Ad0,D9, 0xf1f8, 0x0108, 0x000, Opcode.movep),      // d68000_movep_er_16
    Instr(sl,Ad0,D9, 0xf1f8, 0x0148, 0x000, Opcode.movep),      // 68000_movep_er_32
    Instr(sw,D9,Ad0,             0xf1f8, 0x0188, 0x000, Opcode.movep),   // d68000_movep_re_16),
	Instr(sl,D9,Ad0,             0xf1f8, 0x01c8, 0x000, Opcode.movep),   // d68000_movep_re_32
	Instr(d68010_moves_8      , 0xffc0, 0x0e00, 0x3f8, iclass:InstrClass.System),
	Instr(d68010_moves_16     , 0xffc0, 0x0e40, 0x3f8, iclass:InstrClass.System),
	Instr(d68010_moves_32     , 0xffc0, 0x0e80, 0x3f8, iclass:InstrClass.System),
	Instr(Q0,D9, 0xf100, 0x7000, 0x000, Opcode.moveq),        // d68000_moveq        
	Instr(d68040_move16_pi_pi , 0xfff8, 0xf620, 0x000),
	Instr(d68040_move16_pi_al , 0xfff8, 0xf600, 0x000),
	Instr(d68040_move16_al_pi , 0xfff8, 0xf608, 0x000),
	Instr(d68040_move16_ai_al , 0xfff8, 0xf610, 0x000),
	Instr(d68040_move16_al_ai , 0xfff8, 0xf618, 0x000),
	Instr(sw,E0,D9, 0xf1c0, 0xc1c0, 0xbff, Opcode.muls),      // d68000_muls         
	Instr(sw,E0,D9, 0xf1c0, 0xc0c0, 0xbff, Opcode.mulu),      // d68000_mulu
	Instr(d68020_mull         , 0xffc0, 0x4c00, 0xbff),
	Instr(d68000_nbcd         , 0xffc0, 0x4800, 0xbf8),
	Instr(sb,E0, 0xffc0, 0x4400, 0xbf8, Opcode.neg),          // d68000_neg_8
	Instr(sw,E0, 0xffc0, 0x4440, 0xbf8, Opcode.neg),          // d68000_neg_16
	Instr(sl,E0, 0xffc0, 0x4480, 0xbf8, Opcode.neg),          // d68000_neg_32
	Instr(sb,E0, 0xffc0, 0x4000, 0xbf8, Opcode.negx),         // d68000_negx_8
	Instr(sw,E0, 0xffc0, 0x4040, 0xbf8, Opcode.negx),         // d68000_negx_16
	Instr(sl,E0, 0xffc0, 0x4080, 0xbf8, Opcode.negx),         // d68000_negx_32,
	Instr(0xffff, 0x4e71, 0x000, Opcode.nop),                   // d68000_nop
	Instr(sb,E0, 0xffc0, 0x4600, 0xbf8, Opcode.not),          // d68000_not_8
	Instr(sw,E0, 0xffc0, 0x4640, 0xbf8, Opcode.not),          // d68000_not_16
	Instr(sl,E0, 0xffc0, 0x4680, 0xbf8, Opcode.not),          // d68000_not_32       
	Instr(sb,E0,D9, 0xf1c0, 0x8000, 0xbff, Opcode.or),        // d68000_or_er_8      
	Instr(sw,E0,D9, 0xf1c0, 0x8040, 0xbff, Opcode.or),        // d68000_or_er_16     
	Instr(sl,E0,D9, 0xf1c0, 0x8080, 0xbff, Opcode.or),        // d68000_or_er_32   
	Instr(sb,D9,E0, 0xf1c0, 0x8100, 0x3f8, Opcode.or),       // d68000_or_re_8     
	Instr(sw,D9,E0, 0xf1c0, 0x8140, 0x3f8, Opcode.or),       // d68000_or_re_16    
	Instr(sl,D9,E0, 0xf1c0, 0x8180, 0x3f8, Opcode.or),        // d68000_or_re_32
	Instr(sb,Ib,ccr, 0xffff, 0x003c, 0x000, Opcode.ori),        // d68000_ori_to_ccr   
	Instr(sw,Iw,SR, 0xffff, 0x007c, 0x000, Opcode.ori),        // d68000_ori_to_sr    
	Instr(s6,Iv,E0, 0xffc0, 0x0000, 0xbf8, Opcode.ori),       // d68000_ori_8        
	Instr(s6,Iv,E0, 0xffc0, 0x0040, 0xbf8, Opcode.ori),       // d68000_ori_16        
	Instr(s6,Iv,E0, 0xffc0, 0x0080, 0xbf8, Opcode.ori),       // d68000_ori_32       
	Instr(d68020_pack_rr      , 0xf1f8, 0x8140, 0x000),
	Instr(d68020_pack_mm      , 0xf1f8, 0x8148, 0x000),
	Instr(E0, 0xffc0, 0x4840, 0x27b, Opcode.pea),             // d68000_pea
	Instr(d68040_pflush       , 0xffe0, 0xf500, 0x000),
	Instr(0xffff, 0x4e70, 0x000, Opcode.reset, InstrClass.Transfer),  // d68000_reset
	Instr(sb,q9,D0, 0xf1f8, 0xe018, 0x000, Opcode.ror),       // d68000_ror_s_8
	Instr(sw,q9,D0, 0xf1f8, 0xe058, 0x000, Opcode.ror),       // d68000_ror_s_16
	Instr(sl,q9,D0, 0xf1f8, 0xe098, 0x000, Opcode.ror),       // d68000_ror_s_32
	Instr(sb,D9,D0, 0xf1f8, 0xe038, 0x000, Opcode.ror),       // d68000_ror_r_8 
	Instr(sw,D9,D0, 0xf1f8, 0xe078, 0x000, Opcode.ror),       // d68000_ror_r_16
	Instr(sl,D9,D0, 0xf1f8, 0xe0b8, 0x000, Opcode.ror),       // d68000_ror_r_32
	Instr(sl,E0, 0xffc0, 0xe6c0, 0x3f8, Opcode.ror),          // d68000_ror_ea
	Instr(sb,q9,D0, 0xf1f8, 0xe118, 0x000, Opcode.rol),       // d68000_rol_s_8
	Instr(sw,q9,D0, 0xf1f8, 0xe158, 0x000, Opcode.rol),       // d68000_rol_s_16
	Instr(sl,q9,D0, 0xf1f8, 0xe198, 0x000, Opcode.rol),       // d68000_rol_s_32
	Instr(sb,D9,D0, 0xf1f8, 0xe138, 0x000, Opcode.rol),       // d68000_rol_r_8
	Instr(sw,D9,D0, 0xf1f8, 0xe178, 0x000, Opcode.rol),       // d68000_rol_r_16
	Instr(sl,D9,D0, 0xf1f8, 0xe1b8, 0x000, Opcode.rol),       // d68000_rol_r_32
	Instr(sl,E0,    0xffc0, 0xe7c0, 0x3f8, Opcode.rol),         // d68000_rol_ea
	Instr(sb,q9,D0, 0xf1f8, 0xe010, 0x000, Opcode.roxr),      // d68000_roxr_s_8 
	Instr(sw,q9,D0, 0xf1f8, 0xe050, 0x000, Opcode.roxr),      // d68000_roxr_s_16
	Instr(sl,q9,D0, 0xf1f8, 0xe090, 0x000, Opcode.roxr),      // d68000_roxr_s_32
	Instr(sb,D9,D0, 0xf1f8, 0xe030, 0x000, Opcode.roxr),      // d68000_roxr_r_8 
	Instr(sw,D9,D0, 0xf1f8, 0xe070, 0x000, Opcode.roxr),      // d68000_roxr_r_16
	Instr(sl,D9,D0, 0xf1f8, 0xe0b0, 0x000, Opcode.roxr),      // d68000_roxr_r_32
	Instr(sl,E0, 0xffc0, 0xe4c0, 0x3f8, Opcode.roxr),         // d68000_roxr_ea  
	Instr(sb,q9,D0, 0xf1f8, 0xe110, 0x000, Opcode.roxl),      // d68000_roxl_s_8 
	Instr(sw,q9,D0, 0xf1f8, 0xe150, 0x000, Opcode.roxl),      // d68000_roxl_s_16
	Instr(sl,q9,D0, 0xf1f8, 0xe190, 0x000, Opcode.roxl),      // d68000_roxl_s_32
	Instr(sb,D9,D0, 0xf1f8, 0xe130, 0x000, Opcode.roxl),      // d68000_roxl_r_8 
	Instr(sw,D9,D0, 0xf1f8, 0xe170, 0x000, Opcode.roxl),      // d68000_roxl_r_16
	Instr(sl,D9,D0, 0xf1f8, 0xe1b0, 0x000, Opcode.roxl),      // d68000_roxl_r_32
	Instr(sl,E0, 0xffc0, 0xe5c0, 0x3f8, Opcode.roxl),         // d68000_roxl_ea 
	Instr(Iw, 0xffff, 0x4e74, 0x000, Opcode.rtd, InstrClass.Transfer),      // d68010_rtd
	Instr(0xffff, 0x4e73, 0x000, Opcode.rte, InstrClass.Transfer|InstrClass.System),        // d68000_rte
	Instr(d68020_rtm, 0xfff0, 0x06c0, 0x000, iclass:InstrClass.Transfer),
	Instr(0xffff, 0x4e77, 0x000, Opcode.rtr, InstrClass.Transfer),        // d68000_rtr
	Instr(0xffff, 0x4e75, 0x000, Opcode.rts, InstrClass.Transfer),        // d68000_rts
	Instr(D0,D9, 0xf1f8, 0x8100, 0x000, Opcode.sbcd),         // d68000_sbcd_rr
	Instr(Pre0,Pre9, 0xf1f8, 0x8108, 0x000, Opcode.sbcd),         // d68000_sbcd_mm
	Instr(d68000_scc          , 0xf0c0, 0x50c0, 0xbf8),
	Instr(d68000_stop         , 0xffff, 0x4e72, 0x000),
	Instr(sb,E0,D9, 0xf1c0, 0x9000, 0xbff, Opcode.sub),       // d68000_sub_er_8
	Instr(sw,E0,D9, 0xf1c0, 0x9040, 0xfff, Opcode.sub),       // d68000_sub_er_16
	Instr(sl,E0,D9, 0xf1c0, 0x9080, 0xfff, Opcode.sub),       // d68000_sub_er_32
	Instr(sb,D9,E0, 0xf1c0, 0x9100, 0x3f8, Opcode.sub),       // d68000_sub_re_8
	Instr(sw,D9,E0, 0xf1c0, 0x9140, 0x3f8, Opcode.sub),       // d68000_sub_re_16
	Instr(sl,D9,E0, 0xf1c0, 0x9180, 0x3f8, Opcode.sub),       // d68000_sub_re_32
	Instr(sw,E0,A9, 0xf1c0, 0x90c0, 0xfff, Opcode.suba),      // d68000_suba_16
	Instr(sl,E0,A9, 0xf1c0, 0x91c0, 0xfff, Opcode.suba),      // d68000_suba_32
	Instr(sb,Ib,E0,   0xffc0, 0x0400, 0xbf8, Opcode.subi),      // d68000_subi_8
	Instr(sw,Iw,E0,   0xffc0, 0x0440, 0xbf8, Opcode.subi),      // d68000_subi_16
	Instr(sl,Il,E0,   0xffc0, 0x0480, 0xbf8, Opcode.subi),      // d68000_subi_32
	Instr(sb,q9,E0,   0xf1c0, 0x5100, 0xbf8, Opcode.subq),      // d68000_subq_8
	Instr(sw,q9,E0,   0xf1c0, 0x5140, 0xff8, Opcode.subq),      // d68000_subq_16
	Instr(sl,q9,E0,   0xf1c0, 0x5180, 0xff8, Opcode.subq),      // d68000_subq_32
	Instr(sb,D0,D9,   0xf1f8, 0x9100, 0x000, Opcode.subx),      // d68000_subx_rr_8
	Instr(sw,D0,D9,   0xf1f8, 0x9140, 0x000, Opcode.subx),      // d68000_subx_rr_16
	Instr(sl,D0,D9,   0xf1f8, 0x9180, 0x000, Opcode.subx),      // d68000_subx_rr_32
	Instr(sb,Pre0,Pre9,   0xf1f8, 0x9108, 0x000, Opcode.subx),      // d68000_subx_mm_8
	Instr(sw,Pre0,Pre9,   0xf1f8, 0x9148, 0x000, Opcode.subx),      // d68000_subx_mm_16
	Instr(sl,Pre0,Pre9,     0xf1f8, 0x9188, 0x000, Opcode.subx),      // d68000_subx_mm_32
	Instr(sl,D0,      0xfff8, 0x4840, 0x000, Opcode.swap),      // d68000_swap
	Instr(d68000_tas          , 0xffc0, 0x4ac0, 0xbf8),
	Instr(d68000_trap         , 0xfff0, 0x4e40, 0x000, iclass:InstrClass.Transfer|InstrClass.Call),
	Instr(d68020_trapcc_0     , 0xf0ff, 0x50fc, 0x000, iclass:InstrClass.Transfer|InstrClass.Call),
	Instr(d68020_trapcc_16    , 0xf0ff, 0x50fa, 0x000, iclass:InstrClass.Transfer|InstrClass.Call),
	Instr(d68020_trapcc_32    , 0xf0ff, 0x50fb, 0x000, iclass:InstrClass.Transfer|InstrClass.Call),
	Instr(0xffff, 0x4e76, 0x000, Opcode.trapv, InstrClass.Transfer|InstrClass.Call),  // d68000_trapv
	Instr(sb,E0, 0xffc0, 0x4a00, 0xbf8, Opcode.tst),              // d68000_tst_8
	Instr(d68020_tst_pcdi_8   , 0xffff, 0x4a3a, 0x000),
	Instr(d68020_tst_pcix_8   , 0xffff, 0x4a3b, 0x000),
	Instr(d68020_tst_i_8      , 0xffff, 0x4a3c, 0x000),
	Instr(sw,E0, 0xffc0, 0x4a40, 0xbf8, Opcode.tst),              // d68000_tst_16
	Instr(d68020_tst_a_16     , 0xfff8, 0x4a48, 0x000),
	Instr(d68020_tst_pcdi_16  , 0xffff, 0x4a7a, 0x000),
	Instr(d68020_tst_pcix_16  , 0xffff, 0x4a7b, 0x000),
	Instr(d68020_tst_i_16     , 0xffff, 0x4a7c, 0x000),
	Instr(sl,E0,      0xffc0, 0x4a80, 0xbf8, Opcode.tst),         // d68000_tst_32
	Instr(d68020_tst_a_32     , 0xfff8, 0x4a88, 0x000),
	Instr(sl,E0,      0xffff, 0x4aba, 0x000, Opcode.tst),         // d68020_tst_pcdi_32
	Instr(d68020_tst_pcix_32  , 0xffff, 0x4abb, 0x000),
	Instr(sl,E0, 0xffff, 0x4abc, 0x000, Opcode.tst),              // d68020_tst_i_32
	Instr(A0, 0xfff8, 0x4e58, 0x000, Opcode.unlk),                // d68000_unlk
	Instr(d68020_unpk_rr      , 0xf1f8, 0x8180, 0x000),
	Instr(d68020_unpk_mm      , 0xf1f8, 0x8188, 0x000),
	Instr(d68851_p000         , 0xffc0, 0xf000, 0x000),
	Instr(d68851_pbcc16       , 0xffc0, 0xf080, 0x000),
	Instr(d68851_pbcc32       , 0xffc0, 0xf0c0, 0x000),
	Instr(d68851_pdbcc        , 0xfff8, 0xf048, 0x000),
	Instr(d68851_p001         , 0xffc0, 0xf040, 0x000),
	Instr(d => false, 0, 0, 0),
};
        }

        private static Decoder illegalOpcode = new Decoder(new Mutator[0], 0, 0, 0, Opcode.illegal);

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

        static int compare_nof_true_bits(Decoder aptr, Decoder bptr)
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

        private static void build_opcode_table()
        {
            int ostruct;
            Decoder[] opcode_info = (Decoder[])g_opcode_info.Clone();
            Array.Sort<Decoder>(opcode_info, compare_nof_true_bits);

            for (uint i = 0; i < 0x10000; i++)
            {
                g_instruction_table[i] = illegalOpcode;     //default to illegal
                uint opcode = i;
                // search through opcode info for a match
                for (ostruct = 0; opcode_info[ostruct].mutators != null; ostruct++)
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
                                 !valid_ea(((opcode >> 9) & 7) | ((opcode >> 3) & 0x38), 0xbf8))
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