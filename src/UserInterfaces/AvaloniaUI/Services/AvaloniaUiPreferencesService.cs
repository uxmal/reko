#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Gui.Controls;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaUiPreferencesService : IUiPreferencesService
    {
        private IServiceProvider services;

        public AvaloniaUiPreferencesService(IServiceProvider services)
        {
            this.services = services;
        }

        public IDictionary<string, UiStyle> Styles => throw new NotImplementedException();

        public Size WindowSize {
            get => new Size(800, 600);
            set { }  //$TODO 
        }

        public FormWindowState WindowState
        {
            get
            {
                return FormWindowState.Normal;
            }
            set
            {
                //$TODO
                throw new NotImplementedException();
            }
        }

        public event EventHandler? UiPreferencesChanged;

        public void Load()
        {
            //$TODO
        }

        public void ResetStyle(string styleName)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void UpdateControlStyle(string styleName, object ctrl)
        {
            throw new NotImplementedException();
        }

        public void UpdateControlStyle(string styleName, IControl ctrl)
        {
            throw new NotImplementedException();
        }
    }
}