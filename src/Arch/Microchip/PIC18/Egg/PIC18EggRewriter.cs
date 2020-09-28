#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Libraries.Microchip;
using System;

namespace Reko.Arch.MicrochipPIC.PIC18
{
    using Common;

    public class PIC18EggRewriter : PIC18RewriterBase
    {
        private PIC18EggRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
            : base(arch, dasm, state, binder, host)
        {
        }

        public static PICRewriter Create(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new PIC18EggRewriter(
                arch ?? throw new ArgumentNullException(nameof(arch)),
                dasm ?? throw new ArgumentNullException(nameof(dasm)),
                state ?? throw new ArgumentNullException(nameof(state)),
                binder ?? throw new ArgumentNullException(nameof(binder)),
                host ?? throw new ArgumentNullException(nameof(host))
              );
        }

        protected override void RewriteInstr()
        {
            switch (instrCurr.Mnemonic)
            {
                default:
                    base.RewriteInstr();
                    break;

                case Mnemonic.ADDFSR:
                    RewriteADDFSR();
                    break;

                case Mnemonic.ADDULNK:
                    RewriteADDULNK();
                    break;

                case Mnemonic.CALLW:
                    RewriteCALLW();
                    break;

                case Mnemonic.MOVSF:
                    RewriteMOVSF();
                    break;

                case Mnemonic.MOVSS:
                    RewriteMOVSS();
                    break;

                case Mnemonic.PUSHL:
                    RewritePUSHL();
                    break;

                case Mnemonic.SUBFSR:
                    RewriteSUBFSR();
                    break;

                case Mnemonic.SUBULNK:
                    RewriteSUBULNK();
                    break;
            }

        }

        private void RewriteADDULNK()
        {
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            m.Assign(Fsr2, m.IAdd(Fsr2, k.ImmediateValue));
            var src = PopFromHWStackAccess();
            m.Assign(tos, src);
            rtlc = InstrClass.Transfer;
            m.Return(0, 0);
        }

        private void RewriteCALLW()
        {

            rtlc = InstrClass.Transfer | InstrClass.Call;

            var pclat = binder.EnsureRegister(PIC18Registers.PCLAT);
            var target = m.Fn(host.PseudoProcedure("__callw", VoidType.Instance, Wreg, pclat));
            var retaddr = instrCurr.Address + instrCurr.Length;
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            var dst = PushToHWStackAccess();
            m.Assign(dst, retaddr);
            m.Assign(tos, retaddr);
            m.Call(target, 0);
        }

        private void RewriteMOVSF()
        {
            var zs = GetFSR2IdxAddress(instrCurr.Operands[0]);
            var (indMode, memPtr) = GetUnaryAbsPtrs(instrCurr.Operands[1], out Expression memExpr);
            ArithAssignIndirect(memExpr, zs, indMode, memPtr);
        }

        private void RewriteMOVSS()
        {
            var zs = GetFSR2IdxAddress(instrCurr.Operands[0]);
            var zd = GetFSR2IdxAddress(instrCurr.Operands[1]);
            m.Assign(zd, zs);
        }

        private void RewritePUSHL()
        {
            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            m.Assign(DataMem8(Fsr2), k.ImmediateValue);
            m.Assign(Fsr2, m.IAdd(Fsr2, 1));
        }

        private void RewriteSUBULNK()
        {
            rtlc = InstrClass.Transfer;

            var k = instrCurr.Operands[0] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {instrCurr.Operands[0]}");
            var tos = binder.EnsureRegister(PIC18Registers.TOS);

            m.Assign(Fsr2, m.ISub(Fsr2, k.ImmediateValue));
            var src = PopFromHWStackAccess();
            m.Assign(tos, src);
            rtlc = InstrClass.Transfer;
            m.Return(0, 0);
        }

        private Expression GetFSR2IdxAddress(MachineOperand op)
        {
            switch (op)
            {
                case PICOperandFSRIndexation fsr2idx:
                    return DataMem8(m.IAdd(Fsr2, fsr2idx.Offset));

                default:
                    throw new InvalidOperationException($"Invalid FSR2 indexed address operand.");
            }
        }

    }

}
