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

using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Gui
{
    public class CommandDefinitions
    {
        public static readonly Guid Reko = CmdSets.GuidReko;

        // Menus and Toolbars ==========================================================

        public readonly MenuDefinition[] Menus = new[]
        {
            new MenuDefinition{ type = MenuType.MainMenu, cmdSet = Reko, id = MenuIds.MainMenu },

            new MenuDefinition{ type = MenuType.Submenu, id=MenuIds.FileMenu    , cmdSet = Reko, container = GroupIds.GrpMain, Text = "_File" },
            new MenuDefinition{ type = MenuType.Submenu, id=MenuIds.EditMenu    , cmdSet = Reko, container = GroupIds.GrpMain, Text = "_Edit" },
            new MenuDefinition{ type = MenuType.Submenu, id=MenuIds.ViewMenu    , cmdSet = Reko, container = GroupIds.GrpMain, Text = "_View" },
            new MenuDefinition{ type = MenuType.Submenu, id=MenuIds.ActionMenu  , cmdSet = Reko, container = GroupIds.GrpMain, Text = "_Actions" },
            new MenuDefinition{ type = MenuType.Submenu, id=MenuIds.ToolsMenu   , cmdSet = Reko, container = GroupIds.GrpMain, Text = "_Tools" },
            new MenuDefinition{ type = MenuType.Submenu, id=MenuIds.WindowsMenu , cmdSet = Reko, container = GroupIds.GrpMain, Text = "_Windows" },
            new MenuDefinition{ type = MenuType.Submenu, id=MenuIds.HelpMenu,     cmdSet = Reko, container = GroupIds.GrpMain, Text = "_Help" },
            new MenuDefinition{ type = MenuType.Submenu, id=MenuIds.TextEncodingMenu, cmdSet = Reko, container = GroupIds.GrpLowLevel, Text = "_Text Encoding" },

            // Context menus
            new MenuDefinition { type = MenuType.ContextMenu, id = MenuIds.CtxMemoryControl, cmdSet = Reko },
            new MenuDefinition { type = MenuType.ContextMenu, id = MenuIds.CtxDisassembler, cmdSet = Reko },
            new MenuDefinition { type = MenuType.ContextMenu, id = MenuIds.CtxBrowser, cmdSet = Reko },
            new MenuDefinition { type = MenuType.ContextMenu, id = MenuIds.CtxProcedureList, cmdSet = Reko },
            new MenuDefinition { type = MenuType.ContextMenu, id = MenuIds.CtxProcedure, cmdSet = Reko },
            new MenuDefinition { type = MenuType.ContextMenu, id = MenuIds.CtxAddressSearch, cmdSet = Reko },
            new MenuDefinition { type = MenuType.ContextMenu, id = MenuIds.CtxCodeView, cmdSet = Reko },

        // Toolbars
            new MenuDefinition { type = MenuType.Toolstrip, id = MenuIds.MainToolbar, cmdSet = Reko },
            new MenuDefinition { type = MenuType.Toolstrip, id = MenuIds.ProjectBrowserToolbar, cmdSet = Reko },
        };

        // Groups ==========================================================

        // Menu groups
        public GroupDefinition[] Groups = new[]
        {
            new GroupDefinition { id = GroupIds.GrpMain, cmdSet = Reko, container = MenuIds.MainMenu },

            new GroupDefinition { id = GroupIds.GrpFile, cmdSet = Reko, container = MenuIds.FileMenu, priority = 0 },
            new GroupDefinition { id = GroupIds.GrpFileMru, cmdSet = Reko, container = MenuIds.FileMenu, priority = 0 },
            new GroupDefinition { id = GroupIds.GrpFileEnd, cmdSet = Reko, container = MenuIds.FileMenu, priority = 1000 },

            new GroupDefinition { id = GroupIds.GrpEdit, cmdSet = Reko, container = MenuIds.EditMenu },

            new GroupDefinition { id = GroupIds.GrpView, cmdSet = Reko, container = MenuIds.ViewMenu },
            new GroupDefinition { id = GroupIds.GrpLowLevel, cmdSet = Reko, container = MenuIds.ViewMenu },
            new GroupDefinition { id = GroupIds.GrpViewScanned, cmdSet = Reko, container = MenuIds.ViewMenu },

            new GroupDefinition { id = GroupIds.GrpTextEncoding, cmdSet = Reko, container = MenuIds.TextEncodingMenu },

            new GroupDefinition { id = GroupIds.GrpActions, cmdSet = Reko, container = MenuIds.ActionMenu },
            new GroupDefinition { id = GroupIds.GrpActionsScanned, cmdSet = Reko, container = MenuIds.ActionMenu },
            new GroupDefinition { id = GroupIds.GrpActionsRewritten, cmdSet = Reko, container = MenuIds.ActionMenu },

            new GroupDefinition { id = GroupIds.GrpTools, cmdSet = Reko, container = MenuIds.ToolsMenu },

            new GroupDefinition { id = GroupIds.GrpWindows, cmdSet = Reko, container = MenuIds.WindowsMenu },

            new GroupDefinition { id = GroupIds.GrpHelp, cmdSet = Reko, container = MenuIds.HelpMenu },

            new GroupDefinition { id = GroupIds.GrpMemoryControl, cmdSet = Reko, container = MenuIds.CtxMemoryControl },
            new GroupDefinition { id = GroupIds.GrpDisassemblerNav, cmdSet = Reko, container = MenuIds.CtxDisassembler },
            new GroupDefinition { id = GroupIds.GrpDisassemblerShow, cmdSet = Reko, container = MenuIds.CtxDisassembler },
            new GroupDefinition { id = GroupIds.GrpDisassemblerEdit, cmdSet = Reko, container = MenuIds.CtxDisassembler },
            new GroupDefinition { id = GroupIds.GrpCodeView, cmdSet = Reko, container = MenuIds.CtxCodeView },
            new GroupDefinition { id = GroupIds.GrpCodeViewMode, cmdSet = Reko, container = MenuIds.CtxCodeView },
            new GroupDefinition { id = GroupIds.GrpBrowser, cmdSet = Reko, container = MenuIds.CtxBrowser },
            new GroupDefinition { id = GroupIds.GrpBrowserProc, cmdSet = Reko }, // commands used for procedures in lists

            new GroupDefinition { id = GroupIds.GrpToolbarFileOps, cmdSet = Reko, container = MenuIds.MainToolbar },
            new GroupDefinition { id = GroupIds.GrpToolbarActions, cmdSet = Reko, container = MenuIds.MainToolbar },

            new GroupDefinition { id = GroupIds.GrpBrowserToolbar, cmdSet = Reko, container = MenuIds.ProjectBrowserToolbar },

            new GroupDefinition { id = GroupIds.GrpProcedure, cmdSet = Reko, container = MenuIds.CtxProcedure },
            new GroupDefinition { id = GroupIds.GrpProcedureListDebug, cmdSet = Reko, container = MenuIds.CtxProcedureList },

            new GroupDefinition { id = GroupIds.GrpAddressSearchView, cmdSet = Reko, container = MenuIds.CtxAddressSearch },
            new GroupDefinition { id = GroupIds.GrpAddressSearch, cmdSet = Reko, container = MenuIds.CtxAddressSearch },
        };

        // Commands ==========================================================

        public readonly CmdDefinition[] Commands = new[] {
            new CmdDefinition { id = CmdIds.FileOpen, cmdSet = Reko, image = 0, text = "_Open..." },
            new CmdDefinition { id = CmdIds.FileOpenAs, cmdSet = Reko, text = "Op_en As..." },
            new CmdDefinition { id = CmdIds.FileSave, cmdSet = Reko, image = 1, text = "_Save" },
            new CmdDefinition { id = CmdIds.FileNewScript, cmdSet = Reko, text = "New script file..." },
            new CmdDefinition { id = CmdIds.FileAddBinary, cmdSet = Reko, text = "Add _binary file..." },
            new CmdDefinition { id = CmdIds.FileAddMetadata, cmdSet = Reko, text = "Add _metadata file..." },
            new CmdDefinition { id = CmdIds.FileAddScript, cmdSet = Reko, text = "Add _script file..." },
            new CmdDefinition { id = CmdIds.FileAssemble, cmdSet = Reko, text = "Add asse_mbler file..." },
            new CmdDefinition { id = CmdIds.FileCloseProject, cmdSet = Reko, text = "Close projec_t" },
            new CmdDefinition { id = CmdIds.FileMru, cmdSet = Reko, container = GroupIds.GrpFileMru, dynamicItemId = 2200, text="" },
            new CmdDefinition { id = CmdIds.FileExit, cmdSet = Reko, container = GroupIds.GrpFileEnd, text = "E_xit" },

            new CmdDefinition { id = CmdIds.EditFind, cmdSet = Reko, container = GroupIds.GrpEdit, text = "_Find..." },
            new CmdDefinition { id = CmdIds.EditCopy, cmdSet = Reko, text = "_Copy" },
            new CmdDefinition { id = CmdIds.EditCopyAll, cmdSet = Reko, text = "_Copy All" },
            new CmdDefinition { id = CmdIds.EditRename, cmdSet = Reko, container = GroupIds.GrpEdit, text = "_Rename" },
            new CmdDefinition { id = CmdIds.EditStructures, cmdSet = Reko, container = GroupIds.GrpEdit, text = "_Structures"},
            new CmdDefinition { id = CmdIds.EditRegisterValues, cmdSet = Reko, container = GroupIds.GrpEdit, text = "Re_gister Values" },
            new CmdDefinition { id = CmdIds.EditAnnotation, cmdSet = Reko, container = GroupIds.GrpEdit, text = "Co_mment" },

            new CmdDefinition { id = CmdIds.EditSelectAll, cmdSet = Reko, container = GroupIds.GrpEdit, text = "Select _all" },
            new CmdDefinition { id = CmdIds.EditProperties, cmdSet = Reko, text = "P_roperties" },

            new CmdDefinition { id = CmdIds.ViewProjectBrowser, cmdSet = Reko, container = GroupIds.GrpView, text = "Project _browser" },
            new CmdDefinition { id = CmdIds.ViewProcedureList, cmdSet = Reko, container = GroupIds.GrpView, text = "_Procedure list" },
            new CmdDefinition { id = CmdIds.ViewSegmentList, cmdSet = Reko, container = GroupIds.GrpView, text = "_Segment list" },
            new CmdDefinition { id = CmdIds.ViewMemory, cmdSet = Reko, container = GroupIds.GrpLowLevel, text = "_Memory" },
            new CmdDefinition { id = CmdIds.ViewDisassembly, cmdSet = Reko, container = GroupIds.GrpLowLevel, text = "_Disassembly" },
            new CmdDefinition { id = CmdIds.TextEncodingChoose, cmdSet = Reko, container = GroupIds.GrpTextEncoding, text = "_Choose..." },
            new CmdDefinition { id = CmdIds.ViewCallGraph, cmdSet = Reko, container = GroupIds.GrpLowLevel, text = "_Call graph" },
            new CmdDefinition { id = CmdIds.OpenLink, cmdSet = Reko, text = "_Open" },
            new CmdDefinition { id = CmdIds.OpenLinkInNewWindow, cmdSet = Reko, text = "Open in ne_w window" },

            new CmdDefinition { id = CmdIds.ViewGoToAddress, cmdSet = Reko, container = GroupIds.GrpViewScanned, text = "_Go to Address..." },
            new CmdDefinition { id = CmdIds.ViewFindAllProcedures, cmdSet = Reko, container = GroupIds.GrpViewScanned, text = "Find all _procedures" },
            new CmdDefinition { id = CmdIds.ViewShowAllFragments, cmdSet = Reko, container = GroupIds.GrpViewScanned, text = "Show _all fragments" },
            new CmdDefinition { id = CmdIds.ViewShowUnscanned, cmdSet = Reko, container = GroupIds.GrpViewScanned, text = "Show _unscanned fragments" },
            new CmdDefinition { id = CmdIds.ViewFindPattern, cmdSet = Reko, container = GroupIds.GrpViewScanned, text = "Find selected _pattern..." },
            new CmdDefinition { id = CmdIds.ViewFindStrings, cmdSet = Reko, container = GroupIds.GrpViewScanned, text = "Find _strings..." },
            new CmdDefinition { id = CmdIds.ViewFindWhatPointsHere, cmdSet = Reko, container = GroupIds.GrpViewScanned, text = "Find _what points here" },
            new CmdDefinition { id = CmdIds.ViewAsCode, cmdSet = Reko, container = GroupIds.GrpAddressSearchView, text = "View as _code" },
            new CmdDefinition { id = CmdIds.ViewAsStrings, cmdSet = Reko, container = GroupIds.GrpAddressSearchView, text = "View as _strings" },
            new CmdDefinition { id = CmdIds.ViewAsData, cmdSet = Reko, container = GroupIds.GrpAddressSearchView, text = "View as _data" },
            new CmdDefinition { id = CmdIds.ViewCfgCode, cmdSet = Reko, container = GroupIds.GrpCodeViewMode, priority = 1, text = "View _code" },
            new CmdDefinition { id = CmdIds.ViewCfgGraph, cmdSet = Reko, container = GroupIds.GrpCodeViewMode, priority = 2, text = "View control _graph" },
            new CmdDefinition { id = CmdIds.ViewPcRelative, cmdSet = Reko, container = GroupIds.GrpLowLevel, text = "Show _PC relative addresses" },
            new CmdDefinition { id = CmdIds.OpenInNewTab, cmdSet = Reko, container = GroupIds.GrpCodeView, priority = 0, text = "Open in new tab" },
            new CmdDefinition { id = CmdIds.EditDeclaration, cmdSet = Reko, container = GroupIds.GrpCodeView, priority = 1, text = "Edit declaration" },
            new CmdDefinition { id = CmdIds.EditComment, cmdSet = Reko, container = GroupIds.GrpCodeView, priority = 2, text = "Edit comment" },
            new CmdDefinition { id = CmdIds.EditLabel, cmdSet = Reko, container = GroupIds.GrpCodeView, priority = 2, text = "Edit label" },
            new CmdDefinition { id = CmdIds.ActionNextSearchHit, cmdSet = Reko },
            new CmdDefinition { id = CmdIds.ActionPrevSearchHit, cmdSet = Reko },

            new CmdDefinition { id = CmdIds.ActionRestartDecompilation, cmdSet = Reko, container = GroupIds.GrpActions, image = 2, text = "_Restart" },
            new CmdDefinition { id = CmdIds.ActionNextPhase, cmdSet = Reko, container = GroupIds.GrpActions, image = 3, text = "_Next Phase" },
            new CmdDefinition { id = CmdIds.ActionFinishDecompilation, cmdSet = Reko, container = GroupIds.GrpActions, image = 4, text = "Finish _Decompilation" },
            new CmdDefinition { id = CmdIds.ActionMarkProcedure, cmdSet = Reko, text = "Mark _Procedure Entry" },
            new CmdDefinition { id = CmdIds.ActionCallTerminates, cmdSet = Reko, container = GroupIds.GrpDisassemblerEdit, text = "Mark call as _terminating" },
            new CmdDefinition { id = CmdIds.ActionScanHeuristically, cmdSet = Reko, text = "Scan _heuristically" },
            new CmdDefinition { id = CmdIds.ActionEditSignature, cmdSet = Reko, text = "Edit Procedure _Signature..." },
            new CmdDefinition { id = CmdIds.ActionMarkType, cmdSet = Reko, text = "Mark _Type" },
            new CmdDefinition { id = CmdIds.ActionMarkStrings, cmdSet = Reko, text = "Mark selection as _strings" },
            new CmdDefinition { id = CmdIds.ActionAssumeRegisterValues, cmdSet = Reko, text = "_Register values..." },

            new CmdDefinition { id = CmdIds.ToolsHexDisassembler, cmdSet = Reko, container = GroupIds.GrpTools, text = "_Hex disassembler" },
            new CmdDefinition { id = CmdIds.ToolsOptions, cmdSet = Reko, container = GroupIds.GrpTools, text = "_Options..." },
            new CmdDefinition { id = CmdIds.ToolsKeyBindings, cmdSet = Reko, container = GroupIds.GrpTools, text = "_Key bindings..." },
            new CmdDefinition { id = CmdIds.WindowsCascade, cmdSet = Reko, container = GroupIds.GrpWindows, text = "_Cascade" },
            new CmdDefinition { id = CmdIds.WindowsTileVertical, cmdSet = Reko, container = GroupIds.GrpWindows, text = "Tile _Vertically" },
            new CmdDefinition { id = CmdIds.WindowsTileHorizontal, cmdSet = Reko, container = GroupIds.GrpWindows, text = "Tile _Horizontally" },
            new CmdDefinition { id = CmdIds.WindowsCloseAll, cmdSet = Reko, container = GroupIds.GrpWindows, text = "C_lose All Windows" },
            new CmdDefinition { id = CmdIds.HelpAbout, cmdSet = Reko, container = GroupIds.GrpHelp, text = "_About Decompiler..." },

            new CmdDefinition { id = CmdIds.ShowProcedureCallHierarchy, cmdSet = Reko, container = GroupIds.GrpProcedure, text = "Show Call _Hierarchy" },
            new CmdDefinition { id = CmdIds.ProcedureDebugTrace, cmdSet = Reko, container = GroupIds.GrpProcedureListDebug, text = "_Debug procedure decompilation" },

            new CmdDefinition { id = CmdIds.CollapseAllNodes, cmdSet = Reko, container = GroupIds.GrpBrowserToolbar, imageKey = "Collapse.ico", tip = "Collapse All" },
            new CmdDefinition { id = CmdIds.CreateUserSegment, cmdSet = Reko, container = GroupIds.GrpBrowserToolbar, imageKey = "CreateSegment.ico", tip = "Create Segment" },

            new CmdDefinition { id = CmdIds.LoadSymbols, cmdSet = Reko, container = GroupIds.GrpBrowser, text = "Load Symbols..." },
        };
        
        // Placements =======================================================

        public readonly Placement[] Placements = new[]
        {
            new Placement { item=(int)CmdIds.FileOpen, container=GroupIds.GrpFile },

            new Placement { item=(int)CmdIds.FileOpenAs, container=GroupIds.GrpFile },

            new Placement { item=(int)CmdIds.FileSave, container=GroupIds.GrpFile },
            new Placement { item=(int)CmdIds.FileCloseProject, container=GroupIds.GrpFile },
            new Placement { item=(int)CmdIds.FileNewScript, container=GroupIds.GrpFile },
            new Placement { item=(int)CmdIds.FileAddBinary, container=GroupIds.GrpFile },
            new Placement { item=(int)CmdIds.FileAddMetadata, container=GroupIds.GrpFile },
            new Placement { item=(int)CmdIds.FileAddScript, container=GroupIds.GrpFile },
            new Placement { item=(int)CmdIds.FileAssemble, container=GroupIds.GrpFile },

            new Placement { item=GroupIds.GrpBrowserProc, container=MenuIds.CtxBrowser },
            new Placement { item=GroupIds.GrpBrowserProc, container=MenuIds.CtxProcedureList },
            // new Placement { item=GroupIds.GrpProcedureListDebug, container=CtxProcedureList },

            new Placement { item=(int)CmdIds.FileOpen, container=GroupIds.GrpToolbarFileOps },
            new Placement { item=(int)CmdIds.FileSave, container=GroupIds.GrpToolbarFileOps },

            new Placement { item=(int)CmdIds.EditCopy, container=GroupIds.GrpEdit },
            new Placement { item=(int)CmdIds.EditProperties, container=GroupIds.GrpEdit },

            new Placement { item=(int)CmdIds.ActionRestartDecompilation, container=GroupIds.GrpToolbarActions },
            new Placement { item=(int)CmdIds.ActionNextPhase, container=GroupIds.GrpToolbarActions },
            new Placement { item=(int)CmdIds.ActionFinishDecompilation, container=GroupIds.GrpToolbarActions },

            new Placement { item=(int)CmdIds.ActionMarkProcedure, container=GroupIds.GrpActionsScanned },
            new Placement { item=(int)CmdIds.ActionScanHeuristically, container=GroupIds.GrpActionsScanned },
            new Placement { item=(int)CmdIds.ActionMarkType, container=GroupIds.GrpActionsScanned },

            new Placement { item=(int)CmdIds.EditCopy, container=GroupIds.GrpMemoryControl },
            new Placement { item=(int)CmdIds.EditCopy, container=GroupIds.GrpCodeView, priority=2 },
            new Placement { item=(int)CmdIds.ViewGoToAddress, container=GroupIds.GrpMemoryControl },
            new Placement { item=(int)CmdIds.ActionMarkProcedure, container=GroupIds.GrpMemoryControl },
            new Placement { item=(int)CmdIds.ActionMarkType, container=GroupIds.GrpMemoryControl },
            new Placement { item=(int)CmdIds.ActionEditSignature, container=GroupIds.GrpMemoryControl },
            new Placement { item=(int)CmdIds.ViewFindPattern, container=GroupIds.GrpMemoryControl },
            new Placement { item=(int)CmdIds.ViewFindWhatPointsHere, container=GroupIds.GrpMemoryControl },

            new Placement { item=(int)CmdIds.OpenLink, container=GroupIds.GrpDisassemblerNav },
            new Placement { item=(int)CmdIds.OpenLinkInNewWindow, container=GroupIds.GrpDisassemblerNav },
            new Placement { item=(int)CmdIds.ViewPcRelative, container=GroupIds.GrpDisassemblerShow },
            new Placement { item=(int)CmdIds.EditCopy, container=GroupIds.GrpDisassemblerEdit },
            new Placement { item=(int)CmdIds.EditRename, container=GroupIds.GrpDisassemblerEdit },
            new Placement { item=(int)CmdIds.EditRegisterValues, container=GroupIds.GrpDisassemblerEdit },
            new Placement { item=(int)CmdIds.EditAnnotation, container=GroupIds.GrpDisassemblerEdit },

            new Placement { item=(int)CmdIds.ActionEditSignature, container=GroupIds.GrpProcedure },
            new Placement { item=(int)CmdIds.ViewGoToAddress, container=GroupIds.GrpProcedure },
            new Placement { item=(int)CmdIds.ActionEditSignature, container=GroupIds.GrpActionsRewritten },

            new Placement { item=(int)CmdIds.EditProperties, container=GroupIds.GrpBrowser },
            new Placement { item=(int)CmdIds.ViewGoToAddress, container=GroupIds.GrpBrowserProc },
            new Placement { item=(int)CmdIds.ViewFindWhatPointsHere, container=GroupIds.GrpBrowserProc },
            new Placement { item=(int)CmdIds.ShowProcedureCallHierarchy, container=GroupIds.GrpBrowserProc },
            new Placement { item=(int)CmdIds.ActionEditSignature, container=GroupIds.GrpBrowserProc },
            new Placement { item=(int)CmdIds.ActionAssumeRegisterValues, container=GroupIds.GrpBrowserProc },

            new Placement { item=(int)CmdIds.ViewFindWhatPointsHere, container=GroupIds.GrpAddressSearch },
            new Placement { item=(int)CmdIds.ActionMarkProcedure, container=GroupIds.GrpAddressSearch },
            new Placement { item=(int)CmdIds.ActionMarkType, container=GroupIds.GrpAddressSearch },
            new Placement { item=(int)CmdIds.ActionMarkStrings, container=GroupIds.GrpAddressSearch },
            new Placement { item=(int)CmdIds.ActionScanHeuristically, container=GroupIds.GrpAddressSearch },
        };

        // Accelerators ==================================================

        public readonly KeyBinding[] KeyBindings = new KeyBinding[]
        {
            new KeyBinding { id=CmdIds.ActionMarkType, cmdSet=Reko, key1="T", alt1=KeyModifiers.Control },
            new KeyBinding { id=CmdIds.EditDeclaration, cmdSet=Reko, editor="Reko.Gui.Windows.CombinedCodeViewInteractor", key1="D", alt1=KeyModifiers.Control },
            new KeyBinding { id=CmdIds.EditComment, cmdSet=Reko, editor="Reko.Gui.Windows.CombinedCodeViewInteractor",  key1="OemSemicolon" },
            new KeyBinding { id=CmdIds.ActionNextSearchHit, cmdSet=Reko, editor="", key1="F8" },
            new KeyBinding { id=CmdIds.ActionPrevSearchHit, cmdSet=Reko, editor="", key1="F8", alt1=KeyModifiers.Shift },
            new KeyBinding { id=CmdIds.EditCopy, cmdSet = Reko, editor = "", key1 = "C", alt1 = KeyModifiers.Control },
            new KeyBinding { id=CmdIds.EditCopyAll, cmdSet = Reko, editor = "Reko.Gui.Windows.CodeViewerPane", key1 = "C", alt1 = KeyModifiers.Control },
            new KeyBinding { id=CmdIds.EditSelectAll, cmdSet=Reko, editor="", key1="A", alt1=KeyModifiers.Control }
         };
    }

    public class GroupDefinition
    {
        public int id;
        public Guid cmdSet;
        public int container;
        public int priority;
    }

    public class CmdDefinition
    {
        public CmdIds id;
        public Guid cmdSet;
        public int container;
        public int priority;
        public int image = -1;
        public string? text;
        public string? imageKey;
        public int dynamicItemId;
        public string? tip;
    }

    public class Placement
    {
        public int item;
        public int container;
        public int priority;
    }

    public class MenuDefinition
    {
        public MenuType type;
        public int id;
        public Guid cmdSet;
        public string? Text;
        public int priority;
        public int container;
    }

    public class KeyBinding
    {
        public CmdIds id;
        public Guid cmdSet;
        public string? key1;
        public KeyModifiers alt1;
        public string? key2;
        public KeyModifiers alt2;
        public string editor = "";
    }

    [Flags]
    public enum KeyModifiers
    {
        Shift = 1,
        Control = 2,
        Alt = 4,
    }

    public enum MenuType
    {
        Submenu,
        MainMenu,
        ContextMenu,
        Toolstrip,
    }
}