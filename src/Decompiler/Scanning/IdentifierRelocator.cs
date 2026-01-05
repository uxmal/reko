#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

        /// <summary>
        /// Constructs an instance of <see cref="IdentifierRelocator"/>.
        /// </summary>
        /// <param name="frameOld">Old <see cref="Frame"/> from which to obtain the identifiers.
        /// </param>
        /// <param name="frameNew">New <see cref="Frame"/> to which the relocated identifiers are
        /// placed.
        /// </param>
        public IdentifierRelocator(Frame frameOld, Frame frameNew)
        {
            this.frameOld  = frameOld;
            this.frameNew = frameNew;
            this.mapIds = [];
        }

        /// <summary>
        /// Performs the identifier relocation on the given instruction.
        /// </summary>
        /// <param name="instr">Instruction whose identifiers are to be relocated.
        /// </param>
        /// <returns></returns>
        public Instruction ReplaceIdentifiers(Instruction instr)
        {
            return instr.Accept(this);
        }

        /// <inheritdoc/>
        public override Expression VisitIdentifier(Identifier id)
        {
            this.id = id;
            if (!mapIds.TryGetValue(id, out Identifier? idNew))
            {
                idNew = id.Storage.Accept(this);
                mapIds.Add(id, idNew);
            }
            return idNew;
        }

        /// <inheritdoc/>
        public Identifier VisitFlagGroupStorage(FlagGroupStorage flags)
        {
            return frameNew.EnsureFlagGroup(flags);
        }

        /// <inheritdoc/>
        public Identifier VisitFpuStackStorage(FpuStackStorage fpu)
        {
            return frameNew.EnsureFpuStackVariable(fpu.FpuStackOffset, id!.DataType);
        }

        /// <inheritdoc/>
        public Identifier VisitRegisterStorage(RegisterStorage reg)
        {
            return frameNew.EnsureRegister(reg);
        }

        /// <inheritdoc/>
        public Identifier VisitMemoryStorage(MemoryStorage mem)
        {
            if (id! == frameOld.Memory)
                return frameNew.Memory;
            else
                return id!;
        }

        /// <inheritdoc/>
        public Identifier VisitStackStorage(StackStorage arg)
        {
            return frameNew.EnsureStackVariable(arg.StackOffset, arg.DataType);
        }

        /// <inheritdoc/>
        public Identifier VisitTemporaryStorage(TemporaryStorage tmp)
        {
            if (tmp is GlobalStorage)
                return id!;
            return frameNew.CreateTemporary(id!.DataType);
        }

        /// <inheritdoc/>
        public Identifier VisitSequenceStorage(SequenceStorage seq)
        {
            return frameNew.EnsureSequence(id!.DataType, seq.Elements);
        }
    }
}
