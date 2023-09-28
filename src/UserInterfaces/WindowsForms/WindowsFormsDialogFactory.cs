#region License
/* 
* Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using Reko.Gui.ViewModels.Dialogs;
using Reko.Scanning;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    public class WindowsFormsDialogFactory : IDialogFactory
	{
        private readonly IServiceProvider services;
        private readonly Form syncForm;  // used _only_ to make sure forms are created on the UI thread for Mono compatibility.

		public WindowsFormsDialogFactory(IServiceProvider services)
		{
            this.services = services;
            // Note: this constructor must run on the main, UI thread under Mono, as the FAQ states:
            // http://www.mono-project.com/docs/faq/winforms/
            // Mono’s implementation of WinForms does not support Forms or Controls being created on multiple threads. 
            // All Forms/Controls must be created on the same thread.
            this.syncForm = new Form();     // this form is never shown.
            this.syncForm.Visible = false;
            this.syncForm.CreateControl();
        }

        public IAboutDialog CreateAboutDialog()
        {
            return new AboutDialog();
        }

        public IAddressPromptDialog CreateAddressPromptDialog()
		{
			return new AddressPromptDialog();
		}

        public IArchiveBrowserDialog CreateArchiveBrowserDialog(ICollection<ArchiveDirectoryEntry> archiveEntries)
        {
            return new ArchiveBrowserDialog()
            {
                ArchiveEntries = archiveEntries
            };
        }

        public IAssembleFileDialog CreateAssembleFileDialog()
        {
            return new AssembleFileDialog
            {
                Services = services
            };
        }

        public IAssumedRegisterValuesDialog CreateAssumedRegisterValuesDialog(
            IProcessorArchitecture arch)
        {
            return new AssumedRegisterValuesDialog
            {
                Architecture = arch,
            };
        }

        public ICallSiteDialog CreateCallSiteDialog(Program program, UserCallData ucd)
        {
            Debug.Assert(ucd != null && ucd.Address != null);
            var dlg = new CallSiteDialog();
            dlg.Address = ucd.Address;
            dlg.NoReturn.Checked = ucd.NoReturn;
            return dlg;
        }

        public IDialog<StringFinderCriteria> CreateFindStringDialog(Program program)
        {
            var uiSvc = services.RequireService<IDecompilerShellUiService>();
            var settingsSvc = services.RequireService<ISettingsService>();
            var dlgFactory = services.RequireService<IDialogFactory>();
            return new FindStringsDialog
            {
                DataContext = new FindStringsDialogInteractor(program, uiSvc, settingsSvc, dlgFactory),
            };
        }

        public IKeyBindingsDialog CreateKeyBindingsDialog(Dictionary<string, Dictionary<int, CommandID>> keyBindings)
        {
            var dlg = new KeyBindingsDialog();
            dlg.KeyBindings = keyBindings;
            return dlg;
        }

        public IDialog<LoadDetails> CreateOpenAsDialog(string initialFilename)
        {
            var dlg = new OpenAsDialog
            {
                Services = services,
                ArchitectureOptions = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            };
            dlg.FileName.Text = initialFilename;
            return dlg;
        }

        public IDialog<UserProcedure> CreateProcedureDialog(Program program, Core.UserProcedure proc)
        {
            var i = new ProcedureDialogInteractor(program, proc);
            return i.CreateDialog();
        }

        public IProgramPropertiesDialog CreateProgramPropertiesDialog(Program program)
        {
            return new ProgramPropertiesDialog
            {
                Services = services,
                Program = program,
            };
        }

        public IResourceEditor CreateResourceEditor()
        {
            return new ResourceEditor();
        }

        public ISearchDialog CreateSearchDialog()
        {
            return new SearchDialog()
            {
				Services = services,
            };
        }

        public IUserPreferencesDialog CreateUserPreferencesDialog()
        {
            return new UserPreferencesDialog
            {
                Services = services,
            };
        }

        public IWorkerDialog CreateWorkerDialog()
        {
            if (syncForm.InvokeRequired)
            {
                IWorkerDialog dlg = null;
                syncForm.Invoke(new Action(() =>
                {
                    dlg = new WorkerDialog();
                }));
                return dlg;
            }
            else
            { 
                return new WorkerDialog();
            }
        }

        public ITextEncodingDialog CreateTextEncodingDialog()
        {
            return new TextEncodingDialog();
        }

        public IDeclarationForm CreateDeclarationForm()
        {
            return new DeclarationForm();
        }

        public IDeclarationForm CreateCommentForm()
        {
            return new CommentForm();
        }

        public IJumpTableDialog CreateJumpTableDialog(Program program, IProcessorArchitecture arch,  MachineInstruction instrIndirectJmp, Address addrVector, int stride)
        {
            return new JumpTableDialog()
            {
                Services = this.services,
                Program = program,
                Architecture = arch,
                Instruction = instrIndirectJmp,
                VectorAddress = addrVector,
                Stride = stride
            };
        }

        public ISymbolSourceDialog CreateSymbolSourceDialog()
        {
            return new SymbolSourceDialog
            {
                Services = this.services
            };
        }

        public ISelectItemDialog CreateSelectItemDialog(string caption, object[] items, bool multiSelect)
        {
            return new SelectItemDialog
            {
                Text = caption,
                Items = items,
                MultiSelect = multiSelect
            };
        }

        public ISegmentEditorDialog CreateSegmentEditorDialog()
        {
            var dlg = new SegmentEditorDialog
            {
                Services = services,
            };
            return dlg;
        }

        public IRegisterValuesDialog CreateRegisterValuesDialog(IProcessorArchitecture arch, List<UserRegisterValue> regValues)
        {
            var dlg = new RegisterValuesDialog
            {
                Architecture = arch,
                RegisterValues = regValues,
            };
            return dlg;
        }

        public IBlockNameDialog CreateBlockNameDialog(Procedure proc, Block block)
        {
            var dlg = new BlockNameDialog(proc, block);
            return dlg;
        }

        public IDiagnosticFilterDialog CreateDiagnosticFilterDialog(DiagnosticFilters filter)
        {
            var svc = this.services.RequireService<ISettingsService>();
            var dlg = new DiagnosticFilterDialog
            {
                Value  = filter,
            };
            return dlg;
        }

        public IDialog<SearchArea> CreateSearchAreaDialog(Program program, SearchArea searchArea)
        {
            var uiPreferencesSvc = services.RequireService<IUiPreferencesService>();
            return new SearchAreaDialog
            {
                DataContext = new SearchAreaViewModel(program, searchArea, uiPreferencesSvc)
            };
        }
    }
}

