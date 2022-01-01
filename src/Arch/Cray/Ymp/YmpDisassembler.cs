#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
        private readonly Decoder rootDecoder;
        
        private readonly CrayYmpArchitecture arch;
        private readonly Word16BeImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public YmpDisassembler(CrayYmpArchitecture arch, Decoder<YmpDisassembler, Mnemonic, CrayInstruction> decoder, EndianImageReader rdr)
        {
            this.arch = arch;
            // Crays are weird; we can only disassemble areas that have 16-bit granularity.
            this.rdr = (Word16BeImageReader) rdr;
            this.ops = new List<MachineOperand>();
            this.rootDecoder = decoder;
            this.addr = null!;
        }

        public override CrayInstruction? DisassembleInstruction()
        {
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

        internal static Mutator<YmpDisassembler> Reg(int bitOffset, int bitSize, RegisterStorage[] regs)
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
        internal static readonly Mutator<YmpDisassembler> Si = Reg(6, 3, Registers.SRegs);
        internal static readonly Mutator<YmpDisassembler> Sj = Reg(3, 3, Registers.SRegs);
        internal static readonly Mutator<YmpDisassembler> Sk = Reg(0, 3, Registers.SRegs);

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
        internal static readonly Mutator<YmpDisassembler> Sk_SB = Reg0(0, 3, Registers.sb, Registers.SRegs);

        internal static readonly Mutator<YmpDisassembler> Ah = Reg(9, 3, Registers.ARegs);
        internal static readonly Mutator<YmpDisassembler> Ai = Reg(6, 3, Registers.ARegs);
        internal static readonly Mutator<YmpDisassembler> Aj = Reg(3, 3, Registers.ARegs);
        internal static readonly Mutator<YmpDisassembler> Ak = Reg(0, 3, Registers.ARegs);

        internal static readonly Mutator<YmpDisassembler> Bjk = Reg(0, 6, Registers.BRegs);
        internal static readonly Mutator<YmpDisassembler> Tjk = Reg(0, 6, Registers.TRegs);

        internal static readonly Mutator<YmpDisassembler> STj = Reg(3, 3, Registers.STRegs);

        internal static readonly Mutator<YmpDisassembler> Vi = Reg(6, 3, Registers.VRegs);
        internal static readonly Mutator<YmpDisassembler> Vj = Reg(3, 3, Registers.VRegs);
        internal static readonly Mutator<YmpDisassembler> Vk = Reg(0, 3, Registers.VRegs);

        internal static Mutator<YmpDisassembler> Reg(RegisterStorage reg)
        {
            var r = new RegisterOperand(reg);
            return (u, d) =>
            {
                d.ops.Add(r);
                return true;
            };
        }

        internal static readonly Mutator<YmpDisassembler> A0 = Reg(Registers.ARegs[0]);
        internal static readonly Mutator<YmpDisassembler> S0 = Reg(Registers.SRegs[0]);
        internal static readonly Mutator<YmpDisassembler> SB = Reg(Registers.sb);

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

        internal static readonly Mutator<YmpDisassembler> Ijk = Imm(PrimitiveType.Byte, 0, 6);
        internal static readonly Mutator<YmpDisassembler> Ijk_32 = Imm(PrimitiveType.Word32, 0, 6);

        internal static Mutator<YmpDisassembler> ImmFrom(uint from, PrimitiveType dt, int bitpos, int bitlen)
        {
            var bitfield = new Bitfield(bitpos, bitlen);
            return (u, d) =>
            {
                var imm = from - bitfield.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, imm)));
                return true;
            };
        }

        internal static readonly Mutator<YmpDisassembler> Ijk_from_40 = ImmFrom(0x40, PrimitiveType.Byte, 0, 6);

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
        internal static readonly Mutator<YmpDisassembler> IjkFrom64 = Imm(PrimitiveType.Byte, 0, 6, 64);

        internal static bool Inm(uint uInstr, YmpDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out ushort m))
                return false;
            if (!dasm.rdr.TryReadBeUInt16(out ushort n))
                return false;
            uint nm = n;
            nm = nm << 16 | m;
            dasm.ops.Add(ImmediateOperand.Word32(nm));
            return true;
        }

        internal static bool InmZext(uint uInstr, YmpDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt32(out uint nm))
                return false;
            dasm.ops.Add(ImmediateOperand.Word64(nm));
            return true;
        }

        internal static bool Jijkm(uint uInstr, YmpDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out ushort n))
                return false;
            var uAddr = ((uInstr & 0x1FF) << 16) | n;
            dasm.ops.Add(AddressOperand.Ptr32(uAddr));
            return true;
        }

        internal static bool Jnm(uint uInstr, YmpDisassembler dasm)
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

        internal static bool Imm0(uint word, YmpDisassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Word64(0));
            return true;
        }

        internal static bool Imm1(uint word, YmpDisassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Word64(1));
            return true;
        }

        internal static bool Imm_1(uint word, YmpDisassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Word64(-1L));
            return true;
        }

        internal static Mutator<YmpDisassembler> Imm32(uint n)
        {
            var imm = ImmediateOperand.Word32(n);
            return (u, d) =>
            {
                d.ops.Add(imm);
                return true;
            };
        }
        internal static Mutator<YmpDisassembler> Imm64(ulong n)
        {
            var imm = ImmediateOperand.Word64(n);
            return (u, d) =>
            {
                d.ops.Add(imm);
                return true;
            };
        }

        internal static readonly Mutator<YmpDisassembler> Imm_2_63 = Imm64(1ul << 63);

        #endregion

        internal static bool Is0(uint u) => u == 0;
    }
}
