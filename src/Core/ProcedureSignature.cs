#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Expressions;
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
	/// Calling a procedure affects a few things: the registers, the stack depth, and in the case of the Intel x86
	/// architecture the FPU stack depth. These effects are summarized by the signature.
    /// <para>
    /// $TODO: There are CPU-specific items (like x86 FPU stack gunk). Move these into processor-specific subclasses.
    /// Also, some architectures -- like the FORTH language -- have multiple stacks.
    /// </para>
	/// </remarks>
    public class ProcedureSignature
    {
        public ProcedureSignature()
        {
            this.FpuStackArgumentMax = -1;
        }

        public ProcedureSignature(Identifier returnId, params Identifier[] formalArguments)
            : this()
        {
            this.ReturnValue = returnId;
            this.Parameters = formalArguments;
        }

        public TypeVariable TypeVariable { get; set; }
        public Identifier[] Parameters { get; private set; } 
        public Identifier ReturnValue { get; private set; }
        public int ReturnAddressOnStack { get; set; }           // The size of the return address if pushed on stack.

        /// <summary>
        /// Number of slots by which the FPU stack grows or shrinks after the procedure is called. A positive number 
        /// means that items are left on the stack, a negative number means items are removed from stack.
        /// </summary>
        /// <remarks>
        /// This is x86-specific.
        /// </remarks>
        public int FpuStackDelta { get; set; }

        /// <summary>
        /// Number of bytes to add to the stack pointer after returning from the procedure.
        /// Note that this does include the return address size, if the return address is 
        /// passed on the stack. 
        /// </summary>
        public int StackDelta { get; set; }

        /// <summary>
        /// The index of the 'deepest' FPU stack argument used. -1 means no stack parameters are used.
        /// </summary>
        public int FpuStackArgumentMax { get; set; }

        /// <summary>
        /// The index of the 'deepest' FPU stack argument written. -1 means no stack parameters are written.
        /// </summary>
        public int FpuStackOutArgumentMax { get; set; }

        /// <summary>
        /// True if the medium-level arguments have been discovered. Otherwise, the signature just contains the net effect
        /// on the processor state.
        /// </summary>
        public bool ParametersValid
        {
            get { return Parameters != null || ReturnValue != null; }
        }

        #region Output methods
        public void Emit(string fnName, EmitFlags f, TextWriter writer)
        {
            Emit(fnName, f, new TextFormatter(writer));
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
                if (ReturnValue != null)
                {
                    w.WriteFormalArgumentType(ReturnValue, emitStorage);
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
                if (ReturnValue == null)
                    fmt.Write("void {0}", fnName);
                else
                {
                    t.Write(ReturnValue.DataType, fnName);           //$TODO: won't work with fn's that return pointers to functions or arrays.
                }
                fmt.Write("(");
            }
            var sep = "";
            if (Parameters != null)
            {
                for (int i = 0; i < Parameters.Length; ++i)
                {
                    fmt.Write(sep);
                    sep = ", ";
                    w.WriteFormalArgument(Parameters[i], emitStorage, t);
                }
            }
            fmt.Write(")");

            if ((f & EmitFlags.LowLevelInfo) == EmitFlags.LowLevelInfo)
            {
                fmt.WriteLine();
                fmt.Write("// stackDelta: {0}; fpuStackDelta: {1}; fpuMaxParam: {2}", StackDelta, FpuStackDelta, FpuStackArgumentMax);
                fmt.WriteLine();
            }
        }

        public override string ToString()
        {
            StringWriter w = new StringWriter();
            TextFormatter f = new TextFormatter(w);
            CodeFormatter cf = new CodeFormatter(f);
            TypeFormatter tf = new TypeFormatter(f, false);
            Emit("()", EmitFlags.ArgumentKind | EmitFlags.LowLevelInfo, f, cf, tf);
            return w.ToString();
        }

        public string ToString(string name)
        {
            StringWriter sw = new StringWriter();
            TextFormatter f = new TextFormatter(sw);
            CodeFormatter cf = new CodeFormatter(f);
            TypeFormatter tf = new TypeFormatter(f, false);
            Emit(name, EmitFlags.ArgumentKind, f, cf, tf);
            return sw.ToString();
        }

        [Flags]
        public enum EmitFlags
        {
            None = 0,
            ArgumentKind = 1,
            LowLevelInfo = 2,
        }
        #endregion

    }
}
