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

using Reko.Core;
using System;

namespace Reko.Gui
{
    public class SelectionChangedEventArgs : EventArgs
    {
        private AddressRange range;

        public SelectionChangedEventArgs(AddressRange range)
        {
            this.range = range;
        }

        public AddressRange AddressRange { get { return range; } }
    }

    /// <summary>
    /// The ILowLevelViewService can be used by user interface methods 
    /// to display a memory dump and raw disassembly of a Program.
    /// </summary>
    public interface ILowLevelViewService
    {
        /// <summary>
        /// Show a low level window for the specified <paramref name="program"/>.
        /// If the window is already visible, bring it to the foreground.
        /// </summary>
        /// <param name="program">Program whose low-level details are to 
        /// be shown.</param>
        void ShowWindow(Program program);
        /// <summary>
        /// Show a low level window for the specified <paramref name="program"/>, and
        /// display the first few bytes.
        /// </summary>
        /// <param name="program">Program whose low-level details are to 
        /// be shown.</param>
        void ViewImage(Program program);

        /// <summary>
        /// Show a low-level window for the specified <paramref name="program"/>, and
        /// make the address <paramref name="addr"/> visible on screen.
        /// </summary>
        /// <param name="program">Program whose low-level details are to 
        /// be shown.</param>
        /// <param name="addr">Address to show.</param>
        void ShowMemoryAtAddress(Program program, Address addr);
    }
}
