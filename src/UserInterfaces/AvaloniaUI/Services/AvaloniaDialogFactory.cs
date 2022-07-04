#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Machine;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using Reko.UserInterfaces.AvaloniaUI.ViewModels;
using Reko.UserInterfaces.AvaloniaUI.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaDialogFactory : IDialogFactory
    {
        private readonly IServiceContainer services;

        public AvaloniaDialogFactory(IServiceContainer services)
        {
            this.services = services;
        }

        public IAboutDialog CreateAboutDialog()
        {
            throw new NotImplementedException();
        }

        public IAddressPromptDialog CreateAddressPromptDialog()
        {
            throw new NotImplementedException();
        }

        public IArchiveBrowserDialog CreateArchiveBrowserDialog()
        {
            throw new NotImplementedException();
        }

        public IAssembleFileDialog CreateAssembleFileDialog()
        {
            throw new NotImplementedException();
        }

        public IAssumedRegisterValuesDialog CreateAssumedRegisterValuesDialog(IProcessorArchitecture arch)
        {
            throw new NotImplementedException();
        }

        public IBlockNameDialog CreateBlockNameDialog(Procedure proc, Block block)
        {
            throw new NotImplementedException();
        }

        public ICallSiteDialog CreateCallSiteDialog(Program program, UserCallData ucd)
        {
            throw new NotImplementedException();
        }

        public IDeclarationForm CreateCommentForm()
        {
            throw new NotImplementedException();
        }

        public IDeclarationForm CreateDeclarationForm()
        {
            throw new NotImplementedException();
        }

        public IFindStringsDialog CreateFindStringDialog()
        {
            throw new NotImplementedException();
        }

        public IJumpTableDialog CreateJumpTableDialog(Program program, IProcessorArchitecture arch, MachineInstruction instrIndirectJmp, Address addrVector, int stride)
        {
            throw new NotImplementedException();
        }

        public IKeyBindingsDialog CreateKeyBindingsDialog(Dictionary<string, Dictionary<int, CommandID>> keyBindings)
        {
            throw new NotImplementedException();
        }

        public IOpenAsDialog CreateOpenAsDialog(string initialFilename)
        {
            throw new NotImplementedException();
        }

        public IDialog<UserProcedure?> CreateProcedureDialog(Program program, UserProcedure proc)
        {
            var procDlgModel = new ProcedureDialogModel(program, proc);
            var procDlg = new ProcedureDialog()
            {
                DataContext = procDlgModel
            };
            return procDlg;
        }

        public IProgramPropertiesDialog CreateProgramPropertiesDialog(Program program)
        {
            throw new NotImplementedException();
        }

        public IRegisterValuesDialog CreateRegisterValuesDialog(IProcessorArchitecture architecture, List<UserRegisterValue> regValues)
        {
            throw new NotImplementedException();
        }

        public IResourceEditor CreateResourceEditor()
        {
            throw new NotImplementedException();
        }

        public ISearchDialog CreateSearchDialog()
        {
            throw new NotImplementedException();
        }

        public ISegmentEditorDialog CreateSegmentEditorDialog()
        {
            throw new NotImplementedException();
        }

        public ISelectItemDialog CreateSelectItemDialog(string caption, object[] items, bool multiSelect)
        {
            throw new NotImplementedException();
        }

        public ISymbolSourceDialog CreateSymbolSourceDialog()
        {
            throw new NotImplementedException();
        }

        public ITextEncodingDialog CreateTextEncodingDialog()
        {
            throw new NotImplementedException();
        }

        public IUserPreferencesDialog CreateUserPreferencesDialog()
        {
            throw new NotImplementedException();
        }

        public IWorkerDialog CreateWorkerDialog()
        {
            throw new NotImplementedException();
        }

        public IDiagnosticFilterDialog CreateDiagnosticFilterDialog(DiagnosticFilters filters)
        {
            throw new NotImplementedException();
        }
    }
}
