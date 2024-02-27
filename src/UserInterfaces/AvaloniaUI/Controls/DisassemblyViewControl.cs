#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Avalonia;
using Avalonia.Input;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Gui.TextViewing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Controls
{
    public class DisassemblyViewControl : TextView
    {
        //$TODO Many of these properties belong to the DisassemblyTextModel

        public static readonly DirectProperty<DisassemblyViewControl, Address?> SelectedAddressProperty =
            AvaloniaProperty.RegisterDirect<DisassemblyViewControl, Address?>(
                nameof(SelectedAddress),
                o => o.SelectedAddress,
                (o, v) => o.SelectedAddress = v);

        public static readonly DirectProperty<DisassemblyViewControl, Program?> ProgramProperty =
            AvaloniaProperty.RegisterDirect<DisassemblyViewControl, Program?>(
                nameof(SelectedAddress),
                o => o.Program,
                (o, v) => o.Program = v);

        public static readonly DirectProperty<DisassemblyViewControl, IProcessorArchitecture?> ArchitectureProperty =
            AvaloniaProperty.RegisterDirect<DisassemblyViewControl, IProcessorArchitecture?>(
                nameof(Architecture),
                o => o.Architecture,
                (o, v) => o.Architecture = v);

        public DisassemblyTextModel? ViewModel { get => (DisassemblyTextModel?) this.DataContext; }


        public Program? Program {
            get { return program; }
            set {
                if (this.program == value)
                    return;
                this.SetAndRaise(ProgramProperty, ref program, value);
                ProgramChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler? ProgramChanged;
        private Program? program;

        public Address? SelectedAddress
        {
            get { return selectedAddress; }
            set
            {
                if (this.selectedAddress == value)
                    return;
                this.SetAndRaise(SelectedAddressProperty, ref this.selectedAddress, value);
                StartAddressChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler? StartAddressChanged;
        private Address? selectedAddress;

        public Address? TopAddress
        {
            get { return topAddress; }
            set
            {
                topAddress = value; TopAddressChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler? TopAddressChanged;
        private Address? topAddress;

        public object? SelectedObject
        {
            get { return selectedObject; }
            set
            {
                selectedObject = value;
                InvalidateVisual();
                SelectedObjectChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler? SelectedObjectChanged;
        private object? selectedObject;

        /// <summary>
        /// Architecture to use when displaying disassembly. Can be used
        /// to override the architecture from the <see cref="Program"/>
        /// property.
        /// </summary>
        public IProcessorArchitecture? Architecture
        {
            get { return this.arch; }
            set
            {
                if (this.arch == value)
                    return;
                this.arch = value;
                DisassemblyControl_StateChange(this, EventArgs.Empty);
            }
        }
        private IProcessorArchitecture? arch;

        public bool RenderInstructionsCanonically
        {
            get { return this.ViewModel != null && this.ViewModel.RenderInstructionsCanonically; }
            set
            {
                if (this.ViewModel is null)
                    return;
                this.ViewModel.RenderInstructionsCanonically = value;
                InvalidateVisual();
            }
        }

        public bool ShowPcRelative
        {
            get { return this.ViewModel != null ? this.ViewModel.ShowPcRelative : false; }
            set
            {
                if (this.ViewModel is null)
                    return;
                this.ViewModel.ShowPcRelative = value;
                InvalidateVisual();
                ShowPcRelativeChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler? ShowPcRelativeChanged;

        void DisassemblyControl_StateChange(object sender, EventArgs e)
        {
            if (program == null || topAddress == null)
            {
                Model = new EmptyEditorModel();
            }
            else
            {
                if (!program.SegmentMap.TryFindSegment(topAddress, out ImageSegment? segment))
                {
                    Model = new EmptyEditorModel();
                }
                else
                {
                    //var addr = topAddress;
                    //this.ViewModel = new DisassemblyTextModel(factory, program, this.Architecture, segment);
                    //Model = dasmModel;
                    //dasmModel.MoveToAddress(addr);
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

        /*
         * Maybe Windows Forms-specific
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
        */

        protected override void OnKeyDown(KeyEventArgs e)
        {
            //Debug.Print("Disassembly control: Down:  c:{0} d:{1} v:{2}", e.KeyCode, e.KeyData, e.KeyValue);
            switch (e.Key)
            {
            case Key.Down:
                Model.MoveToLine(Model.CurrentPosition, 1);
                break;
            case Key.Up:
                Model.MoveToLine(Model.CurrentPosition, -1);
                break;
            default:
                base.OnKeyDown(e);
                return;
            }
            e.Handled = true;
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            Focus();
            var curPoint = e.GetCurrentPoint(this);
            var obj = GetTagFromPoint(curPoint.Position);
            if (curPoint.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
                SelectedObject = obj;       // Fire a notification only if left-click.
            else
                selectedObject = obj;
            base.OnPointerPressed(e);
        }
    }
}
