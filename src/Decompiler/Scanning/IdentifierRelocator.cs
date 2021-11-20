#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    /// <summary>
    /// Relocates identifiers in an instruction from their current frame to an
    /// another given frame.
    /// </summary>
    public class IdentifierRelocator : InstructionTransformer, StorageVisitor<Identifier>
    {
        private readonly Frame frameOld;
        private readonly Frame frameNew;
        private readonly Dictionary<Identifier, Identifier> mapIds;
        private Identifier? id; //$TODO: use 'id' as context?

        public IdentifierRelocator(Frame frameOld, Frame frameNew)
        {
            this.frameOld  = frameOld;
            this.frameNew = frameNew;
            this.mapIds = new Dictionary<Identifier, Identifier>();
        }

        public Instruction ReplaceIdentifiers(Instruction instr)
        {
            return instr.Accept(this);
        }

        public override Expression VisitIdentifier(Identifier id)
        {
            this.id = id;
            if (!mapIds.TryGetValue(id, out Identifier idNew))
            {
                idNew = id.Storage.Accept(this);
                mapIds.Add(id, idNew);
            }
            return idNew;
        }

        public Identifier VisitFlagGroupStorage(FlagGroupStorage flags)
        {
            return frameNew.EnsureFlagGroup(flags.FlagRegister, flags.FlagGroupBits, flags.Name, id!.DataType);
        }

        public Identifier VisitFpuStackStorage(FpuStackStorage fpu)
        {
            return frameNew.EnsureFpuStackVariable(fpu.FpuStackOffset, id!.DataType);
        }

        public Identifier VisitRegisterStorage(RegisterStorage reg)
        {
            return frameNew.EnsureRegister(reg);
        }

        public Identifier VisitMemoryStorage(MemoryStorage mem)
        {
            if (id! == frameOld.Memory)
                return frameNew.Memory;
            else
                return id!;
        }

        public Identifier VisitStackArgumentStorage(StackArgumentStorage arg)
        {
            return frameNew.EnsureStackArgument(arg.StackOffset, arg.DataType);
        }

        public Identifier VisitStackLocalStorage(StackLocalStorage loc)
        {
            return frameNew.EnsureStackLocal(loc.StackOffset, loc.DataType);
        }

        public Identifier VisitTemporaryStorage(TemporaryStorage tmp)
        {
            if (tmp is GlobalStorage)
                return id!;
            return frameNew.CreateTemporary(id!.DataType);
        }

        public Identifier VisitSequenceStorage(SequenceStorage seq)
        {
            return frameNew.EnsureSequence(id!.DataType, seq.Elements);
        }

        public Identifier VisitOutArgumentStorage(OutArgumentStorage ost)
        {
            var idOut = id;
            return frameNew.EnsureOutArgument((Identifier) VisitIdentifier(ost.OriginalIdentifier), idOut!.DataType);
        }
    }
}
