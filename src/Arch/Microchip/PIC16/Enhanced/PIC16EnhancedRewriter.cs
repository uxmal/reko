#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;

namespace Reko.Arch.Microchip.PIC16
{
    using Common;

    public class PIC16EnhancedRewriter : PIC16RewriterBase
    {

        private PIC16EnhancedRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            : base(arch, dasm, state, binder, host)
        {
        }

        public static PICRewriter Create(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new PIC16EnhancedRewriter(
                arch ?? throw new ArgumentNullException(nameof(arch)),
                dasm ?? throw new ArgumentNullException(nameof(dasm)),
                state ?? throw new ArgumentNullException(nameof(state)),
                binder ?? throw new ArgumentNullException(nameof(binder)),
                host ?? throw new ArgumentNullException(nameof(host))
              );
        }


        protected override void RewriteInstr()
        {
            switch (instrCurr.Opcode)
            {
                default:
                    base.RewriteInstr();
                    break;

                case Opcode.ADDFSR:
                case Opcode.ADDWFC:
                case Opcode.ASRF:
                case Opcode.BRW:
                case Opcode.CALLW:
                case Opcode.LSLF:
                case Opcode.LSRF:
                case Opcode.MOVIW:
                case Opcode.MOVLB:
                case Opcode.MOVLP:
                case Opcode.MOVWI:
                case Opcode.RESET:
                case Opcode.SUBWFB:
                    break;
            }

        }


        #region Helpers

        private Expression GetFSRRegister(MachineOperand op)
        {
            if (op is PIC16FSROperand fsr)
            {
                switch (fsr.FSRNum.ToByte())
                {
                    case 0:
                        return binder.EnsureRegister(PIC16FullRegisters.FSR0);
                    case 1:
                        return binder.EnsureRegister(PIC16FullRegisters.FSR1);
                    default:
                        throw new InvalidOperationException($"Invalid FSR number: {fsr.FSRNum.ToByte()}");
                }
            }
            else
                throw new InvalidOperationException($"Invalid FSR operand.");

        }

        #endregion

        #region Rewrite methods

        #endregion

    }

}
