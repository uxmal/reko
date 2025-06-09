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
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Vax
{
    using Decoder = Decoder<VaxDisassembler, Mnemonic, VaxInstruction>;
    using Mutator = Mutator<VaxDisassembler>;
#pragma warning disable IDE1006

    public partial class VaxDisassembler : DisassemblerBase<VaxInstruction, Mnemonic>
    {
        private readonly VaxArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public VaxDisassembler(VaxArchitecture arch, EndianImageReader imageReader)
        {
            this.arch = arch;
            this.rdr = imageReader;
            this.ops = new List<MachineOperand>();
        }

        public override VaxInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte op))
                return null;
            VaxInstruction instr;
            try
            {
                instr = oneByteInstructions[op].Decode(op, this);
            }
            catch
            {
                instr = CreateInvalidInstruction();
            }
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            ops.Clear();
            return instr;
        }

        public override VaxInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new VaxInstruction
            {
                Mnemonic = mnemonic,
                InstructionClass = iclass,
                Operands = this.ops.ToArray()
            };
            return instr;
        }

        public override VaxInstruction CreateInvalidInstruction()
        {
            return new VaxInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = Array.Empty<MachineOperand>()
            };
        }

        public override VaxInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("VaxDis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        private bool TryDecodeOperand(PrimitiveType width, int maxReg, out MachineOperand op)
        {
            op = null!;
            if (!rdr.TryReadByte(out byte bSpecifier))
            {
                return false;
            }
            var reg = arch.GetRegister(bSpecifier & 0xF)!;
            switch (bSpecifier >> 4)
            {
            case 0: // Literal mode
            case 1:
            case 2:
            case 3:
                op = LiteralOperand(width, bSpecifier);
                break;
            case 4: // Index mode
                op = IndexOperand(width, reg)!;
                if (op is null)
                    return false;
                break;
            case 5: // Register mode
                if (reg.Number > maxReg)
                    return false;
                op = reg;
                break;
            case 6: // Register deferred
                op = new MemoryOperand(width)
                {
                    Base = reg
                };
                break;
            case 7: // Autodecrement mode
                op = new MemoryOperand(width)
                {
                    Base = reg,
                    AutoDecrement = true,
                };
                break;
            case 8: // Autoincrement mode
                if (reg.Number == 0x0F)
                {
                    op = ImmediateOperand(width)!;
                    return op is not null;

                }
                else
                {
                    op = new MemoryOperand(width)
                    {
                        Base = reg,
                        AutoIncrement = true,
                    };
                }
                break;
            case 9: // Deferred Autoincrement mode
                op = new MemoryOperand(width)
                {
                    Base = reg,
                    AutoIncrement = true,
                    Deferred = true,
                };
                break;
            case 0xA: // Displacement mode
            case 0xB:
                if (!rdr.TryReadByte(out byte b))
                    return false;
                op = DisplacementOperand(width, reg, Constant.SByte((sbyte)b), bSpecifier);
                break;
            case 0xC:
            case 0xD:
                if (!rdr.TryReadUInt16(out ushort w))
                    return false;
                op = DisplacementOperand(width, reg, Constant.Int16((short)w), bSpecifier);
                break;
            case 0xE:
            case 0xF:
                if (!rdr.TryReadUInt32(out uint dw))
                    return false;
                op = DisplacementOperand(width, reg, Constant.Word32(dw), bSpecifier);
                break;
            default:
                throw new InvalidCastException("Impossiburu!");
            }
            return true;
        }

        private Constant? ImmediateOperand(PrimitiveType width)
        {
            if (!rdr.TryRead(width, out Constant? imm))
                return null;
            return imm;
        }

        private MachineOperand? IndexOperand(PrimitiveType width, RegisterStorage reg)
        {
            if (!TryDecodeOperand(width, 15, out MachineOperand op))
                return null;
            return new IndexOperand(width, op, reg);
        }

        private MachineOperand DisplacementOperand(PrimitiveType width, RegisterStorage reg, Constant c, byte bSpecifier)
        {
            bool deferred = ((bSpecifier >> 4) & 0x1) != 0;
            if (reg.Number == 15)
            {
                var addr = Address.Ptr32((uint) ((int) rdr.Address.ToLinear() + c.ToInt32()));
                return new MemoryOperand(width)
                {
                    Offset = addr.ToConstant(),
                    Deferred = deferred
                };
            }
            return new MemoryOperand(width)
            {
                Base = reg,
                Offset = c,
                Deferred = deferred
            };
        }

        public static MachineOperand LiteralOperand(PrimitiveType width, byte b)
        {
            Constant c;
            if (width.Domain == Domain.Real)
            {
                float exp = 1 << ((b >> 3) & 7);
                float frac = 8 | (b & 7);
                if (width.BitSize == 32)
                {
                    c = Constant.Real32(frac * exp / 16.0F);
                }
                else if (width.BitSize == 64)
                {
                    c = Constant.Real64(frac * exp / 16.0F);
                }
                else
                    throw new NotImplementedException();
            }
            else
            {
                c = Constant.Create(width, b & 0x3F);
            }
            return c;
        }


        public static Decoder Instr(Mnemonic mnemonic, params Mutator<VaxDisassembler> [] mutators)
            {
            return new InstrDecoder<VaxDisassembler, Mnemonic, VaxInstruction>(InstrClass.Linear, mnemonic, mutators);
            }

        public static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<VaxDisassembler>[] mutators)
            {
            return new InstrDecoder<VaxDisassembler, Mnemonic, VaxInstruction>(iclass, mnemonic, mutators);
                    }

        public static Decoder Instr(Mnemonic mnemonic, int ignored)
        {
            return new InstrDecoder<VaxDisassembler, Mnemonic, VaxInstruction>(InstrClass.Linear, mnemonic);
        }

        private static Mutator a(PrimitiveType dt)
        {
            return (u, d) =>
            {
                if (!d.TryDecodeOperand(dt, 15, out var op))
                    return false;
                d.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator ab = a(PrimitiveType.Byte);
        private static readonly Mutator aw = a(PrimitiveType.Word16);
        private static readonly Mutator al = a(PrimitiveType.Word32);
        private static readonly Mutator aq = a(PrimitiveType.Word64);


        private static Mutator b(PrimitiveType width)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadLeSigned(width, out long jOffset))
                    return false;
                uint uAddr = (uint) ((long) d.rdr.Address.Offset + jOffset);
                d.ops.Add(Address.Ptr32(uAddr));
                return true;
            };
        }

        private static readonly Mutator bb = b(PrimitiveType.Byte);
        private static readonly Mutator bw = b(PrimitiveType.Word16);
        private static readonly Mutator bl = b(PrimitiveType.Word32);


        private static Mutator r(PrimitiveType dt, int maxReg = 15)
        {
            return (u, d) =>
            {
                if (!d.TryDecodeOperand(dt, maxReg, out var op))
                    return false;
                d.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator rb = r(PrimitiveType.Byte);
        private static readonly Mutator rw = r(PrimitiveType.Word16);
        private static readonly Mutator rl = r(PrimitiveType.Word32);
        private static readonly Mutator rf = r(PrimitiveType.Real32, 14);  //$TODO: this is not IEEE
        private static readonly Mutator rd = r(PrimitiveType.Real64, 13);  //$TODO: this is not IEEE
        private static readonly Mutator rg = r(PrimitiveType.Real64, 13);  //$TODO: this is not IEEE
        private static readonly Mutator rh = r(PrimitiveType.Real128, 11);  //$TODO: this is not IEEE
        private static readonly Mutator rq = r(PrimitiveType.Word64, 14);

        private static readonly Mutator vb = r(PrimitiveType.Byte);


        private static Mutator w(PrimitiveType dt, int maxReg = 15)
        {
            return (u, d) =>
            {
                if (!d.TryDecodeOperand(dt, maxReg, out var op))
                    return false;
                if (op is Constant)
                    return false;    // Can't modify a constant! 
                d.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator wb = w(PrimitiveType.Byte);
        private static readonly Mutator ww = w(PrimitiveType.Word16);
        private static readonly Mutator wl = w(PrimitiveType.Word32);
        private static readonly Mutator wf = w(PrimitiveType.Real32, 14);  //$TODO: this is not IEEE
        private static readonly Mutator wd = w(PrimitiveType.Real64, 13);  //$TODO: this is not IEEE
        private static readonly Mutator wg = w(PrimitiveType.Real64, 13);  //$TODO: this is not IEEE
        private static readonly Mutator wh = w(PrimitiveType.Real128, 11);  //$TODO: this is not IEEE
        private static readonly Mutator wq = w(PrimitiveType.Word64, 14);

        private static readonly Mutator mb = w(PrimitiveType.Byte);
        private static readonly Mutator mw = w(PrimitiveType.Word16);
        private static readonly Mutator ml = w(PrimitiveType.Word32);
        private static readonly Mutator mf = w(PrimitiveType.Real32, 14);  //$TODO: this is not IEEE
        private static readonly Mutator md = w(PrimitiveType.Real64, 13);  //$TODO: this is not IEEE
        private static readonly Mutator mg = w(PrimitiveType.Real64, 13);  //$TODO: this is not IEEE
        private static readonly Mutator mh = w(PrimitiveType.Real128, 11);  //$TODO: this is not IEEE
    }
}
