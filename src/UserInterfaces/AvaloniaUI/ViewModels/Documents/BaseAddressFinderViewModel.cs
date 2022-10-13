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

using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;
using Reko.Gui;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents
{
    public class BaseAddressFinderViewModel : Document, IWindowPane
    {
        public BaseAddressFinderViewModel()
        {
            this.startStopButtonText = "_Start";
            this.Results = new();
        }

        public bool ByString
        {
            get => byString;
            set => this.RaiseAndSetIfChanged(ref byString, value);
        }
        private bool byString;

        public bool ByProlog
        {
            get => byProlog;
            set => this.RaiseAndSetIfChanged(ref byProlog, value);
        }
        private bool byProlog;

        public int ByStringProgress
        {
            get => byStringProgress;
            set => this.RaiseAndSetIfChanged(ref byStringProgress, value);
        }
        private int byStringProgress;

        public int ByPrologProgress
        {
            get => byPrologProgress;
            set => this.RaiseAndSetIfChanged(ref byPrologProgress, value);
        }
        private int byPrologProgress;

        public string StartStopButtonText
        {
            get => startStopButtonText;
            set => this.RaiseAndSetIfChanged(ref startStopButtonText, value);
        }
        public string startStopButtonText;

        public ObservableCollection<BaseAddressResult> Results { get; }

        public string BaseAddress
        {
            get => baseAddress;
            set => this.RaiseAndSetIfChanged(ref baseAddress, value);
        }
        public string baseAddress;

        public IWindowFrame? Frame { get; set; }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public object CreateControl()
        {
            throw new NotImplementedException();
        }

        public void SetSite(IServiceProvider services)
        {
            throw new NotImplementedException();
        }
    }

    public class BaseAddressResult
    {
        public string? Address { get; set; }

        public int Hits { get; set; }
    }
}

