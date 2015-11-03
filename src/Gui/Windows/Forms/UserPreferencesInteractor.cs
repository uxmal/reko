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

using Reko.Core;
using Reko.Core.Absyn;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Output;
using Reko.Core.Types;
using Reko.Gui.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.Gui.Windows.Forms
{
    class UserPreferencesInteractor
    {
        private UserPreferencesDialog dlg;
        private Program program;
        private TreeNode curNodeWnd;
        private UiPreferencesService localSettings;
        private ServiceContainer sc;
        private Dictionary<string, string> descs;
        private IUiPreferencesService uipSvc;

        public void Attach(UserPreferencesDialog userPreferencesDialog)
        {
            this.dlg = userPreferencesDialog;
            this.descs = new Dictionary<string, string> {
                { "Code", "Bytes that are known to be executable code." },
                { "Heuristic code", "Bytes that have been determined to be executable code using heuristics." },
                { "Data", "Bytes that are known to be used as data." },
                { "Address", "Bytes that are known to be an address." },
                { "Unknown", "Bytes whose function is unknown. " }
            };

            dlg.Load += dlg_Load;
            dlg.WindowTree.AfterSelect += WindowTree_AfterSelect;
            dlg.WindowFontButton.Click += WindowFontButton_Click;
            dlg.WindowFgButton.Click += WindowFgButton_Click;
            dlg.WindowBgButton.Click += WindowBgButton_Click;

            dlg.ImagebarList.SelectedIndexChanged += ImagebarList_SelectedIndexChanged;
            dlg.ImagebarFgButton.Click += ImagebarFgButton_Click;
            dlg.ImagebarBgButton.Click += ImagebarBgButton_Click;

        }

        void PopulateStyleTree()
        {
            var nodes = new TreeNode[] {
            AddControl("Memory Window", UiStyles.MemoryWindow, dlg.MemoryControl,
                AddStyle("Code", UiStyles.MemoryCode, dlg.MemoryControl),
                AddStyle("Heuristic code", UiStyles.MemoryHeuristic, dlg.MemoryControl),
                AddStyle("Data", UiStyles.MemoryData, dlg.MemoryControl)),
            AddControl("Disassembly Window", UiStyles.Disassembler, dlg.DisassemblyControl,
                AddStyle("Opcode", UiStyles.DisassemblerOpcode, dlg.DisassemblyControl)),
            AddControl("Code Window", UiStyles.CodeWindow, dlg.CodeControl,
                AddStyle("Keyword", UiStyles.CodeKeyword, dlg.CodeControl),
                AddStyle("Comment", UiStyles.CodeComment, dlg.CodeControl))
            };
            dlg.WindowTree.Nodes.AddRange(nodes);
            dlg.WindowTree.SelectedNode = dlg.WindowTree.Nodes[0];
            dlg.WindowTree.ExpandAll();
        }
        

        private TreeNode AddControl(string text, string styleName, Control control, params TreeNode [] nodes)
        {
            var node = new TreeNode
            {
                Text = text,
                Tag = new UiStyleDesigner(this)
                {
                    Style = GetStyle(styleName),
                    Control = control,
                    EnableFont = true,
                },
            };
            node.Nodes.AddRange(nodes);
            return node;
        }

        private TreeNode AddStyle(string text, string styleName, Control control)
        {
            return new TreeNode
            {
                Text = text,
                Tag = new UiStyleDesigner(this)
                {
                    Style = GetStyle(styleName),
                    Control = control,
                }
            };
        }

        private UiStyle GetStyle(string styleName)
        {
            return uipSvc.Styles[styleName];
        }

        private class UiStyleDesigner
        {
            public  Control Control;
            public bool EnableFont;
            public UiStyle Style { get; set; }

            private UserPreferencesInteractor outer;

            public UiStyleDesigner(UserPreferencesInteractor outer)
            {
                this.outer = outer;
            }

            public Color GetForeColor()
            {
                var style = outer.localSettings.Styles[Style.Name];
                return style.Foreground != null
                    ? style.Foreground.Color
                    : Color.Empty;
            }

            public Color GetBackColor()
            {
                var style = outer.localSettings.Styles[Style.Name];
                return style.Background != null
                    ? style.Background.Color
                    : Color.Empty;
            }

            public void SetForeColor(Color color)
            {
                var style = outer.localSettings.Styles[Style.Name];
                if (style.Foreground != null)
                {
                    style.Foreground.Dispose();
                }
                style.Foreground = new SolidBrush(color);
                Control.Refresh();
            }

            public void SetBackColor(Color color)
            {
                var style = outer.localSettings.Styles[Style.Name];
                if (style.Background != null)
                {
                    style.Background.Dispose();
                }
                style.Background = new SolidBrush(color);
                Control.Refresh();
            }
        }

        void dlg_Load(object sender, EventArgs e)
        {
            this.dlg.MemoryControl.Services = dlg.Services;
            this.uipSvc = dlg.Services.RequireService<IUiPreferencesService>();
            PopulateStyleTree();

            this.sc = new ServiceContainer();
            this.localSettings = new UiPreferencesService(null, null);
            sc.AddService(typeof(IUiPreferencesService), localSettings);
            CopyStyles(uipSvc, localSettings);

            GenerateSimulatedProgram();
            dlg.MemoryControl.Services = sc;
            dlg.MemoryControl.ProgramImage = program.Image;
            dlg.MemoryControl.ImageMap = program.ImageMap;
            dlg.MemoryControl.Architecture = program.Architecture;
            dlg.MemoryControl.Font = new System.Drawing.Font("Lucida Console", 9.0f);
            dlg.DisassemblyControl.Services = sc;
            dlg.DisassemblyControl.Model = new DisassemblyTextModel(program);
            dlg.CodeControl.Model = GenerateSimulatedHllCode();
        }

        private void CopyStyles(IUiPreferencesService from, IUiPreferencesService to)
        {
            foreach (var style in from.Styles.Values)
            {
                to.Styles[style.Name] = style.Clone();
            }
        }

        /// <summary>
        /// Create a simulatd program to use in the code /data display.
        /// </summary>
        private void GenerateSimulatedProgram()
        {
            var row = Enumerable.Range(0, 0x100).Select(b => (byte)b).ToArray();
            var image = new LoadedImage(
                    Address.Ptr32(0x0010000),
                    Enumerable.Repeat(
                        row,
                        40).SelectMany(r => r).ToArray());
            var imageMap = image.CreateImageMap();
            var addrCode = Address.Ptr32(0x0010008);
            var addrData = Address.Ptr32(0x001001A);
            imageMap.AddItemWithSize(addrCode, new ImageMapBlock { Address = addrCode, Size = 0x0E });
            imageMap.AddItemWithSize(addrData, new ImageMapItem { Address = addrData, DataType = PrimitiveType.Byte, Size = 0x0E });
            var arch = dlg.Services.RequireService<IConfigurationService>().GetArchitecture("x86-protected-32");
            this.program = new Program
            {
                Image = image,
                ImageMap = imageMap,
                Architecture = arch,
            };
        }

        private TextViewModel GenerateSimulatedHllCode()
        {
            var code = new List<AbsynStatement>();
            var ase = new Structure.AbsynStatementEmitter(code);
            var m = new ExpressionEmitter();
            var a_id = new Identifier("a", PrimitiveType.Int32, null);
            var sum_id = new Identifier("sum", PrimitiveType.Int32, null);
            ase.EmitAssign(a_id, Constant.Int32(10));
            ase.EmitAssign(sum_id, Constant.Int32(0));

            var tsf = new TextSpanFormatter();
            var fmt = new AbsynCodeFormatter(tsf);
            fmt.InnerFormatter.UseTabs = false;
            foreach (var stm in code)
            {
                stm.Accept(fmt);
            }
            return tsf.GetModel();
        }

        void ImagebarBgButton_Click(object sender, EventArgs e)
        {
        }

        void ImagebarFgButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void ImagebarList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var desc = descs[(string)dlg.ImagebarList.SelectedItem];
        }


        private UiStyleDesigner GetSelectedDesigner()
        {
            return (UiStyleDesigner)dlg.WindowTree.SelectedNode.Tag;
        }

        void WindowFgButton_Click(object sender, EventArgs e)
        {
            dlg.ColorPicker.Color = GetSelectedDesigner().GetForeColor();
            if (dlg.ColorPicker.ShowDialog(dlg) == DialogResult.OK)
            {
                GetSelectedDesigner().SetForeColor(dlg.ColorPicker.Color);
            }
        }

        void WindowBgButton_Click(object sender, EventArgs e)
        {
            dlg.ColorPicker.Color = GetSelectedDesigner().GetBackColor();
            if (dlg.ColorPicker.ShowDialog(dlg) == DialogResult.OK)
            {
                GetSelectedDesigner().SetBackColor(dlg.ColorPicker.Color);
            }
        }

        void WindowFontButton_Click(object sender, EventArgs e)
        {
            dlg.FontPicker.Font = dlg.MemoryControl.Font;
            if (dlg.FontPicker.ShowDialog(dlg) == DialogResult.OK)
            {
                dlg.MemoryControl.Font = dlg.FontPicker.Font;
            }
        }

        void WindowTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = dlg.WindowTree.SelectedNode;
            var nodeWnd = node;
            if (node.Parent != null)
                nodeWnd = node.Parent;

            if (nodeWnd != curNodeWnd)
            {
                var designer = (UiStyleDesigner)nodeWnd.Tag;
                dlg.WindowFontButton.Enabled = designer.EnableFont;
                designer.Control.BringToFront();
            }
        }
    }
}