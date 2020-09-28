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
using Reko.Core.Machine;
using Reko.Gui;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    /// <summary>
    /// Renders disassembled machine instructions.
    /// </summary>
    public class DisassemblyControl : TextView
    {
        private DisassemblyTextModel dasmModel;

        public DisassemblyControl()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            StartAddressChanged += DisassemblyControl_StateChange;
            TopAddressChanged += DisassemblyControl_StateChange;
            ProgramChanged += DisassemblyControl_StateChange;
        }

        [Browsable(false)]
        public Program Program { get { return program; } set { program = value; ProgramChanged.Fire(this); } }
        public event EventHandler ProgramChanged;
        private Program program;

        [Browsable(false)]
        public Address StartAddress { get { return startAddress; } set { startAddress = value; StartAddressChanged.Fire(this); } }
        public event EventHandler StartAddressChanged;
        private Address startAddress;

        [Browsable(false)]
        public Address TopAddress { get { return topAddress; } set { topAddress = value; TopAddressChanged.Fire(this); } }
        public event EventHandler TopAddressChanged;
        private Address topAddress;

        [Browsable(false)]
        public object SelectedObject
        {
            get { return selectedObject; }
            set
            {
                selectedObject = value;
                Invalidate();
                SelectedObjectChanged.Fire(this);
            }
        }
        public event EventHandler SelectedObjectChanged;
        private object selectedObject;

        public bool ShowPcRelative
        {
            get { return dasmModel != null ? dasmModel.ShowPcRelative : false; }
            set
            {
                if (dasmModel == null)
                    return;
                dasmModel.ShowPcRelative = value;
                Invalidate();
                ShowPcRelativeChanged.Fire(this);
            }
        }
        public event EventHandler ShowPcRelativeChanged;

        void DisassemblyControl_StateChange(object sender, EventArgs e)
        {
            if (program == null || topAddress == null)
            {
                Model = new EmptyEditorModel();
            }
            else
            {
                ImageSegment segment;
                if (!program.SegmentMap.TryFindSegment(topAddress, out segment) ||
                    segment.MemoryArea == null)
                {
                    Model = new EmptyEditorModel();
                }
                else
                {
                    var addr = topAddress;
                    this.dasmModel = new DisassemblyTextModel(program, segment);
                    Model = dasmModel;
                    Model.MoveToLine(addr, 0);
                }
            }
            RecomputeLayout();
            base.UpdateScrollbar();
        }

        /*
          n = number of segments
          c = constant width
          b = per-segment constant width
          cb[i] = per-segment size in bytes
          Width = c + sum(b + cb[i] * scale)
          Width - c - n * b = scale * sum(cb[i])
          scale = ceil((width - c - n * b) / sum(cb[i]))
        */

        protected override void OnScroll()
        {
            base.OnScroll();
            topAddress = Model.CurrentPosition as Address;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData & ~Keys.Modifiers)
            {
            case Keys.Down:
            case Keys.Up:
            case Keys.Left:
            case Keys.Right:
                return true;
            default:
                return base.IsInputKey(keyData);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            //Debug.Print("Disassembly control: Down:  c:{0} d:{1} v:{2}", e.KeyCode, e.KeyData, e.KeyValue);
            switch (e.KeyCode)
            {
            case Keys.Down:
                Model.MoveToLine(Model.CurrentPosition, 1);
                break;
            case Keys.Up:
                Model.MoveToLine(Model.CurrentPosition, -1);
                break;
            default:
                base.OnKeyDown(e);
                return;
            }
            e.Handled = true;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            Invalidate();
            base.OnSizeChanged(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            var obj = GetTagFromPoint(new Point(e.X, e.Y));
            if (e.Button == MouseButtons.Left)
                SelectedObject = obj;       // Fire a notification only if left-click.
            else
                selectedObject = obj;
            base.OnMouseDown(e);
        }
    }

    public class AddressSpan : TextSpan
    {
        private string formattedAddress;

        public AddressSpan(string formattedAddress, Address addr, string style)
        {
            this.formattedAddress = formattedAddress;
            this.Tag = addr;
            base.Style = style;
        }

        public override string GetText()
        {
            return formattedAddress;
        }
    }
}
