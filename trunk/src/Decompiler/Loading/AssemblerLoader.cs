#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Core.Assemblers;
using Decompiler.Core.Configuration;
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Loading
{
    /// <summary>
    /// Loader that assembles assembler code from a file.
    /// </summary>
    public class AssemblerLoader : LoaderBase
    {
        private string asmfile;
        private Assembler asm;

        public AssemblerLoader(IServiceProvider services, Assembler asm, string asmfile) : base(services)
        {
            this.asm = asm;
            this.asmfile = asmfile;
        }

        public Program Load(Address addrLoad)
        {
            byte[] image = LoadImageBytes(asmfile, 0);
            return Load(asmfile, image, addrLoad);
        }

        public override Program Load(string fileName, byte[] image, Address addrLoad)
        {
            var lr = asm.Assemble(addrLoad, new StreamReader(new MemoryStream(image), Encoding.UTF8));
            Program prog = new Program(
                lr.Image,
                new ImageMap(lr.Image),
                lr.Architecture,
                lr.Platform);
            prog.Name = Path.GetFileName(fileName);
            EntryPoints.AddRange(asm.EntryPoints);
            EntryPoints.Add(new EntryPoint(asm.StartAddress, prog.Architecture.CreateProcessorState()));
            CopyImportThunks(asm.ImportThunks, prog);
            return prog;
        }

        public override byte[] LoadImageBytes(string fileName, int offset)
        {
            return base.LoadImageBytes(fileName, offset);
        }
    }
}
