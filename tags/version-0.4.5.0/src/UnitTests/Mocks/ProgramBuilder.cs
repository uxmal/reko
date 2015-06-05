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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Mocks
{
	/// <summary>
	/// Supports building a intermediate code program directly without having to first generate machine code and scanning it.
	/// </summary>
	public class ProgramBuilder
	{
		private uint procCount;
        private Dictionary<string, Procedure> nameToProcedure = new Dictionary<string, Procedure>();
        private Dictionary<string, Block> blocks = new Dictionary<string, Block>();
		private List<ProcUpdater> unresolvedProcedures = new List<ProcUpdater>();

		public ProgramBuilder() : this(new FakeArchitecture())
		{
		}

        public ProgramBuilder(IProcessorArchitecture arch)
        {
            Program = new Program {
                Architecture = arch,
            };
        }

        public ProgramBuilder(LoadedImage loadedImage)
        {
            Program = new Program
            {
                Image = loadedImage,
                ImageMap = loadedImage.CreateImageMap(),
                Architecture = new FakeArchitecture()
            };
        }

		public Program Program { get; set; }

        public void Add(Procedure proc)
        {
            ++procCount;
            Program.Procedures[Address.Ptr32(procCount * 0x1000u)] = proc;
            Program.CallGraph.AddProcedure(proc);
            nameToProcedure[proc.Name] = proc;
        }

        public void Add(ProcedureBuilder mock)
        {
            mock.ProgramMock = this;
            Add(mock.Procedure);
            unresolvedProcedures.AddRange(mock.UnresolvedProcedures);
        }

        public Procedure Add(string procName, Action<ProcedureBuilder> testCodeBuilder)
        {
            var mock = new ProcedureBuilder(Program.Architecture, procName, null);
            mock.ProgramMock = this;
            mock.LinearAddress = (uint)((procCount + 1) * 0x1000);
            testCodeBuilder(mock);
            Add(mock.Procedure);
            unresolvedProcedures.AddRange(mock.UnresolvedProcedures);
            return mock.Procedure;
        }

        public void BuildCallgraph()
        {
            foreach (Procedure proc in Program.Procedures.Values)
            {
                foreach (Statement stm in proc.Statements)
                {
                    var call = stm.Instruction as CallInstruction;
                    if (call == null)
                        continue;
                    var pc = call.Callee as ProcedureConstant;
                    if (pc == null)
                        continue;
                    var callee = pc.Procedure as Procedure;
                    if (callee == null)
                        continue;
                    Program.CallGraph.AddEdge(stm, callee);
                }
            }
        }

        public Program BuildProgram()
        {
            return BuildProgram(new FakeArchitecture());
        }

		public Program BuildProgram(IProcessorArchitecture arch)
		{
            Program.Architecture = arch;
            ResolveUnresolved();
			BuildCallgraph();
            Program.ImageMap = new ImageMap(Address.Ptr32(0x1000), Program.Procedures.Count * 0x1000);
            var seg = Program.ImageMap.AddSegment(Address.Ptr32(0x1000), ".text", AccessMode.Execute);
            seg.Size = (uint)(Program.Procedures.Count * 0x1000);
            Program.Platform = new DefaultPlatform(null, arch);
			return Program;
		}

		public void ResolveUnresolved()
		{
			foreach (ProcUpdater pcu in unresolvedProcedures)
			{
				Procedure proc;
                if (!nameToProcedure.TryGetValue(pcu.Name, out proc))
					throw new ApplicationException("Unresolved procedure name: " + pcu.Name);
				pcu.Update(proc);
			}
            unresolvedProcedures.Clear();
		}
	}

	public abstract class ProcUpdater
	{
		private string name;
		public ProcUpdater(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		public abstract void Update(Procedure proc);
	}

	public class ProcedureConstantUpdater : ProcUpdater
	{
		private CallInstruction ci;

		public ProcedureConstantUpdater(string name, CallInstruction ci) : base(name)
		{
			this.ci = ci;
		}

		public override void Update(Procedure proc)
		{
			ci.Callee = new ProcedureConstant(PrimitiveType.Word32,proc);
		}

	}

	public class ApplicationUpdater : ProcUpdater
	{
		private Application appl;

		public ApplicationUpdater(string name, Application appl) : base(name)
		{
			this.appl = appl;
		}

		public override void Update(Procedure proc)
		{
			appl.Procedure = new ProcedureConstant(PrimitiveType.Pointer32, proc);
		}
	}
}

