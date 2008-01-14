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
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Decompiler.Core
{
	/// <summary>
	/// Contains the non-global locations accessed by the code inside a procedure. 
	/// </summary>
	/// <remarks>
	/// Variables accessed by the procedure can live in many places: registers, stack, and temporaries.
	/// <para>
	/// The frame layout is particularly interesting. On the Intel x86 architecture in real mode we have the following:
	/// </para>
	/// <code>
	///   Layout             offset value
	/// +-----------------+
	/// | arg3            |   4
	/// +-----------------+
	/// | arg2            |   2
	/// +-----------------+
	/// | arg1            |   0
	/// +-----------------+
	/// | return address  |	  -2
	/// +-----------------+
	/// | frame pointer   |   -4
	/// +-----------------+
	/// | local1          |   -6
	/// +-----------------+
	/// | local2          |
	/// 
	/// etc.
	/// </code>
	/// <para>Note that variables that are stack arguments use offsets based on the state of stack
	/// _after_ the return address was pushed.</para>
	/// <para>In addition, support has to be provided for architectures that have separate FPU stacks.</para>
	/// </remarks>
	public class Frame
	{
		private IdentifierCollection identifiers;	// Identifiers for each access.
		private int returnAddressSize;			// Size of return value on stack -- (some processors pass it in a register)
		private bool escapes;
		private Identifier framePointer;
		private int frameOffset;				// frame offset from stack pointer in bytes.

		//$REFACTOR: perhaps we can get rid of framePointerSize? It's only used by the constructor.
		public Frame(PrimitiveType framePointerSize)
		{
			identifiers = new IdentifierCollection();

			// There is always a "variable" for the global memory and the frame
			// pointer.

			Identifier g = new MemoryIdentifier(0, PrimitiveType.Void);
			identifiers.Add(g);
			framePointer = CreateTemporary("fp", framePointerSize);
		}

		public Identifier CreateSequence(Identifier head, Identifier tail, DataType dt)
		{
			Identifier id = new Identifier(string.Format("{0}_{1}", head.Name, tail.Name), identifiers.Count, dt, new
				SequenceStorage(head, tail));
			identifiers.Add(id);
			return id;
		}

		/// <summary>
		/// Creates a temporary variable whose storage and name is guaranteed not to collide with any other variable.
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public Identifier CreateTemporary(DataType dt)
		{
			Identifier id = new Identifier("v" + identifiers.Count, identifiers.Count, dt, new TemporaryStorage());
			identifiers.Add(id);
			return id;
		}

		public Identifier CreateTemporary(string name, DataType dt)
		{
			Identifier id = new Identifier(name, identifiers.Count, dt, new TemporaryStorage());
			identifiers.Add(id);
			return id;
		}

		public Identifier EnsureFlagGroup(uint grfMask, string name, DataType dt)
		{
			if (grfMask == 0)
				return null;
			Identifier id = FindFlagGroup(grfMask);
			if (id == null)
			{
				id = new Identifier(name, identifiers.Count, dt, new FlagGroupStorage(grfMask, name));
				identifiers.Add(id);
			}
			return id;
		}


		public Identifier EnsureFpuStackVariable(int depth, DataType type)
		{
			Identifier id = FindFpuStackVariable(depth);
			if (id == null)
			{
				string name = string.Format("{0}{1}", (depth < 0 ? "rLoc" : "rArg"), Math.Abs(depth));
				id = new Identifier(name, identifiers.Count, type, new FpuStackStorage(depth, type));
				identifiers.Add(id);
			}
			return id;
		}

		/// <summary>
		/// Ensures a register access in this function. 
		/// </summary>
		/// <param name="reg"></param>
		/// <param name="name"></param>
		/// <param name="vt"></param>
		/// <returns></returns>
		public Identifier EnsureRegister(MachineRegister reg)
		{
			Identifier id = FindRegister(reg);
			if (id == null)
			{
				id = new Identifier(reg.Name, identifiers.Count, reg.DataType, new RegisterStorage(reg));
				identifiers.Add(id);
			}
			return id;
		}

		public Identifier EnsureOutArgument(Identifier idOrig)
		{
			Identifier idOut = FindOutArgument(idOrig);
			if (idOut == null)
			{
				idOut = new Identifier(idOrig.Name + "Out", identifiers.Count, PrimitiveType.Pointer, new OutArgumentStorage(idOrig));
				identifiers.Add(idOut);
			}
			return idOut;
		}

		public Identifier EnsureSequence(Identifier head, Identifier tail, DataType dt)
		{
			Identifier idSeq = FindSequence(head, tail);
			if (idSeq == null)
			{
				idSeq = CreateSequence(head, tail, dt);
			}
			return idSeq;
		}

		/// <summary>
		/// Makes sure that there is a local variable at the given offset.
		/// </summary>
		/// <param name="cbOffset"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public Identifier EnsureStackLocal(int cbOffset, DataType type)
		{
			return EnsureStackLocal(cbOffset, type, null);
		}

		public Identifier EnsureStackLocal(int cbOffset, DataType type, string name)
		{
			Identifier id = FindStackLocal(cbOffset, type.Size);
			if (id == null)
			{
				id = new Identifier(FormatStackAccessName(type, "Loc", cbOffset, name), identifiers.Count, type, new StackLocalStorage(cbOffset, type));
				identifiers.Add(id);
			}
			return id;
		}

		public Identifier EnsureStackArgument(int cbOffset, DataType type)
		{
			return EnsureStackArgument(cbOffset, type, null);
		}

		public Identifier EnsureStackArgument(int cbOffset, DataType type, string argName)
		{
			Identifier id = FindStackArgument(cbOffset, type.Size);
			if (id == null)
			{
				id = new Identifier(
					FormatStackAccessName(type, "Arg", cbOffset, argName), 
					identifiers.Count,
					type, 
					new StackArgumentStorage(cbOffset, type));
				identifiers.Add(id);
			}			
			return id;
		}


		public Identifier EnsureStackVariable(Value imm, int cbOffset, DataType type)
		{
			if (imm.IsValid)
			{
				cbOffset = imm.Signed - cbOffset;
			}
			else
			{
				cbOffset = -cbOffset;
			}

			return (cbOffset >= 0) 
				? EnsureStackArgument(cbOffset, type)
				: EnsureStackLocal(cbOffset, type);

		}

		public bool Escapes
		{
			get { return escapes; }
			set { escapes = value; }
		}

		/// <summary>
		/// The offset of a variable from the return address, as seen from a caller.
		/// </summary>
		/// <param name="var"></param>
		/// <returns></returns>
		public int ExternalOffset(Identifier id)
		{
			if (id == null)
				return 0;
			StackArgumentStorage stVar = id.Storage as StackArgumentStorage;
			if (stVar != null)
				return stVar.StackOffset;
			FpuStackStorage fstVar = id.Storage as FpuStackStorage;
			if (fstVar != null)
				return fstVar.FpuStackOffset;

			throw new ArgumentOutOfRangeException("var", "Variable must be an argument.");
		}

		public Identifier FindSequence(Identifier n1, Identifier n2)
		{
			foreach (Identifier id in identifiers)
			{
				SequenceStorage seq = id.Storage as SequenceStorage;
				if (seq != null && seq.Head == n1 && seq.Tail == n2)
					return id;
			}
			return null;
		}
		

		public string FormatStackAccessName(DataType type, string prefix, int cbOffset)
		{
			cbOffset = Math.Abs(cbOffset);
			string fmt = (cbOffset > 0xFF) ? "{0}{1}{2:X4}" : "{0}{1}{2:X2}";
			return string.Format(fmt, type.Prefix, prefix, cbOffset);
		}

		public string FormatStackAccessName(DataType type, string prefix, int cbOffset, string nameOverride)
		{
			if (nameOverride != null)
				return nameOverride;
			else return FormatStackAccessName(type, prefix, cbOffset);
		}

		public int FrameOffset
		{
			get { return frameOffset; }
			set { frameOffset = value; }
		}

		public Identifier FramePointer
		{
			get { return framePointer; }
		}

		/// <summary>
		/// Returns the number of bytes the stack arguments consume on the stack.
		/// </summary>
		/// <returns></returns>
		public int GetStackArgumentSpace()
		{
			int cbMax = 0;
			foreach (Identifier id in identifiers)
			{
				StackArgumentStorage sa = id.Storage as StackArgumentStorage;
				if (sa == null)
					continue;
				cbMax = Math.Max(cbMax, sa.StackOffset + sa.DataType.Size);
			}
			return cbMax;
		}

		public Identifier FindFlagGroup(uint grfMask)
		{
			foreach (Identifier id in identifiers)
			{
				FlagGroupStorage flags = id.Storage as FlagGroupStorage;
				if (flags != null && flags.FlagGroup == grfMask)
				{
					return id;
				}
			}
			return null;
		}

		public Identifier FindFpuStackVariable(int off)
		{
			foreach (Identifier id in identifiers)
			{
				FpuStackStorage fst = id.Storage as FpuStackStorage;
				if (fst != null && fst.FpuStackOffset == off)
					return id;
			}
			return null;
		}

		public Identifier FindOutArgument(Identifier idOrig)
		{
			foreach (Identifier id in identifiers)
			{
				OutArgumentStorage s = id.Storage as OutArgumentStorage;
				if (s != null && s.OriginalIdentifier == idOrig)
				{
					return id;
				}
			}
			return null;
		}


		public Identifier FindRegister(MachineRegister reg)
		{
			foreach (Identifier id in identifiers)
			{
				RegisterStorage s = id.Storage as RegisterStorage;
				if (s != null && s.Register == reg)
					return id;
			}
			return null;
		}


		public Identifier FindStackArgument(int offset, int size)
		{
			foreach (Identifier id in identifiers)
			{
				StackArgumentStorage s = id.Storage as StackArgumentStorage;
				if (s != null && s.StackOffset == offset && id.DataType.Size == size)
				{
					return id;
				}
			}
			return null;
		}

		public Identifier FindStackLocal(int offset, int size)
		{
			foreach (Identifier id in identifiers)
			{
				StackLocalStorage loc = id.Storage as StackLocalStorage;
				if (loc != null && loc.StackOffset == offset && id.DataType.Size == size)
					return id;
			}
			return null;
		}

		/// <summary>
		/// Amount of bytes that the calling function pushed on the stack to store 
		/// the return address. Some architectures pass the return address in a register,
		/// which implies that in those architectures the return address size should be zero.
		/// </summary>
		public int ReturnAddressSize
		{
			get { return returnAddressSize; }
			set { returnAddressSize = value; }
		}

		public void SetFramePointerWidth(DataType width)
		{
			if (framePointer != null)
			{
				if (framePointer.DataType.Size != width.Size)
					throw new InvalidOperationException("Frame pointer must have only one width");
			}
			else
			{
			}
		}

		public IdentifierCollection Identifiers
		{
			get { return identifiers; }
		}

		public void Write(TextWriter text)
		{
			foreach (Identifier id in identifiers)
			{
				text.Write("// ");
				text.Write(id.Name);
				text.Write(":");
				id.Storage.Write(text);
				text.WriteLine();
			}
			if (Escapes)
				text.WriteLine("// Frame escapes");
			text.WriteLine("// return address size: {0}", ReturnAddressSize);
		}
	}
}