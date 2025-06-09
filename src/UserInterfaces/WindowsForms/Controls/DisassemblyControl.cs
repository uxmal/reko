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
using Reko.Gui.TextViewing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    /// <summary>
    /// Renders disassembled machine instructions.
    /// </summary>
    public class DisassemblyControl : TextView
    {
        private readonly TextSpanFactory factory;
        private DisassemblyTextModel dasmModel;

        public DisassemblyControl()
        {
            this.factory = new WindowsFormsTextSpanFactory();
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            StartAddressChanged += DisassemblyControl_StateChange;
            TopAddressChanged += DisassemblyControl_StateChange;
            ProgramChanged += DisassemblyControl_StateChange;
        }

        //$TODO All of these properties belong to the DisassemblyTextModel

        [Browsable(false)]
        public Program Program { get { return program; } set { program = value; ProgramChanged?.Invoke(this, EventArgs.Empty); } }
        public event EventHandler ProgramChanged;
        private Program program;

        [Browsable(false)]
        public Address? StartAddress { 
            get { return startAddress; }
            set { 
                startAddress = value; StartAddressChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler StartAddressChanged;
        private Address? startAddress;

        [Browsable(false)]
        public Address? TopAddress
        {
            get { return topAddress; } 
            set { 
                topAddress = value; TopAddressChanged?.Invoke(this, EventArgs.Empty); 
            }
        }
        public event EventHandler TopAddressChanged;
        private Address? topAddress;

        [Browsable(false)]
        public object SelectedObject
        {
            get { return selectedObject; }
            set
            {
                selectedObject = value;
                Invalidate();
                SelectedObjectChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler SelectedObjectChanged;
        private object selectedObject;

        /// <summary>
        /// Architecture to use when displaying disassembly. Can be used
        /// to override the architecture from the <see cref="Program"/>
        /// property.
        /// </summary>
        [Browsable(false)]
        public IProcessorArchitecture Architecture
        {
            get { return this.arch;}
            set
            {
                if (this.arch == value)
                    return;
                this.arch = value;
                DisassemblyControl_StateChange(this, EventArgs.Empty);
            }
        }
        private IProcessorArchitecture arch;

        public bool RenderInstructionsCanonically
        {
            get { return dasmModel is not null && dasmModel.RenderInstructionsCanonically; }
            set
            {
                if (dasmModel is null)
                    return;
                dasmModel.RenderInstructionsCanonically = value;
                Invalidate();
            }
        }

        public bool ShowPcRelative
        {
            get { return dasmModel is not null ? dasmModel.ShowPcRelative : false; }
            set
            {
                if (dasmModel is null)
                    return;
                dasmModel.ShowPcRelative = value;
                Invalidate();
                ShowPcRelativeChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler ShowPcRelativeChanged;

        void DisassemblyControl_StateChange(object sender, EventArgs e)
        {
            if (program is null || topAddress is null)
            {
                Model = new EmptyEditorModel();
            }
            else
            {
                if (!program.SegmentMap.TryFindSegment(topAddress.Value, out ImageSegment segment))
                {
                    Model = new EmptyEditorModel();
                }
                else
                {
                    var addr = topAddress;
                    this.dasmModel = new DisassemblyTextModel(factory, program, this.Architecture, segment);
                    Model = dasmModel;
                    dasmModel.MoveToAddress(addr.Value);
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
            topAddress = Model.CurrentPosition as Address?;
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

}
