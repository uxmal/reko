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

using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Core.Serialization;
using System;
using System.IO;

namespace Decompiler.Core
{
	/// <summary>
	/// Encapsulates architecture-dependent storage mechanism for an identifier.
	/// </summary>
	public abstract class Storage
	{
		private string storageKind;

		public Storage(string storageKind)
		{
			this.storageKind = storageKind;
		}

		public abstract void Accept(StorageVisitor visitor);

		public virtual Identifier BindFormalArgumentToFrame(Frame callingframe, CallSite cs)
		{
			throw new NotSupportedException(string.Format("A {0} can't be used as a formal parameter.", GetType().FullName));
		}

		public string Kind
		{
			get { return storageKind; }
		}

		public abstract int OffsetOf(Storage storage);

		public virtual SerializedKind Serialize()
		{
			throw new NotImplementedException(this.GetType().Name + ".Serialize not implemented.");
		}

		public override string ToString()
		{
			StringWriter w = new StringWriter();
			Write(w);
			return w.ToString();
		}

		public abstract void Write(TextWriter writer);
	}


	public class FlagGroupStorage : Storage
	{
		private string name;
		private uint grfMask;

		public FlagGroupStorage(uint grfMask, string name) : base("FlagGroup")
		{
			this.grfMask = grfMask;
			this.name = name;
		}

		public override void Accept(StorageVisitor visitor)
		{
			visitor.VisitFlagGroupStorage(this);
		}

		public override Identifier BindFormalArgumentToFrame(Frame frame, CallSite cs)
		{
			return frame.EnsureFlagGroup(grfMask, name, PrimitiveType.Byte);		//$REVIEW: PrimitiveType.Byte is hard-wired here.
		}

		public override bool Equals(object obj)
		{
			FlagGroupStorage fgs = obj as FlagGroupStorage;
			if (fgs == null)
				return false;
			return grfMask == fgs.grfMask;
		}

		public uint FlagGroup
		{
			get { return grfMask; }
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ grfMask.GetHashCode();
		}

		public override int OffsetOf(Storage stgSub)
		{
			FlagGroupStorage f = stgSub as FlagGroupStorage;
			if (f == null)
				return -1;
			return ((f.grfMask & grfMask) != 0) ? 0 : -1;
		}

		public override SerializedKind Serialize()
		{
			return new SerializedFlag(name);
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("Flags");
		}
	}

	public class FpuStackStorage : Storage
	{
		private int depth;
		private DataType dataType;

		public FpuStackStorage(int depth, DataType dataType) : base("FpuStack")
		{
			this.depth = depth;
			this.dataType = dataType;
		}

		public override void Accept(StorageVisitor visitor)
		{
			visitor.VisitFpuStackStorage(this);
		}

		public override Identifier BindFormalArgumentToFrame(Frame frame, CallSite cs)
		{
			return frame.EnsureFpuStackVariable(depth - cs.FpuStackDepthBefore, dataType);
		}

		public override bool Equals(object obj)
		{
			FpuStackStorage fss = obj as FpuStackStorage;
			if (fss == null)
				return false;
			return depth == fss.depth;
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ depth.GetHashCode();
		}

		public int FpuStackOffset
		{
			get { return depth; }
		}

		public override int OffsetOf(Storage stgSub)
		{
			return -1;
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("FPU stack");
		}
	}

	/// <summary>
	/// Storage is some unspecified part of global memory.
	/// </summary>
	public class MemoryStorage : Storage
	{
		public MemoryStorage() : base("Global")
		{
		}

		public override void Accept(StorageVisitor visitor)
		{
			visitor.VisitMemoryStorage(this);
		}

		public override int OffsetOf(Storage stgSub)
		{
			return -1;
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("Global memory");
		}
	}

	/// <summary>
	/// Storage for registers or other identifiers that are live-out of a procedure.
	/// </summary>
	public class OutArgumentStorage : Storage
	{
		private Identifier originalId; 

		public OutArgumentStorage(Identifier originalId) : base("out")
		{
			this.originalId = originalId;
		}

		public override void Accept(StorageVisitor visitor)
		{
			visitor.VisitOutArgumentStorage(this);
		}

		public override Identifier BindFormalArgumentToFrame(Frame frame, CallSite cs)
		{
			return originalId.Storage.BindFormalArgumentToFrame(frame, cs);
		}

		public override bool Equals(object obj)
		{
			OutArgumentStorage oas = obj as OutArgumentStorage;
			if (oas == null)
				return false;
			return oas.OriginalIdentifier.Equals(OriginalIdentifier);
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ originalId.GetHashCode();
		}

		public override int OffsetOf(Storage stgSub)
		{
			return -1;
		}

		public Identifier OriginalIdentifier
		{
			get { return originalId; }
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("Out:");
			OriginalIdentifier.Storage.Write(writer);
		}
	}

	public class RegisterStorage : Storage
	{
		private MachineRegister reg;

		public RegisterStorage(MachineRegister reg) : base("Register")
		{
			this.reg = reg;
		}

		public override void Accept(StorageVisitor visitor)
		{
			visitor.VisitRegisterStorage(this);
		}

		public override bool Equals(object obj)
		{
			RegisterStorage rs = obj as RegisterStorage;
			if (rs == null)
				return false;
			return reg.Number == rs.Register.Number;
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ reg.Number;
		}


		public override Identifier BindFormalArgumentToFrame(Frame frame, CallSite cs)
		{
			return frame.EnsureRegister(reg);
		}

		public override int OffsetOf(Storage stgSub)
		{
			RegisterStorage regSub = stgSub as RegisterStorage;
			if (regSub == null)
				return -1;
			if (regSub.Register == Register)
				return 0;
			return regSub.Register.IsSubRegisterOf(Register)
				? regSub.Register.AliasOffset
				: -1;
		}

		public MachineRegister Register
		{
			get { return reg; }
		}

		public override SerializedKind Serialize()
		{
			return new SerializedRegister(reg.Name);
		}


		public override void Write(TextWriter writer)
		{
			writer.Write("Register ");
			writer.Write(reg.Name);
		}
	}

	public class SequenceStorage : Storage
	{
		private Identifier head;
		private Identifier tail;

		//$REFACTOR: make this params Identifier [], to support arbitrarily long identifiers
		public SequenceStorage(Identifier head, Identifier tail) 
			: base("Sequence")		
		{
			this.head = head;
			this.tail = tail;
		}

		public override void Accept(StorageVisitor visitor)
		{
			visitor.VisitSequenceStorage(this);
		}

		public override Identifier BindFormalArgumentToFrame(Frame callingFrame, CallSite cs)
		{
			Identifier idHead = head.Storage.BindFormalArgumentToFrame(callingFrame, cs);
			Identifier idTail = tail.Storage.BindFormalArgumentToFrame(callingFrame, cs);
			return callingFrame.EnsureSequence(idHead, idTail, PrimitiveType.CreateWord(idHead.DataType.Size + idTail.DataType.Size));
		}

		public override bool Equals(object obj)
		{
			SequenceStorage ss = obj as SequenceStorage;
			if (ss == null)
				return false;
			return head.Equals(ss.head) && tail.Equals(ss.tail);
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ head.GetHashCode() ^ (3 * tail.GetHashCode());
		}

		public Identifier Head
		{
			get { return head; }
		}

		public Identifier Tail
		{
			get { return tail; }
		}

		public override int OffsetOf(Storage stgSub)
		{
			int off = tail.Storage.OffsetOf(stgSub);
			if (off != -1)
				return off;
			off = head.Storage.OffsetOf(stgSub);
			if (off != -1)
				return off + tail.DataType.BitSize;
			return -1;
		}

		public override SerializedKind Serialize()
		{
			return new SerializedSequence(this);
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("Sequence {0}:{1}", head.Name, tail.Name);
		}
	}

	public class StackArgumentStorage : Storage
	{
		private int cbOffset;		// offset stack pointer on entry to routine.
		private DataType dataType;

		public StackArgumentStorage(int cbOffset, DataType dataType) : base("Stack")
		{
			this.cbOffset = cbOffset;
			this.dataType = dataType;
		}

		public override void Accept(StorageVisitor visitor)
		{
			visitor.VisitStackArgumentStorage(this);
		}

		public override Identifier BindFormalArgumentToFrame(Frame callingFrame, CallSite cs)
		{
			return callingFrame.EnsureStackLocal(cbOffset - cs.StackDepthBefore, DataType);
		}

		public DataType DataType
		{
			get { return dataType; }
		}

		public override bool Equals(object obj)
		{
			StackArgumentStorage sas = obj as StackArgumentStorage;
			if (sas == null)
				return false;
			return cbOffset == sas.cbOffset;
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ cbOffset;
		}

		public override int OffsetOf(Storage stgSub)
		{
			StackArgumentStorage arg = stgSub as StackArgumentStorage;
			if (arg == null)
				return -1;
			if (arg.cbOffset >= cbOffset && arg.cbOffset + arg.DataType.Size <= cbOffset + DataType.Size)
				return (arg.cbOffset - cbOffset) * DataType.BitsPerByte;
			return -1;
		}

		/// <summary>
		/// Offset from stack pointer as it was when the procedure was entered.
		/// </summary>
		/// <remarks>
		/// If the architecture stores the return address on the stack, the return address will be at offset 0 and
		/// any stack arguments will have offsets > 0. If the architecture passes the return address in a
		/// register, there may be stack arguments with offset 0. In either case, negative stack offsets for parameters
		/// are not legal.
		/// </remarks>
		public int StackOffset
		{
			get { return cbOffset; }
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("{0} +{1:X4}", Kind, cbOffset);
		}
	}

	public class StackLocalStorage : Storage
	{
		private int cbOffset;
		private DataType dataType;

		public StackLocalStorage(int cbOffset, DataType dataType) : base("Local")
		{
			this.cbOffset = cbOffset;
			this.dataType = dataType;
		}

		public override void Accept(StorageVisitor visitor)
		{
			visitor.VisitStackLocalStorage(this);
		}

		public override bool Equals(object obj)
		{
			StackLocalStorage sas = obj as StackLocalStorage;
			if (sas == null)
				return false;
			return cbOffset == sas.cbOffset;
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ cbOffset;
		}


		public override int OffsetOf(Storage stgSub)
		{
			StackLocalStorage local = stgSub as StackLocalStorage;
			if (local == null)
				return -1;
			if (local.cbOffset >= cbOffset && local.cbOffset + local.DataType.Size <= cbOffset + DataType.Size)
				return (local.cbOffset - cbOffset) * DataType.BitsPerByte;
			return -1;
		}

		public DataType DataType
		{
			get { return dataType; }
		}

		public int StackOffset
		{
			get { return cbOffset; }
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("{0} -{1:X4}", base.Kind, Math.Abs(cbOffset));
		}
	}

	public class TemporaryStorage : Storage
	{
		public TemporaryStorage() : base("Temporary")
		{
		}

		public override void Accept(StorageVisitor visitor)
		{
			visitor.VisitTemporaryStorage(this);
		}

		public override int OffsetOf(Storage stgSub)
		{
			return -1;
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("Temporary");
		}
	}
}

