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

using Reko.Core;
using Reko.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

namespace Reko.Gui
{
    public class UiStyle
    {
        public string Name { get; set; }     // Name of this style
        public Font Font { get; set; }
        public SolidBrush Foreground { get; set; }
        public SolidBrush Background { get; set; }
        public Cursor Cursor { get; set; }
        public int? Width { get; set; } // If set, the width is fixed at a certain size.
    }

    public static class UiStyles
    {
        public const string MemoryWindow = "mem";
        public const string MemoryFont = "mem-font";
        public const string MemoryForeColor= "mem-fore";
        public const string MemoryBackColor= "mem-back";
        public const string MemoryCode = "mem-code";
        public const string MemoryCodeForeColor = "mem-code-fore";
        public const string MemoryCodeBackColor = "mem-codeback";
        public const string MemoryHeuristic = "mem-heur";
        public const string MemoryHeuristicForeColor = "mem-heur-fore";
        public const string MemoryHeuristicBackColor = "mem-heur-fore";
        public const string MemoryData = "mem-data";
        public const string MemoryDataForeColor = "mem-data-fore";
        public const string MemoryDataBackColor = "mem-data-back";

        public const string Disassembler = "dasm";
        public const string DisassemblerFont = "dasm-font";
        public const string DisassemblerForeColor = "dasm-fore";
        public const string DisassemblerBackColor = "dasm-back";
        public const string DisassemblerOpcode = "dasm-opcode";
        public const string DisassemblerOpcodeColor = "dasm-opcode-fore";

        public const string CodeWindow = "code";
        public const string CodeFont = "code-font";
        public const string CodeForeColor = "code-fore";
        public const string CodeBackColor = "code-back";
        public const string CodeKeyword= "code-kw";
        public const string CodeKeywordFont = "code-kw-font";
        public const string CodeKeywordColor = "code-kw-fore";
        public const string CodeComment = "code-cmt";
        public const string CodeCommentFont = "code-cmt-font";
        public const string CodeCommentColor = "code-cmt-fore";
    }

    public interface IUiPreferencesService
    {
        event EventHandler UiPreferencesChanged;

        IDictionary<string, UiStyle> Styles { get; }

        Size WindowSize { get; set; }
        FormWindowState WindowState { get; set; }

        void Load();
        void Save();
    }

    public class UiPreferencesService : IUiPreferencesService
    {
        private IConfigurationService configSvc;
        private ISettingsService settingsSvc;

        public event EventHandler UiPreferencesChanged;

        public UiPreferencesService(IConfigurationService configSvc, ISettingsService settingsSvc)
        {
            this.configSvc = configSvc;
            this.settingsSvc = settingsSvc;
            this.Styles = new Dictionary<string, UiStyle>();
        }

        public IDictionary<string, UiStyle> Styles { get; private set; }

        public Font MemoryStyle { get; set; }

        public Color DisassemblerForegroundColor { get; set; }
        public Color DisassemblerBackgroundColor { get; set; }

        public Color SourceCodeForegroundColor { get; set; }
        public Color SourceCodeBackgroundColor { get; set; }

        [Browsable(false)]
        public Size WindowSize { get; set; }

        [Browsable(false)]
        public FormWindowState WindowState { get; set; }

        public void Load()
        {
            var fontCvt = TypeDescriptor.GetConverter(typeof(Font));
            var sizeCvt = TypeDescriptor.GetConverter(typeof(Size));
            var fwsCvt = TypeDescriptor.GetConverter(typeof(FormWindowState));
            var colorCvt = TypeDescriptor.GetConverter(typeof(Color));
            foreach (var dStyle in configSvc.GetDefaultPreferences())
            {
                int? width = null;
                int w;
                if (Int32.TryParse(dStyle.Width, out w))
                    width = w;
                AddStyle(new UiStyle
                    {
                        Name = dStyle.Name,
                        Foreground = GetBrush(dStyle.ForeColor),
                        Background = GetBrush(dStyle.BackColor),
                        Font = GetFont(dStyle.FontName),
                        Width = width
                    });
            }
            var q = configSvc.GetDefaultPreferences();

            var defMemStyle = q.Where(s => s.Name == UiStyles.MemoryWindow).Single();
            var defMemCodeStyle = q.Where(s => s.Name == UiStyles.MemoryCode).Single();
            var defMemHeurStyle = q.Where(s => s.Name == UiStyles.MemoryHeuristic).Single();
            var defMemDataStyle = q.Where(s => s.Name == UiStyles.MemoryData).Single();
            AddStyle(new UiStyle
            {
                Name = UiStyles.MemoryWindow,
                Foreground = GetBrush((string)settingsSvc.Get(UiStyles.MemoryForeColor, defMemStyle.ForeColor)),
                Background = GetBrush((string)settingsSvc.Get(UiStyles.MemoryBackColor, defMemStyle.BackColor)),
                Font = GetFont((string)settingsSvc.Get(UiStyles.MemoryFont, defMemStyle.FontName)),
            });
            AddStyle(new UiStyle
            {
                Name = UiStyles.MemoryCode,
                Foreground = GetBrush((string)settingsSvc.Get(UiStyles.MemoryCodeForeColor, defMemCodeStyle.ForeColor)),
                Background = GetBrush((string)settingsSvc.Get(UiStyles.MemoryCodeBackColor, defMemCodeStyle.BackColor)),
            });
            AddStyle(new UiStyle
            {
                Name = UiStyles.MemoryHeuristic,
                Foreground = GetBrush((string)settingsSvc.Get(UiStyles.MemoryHeuristicForeColor, defMemHeurStyle.ForeColor)),
                Background = GetBrush((string)settingsSvc.Get(UiStyles.MemoryHeuristicBackColor, defMemHeurStyle.BackColor)),
            });
            AddStyle(new UiStyle
            {
                Name = UiStyles.MemoryData,
                Foreground = GetBrush((string)settingsSvc.Get(UiStyles.MemoryDataForeColor, defMemDataStyle.ForeColor)),
                Background = GetBrush((string)settingsSvc.Get(UiStyles.MemoryDataBackColor, defMemDataStyle.BackColor)),
            });

            var defDisStyle = q.Where(s => s.Name == UiStyles.Disassembler).Single();
            var disStyle = new UiStyle
            {
                Name = UiStyles.Disassembler,
                Foreground = GetBrush((string)settingsSvc.Get(UiStyles.DisassemblerForeColor, defDisStyle.ForeColor)),
                Background = GetBrush((string)settingsSvc.Get(UiStyles.DisassemblerBackColor, defDisStyle.BackColor)),
                Font = GetFont((string)settingsSvc.Get(UiStyles.DisassemblerFont, defDisStyle.FontName)),
            };
            AddStyle(disStyle);
            var defDisOpStyle = q.Where(s => s.Name == UiStyles.DisassemblerOpcode).Single();
            var disOpStyle = new UiStyle
            {
                Name = UiStyles.DisassemblerOpcode,
                Foreground = GetBrush((string)settingsSvc.Get(UiStyles.DisassemblerOpcodeColor, defDisOpStyle.ForeColor)),
                Width = string.IsNullOrEmpty(defDisOpStyle.Width) ? default(int?) : Convert.ToInt32(defDisOpStyle.Width),
            };
            AddStyle(disOpStyle);


            var defCodeStyle = q.Where(s => s.Name == UiStyles.CodeWindow).Single();
            var codeStyle = new UiStyle
            {
                Name = UiStyles.CodeWindow,
                Foreground = GetBrush((string)settingsSvc.Get(UiStyles.CodeForeColor, defCodeStyle.ForeColor)),
                Background = GetBrush((string)settingsSvc.Get(UiStyles.CodeBackColor, defCodeStyle.BackColor)),
                Font = GetFont((string)settingsSvc.Get(UiStyles.CodeFont, defDisStyle.FontName)),
            };
            AddStyle(codeStyle);

            var defCodeKwStyle = q.Where(s => s.Name == UiStyles.CodeKeyword).Single();
            var codeKwStyle = new UiStyle
            {
                Name = UiStyles.CodeKeyword,
                Foreground = GetBrush((string)settingsSvc.Get(UiStyles.CodeKeywordColor, defCodeKwStyle.ForeColor)),
                Font = GetFont((string)settingsSvc.Get(UiStyles.CodeKeywordFont, defCodeStyle.FontName))
            };
            AddStyle(codeKwStyle);

            var defCodeCommentStyle = q.Where(s => s.Name == UiStyles.CodeComment).Single();
            var codeCommentStyle = new UiStyle
            {
                Name = UiStyles.CodeComment,
                Foreground = GetBrush((string)settingsSvc.Get(UiStyles.CodeCommentColor, defCodeCommentStyle.ForeColor)),
                Font = GetFont((string)settingsSvc.Get(UiStyles.CodeCommentFont, defCodeStyle.FontName))
            };
            AddStyle(codeCommentStyle);

            this.WindowSize = ConvertFrom<Size>(sizeCvt, settingsSvc.Get("WindowSize", null));
            this.WindowState = ConvertFrom<FormWindowState>(fwsCvt, settingsSvc.Get("WindowState", "Normal"));
        }

        private Font GetFont(string fontName)
        {
            if (fontName == null)
                return null;
            var fontCvt = TypeDescriptor.GetConverter(typeof(Font));
            var font =  ConvertFrom<Font>(fontCvt, fontName);
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

        private string SaveFont(Font font)
        {
            if (font == null)
                return null;
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}pt", font.Name, font.Size);
        }

        private string SaveBrush(SolidBrush brush)
        {
            if (brush == null)
                return null;
            return string.Format(CultureInfo.InvariantCulture, "#{0:X6}", brush.Color.ToArgb() & 0xFFFFFF);
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
            settingsSvc.Set(UiStyles.CodeCommentFont,  SaveFont(codeStyle.Font));

            settingsSvc.Set("WindowSize", sizeCvt.ConvertToInvariantString(WindowSize));
            settingsSvc.Set("WindowState", WindowState.ToString());
            UiPreferencesChanged.Fire(this);
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
