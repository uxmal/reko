/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Output;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public class RichEditFormatter : CodeFormatter
    {
        private RichTextBox txt;
        private SortedList<int, Procedure> procedureLinks;

        public RichEditFormatter(RichTextBox txt) : base(new InnerFormatter(txt)) 
        {
            this.txt = txt;
            procedureLinks = new SortedList<int, Procedure>();
        }

        public override void VisitProcedureConstant(ProcedureConstant pc)
        {
            Procedure proc = pc.Procedure as Procedure;
            if (proc == null)
            {
                base.VisitProcedureConstant(pc);
                return;
            }
            procedureLinks.Add(txt.SelectionStart, proc);
            Color old = txt.SelectionColor;
            Font oldFont = txt.SelectionFont;
            txt.SelectionColor = Color.Blue;
            txt.SelectionFont = new Font(oldFont, FontStyle.Underline);
            base.VisitProcedureConstant(pc);
            txt.SelectionColor = old;
            txt.SelectionFont = oldFont;
        }

        private class InnerFormatter : Formatter
        {
            private RichTextBox txt;

            public InnerFormatter(RichTextBox txt) : base(StringWriter.Null)
            {
                this.txt = txt;
            }
            public override void Write(string s)
            {
                txt.SelectedText = s;
            }
            public override void Write(string format, params object[] arguments)
            {
                txt.SelectedText = string.Format(format, arguments);
            }

            public override void WriteKeyword(string keyword)
            {
                Color old = txt.SelectionColor;
                txt.SelectionColor = Color.Blue;
                txt.SelectedText = keyword;
                txt.SelectionColor = old;
            }

            public override void WriteLine()
            {
                txt.SelectedText = "\n";
            }
        }



        public Procedure GetProcedureAtIndex(int i)
        {
            foreach (KeyValuePair<int, Procedure> item in procedureLinks)
            {
                if (item.Key <= i && i < item.Key + item.Value.Name.Length)
                {
                    return item.Value;
                }
            }
            return null;
        }
    }
}
