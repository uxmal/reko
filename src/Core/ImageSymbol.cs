#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Represents a location in an image whose name or size is 
    /// known. ImageSymbols are used to "seed" the Scanner
    /// phase of the decompiler.
    /// </summary>
    /// <remarks>
    /// This information is derived purely from what the image loader can 
    /// garner from an input binary file. Any user-provided information must
    /// be specified separately and will then override this information.
    /// </remarks>
    public class ImageSymbol
    {
        /// <summary>
        /// Use this ctor when the symbol is imported from another module.
        /// </summary>
        public ImageSymbol()
        {
        }

        /// <summary>
        /// Use this ctor when only the address of the symbol is known.
        /// </summary>
        /// <param name="address"></param>
        public ImageSymbol(Address address)
        {
            this.Address = address;
        }

        /// <summary>
        /// Use this ctor when symbolic data is available.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="name"></param>
        /// <param name="dataType"></param>
        public ImageSymbol(Address address, string name, DataType dataType)
        {
            this.Address = address;
            this.Name = name;
            this.DataType = dataType;
        }

        public SymbolType Type { get; set; }

        /// <summary>
        /// The location of the object referred to by the symbol.
        /// </summary>
        public Address Address { get; private set; }

        /// <summary>
        /// The name of the symbol if known, null or blank if unknown.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The data type of the symbol if it known.
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// If set, Reko should just make note of the symbol and not 
        /// attempt to decompile it.
        /// </summary>
        public bool NoDecompile { get; set; }

        /// <summary>
        /// If non-null, the state of the processor at the time when
        /// the signature is referred.
        /// </summary>
        public ProcessorState ProcessorState { get; set; }

        /// <summary>
        /// If non-null, the signature of this symbol (if it is a function)
        /// </summary>
        public SerializedSignature Signature { get; set; }

        public DataType DataType { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0} ({1})",
                string.IsNullOrEmpty(Name) ? "???" : Name,
                Address);
        }
    }

    public enum SymbolType
    {
        Unknown,            // Unknown type
        Code,               // executable code, but not the entry point of a procedure.
        Data,               // non-executable data
        Procedure,          // Something that is called.
        ExternalProcedure   // A procedure outside of the binary
    }
}
