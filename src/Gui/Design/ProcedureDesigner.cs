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
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;

namespace Reko.Gui.Design
{
    public class ProcedureDesigner : TreeNodeDesigner, IEquatable<ProcedureDesigner>
    {
        private Program program;
        private Procedure procedure;
        private string? name;
        private UserProcedure? userProc;
        private bool isEntryPoint;

        public ProcedureDesigner(
            Program program,
            Procedure procedure,
            UserProcedure? userProc,
            Address address,
            bool isEntryPoint)
        {
            base.Component = procedure;
            this.program = program;
            this.procedure = procedure;
            this.userProc = userProc;
            this.Address = address;
            this.isEntryPoint = isEntryPoint;
            if (userProc is not null && !string.IsNullOrEmpty(userProc.Name))
                this.name = userProc.Name!;
            else if (procedure is not null)
                this.name = procedure.Name;
            if (procedure is not null)
                procedure.NameChanged += procedure_NameChanged;
        }

        public Address Address { get; set; }

        public override void Initialize(object obj)
        {
            base.Initialize(obj);
            base.Component = obj;
            SetTreeNodeText();
        }

        private void SetTreeNodeText()
        {
            if (TreeNode is null)
                return;
            TreeNode.Text = name ?? "(None)";
            TreeNode.ToolTipText = Address.ToString();
            TreeNode.ImageName = userProc is not null
                ? (isEntryPoint ? "UserEntryProcedure.ico" : "Userproc.ico")
                : (isEntryPoint ? "EntryProcedure.ico" : "Procedure.ico");
        }

        public override void DoDefaultAction()
        {
            Services!.RequireService<ICodeViewerService>().DisplayProcedure(program, procedure, program.NeedsScanning);
        }

        public override bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch ((CmdIds)cmdId.ID)
                {
                case CmdIds.ViewGoToAddress:
                case CmdIds.ActionEditSignature:
                case CmdIds.EditRename:
                    status.Status = MenuStatus.Visible | MenuStatus.Enabled;
                    return true;
                case CmdIds.ViewFindWhatPointsHere:
                case CmdIds.ShowCallGraphNavigator:
                    status.Status = MenuStatus.Visible;
                    if (procedure is not null)
                        status.Status |= MenuStatus.Enabled;
                    return true;
                case CmdIds.ActionAssumeRegisterValues:
                    status.Status = MenuStatus.Visible | MenuStatus.Enabled;
                    return true;
                }
            }
            return false;
        }

        public async override ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            bool result = true;
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch ((CmdIds)cmdId.ID)
                {
                case CmdIds.ViewGoToAddress:
                    Services!.RequireService<ILowLevelViewService>().ShowMemoryAtAddress(program, Address);
                    break;
                case CmdIds.ActionEditSignature:
                    await EditSignature();
                    break;
                case CmdIds.ShowCallGraphNavigator:
                    Services!.RequireService<ICallGraphNavigatorService>().Show(program, procedure);
                    break;
                case CmdIds.ActionAssumeRegisterValues:
                    await AssumeRegisterValues();
                    break;
                case CmdIds.ViewFindWhatPointsHere:
                    ViewWhatPointsHere();
                    break;
                case CmdIds.EditRename:
                    Rename();
                    break;
                default:
                    result = false;
                    break;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        private ValueTask EditSignature()
        {
            return Services!.RequireService<ICommandFactory>().EditSignature(program, procedure, Address).DoAsync();
        }

        private void ViewWhatPointsHere()
        {
            var resultSvc = Services!.GetService<ISearchResultService>();
            if (resultSvc is null)
                return;
            var arch = procedure.Architecture;
            if (!program.TryCreateImageReader(arch, program.ImageMap.BaseAddress, out var rdr))
                return;
            var addrControl = arch.CreatePointerScanner(
                program.SegmentMap,
                rdr,
                new Address[]  {
                    this.Address,
                },
                PointerScannerFlags.All);
            resultSvc.ShowAddressSearchResults(
                addrControl.Select(a => new AddressSearchHit(program, a, 1)),
                new CodeSearchDetails());
        }

        private void Rename()
        {
        }

        private async ValueTask AssumeRegisterValues()
        {
            var dlgFactory = Services!.RequireService<IDialogFactory>();
            var uiSvc = Services!.RequireService<IDecompilerShellUiService>();
            using (var dlg = dlgFactory.CreateAssumedRegisterValuesDialog(program.Architecture))
            {
                dlg.Values = GetAssumedRegisterValues(Address);
                if (await uiSvc.ShowModalDialog(dlg) != DialogResult.OK)
                    return;
                SetAssumedRegisterValues(Address, dlg.Values);
                SetTreeNodeText();
            }
        }

        private Dictionary<RegisterStorage, string> GetAssumedRegisterValues(Address Address)
        {
            if (!program.User.Procedures.TryGetValue(this.Address, out var up))
                return new Dictionary<RegisterStorage, string>();

            return up.Assume
                .Select(ass => new
                {
                    Register = program.Architecture.GetRegister(ass.Register!),
                    Value = ass.Value
                })
                .Where(ass => ass.Register is not null)
                .ToDictionary(ass => ass.Register!, ass => ass.Value!);
        }

        private void SetAssumedRegisterValues(Address Address, Dictionary<RegisterStorage, string> dictionary)
        {
            userProc = program.EnsureUserProcedure(
                this.Address,
                procedure is not null
                    ? procedure.Name
                    : userProc?.Name);
            userProc.Assume = dictionary
                .Select(de => new Core.Serialization.RegisterValue_v2
                {
                    Register = de.Key.Name,
                    Value = de.Value
                })
                .ToList();
        }

        public override int GetHashCode()
        {
            return Address.ToLinear().GetHashCode();
        }

        public bool Equals(ProcedureDesigner? other)
        {
            return other is not null && Address.ToLinear() == other.Address.ToLinear();
        }

        void procedure_NameChanged(object? sender, EventArgs e)
        {
            if (TreeNode is not null)
            {
                TreeNode.Invoke(OnNameChanged);
            }
            else
            {
                OnNameChanged();
            }
        }

        private void OnNameChanged()
        {
            userProc = program.EnsureUserProcedure(Address, procedure.Name);
            userProc.Name = procedure.Name;
            this.name = procedure.Name;
            SetTreeNodeText();
        }
    }
}
