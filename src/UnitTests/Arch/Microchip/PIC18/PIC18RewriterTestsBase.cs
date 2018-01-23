#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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

using Microchip.Crownking;
using Reko.Arch.Microchip.PIC18;
using Reko.Core;
using Reko.Core.Rtl;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Arch.Microchip.PIC18.Rewriter
{
    public class PIC18RewriterTestsBase : RewriterTestBase
    {
        protected static PIC18Architecture arch;
        protected Address baseAddr = Address.Ptr32(0x200);
        protected PIC18State state;
        protected MemoryArea image;

        public override IProcessorArchitecture Architecture => arch;

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder frame, IRewriterHost host)
        {
            return new PIC18Rewriter(arch, new LeImageReader(image, 0), state, new Frame(arch.WordWidth), host);
        }

        public override Address LoadAddress => baseAddr;

        protected override MemoryArea RewriteCode(uint[] words)
        {
            byte[] bytes = words.SelectMany(w => new byte[]
            {
                (byte) w,
                (byte) (w >> 8),
            }).ToArray();
            image = new MemoryArea(LoadAddress, bytes);
            return image;
        }

        protected void SetPICMode(InstructionSetID id, PICExecMode mode)
        {
            arch = new PIC18Architecture(PICSamples.GetSample(id))
            {
                ExecMode = mode
            };
            state = (PIC18State)arch.CreateProcessorState();
        }

    }

}
