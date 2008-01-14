/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using System;
using System.Collections;

namespace Decompiler.Analysis
{
	public class TrashStorageHelper : StorageVisitor
	{
		private uint grfDefs;
		private Hashtable regDefs;
		private bool defining;
		private object value;

		public TrashStorageHelper()
		{
			this.regDefs = new Hashtable();
		}


		public TrashStorageHelper(Hashtable defs, uint grfDefs)
		{
			this.regDefs = defs;
			this.grfDefs = grfDefs;
		}

		public TrashStorageHelper Clone()
		{
			return new TrashStorageHelper((Hashtable) regDefs.Clone(), grfDefs);
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

		public void Trash(Identifier dst, object v)
		{
			defining = true;
			value = v;
			dst.Storage.Accept(this);
		}

		public Hashtable TrashedRegisters
		{
			get { return regDefs; }
		}

		public uint TrashedFlags
		{
			get { return grfDefs; }
			set { grfDefs = value; }
		}

		#region StorageVisitor Members

		public void VisitFlagGroupStorage(FlagGroupStorage grf)
		{
			if (defining)
				this.grfDefs |= grf.FlagGroup;
		}

		public void VisitFpuStackStorage(FpuStackStorage fpu)
		{
			if (defining)
				regDefs[fpu] = value;
			else
				value = regDefs[fpu];
		}

		public void VisitMemoryStorage(MemoryStorage global)
		{
			throw new NotImplementedException();
		}

		public void VisitOutArgumentStorage(OutArgumentStorage arg)
		{
			throw new NotImplementedException();
		}

		public void VisitRegisterStorage(RegisterStorage reg)
		{
			if (defining)
			{
				regDefs[reg] = value;
			}
			else
				value = regDefs[reg];
		}

		public void VisitSequenceStorage(SequenceStorage seq)
		{
			seq.Head.Storage.Accept(this);
			seq.Tail.Storage.Accept(this);
			if (defining)
			{
				regDefs[seq] = value;
			}
			else
			{
				value = regDefs[seq];
			}
		}

		public void VisitStackArgumentStorage(StackArgumentStorage stack)
		{
			if (defining)
				regDefs[stack] = value;
			else
				value = regDefs[stack];
		}

		public void VisitStackLocalStorage(StackLocalStorage local)
		{
			if (defining)
				regDefs[local] = value;
			else
				value = regDefs[local];
		}

		public void VisitTemporaryStorage(TemporaryStorage temp)
		{
			if (defining)
				regDefs[temp] = value;
			else
				value = regDefs[temp];
		}

		#endregion
	}
}
