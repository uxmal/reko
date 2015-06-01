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

using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Gui;
using Decompiler.Gui.Windows.Controls;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Controls
{
    /// <summary>
    /// Renders disassembled machine instructions.
    /// </summary>
    public class DisassemblyControl : TextView
    {
        public DisassemblyControl()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            StartAddressChanged += DisassemblyControl_StateChange;
            TopAddressChanged += DisassemblyControl_StateChange;

            //$TODO: allow user to manipulate these hard-wired constants.
            Styles.Add("opcode", new EditorStyle { Width = 100 });
            Styles.Add("bytes", new EditorStyle { Width = 200 });
            Styles.Add("addrText", new EditorStyle { 
                Foreground = new SolidBrush(Color.FromArgb(0x00, 0x80, 0x80)),
                Cursor = Cursors.Hand
            });
        }

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

        void DisassemblyControl_StateChange(object sender, EventArgs e)
        {
            Model.MoveTo(topAddress, 0);
            RecomputeLayout();
            base.UpdateScrollbar();
            Invalidate();
        }

        protected override void OnScroll()
        {
            base.OnScroll();
            topAddress = (Address)Model.CurrentPosition;
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
            Debug.Print("Disassembly control: Down:  c:{0} d:{1} v:{2}", e.KeyCode, e.KeyData, e.KeyValue);
            switch (e.KeyCode)
            {
            case Keys.Down:
                Model.MoveTo(Model.CurrentPosition, 1);
                break;
            case Keys.Up:
                Model.MoveTo(Model.CurrentPosition, -1);
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
