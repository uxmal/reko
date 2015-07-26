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

using Decompiler.Core;
using Decompiler.Core.Configuration;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    class UserPreferencesInteractor
    {
        private UserPreferencesDialog dlg;
        private Program program;
        private TreeNode curNodeWnd;
        private UiPreferencesService localSettings;
        private ServiceContainer sc;

        public void Attach(UserPreferencesDialog userPreferencesDialog)
        {
            this.dlg = userPreferencesDialog;

            dlg.Load += dlg_Load;

            dlg.WindowTree.AfterSelect += WindowTree_AfterSelect;
            dlg.WindowFontButton.Click += WindowFontButton_Click;
            dlg.WindowFgButton.Click += WindowFgButton_Click;
            dlg.WindowBgButton.Click += WindowBgButton_Click;

            dlg.ImagebarList.SelectedIndexChanged += ImagebarList_SelectedIndexChanged;
            dlg.ImagebarFgButton.Click += ImagebarFgButton_Click;
            dlg.ImagebarBgButton.Click += ImagebarBgButton_Click;

            dlg.WindowTree.SelectedNode = dlg.WindowTree.Nodes[0];
        }

        void dlg_Load(object sender, EventArgs e)
        {
            this.sc = new ServiceContainer();
            this.localSettings = new UiPreferencesService(null, null);
            sc.AddService(typeof(IUiPreferencesService), localSettings);

            GenerateSimulatedProgram();
            dlg.MemoryControl.ProgramImage = program.Image;
            dlg.MemoryControl.ImageMap = program.ImageMap;
            dlg.MemoryControl.Architecture = program.Architecture;
            dlg.MemoryControl.Font = localSettings.DisassemblerFont ?? new System.Drawing.Font("Lucida Console", 9.0f);
            dlg.DisassemblyControl.Model = null;/**/
            dlg.CodeControl.Model = null; /*;*/ 
        }

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

        void ImagebarBgButton_Click(object sender, EventArgs e)
        {
        }

        void ImagebarFgButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void ImagebarList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        void WindowBgButton_Click(object sender, EventArgs e)
        {
            if (dlg.ColorPicker.ShowDialog(dlg) == DialogResult.OK)
            {
                var node = dlg.WindowTree.SelectedNode;
                if (node == null)
                    return;
            }
        }

        void WindowFgButton_Click(object sender, EventArgs e)
        {
            dlg.ColorPicker.Color = dlg.MemoryControl.ForeColor;
            if (dlg.ColorPicker.ShowDialog(dlg) == DialogResult.OK)
            {
                dlg.MemoryControl.ForeColor = dlg.ColorPicker.Color;
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
                switch ((string)nodeWnd.Tag)
                {
                case "mem":
                    dlg.MemoryControl.BringToFront();
                    break;
                case "dasm":
                    dlg.DisassemblyControl.BringToFront();
                    break;
                case "code":
                    dlg.CodeControl.BringToFront();
                    break;
                }
            }
        }
    }
}