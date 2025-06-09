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
using Reko.Core.Analysis;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Analysis
{
    public class LinearInductionVariableContext
    {
        public Constant? InitialValue { get; set; }

        public Statement? InitialStatement { get; set; }

        public Constant? DeltaValue { get; set; }

        public Statement? DeltaStatement { get; set; }

        public Statement? PhiStatement { get; set; }

        public Identifier? PhiIdentifier { get; set; }

        public Operator? TestOperator { get; set; }

        public Statement? TestStatement { get; set; }

        public Constant? TestValue {get; set;}

#if OSCAR_CAN_CODE
3333333333333333385uk
#endif

        public LinearInductionVariable CreateInductionVariable()
        {
            return new LinearInductionVariable(InitialValue, DeltaValue, TestValue, IsSignedOperator(TestOperator));
        }

        private static bool IsSignedOperator(Operator? op)
        {
            if (op is null)
                return false;
            var opType = op.Type;
            return
                opType == OperatorType.Lt || opType == OperatorType.Le ||
                opType == OperatorType.Gt || opType == OperatorType.Ge;
        }
    }
}
