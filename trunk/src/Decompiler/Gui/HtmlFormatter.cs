#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Gui
{
    /// <summary>
    /// Formats code as HTML for presentation in Web browsers.
    /// </summary>
    public class HtmlFormatter : Formatter
    {
        public HtmlFormatter(TextWriter writer) : base(writer)
        {
            base.UseTabs = false;
        }

        public override void Write(string text)
        {
            if (text == null) 
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
                case '<': writer.Write("&lt;"); break;
                case '>': writer.Write("&gt;"); break;
                case '&': writer.Write("&amp;"); break;
                case '"': writer.Write("&quot;"); break;
                case ' ': writer.Write("&nbsp;"); break;
                default: writer.Write(ch); break;
                }
            }
        }

        public override void Write(string format, params object[] arguments)
        {
            if (format == null)
                throw new ArgumentNullException("format");
            Write(string.Format(format, arguments));
        }

        public override void WriteComment(string comment)
        {
            if (string.IsNullOrEmpty(comment))
                return;
            writer.Write("<span class=\"comment\">");
            Write(comment);
            writer.Write("</span>");
        }

        public override void WriteKeyword(string keyword)
        {
            if (keyword == null)
                return;
            writer.Write("<span class=\"kw\">");
            Write(keyword);
            writer.Write("</span>");
        }

        public override void WriteLine()
        {
            writer.WriteLine("<br />");
        }

        public void WriteHyperlink(string text, string href)
        {
            if (text == null)
                return;
            writer.Write("<a");
            if (!string.IsNullOrEmpty(href))
            {
                writer.Write(" href=\"");
                WriteEntityEscaped(href);
                writer.Write("\"");
            }
            writer.Write(">");
            WriteEntityEscaped(text);
            writer.Write("</a>");
        }
    }
}