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
using Reko.Core.Absyn;
using Reko.Core.Collections;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Gui.Services;
using Reko.Gui.TextViewing;
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class UserPreferencesInteractor
    {
        private UserPreferencesDialog dlg;
        private Program program;
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
            dlg.FormClosed += dlg_Closed;
            dlg.WindowTree.AfterSelect += WindowTree_AfterSelect;
            dlg.WindowFontButton.Click += WindowFontButton_Click;
            dlg.WindowFgButton.Click += WindowFgButton_Click;
            dlg.WindowBgButton.Click += WindowBgButton_Click;
            dlg.ResetButton.Click += ResetButton_Click;

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
                AddStyle("Comment", UiStyles.CodeComment, dlg.CodeControl)),
            AddStdControl("Project Browser", UiStyles.Browser, dlg.Browser),
            AddStdControl("Lists", UiStyles.List, dlg.List),
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

        private TreeNode AddStdControl(string text, string styleName, Control control, params TreeNode[] nodes)
        {
            var des = new StdUiStyleDesigner(this)
            {
                Style = GetStyle(styleName),
                Control = control,
                EnableFont = false,
            };
            des.ApplyStyleToControl();
            var node = new TreeNode
            {
                Text = text,
                Tag = des,
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

        private Gui.Services.UiStyle GetStyle(string styleName)
        {
            return uipSvc.Styles[styleName];
        }

        private class UiStyleDesigner
        {
            public  Control Control;
            public bool EnableFont;
            public Gui.Services.UiStyle Style { get; set; }

            public UserPreferencesInteractor outer;

            public UiStyleDesigner(UserPreferencesInteractor outer)
            {
                this.outer = outer;
            }

            public Color GetForeColor()
            {
                var style = outer.localSettings.Styles[Style.Name];
                return style.Foreground is not null
                    ? ((SolidBrush)style.Foreground).Color
                    : Color.Empty;
            }

            public Color GetBackColor()
            {
                var style = outer.localSettings.Styles[Style.Name];
                return style.Background is not null
                    ? ((SolidBrush) style.Background).Color
                    : Color.Empty;
            }

            public virtual void SetForeColor(Color color)
            {
                var style = outer.localSettings.Styles[Style.Name];
                if (style.Foreground is not null)
                {
                    ((SolidBrush)style.Foreground).Dispose();
                }
                style.Foreground = new SolidBrush(color);
                Control.Refresh();
            }

            public virtual void SetBackColor(Color color)
            {
                var style = outer.localSettings.Styles[Style.Name];
                if (style.Background is not null)
                {
                    ((SolidBrush)style.Background).Dispose();
                }
                style.Background = new SolidBrush(color);
                Control.Refresh();
            }

            public virtual void SetFont(Font font)
            {
                var style = outer.localSettings.Styles[Style.Name];
                if (style.Font is not null)
                {
                    ((SolidBrush)style.Font).Dispose();
                }
                style.Font = font;
                Control.Refresh();
            }
        }

        private class StdUiStyleDesigner : UiStyleDesigner
        {
            public StdUiStyleDesigner(UserPreferencesInteractor outer) : base(outer)
            {
            }

            public override void SetForeColor(Color color)
            {
                var style = outer.localSettings.Styles[Style.Name];
                if (style.Foreground is not null)
                {
                    ((SolidBrush)style.Foreground).Dispose();
                }
                style.Foreground = new SolidBrush(color);
                Control.ForeColor = color;
                Control.Refresh();
            }

            public override void SetBackColor(Color color)
            {
                var style = outer.localSettings.Styles[Style.Name];
                if (style.Background is not null)
                {
                    ((SolidBrush) style.Background).Dispose();
                }
                style.Background = new SolidBrush(color);
                Control.BackColor = color;
                Control.Refresh();
            }

            public override void SetFont(Font font)
            {
                var style = outer.localSettings.Styles[Style.Name];
                if (style.Font is not null)
                {
                    ((Font) style.Font).Dispose();
                }
                style.Font = font;
                Control.Font = font;
                Control.Refresh();
            }

            public virtual void ApplyStyleToControl()
            {
                if (Style is not null)
                {
                    if (Style.Foreground is not null)
                    {
                        Control.ForeColor = ((SolidBrush)Style.Foreground).Color;
                    }
                    if (Style.Background is not null)
                    {
                        Control.BackColor = ((SolidBrush)Style.Background).Color;
                    }
                }
            }
        }

        void dlg_Load(object sender, EventArgs e)
        {
            this.dlg.MemoryControl.Services = dlg.Services;
            this.uipSvc = dlg.Services.RequireService<IUiPreferencesService>();
            PopulateStyleTree();

            this.sc = new ServiceContainer();
            var cfgSvc = dlg.Services.RequireService<IConfigurationService>();
            var settingsSvc = dlg.Services.RequireService<ISettingsService>();
            this.localSettings = new UiPreferencesService(cfgSvc, settingsSvc);
            sc.AddService<IUiPreferencesService>(localSettings);
            CopyStyles(uipSvc, localSettings);

            GenerateSimulatedProgram();
            dlg.MemoryControl.Services = sc;
            dlg.MemoryControl.SegmentMap = program.SegmentMap;
            dlg.MemoryControl.ImageMap = program.ImageMap;
            dlg.MemoryControl.Procedures = program.Procedures;
            dlg.MemoryControl.Architecture = program.Architecture;
            dlg.MemoryControl.SelectedAddress = program.SegmentMap.BaseAddress;
            dlg.MemoryControl.Font = new System.Drawing.Font("Lucida Console", 9.0f);
            dlg.DisassemblyControl.StyleClass = UiStyles.Disassembler;
            dlg.DisassemblyControl.Services = sc;
            dlg.DisassemblyControl.Model = new DisassemblyTextModel(dlg.TextSpanFactory, program, null, program.SegmentMap.Segments.Values.First());
            dlg.CodeControl.Services = sc;
            dlg.CodeControl.Model = GenerateSimulatedHllCode();
        }

        private void CopyStyles(IUiPreferencesService from, IUiPreferencesService to)
        {
            to.Styles.Clear();
            foreach (var style in from.Styles.Values)
            {
                to.Styles[style.Name] = style.Clone();
            }
        }

        /// <summary>
        /// Create a simulated program to use in the code /data display.
        /// </summary>
        private void GenerateSimulatedProgram()
        {
            var row = Enumerable.Range(0, 0x100).Select(b => (byte)b).ToArray();
            var image = new ByteMemoryArea(
                    Address.Ptr32(0x0010000),
                    Enumerable.Repeat(
                        row,
                        40).SelectMany(r => r).ToArray());
            var addrCode = Address.Ptr32(0x0010008);
            var addrData = Address.Ptr32(0x001001A);
            var segmentMap = new SegmentMap(
                image.BaseAddress,
                new ImageSegment("code", image,  AccessMode.ReadWriteExecute));
            var imageMap = segmentMap.CreateImageMap();
            imageMap.AddItemWithSize(addrCode, new ImageMapBlock(addrCode) { Size = 0x0E });
            imageMap.AddItemWithSize(addrData, new ImageMapItem(addrData) { DataType = PrimitiveType.Byte, Size = 0x0E });
            var arch = dlg.Services.RequireService<IConfigurationService>().GetArchitecture("x86-protected-32");
            this.program = new Program
            {
                Memory = new ByteProgramMemory(segmentMap),
                SegmentMap = segmentMap,
                Architecture = arch,
                ImageMap = imageMap,
            };

            dlg.Browser.Nodes.AddRange(new[]
            {
                new TreeNode {
                    Text = "Image",
                    ImageKey = "Binary.ico",
                    SelectedImageKey = "Binary.ico",
                    Nodes =
                    {
                        new TreeNode
                        {
                            Text = ".text",
                            ImageKey = "RxSection.ico",
                            SelectedImageKey = "RxSection.ico",
                            Nodes =
                            {
                                new TreeNode
                                {
                                    Text = "fn0040000",
                                    ImageKey = "EntryProcedure.ico",
                                    SelectedImageKey = "EntryProcedure.ico",
                                },
                                new TreeNode
                                {
                                    Text = "myFunc",
                                    ImageKey = "Usercode.ico",
                                    SelectedImageKey = "Usercode.ico",
                                },
                                new TreeNode
                                {
                                    Text = "fn0040200",
                                    ImageKey = "Cde.ico",
                                    SelectedImageKey = "Code.ico",
                                },
                            }
                        }
                    }
                },
            });
            dlg.Browser.ExpandAll();

            dlg.List.Items.AddRange(new ListViewItem[]
            {
                new ListViewItem(new string[] { "foo.exe", "00400000", "Code"}),
                new ListViewItem(new string[] { "foo.exe", "00400210", "Code"}),
                new ListViewItem(new string[] { "foo.exe", "00400800", "Data"}),
            });
        }

        private ITextViewModel GenerateSimulatedHllCode()
        {
            var code = new List<AbsynStatement>();
            var ase = new Structure.AbsynStatementEmitter(code);
            var a_id = new Identifier("a", PrimitiveType.Int32, null);
            var sum_id = new Identifier("sum", PrimitiveType.Int32, null);
            ase.EmitAssign(a_id, Constant.Int32(10));
            ase.EmitAssign(sum_id, Constant.Int32(0));

            var tsf = new TextSpanFormatter(dlg.TextSpanFactory);
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
            if ((Gui.Services.DialogResult)dlg.ColorPicker.ShowDialog(dlg) == Gui.Services.DialogResult.OK)
            {
                GetSelectedDesigner().SetForeColor(dlg.ColorPicker.Color);
            }
        }

        void WindowBgButton_Click(object sender, EventArgs e)
        {
            dlg.ColorPicker.Color = GetSelectedDesigner().GetBackColor();
            if ((Gui.Services.DialogResult)dlg.ColorPicker.ShowDialog(dlg) == Gui.Services.DialogResult.OK)
            {
                GetSelectedDesigner().SetBackColor(dlg.ColorPicker.Color);
            }
        }

        void WindowFontButton_Click(object sender, EventArgs e)
        {
            dlg.FontPicker.Font = dlg.MemoryControl.Font;
            if ((Gui.Services.DialogResult)dlg.FontPicker.ShowDialog(dlg) == Gui.Services.DialogResult.OK)
            {
                GetSelectedDesigner().SetFont(dlg.FontPicker.Font);
            }
        }

        void ResetButton_Click(object sender, EventArgs e)
        {
            var des = GetSelectedDesigner();
            var name = des.Style.Name;
            localSettings.ResetStyle(name);
            des.Control.Refresh();
        }

        void WindowTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = dlg.WindowTree.SelectedNode;
            var nodeWnd = node;
            if (node.Parent is not null)
                nodeWnd = node.Parent;

            var designer = (UiStyleDesigner)nodeWnd.Tag;
            dlg.WindowFontButton.Enabled = designer.EnableFont;
            dlg.ResetButton.Enabled = designer.EnableFont;
            designer.Control.BringToFront();
        }

        private void dlg_Closed(object sender, FormClosedEventArgs e)
        {
            if ((Gui.Services.DialogResult)dlg.DialogResult != Gui.Services.DialogResult.OK)
                return;
            CopyStyles(localSettings, uipSvc);
        }
    }
}