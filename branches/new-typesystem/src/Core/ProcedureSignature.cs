/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.Core.Types;
using System;
using System.IO;
using System.Xml;

namespace Decompiler.Core
{
	/// <summary>
	/// Summarizes the effects of calling a procedure, as seen by the caller. Procedure signatures
	/// may be shared by several procedures.
	/// </summary>
	/// <remarks>
	/// Calling a procedure affects a few things: the registers, the stack depth, and in the case of the intel x86
	/// architecture the fpu stack depth. These effects are summarized by the signature.
	/// </remarks>
	public class ProcedureSignature
	{
		private Identifier ret;
		private Identifier [] args;
		private int stackDelta;
		private TypeVariable typeVar;

		private int fpuStackDelta;	
		private int fpuStackMax;
		private int fpuStackWriteMax;

		public ProcedureSignature()
		{
		}

		public ProcedureSignature(Identifier returnId, Identifier [] arguments)
		{
			this.ret = returnId;
			this.args = arguments;
		}
		
		public Identifier [] Arguments
		{
			get { return args; } 
		}

		public void Emit(string fnName, EmitFlags f, TextWriter w)
		{
			bool emitStorage = (f & EmitFlags.ArgumentKind) == EmitFlags.ArgumentKind;
			if (ret != null)
			{
				ret.WriteType(emitStorage, w);
				w.Write(" ");
			}
			else
			{
				w.Write("void ");
			}
			w.Write("{0}(", fnName);
			if (args != null && args.Length > 0)
			{
				args[0].Write(emitStorage, w);
				for (int i = 1; i < args.Length; ++i)
				{
					w.Write(", ");
					args[i].Write(emitStorage, w);
				}
			}
			w.Write(")");
			if ((f & EmitFlags.LowLevelInfo) == EmitFlags.LowLevelInfo)
			{
				w.WriteLine();
				w.WriteLine("// stackDelta: {0}; fpuStackDelta: {1}; fpuMaxParam: {2}", stackDelta, FpuStackDelta, FpuStackParameterMax);
			}
		}

		private void EmitArgument(Identifier arg, bool emitStorage, TextWriter w)
		{
			arg.Write(emitStorage, w);
		}

		/// <summary>
		/// Amount by which the FPU stack grows or shrinks after the procedure is called.
		/// </summary>
		public int FpuStackDelta 
		{
			get { return fpuStackDelta; }
			set { fpuStackDelta = value; }
		}

		/// <summary>
		/// The index of the 'deepest' FPU stack argument written. -1 means no stack parameters are used.
		/// </summary>
		public int FpuStackOutParameterMax
		{
			get { return fpuStackWriteMax; }
			set { fpuStackWriteMax = value; }
		}

		/// <summary>
		/// The index of the 'deepest' FPU stack argument used. -1 means no stack parameters are used.
		/// </summary>
		public int FpuStackParameterMax
		{
			get { return fpuStackMax; }
			set { fpuStackMax = value; }
		}

		public bool ArgumentsValid
		{
			get { return args != null || ret != null; }
		}

		/// <summary>
		/// Amount of bytes to add to the stack pointer after returning from the procedure.
		/// Note that this also includes the return address size, if the return address is 
		/// passed on the stack.
		/// </summary>
		public int StackDelta
		{
			get { return stackDelta; }
			set 
			{
				stackDelta = value; 
			}
		}

		public Identifier ReturnValue
		{
			get { return ret; }
		}

		public override string ToString()
		{
			StringWriter w = new StringWriter();
			Emit("()", EmitFlags.ArgumentKind|EmitFlags.LowLevelInfo, w);
			return w.ToString();
		}

		public string ToString(string name)
		{
			StringWriter sw = new StringWriter();
			Emit(name, EmitFlags.ArgumentKind, sw);
			return sw.ToString();
		}

		public TypeVariable TypeVariable
		{
			get { return typeVar; }
			set { typeVar = value; }
		}

		[Flags]
		public enum EmitFlags
		{
			None = 0,
			ArgumentKind = 1,
			LowLevelInfo = 2,
		}
	}
}
