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

using Reko.Core.Output;
using System;
using System.IO;

namespace Reko.Gui
{
    /// <summary>
    /// Formats code as HTML for presentation in Web browsers.
    /// </summary>
    public class HtmlFormatter : TextFormatter
    {
        public HtmlFormatter(TextWriter writer) : base(writer)
        {
            base.UseTabs = false;
        }

        public override void Write(string text)
        {
            if (text is null) 
                return;
            WriteEntityEscaped(text);
        }

        private void WriteEntityEscaped(string text)
        {
            for (int i = 0; i < text.Length; ++i)
            {
                char ch = text[i];
                switch (ch)
                {
                case '<': base.TextWriter.Write("&lt;"); break;
                case '>': base.TextWriter.Write("&gt;"); break;
                case '&': base.TextWriter.Write("&amp;"); break;
                case '"': base.TextWriter.Write("&quot;"); break;
                case ' ': base.TextWriter.Write("&nbsp;"); break;
                default: base.TextWriter.Write(ch); break;
                }
            }
        }

        public override void Write(string format, params object[] arguments)
        {
            if (format is null)
                throw new ArgumentNullException(nameof(format));
            Write(string.Format(format, arguments));
        }

        public override void WriteComment(string comment)
        {
            if (string.IsNullOrEmpty(comment))
                return;
            TextWriter.Write("<span class=\"comment\">");
            Write(comment);
            TextWriter.Write("</span>");
        }

        public override void WriteKeyword(string keyword)
        {
            if (keyword is null)
                return;
            TextWriter.Write("<span class=\"kw\">");
            Write(keyword);
            TextWriter.Write("</span>");
        }

        public override void WriteLine()
        {
            TextWriter.WriteLine("<br />");
        }

        public override void WriteHyperlink(string text, object href)
        {
            if (text is null)
                return;
            var dest = href as string;
            TextWriter.Write("<a");
            if (!string.IsNullOrEmpty(dest))
            {
                TextWriter.Write(" href=\"");
                WriteEntityEscaped(dest!);
                TextWriter.Write("\"");
            }
            TextWriter.Write(">");
            WriteEntityEscaped(text);
            TextWriter.Write("</a>");
        }
    }
}