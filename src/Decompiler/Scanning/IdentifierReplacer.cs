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
using Reko.Core.Expressions;
using Reko.Core.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    public class IdentifierReplacer : InstructionTransformer, StorageVisitor<Identifier>
    {
        private Frame frame;
        private Dictionary<Identifier, Identifier> mapIds;
        private Identifier id;

        public IdentifierReplacer(Frame frame)
        {
            this.frame = frame;
            this.mapIds = new Dictionary<Identifier, Identifier>();
        }

        public Instruction ReplaceIdentifiers(Instruction instr)
        {
            return instr.Accept(this);
        }

        public override Expression VisitIdentifier(Identifier id)
        {
            this.id = id;
            Identifier idNew;
            if (!mapIds.TryGetValue(id, out idNew))
            {
                idNew = id.Storage.Accept(this);
                mapIds.Add(id, idNew);
            }
            return idNew;
        }

        public Identifier VisitFlagGroupStorage(FlagGroupStorage flags)
        {
            return frame.EnsureFlagGroup(flags.FlagRegister, flags.FlagGroupBits, flags.Name, id.DataType);
        }

        public Identifier VisitFpuStackStorage(FpuStackStorage fpu)
        {
            return frame.EnsureFpuStackVariable(fpu.FpuStackOffset, id.DataType);
        }

        public Identifier VisitRegisterStorage(RegisterStorage reg)
        {
            return frame.EnsureRegister(reg);
        }

        public Identifier VisitMemoryStorage(MemoryStorage mem)
        {
            return frame.Memory;
        }

        public Identifier VisitStackArgumentStorage(StackArgumentStorage arg)
        {
            return frame.EnsureStackArgument(arg.StackOffset, arg.DataType);
        }

        public Identifier VisitStackLocalStorage(StackLocalStorage loc)
        {
            return frame.EnsureStackLocal(loc.StackOffset, loc.DataType);
        }

        public Identifier VisitTemporaryStorage(TemporaryStorage tmp)
        {
            return frame.CreateTemporary(id.DataType);
        }

        public Identifier VisitSequenceStorage(SequenceStorage seq)
        {
            var idSeq = id;
            var newElems = seq.Elements.Select(e => e.Accept(this).Storage).ToArray();
            return frame.EnsureSequence(idSeq.DataType, newElems);
        }

        public Identifier VisitOutArgumentStorage(OutArgumentStorage ost)
        {
            var idOut = id;
            return frame.EnsureOutArgument((Identifier) VisitIdentifier(ost.OriginalIdentifier), idOut.DataType);
        }
    }
}
