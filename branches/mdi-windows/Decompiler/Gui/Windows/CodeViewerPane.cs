/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Gui.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class CodeViewerPane : IWindowPane
    {
        private RichTextBox txtProcedure;
        private RichEditFormatter formatter;

        public RichTextBox ProcedureText
        {
            get { return txtProcedure; }
        }

        #region IWindowPane Members

        public Control CreateControl()
        {
            this.txtProcedure = new RichTextBox();
            this.txtProcedure.Dock = DockStyle.Fill;
            this.txtProcedure.Name = "txtProcedure";
            this.txtProcedure.Text = "";
            this.txtProcedure.MouseClick += new MouseEventHandler(ProcedureText_MouseClick);

            return txtProcedure;
        }

        public void SetSite(IServiceProvider sp)
        {
        }

        public void Close()
        {
            txtProcedure.Dispose();
        }

        #endregion

        public void DisplayProcedure(Procedure proc)
        {
            if (txtProcedure == null)
                return;

            formatter = new RichEditFormatter(txtProcedure);
            if (proc != null)
            {
                formatter.Write(proc);
            }
        }

        private void ProcedureText_MouseClick(object sender, MouseEventArgs e)
        {
            int i = txtProcedure.GetCharIndexFromPosition(e.Location);
            Procedure proc = formatter.GetProcedureAtIndex(i);
            if (proc == null)
                return;
            DisplayProcedure(proc);
        }

    }
}
