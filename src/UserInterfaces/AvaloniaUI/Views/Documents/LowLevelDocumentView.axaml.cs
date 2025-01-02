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

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Reko.Gui.ViewModels.Documents;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents;
using System;

namespace Reko.UserInterfaces.AvaloniaUI.Views.Documents
{
    public partial class LowLevelDocumentView : UserControl
    {
        public LowLevelDocumentView()
        {
            InitializeComponent();
        }

        public LowLevelViewModel? ViewModel => ((LowLevelDocumentViewModel?) DataContext)?.ViewModel;


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            if (ViewModel is null)
            {
                if (hexView is not null)
                {
                    hexView.Architecture = null;
                    hexView.ImageMap = null;
                    hexView.SegmentMap = null;
                    hexView.MemoryArea = null;
                }
            }
            else
            {
                //HexViewControl1.BytesWidth = 16;
                ////var bytes = Enumerable.Range(0, 65534).Select(i => (byte)i).ToArray();
                //var bytes = Enumerable.Range(0, 509).Select(i => (byte) i).ToArray();
                //var mem = new ByteMemoryArea(Address.Ptr32(0), bytes);
                //var segmentMap = new SegmentMap(new ImageSegment(".text", mem, AccessMode.ReadWriteExecute));
                //var imageMap = segmentMap.CreateImageMap();
                //var addr = mem.BaseAddress;
                //imageMap.AddItemWithSize(addr + 2, new ImageMapBlock(addr + 2) { Size = 0x20 });
                //imageMap.AddItemWithSize(addr + 0x3D, new ImageMapBlock(addr + 0x3D) { Size = 0x1D });
                //imageMap.AddItemWithSize(addr + 0x81, new ImageMapItem(addr + 0x81, 4) { DataType = PrimitiveType.Word32 });
                //HexViewControl1.Architecture = new X86ArchitectureFlat32(new ServiceContainer(), "x86", new());
                //HexViewControl1.ImageMap = imageMap;
                //HexViewControl1.SegmentMap = segmentMap;
                //HexViewControl1.MemoryArea = mem;
            }
        }
    }
}
