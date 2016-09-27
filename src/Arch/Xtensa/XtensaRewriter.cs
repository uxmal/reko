#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Rtl;
using System.Collections;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;

namespace Reko.Arch.Xtensa
{
    public partial class XtensaRewriter : IEnumerable<RtlInstructionCluster>
    {
        private Frame frame;
        private IRewriterHost host;
        private ImageReader rdr;
        private ProcessorState state;
        private XtensaArchitecture arch;
        private IEnumerator<XtensaInstruction> dasm;
        private RtlInstructionCluster rtlc;
        private RtlEmitter emitter;
        private XtensaInstruction instr;

        public XtensaRewriter(XtensaArchitecture arch, ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.frame = frame;
            this.host = host;
            this.dasm = new XtensaDisassembler(this.arch, rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                rtlc = new RtlInstructionCluster(dasm.Current.Address, dasm.Current.Length);
                rtlc.Class = RtlClass.Linear;
                emitter = new RtlEmitter(rtlc.Instructions);
                this.instr = dasm.Current;
                switch (instr.Opcode)
                {
                default:
                    throw new AddressCorrelatedException(
                       instr.Address,
                       "Rewriting of Xtensa instruction '{0}' not implemented yet.",
                       instr.Opcode);
                case Opcodes.call0: RewriteCall0(); break;
                case Opcodes.ill: RewriteIll(); break;
                case Opcodes.l32i_n: RewriteL32i(); break;
                case Opcodes.l32r: RewriteCopy(); break;
                case Opcodes.memw: RewriteNop(); break;
                case Opcodes.movi: RewriteCopy(); break;
                case Opcodes.movi_n: RewriteMovi_n(); break;
                case Opcodes.or: RewriteOr(); break;
                case Opcodes.reserved: RewriteReserved(); break;
                case Opcodes.ret: RewriteRet(); break;
                case Opcodes.s32i: RewriteS32i(); break;
                case Opcodes.sub: RewriteSub(); break;
                case Opcodes.wsr: RewriteWsr(); break;
                }
                yield return rtlc;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression RewriteOp(MachineOperand op)
        {
            var rOp = op as RegisterOperand;
            if (rOp != null)
            {
                return frame.EnsureRegister(rOp.Register);
            }
            var aOp = op as AddressOperand;
            if (aOp != null)
            {
                return aOp.Address;
            }
            var iOp = op as ImmediateOperand;
            if (iOp != null)
            {
                return iOp.Value;
            }
            throw new NotImplementedException(op.GetType().FullName);
        }
    }
}