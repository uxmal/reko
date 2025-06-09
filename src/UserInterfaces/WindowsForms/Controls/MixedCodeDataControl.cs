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
using Reko.Core.Services;
using Reko.Gui.Services;
using Reko.Gui.TextViewing;
using System;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    /// <summary>
    /// Renders code and data side by side.
    /// </summary>
    public class MixedCodeDataControl : TextView
    {
  
        public MixedCodeDataControl()
        {
            this.ProgramChanged += delegate { OnPropertyChanged(); };

            OnPropertyChanged();

            this.Disposed += MixedCodeDataControl_Disposed;
       }

        public Program Program
        {
            get { return program; }
            set
            {
                if (program is not null)
                {
                    program.User.Annotations
                        .AnnotationChanged -= AnnotationChanged;
                }
                program = value;
                if (program is not null)
                {
                    program.User.Annotations
                        .AnnotationChanged += AnnotationChanged;
                }
                ProgramChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private Program program;
        public event EventHandler ProgramChanged;

        public Address? TopAddress { get { return addrTop; } set { addrTop = value; OnTopAddressChanged(); } }
        private Address? addrTop;

 
        private void OnPropertyChanged()
        {
            try
            {
                if (program is not null && Services is not null)
                {
                    var selSvc = Services.RequireService<ISelectedAddressService>();
                    Model = new MixedCodeDataModel(
                        program, 
                        program.ImageMap.Clone(),
                        new WindowsFormsTextSpanFactory(),
                        selSvc);
                    var currentPos = Model.CurrentPosition;
                    addrTop = MixedCodeDataModel.PositionAddress(currentPos);
                    return;
                }
            }
            catch (Exception ex)
            {
                Services.RequireService<IDiagnosticsService>().Error(ex, "An error occurred while displaying the program.");
            }
            Model = new EmptyEditorModel();
            addrTop = null;
        }

        protected override void OnServicesChanged()
        {
            OnPropertyChanged();
            base.OnServicesChanged();
        }

        private void OnTopAddressChanged()
        {
            if (program is not null)
            {
                var addrTopPos = MixedCodeDataModel.Position(addrTop.Value, 0);
                Model.MoveToLine(addrTopPos, 0);
                RecomputeLayout();
                UpdateScrollbar();
                Invalidate();
            }
        }

        protected override void OnScroll()
        {
            if (Model is MixedCodeDataModel)
            {
                var currentPos = Model.CurrentPosition;
                addrTop = MixedCodeDataModel.PositionAddress(currentPos);
            }
            else
            {
                addrTop = null;
            }
            base.OnScroll();
        }

        private void RefreshModel()
        {
            var currentPos = Model.CurrentPosition;
            var selSvc = Services.RequireService<ISelectedAddressService>();
            var model = new MixedCodeDataModel(program, program.ImageMap.Clone(), new WindowsFormsTextSpanFactory(),  selSvc);
            model.MoveToLine(currentPos, 0);
            currentPos = Model.CurrentPosition;
            this.addrTop = MixedCodeDataModel.PositionAddress(currentPos);
            this.Model = model;
        }

        private void AnnotationChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new Action(RefreshModel));
            else
                RefreshModel();
        }

        private void MixedCodeDataControl_Disposed(object sender, EventArgs e)
        {
        }

        public Address GetAnchorAddress()
        {
            var pt = GetAnchorMiddlePoint();
            var memoryTextSpan = GetTagFromPoint(pt) as WindowsFormsTextSpanFactory.MemoryTextSpan;
            if (memoryTextSpan is null)
                return MixedCodeDataModel.PositionAddress(anchorPos.Line);
            return memoryTextSpan.Address;
        }
    }
}
