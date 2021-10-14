#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// This class is used to generate the <see cref="FunctionType"/> of an <see cref="IntrinsicProcedure"/>
    /// that models a CPU instruction's effect on architectural state.
    /// </summary>
    /// <remarks>
    /// The notion of <see cref="Storage"/> for the parameters and the return value of 
    /// an intrinsic function are meaningless, just as they are meaningless for the +, -,
    /// or other operators.
    /// </remarks>
    public class IntrinsicBuilder
    {
        private readonly string intrinsicName;
        private readonly bool hasSideEffect;
        private readonly ProcedureCharacteristics characteristics;
        private readonly List<Identifier> parameters;

        public IntrinsicBuilder(string intrinsicName, bool hasSideEffect) : this(intrinsicName, hasSideEffect, DefaultProcedureCharacteristics.Instance)
        {
        }

        public IntrinsicBuilder(string intrinsicName, bool hasSideEffect, ProcedureCharacteristics characteristics)
        {
            this.intrinsicName = intrinsicName;
            this.hasSideEffect = hasSideEffect;
            this.characteristics = characteristics;
            this.parameters = new List<Identifier>();
        }

        public IntrinsicBuilder Param(DataType dt)
        {
            var param = new Identifier($"p{parameters.Count + 1}", dt, null!);
            parameters.Add(param);
            return this;
        }

        public IntrinsicProcedure Void()
        {
            var signature = FunctionType.Action(parameters.ToArray());
            return new IntrinsicProcedure(intrinsicName, !hasSideEffect, signature);
        }

        public IntrinsicProcedure Returns(DataType dt)
        {
            var signature = FunctionType.Func(
                new Identifier("", dt, null!));
            return new IntrinsicProcedure(intrinsicName, !hasSideEffect, signature);
        }
    }
}