#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Decompiler.Gui
{
    public interface IUiPreferencesService
    {
        Font DisassemblyFont { get; set; }
        Font SourceCodeFont { get; set; }
        Size WindowSize { get; set; }
        FormWindowState WindowState { get; set; }

        void Load();
        void Save();
    }

    public class UiPreferencesService : IUiPreferencesService
    {
        private IDecompilerConfigurationService configSvc;
        private ISettingsService settingsSvc;

        public UiPreferencesService(IDecompilerConfigurationService configSvc, ISettingsService settingsSvc)
        {
            this.configSvc = configSvc;
            this.settingsSvc = settingsSvc;
        }

        public Font DisassemblyFont { get { return dasmFont; } set { dasmFont = value; DisassemblyFontChanged.Fire(this); } }
        public event EventHandler DisassemblyFontChanged;
        private Font dasmFont;

        public Font SourceCodeFont { get { return srcFont; } set { srcFont = value; SourceCodeFontChanged.Fire(this); } }
        public event EventHandler SourceCodeFontChanged;
        private Font srcFont;

        [Browsable(false)]
        public Size WindowSize { get; set; }

        [Browsable(false)]
        public FormWindowState WindowState { get; set; }

        public void Load()
        {
            var fontCvt = TypeDescriptor.GetConverter(typeof(Font));
            var sizeCvt = TypeDescriptor.GetConverter(typeof(Size));
            var q = configSvc.GetDefaultPreferences();
            var dis = settingsSvc.Get("DisassemblyFont", q.DisassemblyFont);
            var src = settingsSvc.Get("SourceCodeFont", q.SourceCodeFont);
            var size = settingsSvc.Get("WindowSize", null);
            var sState = (string)settingsSvc.Get("WindowState", "Normal");
            this.dasmFont = ConvertFrom<Font>(fontCvt, dis);
            this.srcFont = ConvertFrom<Font>(fontCvt, src);
            this.WindowSize = ConvertFrom<Size>(sizeCvt, size);
            FormWindowState state;
            Enum.TryParse<FormWindowState>(sState, out state);
            this.WindowState = state;
        }

        public void Save()
        {
            var fontCvt = TypeDescriptor.GetConverter(typeof(Font));
            var sizeCvt = TypeDescriptor.GetConverter(typeof(Size));
            settingsSvc.Set("DisassemblyFont", fontCvt.ConvertToInvariantString(dasmFont));
            settingsSvc.Set("SourceCodeFont", fontCvt.ConvertToInvariantString(srcFont));
            settingsSvc.Set("WindowSize", sizeCvt.ConvertToInvariantString(WindowSize));
            settingsSvc.Set("WindowState", WindowState.ToString());
        }

        private T ConvertFrom<T>(TypeConverter conv, object value)
        {
            if (value == null)
                return default(T);
            try
            {
                return (T) conv.ConvertFrom(value);
            }
            catch
            {
                return default(T);
            }
        }
    }
}
