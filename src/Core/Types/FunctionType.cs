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
using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Core.Types
{
    /// <summary>
    /// Models a function type, including summarizing the effects of calling
    /// a procedure, as seen by the caller.
    /// </summary>
    /// <remarks>
    /// Calling a procedure affects a few things: the registers, the stack 
    /// depth, and in the case of the Intel x86 architecture the FPU stack 
    /// depth. These effects are summarized by the signature.
    /// <para>
    /// $TODO: There are CPU-specific items (like x86 FPU stack gunk). Move
    /// these into processor-specific subclasses. Also, some architectures 
    /// -- like the FORTH language -- have multiple stacks.
    /// </para>
    /// </remarks>
    //$TODO: consider breaking out a base class "CallableType" and have 
    // a sibling class "LowLevelFunctionType".
    public class FunctionType : CompositeType
	{
        /// <summary>
        /// Constructs an empty function type.
        /// </summary>
        public FunctionType()
            : base(Domain.Function, null)
        {
            this.ParametersValid = false;
            this.FpuStackArgumentMax = -1;
            this.Outputs = [];
        }

        /// <summary>
        /// Constructs a function type with a known return value and parameters.
        /// </summary>
        /// <param name="inputs">Input parameters.</param>
        /// <param name="outputs">Return values. If multiple values
        /// are supplied, the first one will become the "return" value,
        /// any others will be used as "out" parameters.</param>
        public FunctionType(
            Identifier[] inputs,
            Identifier[] outputs)
            : base(Domain.Function, null)
        {
            this.Parameters = inputs;
            this.ParametersValid = true;
            this.FpuStackArgumentMax = -1;
            this.Outputs = outputs.Length > 0
                ? outputs
                : [ VoidReturnValue() ];
        }

        private static Identifier VoidReturnValue()
        {
            return new Identifier("", VoidType.Instance, null!);
        }

        /// <summary>
        /// Factory method to create a user defined function type.
        /// </summary>
        /// <param name="formals">Formal parameters.</param>
        /// <param name="outputs">Output parameters.</param>
        /// <returns>The new function type.</returns>
        public static FunctionType CreateUserDefined(
            Identifier[] formals,
            Identifier[] outputs)
        {
            var ft = new FunctionType(formals, outputs);
            ft.UserDefined = true;
            return ft;
        }

        /// <summary>
        /// Factory method to create a function type with a single
        /// return value.
        /// </summary>
        /// <param name="returnId">Return value.</param>
        /// <param name="formals">Formal parameters.</param>
        /// <returns>The new function type.</returns>
        public static FunctionType Create(Identifier? returnId, params Identifier[] formals)
        {
            var ret = new Identifier[] { returnId ?? VoidReturnValue() };
            return new FunctionType(formals, ret);
        }

        /// <summary>
        /// Create a function type with a void return type.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>A function type.</returns>
        public static FunctionType Action(params Identifier [] parameters)
        {
            return new FunctionType(parameters, [VoidReturnValue() ]);
        }


        /// <summary>
        /// True if the function type is variadic.
        /// </summary>
        public bool IsVariadic { get; set; }

        /// <summary>
        /// The return types of the function.
        /// </summary>
        public Identifier[] Outputs { get; private set; }

        /// <summary>
        /// The return value of a function. 
        /// </summary>
        /// <remarks>
        /// The 'Name' property of the <see cref="Identifier"/> is not applicable, and will typically
        /// be the empty string.
        /// </remarks>
        public Identifier? ReturnValue => Outputs.Length == 0 ? null : Outputs[0];

        /// <summary>
        /// The parameters of the function.
        /// </summary>
        /// <remarks>
        /// There may be additional arguments to the function if it is a variadic
        /// function.
        /// </remarks>
        public Identifier[]? Parameters { get; private set; }

        /// <summary>
        /// Indicates if the function return type is 'void'.
        /// </summary>
        public bool HasVoidReturn => Outputs.Length == 0 || Outputs[0].DataType == VoidType.Instance;

        /// <summary>
        /// Type variable associated with this function type.
        /// </summary>
        public TypeVariable? TypeVariable { get; set; }  //$REVIEW: belongs on the Procedure itself!

        /// <inheritdoc/>
        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitFunctionType(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitFunctionType(this);
        }

        /// <inheritdoc/>
        public override DataType Clone(IDictionary<DataType, DataType>? clonedTypes)
        {
            return Clone(clonedTypes, false);
        }

        /// <summary>
        /// Creates a copy of the function type.
        /// </summary>
        /// <param name="clonedTypes">Optional already cloned members.</param>
        /// <param name="shareIncompleteTypes"></param>
        /// <returns>A cloned copy of the <see cref="FunctionType"/>.</returns>
        public FunctionType Clone(
            IDictionary<DataType, DataType>? clonedTypes = null,
            bool shareIncompleteTypes = false)
		{
            FunctionType ft;
            if (ParametersValid)
            {
                var outputs = this.Outputs
                    .Select(p => CloneIdentifier(p, clonedTypes, shareIncompleteTypes))
                    .ToArray();
                var parameters = this.Parameters!
                    .Select(p => CloneIdentifier(p, clonedTypes, shareIncompleteTypes))
                    .ToArray();
                ft = new FunctionType(parameters, outputs);
            }
            else
            {
                ft = new FunctionType();
            }
            ft.Qualifier = Qualifier;
            ft.ParametersValid = ParametersValid;
            ft.IsInstanceMetod = IsInstanceMetod;
            ft.ReturnAddressOnStack = ReturnAddressOnStack;
            ft.FpuStackDelta = FpuStackDelta;
            ft.StackDelta = StackDelta;
            ft.FpuStackArgumentMax = FpuStackArgumentMax;
            ft.FpuStackOutArgumentMax = FpuStackOutArgumentMax;
            ft.IsVariadic = IsVariadic;
            return ft;
		}

        /// <summary>
        /// Create a new signature with the parameters replaced with
        /// the provided parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public FunctionType ReplaceParameters(params Identifier[] parameters)
        {
            var sig = (FunctionType)Clone();
            sig.Parameters = parameters;
            sig.IsVariadic = false;
            return sig;
        }

        /// <summary>
        /// The size of the return address if pushed on stack.
        /// </summary>
        /// <remarks>
        /// This low level detail is commonly implicit in ABI's.
        /// </remarks>
        public int ReturnAddressOnStack { get; set; }

        /// <summary>
        /// Number of slots by which the FPU stack grows or shrinks after the
        /// procedure is called. A positive number means that items are left
        /// on the stack, a negative number means items are removed from stack.
        /// </summary>
        /// <remarks>
        /// This is x86-specific.
        /// </remarks>
        public int FpuStackDelta { get; set; }

        /// <summary>
        /// Number of bytes to add to the stack pointer after returning from 
        /// the procedure. Note that this does include the return address 
        /// size, if the return address is passed on the stack. 
        /// </summary>
        public int StackDelta { get; set; }

        /// <summary>
        /// The index of the 'deepest' FPU stack argument used. -1 means no
        /// stack parameters are used.
        /// </summary>
        public int FpuStackArgumentMax { get; set; }

        /// <summary>
        /// The index of the 'deepest' FPU stack argument written. -1 means no
        /// stack parameters are written.
        /// </summary>
        public int FpuStackOutArgumentMax { get; set; }

        /// <summary>
        /// True if the medium-level arguments have been discovered. Otherwise,
        /// the signature just contains the net effect
        /// on the processor state.
        /// </summary>
        public bool ParametersValid { get; private set;  }

        /// <summary>
        /// True if this is an instance method of the EnclosingType.
        /// </summary>
        public bool IsInstanceMetod { get; set; }

        /// <inheritdoc/>
        public override int Size
		{
			get { return 0; }
			set { ThrowBadSize(); }
		}

        #region Output methods

        /// <summary>
        /// Writes a text representation of the function type to a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="fnName">Name of the function.</param>
        /// <param name="f">Flags controlling the output.</param>
        /// <param name="writer">Output sink.</param>
        public void Emit(string fnName, EmitFlags f, TextWriter writer)
        {
            Emit(fnName, f, new TextFormatter(writer));
        }

        /// <summary>
        /// Writes a text representation of the function type to a <see cref="Formatter"/>.
        /// </summary>
        /// <param name="fnName">Name of the function.</param>
        /// <param name="flags">Flags controlling the output.</param>
        /// <param name="formatter">Output sink.</param>
        public void Emit(string fnName, EmitFlags flags, Formatter formatter)
        {
            Emit(fnName, flags, formatter, new CodeFormatter(formatter), new TypeReferenceFormatter(formatter));
        }

        /// <summary>
        /// Writes a text representation of the function type to a <see cref="Formatter"/>.
        /// </summary>
        /// <param name="fnName">Name of the function.</param>
        /// <param name="f">Flags controlling the output.</param>
        /// <param name="fmt">Output sink.</param>
        /// <param name="w">Code formatter.</param>
        /// <param name="t">Type reference formatter.</param>
        public void Emit(string fnName, EmitFlags f, Formatter fmt, CodeFormatter w, TypeReferenceFormatter t)
        {
            bool emitStorage = (f & EmitFlags.ArgumentKind) == EmitFlags.ArgumentKind;
           
            if (ParametersValid)
            {
                if (emitStorage)
                {
                    if (HasVoidReturn)
                    {
                        fmt.Write("void ");
                    }
                    else
                    {
                        w.WriteFormalArgumentType(ReturnValue!, emitStorage, false);
                        fmt.Write(" ");
                    }
                    fmt.Write("{0}(", fnName);
                }
                else
                {
                    if (HasVoidReturn)
                    {
                        fmt.Write("void {0}", fnName);
                    }
                    else
                    {
                        t.WriteDeclaration(ReturnValue!.DataType, fnName);           //$TODO: won't work with fn's that return pointers to functions or arrays.
                    }
                    fmt.Write("(");
                }
                var sep = "";
                if (Parameters is not null)
                {
                    IEnumerable<Identifier> parms = this.IsInstanceMetod
                        ? Parameters.Skip(1)
                        : Parameters;
                    foreach (var p in parms)
                    {
                        fmt.Write(sep);
                        sep = ", ";
                        w.WriteFormalArgument(p, emitStorage, false, t);
                    }
                    foreach (var op in Outputs.Skip(1))
                    {
                        fmt.Write(sep);
                        sep = ", ";
                        w.WriteFormalArgument(op, emitStorage, true, t);
                    }
                    if (this.IsVariadic)
                    {
                        fmt.Write(sep);
                        if (sep.Length == 0)
                            fmt.Write(' ');
                        fmt.Write("...");
                    }
                }
                fmt.Write(")");
            }
            else
            {
                fmt.WriteKeyword("define");
                fmt.Write(" ");
                fmt.Write(fnName);
            }

            if ((f & EmitFlags.LowLevelInfo) == EmitFlags.LowLevelInfo)
            {
                fmt.WriteLine();
                fmt.Write("// stackDelta: {0}; fpuStackDelta: {1}; fpuMaxParam: {2}", StackDelta, FpuStackDelta, FpuStackArgumentMax);
                fmt.WriteLine();
            }
        }

        /// <summary>
        /// Creates a string representation of the function type.
        /// </summary>
        /// <param name="name">Optionalal function name.</param>
        /// <param name="flags">Flags controlling output.</param>
        public string ToString(string name, EmitFlags flags = EmitFlags.ArgumentKind)
        {
            var sw = new StringWriter();
            var f = new TextFormatter(sw);
            var cf = new CodeFormatter(f);
            var tf = new TypeReferenceFormatter(f);
            Emit(name, flags, f, cf, tf);
            return sw.ToString();
        }

        /// <summary>
        /// Flags controlling the output formatting of the function type.
        /// </summary>
        [Flags]
        public enum EmitFlags
        {
            /// <summary>
            /// Default value.
            /// </summary>
            None = 0,

            /// <summary>
            /// Render the storage for each parameter.
            /// </summary>
            ArgumentKind = 1,

            /// <summary>
            /// Emit low-level information about the parameters.
            /// </summary>
            LowLevelInfo = 2,

            /// <summary>
            /// Emit full information.
            /// </summary>
            AllDetails = ArgumentKind|LowLevelInfo,
        }
        #endregion

        private static Identifier CloneIdentifier(
            Identifier id,
            IDictionary<DataType, DataType>? clonedTypes,
            bool shareIncompleteTypes)
        {
            if (
                shareIncompleteTypes &&
                // Share incomplete(ptrXX, wordXX) and unknown types so that
                // they can be reconstructed during Type Analysis
                ((id.DataType is PrimitiveType && id.DataType.IsPointer) ||
                id.DataType.IsWord || id.DataType is UnknownType)
            )
                return id;
            return new Identifier(
                id.Name,
                id.DataType.Clone(clonedTypes),
                id.Storage);
        }
    }
}
