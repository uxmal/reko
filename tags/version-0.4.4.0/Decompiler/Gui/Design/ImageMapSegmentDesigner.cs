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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Gui.Design
{
    public class ImageMapSegmentDesigner : TreeNodeDesigner
    {
        private ImageMapSegment segment;

        public override void Initialize(object obj)
        {
            this.segment = (ImageMapSegment) obj;
            base.TreeNode.Text = segment.Name;
            base.TreeNode.ImageName = GetImageName();
            base.TreeNode.ToolTipText = GetTooltip();
            PopulateChildren();
        }

        public string GetImageName()
        {
            if (segment.IsDiscardable)
                return "DiscardableSection.ico";
            switch (segment.Access)
            {
            case AccessMode.ReadWriteExecute:
                return "WxSection.ico";
            case AccessMode.ReadExecute:
                return "RxSection.ico";
            case AccessMode.ReadWrite:
                return "RwSection.ico";
            default:
                return "RoSection.ico";
            }
        }

        public string GetTooltip()
        {
            var sb = new StringBuilder();
            sb.AppendLine(segment.Name);
            sb.AppendFormat("Address: {0}", segment.Address);
            sb.AppendLine();
            sb.AppendFormat("Size: {0:X}", segment.Size);
            sb.AppendLine();
            sb.AppendFormat("{0}{1}{2}", 
              (segment.Access & AccessMode.Read) != 0 ? 'r' : '-',
                 (segment.Access & AccessMode.Write) != 0 ? 'w' : '-',
                 (segment.Access & AccessMode.Execute) != 0 ? 'x' : '-'
                 );
            if (segment.IsDiscardable)
            {
                sb.AppendLine();
                sb.Append("Discardable");
            }
            return sb.ToString();
        }

        private void PopulateChildren()
        {
            var program = GetProgram();
            if (program == null)
                return;
            var desDictionary =
                (from proc in program.Procedures.Where(p => segment.IsInRange(p.Key))
                join up in program.UserProcedures on proc.Key.ToLinear() equals up.Key.ToLinear() into ups
                from up in ups.DefaultIfEmpty()
                select new ProcedureDesigner(program, proc.Value, up.Value, proc.Key)).
                Union(
                from up in program.UserProcedures.Where(p => segment.IsInRange(p.Key))
                join proc in program.Procedures on up.Key.ToLinear() equals proc.Key.ToLinear() into ups
                from proc in ups.DefaultIfEmpty()
                select new ProcedureDesigner(program, proc.Value, up.Value, up.Key)
                );
            Host.AddComponents(Component, desDictionary);
        }

        private Program GetProgram()
        {
            if (this.Parent == null)
                return null;
            return this.Parent.Component as Program;
        }

        public override void DoDefaultAction()
        {
            if (segment.Renderer != null)
            {
                var segSvc = Services.RequireService<ImageSegmentService>();
                segSvc.DisplayImageSegment(segment, GetProgram());
            }
            else
            {
                var memSvc = Services.RequireService<ILowLevelViewService>();
                memSvc.ShowMemoryAtAddress(GetProgram(), segment.Address);
            }
        }
    }
}
