#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Globalization;
using Reko.Gui.Controls;

namespace Reko.Gui
{
    public class UiStyle
    {
        public string Name { get; set; }     // Name of this style
        public object Font { get; set; }
        public object Foreground { get; set; }
        public object Background { get; set; }
        public object Cursor { get; set; }
        public string TextAlign { get; set; }
        public int? Width { get; set; } // If set, the width is fixed at a certain size.
        public float PaddingTop { get; set; }
        public float PaddingLeft { get; set; }
        public float PaddingBottom { get; set; }
        public float PaddingRight { get; set; }

        public UiStyle Clone()
        {
            return new UiStyle
            {
                Name = Name,
                Font = Font,
                Foreground = Foreground,
                Background = Background,
                Cursor = Cursor,
                Width = Width,
                TextAlign = TextAlign,
                PaddingTop = PaddingTop,
                PaddingLeft = PaddingLeft,
                PaddingBottom = PaddingBottom,
                PaddingRight = PaddingRight,
            };
        }
    }

    public static class UiStyles
    {
        public const string MemoryWindow = "mem";
        public const string MemoryFont = "mem-font";
        public const string MemoryForeColor = "mem-fore";
        public const string MemoryBackColor = "mem-back";
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
        public const string CodeKeyword = "code-kw";
        public const string CodeKeywordFont = "code-kw-font";
        public const string CodeKeywordColor = "code-kw-fore";
        public const string CodeComment = "code-cmt";
        public const string CodeCommentFont = "code-cmt-font";
        public const string CodeCommentColor = "code-cmt-fore";

        public const string Browser = "browser";
        public const string BrowserFont = "browser-font";
        public const string BrowserForeColor = "browser-fore";
        public const string BrowserBackColor = "browser-back";

        public const string List = "list";
        public const string ListFont = "list-font";
        public const string ListForeColor = "list-fore";
        public const string ListBackColor = "list-back";
    }

    public interface IUiPreferencesService
    {
        event EventHandler UiPreferencesChanged;

        IDictionary<string, UiStyle> Styles { get; }

        Size WindowSize { get; set; }
        Forms.FormWindowState WindowState { get; set; }

        void Load();
        void Save();
        void ResetStyle(string styleName);
        void UpdateControlStyle(string styleName, object ctrl);
        void UpdateControlStyle(string styleName, IControl ctrl);
    }
}
