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
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Loading
{
    /// <summary>
    /// Loader that assembles a literal string. Mostly used for testing.
    /// </summary>
    public class AssemblerFragmentLoader : LoaderBase
    {
        private string asmFragment;
        private Assembler asm;

        public AssemblerFragmentLoader(string asmFragment, Assembler asm)
        {
            this.asmFragment = asmFragment;
            this.asm = asm;
        }

        public override Program Load(byte[] imageFile, Address userSpecifiedAddress)
        {
            throw new NotImplementedException("Assembler asm = arch.CreateAssembler();");
            //asm.AssembleFragment(null, addrLoad, asmFragment);
            //Program.Image = asm.Image;
            //EntryPoints.Add(new EntryPoint(asm.StartAddress, arch.CreateProcessorState()));
            //return new DecompilerProject();
        }
    }
}
