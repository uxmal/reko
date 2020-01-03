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

        public StorageFactory(StorageDomain domain = StorageDomain.Register)
        {
            this.iReg = (int) domain;
        }

        public Dictionary<string, RegisterStorage> NamesToRegisters { get; } = new Dictionary<string, RegisterStorage>();
        public Dictionary<StorageDomain, RegisterStorage> DomainsToRegisters { get; } = new Dictionary<StorageDomain, RegisterStorage>();

        /// <summary>
        /// Create a single register.
        /// </summary>
        /// <param name="format">Format string used to generate the register name. Use {0} to 
        /// inject the current register number.
        /// </param>
        /// <param name="size">
        /// A <see cref="PrimitiveType"/> describing the size of the register.
        /// </param>
        /// <returns>A freshly created register.</returns>
        public RegisterStorage Reg(string format, PrimitiveType size)
        {
            var name = string.Format(format, iReg);
            return MakeReg(name, size);
        }

        private RegisterStorage MakeReg(string name, PrimitiveType size)
        {
            var reg = new RegisterStorage(name, iReg, 0, size);
            NamesToRegisters.Add(name, reg);
            DomainsToRegisters.Add(reg.Domain, reg);
            ++iReg;
            return reg;
        }

        /// <summary>
        /// Create a 16-bit register.
        /// </summary>
        public RegisterStorage Reg16(string format)
        {
            return Reg(format, PrimitiveType.Word16);
        }

        /// <summary>
        /// Create a 32-bit register.
        /// </summary>
        public RegisterStorage Reg32(string format)
        {
            return Reg(format, PrimitiveType.Word32);
        }

        /// <summary>
        /// Create a 64-bit register.
        /// </summary>
        public RegisterStorage Reg64(string format)
        {
            return Reg(format, PrimitiveType.Word64);
        }

        /// <summary>
        /// Generate a range of <paramref name="count"/> registers of the given
        /// <paramref name="size"/>. The name of each register is syntheiszed by
        /// the <paramref name="formatter"/>. 
        /// </summary>
        public RegisterStorage[] RangeOfReg(int count, Func<int, string> formatter, PrimitiveType size)
        {
            return Enumerable.Range(0, count)
                .Select(n => MakeReg(formatter(n), size))
                .ToArray();
        }

        /// <summary>
        /// Creates an array of 32-bit registers.
        /// </summary>
        public RegisterStorage[] RangeOfReg32(int count, string format)
            => RangeOfReg(count, n => string.Format(format, n), PrimitiveType.Word32);

        /// <summary>
        /// Creates an array of 64-bit registers.
        /// </summary>
        public RegisterStorage[] RangeOfReg64(int count, string format)
            => RangeOfReg(count, n => string.Format(format, n), PrimitiveType.Word64);
    }
}
