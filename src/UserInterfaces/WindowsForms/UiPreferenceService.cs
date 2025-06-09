#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    public class UiPreferencesService : IUiPreferencesService
    {
        private IConfigurationService configSvc;
        private ISettingsService settingsSvc;
        private Dictionary<string, StyleSettingNames> SettingNames;

        public event EventHandler UiPreferencesChanged;

        public UiPreferencesService(IConfigurationService configSvc, ISettingsService settingsSvc)
        {
            this.configSvc = configSvc;
            this.settingsSvc = settingsSvc;
            this.Styles = new Dictionary<string, Gui.Services.UiStyle>();
            this.SettingNames = new StyleSettingNames[] {
                new StyleSettingNames
                {
                    Name = UiStyles.MemoryWindow,
                    ForeColor = UiStyles.MemoryForeColor,
                    BackColor = UiStyles.MemoryBackColor,
                    FontName = UiStyles.MemoryFont
                },
                new StyleSettingNames
                {
                    Name = UiStyles.MemoryCode,
                    ForeColor = UiStyles.MemoryCodeForeColor,
                    BackColor = UiStyles.MemoryCodeBackColor,
                },
                new StyleSettingNames
                {
                    Name = UiStyles.MemoryData,
                    ForeColor = UiStyles.MemoryDataForeColor,
                    BackColor = UiStyles.MemoryDataBackColor,
                },
                new StyleSettingNames
                {
                    Name = UiStyles.MemoryHeuristic,
                    ForeColor = UiStyles.MemoryHeuristicForeColor,
                    BackColor = UiStyles.MemoryHeuristicBackColor,
                },
                new StyleSettingNames
                {
                    Name = UiStyles.Disassembler,
                    ForeColor = UiStyles.DisassemblerForeColor,
                    BackColor = UiStyles.DisassemblerBackColor,
                    FontName = UiStyles.DisassemblerFont,
                },
                new StyleSettingNames
                {
                    Name = UiStyles.DisassemblerOpcode,
                    ForeColor = UiStyles.DisassemblerOpcodeColor,
                },
                new StyleSettingNames
                {
                    Name = UiStyles.CodeWindow,
                    ForeColor = UiStyles.CodeForeColor,
                    BackColor = UiStyles.CodeBackColor,
                    FontName = UiStyles.CodeFont,
                },
                new StyleSettingNames
                {
                    Name = UiStyles.CodeKeyword,
                    ForeColor = UiStyles.CodeKeywordColor,
                    FontName = UiStyles.CodeKeywordFont,
                },
                new StyleSettingNames
                {
                    Name = UiStyles.CodeComment,
                    ForeColor = UiStyles.CodeCommentColor,
                    FontName = UiStyles.CodeCommentFont,
                },
                new StyleSettingNames
                {
                    Name = UiStyles.Browser,
                    ForeColor = UiStyles.BrowserForeColor,
                    BackColor = UiStyles.BrowserBackColor,
                    FontName = UiStyles.BrowserFont,
                },
                new StyleSettingNames
                {
                    Name = UiStyles.List,
                    ForeColor = UiStyles.ListForeColor,
                    BackColor = UiStyles.ListBackColor,
                    FontName = UiStyles.ListFont,
                }
            }.ToDictionary(k => k.Name);
        }

        public class StyleSettingNames
        {
            internal string Name;
            internal string ForeColor;
            internal string BackColor;
            internal string FontName;
        }

        public IDictionary<string, Gui.Services.UiStyle> Styles { get; private set; }

        public Font MemoryStyle { get; set; }

        [Browsable(false)]
        public Size WindowSize { get; set; }

        [Browsable(false)]
        public Gui.Forms.FormWindowState WindowState { get; set; }

        public void Load()
        {
            var fontCvt = TypeDescriptor.GetConverter(typeof(Font));
            var sizeCvt = TypeDescriptor.GetConverter(typeof(Size));
            var fwsCvt = TypeDescriptor.GetConverter(typeof(FormWindowState));
            var colorCvt = TypeDescriptor.GetConverter(typeof(Color));
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
                    Foreground = GetBrush(dStyle.ForeColor),
                    Background = GetBrush(dStyle.BackColor),
                    Font = GetFont(dStyle.FontName),
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

            this.WindowSize = ConvertFrom<Size>(sizeCvt, (string) settingsSvc.Get("WindowSize", null));
            this.WindowState = ConvertFrom<Gui.Forms.FormWindowState>(fwsCvt, (string) settingsSvc.Get("WindowState", "Normal"));

            UiPreferencesChanged?.Invoke(this, EventArgs.Empty);
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
                Foreground = GetBrush((string) settingsSvc.Get(snames.ForeColor, defStyle.ForeColor)),
                Background = GetBrush((string) settingsSvc.Get(snames.BackColor, defStyle.BackColor)),
                Font = GetFont((string) settingsSvc.Get(snames.FontName, defStyle.FontName)),
                Width = string.IsNullOrEmpty(defStyle.Width) ? default(int?) : Convert.ToInt32(defStyle.Width),
                PaddingLeft = padLeft,
                PaddingTop = padTop,
                PaddingRight = padRight,
                PaddingBottom = padBottom,
            };
            AddStyle(uiStyle);
        }

        private Font GetFont(string fontName)
        {
            if (fontName is null)
                return null;
            var fontCvt = TypeDescriptor.GetConverter(typeof(Font));
            var font = ConvertFrom<Font>(fontCvt, fontName);
            return font;
        }

        private SolidBrush GetBrush(string sColor)
        {
            if (string.IsNullOrEmpty(sColor))
                return null;
            var colorCvt = TypeDescriptor.GetConverter(typeof(Color));
            var brush = new SolidBrush(ConvertFrom<Color>(colorCvt, sColor));
            return brush;
        }

        private Cursor GetCursor(string sCursor)
        {
            if (string.IsNullOrEmpty(sCursor))
                return null;
            var cursorCvt = TypeDescriptor.GetConverter(typeof(Cursor));
            var cursor = ConvertFrom<Cursor>(cursorCvt, sCursor);
            return cursor;
        }

        private string SaveFont(object oFont)
        {
            if (!(oFont is Font font))
                return null;
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}pt", font.Name, font.Size);
        }

        private string SaveBrush(object brush)
        {
            if (brush is null)
                return null;
            return string.Format(CultureInfo.InvariantCulture, "#{0:X6}", ((SolidBrush)brush).Color.ToArgb() & 0xFFFFFF);
        }

        private void AddStyle(UiStyle s)
        {
            Styles[s.Name] = s;
        }

        public void Save()
        {
            var fontCvt = TypeDescriptor.GetConverter(typeof(Font));
            var sizeCvt = TypeDescriptor.GetConverter(typeof(Size));

            var memStyle = Styles[UiStyles.MemoryWindow];
            settingsSvc.Set(UiStyles.MemoryForeColor, SaveBrush(memStyle.Foreground));
            settingsSvc.Set(UiStyles.MemoryBackColor, SaveBrush(memStyle.Background));
            settingsSvc.Set(UiStyles.MemoryFont, SaveFont(memStyle.Font));

            var memCodeStyle = Styles[UiStyles.MemoryCode];
            settingsSvc.Set(UiStyles.MemoryCodeForeColor, SaveBrush(memCodeStyle.Foreground));
            settingsSvc.Set(UiStyles.MemoryCodeBackColor, SaveBrush(memCodeStyle.Background));
            var memHeurStyle = Styles[UiStyles.MemoryHeuristic];
            settingsSvc.Set(UiStyles.MemoryHeuristicForeColor, SaveBrush(memHeurStyle.Foreground));
            settingsSvc.Set(UiStyles.MemoryHeuristicBackColor, SaveBrush(memHeurStyle.Background));
            var memDataStyle = Styles[UiStyles.MemoryData];
            settingsSvc.Set(UiStyles.MemoryDataForeColor, SaveBrush(memDataStyle.Foreground));
            settingsSvc.Set(UiStyles.MemoryDataBackColor, SaveBrush(memDataStyle.Background));

            var disStyle = Styles[UiStyles.Disassembler];
            settingsSvc.Set(UiStyles.DisassemblerForeColor, SaveBrush(disStyle.Foreground));
            settingsSvc.Set(UiStyles.DisassemblerBackColor, SaveBrush(disStyle.Background));
            settingsSvc.Set(UiStyles.DisassemblerFont, SaveFont(disStyle.Font));

            var disOpStyle = Styles[UiStyles.DisassemblerOpcode];
            settingsSvc.Set(UiStyles.DisassemblerOpcodeColor, SaveBrush(disOpStyle.Foreground));

            var codeStyle = Styles[UiStyles.CodeWindow];
            settingsSvc.Set(UiStyles.CodeForeColor, SaveBrush(codeStyle.Foreground));
            settingsSvc.Set(UiStyles.CodeBackColor, SaveBrush(codeStyle.Background));
            settingsSvc.Set(UiStyles.CodeFont, SaveFont(codeStyle.Font));

            var codeKwStyle = Styles[UiStyles.CodeKeyword];
            settingsSvc.Set(UiStyles.CodeKeywordColor, SaveBrush(codeKwStyle.Foreground));
            settingsSvc.Set(UiStyles.CodeKeywordFont, SaveFont(codeStyle.Font));

            var codeCommentStyle = Styles[UiStyles.CodeComment];
            settingsSvc.Set(UiStyles.CodeCommentColor, SaveBrush(codeCommentStyle.Foreground));
            settingsSvc.Set(UiStyles.CodeCommentFont, SaveFont(codeStyle.Font));

            var browserStyle = Styles[UiStyles.Browser];
            settingsSvc.Set(UiStyles.BrowserForeColor, SaveBrush(browserStyle.Foreground));
            settingsSvc.Set(UiStyles.BrowserBackColor, SaveBrush(browserStyle.Background));
            settingsSvc.Set(UiStyles.BrowserFont, SaveFont(browserStyle.Font));

            var listStyle = Styles[UiStyles.List];
            settingsSvc.Set(UiStyles.ListForeColor, SaveBrush(listStyle.Foreground));
            settingsSvc.Set(UiStyles.ListBackColor, SaveBrush(listStyle.Background));
            settingsSvc.Set(UiStyles.ListFont, SaveFont(listStyle.Font));

            settingsSvc.Set("WindowSize", sizeCvt.ConvertToInvariantString(WindowSize));
            settingsSvc.Set("WindowState", WindowState.ToString());
            UiPreferencesChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ResetStyle(string styleName)
        {
            this.Styles.Remove(styleName);
            var snames = SettingNames[styleName];
            settingsSvc.Delete(snames.ForeColor);
            settingsSvc.Delete(snames.BackColor);
            settingsSvc.Delete(snames.FontName);
            SetStyle(styleName);
            UiPreferencesChanged?.Invoke(this, EventArgs.Empty);
        }

        private T ConvertFrom<T>(TypeConverter conv, string value)
        {
            if (value is null)
                return default(T);
            try
            {
                return (T) conv.ConvertFromInvariantString(value);
            }
            catch
            {
                return default(T);
            }
        }

        public void UpdateControlStyle(string list, object oCtrl)
        {
            if (oCtrl is not Control ctrl) throw new ArgumentNullException(nameof(oCtrl));
            if (Styles.TryGetValue(UiStyles.List, out UiStyle style))
            {
                if (style.Background is not null)
                {
                    ctrl.BackColor = ((SolidBrush)style.Background).Color;
                }
                if (style.Foreground is not null)
                {
                    ctrl.ForeColor = ((SolidBrush)style.Foreground).Color;
                }
            }
        }

        public void UpdateControlStyle(string list, IControl ctrl)
        {
            if (ctrl is null) throw new ArgumentNullException(nameof(ctrl));
            if (Styles.TryGetValue(UiStyles.List, out UiStyle style))
            {
                if (style.Background is not null)
                {
                    ctrl.BackColor = ((SolidBrush) style.Background).Color;
                }
                if (style.Foreground is not null)
                {
                    ctrl.ForeColor = ((SolidBrush) style.Foreground).Color;
                }
            }
        }
    }
}
