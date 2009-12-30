/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using System;
using System.Collections;
using System.IO;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Describes the flow of registers in and out of a procedure. 
	/// We are usually interested in registers modified, live in, &c.
	/// </summary>
	public class ProcedureFlow : DataFlow
	{
		private Procedure proc;

		public BitSet PreservedRegisters;			// Registers explicitly preserved by the procedure.
		public uint grfPreserved;

		public uint grfTrashed;
		public BitSet TrashedRegisters;		// Registers globally trashed by procedure and/or callees.

		public BitSet ByPass;
		public uint grfByPass;
		public BitSet MayUse;
		public uint grfMayUse;
		public BitSet Summary;
		public uint grfSummary;

		public Hashtable StackArguments;		//$REFACTOR: make this a strongly typed dictionary (Var -> PrimitiveType)

		public BitSet LiveOut;
		public uint grfLiveOut;

		public ProcedureSignature Signature;

		public ProcedureFlow(Procedure proc, IProcessorArchitecture arch)
		{
			this.proc = proc;

			PreservedRegisters = arch.CreateRegisterBitset();
			TrashedRegisters = arch.CreateRegisterBitset();

			ByPass = arch.CreateRegisterBitset();
			MayUse = arch.CreateRegisterBitset();
			LiveOut = arch.CreateRegisterBitset();

			StackArguments = new Hashtable();
		}

		public override void Emit(IProcessorArchitecture arch, TextWriter sb)
		{
			EmitRegisters(arch, "// MayUse: ", grfMayUse, MayUse, sb);
			sb.WriteLine();
			EmitRegisters(arch, "// LiveOut:", grfLiveOut, LiveOut, sb);
			sb.WriteLine();
			EmitRegisters(arch, "// Trashed:", grfTrashed, TrashedRegisters, sb);
			sb.WriteLine();
			EmitRegisters(arch, "// Preserved:", grfPreserved, PreservedRegisters, sb);
			sb.WriteLine();
			EmitStackArguments(StackArguments, sb);
		}

		public void EmitStackArguments(Hashtable args, TextWriter sb)
		{
			if (args.Count > 0)
			{
				sb.Write("// Stack args:");
				SortedList sort = new SortedList();
				foreach (DictionaryEntry de in args)
				{
					sort.Add(string.Format("{0}({1})", de.Key, de.Value), de.Key);
				}

				foreach (string s in sort.Keys)
				{
					sb.Write(' ');
					sb.Write(s);
				}
				sb.WriteLine();
			}
		}

		public bool IsLiveOut(Identifier id)
		{
			FlagGroupStorage flags = id.Storage as FlagGroupStorage;
			if (flags != null)
			{
				uint grf = flags.FlagGroup;
				return ((grf & grfLiveOut) != 0);
			}
			RegisterStorage reg = id.Storage as RegisterStorage;
			if (reg != null)
			{
				return LiveOut[reg.Register.Number];
			}
			return false;
		}

		public Procedure Procedure
		{
			get { return proc; } 
		}
	}
}
