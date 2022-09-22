#region License
/* 
* Copyright (C) 2021-2022 Sven Almgren.
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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core
{
    public class UserGlobal
    {
        public Address Address { get; set; }
        public string Name { get; set; }
        public SerializedType DataType { get; set; }
        public string? Comment { get; set; }

        public UserGlobal(Address address, string name, SerializedType dataType)
        {
            Address = address;
            Name = name;
            DataType = dataType;
        }

        public static string GenerateDefaultName(Address address) => $"g_{address.ToLinear():X}";
    }
}
