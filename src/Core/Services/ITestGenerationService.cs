#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using Reko.Core.Machine;
using Reko.Core.Memory;
using System;

namespace Reko.Core.Services
{
    /// <summary>
    /// Reko calls methods on this interface when it encounters errors. The methods generate unit tests
    /// that can be incorporated into the UnitTest project.
    /// </summary>
    public interface ITestGenerationService
    {
        /// <summary>
        /// Users can optionally override the output directory for more control 
        /// of where the output goes.
        /// </summary>
        string? OutputDirectory { get; set; }

        /// <summary>
        /// This method is called when an incomplete disassembler can't decode a byte sequence.
        /// </summary>
        /// <remarks>
        /// This must only be called on byte sequences that are known to be valid machine code, but
        /// haven't had a decoder written for them yet. Byte sequences that are known to be invalid
        /// machine code should never result in this method being called.
        /// </remarks>
        /// <param name="testPrefix">Prefix to use in the generated unit test.</param>
        /// <param name="addrStart">Address at which the undecoded byte sequence started.</param>
        /// <param name="rdr">Image reader positioned at the end of the byte sequence.</param>
        /// <param name="message">Optional message that will be emitted as a comment.</param>
        /// <param name="hexize">Optional function to convert raw bytes into text. By default, a hexadecimal string is 
        /// generated.</param>
        void ReportMissingDecoder(string testPrefix, Address addrStart, EndianImageReader rdr, string message, Func<byte[], string>? hexize = null);

        /// <summary>
        /// This method is called when an incomplete rewriter fails to rewrite a valid machine 
        /// instruction.
        /// </summary>
        /// <param name="testPrefix">Prefix to use in the generated unit test.</param>
        /// <param name="instr">The <see cref="MachineInstruction"/> that didn't get rewritten.</param>
        /// <param name="mnemonic">The mnemonic of the <see cref="MachineInstruction"/> that didn't get rewritten.</param>
        /// <param name="rdr">Image reader positioned after the end of the machine instruction.</param>
        /// <param name="message">Optional message that will be emitted as a comment.</param>
        /// <param name="hexize">Optional function to convert raw bytes into text. By default, a hexadecimal string is 
        /// generated.</param>
        void ReportMissingRewriter(string testPrefix, MachineInstruction instr, string mnemonic, EndianImageReader rdr, string message, Func<byte[], string>? hexize = null);

        /// <summary>
        /// Remove files starting with the given <paramref name="filePrefix"/> from the output directory.
        /// </summary>
        /// <param name="filePrefix"></param>
        void RemoveFiles(string filePrefix);

        /// <summary>
        /// Report the state of a procedure to a file determine by the filename.
        /// </summary>
        /// <param name="filePrefix"></param>
        /// <param name="testCaption"></param>
        /// <param name="proc"></param>
        void ReportProcedure(string fileName, string testCaption, Procedure proc);
    }
}
