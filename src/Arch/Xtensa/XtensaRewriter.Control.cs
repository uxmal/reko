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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Xtensa
{
    public partial class XtensaRewriter
    {
        private void RewriteBall()
        {
            iclass = InstrClass.ConditionalTransfer;
            var a = RewriteOp(instr.Operands[0]);
            var b = RewriteOp(instr.Operands[1]);
            var cond = m.Eq0(m.And(m.Comp(a), b));
            m.Branch(
                cond, 
                (Address)RewriteOp(instr.Operands[2]),
                InstrClass.ConditionalTransfer);
        }

        private void RewriteBany()
        {
            iclass = InstrClass.ConditionalTransfer;
            var a = RewriteOp(instr.Operands[0]);
            var b = RewriteOp(instr.Operands[1]);
            var cond = m.Ne0(m.And(a, b));
            m.Branch(
                cond,
                (Address)RewriteOp(instr.Operands[2]),
                InstrClass.ConditionalTransfer);
        }

        private void RewriteBbx(Func<Expression, Expression> cmp0)
        {
            iclass = InstrClass.ConditionalTransfer;
            var src = RewriteOp(instr.Operands[0]);
            Expression mask;
            if (instr.Operands[1] is Constant immOp)
            {
                mask = Constant.Word32(1 << immOp.ToInt32());
            }
            else
            {
                mask = m.Shl(Constant.UInt32(1), RewriteOp(instr.Operands[1]));
            }
            m.Branch(
                cmp0(m.And(src, mask)),
                (Address)RewriteOp(instr.Operands[2]),
                InstrClass.ConditionalTransfer);
        }

        private void RewriteBnall()
        {
            iclass = InstrClass.ConditionalTransfer;
            var a = RewriteOp(instr.Operands[0]);
            var b = RewriteOp(instr.Operands[1]);
            var cond = m.Ne0(m.And(m.Comp(a), b));
            m.Branch(
                cond,
                (Address)RewriteOp(instr.Operands[2]),
                InstrClass.ConditionalTransfer);
        }

        private void RewriteBnone()
        {
            iclass = InstrClass.ConditionalTransfer;
            var a = RewriteOp(instr.Operands[0]);
            var b = RewriteOp(instr.Operands[1]);
            var cond = m.Eq0(m.And(a, b));
            m.Branch(
                cond,
                (Address)RewriteOp(instr.Operands[2]),
                InstrClass.ConditionalTransfer);
        }

        private void RewriteBranch(Func<Expression,Expression,Expression> cmp)
        {
            iclass = InstrClass.ConditionalTransfer;
            var left = RewriteOp(instr.Operands[0]);
            var right = RewriteOp(instr.Operands[1]);
            m.Branch(
                cmp(left, right), 
                (Address)RewriteOp(instr.Operands[2]),
                InstrClass.ConditionalTransfer);
        }

        private void RewriteBranchZ(Func<Expression, Expression> cmp0)
        {
            iclass = InstrClass.ConditionalTransfer;
            var src = RewriteOp(instr.Operands[0]);
            m.Branch(
                cmp0(src),
                (Address)RewriteOp(instr.Operands[1]),
                InstrClass.ConditionalTransfer);
        }

        private void RewriteCall0()
        {
            iclass = InstrClass.Transfer | InstrClass.Call;
            var dst = RewriteOp(instr.Operands[0]);
            var rDst = dst as Identifier;
            if (rDst is not null && rDst.Storage == Registers.a0)
            {
                var tmp = binder.CreateTemporary(rDst.DataType);
                m.Assign(tmp, dst);
                dst = tmp;
            }
            var cont = instr.Address + instr.Length;
            m.Assign(binder.EnsureRegister(Registers.a0), cont);
            m.Call(dst, 0);
        }

        private void RewriteCallW(int iReg)
        {
            void ShiftRegisters(int shift)
            {
                int iDst = 2;
                for (int i = 0; i < 6; ++i)
                {
                    var iSrc = iDst + i + shift;
                    if (iSrc < 16)
                    {
                        m.Assign(
                            binder.EnsureRegister(arch.GetAluRegister(iDst + i)),
                            binder.EnsureRegister(arch.GetAluRegister(iSrc)));
                    }
                }
            }

            void UnshiftRegisters(int shift)
            {
                int iDst = 2;
                for (int i = 5; i >= 0; --i)
                {
                    var iSrc = iDst + i + shift;
                    if (iSrc < 16)
                    {
                        m.Assign(
                          binder.EnsureRegister(arch.GetAluRegister(iSrc)),
                          binder.EnsureRegister(arch.GetAluRegister(iDst + i)));
                    }
                }
            }

            iclass = InstrClass.Transfer | InstrClass.Call;
            ShiftRegisters(iReg);
            var dst = RewriteOp(instr.Operands[0]);
            var rDst = dst as Identifier;
            if (rDst is not null && rDst.Storage == Registers.a0)
            {
                var tmp = binder.CreateTemporary(rDst.DataType);
                m.Assign(tmp, dst);
                dst = tmp;
            }
            var cont = instr.Address + instr.Length;
            m.Assign(binder.EnsureRegister(Registers.a0), cont);
            m.Call(dst, 0);
            UnshiftRegisters(iReg);
        }


        private void RewriteJ()
        {
            iclass = InstrClass.Transfer;
            m.Goto(RewriteOp(instr.Operands[0]));
        }

        private void RewriteLoop()
        {
            if (this.lend is not null)
            {
                // Nested LOOPxx instructions are not valid according to the
                // Xtensa instruction manual.
                iclass = InstrClass.Invalid;
                this.lbegin = null;
                this.lend = null;
                m.Invalid();
                return;
            }
            this.lbegin = instr.Address + instr.Length;
            this.lend = (Address) instr.Operands[1];
            var reg = RewriteOp(instr.Operands[0]);
            m.Assign(binder.EnsureRegister(Registers.LCOUNT), m.ISub(reg, 1));
        }

        private void RewriteRet()
        {
            iclass = InstrClass.Transfer | InstrClass.Return;
            m.Return(0, 0);
        }
    }
}