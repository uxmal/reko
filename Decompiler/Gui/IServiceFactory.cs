#region License
/* 
* Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core.Services;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Forms;
using Decompiler.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace Decompiler.Gui
{
    /// <summary>
    /// Decouples the creation of services, so that proper unit testing can be done.
    /// </summary>
    public interface IServiceFactory
    {
        IDecompilerConfigurationService CreateDecompilerConfiguration();
        IDiagnosticsService CreateDiagnosticsService(ListView list);
        IMemoryViewService CreateMemoryViewService();
        IDisassemblyViewService  CreateDisassemblyViewService();
        IDecompilerService CreateDecompilerService();
        DecompilerEventListener CreateDecompilerEventListener();
        InitialPageInteractor CreateInitialPageInteractor();
        ILoadedPageInteractor CreateLoadedPageInteractor();
    }

    public class ServiceFactory : IServiceFactory
    {
        private IServiceProvider services;

        public ServiceFactory(IServiceProvider services)
        {
            this.services = services;
        }
        public IDecompilerConfigurationService CreateDecompilerConfiguration()
        {
            return new DecompilerConfiguration();
        }

        public IDiagnosticsService CreateDiagnosticsService(ListView list)
        {
            var d = new DiagnosticsInteractor();
            d.Attach(list);
            return d;
        }
        public IMemoryViewService CreateMemoryViewService()
        {
             return new MemoryViewServiceImpl(services);
        }

        public IDisassemblyViewService CreateDisassemblyViewService()
        {
            return new DisassemblyViewServiceImpl(services);
        }

        public IDecompilerService CreateDecompilerService()
        {
            return new DecompilerService();
        }

        public DecompilerEventListener CreateDecompilerEventListener()
        {
            return new WindowsDecompilerEventListener(services);
        }

        public InitialPageInteractor CreateInitialPageInteractor()
        {
            return new InitialPageInteractorImpl();
        }

        public ILoadedPageInteractor CreateLoadedPageInteractor()
        {
            return new LoadedPageInteractor();
        }

    }
}
