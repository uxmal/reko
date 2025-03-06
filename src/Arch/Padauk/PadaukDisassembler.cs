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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Padauk
{
    using Decoder = Reko.Core.Machine.Decoder<PadaukDisassembler, Mnemonic, PadaukInstruction>;
    using Mutator = Reko.Core.Machine.Mutator<PadaukDisassembler>;

    public partial class PadaukDisassembler : DisassemblerBase<PadaukInstruction, Mnemonic>
    {
        private readonly PadaukArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private readonly Decoder rootDecoder;
        private Address addr;

        public PadaukDisassembler(PadaukArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            var isa = arch.CreateInstructionSet();
            this.rootDecoder = isa.CreateDecoder();
            this.addr = default!;
        }

        public override PadaukInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt16(out ushort uInstr))
                return null;
            var instr = this.rootDecoder.Decode(uInstr, this);
            this.ops.Clear();
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            return instr;
        }

        public override PadaukInstruction CreateInvalidInstruction()
        {
            return new PadaukInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
            };
        }

        public override PadaukInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new PadaukInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override PadaukInstruction NotYetImplemented(string message)
        {
            throw new NotImplementedException();
        }

        public abstract class InstructionSet
        {
            public abstract Decoder CreateDecoder();

            public static Decoder Instr(Mnemonic mnemonic, params Mutator[] mutators)
            {
                return new InstrDecoder<PadaukDisassembler, Mnemonic, PadaukInstruction>(InstrClass.Linear, mnemonic, mutators);
            }

            public static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator[] mutators)
            {
                return new InstrDecoder<PadaukDisassembler, Mnemonic, PadaukInstruction>(iclass, mnemonic, mutators);
            }
        }

        private static bool a(uint uInstr, PadaukDisassembler dasm)
        {
            dasm.ops.Add(Registers.a);
            return true;
        }

        private static readonly Bitfield bf0_4 = new Bitfield(0, 4);
        private static readonly Bitfield bf0_5 = new Bitfield(0, 5);
        private static readonly Bitfield bf0_6 = new Bitfield(0, 6);
        private static readonly Bitfield bf0_7 = new Bitfield(0, 7);
        private static readonly Bitfield bf0_8 = new Bitfield(0, 8);
        private static readonly Bitfield bf0_10 = new Bitfield(0, 10);
        private static readonly Bitfield bf0_11 = new Bitfield(0, 11);
        private static readonly Bitfield bf0_12 = new Bitfield(0, 12);
        private static readonly Bitfield bf1_4 = new Bitfield(1, 4);
        private static readonly Bitfield bf1_7 = new Bitfield(1, 7);
        private static readonly Bitfield bf5_3 = new Bitfield(5, 3);
        private static readonly Bitfield bf6_3 = new Bitfield(6, 3);
        private static readonly Bitfield bf7_3 = new Bitfield(7, 3);

        private static bool m0_4(uint uInstr, PadaukDisassembler dasm)
        {
            var m = bf0_4.Read(uInstr);
            var mem = new MemoryOperand(PrimitiveType.Word16)
            {
                Offset = m,
            };
            dasm.ops.Add(mem);
            return true;
        }

        private static Mutator<PadaukDisassembler> Mem(Bitfield bf)
        {
            return (u, d) =>
            {
                var m = bf.Read(u);
                var mem = new MemoryOperand(PrimitiveType.Word16)
                {
                    Offset = m,
                    Bit = null,
                };
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<PadaukDisassembler> m0_6 = Mem(bf0_6);
        private static readonly Mutator<PadaukDisassembler> m0_7 = Mem(bf0_7);
        private static readonly Mutator<PadaukDisassembler> m0_8 = Mem(bf0_8);

        private static Mutator<PadaukDisassembler> IndirectMem(Bitfield bf)
        {
            return (u, d) =>
            {
                var m = bf.Read(u);
                var mem = new MemoryOperand(PrimitiveType.Word16)
                {
                    Offset = m,
                    Bit = null,
                    Indirect = true,
                };
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<PadaukDisassembler> im1_4 = IndirectMem(bf1_4);
        private static readonly Mutator<PadaukDisassembler> im1_7 = IndirectMem(bf1_7);

        private static Mutator<PadaukDisassembler> MemBit(Bitfield bfAddr, Bitfield bfBit)
        {
            return (u, d) =>
            {
                var m = bfAddr.Read(u);
                var bit = (int)bfBit.Read(u);
                var mem = new MemoryOperand(PrimitiveType.Word16)
                {
                    Offset = m,
                    Bit = bit,
                };
                d.ops.Add(mem);
                return true;
            };
        }

        private static readonly Mutator<PadaukDisassembler> Mn_0_5 = MemBit(bf0_5, bf5_3);
        private static readonly Mutator<PadaukDisassembler> Mn_0_6 = MemBit(bf0_6, bf6_3);
        private static readonly Mutator<PadaukDisassembler> Mn_0_7 = MemBit(bf0_7, bf7_3);


        private static bool i0_5(uint uInstr, PadaukDisassembler dasm)
        {
            var i = bf0_5.Read(uInstr);
            dasm.ops.Add(Constant.Int16((short) i));
            return true;
        }

        private static bool i0_8(uint uInstr, PadaukDisassembler dasm)
        {
            var i = bf0_8.Read(uInstr);
            dasm.ops.Add(Constant.Byte((byte) i));
            return true;
        }

        private static bool i5_3(uint uInstr, PadaukDisassembler dasm)
        {
            var i = bf5_3.Read(uInstr);
            dasm.ops.Add(Constant.Int16((short)i));
            return true;
        }

        private static Mutator<PadaukDisassembler> absAddress(Bitfield addrField)
        {
            return (u, d) =>
            {
                var i = addrField.Read(u);
                d.ops.Add(Address.Ptr16((ushort)i));
                return true;
            };
        }
        private static readonly Mutator<PadaukDisassembler> a10 = absAddress(bf0_10);
        private static readonly Mutator<PadaukDisassembler> a11 = absAddress(bf0_11);
        private static readonly Mutator<PadaukDisassembler> a12 = absAddress(bf0_12);

        private static Mutator<PadaukDisassembler> Port(Bitfield bf)
        {
            return (u, d) =>
            {
                var i = bf.Read(u);
                d.ops.Add(PortOperand.Create((ushort) i));
                return true;
            };
        }
        private static readonly Mutator<PadaukDisassembler> p0_5 = Port(bf0_5);
        private static readonly Mutator<PadaukDisassembler> p0_6 = Port(bf0_6);
        private static readonly Mutator<PadaukDisassembler> p0_7 = Port(bf0_7);

        private static Mutator<PadaukDisassembler> PortBit(Bitfield bfPort, Bitfield bfBit)
        {
            return (u, d) =>
            {
                var port = bfPort.Read(u);
                var bit = (int)bfBit.Read(u);
                d.ops.Add(PortOperand.CreateBitAccess(port, bit));
                return true;
            };
        }
        private static readonly Mutator<PadaukDisassembler> Pn_0_5 = PortBit(bf0_5, bf5_3);
        private static readonly Mutator<PadaukDisassembler> Pn_0_6 = PortBit(bf0_6, bf6_3);
        private static readonly Mutator<PadaukDisassembler> Pn_0_7 = PortBit(bf0_7, bf7_3);


        private static Mutator<PadaukDisassembler> Word(Bitfield bf)
        {
            return (u, d) =>
            {
                var i = bf.Read(u);
                d.ops.Add(Constant.Word16((ushort) i));
                return true;
            };
        }
        private static readonly Mutator<PadaukDisassembler> w1_4 = Word(bf1_4);
        private static readonly Mutator<PadaukDisassembler> w1_7 = Word(bf1_7);

    }
}
