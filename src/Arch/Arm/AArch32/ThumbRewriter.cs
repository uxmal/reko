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
using Reko.Core.Memory;
using Reko.Core.Services;

namespace Reko.Arch.Arm.AArch32
{
    public class ThumbRewriter : ArmRewriter
    {
        public ThumbRewriter(
            ThumbArchitecture arch,
            EndianImageReader rdr,
            IRewriterHost host,
            IStorageBinder binder) :
            base(arch, rdr, host, binder, new T32Disassembler(arch, rdr).GetEnumerator())
        {
            base.pcValueOffset = 4;
        }

        public override Address ComputePcRelativeOffset(MemoryOperand mop)
        {
            var dst = (int) instr.Address.ToLinear() + 2;
            if (mop.Offset is not null)
            {
                dst += mop.Offset.ToInt32();
            }
            return Address.Ptr32((uint) dst).Align(4);
        }

        protected override void ConditionalSkip(bool force)
        {
            if (instr.Mnemonic == Mnemonic.it)
                return;
            base.ConditionalSkip(force);
        }

        protected override void EmitUnitTest(AArch32Instruction instr)
        {
            var testgenSvc = arch.Services.GetService<ITestGenerationService>();
            testgenSvc?.ReportMissingRewriter("ThumbRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        protected override void RewriteIt()
        {
            m.Nop();
        }
    }
}
