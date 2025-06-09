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

using System;

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// Evaluates a C enum value.
    /// </summary>
    public class EnumEvaluator
    {
        private int value;
        private readonly CConstantEvaluator ceval;

        /// <summary>
        /// Constructs an <see cref="EnumEvaluator"/> instance.
        /// </summary>
        /// <param name="ceval">Constant evaluator.</param>
        public EnumEvaluator(CConstantEvaluator ceval)
        {
            this.value = 0;
            this.ceval = ceval;
        }

        /// <summary>
        /// Evaluates an expression to a value.
        /// </summary>
        /// <param name="cExpression"></param>
        /// <returns></returns>
        public int GetValue(CExpression? cExpression)
        {
            if (cExpression is not null)
            {
                value = Convert.ToInt32(cExpression.Accept(ceval));
            }
            return value++;
        }
    }
}
