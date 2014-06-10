#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Core.Types;
using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Decompiler.Core.Serialization
{
	/// <summary>
	/// Helper class that serializes and deserializes procedures with their signatures.
	/// </summary>
	public class ProcedureSerializer
	{
		private IProcessorArchitecture arch;
        private string defaultConvention;
		private int identifierNumber;

        public ProcedureSerializer(IProcessorArchitecture arch, string defaultConvention)
            : this(arch, new TypeLibraryLoader(arch, true), defaultConvention)
        { }


		public ProcedureSerializer(IProcessorArchitecture arch, ISerializedTypeVisitor<DataType> typeLoader,  string defaultConvention)
		{
			this.arch = arch;
            this.TypeLoader = typeLoader;
			this.defaultConvention = defaultConvention;
			this.identifierNumber = 0;
		}

        public int FpuStackOffset { get; set; }
        public int StackOffset { get; set; }
        public ISerializedTypeVisitor<DataType> TypeLoader { get; private set; }

		public void ApplyConvention(SerializedSignature ssig, ProcedureSignature sig)
		{
			string d = ssig.Convention;
			if (d == null || d.Length == 0)
				d = defaultConvention;
			if (d != null && d == "stdapi")
				sig.StackDelta = StackOffset;

			sig.FpuStackDelta = FpuStackOffset;
		}

		public Identifier CreateId(string name, DataType type, Storage storage)
		{
			return new Identifier(name, identifierNumber++, type, storage);
		}

		public ProcedureSignature Deserialize(SerializedSignature ss, Frame frame)
		{
			var argser = new ArgumentSerializer(this, arch, frame, ss.Convention);
			Identifier ret = null;
            int fpuDelta = FpuStackOffset;

            FpuStackOffset = 0;
			if (ss.ReturnValue != null)
			{
				ret = argser.Deserialize(ss.ReturnValue);
                fpuDelta += FpuStackOffset;
			}

            FpuStackOffset = 0;
			var args = new List<Identifier>();
			if (ss.Arguments != null)
			{
				foreach (Argument_v1 arg in ss.Arguments)
				{
					args.Add(argser.Deserialize(arg));
				}
                fpuDelta -= FpuStackOffset;
			}
            FpuStackOffset = fpuDelta;

            var sig = new ProcedureSignature(ret, args.ToArray());
			ApplyConvention(ss, sig);
			return sig;
		}

        public SerializedSignature Serialize(ProcedureSignature sig)
        {
            SerializedSignature ssig = new SerializedSignature();
            if (!sig.ArgumentsValid)
                return ssig;
            ArgumentSerializer argSer = new ArgumentSerializer(this, arch, null, null);
            ssig.ReturnValue = argSer.Serialize(sig.ReturnValue);
            ssig.Arguments = new Argument_v1[sig.FormalArguments.Length];
            for (int i = 0; i < sig.FormalArguments.Length; ++i)
            {
                Identifier formal = sig.FormalArguments[i];
                ssig.Arguments[i] = argSer.Serialize(formal);
            }
            ssig.StackDelta = sig.StackDelta;
            ssig.FpuStackDelta = sig.FpuStackDelta;
            return ssig;
        }

        public SerializedProcedure Serialize(Procedure proc, Address addr)
        {
            SerializedProcedure sproc = new SerializedProcedure();
            sproc.Address = addr.ToString();
            sproc.Name = proc.Name;
            if (proc.Signature != null)
                sproc.Signature = Serialize(proc.Signature);
            return sproc;
        }
    }
}