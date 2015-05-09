#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

namespace Decompiler.Core
{
    /// <summary>
    /// Represents a table of jumps or calls. Initially, it is empty.
    /// item, but further analysis may grow this size.
    /// </summary>
    public class ImageMapVectorTable : ImageMapItem
    {
        public ImageMapVectorTable(Address addrTable, bool fCallTable)
        {
            this.TableAddress = addrTable;
            this.IsCallTable = fCallTable;
            this.Addresses = new List<Address>();
            this.RegisterUsed = new Dictionary<Address, int>();
        }

        public ImageMapVectorTable(bool isCallTable, Address[] vector, int size)
        {
            this.IsCallTable = isCallTable;
            this.Addresses = new List<Address>(vector);
            this.RegisterUsed = new Dictionary<Address,int>();
        }

        public Address TableAddress { get; private set; }
        public List<Address> Addresses { get; private set; }
        public bool IsCallTable { get; private set;}
        public Dictionary<Address,int> RegisterUsed { get; private set; }
    }
}