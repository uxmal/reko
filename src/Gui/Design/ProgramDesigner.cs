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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Design
{
    public class ProgramDesigner : TreeNodeDesigner
    {
        private Program? program;

        public override void Initialize(object obj)
        {
            base.Initialize(obj);
            program = (Program) obj;
            var archesDes = new TreeNodeCollectionDesigner(
                "Architectures",
                "",
                ArchitectureCollection(program));
            Host!.AddComponent(program, archesDes);
            if (program.Platform is not null)
                Host.AddComponent(program, program.Platform);
            if (program.ImageMap is not null)
                Host.AddComponents(program, program.SegmentMap.Segments.Values);
            else if (!program.NeedsScanning)
            {
                Host.AddComponents(program, program.Procedures.Select(MakeProcedureDesigner));
            }
            if (program.ImportReferences.Count > 0)
            {
                var des = new ImportDesigner(program);
                Host.AddComponent(program, des);
            }
            if (program.Resources.Count > 0)
            {
                var r = new TreeNodeCollectionDesigner("Resources", "", program.Resources);
                Host.AddComponent(program, r);
            }
            SetTreeNodeProperties(program);
        }

        private IEnumerable ArchitectureCollection(Program program)
        {
            return program.Architectures.Values.OrderBy(a => a.Name);
        }

        private ProcedureDesigner MakeProcedureDesigner(KeyValuePair<Address,Procedure> p)
        {
            var des = new ProcedureDesigner(program!, p.Value, null, p.Key, false);
            return des;
        }

        public void SetTreeNodeProperties(Program program)
        {
            TreeNode!.Text = program.Name;
            TreeNode.ImageName = "Binary.ico";
            var sb = new StringBuilder();
            if (program.Location is null || string.IsNullOrEmpty(program.Location.FilesystemPath))
            {
                sb.Append("No file name");
            }
            else
            {
                sb.Append(program.Location);
            }
            if (program.NeedsScanning)
            {
                sb.AppendLine();
                sb.Append(program.ImageMap.BaseAddress.ToString());
            }
            TreeNode.ToolTipText = sb.ToString();
        }

        public override bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch ((CmdIds)cmdId.ID)
                {
                case CmdIds.EditProperties:
                case CmdIds.LoadSymbols:
                    status.Status = MenuStatus.Enabled|MenuStatus.Visible;
                    return true;
                }
            }
            return base.QueryStatus(cmdId, status, text);
        }

        public async override ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch ((CmdIds)cmdId.ID)
                {
                case CmdIds.EditProperties:
                    var dlgFactory = Services!.RequireService<IDialogFactory>();
                    var dlg = dlgFactory.CreateProgramPropertiesDialog((Program)this.Component!);
                    var uiSvc = Services!.RequireService<IDecompilerShellUiService>();
                    await uiSvc.ShowModalDialog(dlg);
                    return true;
                case CmdIds.LoadSymbols:
                    return await LoadSymbols();
                }
            }
            return await base.ExecuteAsync(cmdId);
        }

        private async ValueTask<bool> LoadSymbols()
        {
            var uiSvc = Services!.RequireService<IDecompilerShellUiService>();
            var filename = await uiSvc.ShowOpenFileDialog("");
            if (filename is null)
            {
                // user canceled.
                return true;
            }

            var symService = Services!.RequireService<ISymbolLoadingService>();
            var symSource = symService.GetSymbolSource(filename);
            var symbols = symSource?.GetAllSymbols();

            return true;
        }
    }
}
