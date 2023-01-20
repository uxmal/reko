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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Services
{
    public abstract class StatusBarService : IStatusBarService
    {
        public abstract void HideProgress();
        public abstract void SetSubtext(string v);
        public abstract void SetText(string text);
        public abstract void ShowProgress(int percentDone);

        protected string RenderAddressSelection(ProgramAddressRange range)
        {
            if (range is null)
                return "";
            var numbase = range.Program.Architecture.DefaultBase;
            if (range.Length > 1)
            {
                return $"{range.Address} (length {Convert.ToString(range.Length, numbase)})";
            }
            else
            {
                return range.Address.ToString();
            }
        }

    }
}
