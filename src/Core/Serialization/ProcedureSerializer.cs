#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Types;
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Reko.Core.Serialization
{
	/// <summary>
	/// Helper class that serializes and deserializes procedures with their signatures.
	/// </summary>
	public abstract class ProcedureSerializer
	{
		public ProcedureSerializer(
            IProcessorArchitecture arch, 
            ISerializedTypeVisitor<DataType> typeLoader, 
            string defaultConvention)
		{
			this.Architecture = arch;
            this.TypeLoader = typeLoader;
			this.DefaultConvention = defaultConvention;
		}

        public IProcessorArchitecture Architecture { get; private set; }
        public ISerializedTypeVisitor<DataType> TypeLoader { get; private set; }
        public string DefaultConvention { get; set; }

        public int FpuStackOffset { get; set; }
        public int StackOffset { get; set; }

		public Identifier CreateId(string name, DataType type, Storage storage)
		{
			return new Identifier(name, type, storage);
		}

        /// <summary>
        /// Deserializes the signature <paramref name="ss"/>. Any instantiated
        /// registers or stack variables are introduced into the Frame.
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public abstract FunctionType Deserialize(SerializedSignature ss, Frame frame);

        public abstract Storage GetReturnRegister(Argument_v1 sArg, int bitSize);

        public virtual SerializedSignature Serialize(FunctionType sig)
        {
            SerializedSignature ssig = new SerializedSignature();
            if (!sig.ParametersValid)
            {
                ssig.ParametersValid = false;
                return ssig;
            }
            ArgumentSerializer argSer = new ArgumentSerializer(Architecture);
            ssig.ReturnValue = argSer.Serialize(sig.ReturnValue);
            ssig.Arguments = new Argument_v1[sig.Parameters.Length];
            for (int i = 0; i < sig.Parameters.Length; ++i)
            {
                Identifier formal = sig.Parameters[i];
                ssig.Arguments[i] = argSer.Serialize(formal);
            }
            ssig.StackDelta = sig.StackDelta;
            ssig.FpuStackDelta = sig.FpuStackDelta;
            return ssig;
        }

        public Procedure_v1 Serialize(Procedure proc, Address addr)
        {
            Procedure_v1 sproc = new Procedure_v1();
            sproc.Address = addr.ToString();
            sproc.Name = proc.Name;
            if (proc.Signature != null)
                sproc.Signature = Serialize(proc.Signature);
            return sproc;
        }
    }
}