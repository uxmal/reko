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
using Reko.Core.Intrinsics;
using Reko.Core.Operators;
using System.Diagnostics;

namespace Reko.Evaluation
{
    public partial class ExpressionSimplifier
    {
        public virtual (Expression, bool) VisitApplication(Application appl)
        {
            bool changed = false;
            var args = new Expression[appl.Arguments.Length];
            var constArgs = new Constant[appl.Arguments.Length];
            bool allConst = true;
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                var (arg, argChanged) = appl.Arguments[i].Accept(this);
                args[i] = arg;
                if (arg is Constant c)
                {
                    constArgs[i] = c;
                }
                else
                {
                    allConst = false;
                }
                changed |= argChanged;
            }

            if (appl.Procedure is ProcedureConstant pc &&
                pc.Procedure is IntrinsicProcedure intrinsic)
            {
                if (allConst)
                {
                    var cResult = intrinsic.ApplyConstants(appl.DataType, constArgs);
                    if (cResult is not null)
                        return (cResult, true);
                }

                // Rotations-with-carries that rotate in a false carry 
                // flag can be simplified to shifts.

                if (intrinsic.Name == CommonOps.RolC.Name)
                {
                    if (IsSingleBitRotationWithClearCarryIn(args))
                    {
                        return (m.Bin(Operator.Shl, appl.DataType, args[0], args[1]), true);
                    }
                }
                else if (intrinsic.Name == CommonOps.RorC.Name)
                {
                    if (IsSingleBitRotationWithClearCarryIn(args))
                    {
                        return (m.Bin(Operator.Shr, appl.DataType, args[0], args[1]), true);
                    }
                }
                else if (intrinsic.Name == CommonOps.Rol.Name)
                {
                    var rol = CombineRotations(intrinsic.Name, appl, args);
                    if (rol is not null)
                    {
                        return (rol, true);
                    }
                }
                else if (intrinsic.Name == CommonOps.Ror.Name)
                {
                    var ror = CombineRotations(intrinsic.Name, appl, args);
                    if (ror is not null)
                    {
                        return (ror, true);
                    }
                }
            }
            var (proc, procChanged) = appl.Procedure.Accept(this);
            changed |= procChanged;
            if (changed)
            {
                appl = new Application(
                    proc,
                    appl.DataType,
                    args);
            }
            var newAppl = ctx.GetValue(appl);
            if (newAppl != appl)
            {
                return (newAppl, true);
            }
            else
            {
                return (appl, changed);
            }
        }

        private Expression? CombineRotations(string rotationName, Application appl, Expression[] args)
        {
            if (args[1] is Constant cOuter &&
                args[0] is Application appInner &&
                appInner.Procedure is ProcedureConstant pcInner &&
                pcInner.Procedure is IntrinsicProcedure intrinsicInner)
            {
                if (intrinsicInner.Name == rotationName)
                {
                    if (appInner.Arguments[1] is Constant cInner)
                    {
                        var cTot = Operator.IAdd.ApplyConstants(cOuter.DataType, cOuter, cInner);
                        return new Application(
                            appl.Procedure,
                            appl.DataType,
                            appInner.Arguments[0],
                            cTot);
                    }
                }
            }
            return null;
        }

        private static bool IsSingleBitRotationWithClearCarryIn(Expression[] args)
        {
            Debug.Assert(args.Length == 3);
            return args[1] is Constant sh &&
                sh.IsIntegerOne &&
                args[2] is Constant c &&
                c.IsIntegerZero;
        }

    }
}
