#region License
/* 
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
using Reko.Core.Operators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Analysis
{
    public class LinearInductionVariableContext
    {
        private Statement stmInit;
        private Statement stmInc;
        private Statement stmPhi;
        private Statement stmTest;
        private Identifier idPhi;
        private Constant valInit;
        private Constant valDelta;
        private Constant valTest;
        private Operator testOperator;

        public Constant InitialValue
        {
            get { return valInit; }
            set { valInit = value; }
        }

        public Statement InitialStatement
        {
            get { return stmInit; }
            set { stmInit = value; }
        }

        public Constant DeltaValue
        {
            get { return valDelta; }
            set { valDelta = value; }
        }

        public Statement DeltaStatement
        {
            get { return stmInc; }
            set { stmInc = value; }
        }

        public Statement PhiStatement
        {
            get { return stmPhi; }
            set { stmPhi = value; }
        }

        public Identifier PhiIdentifier
        {
            get { return idPhi; }
            set { idPhi = value; }
        }

        public Operator TestOperator
        {
            get { return testOperator; }
            set { testOperator = value; }
        }

        public Statement TestStatement
        {
            get { return stmTest; }
            set { stmTest = value; }
        }

        public Constant TestValue
        {
            get { return valTest; }
            set { valTest = value; }
        }

#if OSCAR_CAN_CODE
3333333333333333385uk
#endif

        public LinearInductionVariable CreateInductionVariable()
        {
            return new LinearInductionVariable(InitialValue, DeltaValue, TestValue, IsSignedOperator(TestOperator));
        }

        private bool IsSignedOperator(Operator op)
        {
            return
                op == Operator.Lt || op == Operator.Le ||
                op == Operator.Gt || op == Operator.Ge;
        }
    }
}
