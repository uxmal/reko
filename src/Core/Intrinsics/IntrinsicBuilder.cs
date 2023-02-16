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

using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Intrinsics
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
        private Func<Constant[], Constant>? applyConstants;
        private DataType[]? genericTypes;
        private Dictionary<string, DataType>? genericTypeDictionary;

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

        /// <summary>
        /// Add an evaluator function to the intrinsic.
        /// </summary>
        public IntrinsicBuilder ApplyConstants(Func<Constant[], Constant> fn)
        {
            this.applyConstants = fn;
            return this;
        }

        /// <summary>
        /// Define some types as generic.
        /// </summary>
        /// <param name="typenames">Names of the generic types.</param>
        /// <returns></returns>
        public IntrinsicBuilder GenericTypes(params string[] typenames)
        {
            genericTypeDictionary = new Dictionary<string, DataType>();
            var types = new List<DataType>();
            foreach (var typename in typenames)
            {
                DataType dt = new TypeReference(typename);
                types.Add(dt);
                genericTypeDictionary.Add(typename, dt);
            }
            this.genericTypes = types.ToArray();
            return this;
        }

        public IntrinsicBuilder OutParam(DataType dt)
        {
            var param = new Identifier($"p{parameters.Count + 1}", dt, null!);
            parameters.Add(param);
            return this;
        }

        public IntrinsicBuilder OutParam(string genericType)
        {
            return OutParam(GetGenericArgument(genericType));
        }

        public IntrinsicBuilder Param(DataType dt)
        {
            var param = new Identifier($"p{parameters.Count + 1}", dt, null!);
            parameters.Add(param);
            return this;
        }

        public IntrinsicBuilder Param(string genericType)
        {
            return Param(GetGenericArgument(genericType));
        }

        /// <summary>
        /// Creates a parameter of type "pointer to <paramref name="genericType" />.
        /// </summary>
        public IntrinsicBuilder PtrParam(DataType dt)
        {
            // The '0' size below indicates that we don't know the size of the pointer.
            // When IntrinsicProcedure.MakeInstance is called, the pointer
            // size of the architecture is resolved.
            return Param(new Pointer(dt, 0));
        }

        /// <summary>
        /// Creates a parameter of type "pointer to <paramref name="genericType" />.
        /// </summary>
        public IntrinsicBuilder PtrParam(string genericType)
        {
            // The '0' size below indicates that we don't know the size of the pointer.
            // When IntrinsicProcedure.MakeInstance is called, the pointer
            // size of the architecture is resolved.
            return Param(new Pointer(GetGenericArgument(genericType), 0));
        }

        public IntrinsicBuilder Params(params string[] genericTypes)
        {
            foreach (var type in genericTypes)
                Param(GetGenericArgument(type));
            return this;
        }

        private DataType GetGenericArgument(string genericType)
        {
            if (genericTypeDictionary is null)
                throw new InvalidOperationException("No generic types were specified.");
            if (!genericTypeDictionary.TryGetValue(genericType, out var dt))
                throw new InvalidOperationException($"Unknown generic type '{genericType}'.");
            return dt;
        }

        /// <summary>
        /// Completes a declaration of an <see cref="IntrinsicProcedure"/> by stating
        /// that it returns no value, i.e. is used for its side effects only.
        /// </summary>
        /// <returns>Instance of <see cref="IntrinsicProcedure"/> ready to be used.</returns>
        public IntrinsicProcedure Void()
        {
            var signature = FunctionType.Action(parameters.ToArray());
            return MakeIntrinsic(signature);
        }

        public IntrinsicProcedure Returns(DataType dt)
        {
            var signature = FunctionType.Func(
                new Identifier("", dt, null!),
                parameters.ToArray());
            return MakeIntrinsic(signature);
        }

        public IntrinsicProcedure Returns(string genericType)
        {
            var signature = FunctionType.Func(
                new Identifier("", GetGenericArgument(genericType), null!),
                parameters.ToArray());
            return MakeIntrinsic(signature);
        }

        private IntrinsicProcedure MakeIntrinsic(FunctionType signature)
        {
            IntrinsicProcedure proc;
            if (this.genericTypes is not null)
            {
                proc = new IntrinsicProcedure(intrinsicName, genericTypes, false, hasSideEffect, signature);
            }
            else
            {
                proc = new IntrinsicProcedure(intrinsicName, hasSideEffect, signature);
            }
            proc.Characteristics = characteristics;
            proc.ApplyConstants = this.applyConstants;
            return proc;
        }

        public static IntrinsicBuilder Pure(string name)
        {
            return new IntrinsicBuilder(name, false);
        }

        public static IntrinsicBuilder SideEffect(string name)
        {
            return new IntrinsicBuilder(name, true);
        }

        public static IntrinsicProcedure GenericTernary(string name)
        {
            return new IntrinsicBuilder(name, false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Param("T")
                .Returns("T");
        }

        public static IntrinsicProcedure GenericBinary(string name)
        {
            return new IntrinsicBuilder(name, false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Returns("T");
        }

        public static IntrinsicProcedure GenericBinary(string name, bool hasSideEffect)
        {
            return new IntrinsicBuilder(name, hasSideEffect)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Returns("T");
        }

        public static IntrinsicProcedure GenericUnary(string name)
        {
            return new IntrinsicBuilder(name, false)
                .GenericTypes("T")
                .Param("T")
                .Returns("T");
        }

        public static IntrinsicProcedure Binary(string intrinsicName, DataType dt)
        {
            return new IntrinsicBuilder(intrinsicName, false).Param(dt).Param(dt).Returns(dt);
        }

        public static IntrinsicProcedure Ternary(string intrinsicName, DataType dt)
        {
            return new IntrinsicBuilder(intrinsicName, false)
                .Param(dt)
                .Param(dt)
                .Param(dt)
                .Returns(dt);
        }

        public static IntrinsicProcedure Unary(string intrinsicName, PrimitiveType dt)
        {
            return new IntrinsicBuilder(intrinsicName, false).Param(dt).Returns(dt);
        }

        public static IntrinsicProcedure Predicate(string intrinsicName, params DataType[] dataTypes)
        {
            var b = new IntrinsicBuilder(intrinsicName, false);
            foreach (var dt in dataTypes)
            {
                b.Param(dt);
            }
            return b.Returns(PrimitiveType.Bool);
        }
    }
}