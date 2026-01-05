#region License
/* Copyright (C) 1999-2026 John Källén.
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
using System;

namespace Reko;

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
    /// The current <see cref="Project"/> used by the decompiler, or
    /// null if no project has been loaded.
    /// </summary>
    Project Project { get; }

    /// <summary>
    /// Scans all programs in the <see cref="Project"/>.
    /// </summary>
    void ScanPrograms();

    /// <summary>
    /// Scans a procedure at the given program address <paramref name="paddr"/>.
    /// </summary>
    /// <param name="paddr"><see cref="ProgramAddress"/> to start scanning at.
    /// </param>
    /// <param name="arch"><see cref="IProcessorArchitecture"/> to use when 
    /// scanning.
    /// </param>
    /// <returns>The scanned procedure.
    /// </returns>
    ProcedureBase ScanProcedure(ProgramAddress paddr, IProcessorArchitecture arch);

    /// <summary>
    /// Analyzes the data flow of all procedures in the <see cref="Project"/>.
    /// </summary>
    void AnalyzeDataFlow();

    /// <summary>
    /// Performs data type inference and reconstructs types in the 
    /// <see cref="Project"/>. This is
    /// </summary>
    void ReconstructTypes();

    /// <summary>
    /// Rewrites the mid-level RTL of all procedures in the <see cref="Project"/>
    /// to high-level language, using structured programming constructs.
    /// </summary>
    void StructureProgram();


    /// <summary>
    /// Writes the decompiled products for each program in the current project.
    /// </summary>
    void WriteDecompilerProducts();

    /// <summary>
    /// Extracts any resources that are embedded in the programs of the current <see cref="Project"/>.
    /// </summary>
    void ExtractResources();

    /// <summary>
    /// Replaces any occurrence of <paramref name="oldProgram"/> in the
    /// <see cref="Project"/> with <paramref name="newProgram"/>.
    /// </summary>
    void ReplaceProgram(Program oldProgram, Program newProgram);
}
