#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core;

namespace Reko.Gui.Windows
{
    public class MixedCodeDataViewService : ViewService, IMixedCodeDataViewService
    {
        public MixedCodeDataViewService(IServiceProvider sp) : base(sp)
        {
        }

        /// <summary>
        /// Display the ProgramAddress.
        /// </summary>
        /// <param name="pa"></param>
        public void Show(ProgramAddress pa)
        {
            var frame = this.ShellUiSvc.FindDocumentWindow(GetType().Name, pa.Program);
            MixedCodeDataViewInteractor pane;
            if (frame == null)
            {
                pane = new MixedCodeDataViewInteractor();
                frame = ShellUiSvc.CreateDocumentWindow(GetType().Name, pa.Program, pa.Program.Name, pane);
            }
            else
            {
                pane = (MixedCodeDataViewInteractor)frame.Pane;
            }
            frame.Show();
            pane.Program = pa.Program;
            pane.TopAddress = pa.Address;
        }
    }
}
