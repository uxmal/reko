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

using Reko.Core.Code;
using Reko.Core.Types;
using Reko.Core.Lib;
using System;
using System.Collections;
using System.Xml;
using Reko.Core;

namespace Decompiler.Core.Serialization
{
	/// <summary>
	/// Helper class that serializer and deserializes procedure signatures.
	/// </summary>
	public class SignatureSerializer
	{
		private int stackOffset;
		private int fpuStackOffset;
		private IProcessorArchitecture arch;
		private string defaultConvention;
		private int identifierNumber;

		public SignatureSerializer(IProcessorArchitecture arch, string defaultConvention)
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
			if (d is not null && d == "stdapi")
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
			if (ss.ReturnValue is not null)
			{
				ret = argser.Deserialize(ss.ReturnValue);
			}
			ArrayList args = new ArrayList();
			if (ss.Arguments is not null)
			{
				foreach (SerializedArgument arg in ss.Arguments)
				{
					args.Add(argser.Deserialize(arg));
				}
			}
			ProcedureSignature sig = new ProcedureSignature(
				ret, (Identifier [])args.ToArray(typeof(Identifier)));
			
			ApplyConvention(ss, sig);

			return sig;
		}


		public int FpuStackOffset
		{
			get { return fpuStackOffset; }
			set { fpuStackOffset = value; }
		}

		public int StackOffset
		{
			get { return stackOffset; }
			set { stackOffset = value; }
		}
	}
}