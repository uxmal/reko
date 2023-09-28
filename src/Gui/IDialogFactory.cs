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
using Reko.Gui.Forms;
using Reko.Gui.Services;
using Reko.Gui.ViewModels.Dialogs;
using Reko.Scanning;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.Gui
{
    /// <summary>
    /// This interface abstracts the creation of dialog windows between
    /// different GUI environments.
    /// </summary>
    public interface IDialogFactory
    {
        IAboutDialog CreateAboutDialog();
        IArchiveBrowserDialog CreateArchiveBrowserDialog(ICollection<ArchiveDirectoryEntry> archiveEntries);
        IAssembleFileDialog CreateAssembleFileDialog();
        IAssumedRegisterValuesDialog CreateAssumedRegisterValuesDialog(IProcessorArchitecture arch);
        IAddressPromptDialog CreateAddressPromptDialog();
        ICallSiteDialog CreateCallSiteDialog(Program program, UserCallData ucd);
        IDialog<StringFinderCriteria?> CreateFindStringDialog(Program program);
        IKeyBindingsDialog CreateKeyBindingsDialog(Dictionary<string, Dictionary<int, CommandID>> keyBindings);
        IDialog<LoadDetails?> CreateOpenAsDialog(string initialFilename);
        IDialog<UserProcedure?> CreateProcedureDialog(Program program, UserProcedure proc);
        IProgramPropertiesDialog CreateProgramPropertiesDialog(Program program);
        IResourceEditor CreateResourceEditor();
        ISearchDialog CreateSearchDialog();
        IUserPreferencesDialog CreateUserPreferencesDialog();
        IWorkerDialog CreateWorkerDialog();
        ITextEncodingDialog CreateTextEncodingDialog();
        IDeclarationForm CreateDeclarationForm();
        IDeclarationForm CreateCommentForm();
        IJumpTableDialog CreateJumpTableDialog(Program program, IProcessorArchitecture arch, MachineInstruction instrIndirectJmp, Address addrVector, int stride);
        ISymbolSourceDialog CreateSymbolSourceDialog();
        ISelectItemDialog CreateSelectItemDialog(string caption, object[] items, bool multiSelect);
        ISegmentEditorDialog CreateSegmentEditorDialog();
        IRegisterValuesDialog CreateRegisterValuesDialog(IProcessorArchitecture architecture, List<UserRegisterValue> regValues);
        IBlockNameDialog CreateBlockNameDialog(Procedure proc, Block block);
        IDiagnosticFilterDialog CreateDiagnosticFilterDialog(DiagnosticFilters filterSettings);
        IDialog<SearchArea?> CreateSearchAreaDialog(Program program, SearchArea searchArea);
    }
}
