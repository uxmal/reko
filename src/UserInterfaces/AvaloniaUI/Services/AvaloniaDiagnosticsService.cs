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

using Reko.Core;
using Reko.Gui.Services;
using Reko.UserInterfaces.AvaloniaUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaDiagnosticsService : IDiagnosticsService
    {
        private MainViewModel mainViewModel;

        public AvaloniaDiagnosticsService(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }

        public void ClearDiagnostics()
        {
            return; //$TODO
        }

        public void Error(string message)
        {
            throw new NotImplementedException();
        }

        public void Error(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Error(Exception ex, string message)
        {
            throw new NotImplementedException();
        }

        public void Error(ICodeLocation location, string message)
        {
            throw new NotImplementedException();
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            throw new NotImplementedException();
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Inform(string message)
        {
            throw new NotImplementedException();
        }

        public void Inform(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Inform(ICodeLocation location, string message)
        {
            throw new NotImplementedException();
        }

        public void Inform(ICodeLocation location, string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Warn(string message)
        {
            throw new NotImplementedException();
        }

        public void Warn(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Warn(ICodeLocation location, string message)
        {
            throw new NotImplementedException();
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
