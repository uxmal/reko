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

using System;
using System.Windows.Forms;
using Reko.Gui;
using Reko.Gui.Services;

namespace Reko.UserInterfaces.WindowsForms
{
    public class StatusBarService : IStatusBarService
    {
        private StatusStrip statusStrip;

        public StatusBarService(StatusStrip statusStrip)
        {
            this.statusStrip = statusStrip;
        }

        public void HideProgress()
        {
            throw new NotImplementedException();
        }

        public void SetSubtext(string v)
        {
            throw new NotImplementedException();
        }

        public void SetText(string text)
        {
            statusStrip.Items[0].Text = text;
        }

        public void ShowProgress(int percentDone)
        {
            throw new NotImplementedException();
        }
    }
}