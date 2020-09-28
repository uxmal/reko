#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Lib;
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
    public class UserData
    {
        public UserData()
        {
            this.Procedures = new SortedList<Address, Serialization.Procedure_v1>();
            this.Calls = new SortedList<Address, UserCallData>();
            this.Globals = new SortedList<Address, Serialization.GlobalDataItem_v2>();
            this.Heuristics = new SortedSet<string>();
            this.IndirectJumps = new SortedList<Address, UserIndirectJump>();
            this.JumpTables = new SortedList<Address, ImageMapVectorTable>();
            this.Annotations = new AnnotationList();
            this.TextEncoding = Encoding.ASCII;
            this.RegisterValues = new SortedList<Address, List<UserRegisterValue>>();
            this.Segments = new List<UserSegment>();
            this.ProcedureSourceFiles = new Dictionary<Address, string>();
        }

        // 'Oracular' information provided by the user.
        public string Loader { get; set; }
        public string Processor { get; set; }
        public string Environment { get; set; }
        public Address LoadAddress { get; set; }
        public SortedList<Address, Serialization.Procedure_v1> Procedures { get; set; }
        public SortedList<Address, UserCallData> Calls { get; set; }
        public SortedList<Address, Serialization.GlobalDataItem_v2> Globals { get; set; }
        public SortedList<Address, UserIndirectJump> IndirectJumps { get; set; }
        public SortedList<Address, ImageMapVectorTable> JumpTables { get; set; }
        public AnnotationList Annotations { get; set; }
        public List<UserSegment> Segments { get; set; }

        /// <summary>
        /// A script to run after the image is loaded.
        /// </summary>
        public Serialization.Script_v2 OnLoadedScript { get; set; }

        /// <summary>
        /// Scanning heuristics to try.
        /// </summary>
        public SortedSet<string> Heuristics { get; set; }

        /// <summary>
        /// Text encoding to use to interpret strings.
        /// </summary>
        public Encoding TextEncoding { get; set; }

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
    }

    public class Annotation
    {
        public Annotation(Address address, string text)
        {
            this.Address = address;
            this.Text = text;
        }

        public Address Address { get; private set; }
        public string Text { get; private set; }
    }

    /// <summary>
    /// User-specified information about a particular call site.
    /// </summary>
    public class UserCallData
    {
        public Address Address { get; set; } // The address of the call.

        public string Comment { get; set; }

        public bool NoReturn { get; set; }

        public FunctionType Signature { get; set; }
    }

    public class UserIndirectJump
    {
        public Address Address { get; set; } // the address of the jump

        public RegisterStorage IndexRegister { get; set; }  // Index register used in jump

        public ImageMapVectorTable Table { get; set; } // Table of destinations
    }

    /// <summary>
    /// User-provided information describing a segment of a raw binary.
    /// </summary>
    public class UserSegment
    {
        public Address Address { get; set; } // the start address of the segment.

        public ulong Offset { get; set; }   // the file offset from which this segment was loaded.

        public uint Length { get; set; }    // The length of the segment in addressable units (bytes on a byte oriented machine)

        public string Name { get; set; }    // User provided name

        public IProcessorArchitecture Architecture { get; set; }

        public AccessMode AccessMode { get; set; }
    }
}
