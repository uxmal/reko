#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Procedure_v1 = Decompiler.Core.Serialization.Procedure_v1;

namespace Decompiler.Gui.Design
{
    public class ProcedureDesigner : TreeNodeDesigner
    {
        private Procedure procedure;
        private string name;
        private Procedure_v1 procedure_v1;

        public ProcedureDesigner(Procedure procedure, Address address)
        {
            this.procedure = procedure;
            this.Address = address;
            this.name = procedure.Name;
        }

        public ProcedureDesigner(Procedure_v1 procedure_v1, Core.Address address)
        {
            this.name = procedure_v1.Name ?? name;
            this.procedure_v1 = procedure_v1;
            this.Address = address;
        }

        public Address Address { get; set; }

        public override void Initialize(object obj)
        {
            SetTreeNodeText();
        }

        private void SetTreeNodeText()
        {
            TreeNode.Text = name;
            TreeNode.ToolTipText = Address.ToString();
            TreeNode.ImageName = procedure_v1 != null ? "Userproc.ico" : "Procedure.ico";
        }

        public override void DoDefaultAction()
        {
            Services.RequireService<ICodeViewerService>().DisplayProcedure(procedure);
        }
    }
}
