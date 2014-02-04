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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using Path = System.IO.Path;

namespace Decompiler.Core
{
    /// <summary>
    /// An InputFile describes an binary file or other kind of file that is to be decompiled.
    /// </summary>
    [Designer("Decompiler.Gui.Design.InputFileDesigner,Decompiler")]
    public class InputFile : ProjectFile
    {
        /// <summary>
        /// The address at which the file is loaded.
        /// </summary>
        public Address BaseAddress { get; set; }

        /// <summary>
        /// The name of the file in which disassemblies are dumped.
        /// </summary>
        public string DisassemblyFilename { get; set; }

        /// <summary>
        /// The name of the file in which intermediate results are stored.
        /// </summary>
        public string IntermediateFilename { get; set; }

        /// <summary>
        /// The name of the file in which final output is stored
        /// //$REVIEW: there may be many of these as the user may want to fragment the procedures in groups.
        /// </summary>
        public string OutputFilename { get; set; }

        /// <summary>
        /// The name of the file in which recovered types are stored.
        /// //$REVIEW: there may be many of these as the user may want to fragment the procedures in groups.
        /// </summary>
        public string TypesFilename { get; set; }

        public SortedList<Address, Serialization.SerializedProcedure> UserProcedures;
        public SortedList<Address, Serialization.SerializedCall_v1> UserCalls;

        public void SetDefaultFileNames(string inputFilename)
        {
            Filename = inputFilename;

            DisassemblyFilename = Path.ChangeExtension(inputFilename, ".asm");
            IntermediateFilename = Path.ChangeExtension(inputFilename, ".dis");
            OutputFilename = Path.ChangeExtension(inputFilename, ".c");
            TypesFilename = Path.ChangeExtension(inputFilename, ".h");
        }
    }
}
