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
using Reko.Core.Loading;
using Reko.Gui;
using Reko.Gui.ViewModels.Documents;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class WindowsFormsPaneFactory : IWindowPaneFactory
    {
        private IServiceProvider services;

        public WindowsFormsPaneFactory(IServiceProvider services)
        {
            this.services = services;
        }

        public IWindowPane CreateBaseAddressFinderPane(Program program)
        {
            var vm = new BaseAddressFinderViewModel(services, program, "&Start", "&Stop");
            return new BaseAddressFinderView()
            {
                ViewModel = vm,
            };
        }

        public IWindowPane CreateHexDisassemblerPane()
        {
            return new HexDisassemblerController();
        }

        public IWindowPane CreateSegmentListPane(Program program)
        {
            return new SegmentListViewInteractor(program);
        }
    }
}
