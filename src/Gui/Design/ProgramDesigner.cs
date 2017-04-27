#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.Gui.Design
{
    public class ProgramDesigner : TreeNodeDesigner
    {
        private Program program;

        public override void Initialize(object obj)
        {
            base.Initialize(obj);
            program = (Program) obj;
            if (program.Architecture != null)
                Host.AddComponent(program, program.Architecture);
            if (program.Platform != null)
                Host.AddComponent(program, program.Platform);
            if (program.ImageMap != null)
                Host.AddComponents(program, program.SegmentMap.Segments.Values);
            if (program.ImportReferences.Count > 0)
            {
                var des = new ImportDesigner(program);
                Host.AddComponent(program, des);
            }
            Host.AddComponent(program, program.Resources);
            SetTreeNodeProperties(program);
        }

        public void SetTreeNodeProperties(Program program)
        {
            TreeNode.Text = program.Name;
            TreeNode.ImageName = "Binary.ico";
            TreeNode.ToolTipText = string.Format("{0}{1}{2}",
                program.Filename != null ? program.Filename : "(No file name)",
                Environment.NewLine,
                program.ImageMap.BaseAddress);
        }

        public override bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.EditProperties:
                case CmdIds.LoadSymbols:
                    status.Status = MenuStatus.Enabled|MenuStatus.Visible;
                    return true;
                }
            }
            return base.QueryStatus(cmdId, status, text);
        }

        public override bool Execute(CommandID cmdId)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch (cmdId.ID)
                {
                case CmdIds.EditProperties:
                    var dlgFactory = Services.RequireService<IDialogFactory>();
                    var dlg = dlgFactory.CreateProgramPropertiesDialog((Program)this.Component);
                    var uiSvc = Services.RequireService<IDecompilerShellUiService>();
                    uiSvc.ShowModalDialog(dlg);
                    return true;
                case CmdIds.LoadSymbols:
                    return LoadSymbols();
                }
            }
            return base.Execute(cmdId);
        }

        private bool LoadSymbols()
        {
            var uiSvc = Services.RequireService<IDecompilerShellUiService>();
            var dlgFactory = Services.RequireService<IDialogFactory>();
            using (var dlg = dlgFactory.CreateSymbolSourceDialog())
            {
                if (uiSvc.ShowModalDialog(dlg) != DialogResult.OK)
                {
                    return true;
                }

                var symService = Services.RequireService<ISymbolLoadingService>();
                var ssRef = dlg.GetSymbolSource();
                var symSource = symService.GetSymbolSource(ssRef);
                if (symSource == null)
                    return true;
                var symbols = symSource.GetAllSymbols();
                program.AddSymbols(symbols);
            }
            return true;
        }
    }
}
