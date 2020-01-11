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
using Reko.Core.Expressions;
using Reko.Core.Types;

namespace Reko.Arch.Mos6502
{
    public class Assembler : Reko.Core.Assemblers.Assembler
    {
        private ServiceContainer sc;
        private Mos6502ProcessorArchitecture arch;
        private IPlatform platform;
        private Address addrBase;
        private Core.Assemblers.IEmitter m;
        private List<ImageSymbol> symbols;

        public Assembler(ServiceContainer sc, IPlatform platform, Address addrBase, List<ImageSymbol> symbols)
        {
            this.sc = sc;
            this.arch = new Mos6502ProcessorArchitecture("mos6502");
            this.platform = platform;
            this.addrBase = addrBase;
            this.symbols = symbols;
            this.m = new Core.Assemblers.Emitter();
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

        public void Ldy(ParsedOperand op)
        {
            switch (op.Operand.Mode)
            {
            case AddressMode.Immediate:
                m.EmitByte(0xA0);
                m.EmitByte(op.Operand.Offset.ToByte());
                break;
            default:
               throw new NotImplementedException();
            }
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

        public class Symbol
        {

        }
    }
}
