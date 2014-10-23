#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Gui.Windows.Controls
{
    /// <summary>
    /// Presents disassembled instructions as lines of text
    /// </summary>
    public class DisassemblyTextModel : TextViewModel
    {
        public event EventHandler ModelChanged;

        private IProcessorArchitecture arch;
        private LoadedImage image;
        private Dictionary<int, Address> cache;
        private int mostRecentCacheSize;

        public DisassemblyTextModel(IProcessorArchitecture arch, LoadedImage image)
        {
            this.arch = arch;
            this.image = image;
            this.cache = new Dictionary<int, Address>();
        }

        public int LineCount { get { return GetPositionEstimate(image.Bytes.Length); } }

        public IEnumerable<TextSpan> GetLineSpans(int position)
        {
            throw new NotImplementedException();
        }

        public int EstablishPosition(Address addr)
        {
            if (addr == null || !image.IsValidAddress(addr))
                return -1;
            int idx = GetPositionEstimate(addr - image.BaseAddress);
            cache = new Dictionary<int, Address>
            {
                { idx, addr }
            };
            return idx;
        }

        private int GetPositionEstimate(int byteOffset)
        {
            return 8 * byteOffset / arch.InstructionBitSize;
        }

        public void CacheHint(int index, int count)
        {
            throw new NotImplementedException();
        }
    }
}
