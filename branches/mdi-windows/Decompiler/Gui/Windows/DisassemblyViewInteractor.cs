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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class DisassemblyViewInteractor : IWindowPane
    {
        private TextBox txtDisassembly;
        private IServiceProvider sp;

        #region IWindowPane Members

        public Control CreateControl()
        {
            txtDisassembly = new TextBox();
            this.txtDisassembly.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.txtDisassembly.Multiline = true;
            this.txtDisassembly.Name = "txtDisassembly";
            this.txtDisassembly.ReadOnly = true;
            this.txtDisassembly.WordWrap = false;
            txtDisassembly.Resize += new EventHandler(txtDisassembly_Resize);
            return txtDisassembly;
        }

        public Address SelectedAddress { get; set;}  //$RENAME: StartAddress
        
        public void DumpAssembler()
        {
            var decSvc = (IDecompilerService) sp.GetService(typeof(IDecompilerService));
            if (decSvc == null)
                throw new InvalidOperationException("Expected IDecompilerService to be available.");
            var Decompiler = decSvc.Decompiler;


            if (!IsProgramLoaded(Decompiler))
            {
                txtDisassembly.Text = "";
            }
            else
            {
                int lines = CountVisibleLines();
                if (lines < 1)
                    lines = 1;
                StringWriter writer = new StringWriter();
                var arch = Decompiler.Program.Architecture;
                Dumper dumper = arch.CreateDumper();
                Disassembler dasm = arch.CreateDisassembler(Decompiler.Program.Image.CreateReader(SelectedAddress));
                while (lines > 0)
                {
                    dumper.DumpAssemblerLine(Decompiler.Program.Image, dasm, true, true, writer);
                    --lines;
                }
                txtDisassembly.Text = writer.ToString();
            }
        }

        private int CountVisibleLines()
        {
            return (txtDisassembly.Height + txtDisassembly.Font.Height - 1) / txtDisassembly.Font.Height;
        }


        private bool IsProgramLoaded(IDecompiler decompiler)
        {
            return
                decompiler != null &&
                decompiler.Program.Architecture != null &&
                decompiler.Program.Image != null &&
                SelectedAddress != null;
        }

        public void SetSite(IServiceProvider sp)
        {
            this.sp = sp;
        }

        public void Close()
        {
        }

        #endregion

        internal void ClearText()
        {
            txtDisassembly.Text = "";
        }

        void txtDisassembly_Resize(object sender, EventArgs e)
        {
            DumpAssembler();
        }
    
    
    }
}
