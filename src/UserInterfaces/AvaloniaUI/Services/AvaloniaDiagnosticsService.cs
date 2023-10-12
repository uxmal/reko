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
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Services;
using Reko.UserInterfaces.AvaloniaUI.ViewModels;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaDiagnosticsService : IDiagnosticsService, ICommandTarget
    {
        private readonly IServiceProvider services;
        private readonly DiagnosticsViewModel diagnosticsVm;
        private readonly SynchronizationContext syncCtx;

        public AvaloniaDiagnosticsService(
            IServiceProvider services, 
            DiagnosticsViewModel diagnosticsVm, 
            SynchronizationContext ctx)
        {
            this.services = services;
            this.diagnosticsVm = diagnosticsVm;
            this.syncCtx = ctx;
        }

        public DiagnosticFilters Filter { get; set; }

        private void AddDiagnostic(ICodeLocation location, Diagnostic d)
        {
            syncCtx.Post(x =>
            {
                diagnosticsVm.AddDiagnostic(location, d);
            }, null);
        }

        public void ClearDiagnostics()
        {
            diagnosticsVm.ClearItems();
        }

        public void Error(string message)
        {
            Error(new NullCodeLocation(""), message);
        }

        public void Error(string message, params object[] args)
        {
            Error(string.Format(message, args));
        }

        public void Error(Exception ex, string message)
        {
            AddDiagnostic(new NullCodeLocation(""), new ErrorDiagnostic(message, ex));
        }

        public void Error(ICodeLocation location, string message)
        {
            AddDiagnostic(location, new ErrorDiagnostic(message));
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            Error(location, string.Format(message, args));
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            AddDiagnostic(location, new ErrorDiagnostic(message, ex));
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        {
            Error(location, ex, string.Format(message, args));
        }

        public void Inform(string message)
        {
            AddDiagnostic(new NullCodeLocation(""), new InformationalDiagnostic(message));
        }

        public void Inform(string message, params object[] args)
        {
            Inform(string.Format(message, args));
        }

        public void Inform(ICodeLocation location, string message)
        {
            AddDiagnostic(location, new InformationalDiagnostic(message));
        }

        public void Inform(ICodeLocation location, string message, params object[] args)
        {
            Inform(location, string.Format(message, args));
        }

        public void Warn(string message)
        {
            AddDiagnostic(new NullCodeLocation(""), new WarningDiagnostic(message));
        }

        public void Warn(string message, params object[] args)
        {
            Warn(string.Format(message, args));
        }

        public void Warn(ICodeLocation location, string message)
        {
            AddDiagnostic(location, new WarningDiagnostic(message));
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            Warn(location, string.Format(message, args));
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            throw new NotImplementedException();
        }
    }
}
