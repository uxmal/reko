#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Assemblers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Configuration;

namespace Reko.Core
{
    /// <summary>
    /// Implementors assume responsibility for loading project files or binaries.
    /// </summary>
    public interface ILoader
    {
        string DefaultToFormat { get; set; }

        byte[] LoadImageBytes(string fileName, int offset);
        Program LoadExecutable(string fileName, byte[] bytes, Address loadAddress);
        Program LoadRawImage(string fileName, byte[] image, RawFileElement raw);
        Program LoadRawImage(string filename, byte[] bytes, string archName, string platformName, Address loadAddress);
        Program AssembleExecutable(string fileName, string asmName, Address loadAddress);
        Program AssembleExecutable(string fileName, Assembler asm, Address loadAddress);
        Program AssembleExecutable(string fileName, byte[] bytes, Assembler asm, Address loadAddress);

        TypeLibrary LoadMetadata(string fileName, IPlatform platform, TypeLibrary typeLib);
    }
}
