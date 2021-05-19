#region License
/* Copyright (C) 1999-2021 John Källén.
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

using Reko.Core.Assemblers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Interface to a running instance of the Reko decompiler.
    /// </summary>
    public interface IDecompiler
    {
        /// <summary>
        /// Event is fired when the current <see cref="Project"/> used by
        /// the decompiler is changed.
        /// </summary>
        public event EventHandler? ProjectChanged;

        /// <summary>
        /// The current <see cref="Core.Project"/> used by the decompiler, or
        /// null if no project has been loaded.
        /// </summary>
        Project? Project { get; }

        bool Load(string fileName, string? loader = null, Address? addLoad = null);
        Program LoadRawImage(string file, LoadDetails raw);
        Program LoadRawImage(byte[] bytes, LoadDetails raw);
        void ScanPrograms();
        ProcedureBase ScanProcedure(ProgramAddress paddr, IProcessorArchitecture arch);
        void AnalyzeDataFlow();
        void ReconstructTypes();
        void StructureProgram();
        void WriteDecompilerProducts();
        void ExtractResources();

        void Assemble(string file, IAssembler asm, IPlatform platform);
    }

}
