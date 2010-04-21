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
		private int stackOffset;
		private int fpuStackOffset;
		private IProcessorArchitecture arch;
		private string defaultConvention;
		private int identifierNumber;

		public ProcedureSerializer(IProcessorArchitecture arch, string defaultConvention)
		{
			this.arch = arch;
			this.defaultConvention = defaultConvention;
			this.identifierNumber = 0;
		}

		public void ApplyConvention(SerializedSignature ssig, ProcedureSignature sig)
		{
			string d = ssig.Convention;
			if (d == null || d.Length == 0)
				d = defaultConvention;
			if (d != null && d == "stdapi")
				sig.StackDelta = stackOffset;

			sig.FpuStackDelta = fpuStackOffset;
		}

		public Identifier CreateId(string name, DataType type, Storage storage)
		{
			return new Identifier(name, identifierNumber++, type, storage);
		}

		public ProcedureSignature Deserialize(SerializedSignature ss, Frame frame)
		{
			ArgumentSerializer argser = new ArgumentSerializer(this, arch, frame);
			Identifier ret = null;
			if (ss.ReturnValue != null)
			{
				ret = argser.Deserialize(ss.ReturnValue);
			}
			List<Identifier> args = new List<Identifier>();
			if (ss.Arguments != null)
			{
				foreach (SerializedArgument arg in ss.Arguments)
				{
					args.Add(argser.Deserialize(arg));
				}
			}
            ProcedureSignature sig = new ProcedureSignature(ret, args.ToArray());
			ApplyConvention(ss, sig);
			return sig;
		}


		public int FpuStackOffset
		{
			get { return fpuStackOffset; }
			set { fpuStackOffset = value; }
		}

        public SerializedSignature Serialize(ProcedureSignature sig)
        {
            SerializedSignature ssig = new SerializedSignature();
            if (!sig.ArgumentsValid)
                return ssig;
            ArgumentSerializer argSer = new ArgumentSerializer(this, arch, null);
            ssig.ReturnValue = argSer.Serialize(sig.ReturnValue);
            ssig.Arguments = new SerializedArgument[sig.FormalArguments.Length];
            for (int i = 0; i < sig.FormalArguments.Length; ++i)
            {
                Identifier formal = sig.FormalArguments[i];
                ssig.Arguments[i] = argSer.Serialize(formal);
            }
            ssig.StackDelta = sig.StackDelta;
            ssig.FpuStackDelta = sig.FpuStackDelta;
            return ssig;
        }


        public int StackOffset
        {
            get { return stackOffset; }
            set { stackOffset = value; }
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