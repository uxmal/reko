#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core
{
    /// <summary>
    /// Convenient utility class for making Storages.
    /// </summary>
    public class StorageFactory
    {
        private int iReg;

        public int Current { get { return iReg; } }

        public RegisterStorage Reg(string format, PrimitiveType size)
        {
            var name = string.Format(format, iReg);
            return MakeReg(name, size);
        }

        private RegisterStorage MakeReg(string name, PrimitiveType size)
        {
            var reg = new RegisterStorage(name, iReg, 0, size);
            ++iReg;
            return reg;
        }

        public RegisterStorage Reg32(string format)
        {
            return Reg(format, PrimitiveType.Word32);
        }

        public RegisterStorage[] RangeOfReg(int count, string format, PrimitiveType size)
        {
            return Enumerable.Range(0, count)
                .Select(n => MakeReg(string.Format(format, n), size))
                .ToArray();
        }

        public RegisterStorage[] RangeOfReg32(int count, string format)
            => RangeOfReg(count, format, PrimitiveType .Word32);

        public RegisterStorage[] RangeOfReg64(int count, string format)
            => RangeOfReg(count, format, PrimitiveType.Word64);
    }
}
