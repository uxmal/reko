#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui
{
    public class SelectedAddressService : ISelectedAddressService
    {
        public event EventHandler? SelectedAddressChanged;


        private Address? address;
        private long length;

        public SelectedAddressService()
        {
        }

        public Address? SelectedAddress
        {
            get => address;
            set
            {
                if (object.Equals(this.address, value))
                    return;
                this.address = value;
                SelectedAddressChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public long Length
        {
            get => length;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                if (this.length == value)
                    return;
                this.length = value;
                SelectedAddressChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
