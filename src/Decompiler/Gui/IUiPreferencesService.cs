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
        public string Name;     // Name of this style
        public Font Font;
        public string ForeColor;
        public string BackColor;
        public string Cursor;
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
        public const string MemoryHeuristic = "mem-heu";
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

        [Obsolete] UiStyle MemoryWindowStyle { get; set; }
        [Obsolete] Font DisassemblerFont { get; set; }
        [Obsolete] UiStyle DisassemblerWindowStyle { get; set; }
        [Obsolete] UiStyle CodeWindowStyle { get; set; }
        [Obsolete] Font SourceCodeFont { get; set; }
        [Obsolete] Color SourceCodeForegroundColor { get; set; }
        [Obsolete] Color SourceCodeBackgroundColor { get; set; }

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

        [Obsolete]
        public Font DisassemblerFont { get { return dasmFont; } set { dasmFont = value; DisassemblyFontChanged.Fire(this); } }
        public event EventHandler DisassemblyFontChanged;
        private Font dasmFont; 
        public Color DisassemblerForegroundColor { get; set; }
        public Color DisassemblerBackgroundColor { get; set; }

        [Obsolete]
        public Font SourceCodeFont { get { return srcFont; } set { srcFont = value; SourceCodeFontChanged.Fire(this); } }
        public event EventHandler SourceCodeFontChanged;
        private Font srcFont; 
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
            var q = configSvc.GetDefaultPreferences();

            var defMemStyle = q.Where(s => s.Name == UiStyles.MemoryWindow).Single();
            var defMemCodeStyle = q.Where(s => s.Name == UiStyles.MemoryCode).Single();
            var defMemHeurStyle = q.Where(s => s.Name == UiStyles.MemoryHeuristic).Single();
            var defMemDataStyle = q.Where(s => s.Name == UiStyles.MemoryData).Single();
            AddStyle(new UiStyle
            {
                Name = UiStyles.MemoryWindow,
                ForeColor = (string)settingsSvc.Get(UiStyles.MemoryForeColor, defMemStyle.ForeColor),
                BackColor = (string)settingsSvc.Get(UiStyles.MemoryBackColor, defMemStyle.BackColor),
                Font = GetFont(UiStyles.MemoryFont, defMemStyle),
            });
            AddStyle(new UiStyle
            {
                Name = UiStyles.MemoryCode,
                ForeColor = (string)settingsSvc.Get(UiStyles.MemoryCodeForeColor, defMemCodeStyle.ForeColor),
                BackColor = (string)settingsSvc.Get(UiStyles.MemoryCodeBackColor, defMemCodeStyle.BackColor),
            });
            AddStyle(new UiStyle
            {
                Name = UiStyles.MemoryHeuristic,
                ForeColor = (string)settingsSvc.Get(UiStyles.MemoryHeuristicForeColor, defMemHeurStyle.ForeColor),
                BackColor = (string)settingsSvc.Get(UiStyles.MemoryHeuristicBackColor, defMemHeurStyle.BackColor),
            });
            AddStyle(new UiStyle
            {
                Name = UiStyles.MemoryData,
                ForeColor = (string)settingsSvc.Get(UiStyles.MemoryDataForeColor, defMemDataStyle.ForeColor),
                BackColor = (string)settingsSvc.Get(UiStyles.MemoryDataBackColor, defMemDataStyle.BackColor),
            });

            var defDisStyle = q.Where(s => s.Name == UiStyles.Disassembler).Single();
            var disStyle = new UiStyle
            {
                Name = UiStyles.Disassembler,
                ForeColor = (string)settingsSvc.Get(UiStyles.DisassemblerForeColor, defDisStyle.ForeColor),
                BackColor = (string)settingsSvc.Get(UiStyles.DisassemblerBackColor, defDisStyle.BackColor),
                Font = GetFont(UiStyles.DisassemblerFont, defDisStyle),
            };
            AddStyle(disStyle);
            var defDisOpStyle = q.Where(s => s.Name == UiStyles.DisassemblerOpcode).Single();
            var disOpStyle = new UiStyle
            {
                Name = UiStyles.DisassemblerOpcode,
                ForeColor = (string)settingsSvc.Get(UiStyles.DisassemblerOpcodeColor, defDisOpStyle.ForeColor)
            };
            AddStyle(disOpStyle);


            var defCodeStyle = q.Where(s => s.Name == UiStyles.CodeWindow).Single();
            var codeStyle = new UiStyle
            {
                Name = UiStyles.CodeWindow,
                ForeColor = (string)settingsSvc.Get(UiStyles.CodeForeColor, defCodeStyle.ForeColor),
                BackColor = (string)settingsSvc.Get(UiStyles.CodeBackColor, defCodeStyle.BackColor),
                Font = GetFont(UiStyles.CodeFont, defDisStyle),
            };
            AddStyle(codeStyle);

            var defCodeKwStyle = q.Where(s => s.Name == UiStyles.CodeKeyword).Single();
            var codeKwStyle = new UiStyle
            {
                Name = UiStyles.CodeKeyword,
                ForeColor = (string) settingsSvc.Get(UiStyles.CodeKeywordColor, defCodeKwStyle.ForeColor),
                Font = GetFont(UiStyles.CodeKeywordFont, defCodeStyle)
            };
            AddStyle(codeKwStyle);

            var defCodeCommentStyle = q.Where(s => s.Name == UiStyles.CodeComment).Single();
            var codeCommentStyle = new UiStyle
            {
                Name = UiStyles.CodeComment,
                ForeColor = (string)settingsSvc.Get(UiStyles.CodeCommentColor, defCodeCommentStyle.ForeColor),
                Font = GetFont(UiStyles.CodeCommentFont, defCodeStyle)
            };
            AddStyle(codeCommentStyle);

            this.dasmFont = ConvertFrom<Font>(fontCvt, settingsSvc.Get(UiStyles.DisassemblerFont, defDisStyle.FontName));
            this.srcFont = ConvertFrom<Font>(fontCvt, settingsSvc.Get(UiStyles.CodeFont,  defCodeStyle.FontName));
            
            this.WindowSize = ConvertFrom<Size>(sizeCvt, settingsSvc.Get("WindowSize", null));
            this.WindowState = ConvertFrom<FormWindowState>(fwsCvt, settingsSvc.Get("WindowState", "Normal"));
        }

        private Font GetFont(string fontNameSetting, Core.Configuration.UiStyle defMemStyle)
        {
            var fn = (string)settingsSvc.Get(fontNameSetting, defMemStyle.FontName);
            var fontCvt = TypeDescriptor.GetConverter(typeof(Font));
            var font =  ConvertFrom<Font>(fontCvt, fn);
            fn = SaveFont(font);
            return font;
        }

        private string SaveFont(Font font)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}pt", font.Name, font.Size);
        }

        private void AddStyle(UiStyle s)
        {
            Styles.Add(s.Name, s);
        }

        public void Save()
        {
            var fontCvt = TypeDescriptor.GetConverter(typeof(Font));
            var sizeCvt = TypeDescriptor.GetConverter(typeof(Size));

            var disStyle = Styles[UiStyles.Disassembler];
            settingsSvc.Set(UiStyles.DisassemblerForeColor, disStyle.ForeColor);
            settingsSvc.Set(UiStyles.DisassemblerBackColor, disStyle.BackColor);
            settingsSvc.Set(UiStyles.DisassemblerFont, SaveFont(disStyle.Font));

            var disOpStyle = Styles[UiStyles.DisassemblerOpcode];
            settingsSvc.Set(UiStyles.DisassemblerOpcodeColor, disOpStyle.ForeColor);
            
            var codeStyle = Styles[UiStyles.CodeWindow];
            settingsSvc.Get(UiStyles.CodeForeColor, codeStyle.ForeColor);
            settingsSvc.Get(UiStyles.CodeBackColor, codeStyle.BackColor);
            settingsSvc.Get(UiStyles.CodeFont, SaveFont(codeStyle.Font));

            var codeKwStyle = Styles[UiStyles.CodeKeyword];
            settingsSvc.Set(UiStyles.CodeKeywordColor, codeKwStyle.ForeColor);
            settingsSvc.Set(UiStyles.CodeKeywordFont, SaveFont(codeStyle.Font));

            var codeCommentStyle = Styles[UiStyles.CodeComment];
            settingsSvc.Set(UiStyles.CodeCommentColor, codeCommentStyle.ForeColor);
            settingsSvc.Set(UiStyles.CodeCommentFont, codeStyle.Font.FontFamily.ToString());

            settingsSvc.Set("DisassemblyFont", fontCvt.ConvertToInvariantString(dasmFont));
            settingsSvc.Set("SourceCodeFont", fontCvt.ConvertToInvariantString(srcFont));
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


        public UiStyle MemoryWindowStyle
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public UiStyle DisassemblerWindowStyle
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public UiStyle CodeWindowStyle
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
