#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Output;

namespace Reko.ImageLoaders.WebAssembly.Output
{
    /// <summary>
    /// Renders WASM Export sections.
    /// </summary>
    public class ExportSectionRenderer : ImageSegmentRenderer
    {
        private readonly WasmArchitecture arch;
        private readonly ExportSection section;
        private readonly WasmFile wasmFile;

        public ExportSectionRenderer(WasmArchitecture arch, ExportSection section, WasmFile wasmFile)
        {
            this.arch = arch;
            this.section = section;
            this.wasmFile = wasmFile;
        }

        public override void Render(ImageSegment segment, Program program, Formatter formatter)
        {
            foreach (var entry in section.ExportEntries)
            {
                formatter.Write(entry.ToString());
                formatter.WriteLine();
            }
        }
    }
}
