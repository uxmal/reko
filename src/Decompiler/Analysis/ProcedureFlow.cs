#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Reko.Analysis
{
	/// <summary>
	/// Describes the flow of registers in and out of a procedure. 
	/// We are usually interested in registers modified, live in, &c.
	/// </summary>
	public class ProcedureFlow : DataFlow
	{
		private Procedure proc;

		public HashSet<RegisterStorage> PreservedRegisters;			// Registers explicitly preserved by the procedure.
		public uint grfPreserved;

		public uint grfTrashed;
		public HashSet<RegisterStorage> TrashedRegisters;		// Registers globally trashed by procedure and/or callees.
        public Dictionary<Storage, Constant> ConstantRegisters; // If present, indicates a register always has a constant value leaving the procedure.

		public HashSet<RegisterStorage> ByPass { get; set; }
		public uint grfByPass;
		public HashSet<RegisterStorage> MayUse;
		public uint grfMayUse;
		public HashSet<RegisterStorage> Summary;
		public uint grfSummary;

		public Hashtable StackArguments;		//$REFACTOR: make this a strongly typed dictionary (Var -> PrimitiveType)

		public HashSet<RegisterStorage> LiveOut;
		public uint grfLiveOut;

		public FunctionType Signature;

        // True if calling this procedure terminates the thread/process. This implies
        // that no code path reached the exit block without first terminating the process.
        public bool TerminatesProcess;

        public ProcedureFlow(Procedure proc, IProcessorArchitecture arch)
        {
            this.proc = proc;

            PreservedRegisters = new HashSet<RegisterStorage>();
            TrashedRegisters = new HashSet<RegisterStorage>();
            ConstantRegisters = new Dictionary<Storage, Constant>();

            ByPass = new HashSet<RegisterStorage>();
            MayUse = new HashSet<RegisterStorage>();
            LiveOut = new HashSet<RegisterStorage>();

            StackArguments = new Hashtable();
        }

        [Conditional("DEBUG")]
        public void Dump(IProcessorArchitecture arch)
        {
            var sb = new StringWriter();
            Emit(arch, sb);
            Debug.WriteLine(sb.ToString());
        }

		public override void Emit(IProcessorArchitecture arch, TextWriter writer)
		{
			EmitRegisters(arch, "// MayUse: ", grfMayUse, MayUse, writer);
			writer.WriteLine();
			EmitRegisters(arch, "// LiveOut:", grfLiveOut, LiveOut, writer);
			writer.WriteLine();
			EmitRegisters(arch, "// Trashed:", grfTrashed, TrashedRegisters, writer);
			writer.WriteLine();
			EmitRegisters(arch, "// Preserved:", grfPreserved, PreservedRegisters, writer);
			writer.WriteLine();
			EmitStackArguments(StackArguments, writer);
            if (TerminatesProcess)
                writer.WriteLine("// Terminates process");
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
				uint grf = flags.FlagGroupBits;
				return ((grf & grfLiveOut) != 0);
			}
			RegisterStorage reg = id.Storage as RegisterStorage;
			if (reg != null)
			{
                return LiveOut.Contains(reg);
			}
			return false;
		}

		public Procedure Procedure
		{
			get { return proc; } 
		}
    }
}
