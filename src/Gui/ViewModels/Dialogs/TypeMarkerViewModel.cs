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
using Reko.Gui.Components;
using System;

namespace Reko.Gui.ViewModels.Dialogs
{
    public class TypeMarkerViewModel : ReactingObject
    {
        private Program program;
        private Address addr;
        private string captionFormat;

        public TypeMarkerViewModel(Program program, Address addr, string captionFormat)
        {
            this.program = program;
            this.addr = addr;
            this.captionFormat = captionFormat;
        }

        public void Show()
        {
            this.IsVisible = true;
            this.Caption = string.Format(captionFormat, addr);
        }

        public void Hide()
        {
            this.IsVisible = false;
        }

        public string? Caption
        {
            get => this.caption;
            set => this.RaiseAndSetIfChanged(ref this.caption, value);
        }
        private string? caption;

        public string? UserText
        { 
            get => this.userText;
            set => this.RaiseAndSetIfChanged(ref this.userText, value);
        }
        private string? userText;

        public string? FormattedType 
        {
            get => this.formattedType;
            set => this.RaiseAndSetIfChanged(ref this.formattedType, value);
        }
        private string? formattedType;


        public string FormatType(string text)
        {
            try
            {
                var dataType = HungarianParser.Parse(text);
                if (dataType is null)
                    return " - Null - ";
                else
                    return dataType.ToString();
            }
            catch
            {
                return " - Error - ";
            }
        }

        public bool IsVisible
        {
            get => isVisible;
            set => this.RaiseAndSetIfChanged(ref isVisible, value);
        }
        private bool isVisible;
    }
}
