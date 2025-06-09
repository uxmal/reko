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

using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Gui.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class SegmentEditorDialog : Form, ISegmentEditorDialog
    {
        private readonly SegmentEditorInteractor interactor;

        public SegmentEditorDialog()
        {
            InitializeComponent();
            interactor = new SegmentEditorInteractor();
            interactor.Attach(this);
        }

        public IServiceProvider Services { get; set; }
        public byte[] Bytes { get; set; }

        public TextBox SegmentName => txtSegmentName;
        public TextBox Offset => txtFileOffset;
        public TextBox Length => txtLength;
        public TextBox Address => txtLoadAddress;
        public ComboBox Architectures => ddlArch;

        public CheckBox ReadMode => chkRead;
        public CheckBox WriteMode => chkWrite;
        public CheckBox ExecuteMode => chkExecute;

        public Button OkButton => btnOK;

        public void LoadUserSegment(MemoryArea mem, UserSegment segment)
        {
            if (mem is ByteMemoryArea bmem)
            {
                this.Bytes = bmem.Bytes;
                if (segment is not null)
                {
                    this.SegmentName.Text = segment.Name;
                    this.Offset.Text = segment.Offset.ToString("X");
                    this.Length.Text = segment.Length.ToString("X");
                    this.Address.Text = segment.Address.ToString();
                    interactor.SelectArchitecture(segment.Architecture.Name);
                    this.ReadMode.Checked = (segment.AccessMode & AccessMode.Read) != 0;
                    this.WriteMode.Checked = (segment.AccessMode & AccessMode.Read) != 0;
                    this.ExecuteMode.Checked = (segment.AccessMode & AccessMode.Read) != 0;
                }
                else
                {
                    this.Offset.Text = "0";
                    this.Length.Text = "0";
                    this.Address.Text = "0";
                }
            }
        }

        public UserSegment CreateUserSegment()
        {
            return interactor.CreateUserSegment();
        }
    }
}
