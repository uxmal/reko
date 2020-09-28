#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Operators;
using Reko.Core.Types;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Reko.UnitTests.Mocks
{
    /// <summary>
    /// Supports the building of a procedure without having to go through assembler.
    /// </summary>
    public class ProcedureBuilder : CodeEmitter
    {
        private Dictionary<string, Block> blocks;
        private Block branchBlock;
        private Block lastBlock;
        private int numBlock;
        private List<ProcUpdater> unresolvedProcedures;
        public uint LinearAddress;

        public ProcedureBuilder()
        {
            Init(new FakeArchitecture(), this.GetType().Name, Address.Ptr32(0x00123400), null);
        }

        public ProcedureBuilder(string name)
        {
            Init(new FakeArchitecture(), name, Address.Ptr32(0x00123400), null);
        }

        public ProcedureBuilder(IProcessorArchitecture arch)
        {
            Init(arch, this.GetType().Name, Address.Ptr32(0x00123400), null);
        }

        public ProcedureBuilder(IProcessorArchitecture arch, string name)
        {
            Init(arch,name, Address.Ptr32(0x00123400), null);
        }

        public ProcedureBuilder(IProcessorArchitecture arch, string name, Address addr)
        {
            Init(arch, name, addr, null);
        }

        public ProcedureBuilder(IProcessorArchitecture arch, string name, Dictionary<string, Block> blocks)
        {
            Init(arch, name, Address.Ptr32(0x00123400), blocks);
        }

        private void Init(IProcessorArchitecture arch, string name, Address addr, Dictionary<string, Block> blocks)
        {
            if (arch == null)
                throw new ArgumentNullException("arch");
            this.InstructionSize = 1;
            this.Architecture = arch;
            this.Procedure = new Procedure(arch, name, addr, arch.CreateFrame());
            this.blocks = blocks ?? new Dictionary<string, Block>();
            this.unresolvedProcedures = new List<ProcUpdater>();
            BuildBody();
        }

        /// <summary>
        /// Current block, into which the next statement will be added.
        /// </summary>
        public Block Block { get; private set; }
        public Procedure Procedure { get; private set; }
        public ProgramBuilder ProgramBuilder { get; set; }
        public IProcessorArchitecture Architecture { get; private set; }
        public int InstructionSize { get; set; }

        protected Block BlockOf(string label)
        {
            if (!blocks.TryGetValue(label, out Block b))
            {
                b = Procedure.AddBlock(label);
                blocks.Add(label, b);
            }
            return b;
        }

        public Statement BranchIf(Expression expr, string label)
        {
            Block b = EnsureBlock(null);
            branchBlock = BlockOf(label);

            Emit(new Branch(expr, branchBlock));
            TerminateBlock();
            return b.Statements.Last;
        }

        protected virtual void BuildBody()
        {
        }

        public CallInstruction Call(string procedureName, int retSizeOnStack)
        {
            var ci = new CallInstruction(Constant.Invalid, new CallSite(retSizeOnStack, 0)); 
            unresolvedProcedures.Add(new ProcedureConstantUpdater(procedureName, ci));
            Emit(ci);
            return ci;
        }

        public CallInstruction Call(ProcedureBase callee, int retSizeOnStack)
        {
            var c = new ProcedureConstant(PrimitiveType.Ptr32, callee);
            var ci = new CallInstruction(c, new CallSite(retSizeOnStack, 0));  
            Emit(ci);
            return ci;
        }

        public CallInstruction Call(Expression e, int retSizeOnstack)
        {
            var ci = new CallInstruction(e, new CallSite(retSizeOnstack, 0));
            Emit(ci);
            return ci;
        }

        public Statement Call(
            Expression e,
            int retSizeOnstack,
            IEnumerable<Identifier> uses,
            IEnumerable<Identifier> definitions)
        {
            var ci = new CallInstruction(e, new CallSite(retSizeOnstack, 0));
            ci.Uses.UnionWith(uses.Select(u => new CallBinding(u.Storage, u)));
            ci.Definitions.UnionWith(definitions.Select(d => new CallBinding(d.Storage, d)));
            return Emit(ci);
        }

        public Statement Call(
               string procedureName,
               int retSizeOnStack,
               IEnumerable<Identifier> uses,
               IEnumerable<Identifier> definitions)
        {
            var ci = new CallInstruction(Constant.Invalid, new CallSite(retSizeOnStack, 0));
            ci.Uses.UnionWith(uses.Select(u => new CallBinding(u.Storage, u)));
            ci.Definitions.UnionWith(definitions.Select(d => new CallBinding(d.Storage, d)));
            unresolvedProcedures.Add(new ProcedureConstantUpdater(procedureName, ci));
            return Emit(ci);
        }

        public Statement Call(
            string procedureName,
            int retSizeOnStack,
            IEnumerable<(Storage stg, Expression e)> uses,
            IEnumerable<(Storage stg, Identifier e)> definitions)
        {
            var ci = new CallInstruction(Constant.Invalid, new CallSite(retSizeOnStack, 0));
            ci.Uses.UnionWith(uses.Select(u => new CallBinding(u.stg, u.e)));
            ci.Definitions.UnionWith(definitions.Select(d => new CallBinding(d.stg, d.e)));
            unresolvedProcedures.Add(new ProcedureConstantUpdater(procedureName, ci));
            return Emit(ci);
        }


        public void Compare(string flags, Expression a, Expression b)
        {
            Assign(Flags(flags), new ConditionOf(ISub(a, b)));
        }

        public Block CurrentBlock
        {
            get { return this.Block; }
        }

        public Identifier Declare(DataType dt, string name)
        {
            return Procedure.Frame.CreateTemporary(name, dt);
        }

        public Identifier Declare(DataType dt, string name, Expression expr)
        {
            Identifier id = Procedure.Frame.CreateTemporary(name, dt);
            Emit(new Declaration(id, expr));
            return id;
        }

        public Statement Declare(Identifier id, Expression initial)
        {
            return Emit(new Declaration(id, initial));
        }

        public override Statement Emit(Instruction instr)
        {
            EnsureBlock(null);
            Block.Statements.Add(LinearAddress , instr);
            LinearAddress += (uint)InstructionSize;
            return Block.Statements.Last;
        }

        public Identifier Flags(string s)
        {
            return Frame.EnsureFlagGroup(Architecture.GetFlagGroup(s));
        }

        public Application Fn(string name, params Expression[] exps)
        {
            Application appl = new Application(
                new ProcedureConstant(PrimitiveType.Ptr32, new PseudoProcedure(name, VoidType.Instance, 0)),
                PrimitiveType.Word32, exps);
            unresolvedProcedures.Add(new ApplicationUpdater(name, appl));
            return appl;
        }

        public void Goto(string name)
        {
            EnsureBlock(null);
            Block blockTo = BlockOf(name);
            Procedure.ControlGraph.AddEdge(Block, blockTo);
            Block = null;
        }

        public Block Label(string name)
        {
            TerminateBlock();
            return EnsureBlock(name);
        }

        private Block EnsureBlock(string name)
        {
            if (Block != null)
                return Block;

            if (name == null)
            {
                name = string.Format("l{0}", ++numBlock);
            }
            Block = BlockOf(name);
            Block.Address = Address.Ptr32(LinearAddress);
            if (Procedure.EntryBlock.Succ.Count == 0)
            {
                Procedure.ControlGraph.AddEdge(Procedure.EntryBlock, Block);
            }

            if (lastBlock != null)
            {
                if (branchBlock != null)
                {
                    Procedure.ControlGraph.AddEdge(lastBlock, Block);
                    Procedure.ControlGraph.AddEdge(lastBlock, branchBlock);
                    branchBlock = null;
                }
                else
                {
                    Procedure.ControlGraph.AddEdge(lastBlock, Block);
                }
                lastBlock = null;
            }
            return Block;
        }

        public void FinishProcedure()
        {
            TerminateBlock();
            Procedure.ControlGraph.AddEdge(lastBlock, Procedure.ExitBlock);
        }

        public ICollection<ProcUpdater> UnresolvedProcedures
        {
            get { return unresolvedProcedures; }
        }

        public override Frame Frame
        {
            get { return Procedure.Frame; }
        }

        public Statement Phi(Identifier idDst, params (Expression, string)[] exprs)
        {
            var phi = new PhiFunction(
                idDst.DataType,
                exprs.Select(de => new PhiArgument(
                    BlockOf(de.Item2),
                    de.Item1))
                .ToArray());
            return Emit(new PhiAssignment(idDst, phi));
        }

        public Identifier Register(string name)
        {
            return Frame.EnsureRegister(Architecture.GetRegister(name));
        }

        public Identifier Register(RegisterStorage reg)
        {
            return Frame.EnsureRegister(reg);
        }

        public override void Return()
        {
            base.Return();
            Procedure.ControlGraph.AddEdge(Block, Procedure.ExitBlock);
            TerminateBlock();
            lastBlock = null;
        }

        public override void Return(Expression exp)
        {
            base.Return(exp);
            Procedure.ControlGraph.AddEdge(Block, Procedure.ExitBlock);
            TerminateBlock();
            lastBlock = null;
        }

        public void Switch(Expression e, params string[] labels)
        {
            Block[] blox = new Block[labels.Length];
            for (int i = 0; i < blox.Length; ++i)
            {
                blox[i] = BlockOf(labels[i]);
            }

            Emit(new SwitchInstruction(e, blox));
            for (int i = 0; i < blox.Length; ++i)
            {
                Procedure.ControlGraph.AddEdge(this.Block, blox[i]);
            }
            lastBlock = null;
            Block = null;
        }

        public void TerminateBlock()
        {
            if (Block != null)
            {
                lastBlock = Block;
                Block = null;
            }
        }

        /// <summary>
        /// Call this method right after a terminating function.
        /// </summary>
        public void ExitThread()
        {
            lastBlock = null;
            Block = null;
        }

        public Identifier Reg64(string name, int number)
        {
            return Frame.EnsureRegister(new RegisterStorage(name, number, 0, PrimitiveType.Word64));
        }

        public virtual Identifier Reg32(string name, int number)
        {
            return Frame.EnsureRegister(new RegisterStorage(name, number, 0, PrimitiveType.Word32));
        }

        public Identifier Reg32(string name)
        {
            return Frame.EnsureRegister(Architecture.GetRegister(name));
        }

        public Identifier Reg16(string name, int number)
        {
            return Frame.EnsureRegister(new RegisterStorage(name, number, 0, PrimitiveType.Word16));
        }

        public Identifier Reg8(string name, int number)
        {
            return Frame.EnsureRegister(new RegisterStorage(name, number, 0, PrimitiveType.Byte));
        }

        // Use this method to model the x86 "ah" or the z80 "h" registers which are 
        // offset from the start of their word registers.
        public Identifier Reg8(string name, int number, uint bitOffset)
        {
            return Frame.EnsureRegister(new RegisterStorage(name, number, bitOffset, PrimitiveType.Byte));
        }
    }
}
