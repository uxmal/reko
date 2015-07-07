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
using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.Gui.Design
{
    public class ProgramDesigner : TreeNodeDesigner
    {
        private Program program;

        public override void Initialize(object obj)
        {
            base.Initialize(obj);
            program = (Program) obj;
            if (program.ImageMap != null)
                Host.AddComponents(program, program.ImageMap.Segments.Values);

            SetTreeNodeProperties(program);
        }

        public void SetTreeNodeProperties(Program program)
        {
            TreeNode.Text = program.Name;
            TreeNode.ImageName = "Binary.ico";
            TreeNode.ToolTipText = string.Format("{0}{1}{2}",
                program.Filename != null ? program.Filename : "(No file name)",
                Environment.NewLine,
                program.Image.BaseAddress);
        }

        public override bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidDecompiler)
            {
                switch (cmdId.ID)
                {
                case CmdIds.EditProperties: 
                    status.Status = MenuStatus.Enabled|MenuStatus.Visible;
                    return true;
                }
            }
            return base.QueryStatus(cmdId, status, text);
        }

        public override bool Execute(CommandID cmdId)
        {
            if (cmdId.Guid == CmdSets.GuidDecompiler)
            {
                switch (cmdId.ID)
                {
                case CmdIds.EditProperties:
                    var dlgFactory = Services.RequireService<IDialogFactory>();
                    var dlg = dlgFactory.CreateProgramPropertiesDialog((Program)this.Component);
                    var uiSvc = Services.RequireService<IDecompilerShellUiService>();
                    uiSvc.ShowModalDialog(dlg);
                    return true;
                }
            }
            return base.Execute(cmdId);
        }
    }
}
