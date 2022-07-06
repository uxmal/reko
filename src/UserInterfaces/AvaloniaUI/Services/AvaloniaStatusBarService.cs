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

using ReactiveUI;
using Reko.Gui.Services;
using Reko.UserInterfaces.AvaloniaUI.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    /// <summary>
    /// The status bar service has two "faces": one, its implementation of the
    /// <see cref="IStatusBarService"/>, used by the GUI-agnostic parts to show
    /// status messages etc. The other are the properties it exposes as a ViewModel.
    /// </summary>
    public class AvaloniaStatusBarService : 
        ReactiveObject,
        IStatusBarService
    {
        public AvaloniaStatusBarService(MainViewModel mvvm)
        {
            mvvm.Status = this;
        }

        public bool IsProgressVisible
        {
            get => isProgressVisible;
            set => this.RaiseAndSetIfChanged(ref isProgressVisible, value);
        }
        private bool isProgressVisible;

        public int ProgressPercentage
        {
            get => progressPercentage;
            set => this.RaiseAndSetIfChanged(ref progressPercentage, value);
        }
        private int progressPercentage;

        public string? Text
        {
            get { return text; }
            set { this.RaiseAndSetIfChanged(ref text, value, nameof(Text)); }
        }
        private string? text;

        public string? Subtext
        {
            get { return subtext; }
            set { this.RaiseAndSetIfChanged(ref subtext, value); }
        }
        private string? subtext;

        public void SetText(string text)
        {
            this.Text = text;
        }

        public void SetSubtext(string text)
        {
            this.Subtext = text;
        }

        public void ShowProgress(int percentage)
        {
            this.IsProgressVisible = true;
            this.ProgressPercentage = percentage;
        }

        public void HideProgress()
        {
            this.IsProgressVisible = false;
        }
    }
}
