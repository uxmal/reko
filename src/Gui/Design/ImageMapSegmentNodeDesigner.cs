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
using Reko.Core.Collections;
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Gui.Services;
using System;
using System.Linq;
using System.Text;

namespace Reko.Gui.Design
{
    public class ImageMapSegmentNodeDesigner : TreeNodeDesigner
    {
        private ImageSegment? segment;

        public override void Initialize(object obj)
        {
            this.segment = (ImageSegment) obj;
            base.TreeNode!.Text = segment.Name;
            base.TreeNode.ImageName = GetImageName();
            base.TreeNode.ToolTipText = GetTooltip();
            PopulateChildren();
        }

        public string GetImageName()
        {
            if (segment!.IsDiscardable)
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
            sb.AppendLine(segment!.Name);
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
            var segment = this.segment!;
            var program = GetProgram();
            if (program is null)
                return;
            var eps = program.EntryPoints.Keys.ToHashSet();
            var globals = GetGlobalVariables(program.ImageMap, segment);
            var desDictionary =
                globals.Concat(
                (from proc in program.Procedures.Where(p => segment.IsInRange(p.Key))
                 join up in program.User.Procedures on proc.Key.ToLinear() equals up.Key.ToLinear() into ups
                 from up in ups.DefaultIfEmpty()
                 select new ProcedureDesigner(program, proc.Value, up.Value, proc.Key, eps.Contains(proc.Key))).
                Union(
                from up in program.User.Procedures.Where(p => segment.IsInRange(p.Key))
                join proc in program.Procedures on up.Key.ToLinear() equals proc.Key.ToLinear() into ups
                from proc in ups.DefaultIfEmpty()
                select new ProcedureDesigner(program, proc.Value, up.Value, up.Key, eps.Contains(up.Key))).
                OrderBy(pd => pd.Address));
            Host!.AddComponents(Component!, desDictionary);
        }

        private TreeNodeDesigner[] GetGlobalVariables(ImageMap imageMap, ImageSegment segment)
        {
            if (imageMap.Items.Values
                .Where(i => !(i is ImageMapBlock) && 
                            !(i.DataType is UnknownType))
                .Any(i => segment.IsInRange(i.Address)))
            {
                return new TreeNodeDesigner[]
                {
                    new GlobalVariablesNodeDesigner(segment)
                };
            }
            else
            {
                return Array.Empty<TreeNodeDesigner>();
            }
        }

        private Program? GetProgram()
        {
            if (this.Parent is null)
                return null;
            return this.Parent.Component as Program;
        }

        public override void DoDefaultAction()
        {
            if (segment!.Designer is not null)
            {
                var segSvc = Services!.RequireService<ImageSegmentService>();
                segSvc.DisplayImageSegment(segment, GetProgram());
            }
            else
            {
                var memSvc = Services!.RequireService<ILowLevelViewService>();
                memSvc.ShowMemoryAtAddress(GetProgram(), segment.Address);
            }
        }
    }
}
