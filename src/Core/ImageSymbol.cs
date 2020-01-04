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

using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Represents a location in an image whose name and/or size is known. 
    /// ImageSymbols are used to "seed" the Scanner phase of the decompiler.
    /// </summary>
    /// <remarks>
    /// This information is derived purely from what the image loader can 
    /// garner from an input binary file. Any user-provided information must
    /// be specified separately in <see cref="Reko.Core.UserData"/> and will then override this information.
    /// </remarks>
    public class ImageSymbol
    {
        private ImageSymbol(IProcessorArchitecture arch)
        {
            this.Architecture = arch ?? throw new ArgumentNullException(nameof(arch));
        }

        /// <summary>
        /// Use this method when only the address of the symbol is known.
        /// </summary>
        /// <param name="address">Address of the symbol.</param>
        public static ImageSymbol Location(IProcessorArchitecture arch, Address address)
        {
            return new ImageSymbol(arch)
            {
                Type = SymbolType.Unknown,
                Address = address,
                Architecture = arch,
                DataType = new UnknownType(),
            };
        }

        /// <summary>
        /// Creates a symbol describing the location of a callable procedure.
        /// </summary>
        /// <param name="arch">Processor architecture of the procedure.</param>
        /// <param name="address">Address of the procedure.</param>
        /// <param name="name">Optional name of the procedure.</param>
        /// <param name="dataType">Optional data type of the procedure.</param>
        /// <param name="signature">Optional serialized signature.</param>
        /// <param name="state">Optional processor state.</param>
        /// <returns>A symbol</returns>
        public static ImageSymbol Procedure(
            IProcessorArchitecture arch,
            Address address, 
            string name=null, 
            DataType dataType=null,
            SerializedSignature signature=null,
            ProcessorState state=null)
        {
            return new ImageSymbol(arch)
            {
                Type = SymbolType.Procedure,
                Architecture = arch,
                Address = address,
                Name = name,
                DataType = dataType ?? new FunctionType(),
                Signature = signature,
                ProcessorState = state,
            };
        }

        /// <summary>
        /// A reference to an external procedure.
        /// </summary>
        /// <param name="arch">Processor architecture of the procedure.</param>
        /// <param name="address">Address of stub to the procedure.</param>
        /// <param name="name">Name of the external procedure</param>
        public static ImageSymbol ExternalProcedure(
            IProcessorArchitecture arch,
            Address address,
            string name)
        {
            return new ImageSymbol(arch)
            {
                Type = SymbolType.ExternalProcedure,
                Address = address,
                Name = name
            };
        }

        /// <summary>
        /// Use this method when symbolic information about global data is available.
        /// </summary>
        /// <param name="arch">Architecture to use when interpreting the data object.</param>
        /// <param name="address">Address of data object</param>
        /// <param name="name">Optional name of the object.</param>
        /// <param name="dataType">Optional size of the object.</param>
        public static ImageSymbol DataObject(IProcessorArchitecture arch, Address address, string name = null, DataType dataType = null)
        {
            return new ImageSymbol(arch)
            {
                Type = SymbolType.Data,
                Architecture = arch,
                Address = address,
                Name = name,
                DataType = dataType,
            };
        }

        public static ImageSymbol Create(
            SymbolType type,
            IProcessorArchitecture arch,
            Address address,
            string name = null,
            DataType dataType = null,
            bool decompile = true)
        {
            return new ImageSymbol(arch)
            {
                Type = type,
                Architecture = arch,
                Address = address,
                Name = name,
                DataType = dataType ?? new UnknownType(),
                NoDecompile = !decompile,
            };
        }

        /// <summary>
        /// The processor architecture to use when disassembling this symbol.
        /// </summary>
        public IProcessorArchitecture Architecture { get; set; }

        /// <summary>
        /// The type of this symbol.
        /// </summary>
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
        Code,               // executable code, not necessarily the entry point of a procedure.
        Data,               // non-executable data
        Procedure,          // Something that is called.
        ExternalProcedure   // A procedure outside of the binary
    }
}
