#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Arch.M68k
{
    // M68k opcode map in http://www.freescale.com/files/archives/doc/ref_manual/M68000PRM.pdf

    public class M68kDisassembler : Disassembler
    {
        private ImageReader rdr;

        public M68kDisassembler(ImageReader rdr)
        {
            this.rdr = rdr;
        }

        public Address Address
        {
            get { return rdr.Address; }
        }

        public MachineInstruction DisassembleInstruction()
        {
            return Disassemble();
        }

        private Decoder FindDecoder(ushort opcode)
        {
            int l = 0;
            int h = oprecs.Count - 1;
            int m;
            while (l <= h)
            {
                m = l + (h - l) / 2;
                Opmask d = oprecs.Keys[m];
                int c = d.Compare(opcode);
                if (c == 0)
                    return oprecs.Values[m];
                else if (c < 0)
                    h = m - 1;
                else
                    l = m + 1;
            }
            return null;
        }

        public M68kInstruction Disassemble()
        {
            ushort opcode = rdr.ReadBeUInt16();
            DumpOprecs();
            Decoder decoder = FindDecoder(opcode);
            if (decoder == null)
            {
                string msg = string.Format("Unknown 680x0 opcode {0:X4}.", opcode);
                Debug.WriteLine(msg);
                throw new InvalidOperationException(msg);
            }
            Debug.WriteLine(string.Format("{0:X4}->{1} {2}", opcode, decoder.opcode, decoder.args));
            return decoder.Decode(opcode, rdr);
        }


        private class Opmask : IComparable<Opmask>
        {
            public readonly ushort opcode;
            public readonly ushort mask;

            public Opmask(ushort code, ushort mask)
            {
                this.opcode = code;
                this.mask = mask;
            }


            private ushort Normalize(ushort value)
            {
                return (ushort)(value & mask);
            }

            public int Compare(ushort value)
            {
                return Normalize(value) - Normalize(this.opcode);
            }

            public int CompareTo(Opmask other)
            {
                return Normalize(opcode) - other.Normalize(other.opcode);
            }
        }


        private static SortedList<Opmask, Decoder> oprecs;

        static M68kDisassembler()
        {
            oprecs = new SortedList<Opmask, Decoder>();
            oprecs.Add(new Opmask(0x0000, 0xFF3C), new Decoder(Opcode.ori, "s6:Iv,e0"));
            oprecs.Add(new Opmask(0x0008, 0xFF3C), new Decoder(Opcode.ori, "s6:Iv,e0"));
            oprecs.Add(new Opmask(0x0010, 0xFF3C), new Decoder(Opcode.ori, "s6:Iv,e0"));
            oprecs.Add(new Opmask(0x0018, 0xFF3C), new Decoder(Opcode.ori, "s6:Iv,e0"));
            oprecs.Add(new Opmask(0x0020, 0xFF3C), new Decoder(Opcode.ori, "s6:Iv,e0"));
            oprecs.Add(new Opmask(0x0028, 0xFF3C), new Decoder(Opcode.ori, "s6:Iv,e0"));
            oprecs.Add(new Opmask(0x0030, 0xFF3C), new Decoder(Opcode.ori, "s6:Iv,e0"));
            oprecs.Add(new Opmask(0x003C, 0xFFFF), new Decoder(Opcode.ori, "sb:Ib,c"));
            oprecs.Add(new Opmask(0x007C, 0xFFFF), new Decoder(Opcode.ori, "sw:Iw,s"));
            oprecs.Add(new Opmask(0x0140, 0xF1C0), new Decoder(Opcode.bchg, "D9,E0"));
            oprecs.Add(new Opmask(0x1000, 0xF000), new Decoder(Opcode.move, "sb:E0,e6"));
            oprecs.Add(new Opmask(0x2000, 0xF1C0), new Decoder(Opcode.move, "sl:E0,e6"));
            oprecs.Add(new Opmask(0x2040, 0xF1C0), new Decoder(Opcode.movea, "sl:E0,A9"));
            oprecs.Add(new Opmask(0x2080, 0xF1C0), new Decoder(Opcode.move, "sl:E0,e6"));
            oprecs.Add(new Opmask(0x20C0, 0xF1C0), new Decoder(Opcode.move, "sl:E0,e6"));
            oprecs.Add(new Opmask(0x2100, 0xF1C0), new Decoder(Opcode.move, "sl:E0,e6"));

            oprecs.Add(new Opmask(0x3000, 0xF000), new Decoder(Opcode.move, "sw:E0,e6"));
            oprecs.Add(new Opmask(0x41C0, 0xFFC0), new Decoder(Opcode.lea, "E0,A9"));
            oprecs.Add(new Opmask(0x43C0, 0xFFC0), new Decoder(Opcode.lea, "E0,A9"));
            oprecs.Add(new Opmask(0x45C0, 0xFFC0), new Decoder(Opcode.lea, "E0,A9"));
            oprecs.Add(new Opmask(0x47C0, 0xFFC0), new Decoder(Opcode.lea, "E0,A9"));
            oprecs.Add(new Opmask(0x4BC0, 0xFFC0), new Decoder(Opcode.lea, "E0,A9"));
            oprecs.Add(new Opmask(0x4DC0, 0xFFC0), new Decoder(Opcode.lea, "E0,A9"));
            oprecs.Add(new Opmask(0x4FC0, 0xFFC0), new Decoder(Opcode.lea, "E0,A9"));
            oprecs.Add(new Opmask(0x48C0, 0xFFC0), new Decoder(Opcode.movem, "sl:Iw,E0"));
            oprecs.Add(new Opmask(0x5000, 0xF100), new Decoder(Opcode.addq, "s6:q9,E0"));
            oprecs.Add(new Opmask(0x51C8, 0xFFF8), new Decoder(Opcode.dbf, "D0,Rw"));
            oprecs.Add(new Opmask(0x6000, 0xFF00), new Decoder(Opcode.bra, "J"));
            oprecs.Add(new Opmask(0x7000, 0xF100), new Decoder(Opcode.moveq, "Q0,D9"));
            oprecs.Add(new Opmask(0xD000, 0xF100), new Decoder(Opcode.add, "s6:E0,D9"));
            oprecs.Add(new Opmask(0xD0C0, 0xF1C0), new Decoder(Opcode.adda, "sw:E0,A9"));
            oprecs.Add(new Opmask(0xD100, 0xF1C0), new Decoder(Opcode.add, "D9,s6:E0"));
            oprecs.Add(new Opmask(0xD1C0, 0xF1C0), new Decoder(Opcode.adda, "sl:E0,A9"));
            oprecs.Add(new Opmask(0xE108, 0xF138), new Decoder(Opcode.lsl, "s6:q9,D0"));


            DumpOprecs();
        }

        private static void DumpOprecs()
        {
            foreach (KeyValuePair<Opmask, Decoder> item in oprecs)
            {
                Debug.WriteLine(string.Format("{0:X4},{1:x4}:{2},{3}", item.Key.opcode, item.Key.mask, item.Value.opcode, item.Value.args));
            }
        }
    }

    public class M68kDisassembler2
    {
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
        private static bool BIT_0(uint A) { return ((A) & 0x00000001) != 0; }
        private static bool BIT_1(uint A) { return ((A) & 0x00000002) != 0; }
        private static bool BIT_2(uint A) { return ((A) & 0x00000004) != 0; }
        private static bool BIT_3(uint A) { return ((A) & 0x00000008) != 0; }
        private static bool BIT_4(uint A) { return ((A) & 0x00000010) != 0; }
        private static bool BIT_5(uint A) { return ((A) & 0x00000020) != 0; }
        private static bool BIT_6(uint A) { return ((A) & 0x00000040) != 0; }
        private static bool BIT_7(uint A) { return ((A) & 0x00000080) != 0; }
        private static bool BIT_8(uint A) { return ((A) & 0x00000100) != 0; }
        private static bool BIT_9(uint A) { return ((A) & 0x00000200) != 0; }
        private static bool BIT_A(uint A) { return ((A) & 0x00000400) != 0; }
        private static bool BIT_B(uint A) { return ((A) & 0x00000800) != 0; }
        private static bool BIT_C(uint A) { return ((A) & 0x00001000) != 0; }
        private static bool BIT_D(uint A) { return ((A) & 0x00002000) != 0; }
        private static bool BIT_E(uint A) { return ((A) & 0x00004000) != 0; }
        private static bool BIT_F(uint A) { return ((A) & 0x00008000) != 0; }
        private static bool BIT_10(uint A) { return ((A) & 0x00010000) != 0; }
        private static bool BIT_11(uint A) { return ((A) & 0x00020000) != 0; }
        private static bool BIT_12(uint A) { return ((A) & 0x00040000) != 0; }
        private static bool BIT_13(uint A) { return ((A) & 0x00080000) != 0; }
        private static bool BIT_14(uint A) { return ((A) & 0x00100000) != 0; }
        private static bool BIT_15(uint A) { return ((A) & 0x00200000) != 0; }
        private static bool BIT_16(uint A) { return ((A) & 0x00400000) != 0; }
        private static bool BIT_17(uint A) { return ((A) & 0x00800000) != 0; }
        private static bool BIT_18(uint A) { return ((A) & 0x01000000) != 0; }
        private static bool BIT_19(uint A) { return ((A) & 0x02000000) != 0; }
        private static bool BIT_1A(uint A) { return ((A) & 0x04000000) != 0; }
        private static bool BIT_1B(uint A) { return ((A) & 0x08000000) != 0; }
        private static bool BIT_1C(uint A) { return ((A) & 0x10000000) != 0; }
        private static bool BIT_1D(uint A) { return ((A) & 0x20000000) != 0; }
        private static bool BIT_1E(uint A) { return ((A) & 0x40000000) != 0; }
        private static bool BIT_1F(uint A) { return ((A) & 0x80000000) != 0; }

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
        private static uint EXT_8BIT_DISPLACEMENT(uint A) { return ((A) & 0xff); }
        private static bool EXT_FULL(uint A) { return BIT_8(A); }
        private static bool EXT_EFFECTIVE_ZERO(uint A) { return (((A) & 0xe4) == 0xc4 || ((A) & 0xe2) == 0xc0); }
        private static bool EXT_BASE_REGISTER_PRESENT(uint A) { return !(BIT_7(A)); }
        private static bool EXT_INDEX_REGISTER_PRESENT(uint A) { return !(BIT_6(A)); }
        private static uint EXT_INDEX_REGISTER(uint A) { return (((A) >> 12) & 7); }
        private static bool EXT_INDEX_PRE_POST(uint A) { return (EXT_INDEX_REGISTER_PRESENT(A) && (A & 3) != 0); }
        private static bool EXT_INDEX_PRE(uint A) { return (EXT_INDEX_REGISTER_PRESENT(A) && ((A) & 7) < 4 && ((A) & 7) != 0); }
        private static bool EXT_INDEX_POST(uint A) { return (EXT_INDEX_REGISTER_PRESENT(A) && ((A) & 7) > 4); }
        private static int EXT_INDEX_SCALE(uint A) { return (int)(((A) >> 9) & 3); }
        private static bool EXT_INDEX_LONG(uint A) { return BIT_B(A); }
        private static bool EXT_INDEX_AR(uint A) { return BIT_F(A); }
        private static bool EXT_BASE_DISPLACEMENT_PRESENT(uint A) { return (((A) & 0x30) > 0x10); }
        private static bool EXT_BASE_DISPLACEMENT_WORD(uint A) { return (((A) & 0x30) == 0x20); }
        private static bool EXT_BASE_DISPLACEMENT_LONG(uint A) { return (((A) & 0x30) == 0x30); }
        private static bool EXT_OUTER_DISPLACEMENT_PRESENT(uint A) { return (((A) & 3) > 1 && ((A) & 0x47) < 0x44); }
        private static bool EXT_OUTER_DISPLACEMENT_WORD(uint A) { return (((A) & 3) == 2 && ((A) & 0x47) < 0x44); }
        private static bool EXT_OUTER_DISPLACEMENT_LONG(uint A) { return (((A) & 3) == 3 && ((A) & 0x47) < 0x44); }


        /* Opcode flags */
        private void SET_OPCODE_FLAGS(uint x) { g_opcode_type = x; ; }
        private uint COMBINE_OPCODE_FLAGS(uint x) { return ((x) | g_opcode_type | DASMFLAG_SUPPORTED); }
        private const uint DASMFLAG_SUPPORTED = 0;
        private const uint DASMFLAG_STEP_OVER = 1;
        private const uint DASMFLAG_STEP_OUT = 2;

        /* ======================================================================== */
        /* =============================== PROTOTYPES ============================= */
        /* ======================================================================== */

        ///* Read data at the PC and increment PC */
        //uint  read_imm_8();
        //uint  read_imm_16();
        //uint  read_imm_32();

        ///* Read data at the PC but don't imcrement the PC */
        //uint  peek_imm_8();
        //uint  peek_imm_16();
        //uint  peek_imm_32();

        ///* make signed integers 100% portably */
        //static int make_int_8(int value);
        //static int make_int_16(int value);
        //static int make_int_32(int value);

        ///* make string of ea mode */
        //static string get_ea_mode_str(uint instruction, uint size);

        //string get_ea_mode_str_8(uint instruction);
        //string get_ea_mode_str_16(uint instruction);
        //string get_ea_mode_str_32(uint instruction);

        ///* make string of immediate value */
        //static string get_imm_str_s(uint size);
        //static string get_imm_str_u(uint size);

        //string get_imm_str_s8();
        //string get_imm_str_s16();
        //string get_imm_str_s32();

        ///* Stuff to build the opcode handler jump table */
        //static void  build_opcode_table();
        //static int   valid_ea(uint opcode, uint mask);
        //static int  compare_nof_true_bits(const void aptr, const void *bptr);

        /* used to build opcode handler jump table */
        class opcode_struct
        {
            public readonly Action opcode_handler;
            public readonly uint mask;                  // opcode mask
            public readonly uint match;                 // opcode bit patter (after mask)
            public readonly uint ea_mask;               // Permitted ea modes are allowed 

            public readonly Opcode opcode;              // The decoded opcode.
            public readonly string operandFormat;       // Format string which when interpreted generates the operands of the instruction.

            public opcode_struct(Action handler, uint mask, uint match, uint ea_mask)
            {
                this.opcode_handler = handler;
                this.mask = mask;
                this.match = match;
                this.ea_mask = ea_mask;

                this.opcode = Opcode.illegal;
                this.operandFormat = "$";
            }

            public opcode_struct(uint mask, uint match, uint ea_mask, Opcode opcode, string opFormat)
            {
                this.opcode_handler = null; 
                this.mask = mask;
                this.match = match;
                this.ea_mask = ea_mask;

                this.opcode = opcode;
                this.operandFormat = opFormat;
                this.opcode_handler = UseDecodeMethod; 
            }

            public void UseDecodeMethod()
            {
                throw new InvalidOperationException("Use the Decode method instead.");
            }
        }



        /* ======================================================================== */
        /* ================================= DATA ================================= */
        /* ======================================================================== */

        /* Opcode handler jump table */
        static opcode_struct[] g_instruction_table = new opcode_struct[0x10000];
        /* Flag if disassembler initialized */
        static bool g_initialized = false;

        string g_dasm_str; /* string to hold disassembly */
        string g_helper_str; /* string to hold helpful info */
        ImageReader g_cpu_pc;        /* program counter */
        ushort g_cpu_ir;        /* instruction register */
        uint g_cpu_type;
        uint g_opcode_type;
        string g_rawop;
        uint g_rawbasepc;
        M68kInstruction instr;

        /* used by ops like asr, ror, addq, etc */
        static uint[] g_3bit_qdata_table = new uint[8] { 8, 1, 2, 3, 4, 5, 6, 7 };

        static uint[] g_5bit_data_table = new uint[32]
        {
	        32,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15,
	        16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31
        };

        static string[] g_cc = new string[16] { "t", "f", "hi", "ls", "cc", "cs", "ne", "eq", "vc", "vs", "pl", "mi", "ge", "lt", "gt", "le" };

        static string[] g_cpcc = new string[64] 
        {
                /* 000    001    010    011    100    101    110    111 */
	          "f",  "eq", "ogt", "oge", "olt", "ole", "ogl",  "or", /* 000 */
	          "un", "ueq", "ugt", "uge", "ult", "ule",  "ne",   "t", /* 001 */
	          "sf", "seq",  "gt",  "ge",  "lt",  "le",  "gl",  "gle", /* 010 */
              "ngle", "ngl", "nle", "nlt", "nge", "ngt", "sne",  "st", /* 011 */
	          "?",   "?",   "?",   "?",   "?",   "?",   "?",   "?", /* 100 */
	          "?",   "?",   "?",   "?",   "?",   "?",   "?",   "?", /* 101 */
	          "?",   "?",   "?",   "?",   "?",   "?",   "?",   "?", /* 110 */
	          "?",   "?",   "?",   "?",   "?",   "?",   "?",   "?"  /* 111 */
        };

        static string[] g_mmuregs = new string[8] 
        {
	        "tc", "drp", "srp", "crp", "cal", "val", "sccr", "acr"
        };

        static string[] g_mmucond = new string[16] 
        {
	        "bs", "bc", "ls", "lc", "ss", "sc", "as", "ac",
	        "ws", "wc", "is", "ic", "gs", "gc", "cs", "cc"
        };

        /* ======================================================================== */
        /* =========================== UTILITY FUNCTIONS ========================== */
        /* ======================================================================== */

        private bool LIMIT_CPU_TYPES(uint ALLOWED_CPU_TYPES)
        {
            if ((g_cpu_type & ALLOWED_CPU_TYPES) == 0)
            {
                if ((g_cpu_ir & 0xf000) == 0xf000)
                    d68000_1111();
                else d68000_illegal();
                return false;
            }
            return true;
        }

        //static uint dasm_read_imm_8(uint advance)
        //{
        //    uint result;
        //    result = g_rawop[g_cpu_pc + 1 - g_rawbasepc];
        //    g_cpu_pc += advance;
        //    return result;
        //}

        //static uint dasm_read_imm_16(uint advance)
        //{
        //    uint result;
        //    result = (g_rawop[g_cpu_pc + 0 - g_rawbasepc] << 8) |
        //              g_rawop[g_cpu_pc + 1 - g_rawbasepc];
        //    g_cpu_pc += advance;
        //    return result;
        //}

        //static uint dasm_read_imm_32(uint advance)
        //{
        //    uint result;
        //    result = (g_rawop[g_cpu_pc + 0 - g_rawbasepc] << 24) |
        //             (g_rawop[g_cpu_pc + 1 - g_rawbasepc] << 16) |
        //             (g_rawop[g_cpu_pc + 2 - g_rawbasepc] << 8) |
        //              g_rawop[g_cpu_pc + 3 - g_rawbasepc];
        //    g_cpu_pc += advance;
        //    return result;
        //}

        //private byte read_imm_8()  { dasm_read_imm_8(2)
        //#define read_imm_16() dasm_read_imm_16(2)
        //#define read_imm_32() dasm_read_imm_32(4)

        //#define peek_imm_8()  dasm_read_imm_8(0)
        //#define peek_imm_16() dasm_read_imm_16(0)
        //#define peek_imm_32() dasm_read_imm_32(0)

        /* Fake a split interface */
        private string get_ea_mode_str_8(uint instruction) { return get_ea_mode_str(instruction, 0); }
        private string get_ea_mode_str_16(uint instruction) { return get_ea_mode_str(instruction, 1); }
        private string get_ea_mode_str_32(uint instruction) { return get_ea_mode_str(instruction, 2); }
        private string get_ea_mode_str_8(int instruction) { return get_ea_mode_str((uint)instruction, 0); }
        private string get_ea_mode_str_16(int instruction) { return get_ea_mode_str((uint)instruction, 1); }
        private string get_ea_mode_str_32(int instruction) { return get_ea_mode_str((uint)instruction, 2); }

        private static string get_imm_str_s8() { return get_imm_str_s(0); }
        private static string get_imm_str_s16() { return get_imm_str_s(1); }
        private static string get_imm_str_s32() { return get_imm_str_s(2); }

        private static string get_imm_str_u8() { return get_imm_str_u(0); }
        private static string get_imm_str_u16() { return get_imm_str_u(1); }
        private static string get_imm_str_u32() { return get_imm_str_u(2); }

        static int sext_7bit_int(int value)
        {
            return (value & 0x40) != 0 ? (value | ~0x7F) : (value & 0x7f);
        }


        /* 100% portable signed int generators */
        static int make_int_8(uint value) { return make_int_8((int)value); }
        static int make_int_8(int value)
        {
            return (value & 0x80) != 0 ? value | ~0xff : value & 0xff;
        }

        static int make_int_16(uint value) { return make_int_16((int)value); }
        static int make_int_16(int value)
        {
            return (value & 0x8000) != 0 ? value | ~0xffff : value & 0xffff;
        }

        static int make_int_32(uint value) { return (int)value; } 
        static int make_int_32(int value)
        {
            return value;
        }

        /* Get string representation of hex values */
        static string make_signed_hex_str_8(uint val)
        {
            val &= 0xff;

            if (val == 0x80)
                return "-$80";
            else if ((val & 0x80) != 0)
                return string.Format("-${0:X2}", (0 - val) & 0x7f);
            else
                return string.Format("${0:X2}", val & 0x7f);
        }

        static string make_signed_hex_str_16(uint val)
        {
            val &= 0xffff;

            if (val == 0x8000)
                return "-$8000";
            else if ((val & 0x8000) != 0)
                return string.Format("-${0:X4}", (0 - val) & 0x7fff);
            else
                return string.Format("${0:X4}", val & 0x7fff);
        }

        static string make_signed_hex_str_32(uint val)
        {
            val &= 0xffffffff;

            if (val == 0x80000000)
                return "-$80000000";
            else if ((val & 0x80000000) != 0)
                return string.Format("-${0:X8}", (0 - val) & 0x7fffffff);
            else
                return string.Format("${0:X8}", val & 0x7fffffff);
        }

        private static byte read_imm_8() { throw new NotImplementedException(); }
        private static ushort read_imm_16() { throw new NotImplementedException(); }
        private static uint read_imm_32() { throw new NotImplementedException(); }

        /* make string of immediate value */
        private static string get_imm_str_s(uint size)
        {
            if (size == 0)
                return "#" + make_signed_hex_str_8(read_imm_8());
            else if (size == 1)
                return "#" + make_signed_hex_str_16(read_imm_16());
            else
                return "#" + make_signed_hex_str_32(read_imm_32());
        }

        private static string get_imm_str_u(uint size)
        {
            if (size == 0)
                return string.Format("#${0:X2}", read_imm_8() & 0xff);
            else if (size == 1)
                return string.Format("#${0:Xr}", read_imm_16() & 0xffff);
            else
                return string.Format("#${0:X8}", read_imm_32() & 0xffffffff);
        }

        static string b1 = "";
        static string b2 = "";
        static string mode = b2;

        /* Make string of effective address mode */
        private string get_ea_mode_str(uint instruction, uint size)
        {
            uint extension;
            uint @base;
            uint outer;
            string base_reg = "";
            string index_reg = "";
            bool preindex;
            bool postindex;
            uint comma = 0;
            uint temp_value;
            byte invalid_mode = 0;

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
                /* data register direct */
                return string.Format("D{0}", instruction & 7);
            case 0x08:
            case 0x09:
            case 0x0a:
            case 0x0b:
            case 0x0c:
            case 0x0d:
            case 0x0e:
            case 0x0f:
                /* address register direct */
                return string.Format("A{0}", instruction & 7);
            case 0x10:
            case 0x11:
            case 0x12:
            case 0x13:
            case 0x14:
            case 0x15:
            case 0x16:
            case 0x17:
                /* address register indirect */
                return string.Format("(A{0})", instruction & 7);
            case 0x18:
            case 0x19:
            case 0x1a:
            case 0x1b:
            case 0x1c:
            case 0x1d:
            case 0x1e:
            case 0x1f:
                /* address register indirect with postincrement */
                return string.Format("(A{0})+", instruction & 7);
            case 0x20:
            case 0x21:
            case 0x22:
            case 0x23:
            case 0x24:
            case 0x25:
            case 0x26:
            case 0x27:
                /* address register indirect with predecrement */
                return string.Format("-(A{0}))", instruction & 7);
            case 0x28:
            case 0x29:
            case 0x2a:
            case 0x2b:
            case 0x2c:
            case 0x2d:
            case 0x2e:
            case 0x2f:
                /* address register indirect with displacement*/
                return string.Format("({0},A{1})", make_signed_hex_str_16(read_imm_16()), instruction & 7);
            case 0x30:
            case 0x31:
            case 0x32:
            case 0x33:
            case 0x34:
            case 0x35:
            case 0x36:
            case 0x37:
                /* address register indirect with index */
                extension = read_imm_16();

                if ((g_cpu_type & M68010_LESS) != 0 && EXT_INDEX_SCALE(extension) != 0)
                {
                    invalid_mode = 1;
                    break;
                }

                if (EXT_FULL(extension))
                {
                    if ((g_cpu_type & M68010_LESS) != 0)
                    {
                        invalid_mode = 1;
                        break;
                    }

                    if (EXT_EFFECTIVE_ZERO(extension))
                    {
                        mode = "0";
                        break;
                    }

                    @base = EXT_BASE_DISPLACEMENT_PRESENT(extension) ? (EXT_BASE_DISPLACEMENT_LONG(extension) ? read_imm_32() : read_imm_16()) : 0;
                    outer = EXT_OUTER_DISPLACEMENT_PRESENT(extension) ? (EXT_OUTER_DISPLACEMENT_LONG(extension) ? read_imm_32() : read_imm_16()) : 0;
                    if (EXT_BASE_REGISTER_PRESENT(extension))
                        base_reg = string.Format("A{0}", instruction & 7);
                    else
                        base_reg = "";
                    if (EXT_INDEX_REGISTER_PRESENT(extension))
                    {
                        index_reg = string.Format("{0}{1}.{2}", EXT_INDEX_AR(extension) ? 'A' : 'D', EXT_INDEX_REGISTER(extension), EXT_INDEX_LONG(extension) ? 'l' : 'w');
                        if (EXT_INDEX_SCALE(extension) != 0)
                            index_reg += string.Format("*{0}", 1 << EXT_INDEX_SCALE(extension));
                    }
                    else
                        index_reg = "";
                    preindex = (extension & 7) > 0 && (extension & 7) < 4;
                    postindex = (extension & 7) > 4;

                    mode = "(";
                    if (preindex || postindex)
                        mode += "[";
                    if (@base != 0)
                    {
                        if (EXT_BASE_DISPLACEMENT_LONG(extension))
                        {
                            mode += make_signed_hex_str_32(@base);
                        }
                        else
                        {
                            mode += make_signed_hex_str_16(@base);
                        }
                        comma = 1;
                    }
                    if (base_reg != "")
                    {
                        if (comma != 0)
                            mode += ",";
                        mode += base_reg;
                        comma = 1;
                    }
                    if (postindex)
                    {
                        mode += "]";
                        comma = 1;
                    }
                    if (index_reg != "")
                    {
                        if (comma != 0)
                            mode += ",";
                        mode += index_reg;
                        comma = 1;
                    }
                    if (preindex)
                    {
                        mode += "]"; ;
                        comma = 1;
                    }
                    if (outer != 0)
                    {
                        if (comma != 0)
                            mode += ",";
                        mode += make_signed_hex_str_16(outer);
                    }
                    mode += ")";
                    break;
                }

                if (EXT_8BIT_DISPLACEMENT(extension) == 0)
                    mode = string.Format("(A{0},{1}{2}.{3}", instruction & 7, EXT_INDEX_AR(extension) ? 'A' : 'D', EXT_INDEX_REGISTER(extension), EXT_INDEX_LONG(extension) ? 'l' : 'w');
                else
                    mode = string.Format("({0},A{1},{2}{3}.{4}", make_signed_hex_str_8(extension), instruction & 7, EXT_INDEX_AR(extension) ? 'A' : 'D', EXT_INDEX_REGISTER(extension), EXT_INDEX_LONG(extension) ? 'l' : 'w');
                if (EXT_INDEX_SCALE(extension) != 0)
                    mode += string.Format("*{0}", 1 << EXT_INDEX_SCALE(extension));
                mode += ")";
                break;
            case 0x38:
                // Absolute short address
                mode = string.Format("${0}.w", read_imm_16());
                break;
            case 0x39:
                // Absolute long address
                mode = string.Format("${0}.l", read_imm_32());
                break;
            case 0x3a:
                // Program counter with displacement
                throw new NotImplementedException();
                //temp_value = read_imm_16();
                //mode = string.Format("({0},PC)", make_signed_hex_str_16(temp_value));
                //g_helper_str = string.Format("; (${0})", (make_int_16(temp_value) + g_cpu_pc - 2) & 0xffffffff);
                //break;
            case 0x3b:
                // Program counter with index
                extension = read_imm_16();

                if ((g_cpu_type & M68010_LESS) != 0 && EXT_INDEX_SCALE(extension) != 0)
                {
                    invalid_mode = 1;
                    break;
                }

                if (EXT_FULL(extension))
                {
                    if ((g_cpu_type & M68010_LESS) != 0)
                    {
                        invalid_mode = 1;
                        break;
                    }

                    if (EXT_EFFECTIVE_ZERO(extension))
                    {
                        mode = "0";
                        break;
                    }
                    @base = EXT_BASE_DISPLACEMENT_PRESENT(extension) ? (EXT_BASE_DISPLACEMENT_LONG(extension) ? read_imm_32() : read_imm_16()) : 0;
                    outer = EXT_OUTER_DISPLACEMENT_PRESENT(extension) ? (EXT_OUTER_DISPLACEMENT_LONG(extension) ? read_imm_32() : read_imm_16()) : 0;
                    if (EXT_BASE_REGISTER_PRESENT(extension))
                        base_reg = "PC";
                    else
                        base_reg = "";
                    if (EXT_INDEX_REGISTER_PRESENT(extension))
                    {
                        index_reg = string.Format("{0}{1}.{2}", EXT_INDEX_AR(extension) ? 'A' : 'D', EXT_INDEX_REGISTER(extension), EXT_INDEX_LONG(extension) ? 'l' : 'w');
                        if (EXT_INDEX_SCALE(extension) != 0)
                            index_reg += string.Format("*{0}", 1 << EXT_INDEX_SCALE(extension));
                    }
                    else
                        index_reg = "";
                    preindex = (extension & 7) > 0 && (extension & 7) < 4;
                    postindex = (extension & 7) > 4;

                    mode = "(";
                    if (preindex || postindex)
                        mode += "[";
                    if (@base != 0)
                    {
                        mode += make_signed_hex_str_16(@base);
                        comma = 1;
                    }
                    if (base_reg != "")
                    {
                        if (comma != 0)
                            mode += ",";
                        mode += base_reg;
                        comma = 1;
                    }
                    if (postindex)
                    {
                        mode += "]";
                        comma = 1;
                    }
                    if (index_reg != "")
                    {
                        if (comma != 0)
                            mode += ",";
                        mode += index_reg;
                        comma = 1;
                    }
                    if (preindex)
                    {
                        mode += "]";
                        comma = 1;
                    }
                    if (outer != 0)
                    {
                        if (comma != 0)
                            mode += ",";
                        mode += make_signed_hex_str_16(outer);
                    }
                    mode += ")";
                    break;
                }

                if (EXT_8BIT_DISPLACEMENT(extension) == 0)
                    mode = string.Format("(PC,{0}%d.%c", EXT_INDEX_AR(extension) ? 'A' : 'D', EXT_INDEX_REGISTER(extension), EXT_INDEX_LONG(extension) ? 'l' : 'w');
                else
                    mode = string.Format("({0},PC,%c%d.%c", make_signed_hex_str_8(extension), EXT_INDEX_AR(extension) ? 'A' : 'D', EXT_INDEX_REGISTER(extension), EXT_INDEX_LONG(extension) ? 'l' : 'w');
                if (EXT_INDEX_SCALE(extension) != 0)
                    mode += string.Format("*{0}", 1 << EXT_INDEX_SCALE(extension));
                mode += ")";
                break;
            case 0x3c:
                /* Immediate */
                mode = string.Format("{0}", get_imm_str_u(size));
                break;
            default:
                invalid_mode = 1;
                break;
            }

            if (invalid_mode != 0)
                mode = string.Format("INVALID {0}", instruction & 0x3f);

            return mode;
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

        private void d68000_illegal()
        {
            g_dasm_str = string.Format("dc.w    $%04x; ILLEGAL", g_cpu_ir);
        }

        private void d68000_1010()
        {
            g_dasm_str = string.Format("dc.w    $%04x; opcode 1010", g_cpu_ir);
        }


        private void d68000_1111()
        {
            g_dasm_str = string.Format("dc.w    $%04x; opcode 1111", g_cpu_ir);
        }


        private void d68000_abcd_rr()
        {
            g_dasm_str = string.Format("abcd    D{0},D{1}", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }


        private void d68000_abcd_mm()
        {
            g_dasm_str = string.Format("abcd    -(A{0}), -(A%d)", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_add_er_8()
        {
            g_dasm_str = string.Format("add.b   {0},D{1}", get_ea_mode_str_8(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }


        private void d68000_add_er_16()
        {
            g_dasm_str = string.Format("add.w   {0},D{1}", get_ea_mode_str_16(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_add_er_32()
        {
            g_dasm_str = string.Format("add.l   {0},D{1}", get_ea_mode_str_32(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_add_re_8()
        {
            g_dasm_str = string.Format("add.b   D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_add_re_16()
        {
            g_dasm_str = string.Format("add.w   D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_add_re_32()
        {
            g_dasm_str = string.Format("add.l   D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_adda_16()
        {
            g_dasm_str = string.Format("adda.w  {0},A{1}", get_ea_mode_str_16(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_adda_32()
        {
            g_dasm_str = string.Format("adda.l  {0},A{1}", get_ea_mode_str_32(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_addi_8()
        {
            string str = get_imm_str_s8();
            g_dasm_str = string.Format("addi.b  {0},{1}", str, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_addi_16()
        {
            string str = get_imm_str_s16();
            g_dasm_str = string.Format("addi.w  {0},{1}", str, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_addi_32()
        {
            string str = get_imm_str_s32();
            g_dasm_str = string.Format("addi.l  {0},{1}", str, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_addq_8()
        {
            g_dasm_str = string.Format("addq.b  #{0},{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_addq_16()
        {
            g_dasm_str = string.Format("addq.w  #{0},{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_addq_32()
        {
            g_dasm_str = string.Format("addq.l  #{0},{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_addx_rr_8()
        {
            g_dasm_str = string.Format("addx.b  D{0},D{1}", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_addx_rr_16()
        {
            g_dasm_str = string.Format("addx.w  D{0},D{1}", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_addx_rr_32()
        {
            g_dasm_str = string.Format("addx.l  D{0},D{1}", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_addx_mm_8()
        {
            g_dasm_str = string.Format("addx.b  -(A{0}), -(A%d)", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_addx_mm_16()
        {
            g_dasm_str = string.Format("addx.w  -(A{0}), -(A%d)", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_addx_mm_32()
        {
            g_dasm_str = string.Format("addx.l  -(A{0}), -(A%d)", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_and_er_8()
        {
            g_dasm_str = string.Format("and.b   {0},D{1}", get_ea_mode_str_8(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_and_er_16()
        {
            g_dasm_str = string.Format("and.w   {0},D{1}", get_ea_mode_str_16(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_and_er_32()
        {
            g_dasm_str = string.Format("and.l   {0},D{1}", get_ea_mode_str_32(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_and_re_8()
        {
            g_dasm_str = string.Format("and.b   D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_and_re_16()
        {
            g_dasm_str = string.Format("and.w   D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_and_re_32()
        {
            g_dasm_str = string.Format("and.l   D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_andi_8()
        {
            string str = get_imm_str_u8();
            g_dasm_str = string.Format("andi.b  {0},{1}", str, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_andi_16()
        {
            string str = get_imm_str_u16();
            g_dasm_str = string.Format("andi.w  {0},{1}", str, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_andi_32()
        {
            string str = get_imm_str_u32();
            g_dasm_str = string.Format("andi.l  {0},{1}", str, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_andi_to_ccr()
        {
            g_dasm_str = string.Format("andi    {0}, CCR", get_imm_str_u8());
        }

        private void d68000_andi_to_sr()
        {
            g_dasm_str = string.Format("andi    {0}, SR", get_imm_str_u16());
        }

        private void d68000_asr_s_8()
        {
            g_dasm_str = string.Format("asr.b   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_asr_s_16()
        {
            g_dasm_str = string.Format("asr.w   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_asr_s_32()
        {
            g_dasm_str = string.Format("asr.l   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_asr_r_8()
        {
            g_dasm_str = string.Format("asr.b   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_asr_r_16()
        {
            g_dasm_str = string.Format("asr.w   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_asr_r_32()
        {
            g_dasm_str = string.Format("asr.l   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_asr_ea()
        {
            g_dasm_str = string.Format("asr.w   {0}", get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_asl_s_8()
        {
            g_dasm_str = string.Format("asl.b   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_asl_s_16()
        {
            g_dasm_str = string.Format("asl.w   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_asl_s_32()
        {
            g_dasm_str = string.Format("asl.l   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_asl_r_8()
        {
            g_dasm_str = string.Format("asl.b   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_asl_r_16()
        {
            g_dasm_str = string.Format("asl.w   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_asl_r_32()
        {
            g_dasm_str = string.Format("asl.l   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_asl_ea()
        {
            g_dasm_str = string.Format("asl.w   {0}", get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_bcc_8()
        {
            uint temp_pc = g_cpu_pc.Address.Linear;
            g_dasm_str = string.Format("b%-2s     $%x", g_cc[(g_cpu_ir >> 8) & 0xf], temp_pc + make_int_8(g_cpu_ir));
        }

        private void d68000_bcc_16()
        {
            uint temp_pc = g_cpu_pc.Address.Linear;
            g_dasm_str = string.Format("b%-2s     $%x", g_cc[(g_cpu_ir >> 8) & 0xf], temp_pc + make_int_16(read_imm_16()));
        }

        private void d68020_bcc_32()
        {
            uint temp_pc = g_cpu_pc.Address.Linear;
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("b%-2s     $%x; (2+)", g_cc[(g_cpu_ir >> 8) & 0xf], temp_pc + read_imm_32());
        }

        private void d68000_bchg_r()
        {
            g_dasm_str = string.Format("bchg    D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_bchg_s()
        {
            string str = get_imm_str_u8();
            g_dasm_str = string.Format("bchg    {0},{1}", str, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_bclr_r()
        {
            g_dasm_str = string.Format("bclr    D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_bclr_s()
        {
            string str = get_imm_str_u8();
            g_dasm_str = string.Format("bclr    {0},{1}", str, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68010_bkpt()
        {
            LIMIT_CPU_TYPES(M68010_PLUS);
            g_dasm_str = string.Format("bkpt #{0}; (1+)", g_cpu_ir & 7);
        }

        private void d68020_bfchg()
        {
            uint extension;
            string offset;
            string width;

            LIMIT_CPU_TYPES(M68020_PLUS);

            extension = read_imm_16();

            if (BIT_B(extension))
                offset = string.Format("D{0}", (extension >> 6) & 7);
            else
                offset = string.Format("{0}", (extension >> 6) & 31);
            if (BIT_5(extension))
                width = string.Format("D{0}", extension & 7);
            else
                width = string.Format("{0}", g_5bit_data_table[extension & 31]);
            g_dasm_str = string.Format("bfchg   {0} {%s:%s}; (2+)", get_ea_mode_str_8(g_cpu_ir), offset, width);
        }

        private void d68020_bfclr()
        {
            uint extension;
            string offset;
            string width;

            LIMIT_CPU_TYPES(M68020_PLUS);

            extension = read_imm_16();

            if (BIT_B(extension))
                offset = string.Format("D{0}", (extension >> 6) & 7);
            else
                offset = string.Format("{0}", (extension >> 6) & 31);
            if (BIT_5(extension))
                width = string.Format("D{0}", extension & 7);
            else
                width = string.Format("{0}", g_5bit_data_table[extension & 31]);
            g_dasm_str = string.Format("bfclr   {0} {%s:%s}; (2+)", get_ea_mode_str_8(g_cpu_ir), offset, width);
        }

        private void d68020_bfexts()
        {
            uint extension;
            string offset; ;
            string width; ;

            LIMIT_CPU_TYPES(M68020_PLUS);

            extension = read_imm_16();

            if (BIT_B(extension))
                offset = string.Format("D{0}", (extension >> 6) & 7);
            else
                offset = string.Format("{0}", (extension >> 6) & 31);
            if (BIT_5(extension))
                width = string.Format("D{0}", extension & 7);
            else
                width = string.Format("{0}", g_5bit_data_table[extension & 31]);
            g_dasm_str = string.Format("bfexts  D{0},{1} {%s:%s}; (2+)", (extension >> 12) & 7, get_ea_mode_str_8(g_cpu_ir), offset, width);
        }

        private void d68020_bfextu()
        {
            uint extension;
            string offset; ;
            string width; ;

            LIMIT_CPU_TYPES(M68020_PLUS);

            extension = read_imm_16();

            if (BIT_B(extension))
                offset = string.Format("D{0}", (extension >> 6) & 7);
            else
                offset = string.Format("{0}", (extension >> 6) & 31);
            if (BIT_5(extension))
                width = string.Format("D{0}", extension & 7);
            else
                width = string.Format("{0}", g_5bit_data_table[extension & 31]);
            g_dasm_str = string.Format("bfextu  D{0},{1} {%s:%s}; (2+)", (extension >> 12) & 7, get_ea_mode_str_8(g_cpu_ir), offset, width);
        }

        private void d68020_bfffo()
        {
            uint extension;
            string offset; ;
            string width; ;

            LIMIT_CPU_TYPES(M68020_PLUS);

            extension = read_imm_16();

            if (BIT_B(extension))
                offset = string.Format("D{0}", (extension >> 6) & 7);
            else
                offset = string.Format("{0}", (extension >> 6) & 31);
            if (BIT_5(extension))
                width = string.Format("D{0}", extension & 7);
            else
                width = string.Format("{0}", g_5bit_data_table[extension & 31]);
            g_dasm_str = string.Format("bfffo   D{0},{1} {%s:%s}; (2+)", (extension >> 12) & 7, get_ea_mode_str_8(g_cpu_ir), offset, width);
        }

        private void d68020_bfins()
        {
            uint extension;
            string offset; ;
            string width; ;

            LIMIT_CPU_TYPES(M68020_PLUS);

            extension = read_imm_16();

            if (BIT_B(extension))
                offset = string.Format("D{0}", (extension >> 6) & 7);
            else
                offset = string.Format("{0}", (extension >> 6) & 31);
            if (BIT_5(extension))
                width = string.Format("D{0}", extension & 7);
            else
                width = string.Format("{0}", g_5bit_data_table[extension & 31]);
            g_dasm_str = string.Format("bfins   D{0},{1} {%s:%s}; (2+)", (extension >> 12) & 7, get_ea_mode_str_8(g_cpu_ir), offset, width);
        }

        private void d68020_bfset()
        {
            uint extension;
            string offset; ;
            string width; ;

            LIMIT_CPU_TYPES(M68020_PLUS);

            extension = read_imm_16();

            if (BIT_B(extension))
                offset = string.Format("D{0}", (extension >> 6) & 7);
            else
                offset = string.Format("{0}", (extension >> 6) & 31);
            if (BIT_5(extension))
                width = string.Format("D{0}", extension & 7);
            else
                width = string.Format("{0}", g_5bit_data_table[extension & 31]);
            g_dasm_str = string.Format("bfset   {0} {%s:%s}; (2+)", get_ea_mode_str_8(g_cpu_ir), offset, width);
        }

        private void d68020_bftst()
        {
            uint extension;
            string offset; ;
            string width; ;

            LIMIT_CPU_TYPES(M68020_PLUS);

            extension = read_imm_16();

            if (BIT_B(extension))
                offset = string.Format("D{0}", (extension >> 6) & 7);
            else
                offset = string.Format("{0}", (extension >> 6) & 31);
            if (BIT_5(extension))
                width = string.Format("D{0}", extension & 7);
            else
                width = string.Format("{0}", g_5bit_data_table[extension & 31]);
            g_dasm_str = string.Format("bftst   {0} {%s:%s}; (2+)", get_ea_mode_str_8(g_cpu_ir), offset, width);
        }

        private void d68000_bra_8()
        {
            uint temp_pc = g_cpu_pc.Address.Linear;
            g_dasm_str = string.Format("bra     ${0}", temp_pc + make_int_8(g_cpu_ir));
        }

        private void d68000_bra_16()
        {
            uint temp_pc = g_cpu_pc.Address.Linear;
            g_dasm_str = string.Format("bra     ${0}", temp_pc + make_int_16(read_imm_16()));
        }

        private void d68020_bra_32()
        {
            uint temp_pc = g_cpu_pc.Address.Linear;
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("bra     ${0}; (2+)", temp_pc + read_imm_32());
        }

        private void d68000_bset_r()
        {
            g_dasm_str = string.Format("bset    D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_bset_s()
        {
            string str = get_imm_str_u8();
            g_dasm_str = string.Format("bset    {0},{1}", str, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_bsr_8()
        {
            uint temp_pc = g_cpu_pc.Address.Linear;
            g_dasm_str = string.Format("bsr     ${0}", temp_pc + make_int_8(g_cpu_ir));
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OVER);
        }

        private void d68000_bsr_16()
        {
            uint temp_pc = g_cpu_pc.Address.Linear;
            g_dasm_str = string.Format("bsr     ${0}", temp_pc + make_int_16(read_imm_16()));
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OVER);
        }

        private void d68020_bsr_32()
        {
            uint temp_pc = g_cpu_pc.Address.Linear;
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("bsr     ${0}; (2+)", temp_pc + read_imm_32());
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OVER);
        }

        private void d68000_btst_r()
        {
            g_dasm_str = string.Format("btst    D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_btst_s()
        {
            string str = get_imm_str_u8();
            g_dasm_str = string.Format("btst    {0},{1}", str, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68020_callm()
        {
            string str;
            LIMIT_CPU_TYPES(M68020_ONLY);
            str = get_imm_str_u8();

            g_dasm_str = string.Format("callm   {0},{1}; (2)", str, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68020_cas_8()
        {
            uint extension;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension = read_imm_16();
            g_dasm_str = string.Format("cas.b   D{0},D{1},{1}; (2+)", extension & 7, (extension >> 8) & 7, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68020_cas_16()
        {
            uint extension;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension = read_imm_16();
            g_dasm_str = string.Format("cas.w   D{0},D{1},{1}; (2+)", extension & 7, (extension >> 8) & 7, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68020_cas_32()
        {
            uint extension;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension = read_imm_16();
            g_dasm_str = string.Format("cas.l   D{0},D{1},{1}; (2+)", extension & 7, (extension >> 8) & 7, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68020_cas2_16()
        {
            /* CAS2 Dc1:Dc2,Du1:Dc2:(Rn1):(Rn2)
            f e d c b a 9 8 7 6 5 4 3 2 1 0
             DARn1  0 0 0  Du1  0 0 0  Dc1
             DARn2  0 0 0  Du2  0 0 0  Dc2
            */

            uint extension;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension = read_imm_32();
            g_dasm_str = string.Format("cas2.w  D{0}:D%d:D%d:D%d, (%c%d):(%c%d); (2+)",
                (extension >> 16) & 7, extension & 7, (extension >> 22) & 7, (extension >> 6) & 7,
                BIT_1F(extension) ? 'A' : 'D', (extension >> 28) & 7,
                BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7);
        }

        private void d68020_cas2_32()
        {
            uint extension;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension = read_imm_32();
            g_dasm_str = string.Format("cas2.l  D{0}:D%d:D%d:D%d, (%c%d):(%c%d); (2+)",
                (extension >> 16) & 7, extension & 7, (extension >> 22) & 7, (extension >> 6) & 7,
                BIT_1F(extension) ? 'A' : 'D', (extension >> 28) & 7,
                BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7);
        }

        private void d68000_chk_16()
        {
            g_dasm_str = string.Format("chk.w   {0},D{1}", get_ea_mode_str_16(g_cpu_ir), (g_cpu_ir >> 9) & 7);
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OVER);
        }

        private void d68020_chk_32()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("chk.l   {0},D{1}; (2+)", get_ea_mode_str_32(g_cpu_ir), (g_cpu_ir >> 9) & 7);
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OVER);
        }

        private void d68020_chk2_cmp2_8()
        {
            uint extension;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension = read_imm_16();
            g_dasm_str = string.Format("{0}.b  %s, %c%d; (2+)", BIT_B(extension) ? "chk2" : "cmp2", get_ea_mode_str_8(g_cpu_ir), BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7);
        }

        private void d68020_chk2_cmp2_16()
        {
            uint extension;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension = read_imm_16();
            g_dasm_str = string.Format("{0}.w  %s, %c%d; (2+)", BIT_B(extension) ? "chk2" : "cmp2", get_ea_mode_str_16(g_cpu_ir), BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7);
        }

        private void d68020_chk2_cmp2_32()
        {
            uint extension;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension = read_imm_16();
            g_dasm_str = string.Format("{0}.l  %s, %c%d; (2+)", BIT_B(extension) ? "chk2" : "cmp2", get_ea_mode_str_32(g_cpu_ir), BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7);
        }

        private void d68040_cinv()
        {
            LIMIT_CPU_TYPES(M68040_PLUS);
            switch ((g_cpu_ir >> 3) & 3)
            {
            case 0:
                g_dasm_str = string.Format("cinv (illegal scope); (4)");
                break;
            case 1:
                g_dasm_str = string.Format("cinvl   {0}, (A%d); (4)", (g_cpu_ir >> 6) & 3, g_cpu_ir & 7);
                break;
            case 2:
                g_dasm_str = string.Format("cinvp   {0}, (A%d); (4)", (g_cpu_ir >> 6) & 3, g_cpu_ir & 7);
                break;
            case 3:
                g_dasm_str = string.Format("cinva   {0}; (4)", (g_cpu_ir >> 6) & 3);
                break;
            }
        }

        private void d68000_clr_8()
        {
            g_dasm_str = string.Format("clr.b   {0}", get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_clr_16()
        {
            g_dasm_str = string.Format("clr.w   {0}", get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_clr_32()
        {
            g_dasm_str = string.Format("clr.l   {0}", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_cmp_8()
        {
            g_dasm_str = string.Format("cmp.b   {0},D{1}", get_ea_mode_str_8(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_cmp_16()
        {
            g_dasm_str = string.Format("cmp.w   {0},D{1}", get_ea_mode_str_16(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_cmp_32()
        {
            g_dasm_str = string.Format("cmp.l   {0},D{1}", get_ea_mode_str_32(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_cmpa_16()
        {
            g_dasm_str = string.Format("cmpa.w  {0},A{1}", get_ea_mode_str_16(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_cmpa_32()
        {
            g_dasm_str = string.Format("cmpa.l  {0},A{1}", get_ea_mode_str_32(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_cmpi_8()
        {
            string str = get_imm_str_s8();
            g_dasm_str = string.Format("cmpi.b  {0},{1}", str, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68020_cmpi_pcdi_8()
        {
            string str;
            LIMIT_CPU_TYPES(M68010_PLUS);
            str = get_imm_str_s8();
            g_dasm_str = string.Format("cmpi.b  {0},{1}; (2+)", str, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68020_cmpi_pcix_8()
        {
            string str;
            LIMIT_CPU_TYPES(M68010_PLUS);
            str = get_imm_str_s8();
            g_dasm_str = string.Format("cmpi.b  {0},{1}; (2+)", str, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_cmpi_16()
        {
            string str;
            str = get_imm_str_s16();
            g_dasm_str = string.Format("cmpi.w  {0},{1}", str, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68020_cmpi_pcdi_16()
        {
            string str;
            LIMIT_CPU_TYPES(M68010_PLUS);
            str = get_imm_str_s16();
            g_dasm_str = string.Format("cmpi.w  {0},{1}; (2+)", str, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68020_cmpi_pcix_16()
        {
            string str;
            LIMIT_CPU_TYPES(M68010_PLUS);
            str = get_imm_str_s16();
            g_dasm_str = string.Format("cmpi.w  {0},{1}; (2+)", str, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_cmpi_32()
        {
            string str;
            str = get_imm_str_s32();
            g_dasm_str = string.Format("cmpi.l  {0},{1}", str, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68020_cmpi_pcdi_32()
        {
            string str;
            LIMIT_CPU_TYPES(M68010_PLUS);
            str = get_imm_str_s32();
            g_dasm_str = string.Format("cmpi.l  {0},{1}; (2+)", str, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68020_cmpi_pcix_32()
        {
            string str;
            LIMIT_CPU_TYPES(M68010_PLUS);
            str = get_imm_str_s32();
            g_dasm_str = string.Format("cmpi.l  {0},{1}; (2+)", str, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_cmpm_8()
        {
            g_dasm_str = string.Format("cmpm.b  (A{0})+, (A%d)+", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_cmpm_16()
        {
            g_dasm_str = string.Format("cmpm.w  (A{0})+, (A%d)+", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_cmpm_32()
        {
            g_dasm_str = string.Format("cmpm.l  (A{0})+, (A%d)+", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68020_cpbcc_16()
        {
            uint extension;
            uint new_pc = g_cpu_pc.Address.Linear;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension = read_imm_16();
            new_pc = ((uint)(new_pc + make_int_16(read_imm_16())));
            g_dasm_str = string.Format("%db%-4s  %s; %x (extension = %x) (2-3)", (g_cpu_ir >> 9) & 7, g_cpcc[g_cpu_ir & 0x3f], get_imm_str_s16(), new_pc, extension);
        }

        private void d68020_cpbcc_32()
        {
            uint extension;
            uint new_pc = g_cpu_pc.Address.Linear;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension = read_imm_16();
            new_pc += read_imm_32();
            g_dasm_str = string.Format("%db%-4s  %s; %x (extension = %x) (2-3)", (g_cpu_ir >> 9) & 7, g_cpcc[g_cpu_ir & 0x3f], get_imm_str_s16(), new_pc, extension);
        }

        private void d68020_cpdbcc()
        {
            uint extension1;
            uint extension2;
            uint new_pc = g_cpu_pc.Address.Linear;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension1 = read_imm_16();
            extension2 = read_imm_16();
            new_pc += ((uint)make_int_16(read_imm_16()));
            g_dasm_str = string.Format("%ddb%-4s D%d,%s; %x (extension = %x) (2-3)", (g_cpu_ir >> 9) & 7, g_cpcc[extension1 & 0x3f], g_cpu_ir & 7, get_imm_str_s16(), new_pc, extension2);
        }

        private void d68020_cpgen()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("%dgen    %s; (2-3)", (g_cpu_ir >> 9) & 7, get_imm_str_u32());
        }

        private void d68020_cprestore()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            if (((g_cpu_ir >> 9) & 7) == 1)
            {
                g_dasm_str = string.Format("frestore {0}", get_ea_mode_str_8(g_cpu_ir));
            }
            else
            {
                g_dasm_str = string.Format("%drestore %s; (2-3)", (g_cpu_ir >> 9) & 7, get_ea_mode_str_8(g_cpu_ir));
            }
        }

        private void d68020_cpsave()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            if (((g_cpu_ir >> 9) & 7) == 1)
            {
                g_dasm_str = string.Format("fsave   {0}", get_ea_mode_str_8(g_cpu_ir));
            }
            else
            {
                g_dasm_str = string.Format("%dsave   %s; (2-3)", (g_cpu_ir >> 9) & 7, get_ea_mode_str_8(g_cpu_ir));
            }
        }

        private void d68020_cpscc()
        {
            uint extension1;
            uint extension2;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension1 = read_imm_16();
            extension2 = read_imm_16();
            g_dasm_str = string.Format("%ds%-4s  %s; (extension = %x) (2-3)", (g_cpu_ir >> 9) & 7, g_cpcc[extension1 & 0x3f], get_ea_mode_str_8(g_cpu_ir), extension2);
        }

        private void d68020_cptrapcc_0()
        {
            uint extension1;
            uint extension2;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension1 = read_imm_16();
            extension2 = read_imm_16();
            g_dasm_str = string.Format("%dtrap%-4s; (extension = %x) (2-3)", (g_cpu_ir >> 9) & 7, g_cpcc[extension1 & 0x3f], extension2);
        }

        private void d68020_cptrapcc_16()
        {
            uint extension1;
            uint extension2;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension1 = read_imm_16();
            extension2 = read_imm_16();
            g_dasm_str = string.Format("%dtrap%-4s %s; (extension = %x) (2-3)", (g_cpu_ir >> 9) & 7, g_cpcc[extension1 & 0x3f], get_imm_str_u16(), extension2);
        }

        private void d68020_cptrapcc_32()
        {
            uint extension1;
            uint extension2;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension1 = read_imm_16();
            extension2 = read_imm_16();
            g_dasm_str = string.Format("%dtrap%-4s %s; (extension = %x) (2-3)", (g_cpu_ir >> 9) & 7, g_cpcc[extension1 & 0x3f], get_imm_str_u32(), extension2);
        }

        private void d68040_cpush()
        {
            LIMIT_CPU_TYPES(M68040_PLUS);
            switch ((g_cpu_ir >> 3) & 3)
            {
            case 0:
                g_dasm_str = string.Format("cpush (illegal scope); (4)");
                break;
            case 1:
                g_dasm_str = string.Format("cpushl  {0}, (A%d); (4)", (g_cpu_ir >> 6) & 3, g_cpu_ir & 7);
                break;
            case 2:
                g_dasm_str = string.Format("cpushp  {0}, (A%d); (4)", (g_cpu_ir >> 6) & 3, g_cpu_ir & 7);
                break;
            case 3:
                g_dasm_str = string.Format("cpusha  {0}; (4)", (g_cpu_ir >> 6) & 3);
                break;
            }
        }

        private void d68000_dbra()
        {
            uint temp_pc = g_cpu_pc.Address.Linear;
            g_dasm_str = string.Format("dbra    D{0}, $%x", g_cpu_ir & 7, temp_pc + make_int_16(read_imm_16()));
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OVER);
        }

        private void d68000_dbcc()
        {
            uint temp_pc = g_cpu_pc.Address.Linear;
            g_dasm_str = string.Format("db%-2s    D%d, $%x", g_cc[(g_cpu_ir >> 8) & 0xf], g_cpu_ir & 7, temp_pc + make_int_16(read_imm_16()));
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OVER);
        }

        private void d68000_divs()
        {
            g_dasm_str = string.Format("divs.w  {0},D{1}", get_ea_mode_str_16(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_divu()
        {
            g_dasm_str = string.Format("divu.w  {0},D{1}", get_ea_mode_str_16(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68020_divl()
        {
            uint extension;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension = read_imm_16();

            if (BIT_A(extension))
                g_dasm_str = string.Format("div{0}.l  %s, D%d:D%d; (2+)", BIT_B(extension) ? 's' : 'u', get_ea_mode_str_32(g_cpu_ir), extension & 7, (extension >> 12) & 7);
            else if ((extension & 7) == ((extension >> 12) & 7))
                g_dasm_str = string.Format("div{0}.l  %s, D%d; (2+)", BIT_B(extension) ? 's' : 'u', get_ea_mode_str_32(g_cpu_ir), (extension >> 12) & 7);
            else
                g_dasm_str = string.Format("div%cl.l %s, D%d:D%d; (2+)", BIT_B(extension) ? 's' : 'u', get_ea_mode_str_32(g_cpu_ir), extension & 7, (extension >> 12) & 7);
        }

        private void d68000_eor_8()
        {
            g_dasm_str = string.Format("eor.b   D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_eor_16()
        {
            g_dasm_str = string.Format("eor.w   D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_eor_32()
        {
            g_dasm_str = string.Format("eor.l   D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_eori_8()
        {
            string str = get_imm_str_u8();
            g_dasm_str = string.Format("eori.b  {0},{1}", str, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_eori_16()
        {
            string str = get_imm_str_u16();
            g_dasm_str = string.Format("eori.w  {0},{1}", str, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_eori_32()
        {
            string str = get_imm_str_u32();
            g_dasm_str = string.Format("eori.l  {0},{1}", str, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_eori_to_ccr()
        {
            g_dasm_str = string.Format("eori    {0}, CCR", get_imm_str_u8());
        }

        private void d68000_eori_to_sr()
        {
            g_dasm_str = string.Format("eori    {0}, SR", get_imm_str_u16());
        }

        private void d68000_exg_dd()
        {
            g_dasm_str = string.Format("exg     D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_exg_aa()
        {
            g_dasm_str = string.Format("exg     A{0},A{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_exg_da()
        {
            g_dasm_str = string.Format("exg     D{0},A{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_ext_16()
        {
            g_dasm_str = string.Format("ext.w   D{0}", g_cpu_ir & 7);
        }

        private void d68000_ext_32()
        {
            g_dasm_str = string.Format("ext.l   D{0}", g_cpu_ir & 7);
        }

        private void d68020_extb_32()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("extb.l  D{0}; (2+)", g_cpu_ir & 7);
        }

        static string[] float_data_format = new string[8] 
	{
		".l", ".s", ".x", ".p", ".w", ".d", ".b", ".p"
	};

        private void d68040_fpu()
        {

            string mnemonic;
            uint w2, src, dst_reg;
            LIMIT_CPU_TYPES(M68030_PLUS);
            w2 = read_imm_16();

            src = (w2 >> 10) & 0x7;
            dst_reg = (w2 >> 7) & 0x7;

            // special override for FMOVECR
            if ((((w2 >> 13) & 0x7) == 2) && (((w2 >> 10) & 0x7) == 7))
            {
                g_dasm_str = string.Format("fmovecr   #$%0x, fp%d", (w2 & 0x7f), dst_reg);
                return;
            }

            switch ((w2 >> 13) & 0x7)
            {
            case 0x0:
            case 0x2:
                {
                    switch (w2 & 0x7f)
                    {
                    case 0x00: mnemonic = "fmove"; break;
                    case 0x01: mnemonic = "fint"; break;
                    case 0x02: mnemonic = "fsinh"; break;
                    case 0x03: mnemonic = "fintrz"; break;
                    case 0x04: mnemonic = "fsqrt"; break;
                    case 0x06: mnemonic = "flognp1"; break;
                    case 0x08: mnemonic = "fetoxm1"; break;
                    case 0x09: mnemonic = "ftanh1"; break;
                    case 0x0a: mnemonic = "fatan"; break;
                    case 0x0c: mnemonic = "fasin"; break;
                    case 0x0d: mnemonic = "fatanh"; break;
                    case 0x0e: mnemonic = "fsin"; break;
                    case 0x0f: mnemonic = "ftan"; break;
                    case 0x10: mnemonic = "fetox"; break;
                    case 0x11: mnemonic = "ftwotox"; break;
                    case 0x12: mnemonic = "ftentox"; break;
                    case 0x14: mnemonic = "flogn"; break;
                    case 0x15: mnemonic = "flog10"; break;
                    case 0x16: mnemonic = "flog2"; break;
                    case 0x18: mnemonic = "fabs"; break;
                    case 0x19: mnemonic = "fcosh"; break;
                    case 0x1a: mnemonic = "fneg"; break;
                    case 0x1c: mnemonic = "facos"; break;
                    case 0x1d: mnemonic = "fcos"; break;
                    case 0x1e: mnemonic = "fgetexp"; break;
                    case 0x1f: mnemonic = "fgetman"; break;
                    case 0x20: mnemonic = "fdiv"; break;
                    case 0x21: mnemonic = "fmod"; break;
                    case 0x22: mnemonic = "fadd"; break;
                    case 0x23: mnemonic = "fmul"; break;
                    case 0x24: mnemonic = "fsgldiv"; break;
                    case 0x25: mnemonic = "frem"; break;
                    case 0x26: mnemonic = "fscale"; break;
                    case 0x27: mnemonic = "fsglmul"; break;
                    case 0x28: mnemonic = "fsub"; break;
                    case 0x30:
                    case 0x31:
                    case 0x32:
                    case 0x33:
                    case 0x34:
                    case 0x35:
                    case 0x36:
                    case 0x37:
                        mnemonic = "fsincos"; break;
                    case 0x38: mnemonic = "fcmp"; break;
                    case 0x3a: mnemonic = "ftst"; break;
                    case 0x41: mnemonic = "fssqrt"; break;
                    case 0x45: mnemonic = "fdsqrt"; break;
                    case 0x58: mnemonic = "fsabs"; break;
                    case 0x5a: mnemonic = "fsneg"; break;
                    case 0x5c: mnemonic = "fdabs"; break;
                    case 0x5e: mnemonic = "fdneg"; break;
                    case 0x60: mnemonic = "fsdiv"; break;
                    case 0x62: mnemonic = "fsadd"; break;
                    case 0x63: mnemonic = "fsmul"; break;
                    case 0x64: mnemonic = "fddiv"; break;
                    case 0x66: mnemonic = "fdadd"; break;
                    case 0x67: mnemonic = "fdmul"; break;
                    case 0x68: mnemonic = "fssub"; break;
                    case 0x6c: mnemonic = "fdsub"; break;

                    default: mnemonic = "FPU (?)"; break;
                    }

                    if ((w2 & 0x4000) != 0)
                    {
                        g_dasm_str = string.Format("{0}{1}   {2},FP{3}", mnemonic, float_data_format[src], get_ea_mode_str_32(g_cpu_ir), dst_reg);
                    }
                    else
                    {
                        g_dasm_str = string.Format("{0}.x   FP{1}, FP{2}", mnemonic, src, dst_reg);
                    }
                    break;
                }

            case 0x3:
                {
                    switch ((w2 >> 10) & 7)
                    {
                    case 3:		// packed decimal w/fixed k-factor
                        g_dasm_str = string.Format("fmove{0}   FP%d, %s {#%d}", float_data_format[(w2 >> 10) & 7], dst_reg, get_ea_mode_str_32(g_cpu_ir), sext_7bit_int((int)w2 & 0x7f));
                        break;

                    case 7:		// packed decimal w/dynamic k-factor (register)
                        g_dasm_str = string.Format("fmove{0}   FP%d, %s {D%d}", float_data_format[(w2 >> 10) & 7], dst_reg, get_ea_mode_str_32(g_cpu_ir), (w2 >> 4) & 7);
                        break;

                    default:
                        g_dasm_str = string.Format("fmove{0}   FP%d, %s", float_data_format[(w2 >> 10) & 7], dst_reg, get_ea_mode_str_32(g_cpu_ir));
                        break;
                    }
                    break;
                }

            case 0x4:	// ea to control
                {
                    g_dasm_str = string.Format("fmovem.l   {0}, ", get_ea_mode_str_32(g_cpu_ir));
                    if ((w2 & 0x1000) != 0) g_dasm_str += "fpcr";
                    if ((w2 & 0x0800) != 0) g_dasm_str += "/fpsr";
                    if ((w2 & 0x0400) != 0) g_dasm_str += "/fpiar";
                    break;
                }

            case 0x5:	// control to ea
                {

                    g_dasm_str = "fmovem.l   ";
                    if ((w2 & 0x1000) != 0) g_dasm_str += "fpcr";
                    if ((w2 & 0x0800) != 0) g_dasm_str += "/fpsr";
                    if ((w2 & 0x0400) != 0) g_dasm_str += "/fpiar";
                    g_dasm_str += ", ";
                    g_dasm_str += get_ea_mode_str_32(g_cpu_ir);
                    break;
                }

            case 0x6:	// memory to FPU, list
                {
                    string temp;

                    if (((w2 >> 11) & 1) != 0)	// dynamic register list
                    {
                        g_dasm_str = string.Format("fmovem.x   {0},D{1}", get_ea_mode_str_32(g_cpu_ir), (w2 >> 4) & 7);
                    }
                    else	// static register list
                    {
                        int i;

                        g_dasm_str = string.Format("fmovem.x   {0}, ", get_ea_mode_str_32(g_cpu_ir));

                        for (i = 0; i < 8; i++)
                        {
                            if ((w2 & (1 << i)) != 0)
                            {
                                if (((w2 >> 12) & 1) != 0)	// postincrement or control
                                {
                                    temp = string.Format("FP{0} ", 7 - i);
                                }
                                else			// predecrement
                                {
                                    temp = string.Format("FP{0} ", i);
                                }
                                g_dasm_str += temp;
                            }
                        }
                    }
                    break;
                }

            case 0x7:	// FPU to memory, list
                {
                    string temp;

                    if (((w2 >> 11) & 1) != 0)	// dynamic register list
                    {
                        g_dasm_str = string.Format("fmovem.x   D{0},{1}", (w2 >> 4) & 7, get_ea_mode_str_32(g_cpu_ir));
                    }
                    else	// static register list
                    {
                        int i;

                        g_dasm_str = string.Format("fmovem.x   ");

                        for (i = 0; i < 8; i++)
                        {
                            if ((w2 & (1 << i)) != 0)
                            {
                                if (((w2 >> 12) & 1) != 0)	// postincrement or control
                                {
                                    temp = string.Format("FP{0} ", 7 - i);
                                }
                                else			// predecrement
                                {
                                    temp = string.Format("FP{0} ", i);
                                }
                                g_dasm_str += temp;
                            }
                        }

                        g_dasm_str += ", ";
                        g_dasm_str += get_ea_mode_str_32(g_cpu_ir);
                    }
                    break;
                }

            default:
                {
                    g_dasm_str = string.Format("FPU (?) ");
                    break;
                }
            }
        }

        private void d68000_jmp()
        {
            g_dasm_str = string.Format("jmp     {0}", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_jsr()
        {
            g_dasm_str = string.Format("jsr     {0}", get_ea_mode_str_32(g_cpu_ir));
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OVER);
        }

        private void d68000_lea()
        {
            g_dasm_str = string.Format("lea     {0},A{1}", get_ea_mode_str_32(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_link_16()
        {
            g_dasm_str = string.Format("link    A{0},{1}", g_cpu_ir & 7, get_imm_str_s16());
        }

        private void d68020_link_32()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("link    A{0},{1}; (2+)", g_cpu_ir & 7, get_imm_str_s32());
        }

        private void d68000_lsr_s_8()
        {
            g_dasm_str = string.Format("lsr.b   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_lsr_s_16()
        {
            g_dasm_str = string.Format("lsr.w   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_lsr_s_32()
        {
            g_dasm_str = string.Format("lsr.l   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_lsr_r_8()
        {
            g_dasm_str = string.Format("lsr.b   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_lsr_r_16()
        {
            g_dasm_str = string.Format("lsr.w   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_lsr_r_32()
        {
            g_dasm_str = string.Format("lsr.l   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_lsr_ea()
        {
            g_dasm_str = string.Format("lsr.w   {0}", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_lsl_s_8()
        {
            g_dasm_str = string.Format("lsl.b   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_lsl_s_16()
        {
            g_dasm_str = string.Format("lsl.w   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_lsl_s_32()
        {
            g_dasm_str = string.Format("lsl.l   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_lsl_r_8()
        {
            g_dasm_str = string.Format("lsl.b   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_lsl_r_16()
        {
            g_dasm_str = string.Format("lsl.w   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_lsl_r_32()
        {
            g_dasm_str = string.Format("lsl.l   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_lsl_ea()
        {
            g_dasm_str = string.Format("lsl.w   {0}", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_move_8()
        {
            string str = get_ea_mode_str_8(g_cpu_ir);
            g_dasm_str = string.Format("move.b  {0},{1}", str, get_ea_mode_str_8(((g_cpu_ir >> 9) & 7) | ((g_cpu_ir >> 3) & 0x38)));
        }

        private void d68000_move_16()
        {
            string str = get_ea_mode_str_16(g_cpu_ir);
            g_dasm_str = string.Format("move.w  {0},{1}", str, get_ea_mode_str_16(((g_cpu_ir >> 9) & 7) | ((g_cpu_ir >> 3) & 0x38)));
        }

        private void d68000_move_32()
        {
            string str = get_ea_mode_str_32(g_cpu_ir);
            g_dasm_str = string.Format("move.l  {0},{1}", str, get_ea_mode_str_32(((g_cpu_ir >> 9) & 7) | ((g_cpu_ir >> 3) & 0x38)));
        }

        private void d68000_movea_16()
        {
            g_dasm_str = string.Format("movea.w {0},A{1}", get_ea_mode_str_16(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_movea_32()
        {
            g_dasm_str = string.Format("movea.l {0},A{1}", get_ea_mode_str_32(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_move_to_ccr()
        {
            g_dasm_str = string.Format("move    {0}, CCR", get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68010_move_fr_ccr()
        {
            LIMIT_CPU_TYPES(M68010_PLUS);
            g_dasm_str = string.Format("move    CCR, {0}; (1+)", get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_move_fr_sr()
        {
            g_dasm_str = string.Format("move    SR, {0}", get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_move_to_sr()
        {
            g_dasm_str = string.Format("move    {0}, SR", get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_move_fr_usp()
        {
            g_dasm_str = string.Format("move    USP, A{0}", g_cpu_ir & 7);
        }

        private void d68000_move_to_usp()
        {
            g_dasm_str = string.Format("move    A{0}, USP", g_cpu_ir & 7);
        }

        private void d68010_movec()
        {
            uint extension;
            string reg_name;
            string processor;
            LIMIT_CPU_TYPES(M68010_PLUS);
            extension = read_imm_16();

            switch (extension & 0xfff)
            {
            case 0x000:
                reg_name = "SFC";
                processor = "1+";
                break;
            case 0x001:
                reg_name = "DFC";
                processor = "1+";
                break;
            case 0x800:
                reg_name = "USP";
                processor = "1+";
                break;
            case 0x801:
                reg_name = "VBR";
                processor = "1+";
                break;
            case 0x002:
                reg_name = "CACR";
                processor = "2+";
                break;
            case 0x802:
                reg_name = "CAAR";
                processor = "2,3";
                break;
            case 0x803:
                reg_name = "MSP";
                processor = "2+";
                break;
            case 0x804:
                reg_name = "ISP";
                processor = "2+";
                break;
            case 0x003:
                reg_name = "TC";
                processor = "4+";
                break;
            case 0x004:
                reg_name = "ITT0";
                processor = "4+";
                break;
            case 0x005:
                reg_name = "ITT1";
                processor = "4+";
                break;
            case 0x006:
                reg_name = "DTT0";
                processor = "4+";
                break;
            case 0x007:
                reg_name = "DTT1";
                processor = "4+";
                break;
            case 0x805:
                reg_name = "MMUSR";
                processor = "4+";
                break;
            case 0x806:
                reg_name = "URP";
                processor = "4+";
                break;
            case 0x807:
                reg_name = "SRP";
                processor = "4+";
                break;
            default:
                reg_name = make_signed_hex_str_16(extension & 0xfff);
                processor = "?";
                break;
            }

            if (BIT_0(g_cpu_ir))
                g_dasm_str = string.Format("movec {0}%d, %s; (%s)", BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7, reg_name, processor);
            else
                g_dasm_str = string.Format("movec {0}, %c%d; (%s)", reg_name, BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7, processor);
        }

        private void d68000_movem_pd_16()
        {
            uint data = read_imm_16();
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
                    if (buffer[0] != 0)
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
                    if (buffer[0] != 0)
                        buffer.Append("/");
                    buffer.AppendFormat("A{0}", first);
                    if (run_length > 0)
                        buffer.AppendFormat("-A{0}", first + run_length);
                }
            }
            g_dasm_str = string.Format("movem.w {0},{1}", buffer, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_movem_pd_32()
        {
            uint data = read_imm_16();
            var buffer = new StringBuilder();
            int first;
            uint run_length;

            for (int i = 0; i < 8; i++)
            {
                if ((data & (1 << (15 - i)))!=0)
                {
                    first = i;
                    run_length = 0;
                    while (i < 7 && (data & (1 << (15 - (i + 1))))!=0)
                    {
                        i++;
                        run_length++;
                    }
                    if (buffer[0] != 0)
                        buffer.Append("/");
                    buffer.AppendFormat("D{0}", first);
                    if (run_length > 0)
                        buffer.AppendFormat("-D{0}", first + run_length);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if ((data & (1 << (7 - i)))!=0)
                {
                    first = i;
                    run_length = 0;
                    while (i < 7 && (data & (1 << (7 - (i + 1))))!=0)
                    {
                        i++;
                        run_length++;
                    }
                    if (buffer[0] != 0)
                        buffer.Append("/");
                    buffer.AppendFormat("A{0}", first);
                    if (run_length > 0)
                        buffer.AppendFormat("-A{0}", first + run_length);
                }
            }
            g_dasm_str = string.Format("movem.l {0},{1}", buffer, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_movem_er_16()
        {
            uint data = read_imm_16();
            var buffer = new StringBuilder();
            int first;
            uint run_length;

            for (int i = 0; i < 8; i++)
            {
                if ((data & (1 << i))!=0)
                {
                    first = i;
                    run_length = 0;
                    while (i < 7 && (data & (1 << (i + 1)))!=0)
                    {
                        i++;
                        run_length++;
                    }
                    if (buffer[0] != 0)
                        buffer.Append("/");
                    buffer.AppendFormat("D{0}", first);
                    if (run_length > 0)
                        buffer.AppendFormat("-D{0}", first + run_length);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if ((data & (1 << (i + 8)))!=0)
                {
                    first = i;
                    run_length = 0;
                    while (i < 7 && (data & (1 << (i + 8 + 1)))!=0)
                    {
                        i++;
                        run_length++;
                    }
                    if (buffer[0] != 0)
                        buffer.Append("/");
                    buffer.AppendFormat("A{0}", first);
                    if (run_length > 0)
                        buffer.AppendFormat("-A{0}", first + run_length);
                }
            }
            g_dasm_str = string.Format("movem.w {0}, {1}", get_ea_mode_str_16(g_cpu_ir), buffer);
        }

        private void d68000_movem_er_32()
        {
            uint data = read_imm_16();
            var buffer = new StringBuilder();
            int first;
            uint run_length;

            for (int i = 0; i < 8; i++)
            {
                if ((data & (1 << i))!=0)
                {
                    first = i;
                    run_length = 0;
                    while (i < 7 && (data & (1 << (i + 1)))!=0)
                    {
                        i++;
                        run_length++;
                    }
                    if (buffer[0] != 0)
                        buffer.Append("/");
                    buffer.AppendFormat("D{0}", first);
                    if (run_length > 0)
                        buffer.AppendFormat("-D{0}", first + run_length);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if ((data & (1 << (i + 8)))!=0)
                {
                    first = i;
                    run_length = 0;
                    while (i < 7 && (data & (1 << (i + 8 + 1)))!=0)
                    {
                        i++;
                        run_length++;
                    }
                    if (buffer[0] != 0)
                        buffer.Append("/");
                    buffer.AppendFormat("A{0}", first);
                    if (run_length > 0)
                        buffer.AppendFormat("-A{0}", first + run_length);
                }
            }
            g_dasm_str = string.Format("movem.l {0},{1}", get_ea_mode_str_32(g_cpu_ir), buffer);
        }

        private void d68000_movem_re_16()
        {
            uint data = read_imm_16();
            var buffer = new StringBuilder();
            WriteRegisterSet(data, 0, 1, "D", buffer);
            WriteRegisterSet(data, 8, 1, "A", buffer);
            g_dasm_str = string.Format("movem.w {0},{1}", buffer, get_ea_mode_str_16(g_cpu_ir));
        }

        public static bool bit(uint word, int pos)
        {
            return (word & (1u << pos)) != 0;
        }

        public void WriteRegisterSet(uint data, int bitPos, int incr, string regType, StringBuilder buffer)
        {
            string sep = "";
            for (int i = 0; i < 8; i++, bitPos += incr)
            {
                if (bit(data, bitPos))
                {
                    int first = i;
                    int run_length = 0;
                    while (i < 7 && bit(data, bitPos+incr))
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

        private void d68000_movem_re_32()
        {
            uint data = read_imm_16();
            var buffer = new StringBuilder();
            WriteRegisterSet(data, 0, 1, "D", buffer);
            WriteRegisterSet(data, 8, 1, "A", buffer);
            g_dasm_str = string.Format("movem.l {0},{1}", buffer, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_movep_re_16()
        {
            g_dasm_str = string.Format("movep.w D{0}, ($%x,A%d)", (g_cpu_ir >> 9) & 7, read_imm_16(), g_cpu_ir & 7);
        }

        private void d68000_movep_re_32()
        {
            g_dasm_str = string.Format("movep.l D{0}, ($%x,A%d)", (g_cpu_ir >> 9) & 7, read_imm_16(), g_cpu_ir & 7);
        }

        private void d68000_movep_er_16()
        {
            g_dasm_str = string.Format("movep.w (${0},A%d),D{1}", read_imm_16(), g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_movep_er_32()
        {
            g_dasm_str = string.Format("movep.l (${0},A%d),D{1}", read_imm_16(), g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68010_moves_8()
        {
            uint extension;
            LIMIT_CPU_TYPES(M68010_PLUS);
            extension = read_imm_16();
            if (BIT_B(extension))
                g_dasm_str = string.Format("moves.b {0}{1},{2}; (1+)", BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7, get_ea_mode_str_8(g_cpu_ir));
            else
                g_dasm_str = string.Format("moves.b {0},{1}{2}; (1+)", get_ea_mode_str_8(g_cpu_ir), BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7);
        }

        private void d68010_moves_16()
        {
            uint extension;
            LIMIT_CPU_TYPES(M68010_PLUS);
            extension = read_imm_16();
            if (BIT_B(extension))
                g_dasm_str = string.Format("moves.w {0}{1},{2}; (1+)", BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7, get_ea_mode_str_16(g_cpu_ir));
            else
                g_dasm_str = string.Format("moves.w {0},{1}{2}; (1+)", get_ea_mode_str_16(g_cpu_ir), BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7);
        }

        private void d68010_moves_32()
        {
            uint extension;
            LIMIT_CPU_TYPES(M68010_PLUS);
            extension = read_imm_16();
            if (BIT_B(extension))
                g_dasm_str = string.Format("moves.l {0}%d, %s; (1+)", BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7, get_ea_mode_str_32(g_cpu_ir));
            else
                g_dasm_str = string.Format("moves.l {0}, %c%d; (1+)", get_ea_mode_str_32(g_cpu_ir), BIT_F(extension) ? 'A' : 'D', (extension >> 12) & 7);
        }

        private void d68000_moveq()
        {
            g_dasm_str = string.Format("moveq   #{0}, D{1}", make_signed_hex_str_8(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68040_move16_pi_pi()
        {
            LIMIT_CPU_TYPES(M68040_PLUS);
            g_dasm_str = string.Format("move16  (A{0})+, (A{1})+; (4)", g_cpu_ir & 7, (read_imm_16() >> 12) & 7);
        }

        private void d68040_move16_pi_al()
        {
            LIMIT_CPU_TYPES(M68040_PLUS);
            g_dasm_str = string.Format("move16  (A{0})+,{1}; (4)", g_cpu_ir & 7, get_imm_str_u32());
        }

        private void d68040_move16_al_pi()
        {
            LIMIT_CPU_TYPES(M68040_PLUS);
            g_dasm_str = string.Format("move16  {0}, (A{1})+; (4)", get_imm_str_u32(), g_cpu_ir & 7);
        }

        private void d68040_move16_ai_al()
        {
            LIMIT_CPU_TYPES(M68040_PLUS);
            g_dasm_str = string.Format("move16  (A{0}),{1}; (4)", g_cpu_ir & 7, get_imm_str_u32());
        }

        private void d68040_move16_al_ai()
        {
            LIMIT_CPU_TYPES(M68040_PLUS);
            g_dasm_str = string.Format("move16  {0}, (A{1}); (4)", get_imm_str_u32(), g_cpu_ir & 7);
        }

        private void d68000_muls()
        {
            g_dasm_str = string.Format("muls.w  {0}, D{1}", get_ea_mode_str_16(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_mulu()
        {
            g_dasm_str = string.Format("mulu.w  {0}, D{1} ", get_ea_mode_str_16(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68020_mull()
        {
            uint extension;
            LIMIT_CPU_TYPES(M68020_PLUS);
            extension = read_imm_16();

            if (BIT_A(extension))
                g_dasm_str = string.Format("mul{0}.l %s, D%d-D%d; (2+)", BIT_B(extension) ? 's' : 'u', get_ea_mode_str_32(g_cpu_ir), extension & 7, (extension >> 12) & 7);
            else
                g_dasm_str = string.Format("mul{0}.l  %s, D%d; (2+)", BIT_B(extension) ? 's' : 'u', get_ea_mode_str_32(g_cpu_ir), (extension >> 12) & 7);
        }

        private void d68000_nbcd()
        {
            g_dasm_str = string.Format("nbcd    {0}", get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_neg_8()
        {
            g_dasm_str = string.Format("neg.b   {0}", get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_neg_16()
        {
            g_dasm_str = string.Format("neg.w   {0}", get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_neg_32()
        {
            g_dasm_str = string.Format("neg.l   {0}", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_negx_8()
        {
            g_dasm_str = string.Format("negx.b  {0}", get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_negx_16()
        {
            g_dasm_str = string.Format("negx.w  {0}", get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_negx_32()
        {
            g_dasm_str = string.Format("negx.l  {0}", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_nop()
        {
            g_dasm_str = string.Format("nop");
        }

        private void d68000_not_8()
        {
            g_dasm_str = string.Format("not.b   {0}", get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_not_16()
        {
            g_dasm_str = string.Format("not.w   {0}", get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_not_32()
        {
            g_dasm_str = string.Format("not.l   {0}", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_or_er_8()
        {
            g_dasm_str = string.Format("or.b    {0},D{1}", get_ea_mode_str_8(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_or_er_16()
        {
            g_dasm_str = string.Format("or.w    {0},D{1}", get_ea_mode_str_16(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_or_er_32()
        {
            g_dasm_str = string.Format("or.l    {0},D{1}", get_ea_mode_str_32(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_or_re_8()
        {
            g_dasm_str = string.Format("or.b    D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_or_re_16()
        {
            g_dasm_str = string.Format("or.w    D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_or_re_32()
        {
            g_dasm_str = string.Format("or.l    D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_ori_8()
        {
            string str = get_imm_str_u8();
            g_dasm_str = string.Format("ori.b   {0},{1}", str, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_ori_16()
        {
            string str = get_imm_str_u16();
            g_dasm_str = string.Format("ori.w   {0},{1}", str, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_ori_32()
        {
            string str = get_imm_str_u32();
            g_dasm_str = string.Format("ori.l   {0},{1}", str, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_ori_to_ccr()
        {
            g_dasm_str = string.Format("ori     {0}, CCR", get_imm_str_u8());
        }

        private void d68000_ori_to_sr()
        {
            g_dasm_str = string.Format("ori     {0}, SR", get_imm_str_u16());
        }

        private void d68020_pack_rr()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("pack    D{0},D{1},{1}; (2+)", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7, get_imm_str_u16());
        }

        private void d68020_pack_mm()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("pack    -(A{0}), -(A%d), %s; (2+)", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7, get_imm_str_u16());
        }

        private void d68000_pea()
        {
            g_dasm_str = string.Format("pea     {0}", get_ea_mode_str_32(g_cpu_ir));
        }

        // this is a 68040-specific form of PFLUSH
        private void d68040_pflush()
        {
            LIMIT_CPU_TYPES(M68040_PLUS);

            if ((g_cpu_ir & 0x10) != 0)
            {
                g_dasm_str = string.Format("pflusha{0}", (g_cpu_ir & 8)!=0 ? "" : "n");
            }
            else
            {
                g_dasm_str = string.Format("pflush{0}(A%d)", (g_cpu_ir & 8)!=0 ? "" : "n", g_cpu_ir & 7);
            }
        }

        private void d68000_reset()
        {
            g_dasm_str = string.Format("reset");
        }

        private void d68000_ror_s_8()
        {
            g_dasm_str = string.Format("ror.b   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_ror_s_16()
        {
            g_dasm_str = string.Format("ror.w   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_ror_s_32()
        {
            g_dasm_str = string.Format("ror.l   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_ror_r_8()
        {
            g_dasm_str = string.Format("ror.b   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_ror_r_16()
        {
            g_dasm_str = string.Format("ror.w   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_ror_r_32()
        {
            g_dasm_str = string.Format("ror.l   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_ror_ea()
        {
            g_dasm_str = string.Format("ror.w   {0}", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_rol_s_8()
        {
            g_dasm_str = string.Format("rol.b   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_rol_s_16()
        {
            g_dasm_str = string.Format("rol.w   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_rol_s_32()
        {
            g_dasm_str = string.Format("rol.l   #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_rol_r_8()
        {
            g_dasm_str = string.Format("rol.b   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_rol_r_16()
        {
            g_dasm_str = string.Format("rol.w   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_rol_r_32()
        {
            g_dasm_str = string.Format("rol.l   D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_rol_ea()
        {
            g_dasm_str = string.Format("rol.w   {0}", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_roxr_s_8()
        {
            g_dasm_str = string.Format("roxr.b  #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_roxr_s_16()
        {
            g_dasm_str = string.Format("roxr.w  #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }


        private void d68000_roxr_s_32()
        {
            g_dasm_str = string.Format("roxr.l  #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_roxr_r_8()
        {
            g_dasm_str = string.Format("roxr.b  D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_roxr_r_16()
        {
            g_dasm_str = string.Format("roxr.w  D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_roxr_r_32()
        {
            g_dasm_str = string.Format("roxr.l  D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_roxr_ea()
        {
            g_dasm_str = string.Format("roxr.w  {0}", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_roxl_s_8()
        {
            g_dasm_str = string.Format("roxl.b  #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_roxl_s_16()
        {
            g_dasm_str = string.Format("roxl.w  #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_roxl_s_32()
        {
            g_dasm_str = string.Format("roxl.l  #{0},D{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], g_cpu_ir & 7);
        }

        private void d68000_roxl_r_8()
        {
            g_dasm_str = string.Format("roxl.b  D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_roxl_r_16()
        {
            g_dasm_str = string.Format("roxl.w  D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_roxl_r_32()
        {
            g_dasm_str = string.Format("roxl.l  D{0},D{1}", (g_cpu_ir >> 9) & 7, g_cpu_ir & 7);
        }

        private void d68000_roxl_ea()
        {
            g_dasm_str = string.Format("roxl.w  {0}", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68010_rtd()
        {
            LIMIT_CPU_TYPES(M68010_PLUS);
            g_dasm_str = string.Format("rtd     {0}; (1+)", get_imm_str_s16());
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OUT);
        }

        private void d68000_rte()
        {
            g_dasm_str = string.Format("rte");
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OUT);
        }

        private void d68020_rtm()
        {
            LIMIT_CPU_TYPES(M68020_ONLY);
            g_dasm_str = string.Format("rtm     {0}%d; (2+)", BIT_3(g_cpu_ir) ? 'A' : 'D', g_cpu_ir & 7);
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OUT);
        }

        private void d68000_rtr()
        {
            g_dasm_str = string.Format("rtr");
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OUT);
        }

        private void d68000_rts()
        {
            g_dasm_str = string.Format("rts");
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OUT);
        }

        private void d68000_sbcd_rr()
        {
            g_dasm_str = string.Format("sbcd    D{0},D{1}", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_sbcd_mm()
        {
            g_dasm_str = string.Format("sbcd    -(A{0}), -(A%d)", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_scc()
        {
            g_dasm_str = string.Format("s%-2s     %s", g_cc[(g_cpu_ir >> 8) & 0xf], get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_stop()
        {
            g_dasm_str = string.Format("stop    {0}", get_imm_str_s16());
        }

        private void d68000_sub_er_8()
        {
            g_dasm_str = string.Format("sub.b   {0},D{1}", get_ea_mode_str_8(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_sub_er_16()
        {
            g_dasm_str = string.Format("sub.w   {0},D{1}", get_ea_mode_str_16(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_sub_er_32()
        {
            g_dasm_str = string.Format("sub.l   {0},D{1}", get_ea_mode_str_32(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_sub_re_8()
        {
            g_dasm_str = string.Format("sub.b   D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_sub_re_16()
        {
            g_dasm_str = string.Format("sub.w   D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_sub_re_32()
        {
            g_dasm_str = string.Format("sub.l   D{0},{1}", (g_cpu_ir >> 9) & 7, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_suba_16()
        {
            g_dasm_str = string.Format("suba.w  {0},A{1}", get_ea_mode_str_16(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_suba_32()
        {
            g_dasm_str = string.Format("suba.l  {0},A{1}", get_ea_mode_str_32(g_cpu_ir), (g_cpu_ir >> 9) & 7);
        }

        private void d68000_subi_8()
        {
            string str = get_imm_str_s8();
            g_dasm_str = string.Format("subi.b  {0},{1}", str, get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_subi_16()
        {
            string str = get_imm_str_s16();
            g_dasm_str = string.Format("subi.w  {0},{1}", str, get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_subi_32()
        {
            string str = get_imm_str_s32();
            g_dasm_str = string.Format("subi.l  {0},{1}", str, get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_subq_8()
        {
            g_dasm_str = string.Format("subq.b  #{0},{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_subq_16()
        {
            g_dasm_str = string.Format("subq.w  #{0},{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_subq_32()
        {
            g_dasm_str = string.Format("subq.l  #{0},{1}", g_3bit_qdata_table[(g_cpu_ir >> 9) & 7], get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_subx_rr_8()
        {
            g_dasm_str = string.Format("subx.b  D{0},D{1}", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_subx_rr_16()
        {
            g_dasm_str = string.Format("subx.w  D{0},D{1}", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_subx_rr_32()
        {
            g_dasm_str = string.Format("subx.l  D{0},D{1}", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_subx_mm_8()
        {
            g_dasm_str = string.Format("subx.b  -(A{0}), -(A%d)", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_subx_mm_16()
        {
            g_dasm_str = string.Format("subx.w  -(A{0}), -(A%d)", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_subx_mm_32()
        {
            g_dasm_str = string.Format("subx.l  -(A{0}), -(A%d)", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7);
        }

        private void d68000_swap()
        {
            g_dasm_str = string.Format("swap    D{0}", g_cpu_ir & 7);
        }

        private void d68000_tas()
        {
            g_dasm_str = string.Format("tas     {0}", get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_trap()
        {
            g_dasm_str = string.Format("trap    #${0}", g_cpu_ir & 0xf);
        }

        private void d68020_trapcc_0()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("trap%-2s; (2+)", g_cc[(g_cpu_ir >> 8) & 0xf]);
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OVER);
        }

        private void d68020_trapcc_16()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("trap%-2s  %s; (2+)", g_cc[(g_cpu_ir >> 8) & 0xf], get_imm_str_u16());
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OVER);
        }

        private void d68020_trapcc_32()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("trap%-2s  %s; (2+)", g_cc[(g_cpu_ir >> 8) & 0xf], get_imm_str_u32());
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OVER);
        }

        private void d68000_trapv()
        {
            g_dasm_str = string.Format("trapv");
            SET_OPCODE_FLAGS(DASMFLAG_STEP_OVER);
        }

        private void d68000_tst_8()
        {
            g_dasm_str = string.Format("tst.b   {0}", get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68020_tst_pcdi_8()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("tst.b   {0}; (2+)", get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68020_tst_pcix_8()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("tst.b   {0}; (2+)", get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68020_tst_i_8()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("tst.b   {0}; (2+)", get_ea_mode_str_8(g_cpu_ir));
        }

        private void d68000_tst_16()
        {
            g_dasm_str = string.Format("tst.w   {0}", get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68020_tst_a_16()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("tst.w   {0}; (2+)", get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68020_tst_pcdi_16()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("tst.w   {0}; (2+)", get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68020_tst_pcix_16()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("tst.w   {0}; (2+)", get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68020_tst_i_16()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("tst.w   {0}; (2+)", get_ea_mode_str_16(g_cpu_ir));
        }

        private void d68000_tst_32()
        {
            g_dasm_str = string.Format("tst.l   {0}", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68020_tst_a_32()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("tst.l   {0}; (2+)", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68020_tst_pcdi_32()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("tst.l   {0}; (2+)", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68020_tst_pcix_32()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("tst.l   {0}; (2+)", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68020_tst_i_32()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("tst.l   {0}; (2+)", get_ea_mode_str_32(g_cpu_ir));
        }

        private void d68000_unlk()
        {
            g_dasm_str = string.Format("unlk    A{0}", g_cpu_ir & 7);
        }

        private void d68020_unpk_rr()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("unpk    D{0},D{1}, %s; (2+)", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7, get_imm_str_u16());
        }

        private void d68020_unpk_mm()
        {
            LIMIT_CPU_TYPES(M68020_PLUS);
            g_dasm_str = string.Format("unpk    -(A{0}), -(A%d), %s; (2+)", g_cpu_ir & 7, (g_cpu_ir >> 9) & 7, get_imm_str_u16());
        }


        // PFLUSH:  001xxx0xxxxxxxxx
        // PLOAD:   001000x0000xxxxx
        // PVALID1: 0010100000000000
        // PVALID2: 0010110000000xxx
        // PMOVE 1: 010xxxx000000000
        // PMOVE 2: 011xxxx0000xxx00
        // PMOVE 3: 011xxxx000000000
        // PTEST:   100xxxxxxxxxxxxx
        // PFLUSHR:  1010000000000000
        private void d68851_p000()
        {
            string str;
            ushort modes = read_imm_16();

            // do this after fetching the second PMOVE word so we properly get the 3rd if necessary
            str = get_ea_mode_str_32(g_cpu_ir);

            if ((modes & 0xfde0) == 0x2000)	// PLOAD
            {
                if ((modes & 0x0200) !=0)
                {
                    g_dasm_str = string.Format("pload  #{0},{1}", (modes >> 10) & 7, str);
                }
                else
                {
                    g_dasm_str = string.Format("pload  {0}, #%d", str, (modes >> 10) & 7);
                }
                return;
            }

            if ((modes & 0xe200) == 0x2000)	// PFLUSH
            {
                g_dasm_str = string.Format("pflushr {0}, %x, %s", modes & 0x1f, (modes >> 5) & 0xf, str);
                return;
            }

            if (modes == 0xa000)	// PFLUSHR
            {
                g_dasm_str = string.Format("pflushr {0}", str);
            }

            if (modes == 0x2800)	// PVALID (FORMAT 1)
            {
                g_dasm_str = string.Format("pvalid VAL, {0}", str);
                return;
            }

            if ((modes & 0xfff8) == 0x2c00)	// PVALID (FORMAT 2)
            {
                g_dasm_str = string.Format("pvalid A{0},{1}", modes & 0xf, str);
                return;
            }

            if ((modes & 0xe000) == 0x8000)	// PTEST
            {
                g_dasm_str = string.Format("ptest #{0},{1}", modes & 0x1f, str);
                return;
            }

            switch ((modes >> 13) & 0x7)
            {
            case 0:	// MC68030/040 form with FD bit
            case 2:	// MC68881 form, FD never set
                if ((modes & 0x0100)!=0)
                {
                    if ((modes & 0x0200)!=0)
                    {
                        g_dasm_str = string.Format("pmovefd  {0},{1}", g_mmuregs[(modes >> 10) & 7], str);
                    }
                    else
                    {
                        g_dasm_str = string.Format("pmovefd  {0},{1}", str, g_mmuregs[(modes >> 10) & 7]);
                    }
                }
                else
                {
                    if ((modes & 0x0200)!=0)
                    {
                        g_dasm_str = string.Format("pmove  {0},{1}", g_mmuregs[(modes >> 10) & 7], str);
                    }
                    else
                    {
                        g_dasm_str = string.Format("pmove  {0},{1}", str, g_mmuregs[(modes >> 10) & 7]);
                    }
                }
                break;

            case 3:	// MC68030 to/from status reg
                if ((modes & 0x0200)!=0)
                {
                    g_dasm_str = string.Format("pmove  mmusr, {0}", str);
                }
                else
                {
                    g_dasm_str = string.Format("pmove  {0}, mmusr", str);
                }
                break;

            default:
                g_dasm_str = string.Format("pmove [unknown form] {0}", str);
                break;
            }
        }

        private void d68851_pbcc16()
        {
            uint temp_pc = g_cpu_pc.Address.Linear;
            g_dasm_str = string.Format("pb{0} %x", g_mmucond[g_cpu_ir & 0xf], temp_pc + make_int_16(read_imm_16()));
        }

        private void d68851_pbcc32()
        {
            uint temp_pc = g_cpu_pc.Address.Linear;
            g_dasm_str = string.Format("pb{0} %x", g_mmucond[g_cpu_ir & 0xf], temp_pc + make_int_32(read_imm_32()));
        }

        private void d68851_pdbcc()
        {
            uint temp_pc = g_cpu_pc.Address.Linear;
            ushort modes = read_imm_16();
            g_dasm_str = string.Format("pb{0} %x", g_mmucond[modes & 0xf], temp_pc + make_int_16(read_imm_16()));
        }

        // PScc:  0000000000xxxxxx
        private void d68851_p001()
        {
            g_dasm_str = string.Format("MMU 001 group");
        }

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

        static opcode_struct[] g_opcode_info;

        public M68kDisassembler2(ImageReader rdr)
        {
            GenTable();
            this.g_cpu_pc = rdr;
        }

        private void GenTable()
        {
            if (g_opcode_info != null)
                return;
         g_opcode_info = new opcode_struct[] 
{
//  opcode handler             mask    match   ea mask 
	new opcode_struct(d68000_1010         , 0xf000, 0xa000, 0x000),
	new opcode_struct(d68000_1111         , 0xf000, 0xf000, 0x000),
	new opcode_struct(d68000_abcd_rr      , 0xf1f8, 0xc100, 0x000),
	new opcode_struct(d68000_abcd_mm      , 0xf1f8, 0xc108, 0x000),
	new opcode_struct(0xf1c0, 0xd000, 0xbff, Opcode.add, "s6:E0,D9"),           // d68000_add_er_8   
	new opcode_struct(d68000_add_er_16    , 0xf1c0, 0xd040, 0xfff),
	new opcode_struct(d68000_add_er_32    , 0xf1c0, 0xd080, 0xfff),
	new opcode_struct(0xf1c0, 0xd100, 0x3f8, Opcode.add, "D9,s6:E0"),           // d68000_add_re_8     
	new opcode_struct(d68000_add_re_16    , 0xf1c0, 0xd140, 0x3f8),
	new opcode_struct(d68000_add_re_32    , 0xf1c0, 0xd180, 0x3f8),
	new opcode_struct(0xf1c0, 0xd0c0, 0xfff, Opcode.adda, "sw:E0,A9"),
	new opcode_struct(0xf1c0, 0xd1c0, 0xfff, Opcode.adda, "sw:E0,A9"),
	new opcode_struct(d68000_addi_8       , 0xffc0, 0x0600, 0xbf8),
	new opcode_struct(d68000_addi_16      , 0xffc0, 0x0640, 0xbf8),
	new opcode_struct(d68000_addi_32      , 0xffc0, 0x0680, 0xbf8),
	new opcode_struct(0xf1c0, 0x5000, 0xbf8, Opcode.addq, "s6:q9,E0"),          // d68000_addq_8       , 
	new opcode_struct(0xf1c0, 0x5040, 0xff8, Opcode.addq, "s6:q9,E0"),          // d68000_addq_16      , 
	new opcode_struct(0xf1c0, 0x5080, 0xff8, Opcode.addq, "s6:q9,E0"),          // d68000_addq_32      , 
	new opcode_struct(d68000_addx_rr_8    , 0xf1f8, 0xd100, 0x000),
	new opcode_struct(d68000_addx_rr_16   , 0xf1f8, 0xd140, 0x000),
	new opcode_struct(d68000_addx_rr_32   , 0xf1f8, 0xd180, 0x000),
	new opcode_struct(d68000_addx_mm_8    , 0xf1f8, 0xd108, 0x000),
	new opcode_struct(d68000_addx_mm_16   , 0xf1f8, 0xd148, 0x000),
	new opcode_struct(d68000_addx_mm_32   , 0xf1f8, 0xd188, 0x000),
	new opcode_struct(d68000_and_er_8     , 0xf1c0, 0xc000, 0xbff),
	new opcode_struct(d68000_and_er_16    , 0xf1c0, 0xc040, 0xbff),
	new opcode_struct(d68000_and_er_32    , 0xf1c0, 0xc080, 0xbff),
	new opcode_struct(d68000_and_re_8     , 0xf1c0, 0xc100, 0x3f8),
	new opcode_struct(d68000_and_re_16    , 0xf1c0, 0xc140, 0x3f8),
	new opcode_struct(d68000_and_re_32    , 0xf1c0, 0xc180, 0x3f8),
	new opcode_struct(d68000_andi_to_ccr  , 0xffff, 0x023c, 0x000),
	new opcode_struct(d68000_andi_to_sr   , 0xffff, 0x027c, 0x000),
	new opcode_struct(d68000_andi_8       , 0xffc0, 0x0200, 0xbf8),
	new opcode_struct(d68000_andi_16      , 0xffc0, 0x0240, 0xbf8),
	new opcode_struct(d68000_andi_32      , 0xffc0, 0x0280, 0xbf8),
	new opcode_struct(d68000_asr_s_8      , 0xf1f8, 0xe000, 0x000),
	new opcode_struct(d68000_asr_s_16     , 0xf1f8, 0xe040, 0x000),
	new opcode_struct(d68000_asr_s_32     , 0xf1f8, 0xe080, 0x000),
	new opcode_struct(d68000_asr_r_8      , 0xf1f8, 0xe020, 0x000),
	new opcode_struct(d68000_asr_r_16     , 0xf1f8, 0xe060, 0x000),
	new opcode_struct(d68000_asr_r_32     , 0xf1f8, 0xe0a0, 0x000),
	new opcode_struct(d68000_asr_ea       , 0xffc0, 0xe0c0, 0x3f8),
	new opcode_struct(d68000_asl_s_8      , 0xf1f8, 0xe100, 0x000),
	new opcode_struct(d68000_asl_s_16     , 0xf1f8, 0xe140, 0x000),
	new opcode_struct(d68000_asl_s_32     , 0xf1f8, 0xe180, 0x000),
	new opcode_struct(d68000_asl_r_8      , 0xf1f8, 0xe120, 0x000),
	new opcode_struct(d68000_asl_r_16     , 0xf1f8, 0xe160, 0x000),
	new opcode_struct(d68000_asl_r_32     , 0xf1f8, 0xe1a0, 0x000),
	new opcode_struct(d68000_asl_ea       , 0xffc0, 0xe1c0, 0x3f8),
	new opcode_struct(d68000_bcc_8        , 0xf000, 0x6000, 0x000),
	new opcode_struct(d68000_bcc_16       , 0xf0ff, 0x6000, 0x000),
	new opcode_struct(d68020_bcc_32       , 0xf0ff, 0x60ff, 0x000),
	new opcode_struct(0xf1c0, 0x0140, 0xbf8, Opcode.bchg, "D9,E0"),          // d68000_bchg_r       
	new opcode_struct(d68000_bchg_s       , 0xffc0, 0x0840, 0xbf8),
	new opcode_struct(d68000_bclr_r       , 0xf1c0, 0x0180, 0xbf8),
	new opcode_struct(d68000_bclr_s       , 0xffc0, 0x0880, 0xbf8),
	new opcode_struct(d68020_bfchg        , 0xffc0, 0xeac0, 0xa78),
	new opcode_struct(d68020_bfclr        , 0xffc0, 0xecc0, 0xa78),
	new opcode_struct(d68020_bfexts       , 0xffc0, 0xebc0, 0xa7b),
	new opcode_struct(d68020_bfextu       , 0xffc0, 0xe9c0, 0xa7b),
	new opcode_struct(d68020_bfffo        , 0xffc0, 0xedc0, 0xa7b),
	new opcode_struct(d68020_bfins        , 0xffc0, 0xefc0, 0xa78),
	new opcode_struct(d68020_bfset        , 0xffc0, 0xeec0, 0xa78),
	new opcode_struct(d68020_bftst        , 0xffc0, 0xe8c0, 0xa7b),
	new opcode_struct(d68010_bkpt         , 0xfff8, 0x4848, 0x000),
	new opcode_struct(0xff00, 0x6000, 0x000, Opcode.bra, "J"),              // d68000_bra_8        
	new opcode_struct(d68000_bra_16       , 0xffff, 0x6000, 0x000),
	new opcode_struct(d68020_bra_32       , 0xffff, 0x60ff, 0x000),
	new opcode_struct(d68000_bset_r       , 0xf1c0, 0x01c0, 0xbf8),
	new opcode_struct(d68000_bset_s       , 0xffc0, 0x08c0, 0xbf8),
	new opcode_struct(d68000_bsr_8        , 0xff00, 0x6100, 0x000),
	new opcode_struct(d68000_bsr_16       , 0xffff, 0x6100, 0x000),
	new opcode_struct(d68020_bsr_32       , 0xffff, 0x61ff, 0x000),
	new opcode_struct(d68000_btst_r       , 0xf1c0, 0x0100, 0xbff),
	new opcode_struct(d68000_btst_s       , 0xffc0, 0x0800, 0xbfb),
	new opcode_struct(d68020_callm        , 0xffc0, 0x06c0, 0x27b),
	new opcode_struct(d68020_cas_8        , 0xffc0, 0x0ac0, 0x3f8),
	new opcode_struct(d68020_cas_16       , 0xffc0, 0x0cc0, 0x3f8),
	new opcode_struct(d68020_cas_32       , 0xffc0, 0x0ec0, 0x3f8),
	new opcode_struct(d68020_cas2_16      , 0xffff, 0x0cfc, 0x000),
	new opcode_struct(d68020_cas2_32      , 0xffff, 0x0efc, 0x000),
	new opcode_struct(d68000_chk_16       , 0xf1c0, 0x4180, 0xbff),
	new opcode_struct(d68020_chk_32       , 0xf1c0, 0x4100, 0xbff),
	new opcode_struct(d68020_chk2_cmp2_8  , 0xffc0, 0x00c0, 0x27b),
	new opcode_struct(d68020_chk2_cmp2_16 , 0xffc0, 0x02c0, 0x27b),
	new opcode_struct(d68020_chk2_cmp2_32 , 0xffc0, 0x04c0, 0x27b),
	new opcode_struct(d68040_cinv         , 0xff20, 0xf400, 0x000),
	new opcode_struct(d68000_clr_8        , 0xffc0, 0x4200, 0xbf8),
	new opcode_struct(d68000_clr_16       , 0xffc0, 0x4240, 0xbf8),
	new opcode_struct(d68000_clr_32       , 0xffc0, 0x4280, 0xbf8),
	new opcode_struct(d68000_cmp_8        , 0xf1c0, 0xb000, 0xbff),
	new opcode_struct(d68000_cmp_16       , 0xf1c0, 0xb040, 0xfff),
	new opcode_struct(d68000_cmp_32       , 0xf1c0, 0xb080, 0xfff),
	new opcode_struct(d68000_cmpa_16      , 0xf1c0, 0xb0c0, 0xfff),
	new opcode_struct(d68000_cmpa_32      , 0xf1c0, 0xb1c0, 0xfff),
	new opcode_struct(d68000_cmpi_8       , 0xffc0, 0x0c00, 0xbf8),
	new opcode_struct(d68020_cmpi_pcdi_8  , 0xffff, 0x0c3a, 0x000),
	new opcode_struct(d68020_cmpi_pcix_8  , 0xffff, 0x0c3b, 0x000),
	new opcode_struct(d68000_cmpi_16      , 0xffc0, 0x0c40, 0xbf8),
	new opcode_struct(d68020_cmpi_pcdi_16 , 0xffff, 0x0c7a, 0x000),
	new opcode_struct(d68020_cmpi_pcix_16 , 0xffff, 0x0c7b, 0x000),
	new opcode_struct(d68000_cmpi_32      , 0xffc0, 0x0c80, 0xbf8),
	new opcode_struct(d68020_cmpi_pcdi_32 , 0xffff, 0x0cba, 0x000),
	new opcode_struct(d68020_cmpi_pcix_32 , 0xffff, 0x0cbb, 0x000),
	new opcode_struct(d68000_cmpm_8       , 0xf1f8, 0xb108, 0x000),
	new opcode_struct(d68000_cmpm_16      , 0xf1f8, 0xb148, 0x000),
	new opcode_struct(d68000_cmpm_32      , 0xf1f8, 0xb188, 0x000),
	new opcode_struct(d68020_cpbcc_16     , 0xf1c0, 0xf080, 0x000),
	new opcode_struct(d68020_cpbcc_32     , 0xf1c0, 0xf0c0, 0x000),
	new opcode_struct(d68020_cpdbcc       , 0xf1f8, 0xf048, 0x000),
	new opcode_struct(d68020_cpgen        , 0xf1c0, 0xf000, 0x000),
	new opcode_struct(d68020_cprestore    , 0xf1c0, 0xf140, 0x37f),
	new opcode_struct(d68020_cpsave       , 0xf1c0, 0xf100, 0x2f8),
	new opcode_struct(d68020_cpscc        , 0xf1c0, 0xf040, 0xbf8),
	new opcode_struct(d68020_cptrapcc_0   , 0xf1ff, 0xf07c, 0x000),
	new opcode_struct(d68020_cptrapcc_16  , 0xf1ff, 0xf07a, 0x000),
	new opcode_struct(d68020_cptrapcc_32  , 0xf1ff, 0xf07b, 0x000),
	new opcode_struct(d68040_cpush        , 0xff20, 0xf420, 0x000),
    //new opcode_struct(d68000_dbcc         , 0xf0f8, 0x50c8, 0x000, Opcode.dbf, "D0,Rw"),
    new opcode_struct(0xfff8, 0x51c8, 0x000, Opcode.dbf, "D0,Rw"),      // d68000_dbcc         
	new opcode_struct(d68000_dbra         , 0xfff8, 0x51c8, 0x000),
	new opcode_struct(d68000_divs         , 0xf1c0, 0x81c0, 0xbff),
	new opcode_struct(d68000_divu         , 0xf1c0, 0x80c0, 0xbff),
	new opcode_struct(d68020_divl         , 0xffc0, 0x4c40, 0xbff),
	new opcode_struct(d68000_eor_8        , 0xf1c0, 0xb100, 0xbf8),
	new opcode_struct(d68000_eor_16       , 0xf1c0, 0xb140, 0xbf8),
	new opcode_struct(d68000_eor_32       , 0xf1c0, 0xb180, 0xbf8),
	new opcode_struct(d68000_eori_to_ccr  , 0xffff, 0x0a3c, 0x000),
	new opcode_struct(d68000_eori_to_sr   , 0xffff, 0x0a7c, 0x000),
	new opcode_struct(d68000_eori_8       , 0xffc0, 0x0a00, 0xbf8),
	new opcode_struct(d68000_eori_16      , 0xffc0, 0x0a40, 0xbf8),
	new opcode_struct(d68000_eori_32      , 0xffc0, 0x0a80, 0xbf8),
	new opcode_struct(d68000_exg_dd       , 0xf1f8, 0xc140, 0x000),
	new opcode_struct(d68000_exg_aa       , 0xf1f8, 0xc148, 0x000),
	new opcode_struct(d68000_exg_da       , 0xf1f8, 0xc188, 0x000),
	new opcode_struct(d68020_extb_32      , 0xfff8, 0x49c0, 0x000),
	new opcode_struct(d68000_ext_16       , 0xfff8, 0x4880, 0x000),
	new opcode_struct(d68000_ext_32       , 0xfff8, 0x48c0, 0x000),
	new opcode_struct(d68040_fpu          , 0xffc0, 0xf200, 0x000),
	new opcode_struct(d68000_illegal      , 0xffff, 0x4afc, 0x000),
	new opcode_struct(d68000_jmp          , 0xffc0, 0x4ec0, 0x27b),
	new opcode_struct(d68000_jsr          , 0xffc0, 0x4e80, 0x27b),
	new opcode_struct(0xf1c0, 0x41c0, 0x27b, Opcode.lea, "E0,A9"),                  // d68000_lea          
	new opcode_struct(d68000_link_16      , 0xfff8, 0x4e50, 0x000),
	new opcode_struct(d68020_link_32      , 0xfff8, 0x4808, 0x000),
	new opcode_struct(0xf1f8, 0xe008, 0x000, Opcode.lsr, "s6:q9,D0"),         // d68000_lsr_s_8      
	new opcode_struct(d68000_lsr_s_16     , 0xf1f8, 0xe048, 0x000),
	new opcode_struct(d68000_lsr_s_32     , 0xf1f8, 0xe088, 0x000),
	new opcode_struct(d68000_lsr_r_8      , 0xf1f8, 0xe028, 0x000),
	new opcode_struct(d68000_lsr_r_16     , 0xf1f8, 0xe068, 0x000),
	new opcode_struct(d68000_lsr_r_32     , 0xf1f8, 0xe0a8, 0x000),
	new opcode_struct(d68000_lsr_ea       , 0xffc0, 0xe2c0, 0x3f8),
	new opcode_struct(0xf1f8, 0xe108, 0x000, Opcode.lsl, "s6:q9,D0"),         // d68000_lsl_s_8      
	new opcode_struct(d68000_lsl_s_16     , 0xf1f8, 0xe148, 0x000),
	new opcode_struct(d68000_lsl_s_32     , 0xf1f8, 0xe188, 0x000),
	new opcode_struct(d68000_lsl_r_8      , 0xf1f8, 0xe128, 0x000),
	new opcode_struct(d68000_lsl_r_16     , 0xf1f8, 0xe168, 0x000),
	new opcode_struct(d68000_lsl_r_32     , 0xf1f8, 0xe1a8, 0x000),
	new opcode_struct(d68000_lsl_ea       , 0xffc0, 0xe3c0, 0x3f8),
	new opcode_struct(0xf000, 0x1000, 0xbff, Opcode.move, "sb:E0,e6"),      // d68000_move_8       
	new opcode_struct(0xf000, 0x3000, 0xfff, Opcode.move, "sw:E0,e6"),      // d68000_move_16      , 
	new opcode_struct(0xf000, 0x2000, 0xfff, Opcode.move, "sl:E0,e6"),      // d68000_move_32      , 
	new opcode_struct(0xf1c0, 0x3040, 0xfff, Opcode.movea, "sw:E0,A9"),     // d68000_movea_16     ,
	new opcode_struct(0xf1c0, 0x2040, 0xfff, Opcode.movea, "sl:E0,A9"),     // (d68000_movea_32     ,
	new opcode_struct(d68000_move_to_ccr  , 0xffc0, 0x44c0, 0xbff),
	new opcode_struct(d68010_move_fr_ccr  , 0xffc0, 0x42c0, 0xbf8),
	new opcode_struct(d68000_move_to_sr   , 0xffc0, 0x46c0, 0xbff),
	new opcode_struct(d68000_move_fr_sr   , 0xffc0, 0x40c0, 0xbf8),
	new opcode_struct(d68000_move_to_usp  , 0xfff8, 0x4e60, 0x000),
	new opcode_struct(d68000_move_fr_usp  , 0xfff8, 0x4e68, 0x000),
	new opcode_struct(d68010_movec        , 0xfffe, 0x4e7a, 0x000),
	new opcode_struct(d68000_movem_pd_16  , 0xfff8, 0x48a0, 0x000),
	new opcode_struct(d68000_movem_pd_32  , 0xfff8, 0x48e0, 0x000),
	new opcode_struct(d68000_movem_re_16  , 0xffc0, 0x4880, 0x2f8),
	new opcode_struct(0xffc0, 0x48c0, 0x2f8, Opcode.movem, "sl:Iw,E0"),     // d68000_movem_re_32   
	new opcode_struct(d68000_movem_er_16  , 0xffc0, 0x4c80, 0x37b),
	new opcode_struct(d68000_movem_er_32  , 0xffc0, 0x4cc0, 0x37b),
	new opcode_struct(d68000_movep_er_16  , 0xf1f8, 0x0108, 0x000),
	new opcode_struct(d68000_movep_er_32  , 0xf1f8, 0x0148, 0x000),
	new opcode_struct(d68000_movep_re_16  , 0xf1f8, 0x0188, 0x000),
	new opcode_struct(d68000_movep_re_32  , 0xf1f8, 0x01c8, 0x000),
	new opcode_struct(d68010_moves_8      , 0xffc0, 0x0e00, 0x3f8),
	new opcode_struct(d68010_moves_16     , 0xffc0, 0x0e40, 0x3f8),
	new opcode_struct(d68010_moves_32     , 0xffc0, 0x0e80, 0x3f8),
	new opcode_struct(0xf100, 0x7000, 0x000, Opcode.moveq, "Q0,D9"),        // d68000_moveq        
	new opcode_struct(d68040_move16_pi_pi , 0xfff8, 0xf620, 0x000),
	new opcode_struct(d68040_move16_pi_al , 0xfff8, 0xf600, 0x000),
	new opcode_struct(d68040_move16_al_pi , 0xfff8, 0xf608, 0x000),
	new opcode_struct(d68040_move16_ai_al , 0xfff8, 0xf610, 0x000),
	new opcode_struct(d68040_move16_al_ai , 0xfff8, 0xf618, 0x000),
	new opcode_struct(d68000_muls         , 0xf1c0, 0xc1c0, 0xbff),
	new opcode_struct(d68000_mulu         , 0xf1c0, 0xc0c0, 0xbff),
	new opcode_struct(d68020_mull         , 0xffc0, 0x4c00, 0xbff),
	new opcode_struct(d68000_nbcd         , 0xffc0, 0x4800, 0xbf8),
	new opcode_struct(d68000_neg_8        , 0xffc0, 0x4400, 0xbf8),
	new opcode_struct(d68000_neg_16       , 0xffc0, 0x4440, 0xbf8),
	new opcode_struct(d68000_neg_32       , 0xffc0, 0x4480, 0xbf8),
	new opcode_struct(d68000_negx_8       , 0xffc0, 0x4000, 0xbf8),
	new opcode_struct(d68000_negx_16      , 0xffc0, 0x4040, 0xbf8),
	new opcode_struct(d68000_negx_32      , 0xffc0, 0x4080, 0xbf8),
	new opcode_struct(d68000_nop          , 0xffff, 0x4e71, 0x000),
	new opcode_struct(d68000_not_8        , 0xffc0, 0x4600, 0xbf8),
	new opcode_struct(d68000_not_16       , 0xffc0, 0x4640, 0xbf8),
	new opcode_struct(d68000_not_32       , 0xffc0, 0x4680, 0xbf8),
	new opcode_struct(d68000_or_er_8      , 0xf1c0, 0x8000, 0xbff),
	new opcode_struct(d68000_or_er_16     , 0xf1c0, 0x8040, 0xbff),
	new opcode_struct(d68000_or_er_32     , 0xf1c0, 0x8080, 0xbff),
	new opcode_struct(d68000_or_re_8      , 0xf1c0, 0x8100, 0x3f8),
	new opcode_struct(d68000_or_re_16     , 0xf1c0, 0x8140, 0x3f8),
	new opcode_struct(d68000_or_re_32     , 0xf1c0, 0x8180, 0x3f8),
	new opcode_struct(0xffff, 0x003c, 0x000, Opcode.ori, "sb:Ib,c"),        // d68000_ori_to_ccr   
	new opcode_struct(0xffff, 0x007c, 0x000, Opcode.ori, "sw:Iw,s"),        // d68000_ori_to_sr    
	new opcode_struct(0xffc0, 0x0000, 0xbf8, Opcode.ori, "s6:Iv,e0"),       // d68000_ori_8        
	new opcode_struct(0xffc0, 0x0040, 0xbf8, Opcode.ori, "s6:Iv,e0"),       // d68000_ori_16        
	new opcode_struct(0xffc0, 0x0080, 0xbf8, Opcode.ori, "s6:Iv,e0"),       // d68000_ori_32       
	new opcode_struct(d68020_pack_rr      , 0xf1f8, 0x8140, 0x000),
	new opcode_struct(d68020_pack_mm      , 0xf1f8, 0x8148, 0x000),
	new opcode_struct(d68000_pea          , 0xffc0, 0x4840, 0x27b),
	new opcode_struct(d68040_pflush       , 0xffe0, 0xf500, 0x000),
	new opcode_struct(d68000_reset        , 0xffff, 0x4e70, 0x000),
	new opcode_struct(d68000_ror_s_8      , 0xf1f8, 0xe018, 0x000),
	new opcode_struct(d68000_ror_s_16     , 0xf1f8, 0xe058, 0x000),
	new opcode_struct(d68000_ror_s_32     , 0xf1f8, 0xe098, 0x000),
	new opcode_struct(d68000_ror_r_8      , 0xf1f8, 0xe038, 0x000),
	new opcode_struct(d68000_ror_r_16     , 0xf1f8, 0xe078, 0x000),
	new opcode_struct(d68000_ror_r_32     , 0xf1f8, 0xe0b8, 0x000),
	new opcode_struct(d68000_ror_ea       , 0xffc0, 0xe6c0, 0x3f8),
	new opcode_struct(d68000_rol_s_8      , 0xf1f8, 0xe118, 0x000),
	new opcode_struct(d68000_rol_s_16     , 0xf1f8, 0xe158, 0x000),
	new opcode_struct(d68000_rol_s_32     , 0xf1f8, 0xe198, 0x000),
	new opcode_struct(d68000_rol_r_8      , 0xf1f8, 0xe138, 0x000),
	new opcode_struct(d68000_rol_r_16     , 0xf1f8, 0xe178, 0x000),
	new opcode_struct(d68000_rol_r_32     , 0xf1f8, 0xe1b8, 0x000),
	new opcode_struct(d68000_rol_ea       , 0xffc0, 0xe7c0, 0x3f8),
	new opcode_struct(d68000_roxr_s_8     , 0xf1f8, 0xe010, 0x000),
	new opcode_struct(d68000_roxr_s_16    , 0xf1f8, 0xe050, 0x000),
	new opcode_struct(d68000_roxr_s_32    , 0xf1f8, 0xe090, 0x000),
	new opcode_struct(d68000_roxr_r_8     , 0xf1f8, 0xe030, 0x000),
	new opcode_struct(d68000_roxr_r_16    , 0xf1f8, 0xe070, 0x000),
	new opcode_struct(d68000_roxr_r_32    , 0xf1f8, 0xe0b0, 0x000),
	new opcode_struct(d68000_roxr_ea      , 0xffc0, 0xe4c0, 0x3f8),
	new opcode_struct(d68000_roxl_s_8     , 0xf1f8, 0xe110, 0x000),
	new opcode_struct(d68000_roxl_s_16    , 0xf1f8, 0xe150, 0x000),
	new opcode_struct(d68000_roxl_s_32    , 0xf1f8, 0xe190, 0x000),
	new opcode_struct(d68000_roxl_r_8     , 0xf1f8, 0xe130, 0x000),
	new opcode_struct(d68000_roxl_r_16    , 0xf1f8, 0xe170, 0x000),
	new opcode_struct(d68000_roxl_r_32    , 0xf1f8, 0xe1b0, 0x000),
	new opcode_struct(d68000_roxl_ea      , 0xffc0, 0xe5c0, 0x3f8),
	new opcode_struct(d68010_rtd          , 0xffff, 0x4e74, 0x000),
	new opcode_struct(d68000_rte          , 0xffff, 0x4e73, 0x000),
	new opcode_struct(d68020_rtm          , 0xfff0, 0x06c0, 0x000),
	new opcode_struct(d68000_rtr          , 0xffff, 0x4e77, 0x000),
	new opcode_struct(d68000_rts          , 0xffff, 0x4e75, 0x000),
	new opcode_struct(d68000_sbcd_rr      , 0xf1f8, 0x8100, 0x000),
	new opcode_struct(d68000_sbcd_mm      , 0xf1f8, 0x8108, 0x000),
	new opcode_struct(d68000_scc          , 0xf0c0, 0x50c0, 0xbf8),
	new opcode_struct(d68000_stop         , 0xffff, 0x4e72, 0x000),
	new opcode_struct(d68000_sub_er_8     , 0xf1c0, 0x9000, 0xbff),
	new opcode_struct(d68000_sub_er_16    , 0xf1c0, 0x9040, 0xfff),
	new opcode_struct(d68000_sub_er_32    , 0xf1c0, 0x9080, 0xfff),
	new opcode_struct(d68000_sub_re_8     , 0xf1c0, 0x9100, 0x3f8),
	new opcode_struct(d68000_sub_re_16    , 0xf1c0, 0x9140, 0x3f8),
	new opcode_struct(d68000_sub_re_32    , 0xf1c0, 0x9180, 0x3f8),
	new opcode_struct(d68000_suba_16      , 0xf1c0, 0x90c0, 0xfff),
	new opcode_struct(d68000_suba_32      , 0xf1c0, 0x91c0, 0xfff),
	new opcode_struct(d68000_subi_8       , 0xffc0, 0x0400, 0xbf8),
	new opcode_struct(d68000_subi_16      , 0xffc0, 0x0440, 0xbf8),
	new opcode_struct(d68000_subi_32      , 0xffc0, 0x0480, 0xbf8),
	new opcode_struct(d68000_subq_8       , 0xf1c0, 0x5100, 0xbf8),
	new opcode_struct(d68000_subq_16      , 0xf1c0, 0x5140, 0xff8),
	new opcode_struct(d68000_subq_32      , 0xf1c0, 0x5180, 0xff8),
	new opcode_struct(d68000_subx_rr_8    , 0xf1f8, 0x9100, 0x000),
	new opcode_struct(d68000_subx_rr_16   , 0xf1f8, 0x9140, 0x000),
	new opcode_struct(d68000_subx_rr_32   , 0xf1f8, 0x9180, 0x000),
	new opcode_struct(d68000_subx_mm_8    , 0xf1f8, 0x9108, 0x000),
	new opcode_struct(d68000_subx_mm_16   , 0xf1f8, 0x9148, 0x000),
	new opcode_struct(d68000_subx_mm_32   , 0xf1f8, 0x9188, 0x000),
	new opcode_struct(d68000_swap         , 0xfff8, 0x4840, 0x000),
	new opcode_struct(d68000_tas          , 0xffc0, 0x4ac0, 0xbf8),
	new opcode_struct(d68000_trap         , 0xfff0, 0x4e40, 0x000),
	new opcode_struct(d68020_trapcc_0     , 0xf0ff, 0x50fc, 0x000),
	new opcode_struct(d68020_trapcc_16    , 0xf0ff, 0x50fa, 0x000),
	new opcode_struct(d68020_trapcc_32    , 0xf0ff, 0x50fb, 0x000),
	new opcode_struct(d68000_trapv        , 0xffff, 0x4e76, 0x000),
	new opcode_struct(d68000_tst_8        , 0xffc0, 0x4a00, 0xbf8),
	new opcode_struct(d68020_tst_pcdi_8   , 0xffff, 0x4a3a, 0x000),
	new opcode_struct(d68020_tst_pcix_8   , 0xffff, 0x4a3b, 0x000),
	new opcode_struct(d68020_tst_i_8      , 0xffff, 0x4a3c, 0x000),
	new opcode_struct(d68000_tst_16       , 0xffc0, 0x4a40, 0xbf8),
	new opcode_struct(d68020_tst_a_16     , 0xfff8, 0x4a48, 0x000),
	new opcode_struct(d68020_tst_pcdi_16  , 0xffff, 0x4a7a, 0x000),
	new opcode_struct(d68020_tst_pcix_16  , 0xffff, 0x4a7b, 0x000),
	new opcode_struct(d68020_tst_i_16     , 0xffff, 0x4a7c, 0x000),
	new opcode_struct(d68000_tst_32       , 0xffc0, 0x4a80, 0xbf8),
	new opcode_struct(d68020_tst_a_32     , 0xfff8, 0x4a88, 0x000),
	new opcode_struct(d68020_tst_pcdi_32  , 0xffff, 0x4aba, 0x000),
	new opcode_struct(d68020_tst_pcix_32  , 0xffff, 0x4abb, 0x000),
	new opcode_struct(d68020_tst_i_32     , 0xffff, 0x4abc, 0x000),
	new opcode_struct(d68000_unlk         , 0xfff8, 0x4e58, 0x000),
	new opcode_struct(d68020_unpk_rr      , 0xf1f8, 0x8180, 0x000),
	new opcode_struct(d68020_unpk_mm      , 0xf1f8, 0x8188, 0x000),
	new opcode_struct(d68851_p000         , 0xffc0, 0xf000, 0x000),
	new opcode_struct(d68851_pbcc16       , 0xffc0, 0xf080, 0x000),
	new opcode_struct(d68851_pbcc32       , 0xffc0, 0xf0c0, 0x000),
	new opcode_struct(d68851_pdbcc        , 0xfff8, 0xf048, 0x000),
	new opcode_struct(d68851_p001         , 0xffc0, 0xf040, 0x000),
	new opcode_struct(null, 0, 0, 0),
};
        }

        private static opcode_struct illegalOpcode = new opcode_struct(0, 0, 0, Opcode.illegal, "");

        /* Check if opcode is using a valid ea mode */
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

        static int compare_nof_true_bits(opcode_struct aptr, opcode_struct bptr)
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

        private void build_opcode_table()
        {
            uint i;
            uint opcode;
            int ostruct;
            opcode_struct[] opcode_info = (opcode_struct[])g_opcode_info.Clone();
            Array.Sort<opcode_struct>(opcode_info, compare_nof_true_bits);

            for (i = 0; i < 0x10000; i++)
            {
                g_instruction_table[i] =  illegalOpcode; //default to illegal
                opcode = i;
                // search through opcode info for a match
                for (ostruct = 0; opcode_info[ostruct].opcode_handler != null; ostruct++)
                {
                    // match opcode mask and allowed ea modes
                    if ((opcode & opcode_info[ostruct].mask) == opcode_info[ostruct].match)
                    {
                        // Handle destination ea for move instructions 
                        if ((opcode_info[ostruct].opcode_handler == d68000_move_8 ||
                             opcode_info[ostruct].opcode_handler == d68000_move_16 ||
                             opcode_info[ostruct].opcode_handler == d68000_move_32) &&
                             !valid_ea(((opcode >> 9) & 7) | ((opcode >> 3) & 0x38), 0xbf8))
                            continue;
                        if (valid_ea(opcode, opcode_info[ostruct].ea_mask))
                        {
                            g_instruction_table[i] = opcode_info[ostruct];
                            break;
                        }
                    }
                }
            }
        }


        /* ======================================================================== */
        /* ================================= API ================================== */
        /* ======================================================================== */

        /* Disasemble one instruction at pc and store in str_buff */
        public uint m68k_disassemble(string str_buff, uint pc, uint cpu_type)
        {
            if (!g_initialized)
            {
                build_opcode_table();
                g_initialized = true;
            }
            //switch (cpu_type)
            //{
            //case M68K_CPU_TYPE_68000:
            //    g_cpu_type = TYPE_68000;
            //    break;
            //case M68K_CPU_TYPE_68008:
            //    g_cpu_type = TYPE_68008;
            //    break;
            //case M68K_CPU_TYPE_68010:
            //    g_cpu_type = TYPE_68010;
            //    break;
            //case M68K_CPU_TYPE_68EC020:
            //case M68K_CPU_TYPE_68020:
            //    g_cpu_type = TYPE_68020;
            //    break;
            //case M68K_CPU_TYPE_68EC030:
            //case M68K_CPU_TYPE_68030:
            //    g_cpu_type = TYPE_68030;
            //    break;
            //case M68K_CPU_TYPE_68040:
            //case M68K_CPU_TYPE_68EC040:
            //case M68K_CPU_TYPE_68LC040:
            //    g_cpu_type = TYPE_68040;
            //    break;
            //default:
            //    return 0;
            //}

            //g_cpu_pc = pc;
            g_helper_str = "";
            g_cpu_ir = read_imm_16();
            g_opcode_type = 0;
            g_instruction_table[g_cpu_ir].opcode_handler();
            str_buff = string.Format("{0}{1}", g_dasm_str, g_helper_str);
            return 0;
        }

        public M68kInstruction Disassemble()
        {
            if (!g_initialized)
            {
                build_opcode_table();
                g_initialized = true;
            }
            g_helper_str = "";
            g_cpu_ir = g_cpu_pc.ReadBeUInt16();
            g_opcode_type = 0;

            instr = new M68kInstruction();
            instr.code = g_instruction_table[g_cpu_ir].opcode;
            var args = g_instruction_table[g_cpu_ir].operandFormat;
            int i = 0;
            if (args[0] == 's')
            {
                instr.dataWidth = OperandFormatDecoder.GetSizeType(g_cpu_ir, args[1], null);
                i = 3;
            }
            OperandFormatDecoder opTranslator = new OperandFormatDecoder(g_cpu_ir, args, i);
            instr.op1 = opTranslator.GetOperand(g_cpu_pc, instr.dataWidth);
            instr.op2 = opTranslator.GetOperand(g_cpu_pc, instr.dataWidth);
            instr.op3 = opTranslator.GetOperand(g_cpu_pc, instr.dataWidth);
            return instr;
        }

#if UNUSED_FUNCTION
string m68ki_disassemble_quick(unsigned int pc, unsigned int cpu_type)
{
	static char buff[100];
	buff[0] = 0;
	m68k_disassemble(buff, pc, cpu_type);
	return buff;
}
#endif

        uint m68k_disassemble_raw(string str_buff, uint pc, string opdata, string argdata, uint cpu_type)
        {
            g_rawop = opdata;
            g_rawbasepc = pc;
            var result = m68k_disassemble(str_buff, pc, cpu_type);
            g_rawop = null;
            return result;
        }

#if UNUSED_FUNCTION
/* Check if the instruction is a valid one */
unsigned int m68k_is_valid_instruction(unsigned int instruction, unsigned int cpu_type)
{
	if(!g_initialized)
	{
		build_opcode_table();
		g_initialized = 1;
	}

	instruction &= 0xffff;
	if(g_instruction_table[instruction] == d68000_illegal)
		return 0;

	switch(cpu_type)
	{
		case M68K_CPU_TYPE_68000:
		case M68K_CPU_TYPE_68008:
			if(g_instruction_table[instruction] == d68010_bkpt)
				return 0;
			if(g_instruction_table[instruction] == d68010_move_fr_ccr)
				return 0;
			if(g_instruction_table[instruction] == d68010_movec)
				return 0;
			if(g_instruction_table[instruction] == d68010_moves_8)
				return 0;
			if(g_instruction_table[instruction] == d68010_moves_16)
				return 0;
			if(g_instruction_table[instruction] == d68010_moves_32)
				return 0;
			if(g_instruction_table[instruction] == d68010_rtd)
				return 0;
		case M68K_CPU_TYPE_68010:
			if(g_instruction_table[instruction] == d68020_bcc_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_bfchg)
				return 0;
			if(g_instruction_table[instruction] == d68020_bfclr)
				return 0;
			if(g_instruction_table[instruction] == d68020_bfexts)
				return 0;
			if(g_instruction_table[instruction] == d68020_bfextu)
				return 0;
			if(g_instruction_table[instruction] == d68020_bfffo)
				return 0;
			if(g_instruction_table[instruction] == d68020_bfins)
				return 0;
			if(g_instruction_table[instruction] == d68020_bfset)
				return 0;
			if(g_instruction_table[instruction] == d68020_bftst)
				return 0;
			if(g_instruction_table[instruction] == d68020_bra_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_bsr_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_callm)
				return 0;
			if(g_instruction_table[instruction] == d68020_cas_8)
				return 0;
			if(g_instruction_table[instruction] == d68020_cas_16)
				return 0;
			if(g_instruction_table[instruction] == d68020_cas_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_cas2_16)
				return 0;
			if(g_instruction_table[instruction] == d68020_cas2_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_chk_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_chk2_cmp2_8)
				return 0;
			if(g_instruction_table[instruction] == d68020_chk2_cmp2_16)
				return 0;
			if(g_instruction_table[instruction] == d68020_chk2_cmp2_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_cmpi_pcdi_8)
				return 0;
			if(g_instruction_table[instruction] == d68020_cmpi_pcix_8)
				return 0;
			if(g_instruction_table[instruction] == d68020_cmpi_pcdi_16)
				return 0;
			if(g_instruction_table[instruction] == d68020_cmpi_pcix_16)
				return 0;
			if(g_instruction_table[instruction] == d68020_cmpi_pcdi_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_cmpi_pcix_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_cpbcc_16)
				return 0;
			if(g_instruction_table[instruction] == d68020_cpbcc_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_cpdbcc)
				return 0;
			if(g_instruction_table[instruction] == d68020_cpgen)
				return 0;
			if(g_instruction_table[instruction] == d68020_cprestore)
				return 0;
			if(g_instruction_table[instruction] == d68020_cpsave)
				return 0;
			if(g_instruction_table[instruction] == d68020_cpscc)
				return 0;
			if(g_instruction_table[instruction] == d68020_cptrapcc_0)
				return 0;
			if(g_instruction_table[instruction] == d68020_cptrapcc_16)
				return 0;
			if(g_instruction_table[instruction] == d68020_cptrapcc_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_divl)
				return 0;
			if(g_instruction_table[instruction] == d68020_extb_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_link_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_mull)
				return 0;
			if(g_instruction_table[instruction] == d68020_pack_rr)
				return 0;
			if(g_instruction_table[instruction] == d68020_pack_mm)
				return 0;
			if(g_instruction_table[instruction] == d68020_rtm)
				return 0;
			if(g_instruction_table[instruction] == d68020_trapcc_0)
				return 0;
			if(g_instruction_table[instruction] == d68020_trapcc_16)
				return 0;
			if(g_instruction_table[instruction] == d68020_trapcc_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_tst_pcdi_8)
				return 0;
			if(g_instruction_table[instruction] == d68020_tst_pcix_8)
				return 0;
			if(g_instruction_table[instruction] == d68020_tst_i_8)
				return 0;
			if(g_instruction_table[instruction] == d68020_tst_a_16)
				return 0;
			if(g_instruction_table[instruction] == d68020_tst_pcdi_16)
				return 0;
			if(g_instruction_table[instruction] == d68020_tst_pcix_16)
				return 0;
			if(g_instruction_table[instruction] == d68020_tst_i_16)
				return 0;
			if(g_instruction_table[instruction] == d68020_tst_a_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_tst_pcdi_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_tst_pcix_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_tst_i_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_unpk_rr)
				return 0;
			if(g_instruction_table[instruction] == d68020_unpk_mm)
				return 0;
		case M68K_CPU_TYPE_68EC020:
		case M68K_CPU_TYPE_68020:
		case M68K_CPU_TYPE_68030:
		case M68K_CPU_TYPE_68EC030:
			if(g_instruction_table[instruction] == d68040_cinv)
				return 0;
			if(g_instruction_table[instruction] == d68040_cpush)
				return 0;
			if(g_instruction_table[instruction] == d68040_move16_pi_pi)
				return 0;
			if(g_instruction_table[instruction] == d68040_move16_pi_al)
				return 0;
			if(g_instruction_table[instruction] == d68040_move16_al_pi)
				return 0;
			if(g_instruction_table[instruction] == d68040_move16_ai_al)
				return 0;
			if(g_instruction_table[instruction] == d68040_move16_al_ai)
				return 0;
		case M68K_CPU_TYPE_68040:
		case M68K_CPU_TYPE_68EC040:
		case M68K_CPU_TYPE_68LC040:
			if(g_instruction_table[instruction] == d68020_cpbcc_16)
				return 0;
			if(g_instruction_table[instruction] == d68020_cpbcc_32)
				return 0;
			if(g_instruction_table[instruction] == d68020_cpdbcc)
				return 0;
			if(g_instruction_table[instruction] == d68020_cpgen)
				return 0;
			if(g_instruction_table[instruction] == d68020_cprestore)
				return 0;
			if(g_instruction_table[instruction] == d68020_cpsave)
				return 0;
			if(g_instruction_table[instruction] == d68020_cpscc)
				return 0;
			if(g_instruction_table[instruction] == d68020_cptrapcc_0)
				return 0;
			if(g_instruction_table[instruction] == d68020_cptrapcc_16)
				return 0;
			if(g_instruction_table[instruction] == d68020_cptrapcc_32)
				return 0;
			if(g_instruction_table[instruction] == d68040_pflush)
				return 0;
	}
	if(cpu_type != M68K_CPU_TYPE_68020 && cpu_type != M68K_CPU_TYPE_68EC020 &&
	  (g_instruction_table[instruction] == d68020_callm ||
	  g_instruction_table[instruction] == d68020_rtm))
		return 0;

	return 1;
}
#endif

        //CPU_DISASSEMBLE( m68000 )
        //{
        //    return m68k_disassemble_raw(buffer, pc, oprom, opram, M68K_CPU_TYPE_68000);
        //}

        //CPU_DISASSEMBLE( m68008 )
        //{
        //    return m68k_disassemble_raw(buffer, pc, oprom, opram, M68K_CPU_TYPE_68008);
        //}

        //CPU_DISASSEMBLE( m68010 )
        //{
        //    return m68k_disassemble_raw(buffer, pc, oprom, opram, M68K_CPU_TYPE_68010);
        //}

        //CPU_DISASSEMBLE( m68020 )
        //{
        //    return m68k_disassemble_raw(buffer, pc, oprom, opram, M68K_CPU_TYPE_68020);
        //}

        //CPU_DISASSEMBLE( m68030 )
        //{
        //    return m68k_disassemble_raw(buffer, pc, oprom, opram, M68K_CPU_TYPE_68030);
        //}

        //CPU_DISASSEMBLE( m68040 )
        //{
        //    return m68k_disassemble_raw(buffer, pc, oprom, opram, M68K_CPU_TYPE_68040);
        //}

        // f028 2215 0008

        /* ======================================================================== */
        /* ============================== END OF FILE ============================= */
        /* ======================================================================== */

    }
}