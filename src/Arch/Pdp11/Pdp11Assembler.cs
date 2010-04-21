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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Arch.Pdp11
{
    public class Pdp11Assembler : Assembler
    {
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

        public IProcessorArchitecture Architecture
        {
            get { return new Pdp11Architecture(); }
        }

        public ICollection<EntryPoint> EntryPoints
        {
            get { throw new NotImplementedException(); }
        }

        public ProgramImage Image
        {
            get { throw new NotImplementedException(); }
        }

        public Platform Platform
        {
            get { throw new NotImplementedException(); }
        }

        public Address StartAddress
        {
            get { throw new NotImplementedException(); }
        }

        public Dictionary<uint, PseudoProcedure> ImportThunks
        {
            get { throw new NotImplementedException(); }
        }
    }
}
