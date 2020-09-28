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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Gui
{
    public class AddressNavigator : ICodeLocation
    {
        private IServiceProvider sp;

        public AddressNavigator(Program program, Address addr, IServiceProvider sp)
        {
            this.Program = program;
            this.Address = addr;
            this.sp = sp;
        }

        public Address Address { get; private set; }
        public Program Program { get; private set; }

        #region ICodeLocation Members

        public string Text
        {
            get { return Address.ToString(); }
        }

        public void NavigateTo()
        {
            var svc = sp.RequireService<ILowLevelViewService>();
            svc.ShowMemoryAtAddress(Program, Address);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}!{1}", Program.Filename, Address);
        }
    }
}
