/* 
 * Copyright (C) 1999-2010 John Källén.
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

namespace Decompiler.Core
{
	public interface ICodeWalkerListener
	{
		void OnBranch(ProcessorState st, Address addrInstr, Address addrTerm, Address addrBranch);
		void OnGlobalVariable(Address addr, PrimitiveType width, Constant c);
		void OnIllegalOpcode(Address addrIllegal);
		void OnJump(ProcessorState st, Address addrInstr, Address addrTerm, Address addrJump);
		void OnJumpPointer(ProcessorState st, Address segBase, Address addrPtr, PrimitiveType stride);
		void OnJumpTable(ProcessorState st, 
			Address addrInstr,
			Address addrTable, 
			ushort segBase,
			PrimitiveType stride);
		void OnProcedure(ProcessorState st, Address addr);
		void OnProcedurePointer(ProcessorState st, Address addrBase, Address addrPtr, PrimitiveType stride);
		void OnProcedureTable(
			ProcessorState st,
			Address addrInstr,
			Address addrTable, 
			ushort segBase,
			PrimitiveType stride);
		void OnProcessExit(Address addrTerm);
		void OnReturn(Address addrTerm);
		void OnSystemServiceCall(Address addrInstr, SystemService svc);
		void OnTrampoline(ProcessorState st, Address addrInstr, Address addrGlob);
        void Warn(Address addr, string format, params object[] args);
	}
}
