#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using System.Collections.Generic;

namespace Reko.Core.Configuration
{
    /// <summary>
    /// Contains processor-specific settings for a particular 
    /// platform.
    /// </summary>
    public class PlatformArchitectureDefinition
    {
        /// <summary>
        /// Creates a platform architecture definition.
        /// </summary>
        public PlatformArchitectureDefinition()
        {
            this.TrashedRegisters = new List<string>();
            this.PreservedRegisters = new List<string>();
            this.TypeLibraries = new List<TypeLibraryDefinition>();
            this.ProcedurePrologs = new List<MaskedPattern>();
            this.CallingConventions = new List<string>();
        }

        /// <summary>
        /// The name of the architecture.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The caller-save registers of this platform's ABI.
        /// </summary>
        public List<string> TrashedRegisters { get; set; }

        /// <summary>
        /// The callee-save registers of this Platform's ABI.
        /// </summary>
        public List<string> PreservedRegisters { get; set; }

        /// <summary>
        /// Architecture-specific type libraries used by this platform.
        /// </summary>
        public List<TypeLibraryDefinition> TypeLibraries { get; set; }

        /// <summary>
        /// Typical procedure prologs for this architecture.
        /// </summary>
        public List<MaskedPattern> ProcedurePrologs { get; set; }

        /// <summary>
        /// The names of calling conventions used in this architecture.
        /// </summary>
        public List<string> CallingConventions { get; set; }

        /// <summary>
        /// The name of the default calling convention of this architecture.
        /// </summary>
        public string? DefaultCallingConvention { get; set; }
    }
}
