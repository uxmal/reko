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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Environments.PalmOS.PRC
{
    public class PrcHeader
    {
        public string? name;
        public ushort flags;
        public ushort version;
        public uint create_time;
        public uint mod_time;
        public uint backup_time;
        public uint mod_num;
        public uint app_info;
        public uint sort_info;
        public uint type;
        public uint id;
        public uint unique_id_seed;
        public uint next_record_list;
        public ushort num_records;
    }
}
