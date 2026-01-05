#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Reko.Core
{
    /// <summary>
    /// An intrinsic procedure represents predefined functions or processor instructions that don't have a 
    /// C/C++ equivalent (like rotate operations).
    /// </summary>
    public class IntrinsicProcedure : ProcedureBase, IFunctionalUnit
    {
        /// <summary>
        /// To avoid generating massive amounts of instances of generic intrinsics,
        /// we use a cache to store them.
        /// </summary>
        /// //$REVIEW: perhaps this is premature optimization? Intrinsics aren't _that_
        /// common, but SIMD-heavy programs will have more of them.
        private static readonly ConcurrentDictionary<(IntrinsicProcedure, DataType[]), IntrinsicProcedure> instanceCache = 
            new(new InstanceCacheComparer());

		private readonly int arity;
		private readonly FunctionType? sig;

        /// <summary>
        /// Well-known operations that many processors support but most high- or 
        /// medium level languages do not support.
        /// </summary>
        public const string Syscall = "__syscall";  // Invokes a system call.

#pragma warning disable CS1591
        // MIPS-style unaligned memory accesses
        public const string LwL = "__lwl";
        public const string LwR = "__lwr";
        public const string SwL = "__swl";
        public const string SwR = "__swr";

        public const string Align = "__align";
#pragma warning restore CS1591

        /// <summary>
        /// Use this constructor for intrinsics that model operations that may have parameters of varying sizes.
        /// </summary>
        /// <remarks>
        /// E.g. the rotate intrinsic instructions.</remarks>
        /// <param name="name">The name of the intrinsic procedure.</param>
        /// <param name="hasSideEffect">True if the intrinsic procedure has a side effect
        /// that would prevent it from being optimized away.</param>
        /// <param name="returnType">The return type of this intrinsic.</param>
        /// <param name="arity">The number of arguments accepted by this instrinsic.</param>
        /// <param name="eval">Optional evaluation function.</param>
		public IntrinsicProcedure(
            string name, 
            bool hasSideEffect,
            DataType returnType, 
            int arity,
            Func<DataType, Constant[], Constant?>? eval = null)
            : base(name, hasSideEffect)
		{
            this.ReturnType = returnType;
			this.arity = arity;
            this.Evaluate = eval ?? FailEvaluator;
		}

        /// <summary>
        /// Creates an <see cref="IntrinsicProcedure"/> with a specific signature.
        /// </summary>
        /// <param name="name">The name of the intrinsic procedure.</param>
        /// <param name="hasSideEffect">True of the procedure has a side effect (<see cref="ProcedureBase.HasSideEffect"/></param>
        /// <param name="sig">The signature of the procedure.</param>
        /// <param name="eval">Optional evaluation function.</param>
		public IntrinsicProcedure(
            string name,
            bool hasSideEffect,
            FunctionType sig,
            Func<DataType, Constant[], Constant?>? eval = null)
            : base(name, hasSideEffect)
		{
			this.sig = sig;
            this.ReturnType = sig.ReturnValue?.DataType!;
            this.Evaluate = eval ?? FailEvaluator;
		}

        /// <summary>
        /// Creates an <see cref="IntrinsicProcedure"/> with a specific signature, with one
        /// or more generic arguments.
        /// </summary>
        /// <param name="name">The name of the intrinsic procedure.</param>
        /// <param name="genericTypes">The generic types of this procedure.</param>
        /// <param name="isConcrete">True if this is an instance of the generic intrinsic.</param>
        /// <param name="hasSideEffect">True of the procedure has a side effect (<see cref="ProcedureBase.HasSideEffect"/></param>
        /// <param name="evaluator">Optional partial evaluation procedure.</param>
        /// <param name="sig">The signature of the procedure.</param>
        public IntrinsicProcedure(
            string name, 
            DataType[] genericTypes, 
            bool isConcrete,
            bool hasSideEffect,
            Func<DataType, Constant[], Constant?>? evaluator,
            FunctionType sig)
            : base(name, genericTypes, isConcrete, hasSideEffect)
        {
            this.sig = sig;
            this.ReturnType = sig.ReturnValue?.DataType!;
            this.Evaluate = evaluator ?? FailEvaluator;
        }

        /// <summary>
        /// Optional function used to evaluate constants.
        /// </summary>
        private Func<DataType, Constant[], Constant?> Evaluate { get; }


        /// <summary>
        /// The number of arguments expected by this intrinsic procedure.
        /// </summary>
        public int Arity
		{
			get 
            {
                return sig?.Parameters?.Length ?? arity;
            }
		}

        /// <summary>
        /// The return type of this intrinsic procedure.
        /// </summary>
        public DataType ReturnType { get; }

        /// <summary>
        /// The signature of this intrinsic procedure.
        /// </summary>
		public override FunctionType Signature
		{
			get { return sig!; }
			set { throw new InvalidOperationException("Changing the signature of an IntrinsicProcedure is not allowed."); }
		}

        /// <summary>
        /// Creates an <see cref="Application"/> expression that calls this intrinsic procedure.
        /// </summary>
        /// <param name="dt">Datatype of the result.</param>
        /// <param name="exprs">Argument values.</param>
        /// <returns>An <see cref="Application"/> instance invoking this intrinsic.</returns>
        public virtual Expression Create(DataType dt, params Expression[] exprs)
        {
            return new Application(new ProcedureConstant(PrimitiveType.Ptr32, this), dt, exprs);
        }

        /// <inheritdoc/>
        public virtual Constant? ApplyConstants(DataType dt, params Constant[] cs)
        {
            return this.Evaluate(dt, cs);
        }

        /// <summary>
        /// Makes a concrete instance of this <see cref="IntrinsicProcedure"/> instance, using
        /// the provided <paramref name="concreteTypes" />.
        /// </summary>
        /// <param name="ptrBitsize">The bit size of a pointer in the current architecture.</param>
        /// <param name="concreteTypes">Concrete types to use.</param>
        /// <returns>A newly minted or previously cached concrete instance.
        /// </returns>
        public IntrinsicProcedure MakeInstance(int ptrBitsize, params DataType[] concreteTypes)
        {
            if (this.IsConcreteGeneric)
                throw new InvalidOperationException($"The intrinsic {this} is already a concrete instance.");
            var key = (this, concreteTypes);
            IntrinsicProcedure? instance;
            while (!instanceCache.TryGetValue(key, out instance))
            {
                var sig = base.MakeConcreteSignature(ptrBitsize, concreteTypes);
                instance = DoMakeInstance(concreteTypes, sig);
                if (instanceCache.TryAdd(key, instance))
                    break;
            }
            return instance;
        }

        /// <summary>
        /// Makes a concrete instance of this <see cref="IntrinsicProcedure"/> instance, using
        /// the provided <paramref name="concreteTypes" />.
        /// </summary>
        /// <param name="concreteTypes">Concrete types to use as type parameters.</param>
        /// <param name="sig">The signature of the concrete instance.</param>
        /// <returns>A newly minted concrete instance.
        /// </returns>
        protected virtual IntrinsicProcedure DoMakeInstance(DataType[] concreteTypes, FunctionType sig)
        {
            return new IntrinsicProcedure(this.Name, concreteTypes, true, this.HasSideEffect, Evaluate, sig)
            {
                Characteristics = this.Characteristics,
                EnclosingType = this.EnclosingType
            };
        }

        /// <summary>
        /// Creates a concrete instance of this <see cref="IntrinsicProcedure"/> instance, using
        /// the given concrete types.
        /// </summary>
        /// <param name="concreteTypes">Concrete types to use as type parameters.</param>
        /// <returns>A concrete instance of the intrinsic procedure.</returns>
        public IntrinsicProcedure MakeInstance(params DataType[] concreteTypes)
            => MakeInstance(0, concreteTypes);

        /// <summary>
        /// Resolves any 0-sized pointers, which are used to indicate pointers
        /// of unknown size.
        /// </summary>
        /// <param name="ptrSize">Size of pointers</param>
        /// <returns>A newly instance of <see cref="IntrinsicProcedure"/> with
        /// replaced pointers of unknown size to
        /// <paramref name="ptrSize" />-sized ones.
        /// </returns>
        public IntrinsicProcedure ResolvePointers(int ptrSize)
        {
            if (IsGeneric)
                throw new InvalidOperationException($"{Name} is generic.");
            var sig = this.Signature;
            if (sig is null)
                throw new InvalidOperationException($"Cannot resolve pointers for null signature for {Name}.");
            if (!sig.ParametersValid)
                throw new InvalidOperationException($"Signature for {Name} is not valid.");
            var parameters = new Identifier[sig.Parameters!.Length];
            for (int i = 0; i < sig.Parameters.Length; ++i)
            {
                var param = sig.Parameters[i];
                if (param.DataType is Pointer ptr)
                {
                    param = new Identifier(
                        param.Name,
                        ResolvePointer(param.DataType, ptr.Pointee, ptrSize),
                        param.Storage);
                }
                parameters[i] = param;
            }
            FunctionType newSig;
            if (sig.HasVoidReturn)
            {
                newSig = FunctionType.CreateUserDefined(parameters, []);
            }
            else
            {
                var ret = sig.ReturnValue;
                if (ret?.DataType is Pointer ptr)
                {
                    ret = new Identifier(
                        ret.Name,
                        ResolvePointer(ret.DataType, ptr.Pointee, ptrSize),
                        ret.Storage);
                }
                newSig = FunctionType.CreateUserDefined(
                    parameters, 
                    ret is null ? [] : [ret]);
            }
            return new IntrinsicProcedure(this.Name, this.HasSideEffect, newSig, Evaluate)
            {
                Characteristics = this.Characteristics,
                EnclosingType = this.EnclosingType
            };
        }

        /// <summary>
        /// Returns a string representation of this <see cref="IntrinsicProcedure"/>.
        /// </summary>
        public override string ToString()
		{
			if (Signature is not null)
			{
                return base.ToString();
			}
			else
			{
				return string.Format("{0} {1}({2} args)", ReturnType, Name, arity);
			}
		}

        private static Constant? FailEvaluator(DataType dt, Constant[] cs) => null;

        private class InstanceCacheComparer : IEqualityComparer<(IntrinsicProcedure, DataType[])>
        {
            public bool Equals((IntrinsicProcedure, DataType[]) x, (IntrinsicProcedure, DataType[]) y)
            {
                if (x.Item1 != y.Item1)
                    return false;
                if (x.Item2.Length != y.Item2.Length)
                    return false;
                var cmp = DataTypeComparer.Instance;
                for (int i = 0; i < x.Item2.Length; ++i)
                {
                    if (cmp.Compare(x.Item2[i], y.Item2[i]) != 0)
                        return false;
                }
                return true;
            }

            public int GetHashCode((IntrinsicProcedure, DataType[]) obj)
            {
                int h = obj.Item1.GetHashCode();
                for (int i = 0; i < obj.Item2.Length; ++i)
                {
                    h = (h * 5) ^ obj.Item1.GetHashCode();
                }
                return h;
            }
        }
    }
}
