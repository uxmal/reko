#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.Analysis
{
	public class TrashStorageHelper : StorageVisitor<Storage>
	{
		private uint grfDefs;
		private Dictionary<Storage, Storage> regDefs;
		private bool defining;
		private Storage value;
		private readonly Storage trash;

		public TrashStorageHelper(Storage trash) : this(new Dictionary<Storage, Storage>(), 0, trash)
		{
		}

		public TrashStorageHelper(Dictionary<Storage,Storage> defs, uint grfDefs, Storage trash)
		{
			this.regDefs = defs;
			this.grfDefs = grfDefs;
            this.trash = trash;
		}

		public TrashStorageHelper Clone()
		{
			return new TrashStorageHelper(new Dictionary<Storage,Storage>(), grfDefs, trash);
		}

		public void Copy(Identifier dst, Identifier src)
		{
			defining = false;
			value = null;
			src.Storage.Accept(this);
			if (value == null)
				value = src.Storage;
			Trash(dst, value);
		}

        public Storage TrashedStorage
        {
            get { return trash; }
        }

		public void Trash(Identifier dst, Storage v)
		{
			defining = true;
			value = v;
			dst.Storage.Accept(this);
		}


		public Dictionary<Storage,Storage> TrashedRegisters
		{
			get { return regDefs; }
		}

		public uint TrashedFlags
		{
			get { return grfDefs; }
			set { grfDefs = value; }
		}

        public void Write(TextWriter writer)
        {
            var sortedList = new SortedList<string, string>();
            foreach (KeyValuePair<Storage,Storage> de in TrashedRegisters)
            {
                sortedList[de.Key.ToString()] = de.Value.ToString();
            }
            foreach (string s in sortedList.Keys)
            {
                writer.Write(" ");
                writer.Write(s);
            }
        }

		#region StorageVisitor Members

		public Storage VisitFlagGroupStorage(FlagGroupStorage grf)
		{
			if (defining)
				this.grfDefs |= grf.FlagGroupBits;
            return grf;
		}

        public Storage VisitFlagRegister(FlagRegister freg)
        {
            throw new NotImplementedException();
        }

        public Storage VisitFpuStackStorage(FpuStackStorage fpu)
		{
            if (defining)
                regDefs[fpu] = value;
            else
            {
                value = null;
                regDefs.TryGetValue(fpu, out value);
            }
            return fpu;
		}

		public Storage VisitMemoryStorage(MemoryStorage global)
		{
			throw new NotImplementedException();
		}

		public Storage VisitOutArgumentStorage(OutArgumentStorage arg)
		{
			throw new NotImplementedException();
		}

		public Storage VisitRegisterStorage(RegisterStorage reg)
		{
            if (defining)
            {
                regDefs[reg] = value;
            }
            else
            {
                value = null;
                regDefs.TryGetValue(reg, out value);
            }
            return reg;
		}

		public Storage VisitSequenceStorage(SequenceStorage seq)
		{
			seq.Head.Accept(this);
			seq.Tail.Accept(this);
			if (defining)
			{
				regDefs[seq] = value;
			}
			else
			{
				value = regDefs[seq];
			}
            return seq;
		}

		public Storage VisitStackArgumentStorage(StackArgumentStorage stack)
		{
            if (defining)
                regDefs[stack] = value;
            else
            {
                value = null;
                regDefs.TryGetValue(stack, out value);
            }
            return stack;
		}

		public Storage VisitStackLocalStorage(StackLocalStorage local)
		{
            if (defining)
                regDefs[local] = value;
            else
            {
                value = null;
                regDefs.TryGetValue(local, out value);
            }
            return local;
		}

		public Storage VisitTemporaryStorage(TemporaryStorage temp)
		{
            if (defining)
                regDefs[temp] = value;
            else
            {
                value = null;
                regDefs.TryGetValue(temp, out value);
            }
            return temp;
		}

		#endregion
    }
}
