#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Dock.Model.ReactiveUI.Controls;
using Reko.Core;
using Reko.Core.Memory;
using System;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents
{
    public class MemoryViewModel : Document
    {
        public MemoryViewModel()
        {
            var rnd = new Random();
            var bytes = new byte[12000];
            rnd.NextBytes(bytes);
            this.MemoryArea = new ByteMemoryArea(Address.Ptr32(0x0011200), bytes);
        }

        public MemoryArea? MemoryArea { get; set; }
    }
}
