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
using Reko.Core.Loading;
using Reko.Core.Output;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Reko.ImageLoaders.WebAssembly.Output
{
    public class FunctionSectionRenderer : ImageSegmentRenderer
    {
        private readonly WasmArchitecture wasm;
        private readonly FunctionSection functionSection;
        private readonly WasmFile wasmFile;
        private readonly TypeSection? typeSection;

        public FunctionSectionRenderer(WasmArchitecture wasm, FunctionSection functionSection, WasmFile wasmFile)
        {
            this.wasm = wasm;
            this.functionSection = functionSection;
            this.wasmFile = wasmFile;
            this.typeSection = wasmFile.Sections.OfType<TypeSection>().FirstOrDefault();
        }

        public override void Render(ImageSegment segment, Program program, Formatter formatter)
        {
            if (typeSection is null)
                return;

            int ifunc = -1;
            foreach (var itype in this.functionSection.Declarations)
            {
                ++ifunc;
                var sig = typeSection.Types[(int) itype];
                formatter.WriteLine("  func 0x{0:X}: {1}", ifunc, sig);
            }
        }
    }
}