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

using Reko.Core.Configuration;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using Color = Avalonia.Media.Color;
using TypeFace = Avalonia.Media.Typeface;
using Size = System.Drawing.Size;
using Cursor = Avalonia.Input.Cursor;
using System.Linq;
using Avalonia;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaUiPreferencesService : IUiPreferencesService
    {
        public event EventHandler? UiPreferencesChanged;

        private readonly IConfigurationService configSvc;
        private readonly ISettingsService settingsSvc;
        private readonly Dictionary<string, StyleSettingNames> SettingNames;

        public AvaloniaUiPreferencesService(IConfigurationService configSvc, ISettingsService settingsSvc)
        {
            this.configSvc = configSvc;
            this.settingsSvc = settingsSvc;
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

        public void Load()
        {
            //var fontCvt = TypeDescriptor.GetConverter(typeof(Font));
            //var sizeCvt = TypeDescriptor.GetConverter(typeof(Size));
            //var fwsCvt = TypeDescriptor.GetConverter(typeof(FormWindowState));
            //var colorCvt = TypeDescriptor.GetConverter(typeof(Color));
            foreach (var dStyle in configSvc.GetDefaultPreferences())
            {
                int? width = null;
                if (Int32.TryParse(dStyle.Width, out int w))
                    width = w;
                float.TryParse(dStyle.PaddingTop, out float padTop);
                float.TryParse(dStyle.PaddingLeft, out float padLeft);
                float.TryParse(dStyle.PaddingBottom, out float padBottom);
                float.TryParse(dStyle.PaddingRight, out float padRight);
                AddStyle(new UiStyle
                {
                    Name = dStyle.Name,
                    Foreground = GetColor(dStyle.ForeColor),
                    Background = GetColor(dStyle.BackColor),
                    Font = dStyle.FontName,
                    Width = width,
                    Cursor = GetCursor(dStyle.Cursor),
                    PaddingTop = padTop,
                    PaddingLeft = padLeft,
                    PaddingBottom = padBottom,
                    PaddingRight = padRight,
                });
            }

            SetStyle(UiStyles.MemoryWindow);
            SetStyle(UiStyles.MemoryCode);
            SetStyle(UiStyles.MemoryHeuristic);
            SetStyle(UiStyles.MemoryData);

            SetStyle(UiStyles.Disassembler);
            SetStyle(UiStyles.DisassemblerOpcode);

            SetStyle(UiStyles.CodeWindow);
            SetStyle(UiStyles.CodeKeyword);
            SetStyle(UiStyles.CodeComment);

            SetStyle(UiStyles.Browser);
            SetStyle(UiStyles.List);

            //$TODO: store sizes in some usefule way.
            //this.WindowSize = ConvertFrom<Size>(sizeCvt, (string) settingsSvc.Get("WindowSize", null));
            //this.WindowState = ConvertFrom<Gui.Forms.FormWindowState>(fwsCvt, (string) settingsSvc.Get("WindowState", "Normal"));

            this.UiPreferencesChanged?.Invoke(this, EventArgs.Empty);
        }

        private Cursor? GetCursor(string? cursor)
        {
            //$TODO: implement this.
            return null;
        }

        private void SetStyle(string name)
        {
            var defStyle = configSvc
                .GetDefaultPreferences()
                .Where(s => s.Name == name)
                .Single();

            var snames = this.SettingNames[name];

            float.TryParse(defStyle.PaddingTop, out float padTop);
            float.TryParse(defStyle.PaddingLeft, out float padLeft);
            float.TryParse(defStyle.PaddingBottom, out float padBottom);
            float.TryParse(defStyle.PaddingRight, out float padRight);
            var uiStyle = new UiStyle
            {
                Name = snames.Name,
                Foreground = GetColor((string) settingsSvc.Get(snames.ForeColor, defStyle.ForeColor)),
                Background = GetColor((string) settingsSvc.Get(snames.BackColor, defStyle.BackColor)),
                Font = settingsSvc.Get(snames.FontName, defStyle.FontName),
                Width = string.IsNullOrEmpty(defStyle.Width) ? default(int?) : Convert.ToInt32(defStyle.Width),
                PaddingLeft = padLeft,
                PaddingTop = padTop,
                PaddingRight = padRight,
                PaddingBottom = padBottom,
            };
            AddStyle(uiStyle);
        }


        private void AddStyle(UiStyle s)
        {
            Styles[s.Name] = s;
        }

        private Color GetColor(string hex)
        {
            if (Avalonia.Media.Color.TryParse(hex, out var color))
                return color;
            else
                return Avalonia.Media.Colors.Transparent;
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
            //$TODO: WIP: this may need to change to UpdateClasses
        }

        public class StyleSettingNames
        {
            internal string Name;
            internal string ForeColor;
            internal string BackColor;
            internal string FontName;
        }
    }
}