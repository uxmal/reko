#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using System.Threading.Tasks;

namespace Reko.Core
{
    /// <summary>
    /// A LocationNavigator is used by the GUI front-end to navigate to a 
    /// given location. Locations can be Addresses, Procedures, or blocks.
    /// </summary>
    public interface ICodeLocation
    {
        /// <summary>
        /// Descriptive text for the location.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// When invoked, triggers the user interface to navigate to the location.
        /// </summary>
        /// <returns>An awaitable <see cref="ValueTask"/>.
        /// </returns>
        ValueTask NavigateTo();
    }
}
