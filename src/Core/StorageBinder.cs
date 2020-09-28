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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core
{
    /// <summary>
    /// Binds storages to identifiers.
    /// </summary>
    public class StorageBinder : IStorageBinder, StorageVisitor<Identifier>
    {
        private Dictionary<RegisterStorage, Identifier> regs;
        private Dictionary<RegisterStorage, Dictionary<uint, Identifier>> grfs;
        private Dictionary<Storage[], Identifier> seqs;
        private Dictionary<int, Identifier> fpus;
        private List<Identifier> ids;

        public StorageBinder()
        {
            this.regs = new Dictionary<RegisterStorage, Identifier>();
            this.grfs = new Dictionary<RegisterStorage, Dictionary<uint, Identifier>>();
            this.seqs = new Dictionary<Storage[], Identifier>(new Storage.ArrayComparer());
            this.fpus = new Dictionary<int, Identifier>();
            this.ids = new List<Identifier>();
        }

        public Identifier CreateTemporary(DataType dt)
        {
            var name = "v" + ids.Count;
            var tmp = new TemporaryStorage(name, ids.Count, dt);
            var id = new Identifier(name, dt, tmp);
            ids.Add(id);
            return id;
        }

        public Identifier CreateTemporary(string name, DataType dt)
        {
            var tmp = new TemporaryStorage(name, ids.Count, dt);
            var id = new Identifier(name, dt, tmp);
            ids.Add(id);
            return id;
        }

        public Identifier EnsureFlagGroup(FlagGroupStorage grf)
        {
            return EnsureFlagGroup(grf.FlagRegister, grf.FlagGroupBits, grf.Name, grf.DataType);
        }

        public Identifier EnsureFlagGroup(RegisterStorage flagRegister, uint flagGroupBits, string name, DataType dataType)
        {
            if (!this.grfs.TryGetValue(flagRegister, out var grfs))
            {
                grfs = new Dictionary<uint, Identifier>();
                this.grfs.Add(flagRegister, grfs);
            }
            if (grfs.TryGetValue(flagGroupBits, out var id))
                return id;
            var grf = new FlagGroupStorage(flagRegister, flagGroupBits, name, dataType);
            id = new Identifier(name, dataType, grf);
            grfs.Add(flagGroupBits, id);
            ids.Add(id);
            return id;
        }

        public Identifier EnsureFpuStackVariable(int v, DataType dataType)
        {
            if (this.fpus.TryGetValue(v, out var id))
                return id;
            var fpu = new FpuStackStorage(v, dataType);
            id = new Identifier(fpu.Name, fpu.DataType, fpu);
            this.fpus.Add(v, id);
            ids.Add(id);
            return id;
        }

        public Identifier EnsureIdentifier(Storage stg)
        {
            switch (stg)
            {
            case RegisterStorage reg: return EnsureRegister(reg);
            case SequenceStorage seq: return EnsureSequence(seq.DataType, seq.Name, seq.Elements);
            default: throw new NotImplementedException();
            }
        }

        public Identifier EnsureOutArgument(Identifier idOrig, DataType outArgumentPointer)
        {
            throw new NotImplementedException();
        }

        public Identifier EnsureRegister(RegisterStorage reg)
        {
            if (reg == null)
                return null;
            if (regs.TryGetValue(reg, out var id))
                return id;
            id = new Identifier(reg.Name, reg.DataType, reg);
            regs.Add(reg, id);
            ids.Add(id);
            return id;
        }

        public Identifier EnsureSequence(DataType dataType, params Storage [] elements)
        {
            var stg = new SequenceStorage(elements);
            if (this.seqs.TryGetValue(elements, out var idSeq))
            {
                return idSeq;
            }
            var name = string.Join("_", elements.Select(e => e.Name));
            var seq = new SequenceStorage(dataType, elements);
            var id = new Identifier(name, dataType, seq);
            seqs.Add(seq.Elements, id);
            ids.Add(id);
            return id;
        }


        public Identifier EnsureSequence(DataType dataType, string name, params Storage [] elements)
        {
            var stg = new SequenceStorage(elements);
            if (this.seqs.TryGetValue(elements, out var idSeq))
            {
                return idSeq;
            }
            var seq = new SequenceStorage(dataType, elements);
            var id = new Identifier(name, dataType, seq);
            seqs.Add(seq.Elements, id);
            ids.Add(id);
            return id;
        }

        public Identifier EnsureStackVariable(int offset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        Identifier StorageVisitor<Identifier>.VisitFlagGroupStorage(FlagGroupStorage grf)
        {
            return this.EnsureFlagGroup(grf);
        }

        Identifier StorageVisitor<Identifier>.VisitFpuStackStorage(FpuStackStorage fpu)
        {
            throw new NotImplementedException();
        }

        Identifier StorageVisitor<Identifier>.VisitMemoryStorage(MemoryStorage global)
        {
            throw new NotImplementedException();
        }

        Identifier StorageVisitor<Identifier>.VisitStackLocalStorage(StackLocalStorage local)
        {
            throw new NotImplementedException();
        }

        Identifier StorageVisitor<Identifier>.VisitOutArgumentStorage(OutArgumentStorage arg)
        {
            throw new NotImplementedException();
        }

        Identifier StorageVisitor<Identifier>.VisitRegisterStorage(RegisterStorage reg)
        {
            return EnsureRegister(reg);
        }

        Identifier StorageVisitor<Identifier>.VisitSequenceStorage(SequenceStorage seq)
        {
            return EnsureSequence(PrimitiveType.CreateWord((int)seq.BitSize), seq.Elements);
        }

        Identifier StorageVisitor<Identifier>.VisitStackArgumentStorage(StackArgumentStorage stack)
        {
            throw new NotImplementedException();
        }

        Identifier StorageVisitor<Identifier>.VisitTemporaryStorage(TemporaryStorage temp)
        {
            throw new NotImplementedException();
        }
    }
}