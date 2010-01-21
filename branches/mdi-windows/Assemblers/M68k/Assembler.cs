/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Assemblers;
using Decompiler.Core.Types;
using Decompiler.Arch.M68k;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Assemblers.M68k
{
    public class AssemblerImpl : Assembler
    {
        private Address addrBase;
        private Emitter emitter;
        private SymbolTable symtab;

        public AssemblerImpl(Address addr, Emitter emitter)
        {
            this.addrBase = addr;
            this.emitter = emitter;
            this.symtab = new SymbolTable();
        }

        #region Assembler Members

        public void Assemble(Address baseAddress, string sourcefile)
        {
            throw new NotImplementedException();
        }

        public void Assemble(Address baseAddress, TextReader rdr)
        {
            throw new NotImplementedException();
        }

        public void AssembleFragment(Address baseAddress, string fragment)
        {
            throw new NotImplementedException();
        }


        public ProgramImage GetImage()
        {
            return new ProgramImage(addrBase, emitter.Bytes);
        }

        public ProgramImage Image
        {
            get { return GetImage(); }
        }

        public Address StartAddress
        {
            get { throw new NotImplementedException(); }
        }

        public ICollection<EntryPoint> EntryPoints
        {
            get { throw new NotImplementedException(); }
        }

        public IProcessorArchitecture Architecture
        {
            get { throw new NotImplementedException(); }
        }

        public Platform Platform
        {
            get { throw new NotImplementedException(); }
        }

        public Dictionary<uint, PseudoProcedure> ImportThunks
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public void BraB(string target)
        {
            emitter.EmitByte(0x60);
            emitter.EmitByte(-(emitter.Length + 1));
            symtab.CreateSymbol(target).ReferTo(emitter.Length - 1, PrimitiveType.Byte, emitter);
            
        }

        public void Move(DataRegister dSrc, DataRegister dDst)
        {
            int opcode = 0x2000 | dSrc.Number | (dSrc.Number << 9);
            emitter.EmitBeUint16(opcode);
        }

        public void Label(string label)
        {
            symtab.DefineSymbol(label, emitter.Position).Resolve(emitter);
        }
    }
}
