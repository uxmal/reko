#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System;

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
        private readonly ISelectedAddressService selAddrSvc;

        public AvaloniaStatusBarService(ISelectedAddressService selAddrSvc)
        {
            this.selAddrSvc = selAddrSvc;
            selAddrSvc.SelectedAddressChanged += selAddrSvc_SelectedAddressChanged;
        }

        public bool IsProgressVisible
        {
            get => isProgressVisible;
            set => this.RaiseAndSetIfChanged(ref isProgressVisible, value);
        }
        private bool isProgressVisible;

        public int ProgressPercentage
        {
            get => field;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        public string? Text
        {
            get { return field; }
            set { this.RaiseAndSetIfChanged(ref field, value, nameof(Text)); }
        }

        public string? Subtext
        {
            get { return field; }
            set { this.RaiseAndSetIfChanged(ref field, value); }
        }

        public string? SelectedAddressRange
        {
            get { return field; }
            set { this.RaiseAndSetIfChanged(ref field, value); }
        }

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

        private void selAddrSvc_SelectedAddressChanged(object? sender, EventArgs e)
        {
            var addrRange = selAddrSvc.SelectedAddressRange;
            if (addrRange is null)
            {
                this.SelectedAddressRange = "";
            }
            else
            {
                this.SelectedAddressRange = addrRange.ToString();
            }
        }
    }
}
