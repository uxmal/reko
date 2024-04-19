#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Configuration
{
    /// <summary>
    /// Contains processor-specific settings for a particular 
    /// platform.
    /// </summary>
    public class PlatformArchitectureDefinition
    {
        public PlatformArchitectureDefinition()
        {
            this.TrashedRegisters = new List<string>();
            this.PreservedRegisters = new List<string>();
            this.TypeLibraries = new List<TypeLibraryDefinition>();
            this.ProcedurePrologs = new List<MaskedPattern>();
            this.CallingConventions = new List<string>();
        }

        public string? Name { get; set; }
        public List<string> TrashedRegisters { get; set; }
        public List<string> PreservedRegisters { get; set; }
        public List<TypeLibraryDefinition> TypeLibraries { get; set; }
        public List<MaskedPattern> ProcedurePrologs { get; set; }
        public List<string> CallingConventions { get; set; }
        public string? DefaultCallingConvention { get; set; }
    }
}
