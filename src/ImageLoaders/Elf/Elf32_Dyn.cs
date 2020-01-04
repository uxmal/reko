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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    public class Elf32_Dyn
    {
        public int d_tag; /* how to interpret value */
        private int val;
        public int d_val { get { return val; } set { val = value; } }
        public uint d_ptr { get { return (uint) val; } set { val = (int)value; } }
        public int d_off { get { return val; } set { val = value; } }
    }

    public class Elf64_Dyn
    {
        public long d_tag; /* how to interpret value */
        private long val;
        public long d_val { get { return val; } set { val = value; } }
        public ulong d_ptr { get { return (ulong)val; } set { val = (long)value; } }
        public long d_off { get { return val; } set { val = value; } }
    }
}
