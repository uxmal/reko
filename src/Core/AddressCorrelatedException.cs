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

namespace Reko.Core
{
    /// <summary>
    /// An exception that happened while processing something at a particular address.
    /// </summary>
    public class AddressCorrelatedException : Exception
    {
        public AddressCorrelatedException(Address addr, string format, params object[] args)
            : base(string.Format(format, args))
        {
            this.Address = addr;
        }

        public AddressCorrelatedException(Address addr, Exception innerException, string format, params object[] args)
            : base(string.Format(format, args), innerException)
        {
            this.Address = addr;
        }

        public Address Address { get; }
    }
}
