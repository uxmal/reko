#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Loading;
using Reko.Gui.Services;
using Reko.Gui.ViewModels;
using Reko.Gui.ViewModels.Documents;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaWindowPaneFactory : IWindowPaneFactory
    {
        private readonly IServiceProvider services;

        public AvaloniaWindowPaneFactory(IServiceProvider services)
        {
            this.services = services;
        }

        public IWindowPane CreateBaseAddressFinderPane(Program program)
        {
            var bafVm = new BaseAddressFinderViewModel(services, program, "_Start", "_Stop");
            return new BaseAddressFinderDocumentViewModel(bafVm);
        }

        public IWindowPane CreateHexDisassemblerPane()
        {
            return new HexDisassemblerViewModel(services);
        }

        public IWindowPane CreateSegmentListPane(Program program)
        {
            return new SegmentListViewModel(program);
        }

        public IWindowPane CreateStructureEditorPane(Program program)
        {
            var seVm = new StructureEditorViewModel(services, program);
            return new StructureEditorDocumentViewModel(seVm);
        }

        public ILowLevelViewInteractor CreateLowLevelViewPane(Program program)
        {
            var llvm = new LowLevelViewModel(services, program);
            return new LowLevelDocumentViewModel(llvm);
        }
    }
}
