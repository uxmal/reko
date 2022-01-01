#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.IO;
using Reko.Core;
using Reko.Core.Output;
using Reko.Core.Types;

namespace Reko.Core
{
	/// <summary>
	/// Represents predefined functions or processor instructions that don't have a 
	/// C/C++ equivalent (like rotate operations).
	/// </summary>
	public class IntrinsicProcedure : ProcedureBase
	{
		private readonly int arity;
        private readonly DataType returnType;
		private readonly FunctionType? sig;

        /// <summary>
        /// Well-known operations that many processors support but most high- or 
        /// medium level languages do not support.
        /// </summary>
        public const string Ror = "__ror";      // binary: Rotate right
        public const string Rol = "__rol";      // binary: Rotate left
        public const string RorC = "__rcr";     // ternary: rotate right, passing in the contents of a processor flag (not necessarily the Carry flag)
        public const string RolC = "__rcl";     // ternary: rotate left, passing in the contents of a processor flag
        public const string Syscall = "__syscall";  // Invokes a system call.

        // MIPS-style unaligned memory accesses
        public const string LwL = "__lwl";
        public const string LwR = "__lwr";
        public const string SwL = "__swl";
        public const string SwR = "__swr";

        public const string Align = "__align";

        /// <summary>
        /// Use this constructor for intrinsics that model operators that may have parameters of varying sizes.
        /// </summary>
        /// <remarks>
        /// E.g. the rotate intrinsic instructions.</remarks>
        /// <param name="name"></param>
        /// <param name="returnType"></param>
        /// <param name="arity"></param>
		public IntrinsicProcedure(string name, bool hasSideEffect, DataType returnType, int arity) : base(name, hasSideEffect)
		{
            this.returnType = returnType;
			this.arity = arity;
		}

        /// <summary>
        /// Creates an <see cref="IntrinsicProcedure"/> with a specific signature.
        /// </summary>
        /// <param name="name">The name of the intrinsic procedure.</param>
        /// <param name="hasSideEffect">True of the procedure is idempotent (<see cref="ProcedureBase.IsIdempotent"/></param>
        /// <param name="sig">The signature of the procedure.</param>
		public IntrinsicProcedure(string name, bool hasSideEffect, FunctionType sig) : base(name, hasSideEffect)
		{
			this.sig = sig;
            this.returnType = sig.ReturnValue?.DataType!;
		}

		public int Arity
		{
			get 
            { return sig != null && sig.Parameters != null
                ? sig.Parameters.Length 
                : arity; 
            }
		}

        public DataType ReturnType
        {
            get { return returnType; }
        }

		public override FunctionType Signature
		{
			get { return sig!; }
			set { throw new InvalidOperationException("Changing the signature of an IntrinsicProcedure is not allowed."); }
		}

		public override string ToString()
		{
			if (Signature != null)
			{
                return base.ToString();
			}
			else
			{
				return string.Format("{0} {1}({2} args)", ReturnType, Name, arity);
			}
		}
	}
}
