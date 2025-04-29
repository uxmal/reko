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

using Reko.Core.Expressions;
using Reko.Core.Operators;
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
        private readonly IFunctionalUnit? simdOp;
        private readonly ProcedureCharacteristics characteristics;
        private readonly List<Identifier> parameters;
        private Func<DataType, Constant[], Constant?>? applyConstants;
        private DataType[]? genericTypes;
        private Dictionary<string, DataType>? genericTypeDictionary;
        private bool isVariadic;

        /// <summary>
        /// Construct an instance of <see cref="IntrinsicBuilder"/>.
        /// </summary>
        /// <param name="intrinsicName">The name of the instrinsic function being built.</param>
        /// <param name="hasSideEffect">True if the function has side effects.</param>
        public IntrinsicBuilder(string intrinsicName, bool hasSideEffect) : this(intrinsicName, hasSideEffect, DefaultProcedureCharacteristics.Instance)
        {
        }

        /// <summary>
        /// Construct an instance of <see cref="IntrinsicBuilder"/>.
        /// </summary>
        /// <param name="intrinsicName">The name of the instrinsic function being built.</param>
        /// <param name="op">The <see cref="IFunctionalUnit"/> that implements the intrinsic.</param>
        /// <param name="characteristics">Optional procedure characteristics.</param>
        public IntrinsicBuilder(string intrinsicName, IFunctionalUnit op, ProcedureCharacteristics? characteristics = null)
        {
            this.intrinsicName = intrinsicName;
            this.simdOp = op;
            this.hasSideEffect = false;
            this.characteristics = characteristics ?? DefaultProcedureCharacteristics.Instance;
            this.parameters = [];
        }

        /// <summary>
        /// Construct an instance of <see cref="IntrinsicBuilder"/>.
        /// </summary>
        /// <param name="intrinsicName">The name of the instrinsic function being built.</param>
        /// <param name="hasSideEffect">True if the function has side effects.</param>
        /// <param name="characteristics">Optional procedure characteristics.</param>
        public IntrinsicBuilder(string intrinsicName, bool hasSideEffect, ProcedureCharacteristics characteristics)
        {
            this.intrinsicName = intrinsicName;
            this.hasSideEffect = hasSideEffect;
            this.characteristics = characteristics;
            this.parameters = [];
        }

        /// <summary>
        /// Add an evaluator function to the intrinsic.
        /// </summary>
        public IntrinsicBuilder ApplyConstants(Func<DataType, Constant[], Constant?> fn)
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

        /// <summary>
        /// Creates a out parameter of type <paramref name="dt" />.
        /// </summary>
        /// <param name="dt">Type of the out parameter.</param>
        public IntrinsicBuilder OutParam(DataType dt)
        {
            var param = new Identifier($"p{parameters.Count + 1}", dt, null!);
            parameters.Add(param);
            return this;
        }

        /// <summary>
        /// Creates a generic out parameter.
        /// </summary>
        /// <param name="genericType">Generic type.</param>
        public IntrinsicBuilder OutParam(string genericType)
        {
            return OutParam(GetGenericArgument(genericType));
        }

        /// <summary>
        /// Adds a parameter to the intrinsic function.
        /// </summary>
        /// <param name="dt">Data type of the parameter.</param>
        public IntrinsicBuilder Param(DataType dt)
        {
            var param = new Identifier($"p{parameters.Count + 1}", dt, null!);
            parameters.Add(param);
            return this;
        }

        /// <summary>
        /// Adds a generic parameter to the intrinsic function.
        /// </summary>
        /// <param name="genericType">Generic type.</param>
        public IntrinsicBuilder Param(string genericType)
        {
            return Param(GetGenericArgument(genericType));
        }

        /// <summary>
        /// Creates a parameter of type "pointer to <paramref name="dt" />.
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

        /// <summary>
        /// Adds a list of generic types to the intrinsic function.
        /// </summary>
        /// <param name="genericTypes">Generic types.</param>
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
            signature.IsVariadic = isVariadic;
            return MakeIntrinsic(signature);
        }

        /// <summary>
        /// Creates a return value of type <paramref name="dt" />.
        /// </summary>
        /// <param name="dt">Return type.</param>
        public IntrinsicProcedure Returns(DataType dt)
        {
            var signature = FunctionType.CreateUserDefined(
                new Identifier("", dt, null!),
                parameters.ToArray());
            signature.IsVariadic = isVariadic;
            return MakeIntrinsic(signature);
        }

        /// <summary>
        /// Creates a return value of generic type type <paramref name="genericType" />.
        /// </summary>
        /// <param name="genericType">Generic return type.</param>
        public IntrinsicProcedure Returns(string genericType)
        {
            var signature = FunctionType.CreateUserDefined(
                new Identifier("", GetGenericArgument(genericType), null!),
                parameters.ToArray());
            return MakeIntrinsic(signature);
        }

        /// <summary>
        /// Creates a return value of type pointer to <paramref name="dt" />.
        /// </summary>
        public IntrinsicProcedure PtrReturns(DataType dt)
        {
            // The '0' size below indicates that we don't know the size of the pointer.
            // When IntrinsicProcedure.MakeInstance is called, the pointer
            // size of the architecture is resolved.
            return Returns(new Pointer(dt, 0));
        }

        /// <summary>
        /// Mark the procedure as variadic.
        /// </summary>
        /// <returns></returns>
        public IntrinsicBuilder Variadic()
        {
            this.isVariadic = true;
            return this;
        }

        private IntrinsicProcedure MakeIntrinsic(FunctionType signature)
        {
            IntrinsicProcedure proc;
            if (this.genericTypes is not null)
            {
                if (this.simdOp is not null)
                {
                    proc = new SimdIntrinsic(intrinsicName, this.simdOp, genericTypes, false, signature);
                }
                else
                {
                    proc = new IntrinsicProcedure(intrinsicName, genericTypes, false, hasSideEffect, this.applyConstants, signature);
                }
            }
            else
            {
                proc = new IntrinsicProcedure(intrinsicName, hasSideEffect, signature);
            }
            proc.Characteristics = characteristics;
            return proc;
        }

        /// <summary>
        /// Creates an intrinsic function with no side effects.
        /// </summary>
        /// <param name="name">Name of the intrinsic procedure.</param>
        public static IntrinsicBuilder Pure(string name)
        {
            return new IntrinsicBuilder(name, false);
        }

        /// <summary>
        /// Creates an intrinsic function with side effects.
        /// </summary>
        /// <param name="name">Name of the intrinsic procedure.</param>
        public static IntrinsicBuilder SideEffect(string name)
        {
            return new IntrinsicBuilder(name, true);
        }

        /// <summary>
        /// Creates an intrinsic ternary function with no side effects.
        /// </summary>
        /// <param name="name">Name of the intrinsic procedure.</param>
        public static IntrinsicProcedure GenericTernary(string name)
        {
            return new IntrinsicBuilder(name, false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Param("T")
                .Returns("T");
        }

        /// <summary>
        /// Creates an intrinsic ternary function with no side effects.
        /// </summary>
        /// <param name="name">Name of the intrinsic procedure.</param>
        /// <param name="operation">Function to apply to the constants.</param>
        public static IntrinsicProcedure GenericTernary(string name, Func<DataType, Constant[], Constant?> operation)
        {
            return new IntrinsicBuilder(name, false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Param("T")
                .ApplyConstants(operation)
                .Returns("T");
        }

        /// <summary>
        /// Creates an intrinsic binary function with no side effects.
        /// </summary>
        /// <param name="name">Name of the intrinsic procedure.</param>
        public static IntrinsicProcedure GenericBinary(string name)
        {
            return new IntrinsicBuilder(name, false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Returns("T");
        }

        /// <summary>
        /// Creates an intrinsic binary function with no side effects.
        /// </summary>
        /// <param name="name">Name of the intrinsic procedure.</param>
        /// <param name="eval">Function to apply to the constants.</param>
        public static IntrinsicProcedure GenericBinary(string name, Func<DataType, Constant[], Constant> eval)
        {
            return new IntrinsicBuilder(name, false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .ApplyConstants(eval)
                .Returns("T");
        }

        /// <summary>
        /// Creates an intrinsic binary SIMD function with no side effects.
        /// </summary>
        /// <param name="name">Name of the intrinsic procedure.</param>
        /// <param name="op">Function to apply to the constants.</param>
        public static IntrinsicProcedure SimdBinary(string name, IFunctionalUnit op)
        {
            return new IntrinsicBuilder(name, op)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Returns("T");
        }

        /// <summary>
        /// Creates an intrinsic binary SIMD function with no side effects.
        /// </summary>
        /// <param name="name">Name of the intrinsic procedure.</param>
        /// <param name="intrinsic">Function to apply to the constants.</param>
        public static IntrinsicProcedure SimdBinary(string name, IntrinsicProcedure intrinsic)
        {
            return new IntrinsicBuilder(name, intrinsic)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Returns("T");
        }

        /// <summary>
        /// Creates an intrinsic ternary SIMD function with no side effects.
        /// </summary>
        /// <param name="name">Name of the intrinsic procedure.</param>
        /// <param name="intrinsic">Function to apply to the constants.</param>
        public static IntrinsicProcedure SimdTernary(string name, IntrinsicProcedure intrinsic)
        {
            return new IntrinsicBuilder(name, intrinsic)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Param("T")
                .Returns("T");
        }

        /// <summary>
        /// Creates an intrinsic unary SIMD function with no side effects.
        /// </summary>
        /// <param name="name">Name of the intrinsic procedure.</param>
        /// <param name="fn">Function to apply to the constants.</param>
        public static IntrinsicProcedure SimdUnary(string name, IFunctionalUnit fn)
        {
            return new IntrinsicBuilder(name, fn)
                .GenericTypes("T")
                .Param("T")
                .Returns("T");
        }

        /// <summary>
        /// Creates a generic binary function.
        /// </summary>
        /// <param name="name">Name of the intrinsic procedure.</param>
        /// <param name="hasSideEffect">True if the intrinsic has a side effect.</param>
        public static IntrinsicProcedure GenericBinary(string name, bool hasSideEffect)
        {
            return new IntrinsicBuilder(name, hasSideEffect)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Returns("T");
        }

        /// <summary>
        /// Creates a generic unary function.
        /// </summary>
        /// <param name="name">Name of the intrinsic procedure.</param>
        public static IntrinsicProcedure GenericUnary(string name)
        {
            return new IntrinsicBuilder(name, false)
                .GenericTypes("T")
                .Param("T")
                .Returns("T");
        }

        /// <summary>
        /// Creates an intrinsic unary function with no side effects.
        /// </summary>
        /// <param name="name">Name of the intrinsic procedure.</param>
        /// <param name="eval">Function to apply to the constants.</param>
        public static IntrinsicProcedure GenericUnary(string name, Func<DataType, Constant[], Constant> eval)
        {
            return new IntrinsicBuilder(name, false)
                .GenericTypes("T")
                .Param("T")
                .ApplyConstants(eval)
                .Returns("T");
        }

        /// <summary>
        /// Creates an intrinsic binary function with no side effects.
        /// </summary>
        /// <param name="intrinsicName">Name of the intrinsic procedure.</param>
        /// <param name="dt">Data type of input parameters and return value.</param>
        public static IntrinsicProcedure Binary(string intrinsicName, DataType dt)
        {
            return new IntrinsicBuilder(intrinsicName, false).Param(dt).Param(dt).Returns(dt);
        }

        /// <summary>
        /// Creates an intrinsic ternary function with no side effects.
        /// </summary>
        /// <param name="intrinsicName">Name of the intrinsic procedure.</param>
        /// <param name="dt">Data type of input parameters and return value.</param>
        public static IntrinsicProcedure Ternary(string intrinsicName, DataType dt)
        {
            return new IntrinsicBuilder(intrinsicName, false)
                .Param(dt)
                .Param(dt)
                .Param(dt)
                .Returns(dt);
        }

        /// <summary>
        /// Creates an intrinsic unary function with no side effects.
        /// </summary>
        /// <param name="intrinsicName">Name of the intrinsic procedure.</param>
        /// <param name="dt">Data type of input parameter and return value.</param>
        public static IntrinsicProcedure Unary(string intrinsicName, PrimitiveType dt)
        {
            return new IntrinsicBuilder(intrinsicName, false).Param(dt).Returns(dt);
        }

        /// <summary>
        /// Creates an intrinsic function with no side effects and a boolean return value.
        /// </summary>
        /// <param name="intrinsicName">Name of the intrinsic procedure.</param>
        /// <param name="dataTypes">Data type of input parameter and return value.</param>
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