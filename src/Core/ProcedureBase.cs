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

using Reko.Core.Types;
using Reko.Core.Serialization;
using Reko.Core.Output;
using System;
using System.ComponentModel;
using System.IO;
using Reko.Core.Expressions;

namespace Reko.Core
{
    /// <summary>
    /// Abstract base class for all things callable.
    /// </summary>
	[DefaultProperty("Name")]
	public abstract class ProcedureBase
	{
        private readonly DataType[] genericArguments;
        private readonly bool isConcrete;

		public ProcedureBase(string name, bool hasSideEffect)
		{
			this.name = name;
            this.genericArguments = Array.Empty<DataType>();
            this.HasSideEffect = hasSideEffect;
			this.Characteristics = DefaultProcedureCharacteristics.Instance;
		}

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
        public string Name { get { return name; } set { name = value; NameChanged?.Fire(this); } }
        public event EventHandler? NameChanged;
        private string name;

		public abstract FunctionType Signature { get; set; }

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

        protected FunctionType MakeConcreteSignature(DataType[] concreteTypes)
        {
            var sig = this.Signature;
            if (sig is null)
                throw new InvalidOperationException($"Cannot make a null concrete signature for {Name}.");
            if (!sig.ParametersValid)
                throw new InvalidOperationException($"Signature for {Name} is not valid.");
            if (genericArguments.Length == 0)
                throw new InvalidOperationException($"{Name} is not generic.");
            if (concreteTypes.Length != genericArguments.Length)
                throw new InvalidOperationException($"Mismatched number of concrete types for {Name}.");
            var parameters = new Identifier[sig.Parameters!.Length];
            for (int i = 0; i < sig.Parameters.Length; ++i)
            {
                var param = sig.Parameters[i];
                if (TryGetGenericArgument(param.DataType, out int index))
                {
                    param = new Identifier(param.Name, concreteTypes[index], param.Storage);
                }
                parameters[i] = param;
            }
            if (sig.HasVoidReturn)
            {
                return FunctionType.Action(parameters);
            }
            else
            {
                var ret = sig.ReturnValue;
                if (TryGetGenericArgument(ret.DataType, out int index))
                {
                    ret = new Identifier(ret.Name, concreteTypes[index], ret.Storage);
                }
                return FunctionType.Func(ret, parameters);
            }
        }

        private bool TryGetGenericArgument(DataType dt, out int index)
        {
            for (int i = 0; i < genericArguments.Length; ++i)
            {
                if (dt == genericArguments[i])
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            var name = DecorateGenericName();
            Signature.Emit(name, FunctionType.EmitFlags.ArgumentKind, new TextFormatter(sw));
            return sw.ToString();
        }
    }
}
