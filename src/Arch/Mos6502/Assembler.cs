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
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using Reko.Core;
using Reko.Core.Assemblers;
using Reko.Core.Expressions;
using Reko.Core.Types;

namespace Reko.Arch.Mos6502
{
    public class Assembler : Reko.Core.Assemblers.Assembler
    {
        private static readonly Dictionary<Mnemonic, InstrOpcodes> instrOpcodes;

        private ServiceContainer sc;
        private Mos6502ProcessorArchitecture arch;
        private IPlatform platform;
        private Address addrBase;
        private Core.Assemblers.IEmitter m;
        private List<ImageSymbol> symbols;
        private SymbolTable symtab;

        public Assembler(ServiceContainer sc, IPlatform platform, Address addrBase, List<ImageSymbol> symbols)
        {
            this.sc = sc;
            this.arch = new Mos6502ProcessorArchitecture("mos6502");
            this.platform = platform;
            this.addrBase = addrBase;
            this.symbols = symbols;
            this.m = new Core.Assemblers.Emitter();
            this.symtab = new SymbolTable();
        }

        public Program GetImage()
        {
            var mem = new MemoryArea(addrBase, m.GetBytes());
            var seg = new ImageSegment("code", mem, AccessMode.ReadWriteExecute);
            var program = new Program(
                new SegmentMap(addrBase, seg),
                arch,
                platform);
            return program;
        }

        private void DefineSymbol(string pstr)
        {
            var sym = symtab.DefineSymbol(pstr, m.Position);
            sym.ResolveLe(m);
        }

        public Assembler Label(string label)
        {
            DefineSymbol(label);
            return this;
        }

        private void EmitOpcodeOperand(Mnemonic mnemonic, Operand op)
        {
            var ops = instrOpcodes[mnemonic];
            switch (op.Mode)
            {
            default: throw new NotImplementedException($"Assembler has not implemented address mode {op.Mode}.");
            case AddressMode.Immediate:
                if (ops.Imm == 0)
                    break;
                m.EmitByte(ops.Imm);
                m.EmitByte(op.Offset.ToByte());
                return;
            case AddressMode.ZeroPage:
                if (ops.Zp == 0)
                    break;
                m.EmitByte(ops.Zp);
                m.EmitByte(op.Offset.ToByte());
                return;
            case AddressMode.ZeroPageX:
                if (ops.ZpX == 0)
                    break;
                m.EmitByte(ops.ZpX);
                m.EmitByte(op.Offset.ToByte());
                return;
            case AddressMode.AbsoluteY:
                if (ops.AbsY == 0)
                    break;
                m.EmitByte(ops.AbsY);
                m.EmitLeUInt16(op.Offset.ToUInt16());
                return;
            }
            throw new NotSupportedException($"Instruction {mnemonic} does not support address mode {op.Mode}.");
        }


        private Symbol EmitRelativeTarget(string target, PrimitiveType dt)
        {
            int offBytes = dt.Size;
            switch (offBytes)
            {
            case 1: m.EmitByte(-(m.Length + 1)); break;
            case 2: m.EmitLeUInt16(-(m.Length + 2)); break;
            }
            var sym = symtab.CreateSymbol(target);
            sym.ReferToLe(m.Length - offBytes, dt, m);
            return sym;
        }

        public void Db(params byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; ++i)
            {
                m.EmitByte(bytes[i]);
            }
        }

        public void Bne(string target)
        {
            m.EmitByte(0xD0);
            EmitRelativeTarget(target, PrimitiveType.Byte);
        }

        public void Dex()
        {
            m.EmitByte(0xCA);
        }

        public void Dey()
        {
            m.EmitByte(0x88);
        }

        public void Inx()
        {
            m.EmitByte(0xE8);
        }

        public void Iny()
        {
            m.EmitByte(0xC8);
        }

        public void Jmp(string target)
        {
            m.EmitByte(0x4C);
            m.EmitLeUInt16(addrBase.ToUInt16());  // 6502 jmps are absolute.
            var sym = symtab.CreateSymbol(target);
            sym.ReferToLe(m.Length - 2, PrimitiveType.Word16, m);
        }

        public void Nop()
        {
            m.EmitByte(0xEA);
        }

        public void Lda(ParsedOperand op)
        {
            EmitOpcodeOperand(Mnemonic.lda, op.Operand);
        }

        public void Ldx(ParsedOperand op)
        {
            EmitOpcodeOperand(Mnemonic.ldx, op.Operand);
        }

        public void Ldy(ParsedOperand op)
        {
            EmitOpcodeOperand(Mnemonic.ldy, op.Operand);
        }

        public void Sta(ParsedOperand op)
        {
            EmitOpcodeOperand(Mnemonic.sta, op.Operand);
        }

        public ParsedOperand i8(byte v)
        {
            return new ParsedOperand(
                new Operand(PrimitiveType.Byte)
                {
                    Mode = AddressMode.Immediate,
                    Offset = Constant.Byte(v)
                },
                null);
        }

        /// <summary>
        /// Generate a zero-page address operand.
        /// </summary>
        public ParsedOperand zp(byte zp)
        {
            return new ParsedOperand(
                  new Operand(PrimitiveType.Word16)
                  {
                      Mode = AddressMode.ZeroPage,
                      Offset = Constant.Byte(zp)
                  },
                  null);
        }

        /// <summary>
        /// Generate a zero-page,X address operand.
        /// </summary>
        public ParsedOperand zpX(byte zp)
        {
            return new ParsedOperand(
                  new Operand(PrimitiveType.Word16)
                  {
                      Mode = AddressMode.ZeroPageX,
                      Offset = Constant.Byte(zp)
                  },
                  null);
        }

        /// <summary>
        /// Generate an absolute,Y address operand.
        /// </summary>
        public ParsedOperand ay(uint addr)
        {
            return new ParsedOperand(
                  new Operand(PrimitiveType.Word16)
                  {
                      Mode = AddressMode.AbsoluteY,
                      Offset = Constant.Word16((ushort) addr)
                  },
                  null);
        }

        public Address StartAddress
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<ImageSymbol> EntryPoints
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<ImageSymbol> ImageSymbols
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Dictionary<Address, ImportReference> ImportReferences
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Program Assemble(Address baseAddress, TextReader reader)
        {
            throw new NotImplementedException();
        }

        public Program AssembleFragment(Address baseAddress, string fragment)
        {
            throw new NotImplementedException();
        }



        public class ParsedOperand
        {
            public Operand Operand;

            public ParsedOperand(Operand op, Symbol sym)
            {
                this.Operand = op;
            }
        }

        public class InstrOpcodes
        {
            public byte Imm;
            public byte Zp;
            public byte ZpX;
            public byte ZpY;
            public byte Abs;
            public byte AbsX;
            public byte AbsY;
            public byte IndX;
            public byte IndY;
        }

        static Assembler()
        {
            instrOpcodes = new Dictionary<Mnemonic, InstrOpcodes>
            {
                { Mnemonic.lda, new InstrOpcodes {
                    Imm = 0xA9,
                    Zp = 0xA5,
                    ZpX = 0xB5,
                    Abs = 0xAD,
                    AbsX = 0xBD,
                    AbsY = 0xB9,
                    IndX = 0xA1,
                    IndY = 0xB1,
                } },
                { Mnemonic.ldx, new InstrOpcodes {
                    Imm = 0xA2,
                    Zp = 0xA6,
                    ZpY = 0xB6,
                    Abs = 0xAE,
                    AbsY = 0xBE,
                } },
                { Mnemonic.ldy, new InstrOpcodes {
                    Imm = 0xA0,
                    Zp = 0xA4,
                    ZpX = 0xB4,
                    Abs = 0xAC,
                    AbsX = 0xBC,
                } },
                { Mnemonic.sta, new InstrOpcodes {
                    Zp = 0x85,
                    ZpX = 0x95,
                    Abs = 0x8D,
                    AbsX = 0x9D,
                    AbsY = 0x99,
                    IndX = 0x81,
                    IndY = 0x91,
                } },

            };
        }
    }
}
