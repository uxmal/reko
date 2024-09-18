#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

#pragma warning disable IDE1066,IDE1006

// Based on https://www.pastraiser.com/cpu/gameboy/gameboy_opcodes.html

namespace Reko.Environments.Gameboy
{
    using Decoder = Decoder<GameboyDisassembler, Mnemonic, GameboyInstruction>;

    /// <summary>
    /// Disassembles LR35902 instructions.
    /// </summary>
    public class GameboyDisassembler : DisassemblerBase<GameboyInstruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly GameboyArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> operands;
        private Address addr;

        public GameboyDisassembler(GameboyArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.operands = new List<MachineOperand>();
            this.addr = default!;
        }

        public override GameboyInstruction? DisassembleInstruction()
        {
            addr = rdr.Address;
            if (!rdr.TryReadByte(out byte opcode))
                return null;
            var instr = rootDecoder.Decode(opcode, this);
            operands.Clear();
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override GameboyInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new GameboyInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = operands.ToArray(),
            };
        }

        public override GameboyInstruction CreateInvalidInstruction()
        {
            return new GameboyInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
            };
        }

        public override GameboyInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("GameboyDis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<GameboyDisassembler>[] mutators)
        {
            return new InstrDecoder<GameboyDisassembler, Mnemonic, GameboyInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<GameboyDisassembler>[] mutators)
        {
            return new InstrDecoder<GameboyDisassembler, Mnemonic, GameboyInstruction>(iclass, mnemonic, mutators);
        }

        private static Mutator<GameboyDisassembler> Reg(RegisterStorage reg)
        {
            return (uint uInstr, GameboyDisassembler dasm) =>
            {
                dasm.operands.Add(reg);
                return true;
            };
        }

        private static readonly Mutator<GameboyDisassembler> A = Reg(Registers.a);
        private static readonly Mutator<GameboyDisassembler> F = Reg(Registers.f);
        private static readonly Mutator<GameboyDisassembler> B = Reg(Registers.b);
        private static readonly Mutator<GameboyDisassembler> C = Reg(Registers.c);
        private static readonly Mutator<GameboyDisassembler> D = Reg(Registers.d);
        private static readonly Mutator<GameboyDisassembler> E = Reg(Registers.e);
        private static readonly Mutator<GameboyDisassembler> H = Reg(Registers.h);
        private static readonly Mutator<GameboyDisassembler> L = Reg(Registers.l);
        private static readonly Mutator<GameboyDisassembler> AF = Reg(Registers.af);
        private static readonly Mutator<GameboyDisassembler> BC = Reg(Registers.bc);
        private static readonly Mutator<GameboyDisassembler> DE = Reg(Registers.de);
        private static readonly Mutator<GameboyDisassembler> HL = Reg(Registers.hl);
        private static readonly Mutator<GameboyDisassembler> SP = Reg(Registers.sp);

        private static bool M_bc(uint opcode, GameboyDisassembler dasm)
        {
            var mem = new MemoryOperand(PrimitiveType.Byte)
            {
                Base = Registers.bc,
                Offset = 0,
            };
            dasm.operands.Add(mem);
            return true;
        }

        private static bool M_de(uint opcode, GameboyDisassembler dasm)
        {
            var mem = new MemoryOperand(PrimitiveType.Byte)
            {
                Base = Registers.de,
                Offset = 0,
            };
            dasm.operands.Add(mem);
            return true;
        }

        private static bool M_hl(uint opcode, GameboyDisassembler dasm)
        {
            var mem = new MemoryOperand(PrimitiveType.Byte)
            {
                Base = Registers.hl,
                Offset = 0,
            };
            dasm.operands.Add(mem);
            return true;
        }

        private static bool M_hl_w(uint opcode, GameboyDisassembler dasm)
        {
            var mem = new MemoryOperand(PrimitiveType.Word16)
            {
                Base = Registers.hl,
                Offset = 0,
            };
            dasm.operands.Add(mem);
            return true;
        }

        private static bool M_hl_inc(uint opcode, GameboyDisassembler dasm)
        {
            var mem = new MemoryOperand(PrimitiveType.Byte)
            {
                Base = Registers.hl,
                Offset = 0,
                PostIncrement = true,
            };
            dasm.operands.Add(mem);
            return true;
        }

        private static bool M_hl_dec(uint opcode, GameboyDisassembler dasm)
        {
            var mem = new MemoryOperand(PrimitiveType.Byte)
            {
                Base = Registers.hl,
                Offset = 0,
                PostDecrement = true,
            };
            dasm.operands.Add(mem);
            return true;
        }

        private static bool M_c(uint opcode, GameboyDisassembler dasm)
        {
            var mem = new MemoryOperand(PrimitiveType.Byte)
            {
                Base = Registers.c,
                Offset = 0,
            };
            dasm.operands.Add(mem);
            return true;
        }

        private static bool M_a8(uint opcode, GameboyDisassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;

            var mem = new MemoryOperand(PrimitiveType.Byte)
            {
                Base = null,
                Offset = b,
            };
            dasm.operands.Add(mem);
            return true;
        }

        private static bool M_a16(uint opcode, GameboyDisassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out ushort w))
                return false;

            var mem = new MemoryOperand(PrimitiveType.Byte)
            {
                Base = null,
                Offset = w,
            };
            dasm.operands.Add(mem);
            return true;
        }

        private static bool M_a16_w(uint opcode, GameboyDisassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out ushort w))
                return false;

            var mem = new MemoryOperand(PrimitiveType.Word16)
            {
                Base = null,
                Offset = w,
            };
            dasm.operands.Add(mem);
            return true;
        }


        private static Mutator<GameboyDisassembler> Implicit(int n)
        {
            var imm = ImmediateOperand.Byte((byte) n);
            return (u, d) =>
            {
                d.operands.Add(imm);
                return true;
            };
        }

        /// <summary>
        /// Reads a unsigned byte immediate value.
        /// </summary>
        private static bool d8(uint opcode, GameboyDisassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            var imm = ImmediateOperand.Byte((byte) b);
            dasm.operands.Add(imm);
            return true;
        }

        /// <summary>
        /// Reads a signed byte immediate value.
        /// </summary>
        private static bool s8(uint opcode, GameboyDisassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            var imm = ImmediateOperand.SByte((sbyte) b);
            dasm.operands.Add(imm);
            return true;
        }

        private static bool d16(uint opcode, GameboyDisassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out ushort w))
                return false;
            var imm = ImmediateOperand.Word16(w);
            dasm.operands.Add(imm);
            return true;
        }

        private static bool a16(uint opcode, GameboyDisassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out ushort w))
                return false;
            var imm = Address.Ptr16(w);
            dasm.operands.Add(imm);
            return true;
        }

        private static bool r8(uint opcode, GameboyDisassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            int displacement = (sbyte) b;
            var addrTarget = dasm.rdr.Address + displacement;
            dasm.operands.Add(addrTarget);
            return true;
        }

        private static Mutator<GameboyDisassembler> CCode(CCode cc)
        {
            var op = new ConditionOperand(cc);
            return (u, d) =>
            {
                d.operands.Add(op);
                return true;
            };
        }

        private static readonly Mutator<GameboyDisassembler> Z = CCode(Gameboy.CCode.Z);
        private static readonly Mutator<GameboyDisassembler> Cy = CCode(Gameboy.CCode.C);
        private static readonly Mutator<GameboyDisassembler> NZ = CCode(Gameboy.CCode.NZ);
        private static readonly Mutator<GameboyDisassembler> NC = CCode(Gameboy.CCode.NC);


        private static bool SP_r8(uint opcode, GameboyDisassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte offset))
                return false;
            var op = new StackOperand(Registers.sp, (sbyte) offset);
            dasm.operands.Add(op);
            return true;
        }

        private class PrefixDecoder : Decoder
        {
            private readonly Decoder subdecoder;

            public PrefixDecoder(Decoder subdecoder)
            {
                this.subdecoder = subdecoder;
            }

            public override GameboyInstruction Decode(uint wInstr, GameboyDisassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out byte uInstr2))
                    return dasm.CreateInvalidInstruction();
                return subdecoder.Decode(uInstr2, dasm);
            }
        }

        static GameboyDisassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

            // CB extension decoders
            var cbDecoders = Mask(0, 8, "LR35902",
                // 00
                Instr(Mnemonic.rlc, B), // Z 0 0 C
                Instr(Mnemonic.rlc, C), // Z 0 0 C
                Instr(Mnemonic.rlc, D), // Z 0 0 C
                Instr(Mnemonic.rlc, E), // Z 0 0 C
                Instr(Mnemonic.rlc, H), // Z 0 0 C
                Instr(Mnemonic.rlc, L), // Z 0 0 C
                Instr(Mnemonic.rlc, M_hl), // Z 0 0 C
                Instr(Mnemonic.rlc, A), // Z 0 0 C
                Instr(Mnemonic.rrc, B), // Z 0 0 C
                Instr(Mnemonic.rrc, C), // Z 0 0 C
                Instr(Mnemonic.rrc, D), // Z 0 0 C
                Instr(Mnemonic.rrc, E), // Z 0 0 C
                Instr(Mnemonic.rrc, H), // Z 0 0 C
                Instr(Mnemonic.rrc, L), // Z 0 0 C
                Instr(Mnemonic.rrc, M_hl), // Z 0 0 C
                Instr(Mnemonic.rrc, A), // Z 0 0 C

                // 10
                Instr(Mnemonic.rl, B), // Z 0 0 C
                Instr(Mnemonic.rl, C), // Z 0 0 C
                Instr(Mnemonic.rl, D), // Z 0 0 C
                Instr(Mnemonic.rl, E), // Z 0 0 C
                Instr(Mnemonic.rl, H), // Z 0 0 C
                Instr(Mnemonic.rl, L), // Z 0 0 C
                Instr(Mnemonic.rl, M_hl), // Z 0 0 C
                Instr(Mnemonic.rl, A), // Z 0 0 C
                Instr(Mnemonic.rr, B), // Z 0 0 C
                Instr(Mnemonic.rr, C), // Z 0 0 C
                Instr(Mnemonic.rr, D), // Z 0 0 C
                Instr(Mnemonic.rr, E), // Z 0 0 C
                Instr(Mnemonic.rr, H), // Z 0 0 C
                Instr(Mnemonic.rr, L), // Z 0 0 C
                Instr(Mnemonic.rr, M_hl), // Z 0 0 C
                Instr(Mnemonic.rr, A), // Z 0 0 C

                // 20
                Instr(Mnemonic.sla, B), // Z 0 0 C
                Instr(Mnemonic.sla, C), // Z 0 0 C
                Instr(Mnemonic.sla, D), // Z 0 0 C
                Instr(Mnemonic.sla, E), // Z 0 0 C
                Instr(Mnemonic.sla, H), // Z 0 0 C
                Instr(Mnemonic.sla, L), // Z 0 0 C
                Instr(Mnemonic.sla, M_hl), // Z 0 0 C
                Instr(Mnemonic.sla, A), // Z 0 0 C
                Instr(Mnemonic.sra, B), // Z 0 0 0
                Instr(Mnemonic.sra, C), // Z 0 0 0
                Instr(Mnemonic.sra, D), // Z 0 0 0
                Instr(Mnemonic.sra, E), // Z 0 0 0
                Instr(Mnemonic.sra, H), // Z 0 0 0
                Instr(Mnemonic.sra, L), // Z 0 0 0
                Instr(Mnemonic.sra, M_hl), // Z 0 0 0
                Instr(Mnemonic.sra, A), // Z 0 0 0

                // 30
                Instr(Mnemonic.swap, B), // Z 0 0 0
                Instr(Mnemonic.swap, C), // Z 0 0 0
                Instr(Mnemonic.swap, D), // Z 0 0 0
                Instr(Mnemonic.swap, E), // Z 0 0 0
                Instr(Mnemonic.swap, H), // Z 0 0 0
                Instr(Mnemonic.swap, L), // Z 0 0 0
                Instr(Mnemonic.swap, M_hl), // Z 0 0 0
                Instr(Mnemonic.swap, A), // Z 0 0 0
                Instr(Mnemonic.srl, B), // Z 0 0 C
                Instr(Mnemonic.srl, C), // Z 0 0 C
                Instr(Mnemonic.srl, D), // Z 0 0 C
                Instr(Mnemonic.srl, E), // Z 0 0 C
                Instr(Mnemonic.srl, H), // Z 0 0 C
                Instr(Mnemonic.srl, L), // Z 0 0 C
                Instr(Mnemonic.srl, M_hl), // Z 0 0 C
                Instr(Mnemonic.srl, A), // Z 0 0 C

                // 40
                Instr(Mnemonic.bit, Implicit(0), B), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(0), C), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(0), D), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(0), E), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(0), H), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(0), L), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(0), M_hl), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(0), A), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(1), B), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(1), C), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(1), D), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(1), E), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(1), H), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(1), L), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(1), M_hl), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(1), A), // Z 0 1 -

                // 50
                Instr(Mnemonic.bit, Implicit(2), B), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(2), C), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(2), D), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(2), E), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(2), H), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(2), L), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(2), M_hl), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(2), A), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(3), B), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(3), C), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(3), D), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(3), E), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(3), H), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(3), L), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(3), M_hl), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(3), A), // Z 0 1 -

                // 60
                Instr(Mnemonic.bit, Implicit(4), B), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(4), C), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(4), D), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(4), E), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(4), H), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(4), L), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(4), M_hl), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(4), A), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(5), B), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(5), C), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(5), D), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(5), E), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(5), H), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(5), L), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(5), M_hl), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(5), A), // Z 0 1 -

                // 70
                Instr(Mnemonic.bit, Implicit(6), B), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(6), C), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(6), D), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(6), E), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(6), H), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(6), L), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(6), M_hl), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(6), A), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(7), B), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(7), C), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(7), D), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(7), E), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(7), H), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(7), L), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(7), M_hl), // Z 0 1 -
                Instr(Mnemonic.bit, Implicit(7), A), // Z 0 1 -

                // 80
                Instr(Mnemonic.res, Implicit(0), B), // - - - -
                Instr(Mnemonic.res, Implicit(0), C), // - - - -
                Instr(Mnemonic.res, Implicit(0), D), // - - - -
                Instr(Mnemonic.res, Implicit(0), E), // - - - -
                Instr(Mnemonic.res, Implicit(0), H), // - - - -
                Instr(Mnemonic.res, Implicit(0), L), // - - - -
                Instr(Mnemonic.res, Implicit(0), M_hl), // - - - -
                Instr(Mnemonic.res, Implicit(0), A), // - - - -
                Instr(Mnemonic.res, Implicit(1), B), // - - - -
                Instr(Mnemonic.res, Implicit(1), C), // - - - -
                Instr(Mnemonic.res, Implicit(1), D), // - - - -
                Instr(Mnemonic.res, Implicit(1), E), // - - - -
                Instr(Mnemonic.res, Implicit(1), H), // - - - -
                Instr(Mnemonic.res, Implicit(1), L), // - - - -
                Instr(Mnemonic.res, Implicit(1), M_hl), // - - - -
                Instr(Mnemonic.res, Implicit(1), A), // - - - -

                // 90
                Instr(Mnemonic.res, Implicit(2), B), // - - - -
                Instr(Mnemonic.res, Implicit(2), C), // - - - -
                Instr(Mnemonic.res, Implicit(2), D), // - - - -
                Instr(Mnemonic.res, Implicit(2), E), // - - - -
                Instr(Mnemonic.res, Implicit(2), H), // - - - -
                Instr(Mnemonic.res, Implicit(2), L), // - - - -
                Instr(Mnemonic.res, Implicit(2), M_hl), // - - - -
                Instr(Mnemonic.res, Implicit(2), A), // - - - -
                Instr(Mnemonic.res, Implicit(3), B), // - - - -
                Instr(Mnemonic.res, Implicit(3), C), // - - - -
                Instr(Mnemonic.res, Implicit(3), D), // - - - -
                Instr(Mnemonic.res, Implicit(3), E), // - - - -
                Instr(Mnemonic.res, Implicit(3), H), // - - - -
                Instr(Mnemonic.res, Implicit(3), L), // - - - -
                Instr(Mnemonic.res, Implicit(3), M_hl), // - - - -
                Instr(Mnemonic.res, Implicit(3), A),    // - - - -

                // A0
                Instr(Mnemonic.res, Implicit(4), B), // - - - -
                Instr(Mnemonic.res, Implicit(4), C), // - - - -
                Instr(Mnemonic.res, Implicit(4), D), // - - - -
                Instr(Mnemonic.res, Implicit(4), E), // - - - -
                Instr(Mnemonic.res, Implicit(4), H), // - - - -
                Instr(Mnemonic.res, Implicit(4), L), // - - - -
                Instr(Mnemonic.res, Implicit(4), M_hl), // - - - -
                Instr(Mnemonic.res, Implicit(4), A), // - - - -
                Instr(Mnemonic.res, Implicit(5), B), // - - - -
                Instr(Mnemonic.res, Implicit(5), C), // - - - -
                Instr(Mnemonic.res, Implicit(5), D), // - - - -
                Instr(Mnemonic.res, Implicit(5), E), // - - - -
                Instr(Mnemonic.res, Implicit(5), H), // - - - -
                Instr(Mnemonic.res, Implicit(5), L), // - - - -
                Instr(Mnemonic.res, Implicit(5), M_hl), // - - - -
                Instr(Mnemonic.res, Implicit(5), A), // - - - -

                // B0
                Instr(Mnemonic.res, Implicit(6), B), // - - - -
                Instr(Mnemonic.res, Implicit(6), C), // - - - -
                Instr(Mnemonic.res, Implicit(6), D), // - - - -
                Instr(Mnemonic.res, Implicit(6), E), // - - - -
                Instr(Mnemonic.res, Implicit(6), H), // - - - -
                Instr(Mnemonic.res, Implicit(6), L), // - - - -
                Instr(Mnemonic.res, Implicit(6), M_hl), // - - - -
                Instr(Mnemonic.res, Implicit(6), A), // - - - -
                Instr(Mnemonic.res, Implicit(7), B), // - - - -
                Instr(Mnemonic.res, Implicit(7), C), // - - - -
                Instr(Mnemonic.res, Implicit(7), D), // - - - -
                Instr(Mnemonic.res, Implicit(7), E), // - - - -
                Instr(Mnemonic.res, Implicit(7), H), // - - - -
                Instr(Mnemonic.res, Implicit(7), L), // - - - -
                Instr(Mnemonic.res, Implicit(7), M_hl), // - - - -
                Instr(Mnemonic.res, Implicit(7), A), // - - - -

                // C0
                Instr(Mnemonic.set, Implicit(0), B), // - - - -
                Instr(Mnemonic.set, Implicit(0), C), // - - - -
                Instr(Mnemonic.set, Implicit(0), D), // - - - -
                Instr(Mnemonic.set, Implicit(0), E), // - - - -
                Instr(Mnemonic.set, Implicit(0), H), // - - - -
                Instr(Mnemonic.set, Implicit(0), L), // - - - -
                Instr(Mnemonic.set, Implicit(0), M_hl), // - - - -
                Instr(Mnemonic.set, Implicit(0), A), // - - - -
                Instr(Mnemonic.set, Implicit(1), B), // - - - -
                Instr(Mnemonic.set, Implicit(1), C), // - - - -
                Instr(Mnemonic.set, Implicit(1), D), // - - - -
                Instr(Mnemonic.set, Implicit(1), E), // - - - -
                Instr(Mnemonic.set, Implicit(1), H), // - - - -
                Instr(Mnemonic.set, Implicit(1), L), // - - - -
                Instr(Mnemonic.set, Implicit(1), M_hl), // - - - -
                Instr(Mnemonic.set, Implicit(1), A), // - - - -

                // D0
                Instr(Mnemonic.set, Implicit(2), B), // - - - -
                Instr(Mnemonic.set, Implicit(2), C), // - - - -
                Instr(Mnemonic.set, Implicit(2), D), // - - - -
                Instr(Mnemonic.set, Implicit(2), E), // - - - -
                Instr(Mnemonic.set, Implicit(2), H), // - - - -
                Instr(Mnemonic.set, Implicit(2), L), // - - - -
                Instr(Mnemonic.set, Implicit(2), M_hl), // - - - -
                Instr(Mnemonic.set, Implicit(2), A), // - - - -
                Instr(Mnemonic.set, Implicit(3), B), // - - - -
                Instr(Mnemonic.set, Implicit(3), C), // - - - -
                Instr(Mnemonic.set, Implicit(3), D), // - - - -
                Instr(Mnemonic.set, Implicit(3), E), // - - - -
                Instr(Mnemonic.set, Implicit(3), H), // - - - -
                Instr(Mnemonic.set, Implicit(3), L), // - - - -
                Instr(Mnemonic.set, Implicit(3), M_hl), // - - - -
                Instr(Mnemonic.set, Implicit(3), A), // - - - -

                // E0
                Instr(Mnemonic.set, Implicit(4), B), // - - - -
                Instr(Mnemonic.set, Implicit(4), C), // - - - -
                Instr(Mnemonic.set, Implicit(4), D), // - - - -
                Instr(Mnemonic.set, Implicit(4), E), // - - - -
                Instr(Mnemonic.set, Implicit(4), H), // - - - -
                Instr(Mnemonic.set, Implicit(4), L), // - - - -
                Instr(Mnemonic.set, Implicit(4), M_hl), // - - - -
                Instr(Mnemonic.set, Implicit(4), A), // - - - -
                Instr(Mnemonic.set, Implicit(5), B), // - - - -
                Instr(Mnemonic.set, Implicit(5), C), // - - - -
                Instr(Mnemonic.set, Implicit(5), D), // - - - -
                Instr(Mnemonic.set, Implicit(5), E), // - - - -
                Instr(Mnemonic.set, Implicit(5), H), // - - - -
                Instr(Mnemonic.set, Implicit(5), L), // - - - -
                Instr(Mnemonic.set, Implicit(5), M_hl), // - - - -
                Instr(Mnemonic.set, Implicit(5), A), // - - - -

                // F0
                Instr(Mnemonic.set, Implicit(6), B),    // - - - -
                Instr(Mnemonic.set, Implicit(6), C),    // - - - -
                Instr(Mnemonic.set, Implicit(6), D),    // - - - -
                Instr(Mnemonic.set, Implicit(6), E),    // - - - -
                Instr(Mnemonic.set, Implicit(6), H),    // - - - -
                Instr(Mnemonic.set, Implicit(6), L),    // - - - -
                Instr(Mnemonic.set, Implicit(6), M_hl), // - - - -
                Instr(Mnemonic.set, Implicit(6), A),    // - - - -
                Instr(Mnemonic.set, Implicit(7), B),    // - - - -
                Instr(Mnemonic.set, Implicit(7), C),    // - - - -
                Instr(Mnemonic.set, Implicit(7), D),    // - - - -
                Instr(Mnemonic.set, Implicit(7), E),    // - - - -
                Instr(Mnemonic.set, Implicit(7), H),    // - - - -
                Instr(Mnemonic.set, Implicit(7), L),    // - - - -
                Instr(Mnemonic.set, Implicit(7), M_hl), // - - - -
                Instr(Mnemonic.set, Implicit(7), A));   // - - - -

            rootDecoder = Mask(0, 8, "LR5902",
                Instr(Mnemonic.nop),                // - - - -
                Instr(Mnemonic.ld, BC, d16),        // - - - -
                Instr(Mnemonic.ld, M_bc, A),        // - - - -
                Instr(Mnemonic.inc, BC),            // - - - -
                Instr(Mnemonic.inc, B),             // Z 0 H -
                Instr(Mnemonic.dec, B),             // Z 1 H -
                Instr(Mnemonic.ld, B, d8),          // - - - -
                Instr(Mnemonic.rlca),               // 0 0 0 C
                Instr(Mnemonic.ld, M_a16_w, SP),    // - - - -
                Instr(Mnemonic.add, HL, BC),        // - 0 H C
                Instr(Mnemonic.ld, A, M_bc),        // - - - -
                Instr(Mnemonic.dec, BC),            // - - - -
                Instr(Mnemonic.inc, C),             // Z 0 H -
                Instr(Mnemonic.dec, C),             // Z 1 H -
                Instr(Mnemonic.ld, C, d8),          // - - - -
                Instr(Mnemonic.rrca),               // 0 0 0 C

                // 10
                Instr(Mnemonic.stop, InstrClass.Terminates),            // - - - -
                Instr(Mnemonic.ld, DE, d16),        // - - - -
                Instr(Mnemonic.ld, M_de, A),        // - - - -
                Instr(Mnemonic.inc, DE),            // - - - -
                Instr(Mnemonic.inc, D),             // Z 0 H -
                Instr(Mnemonic.dec, D),             // Z 1 H -
                Instr(Mnemonic.ld, D, d8),          // - - - -
                Instr(Mnemonic.rla),                // 0 0 0 C
                Instr(Mnemonic.jr, InstrClass.Transfer, r8),             // - - - -
                Instr(Mnemonic.add, HL, DE),        // - 0 H C
                Instr(Mnemonic.ld, A, M_de),        // - - - -
                Instr(Mnemonic.dec, DE),            // - - - -
                Instr(Mnemonic.inc, E),             // Z 0 H -
                Instr(Mnemonic.dec, E),             // Z 1 H -
                Instr(Mnemonic.ld, E, d8),          // - - - -
                Instr(Mnemonic.rra),                // 0 0 0 C

                // 20
                Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, NZ, r8),         // - - - -
                Instr(Mnemonic.ld, HL, d16),        // - - - -
                Instr(Mnemonic.ld, M_hl_inc, A),    // - - - -
                Instr(Mnemonic.inc, HL),            // - - - -
                Instr(Mnemonic.inc, H),             // Z 0 H -
                Instr(Mnemonic.dec, H),             // Z 1 H -
                Instr(Mnemonic.ld, H, d8),          // - - - -
                Instr(Mnemonic.daa),                // Z - 0 C
                Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, Z, r8),          // - - - -
                Instr(Mnemonic.add, HL, HL),        // - 0 H C
                Instr(Mnemonic.ld, A, (M_hl_inc)),  // - - - -
                Instr(Mnemonic.dec, HL),            // - - - -
                Instr(Mnemonic.inc, L),             // Z 0 H -
                Instr(Mnemonic.dec, L),             // Z 1 H -
                Instr(Mnemonic.ld, L, d8),          // - - - -
                Instr(Mnemonic.cpl),                // - 1 1 -

                // 30
                Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, NC, r8),         // - - - -
                Instr(Mnemonic.ld, SP, d16),        // - - - -
                Instr(Mnemonic.ld, M_hl_dec, A),    // - - - -
                Instr(Mnemonic.inc, SP),            // - - - -
                Instr(Mnemonic.inc, M_hl),          // Z 0 H -
                Instr(Mnemonic.dec, M_hl),          // Z 1 H -
                Instr(Mnemonic.ld, M_hl, d8),       // - - - -
                Instr(Mnemonic.scf),                // - 0 0 1
                Instr(Mnemonic.jr, InstrClass.ConditionalTransfer, Cy, r8),          // - - - -
                Instr(Mnemonic.add, HL, SP),    // - 0 H C
                Instr(Mnemonic.ld, A, M_hl_dec),  // - - - -
                Instr(Mnemonic.dec, SP),            // - - - -
                Instr(Mnemonic.inc, A),             // Z 0 H -
                Instr(Mnemonic.dec, A),             // Z 1 H -
                Instr(Mnemonic.ld, A, d8),          // - - - -
                Instr(Mnemonic.ccf),                // - 0 0 C

                // 40
                Instr(Mnemonic.ld, B, B), // - - - -
                Instr(Mnemonic.ld, B, C), // - - - -
                Instr(Mnemonic.ld, B, D), // - - - -
                Instr(Mnemonic.ld, B, E), // - - - -
                Instr(Mnemonic.ld, B, H), // - - - -
                Instr(Mnemonic.ld, B, L), // - - - -
                Instr(Mnemonic.ld, B, M_hl), // - - - -
                Instr(Mnemonic.ld, B, A), // - - - -
                Instr(Mnemonic.ld, C, B), // - - - -
                Instr(Mnemonic.ld, C, C), // - - - -
                Instr(Mnemonic.ld, C, D), // - - - -
                Instr(Mnemonic.ld, C, E), // - - - -
                Instr(Mnemonic.ld, C, H), // - - - -
                Instr(Mnemonic.ld, C, L), // - - - -
                Instr(Mnemonic.ld, C, M_hl), // - - - -
                Instr(Mnemonic.ld, C, A), // - - - -

                // 50
                Instr(Mnemonic.ld, D, B),       // - - - -
                Instr(Mnemonic.ld, D, C),       // - - - -
                Instr(Mnemonic.ld, D, D),       // - - - -
                Instr(Mnemonic.ld, D, E),       // - - - -
                Instr(Mnemonic.ld, D, H),       // - - - -
                Instr(Mnemonic.ld, D, L),       // - - - -
                Instr(Mnemonic.ld, D, M_hl),    // - - - -
                Instr(Mnemonic.ld, D, A),       // - - - -
                Instr(Mnemonic.ld, E, B),       // - - - -
                Instr(Mnemonic.ld, E, C),       // - - - -
                Instr(Mnemonic.ld, E, D),       // - - - -
                Instr(Mnemonic.ld, E, E),       // - - - -
                Instr(Mnemonic.ld, E, H), // - - - -
                Instr(Mnemonic.ld, E, L), // - - - -
                Instr(Mnemonic.ld, E, M_hl), // - - - -
                Instr(Mnemonic.ld, E, A), // - - - -

                // 60
                Instr(Mnemonic.ld, H, B), // - - - -
                Instr(Mnemonic.ld, H, C), // - - - -
                Instr(Mnemonic.ld, H, D), // - - - -
                Instr(Mnemonic.ld, H, E), // - - - -
                Instr(Mnemonic.ld, H, H), // - - - -
                Instr(Mnemonic.ld, H, L), // - - - -
                Instr(Mnemonic.ld, H, M_hl), // - - - -
                Instr(Mnemonic.ld, H, A), // - - - -
                Instr(Mnemonic.ld, L, B), // - - - -
                Instr(Mnemonic.ld, L, C), // - - - -
                Instr(Mnemonic.ld, L, D), // - - - -
                Instr(Mnemonic.ld, L, E), // - - - -
                Instr(Mnemonic.ld, L, H), // - - - -
                Instr(Mnemonic.ld, L, L), // - - - -
                Instr(Mnemonic.ld, L, M_hl), // - - - -
                Instr(Mnemonic.ld, L, A), // - - - -

                // 70
                Instr(Mnemonic.ld, M_hl, B), // - - - -
                Instr(Mnemonic.ld, M_hl, C), // - - - -
                Instr(Mnemonic.ld, M_hl, D), // - - - -
                Instr(Mnemonic.ld, M_hl, E), // - - - -
                Instr(Mnemonic.ld, M_hl, H), // - - - -
                Instr(Mnemonic.ld, M_hl, L), // - - - -
                Instr(Mnemonic.halt),        // - - - -
                Instr(Mnemonic.ld, M_hl, A), // - - - -
                Instr(Mnemonic.ld, A, B), // - - - -
                Instr(Mnemonic.ld, A, C), // - - - -
                Instr(Mnemonic.ld, A, D), // - - - -
                Instr(Mnemonic.ld, A, E), // - - - -
                Instr(Mnemonic.ld, A, H), // - - - -
                Instr(Mnemonic.ld, A, L), // - - - -
                Instr(Mnemonic.ld, A, M_hl), // - - - -
                Instr(Mnemonic.ld, A, A), // - - - -

                // 80
                Instr(Mnemonic.add, A, B), // Z 0 H C
                Instr(Mnemonic.add, A, C), // Z 0 H C
                Instr(Mnemonic.add, A, D), // Z 0 H C
                Instr(Mnemonic.add, A, E), // Z 0 H C
                Instr(Mnemonic.add, A, H), // Z 0 H C
                Instr(Mnemonic.add, A, L), // Z 0 H C
                Instr(Mnemonic.add, A, M_hl), // Z 0 H C
                Instr(Mnemonic.add, A, A), // Z 0 H C
                Instr(Mnemonic.adc, A, B), // Z 0 H C
                Instr(Mnemonic.adc, A, C), // Z 0 H C
                Instr(Mnemonic.adc, A, D), // Z 0 H C
                Instr(Mnemonic.adc, A, E), // Z 0 H C
                Instr(Mnemonic.adc, A, H), // Z 0 H C
                Instr(Mnemonic.adc, A, L), // Z 0 H C
                Instr(Mnemonic.adc, A, M_hl), // Z 0 H C
                Instr(Mnemonic.adc, A, A), // Z 0 H C

                // 90
                Instr(Mnemonic.sub, B), // Z 1 H C
                Instr(Mnemonic.sub, C), // Z 1 H C
                Instr(Mnemonic.sub, D), // Z 1 H C
                Instr(Mnemonic.sub, E), // Z 1 H C
                Instr(Mnemonic.sub, H), // Z 1 H C
                Instr(Mnemonic.sub, L), // Z 1 H C
                Instr(Mnemonic.sub, M_hl), // Z 1 H C
                Instr(Mnemonic.sub, A), // Z 1 H C
                Instr(Mnemonic.sbc, A, B), // Z 1 H C
                Instr(Mnemonic.sbc, A, C), // Z 1 H C
                Instr(Mnemonic.sbc, A, D), // Z 1 H C
                Instr(Mnemonic.sbc, A, E), // Z 1 H C
                Instr(Mnemonic.sbc, A, H), // Z 1 H C
                Instr(Mnemonic.sbc, A, L), // Z 1 H C
                Instr(Mnemonic.sbc, A, M_hl), // Z 1 H C
                Instr(Mnemonic.sbc, A, A), // Z 1 H C

                // A0
                Instr(Mnemonic.and, B), // Z 0 1 0
                Instr(Mnemonic.and, C), // Z 0 1 0
                Instr(Mnemonic.and, D), // Z 0 1 0
                Instr(Mnemonic.and, E), // Z 0 1 0
                Instr(Mnemonic.and, H), // Z 0 1 0
                Instr(Mnemonic.and, L), // Z 0 1 0
                Instr(Mnemonic.and, M_hl), // Z 0 1 0
                Instr(Mnemonic.and, A), // Z 0 1 0
                Instr(Mnemonic.xor, B), // Z 0 0 0
                Instr(Mnemonic.xor, C), // Z 0 0 0
                Instr(Mnemonic.xor, D), // Z 0 0 0
                Instr(Mnemonic.xor, E), // Z 0 0 0
                Instr(Mnemonic.xor, H), // Z 0 0 0
                Instr(Mnemonic.xor, L), // Z 0 0 0
                Instr(Mnemonic.xor, M_hl), // Z 0 0 0
                Instr(Mnemonic.xor, A), // Z 0 0 0

                // B0
                Instr(Mnemonic.or, B), // Z 0 0 0
                Instr(Mnemonic.or, C), // Z 0 0 0
                Instr(Mnemonic.or, D), // Z 0 0 0
                Instr(Mnemonic.or, E), // Z 0 0 0
                Instr(Mnemonic.or, H), // Z 0 0 0
                Instr(Mnemonic.or, L), // Z 0 0 0
                Instr(Mnemonic.or, M_hl), // Z 0 0 0
                Instr(Mnemonic.or, A), // Z 0 0 0
                Instr(Mnemonic.cp, B), // Z 1 H C
                Instr(Mnemonic.cp, C), // Z 1 H C
                Instr(Mnemonic.cp, D), // Z 1 H C
                Instr(Mnemonic.cp, E), // Z 1 H C
                Instr(Mnemonic.cp, H), // Z 1 H C
                Instr(Mnemonic.cp, L), // Z 1 H C
                Instr(Mnemonic.cp, M_hl),               // Z 1 H C
                Instr(Mnemonic.cp, A),                  // Z 1 H C

                // C0
                Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return | InstrClass.Conditional, NZ),                // - - - -
                Instr(Mnemonic.pop, BC),                // - - - -
                Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, NZ, a16),            // - - - -
                Instr(Mnemonic.jp, InstrClass.Transfer, a16),                // - - - -
                Instr(Mnemonic.call, InstrClass.ConditionalTransfer | InstrClass.Call, NZ, a16),          // - - - -
                Instr(Mnemonic.push, BC),               // - - - -
                Instr(Mnemonic.add, A, d8),             // Z 0 H C
                Instr(Mnemonic.rst, Implicit(00)),      // - - - -
                Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return | InstrClass.Conditional, Z),
                Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return),      
                Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, Z, a16),
                new PrefixDecoder(cbDecoders),
                Instr(Mnemonic.call, InstrClass.ConditionalTransfer | InstrClass.Call, Z, a16),           // - - - -
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a16),              // - - - -
                Instr(Mnemonic.adc, A, d8),             // Z 0 H C
                Instr(Mnemonic.rst, Implicit(08)),      // - - - -

                // D0
                Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return | InstrClass.Conditional , NC),                // - - - -
                Instr(Mnemonic.pop, DE),                // - - - -
                Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, NC, a16),            // - - - -
                invalid,
                Instr(Mnemonic.call, InstrClass.ConditionalTransfer | InstrClass.Call, NC, a16),          // - - - -
                Instr(Mnemonic.push, DE),               // - - - -
                Instr(Mnemonic.sub, d8),                // Z 1 H C
                Instr(Mnemonic.rst, Implicit(0x10)),    // - - - -
                Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return | InstrClass.Conditional, Cy),                // - - - -
                Instr(Mnemonic.reti, InstrClass.Transfer| InstrClass.Return),
                Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, Cy, a16),            // - - - -
                invalid,
                Instr(Mnemonic.call, InstrClass.ConditionalTransfer | InstrClass.Call, Cy, a16),          // - - - -
                invalid,
                Instr(Mnemonic.sbc, A, d8),             // Z 1 H C
                Instr(Mnemonic.rst, Implicit(0x18)),    // - - - -

                // E0
                Instr(Mnemonic.ldh, M_a8, A),           // - - - -
                Instr(Mnemonic.pop, HL),                // - - - -
                Instr(Mnemonic.ld, M_c, A),             // - - - -
                invalid,
                invalid,
                Instr(Mnemonic.push, HL),               // - - - -
                Instr(Mnemonic.and, d8),                // Z 0 1 0
                Instr(Mnemonic.rst, Implicit(0x20)),    // - - - -
                Instr(Mnemonic.add, SP, s8),            // 0 0 H C
                Instr(Mnemonic.jp, InstrClass.Transfer, M_hl_w),             // - - - -
                Instr(Mnemonic.ld, M_a16, A),           // - - - -
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.xor, d8),                // Z 0 0 0
                Instr(Mnemonic.rst, Implicit(0x28)),    // - - - -

                // F0
                Instr(Mnemonic.ldh, A, M_a8),           // - - - -
                Instr(Mnemonic.pop, AF),                // Z N H C
                Instr(Mnemonic.ld, A, M_c),             // - - - -
                Instr(Mnemonic.di),                     // - - - -
                invalid,
                Instr(Mnemonic.push, AF),               // - - - -
                Instr(Mnemonic.or, d8),                 // Z 0 0 0
                Instr(Mnemonic.rst, Implicit(0x30)),    // - - - -
                Instr(Mnemonic.ld, HL, SP_r8),          // 0 0 H C
                Instr(Mnemonic.ld, SP, HL),             // - - - -
                Instr(Mnemonic.ld, A, M_a16),           // - - - -
                Instr(Mnemonic.ei),                     // - - - -
                invalid,
                invalid,
                Instr(Mnemonic.cp, d8),                 // Z 1 H C
                Instr(Mnemonic.rst, Implicit(0x38)));   // - - - -
        }
    }
}