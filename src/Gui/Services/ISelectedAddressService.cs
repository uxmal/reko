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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Services
{
    /// <summary>
    /// This service is used by components that are interested
    /// in the currently selected address.
    /// </summary>
    public interface ISelectedAddressService
    {
        /// <summary>
        /// This event is raised when the selected address is changed.
        /// </summary>
        event EventHandler? SelectedAddressChanged;

        /// <summary>
        /// This event is raised when the selected procedure is changed.
        /// </summary>
        event EventHandler? SelectedProcedureChanged;

        /// <summary>
        /// This event is raised when the selected procedure is changed.
        /// </summary>
        event EventHandler? SelectedProgramChanged;

        /// <summary>
        /// If not null, indicates the address the user has 
        /// selected. The property value is the currently selected
        /// <see cref="ProgramAddressRange"/>.
        /// A length of 0 indicates that only the address
        /// has been selected, not a range of bytes.
        /// </summary>
        ProgramAddressRange? SelectedAddressRange { get; set; }

        /// <summary>
        /// If not null, indicates which procedure the user has selected.
        /// </summary>
        Procedure? SelectedProcedure { get; set; }

        /// <summary>
        /// If not null, indicates which program the user has selected.
        /// </summary>
        Program? SelectedProgram { get; set; }
    }
}
