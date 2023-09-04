#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Reko.Core.Collections;
using Reko.Core.Lib;
using Reko.Core.Loading;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Oracular information provided by user.
    /// </summary>
    public class UserData : IReadOnlyUserData
    {
        public UserData()
        {
            this.Procedures = new SortedList<Address, UserProcedure>();
            this.Calls = new SortedList<Address, UserCallData>();
            this.Globals = new SortedList<Address, UserGlobal>();
            this.Heuristics = new SortedSet<string>();
            this.IndirectJumps = new SortedList<Address, UserIndirectJump>();
            this.JumpTables = new SortedList<Address, ImageMapVectorTable>();
            this.Annotations = new AnnotationList();
            this.TextEncoding = Encoding.ASCII;
            this.RegisterValues = new SortedList<Address, List<UserRegisterValue>>();
            this.Segments = new List<UserSegment>();
            this.ProcedureSourceFiles = new Dictionary<Address, string>();
            this.Patches = new Dictionary<Address, CodePatch>();
            this.DebugTraceProcedures = new HashSet<string>();
            this.BlockLabels = new Dictionary<string, string>();
        }

        // 'Oracular' information provided by the user.
        public string? Loader { get; set; }
        public string? Processor { get; set; }
        public string? Environment { get; set; }
        public Address? LoadAddress { get; set; }
        public SortedList<Address, UserProcedure> Procedures { get; set; }
        public SortedList<Address, UserCallData> Calls { get; set; }
        public SortedList<Address, UserGlobal> Globals { get; set; }
        public SortedList<Address, UserIndirectJump> IndirectJumps { get; set; }
        public SortedList<Address, ImageMapVectorTable> JumpTables { get; set; }
        public AnnotationList Annotations { get; set; }
        public List<UserSegment> Segments { get; set; }

        //$BUG: this should be Address => string.
        public Dictionary<string, string> BlockLabels { get; set; }
        
        /// <summary>
        /// A script to run after the image is loaded.
        /// </summary>
        public Serialization.Script_v2? OnLoadedScript { get; set; }

        /// <summary>
        /// Scanning heuristics to try.
        /// </summary>
        public ISet<string> Heuristics { get; set; }

        /// <summary>
        /// Text encoding to use to interpret strings. If none is specified,
        /// use the host platform's encoding.
        /// </summary>
        public Encoding? TextEncoding { get; set; }

        /// <summary>
        /// Users can set register values at any location.
        /// </summary>
        public SortedList<Address, List<UserRegisterValue>> RegisterValues { get; set; }

        /// <summary>
        /// If set, display addresses in the written disassembly file.
        /// </summary>
        public bool ShowAddressesInDisassembly { get; set; }

        /// <summary>
        /// If set, display bytes in the written disassembly file.
        /// </summary>
        public bool ShowBytesInDisassembly { get; set; }

        /// <summary>
        /// If set, extract any embedded resources.
        /// </summary>
        public bool ExtractResources { get; set; }

        /// <summary>
        /// Optionally maps a procedure (address) to the source code file in which its output 
        /// is to be placed.
        /// </summary>
        /// <remarks>
        /// The source file names are absolute paths.
        /// </remarks>
        public Dictionary<Address, string> ProcedureSourceFiles { get; set; }

        /// <summary>
        /// Maps patches to addresses. When the scanner encounters a patched
        /// address, it replaces the patch with the IR instructions in the patch.
        /// </summary>
        public Dictionary<Address, CodePatch> Patches { get; set; }

        /// <summary>
        /// Selects the policy to use when generating output files.
        /// </summary>
        public string? OutputFilePolicy { get; set; }

        /// <summary>
        /// For Reko debugging: procedures with these names will be traced.
        /// </summary>
        public HashSet<string> DebugTraceProcedures { get; set; }
    }

    public class Annotation
    {
        public Annotation(Address address, string text)
        {
            this.Address = address;
            this.Text = text;
        }

        public Address Address { get; }
        public string Text { get; }
    }

    /// <summary>
    /// User-specified information about a particular call site.
    /// </summary>
    public class UserCallData
    {
        public Address? Address { get; set; } // The address of the call.

        public string? Comment { get; set; }

        public bool NoReturn { get; set; }

        public FunctionType? Signature { get; set; }
    }

    public class UserIndirectJump
    {
        public Address? Address { get; set; } // the address of the jump

        public RegisterStorage? IndexRegister { get; set; }  // Index register used in jump

        public ImageMapVectorTable? Table { get; set; } // Table of destinations
    }

    /// <summary>
    /// User-provided information describing a segment of a raw binary.
    /// </summary>
    public class UserSegment
    {
        /// <summary>
        /// The start address of the segment.
        /// </summary>
        public Address? Address { get; set; } 

        /// <summary>
        /// The file offset from which this segment was loaded.
        /// </summary>
        public ulong Offset { get; set; }

        /// <summary>
        /// The length of the segment in addressable units (bytes on a byte oriented machine)
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        /// User provided name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Optional processor architecture, overriding that of the 
        /// <see cref="Program"/> insstance.
        /// </summary>
        public IProcessorArchitecture? Architecture { get; set; }

        /// <summary>
        /// The access mode to use for this segment.
        /// </summary>
        public AccessMode AccessMode { get; set; }
    }
}
