#region License
/* 
* Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using Reko.Loading;
using System;
using System.ComponentModel.Design;

namespace Reko.Gui
{
    /// <summary>
    /// Decouples the creation of services, so that proper unit testing can be done.
    /// </summary>
    public interface IServiceFactory
    {
        DecompilerEventListener CreateDecompilerEventListener();
        IArchiveBrowserService CreateArchiveBrowserService();
        ICodeViewerService CreateCodeViewerService();
        IConfigurationService CreateDecompilerConfiguration();
        IDecompilerService CreateDecompilerService();
        IDiagnosticsService CreateDiagnosticsService();
        IDisassemblyViewService CreateDisassemblyViewService();
        IFileSystemService CreateFileSystemService();
        ImageSegmentService CreateImageSegmentService();

        InitialPageInteractor CreateInitialPageInteractor();
        IScannedPageInteractor CreateScannedPageInteractor();
        IAnalyzedPageInteractor CreateAnalyzedPageInteractor();
        IFinalPageInteractor CreateFinalPageInteractor();
        ILowLevelViewService CreateMemoryViewService();
        IProjectBrowserService CreateProjectBrowserService();
        ISearchResultService CreateSearchResultService();
        IResourceEditorService CreateResourceEditorService();
        ITabControlHostService CreateTabControlHost();
        ITypeLibraryLoaderService CreateTypeLibraryLoaderService();
        IUiPreferencesService CreateUiPreferencesService();
        ILoader CreateLoader();
        ICallGraphViewService CreateCallGraphViewService();
        IStatusBarService CreateStatusBarService();
        IViewImportsService CreateViewImportService();
        ISymbolLoadingService CreateSymbolLoadingService();
        ISelectionService CreateSelectionService();
        IProcedureListService CreateProcedureListService();
        ICallHierarchyService CreateCallHierarchyService();
        IDecompiledFileService CreateDecompiledFileService();
    }
}
