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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public class M68kState : ProcessorState
    {
        private M68kArchitecture arch;
        private uint[] values;
        private bool[] isValid;

        public M68kState(M68kArchitecture arch)
        {
            this.arch = arch;
            this.values = new uint[16];
            this.isValid = new bool[16];
        }

        public M68kState(M68kState orig) : base(orig)
        {
            this.arch = orig.arch;
            this.values = (uint[]) orig.values.Clone();
            this.isValid = (bool[]) orig.isValid.Clone();
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        #region ProcessorState Members

        public override ProcessorState Clone()
        {
            return new M68kState(this);
        }

        public override void SetRegister(RegisterStorage r, Constant v)
        {
            if (v != null && v.IsValid)
            {
                values[r.Number] = (uint) v.ToInt32();
                isValid[r.Number] = true;
            }
            else
            {
                isValid[r.Number] = false;
            }
        }

        public override void SetInstructionPointer(Address addr)
        {
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            if (isValid[r.Number])
            {
                return Constant.Create(r.DataType, values[r.Number]);
            }
            else
            {
                return Constant.Invalid;
            }
        }

        public override void OnProcedureEntered()
        {
        }

        public override void OnProcedureLeft(ProcedureSignature sig)
        {
        }

        public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
        {
            if (returnAddressSize > 0)
            {
                var spVal = GetValue(Registers.a7);
                SetValue(
                    arch.StackRegister,
                    new BinaryExpression(
                        Operator.ISub,
                        spVal.DataType,
                        stackReg,
                        Constant.Create(
                            PrimitiveType.CreateWord(returnAddressSize),
                            returnAddressSize)));
            }
            return new CallSite(returnAddressSize, 0);
        }

        public override void OnAfterCall(Identifier sp, ProcedureSignature sigCallee, ExpressionVisitor<Expression> eval)
        {
            var spReg = (RegisterStorage) sp.Storage;
            var spVal = GetValue(spReg);
            var stackOffset = SetValue(
                spReg,
                new BinaryExpression(
                    Operator.IAdd,
                    spVal.DataType,
                    sp,
                    Constant.Create(
                        PrimitiveType.CreateWord(spReg.DataType.Size),
                        sigCallee.StackDelta)).Accept(eval));
            if (stackOffset.IsValid)
            {
                if (stackOffset.ToInt32() > 0)
                    ErrorListener("Possible stack underflow detected.");
            }
        }

        #endregion
    }
}
