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
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Typing
{
	/// <summary>
	/// Some useful inferences can be made when looking at expressions like
	/// reg + Const
	/// if [[reg]] = ptr(struct(...))
	/// </summary>
	public class DerivedPointerAnalysis : InstructionVisitorBase
	{
		private TypeFactory factory;
		private TypeStore store;
		private ITraitHandler handler;
		private Program prog;
		private Identifier globals;
		private Unifier unifier;
        private IProcessorArchitecture arch;

		public DerivedPointerAnalysis(TypeFactory factory, TypeStore store, ITraitHandler handler, IProcessorArchitecture arch)
		{
			this.factory = factory;
			this.store = store;
			this.handler = handler;
            this.unifier = new DataTypeBuilderUnifier(factory, store);
            this.arch = arch;
		}

		public Pointer CreatePointerToField(int offset, TypeVariable tvField)
		{
			return factory.CreatePointer(
				factory.CreateStructureType(null, 0, new StructureField(offset, tvField)),
				arch.PointerType.Size);
		}

		public void FollowConstantPointers(Program prog)
		{
			this.prog = prog;
			foreach (Procedure proc in prog.Procedures.Values)
			{
				foreach (Block b in proc.RpoBlocks)
				{
					foreach (Statement stm in b.Statements)
					{
						stm.Instruction.Accept(this);
					}
				}
			}
		}

		private TypeVariable GetTypeVariableForField(DataType fieldType)
		{
			StructureType s = fieldType as StructureType;
			if (s != null)
			{
				StructureField f = s.Fields.AtOffset(0);
				if (f == null)
					return null;
				return f.DataType as TypeVariable;
			}
			FunctionType fn = fieldType as FunctionType;
			if (fn != null)
			{
				throw new NotImplementedException();
			}
			throw new NotImplementedException(string.Format("Don't know how to handle pointers to {0}.", fieldType));
		}


		public Identifier Globals
		{
			get 
			{
				if (globals != null)
				{
					return globals;
				}
				else
					return prog.Globals;
			}
			set { globals = value; }
		}

		public override void VisitConstant(Constant c)
		{
			DataType dt = store.ResolvePossibleTypeVar(c.TypeVariable);
            int offset = StructureField.ToOffset(c);
			Pointer ptr = dt as Pointer;
			if (ptr != null)
			{
				// C is a constant pointer.
				if (offset == 0)
					return;

				StructureType str = ptr.Pointee as StructureType;
				if (str != null)
				{
					TypeVariable tvField = GetTypeVariableForField(ptr.Pointee);
					if (tvField == null)
						return;

					ptr = CreatePointerToField(offset, tvField);
					Globals.TypeVariable.OriginalDataType =
						unifier.Unify(Globals.TypeVariable.OriginalDataType, ptr);

					ptr = CreatePointerToField(offset, tvField);
					Globals.TypeVariable.Class.DataType =
						unifier.Unify(Globals.TypeVariable.Class.DataType, ptr);
				}
				return;
			}
			MemberPointer mptr = dt as MemberPointer;
			if (mptr != null)
			{
				VisitConstantMemberPointer(offset, mptr);
			}
		}

		private void VisitConstantMemberPointer(int offset, MemberPointer mptr)
		{
			TypeVariable tvField = GetTypeVariableForField(mptr.Pointee);
			if (tvField == null)
				return;

			TypeVariable tvBase = (TypeVariable) mptr.BasePointer;
			Pointer ptr = CreatePointerToField(offset, tvField);
			tvBase.OriginalDataType =
				unifier.Unify(tvBase.OriginalDataType, ptr);

			ptr = CreatePointerToField(offset, tvField);
			tvBase.Class.DataType =
				unifier.Unify(tvBase.Class.DataType, ptr);
		}


		public override void VisitMemoryAccess(MemoryAccess acc)
		{
		}

	}
}
