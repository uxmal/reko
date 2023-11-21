#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Operators
{
    public enum OperatorType
    {
        IAdd,
        ISub,
        USub,
        IMul,
        SMul,
        UMul,
        SDiv,
        UDiv,
        IMod,
        SMod,
        UMod,

        FAdd,
        FSub,
        FMul,
        FDiv,
        FMod,
        FNeg,

        And,
        Or,
        Xor,
        
        Shr,
        Sar,
        Shl,

        Cand,
        Cor,

        Lt,
        Gt,
        Le,
        Ge,

        Feq,
        Fne,
        Flt,
        Fgt,
        Fle,
        Fge,

        Ult,
        Ugt,
        Ule,
        Uge,

        Eq,
        Ne,

        Not,
        Neg,
        Comp,
        AddrOf,

        Comma,
    }

    public static class OperatorTypeExtensions
    {

        public static bool Commutes(this OperatorType op)
        {
            return op switch
            {
                Operators.OperatorType.IAdd or
                Operators.OperatorType.FAdd or
                Operators.OperatorType.And or
                Operators.OperatorType.SMul or
                Operators.OperatorType.UMul or
                Operators.OperatorType.IMul or
                Operators.OperatorType.FMul or
                Operators.OperatorType.Or or
                Operators.OperatorType.Xor => true,
                _ => false
            };
        }

        /// <summary>
        /// Returns whether the <see cref="OperatorType"/> is an integer
        /// addition or subtraction.
        /// </summary>
        public static bool IsAddOrSub(this OperatorType self)
        {
            return self == OperatorType.IAdd || self == OperatorType.ISub;
        }

        public static bool IsIntMultiplication(this OperatorType self)
        {
            return
               self == OperatorType.IMul ||
               self == OperatorType.SMul ||
               self == OperatorType.UMul;
        }

        public static bool IsShift(this OperatorType self)
        {
            return self == OperatorType.Shl || self == OperatorType.Shr || self == OperatorType.Sar;
        }

        public static bool IsIntComparison(this OperatorType type)
        {
            return type switch
            {
                OperatorType.Eq or OperatorType.Ne or
                OperatorType.Ge or OperatorType.Gt or
                OperatorType.Le or OperatorType.Lt or
                OperatorType.Uge or OperatorType.Ugt or
                OperatorType.Ule or OperatorType.Ult => true,
                _ => false
            };
        }

        public static bool IsFloatComparison(this OperatorType type)
        {
            return type switch
            {
                OperatorType.Feq or OperatorType.Fne or
                OperatorType.Fge or OperatorType.Fgt or
                OperatorType.Fle or OperatorType.Flt => true,
                _ => false
            };
        }
    }
}
