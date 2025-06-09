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

#nullable enable

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Mocks
{
    /// <summary>
    /// Supports building a intermediate code program directly without having
    /// to first generate machine code and scanning it.
    /// </summary>
    public class ProgramBuilder
	{
		private uint procCount;
        private readonly Dictionary<string, Procedure> nameToProcedure = new Dictionary<string, Procedure>();
		private readonly List<ProcUpdater> unresolvedProcedures = new List<ProcUpdater>();

		public ProgramBuilder() : this(new FakeArchitecture(new ServiceContainer()))
		{
		}

        public ProgramBuilder(IProcessorArchitecture arch)
        {
            Program = new Program {
                Architecture = arch,
                Platform = new DefaultPlatform(arch.Services, arch)
            };
        }

        public ProgramBuilder(ByteMemoryArea mem)
        {
            var segmentMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment("code", mem, AccessMode.ReadWriteExecute));
            Program = new Program
            {
                Memory = new ByteProgramMemory(segmentMap),
                SegmentMap = segmentMap,
                Architecture = new FakeArchitecture(new ServiceContainer())
            };
        }

		public Program Program { get; set; }

        public void Add(Procedure proc, UserProcedure? userProc = null)
        {
            ++procCount;
            var addr = Address.Ptr32(procCount * 0x1000u);
            Program.Procedures[addr] = proc;
            Program.CallGraph.AddProcedure(proc);
            var name = GuessName(userProc, proc);
            if (name is not null)
            {
                nameToProcedure[name] = proc;
            }
            if (userProc is not null)
            {
                Program.User.Procedures[addr] = userProc;
            }
        }

        private string? GuessName(UserProcedure? userProc, Procedure? proc = null)
        {
            if (userProc is not null)
            {
                if (!string.IsNullOrEmpty(userProc.Name))
                    return userProc.Name;
                if (!string.IsNullOrEmpty(userProc.CSignature))
                {
                    int i = userProc.CSignature.IndexOf('(');
                    if (i > 0)
                    {
                        var name = userProc.CSignature.Remove(i);
                        do
                        {
                            --i;
                        } while (i > 0 && (char.IsLetterOrDigit(name[i]) || name[i] == '_'));
                        return name[(i + 1)..];
                    }
                }
            }
            if (proc is not null)
            {
                return proc.Name;
            }
            else
            {
                return null;
            }
        }

        public void Add(ProcedureBuilder mock)
        {
            mock.ProgramBuilder = this;
            Add(mock.Procedure, null);
            unresolvedProcedures.AddRange(mock.UnresolvedProcedures);
        }

        public Address NextAddress() => Address.Ptr32((procCount + 1) * 0x1000u);

        public Procedure Add(string procName, Action<ProcedureBuilder> testCodeBuilder)
        {
            return Add(new UserProcedure(NextAddress(), procName), testCodeBuilder);
        }

        public Procedure Add(UserProcedure userProc, Action<ProcedureBuilder> testCodeBuilder)
        {
            var mock = new ProcedureBuilder(Program.Architecture, GuessName(userProc));
            mock.ProgramBuilder = this;
            mock.addrCur = userProc.Address;
            testCodeBuilder(mock);
            Add(mock.Procedure, userProc);
            unresolvedProcedures.AddRange(mock.UnresolvedProcedures);
            return mock.Procedure;
        }

        public void BuildCallgraph()
        {
            foreach (Procedure proc in Program.Procedures.Values)
            {
                foreach (Statement stm in proc.Statements)
                {
                    if (stm.Instruction is CallInstruction call &&
                        call.Callee is ProcedureConstant pc &&
                        pc.Procedure is Procedure callee)
                    {
                        Program.CallGraph.AddEdge(stm, callee);
                    }
                }
            }
        }

        public Program BuildProgram()
        {
            return BuildProgram(Program.Architecture);
        }

		public Program BuildProgram(IProcessorArchitecture arch)
		{
            Program.Architecture = arch;
            ResolveUnresolved();
			BuildCallgraph();
            if (Program.SegmentMap is null)
                Program.SegmentMap = new SegmentMap(Address.Ptr16(0x1000));
            Program.SegmentMap.AddSegment(
                new ByteMemoryArea(Address.Ptr32(0x1000), new byte[Program.Procedures.Count * 0x1000]),
                ".text", AccessMode.Execute);
                
            Program.Platform = new DefaultPlatform(new ServiceContainer(), arch);
            Program.Memory = new ByteProgramMemory(Program.SegmentMap);
			return Program;
		}

		public void ResolveUnresolved()
		{
			foreach (ProcUpdater pcu in unresolvedProcedures)
			{
                if (!nameToProcedure.TryGetValue(pcu.Name, out var proc))
					throw new ApplicationException("Unresolved procedure name: " + pcu.Name);
				pcu.Update(proc);
			}
            unresolvedProcedures.Clear();
		}
	}

	public abstract class ProcUpdater
	{
		private readonly string name;
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
		private readonly CallInstruction ci;

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
		private readonly Application appl;

		public ApplicationUpdater(string name, Application appl) : base(name)
		{
			this.appl = appl;
		}

		public override void Update(Procedure proc)
		{
			appl.Procedure = new ProcedureConstant(PrimitiveType.Ptr32, proc);
		}
	}
}

