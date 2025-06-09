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
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace Reko.Core
{
    /// <summary>
    /// Abstract base class for all things callable.
    /// </summary>
	[DefaultProperty("Name")]
	public abstract class ProcedureBase
	{
        /// <summary>
        /// Event raised when the name of the procedure changes.
        /// </summary>
        public event EventHandler? NameChanged;

        private readonly DataType[] genericArguments;
        private readonly bool isConcrete;

        /// <summary>
        /// Initialize an instance of <see cref="ProcedureBase"/>.
        /// </summary>
        /// <param name="name">The name of the callable.</param>
        /// <param name="hasSideEffect">True if calling this procedure has a side effect not
        /// visible in architectural registers; otherwise false.</param>
		public ProcedureBase(string name, bool hasSideEffect)
		{
			this.name = name;
            this.genericArguments = [];
            this.HasSideEffect = hasSideEffect;
			this.Characteristics = DefaultProcedureCharacteristics.Instance;
		}

        /// <summary>
        /// Initialize a generic instance of <see cref="ProcedureBase"/>.
        /// </summary>
        /// <param name="name">The name of the callable.</param>
        /// <param name="genericArguments">An array of <see cref="DataType"/> objects that 
        /// represent the type arguments of a generic procedure.</param>
        /// <param name="isConcrete">True if this is a concrete instance of a generic procedure; otherwise false.</param>
        /// <param name="hasSideEffect">True if calling this procedure has a side effect not
        /// visible in architectural registers; otherwise false.</param>
        public ProcedureBase(
            string name, 
            DataType[] genericArguments,
            bool isConcrete,
            bool hasSideEffect)
        {
            this.name = name;
            this.genericArguments = genericArguments;
            this.isConcrete = isConcrete;
            this.HasSideEffect = hasSideEffect;
            this.Characteristics = DefaultProcedureCharacteristics.Instance;
        }

        /// <summary>
        /// If this is a member function of a class or struct, this property
        /// will be a reference to the enclosing type.
        /// </summary>
        //$TODO: all the infrastructure for class loading remains to be
        // implemented; for now we just store the class name.
        public SerializedType? EnclosingType { get; set; }

        /// <summary>
        /// If this is a generic procedure _and_ it's concrete (i.e. instantiated
        /// with data types), return true.
        /// </summary>
        public bool IsConcreteGeneric => this.isConcrete && this.IsGeneric;

        /// <summary>
        /// This property is true if there is at least one generic argument 
        /// to the procedure.
        /// </summary>
        public bool IsGeneric => this.genericArguments.Length > 0;

        /// <summary>
        /// The name of the procedure.
        /// </summary>
        public string Name
        {
            get { return name; } 
            set { 
                if (name == value) return;
                name = value; 
                NameChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private string name;
        
        /// <summary>
        /// The <see cref="FunctionType">function signature</see> of this procedure.
        /// </summary>
        /// <summary>
        /// The function signature of the procedure.
        /// </summary>
		public abstract FunctionType Signature { get; set; }

        /// <summary>
        /// The <see cref="ProcedureCharacteristics"/> of this procedure.
        /// </summary>
		public ProcedureCharacteristics Characteristics { get; set; }

        /// <summary>
        /// If a <see cref="ProcedureBase"/> has no side effect, calls to it can be removed
        /// if the result of the call is never used. Side effects include but are not limited 
        /// to changing memory, modifying device states, or raising exceptions.
        /// </summary>
        public bool HasSideEffect { get; }

        private string DecorateGenericName()
        {
            var sw = new StringWriter();
            sw.Write(Name);
            if (this.IsGeneric)
            {
                var fm = new TextFormatter(sw);
                var tw = new TypeReferenceFormatter(fm);
                var sep = '<';
                foreach (var arg in this.genericArguments)
                {
                    sw.Write(sep);
                    sep = ',';
                    tw.WriteTypeReference(arg);
                }
                sw.Write('>');
            }
            return sw.ToString();
        }

        /// <summary>
        /// Returns an array of <see cref="DataType"/> objects that represent the 
        /// type arguments of a generic procedure. 
        /// </summary>
        /// <returns>An array of <see cref="DataType"/> objects. If the procedure
        /// is not generic, an empty array is returned.
        /// </returns>
        public DataType[] GetGenericArguments() => this.genericArguments;

        /// <summary>
        /// Creates a concrete version of this procedure by applying the provided
        /// concrete types to the generic arguments of the procedure.
        /// </summary>
        /// <param name="ptrSize">Optional size of pointers, in bits.</param>
        /// <param name="concreteTypes">Concrete types to instantiate with.</param>
        /// <returns>A concrete version of the original generic version.
        /// </returns>
        protected FunctionType MakeConcreteSignature(int ptrSize, DataType[] concreteTypes)
        {
            var sig = this.Signature;
            if (sig is null)
                throw new InvalidOperationException($"Cannot make a null concrete signature for {Name}.");
            if (!sig.ParametersValid)
                throw new InvalidOperationException($"Signature for {Name} is not valid.");
            if (genericArguments.Length == 0)
                throw new InvalidOperationException($"{Name} is not generic.");
            if (concreteTypes.Length != genericArguments.Length)
                throw new InvalidOperationException(
                    $"Mismatched number of concrete types for {Name}; expected {genericArguments.Length} but had {concreteTypes.Length}.");
            var parameters = new Identifier[sig.Parameters!.Length];
            for (int i = 0; i < sig.Parameters.Length; ++i)
            {
                var param = sig.Parameters[i];
                if (TryGetGenericArgument(param.DataType, out int index))
                {
                    param = new Identifier(
                        param.Name,
                        ResolvePointer(param.DataType, concreteTypes[index], ptrSize),
                        param.Storage);
                }
                parameters[i] = param;
            }
            FunctionType concreteSig;
            if (sig.HasVoidReturn)
            {
                concreteSig = FunctionType.Action(parameters);
            }
            else
            {
                Identifier ret = sig.ReturnValue!;
                if (TryGetGenericArgument(ret.DataType, out int index))
                {
                    ret = new Identifier(
                        ret.Name,
                        ResolvePointer(ret.DataType, concreteTypes[index], ptrSize),
                        ret.Storage);
                }
                concreteSig = new FunctionType(parameters, [ ret ]);
            }
            concreteSig.IsVariadic = sig.IsVariadic;
            return concreteSig;
        }

        /// <summary>
        /// Resolves any 0-sized pointers, which are used to indicate pointers
        /// of unknown size.
        /// </summary>
        protected static DataType ResolvePointer(DataType dtGeneric, DataType dtConcrete, int ptrSize)
        {
            if (dtGeneric is Pointer ptr && ptr.BitSize == 0)
            {
                if (ptrSize == 0)
                    throw new InvalidOperationException("Non-zero pointer size has not been specified.");
                return new Pointer(dtConcrete, ptrSize);
            }
            else
            {
                return dtConcrete;
            }
        }

        private bool TryGetGenericArgument(DataType dt, out int index)
        {
            for (int i = 0; i < genericArguments.Length; ++i)
            {
                if (dt == genericArguments[i] ||
                    (dt is Pointer ptr && ptr.Pointee == genericArguments[i]))
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        /// <summary>
        /// If the procedure is a member of a class, write the class name first.
        /// </summary>
        /// <returns>The fully qualified name.</returns>
        public string QualifiedName()
        {
            if (EnclosingType is null)
                return Name;
            if (EnclosingType is StructType_v1 str)
                return $"{str.Name}::{Name}";
            return Name;
        }

        /// <summary>
        /// Returns a string representation of the procedure.
        /// </summary>
        public override string ToString()
        {
            var sw = new StringWriter();
            var name = DecorateGenericName();
            Signature.Emit(name, FunctionType.EmitFlags.ArgumentKind, new TextFormatter(sw));
            return sw.ToString();
        }
    }
}
