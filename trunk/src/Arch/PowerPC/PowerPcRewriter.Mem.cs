#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public partial class PowerPcRewriter
    {
        private void RewriteLbz()
        {
            var op1 = RewriteOperand(instr.op1);
            var ea = EffectiveAddress_r0(dasm.Current.op2, emitter);
            emitter.Assign(op1, emitter.LoadB(ea));
        }

        private void RewriteLfd()
        {
            var op1 = RewriteOperand(instr.op1);
            var ea = EffectiveAddress(dasm.Current.op2, emitter);
            emitter.Assign(op1, emitter.Load(PrimitiveType.Real64, ea));
        }

        private void RewriteLfs()
        {
            var op1 = RewriteOperand(instr.op1);
            var ea = EffectiveAddress(dasm.Current.op2, emitter);
            emitter.Assign(op1, emitter.Cast(PrimitiveType.Real64,
                emitter.Load(PrimitiveType.Real32, ea)));
        }

        private void RewriteLhz()
        {
            var op1 = RewriteOperand(instr.op1);
            var ea = EffectiveAddress_r0(dasm.Current.op2, emitter);
            emitter.Assign(op1, emitter.LoadW(ea));
        }

        private void RewriteLwz()
        {
            var op1 = RewriteOperand(instr.op1);
            var ea = EffectiveAddress_r0(dasm.Current.op2, emitter);
            emitter.Assign(op1, emitter.LoadDw(ea));
        }

        private void RewriteLwzx()
        {
            var op1 = RewriteOperand(instr.op1);
            var a = RewriteOperand(instr.op2, true);
            var b = RewriteOperand(instr.op3);
            var ea = (a.IsZero)
                ? b
                : emitter.IAdd(a, b);
            emitter.Assign(op1, emitter.LoadDw(ea));
        }

        private void RewriteStb()
        {
            var s = RewriteOperand(instr.op1);
            var ea = EffectiveAddress_r0(instr.op2, emitter);
            emitter.Assign(emitter.LoadB(ea), s);
        }

        private void RewriteStbx()
        {
            var s = RewriteOperand(instr.op1);
            var a = RewriteOperand(instr.op2, true);
            var b = RewriteOperand(instr.op3);
            var ea = (a.IsZero)
                ? b
                : emitter.IAdd(a, b);
            emitter.Assign(emitter.LoadB(ea), s);
        }

        private void RewriteStfd()
        {
            var s = RewriteOperand(instr.op1);
            var ea = EffectiveAddress_r0(instr.op2, emitter);
            emitter.Assign(emitter.Load(PrimitiveType.Real64, ea), s);
        }

        private void RewriteStw()
        {
            var s = RewriteOperand(instr.op1);
            var ea = EffectiveAddress_r0(instr.op2, emitter);
            emitter.Assign(emitter.LoadDw(ea), s);
        }

        private void RewriteStwx()
        {
            var s = RewriteOperand(instr.op1);
            var a = RewriteOperand(instr.op2, true);
            var b = RewriteOperand(instr.op3);
            var ea = (a.IsZero)
                ? b
                : emitter.IAdd(a, b);
            emitter.Assign(emitter.LoadDw(ea), s);
        }


        private void RewriteStwu()
        {
            var s = RewriteOperand(instr.op1);
            var ea = EffectiveAddress(instr.op2, emitter);
            emitter.Assign(s, emitter.LoadDw(ea));
            emitter.Assign(((BinaryExpression)ea).Left, EffectiveAddress(instr.op2, emitter));
        }

        private void RewriteStwux()
        {
            var s = RewriteOperand(instr.op1);
            var a = RewriteOperand(instr.op2);
            var b = RewriteOperand(instr.op3);
            emitter.Assign(emitter.LoadDw(emitter.IAdd(a, b)), s);
            emitter.Assign(a, emitter.IAdd(a, b));
        }
    }
}
