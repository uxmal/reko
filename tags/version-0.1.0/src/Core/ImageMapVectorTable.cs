/* 
 * Copyright (C) 1999-2009 John Källén.
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
        private bool fCallTable;
        private List<Address> addresses;
        private Dictionary<Address, int> registerUsed = new Dictionary<Address, int>();

        public ImageMapVectorTable(bool fCallTable)
        {
            this.fCallTable = fCallTable;
            this.addresses = new List<Address>();
        }

        public ImageMapVectorTable(bool isCallTable, Address[] vector, int size)
        {
            this.fCallTable = isCallTable;
            this.addresses = new List<Address>(vector);
        }

        public List<Address> Addresses { get { return addresses; } }

        public bool IsCallTable
        {
            get { return fCallTable; }
        }

        public Dictionary<Address,int> RegisterUsed
        {
            get { return registerUsed; }
        }
    }

}