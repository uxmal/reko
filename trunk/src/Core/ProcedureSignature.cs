/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Core.Output;
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
		private Identifier [] formals;
		private int stackDelta;
		private TypeVariable typeVar;

		private int fpuStackDelta;	
		private int fpuStackMax;
		private int fpuStackWriteMax;

		public ProcedureSignature()
		{
		}

		public ProcedureSignature(Identifier returnId, params Identifier [] formalArguments)
		{
			this.ret = returnId;
			this.formals = formalArguments;
		}
		
		public Identifier [] FormalArguments
		{
			get { return formals; } 
		}

        public void Emit(string fnName, EmitFlags f, TextWriter writer)
        {
            Emit(fnName, f, new Formatter(writer));
        }

		public void Emit(string fnName, EmitFlags f, Formatter fmt)
		{
            Emit(fnName, f, fmt, new CodeFormatter(fmt), new TypeFormatter(fmt, true));
		}

        public void Emit(string fnName, EmitFlags f, Formatter fmt, CodeFormatter w, TypeFormatter t)
        {
            bool emitStorage = (f & EmitFlags.ArgumentKind) == EmitFlags.ArgumentKind;
            if (emitStorage)
            {
                if (ret != null)
                {
                    w.WriteFormalArgumentType(ret, emitStorage);
                    fmt.Write(" ");
                }
                else
                {
                    fmt.Write("void ");
                }
                fmt.Write("{0}(", fnName);
            }
            else
            {
                if (ret == null)
                    fmt.Write("void {0}", fnName);
                else
                {
                    t.Write(ret.DataType, fnName);           //$TODO: won't work with fn's that return pointers to functions or arrays.
                }
                fmt.Write("(");
            }
            if (formals != null && formals.Length > 0)
            {
                w.WriteFormalArgument(formals[0], emitStorage, t);
                for (int i = 1; i < formals.Length; ++i)
                {
                    fmt.Write(", ");
                    w.WriteFormalArgument(formals[i], emitStorage, t);
                }
            }
            fmt.Write(")");
            if ((f & EmitFlags.LowLevelInfo) == EmitFlags.LowLevelInfo)
            {
                fmt.WriteLine();
                fmt.Write("// stackDelta: {0}; fpuStackDelta: {1}; fpuMaxParam: {2}", stackDelta, FpuStackDelta, FpuStackArgumentMax);
                fmt.WriteLine();
            }
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
		public int FpuStackOutArgumentMax
		{
			get { return fpuStackWriteMax; }
			set { fpuStackWriteMax = value; }
		}

		/// <summary>
		/// The index of the 'deepest' FPU stack argument used. -1 means no stack parameters are used.
		/// </summary>
		public int FpuStackArgumentMax
		{
			get { return fpuStackMax; }
			set { fpuStackMax = value; }
		}

		public bool ArgumentsValid
		{
			get { return formals != null || ret != null; }
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
            Formatter f = new Formatter(w);
            CodeFormatter cf = new CodeFormatter(f);
            TypeFormatter tf = new TypeFormatter(f, false);
			Emit("()", EmitFlags.ArgumentKind|EmitFlags.LowLevelInfo, f, cf, tf);
			return w.ToString();
		}

		public string ToString(string name)
		{
			StringWriter sw = new StringWriter();
            Formatter f = new Formatter(sw);
            CodeFormatter cf = new CodeFormatter(f);
            TypeFormatter tf = new TypeFormatter(f, false);
            Emit(name, EmitFlags.ArgumentKind, f, cf, tf);
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
