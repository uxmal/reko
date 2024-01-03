#region License
/* 
 * Copyright (C) 1999-2024 Pavel Tomin.
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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Mocks
{
    /// <summary>
    /// Supports the building of a procedure in Static Single Assignment form.
    /// </summary>
    /// <remarks>
    /// Some unit tests require procedure to be in Static Single Assignment
    /// form. This class gives possibility to build it without the overhead of
    /// using the SSATransform class.
    /// </remarks>
    public class SsaProcedureBuilder : ProcedureBuilder
    {
        public SsaState Ssa { get; private set; }

        public SsaProcedureBuilder(IProcessorArchitecture arch = null, string name = nameof(SsaProcedureBuilder)) 
            : base(arch ?? new FakeArchitecture(new ServiceContainer()), name)
        {
            this.Ssa = new SsaState(Procedure);
        }

        public SsaProcedureBuilder(string name) : this(null, name)
        {
        }

        public RegisterStorage RegisterStorage(string name, PrimitiveType pt)
        {
            return new RegisterStorage(name, Ssa.Identifiers.Count, 0, pt);
        }

        public Identifier Reg(string name, RegisterStorage r)
        {
            var id = new Identifier(r.Name, r.DataType, r);
            return MakeSsaIdentifier(id, name);
        }

        private Identifier Reg(string name, PrimitiveType pt)
        {
            return Reg(name, RegisterStorage(name, pt));
        }

        public Identifier Flags(string name, FlagGroupStorage flags)
        {
            var id = new Identifier(flags.Name, flags.DataType, flags);
            return MakeSsaIdentifier(id, name);
        }

        public Identifier SeqId(string name, DataType dt, params Storage[] storages)
        {
            var id = new Identifier(name, dt, new SequenceStorage(dt, storages));
            return MakeSsaIdentifier(id, name);
        }

        public Identifier Local(PrimitiveType primitiveType, string name, int offset)
        {
            var local = base.Local(primitiveType, name);
            return MakeSsaIdentifier(local, name);
        }

        public override Identifier Local16(string name, int offset)
        {
            var local = base.Local16(null, offset);
            return MakeSsaIdentifier(local, name);
        }

        public override Identifier Local32(string name, int offset)
        {
            var local = base.Local32(name, offset);
            return MakeSsaIdentifier(local, name);
        }

        public override Identifier Temp(DataType type, string name)
        {
            var id = base.Temp(type, name);
            return MakeSsaIdentifier(id, name);
        }

        public Identifier Temp(string name, TemporaryStorage stg)
        {
            var id = new Identifier(stg.Name, stg.DataType, stg);
            return MakeSsaIdentifier(id, name);
        }

        public Identifier Reg64(string name)
        {
            return Reg(name, PrimitiveType.Word64);
        }

        public override Identifier Reg32(string name)
        {
            return Reg(name, PrimitiveType.Word32);
        }

        public override Identifier Reg64(string name, int number)
        {
            var id = base.Reg64(name, number);
            return MakeSsaIdentifier(id, name);
        }

        public override Identifier Reg32(string name, int number)
        {
            var id = base.Reg32(name, number);
            return MakeSsaIdentifier(id, name);
        }

        public Identifier Reg16(string name)
        {
            return Reg(name, PrimitiveType.Word16);
        }

        public Identifier Reg8(string name)
        {
            return Reg(name, PrimitiveType.Byte);
        }

        public override Identifier Reg8(string name, int number)
        {
            var id = base.Reg8(name, number);
            return MakeSsaIdentifier(id, name);
        }

        public override Statement Emit(Instruction instr)
        {
            var stm = base.Emit(instr);
            ProcessInstruction(instr, stm);
            return stm;
        }

        public Identifier FramePointer()
        {
            var sidFp = Ssa.Identifiers.Add(Frame.FramePointer, null, false);
            return sidFp.Identifier;
        }

        private Identifier MakeSsaIdentifier(Identifier id, string name)
        {
            var idNew = new Identifier(name, id.DataType, id.Storage);
            var sid = new SsaIdentifier(idNew, id, null, false);
            Ssa.Identifiers.Add(idNew, sid);
            return sid.Identifier;
        }

        private void ProcessInstruction(Instruction instr, Statement stm)
        {
            switch (instr)
            {
            case Assignment ass:
                Ssa.Identifiers[ass.Dst].DefStatement = stm;
                break;
            case PhiAssignment phiAss:
                Ssa.Identifiers[phiAss.Dst].DefStatement = stm;
                break;
            case Store store:
                if (store.Dst is MemoryAccess access)
                {
                    var memId = AddMemIdToSsa(access.MemoryId);
                    Ssa.Identifiers[memId].DefStatement = stm;
                    var ea = access.EffectiveAddress;
                    var dt = access.DataType;
                    if (access.EffectiveAddress is SegmentedPointer segptr)
                    {
                        ea = SegmentedPointer.Create(segptr.BasePointer, segptr.Offset);
                    }
                    store.Dst = new MemoryAccess(memId, ea, dt);
                }
                break;
            case CallInstruction call:
                foreach (var def in call.Definitions)
                {
                    var id = (Identifier) def.Expression;
                    Ssa.Identifiers[id].DefStatement = stm;
                }
                break;
            case DefInstruction def:
                Ssa.Identifiers[def.Identifier].DefStatement = stm;
                break;
            }
            Ssa.AddUses(stm);
        }

        public void AddDefToEntryBlock(Identifier id)
        {
            var def = new DefInstruction(id);
            var stm = Procedure.EntryBlock.Statements.Add(Procedure.EntryAddress, def);
            ProcessInstruction(def, stm);
        }

        public void AddUseToExitBlock(Identifier id)
        {
            var use = new UseInstruction(id);
            var stm = Procedure.ExitBlock.Statements.Add(Address.Ptr32(0), use);
            ProcessInstruction(use, stm);
        }

        public void AddPhiToExitBlock(Identifier idDst, params (Expression, string)[] exprs)
        {
            var args = exprs
                .Select(de => new PhiArgument(BlockOf(de.Item2), de.Item1))
                .ToArray();
            var phiFunc = new PhiFunction(idDst.DataType, args);
            var phi = new PhiAssignment(idDst, phiFunc);
            var stm = Procedure.ExitBlock.Statements.Add(Address.Ptr32(0), phi);
            ProcessInstruction(phi, stm);
        }

        private Identifier AddMemIdToSsa(Identifier idOld)
        {
            if (Ssa.Identifiers.Contains(idOld))
                return idOld;
            Identifier idNew;
            if (idOld.Storage == MemoryStorage.Instance)
            {
                idNew = new Identifier(
                    "Mem" + Ssa.Identifiers.Count,
                    idOld.DataType,
                    idOld.Storage);
            }
            else
            {
                idNew = new Identifier(
                    idOld.Name,
                    idOld.DataType,
                    idOld.Storage);
            }
            var sid = new SsaIdentifier(idNew, idOld, null, false);
            Ssa.Identifiers.Add(idNew, sid);
            return idNew;
        }

        public override MemoryAccess Mem(
            Identifier mid,
            DataType dt,
            Expression ea)
        {
            var access = base.Mem(mid, dt, ea);
            var memId = AddMemIdToSsa(access.MemoryId);
            return new MemoryAccess(
                memId,
                access.EffectiveAddress,
                access.DataType);
        }

        public override MemoryAccess Mem8(Expression ea)
        {
            var access = base.Mem8(ea);
            var memId = AddMemIdToSsa(access.MemoryId);
            return new MemoryAccess(
                memId,
                access.EffectiveAddress,
                access.DataType);
        }

        public override MemoryAccess Mem16(Expression ea)
        {
            var access = base.Mem16(ea);
            var memId = AddMemIdToSsa(access.MemoryId);
            return new MemoryAccess(
                memId,
                access.EffectiveAddress,
                access.DataType);
        }

        public override MemoryAccess Mem32(Expression ea)
        {
            var access = base.Mem32(ea);
            var memId = AddMemIdToSsa(access.MemoryId);
            return new MemoryAccess(
                memId,
                access.EffectiveAddress,
                access.DataType);
        }

        public override MemoryAccess Mem64(Expression ea)
        {
            var access = base.Mem64(ea);
            var memId = AddMemIdToSsa(access.MemoryId);
            return new MemoryAccess(
                memId,
                access.EffectiveAddress,
                access.DataType);
        }

        public override MemoryAccess SegMem(DataType dt, Expression basePtr, Expression ptr)
        {
            var access = base.SegMem(dt, basePtr, ptr);
            var memId = AddMemIdToSsa(access.MemoryId);
            return new MemoryAccess(
                memId,
                access.EffectiveAddress,
                access.DataType);
        }
    }
}
