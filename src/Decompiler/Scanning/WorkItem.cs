/* 
 * Copyright (C) 1999-2009 John Källén.
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
using Decompiler.Core.Types;
using System;
using System.IO;

namespace Decompiler.Scanning
{
	public enum BlockType
	{
		Segment,
		JumpTarget,
		Procedure,
		Vector
	}

	public class WorkItem
	{
		private WorkItem wiPrev;
		private Address addr;
		public ProcessorState state;
		public BlockType type;

		public WorkItem(WorkItem wiPrev, BlockType type, Address addr)
		{
			this.wiPrev = wiPrev;
			this.type = type;
			this.addr = addr;
		}

		public Address Address
		{
			get { return addr; }
		}

		public WorkItem Previous
		{
			get { return wiPrev; }
		}

		public override string ToString()
		{
			StringWriter sw = new StringWriter();
			Write(sw);
			return sw.ToString();
		}

		public void Write(TextWriter writer)
		{
			writer.WriteLine("WorkItem @ {0}", this.Address);
			if (wiPrev != null)
				wiPrev.Write(writer);
		}
	}


	public class VectorWorkItem : WorkItem
	{
		public Address addrFrom;			// address from which the jump is called.
		public ImageReader reader;
		public PrimitiveType stride;
		public ushort segBase;
		public ImageMapVectorTable table;
		public Procedure proc;

		public VectorWorkItem(WorkItem wiPrev, Procedure proc, Address addrTable) : base(wiPrev, BlockType.Vector, addrTable)
		{
			this.proc = proc;
		}
	}
}
