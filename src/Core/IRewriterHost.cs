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

using Decompiler.Core.Types;
using System;

namespace Decompiler.Core
{
	public interface IRewriterHost
	{
		void AddCallEdge(Procedure caller, Statement stm, Procedure callee);
		ImageReader CreateImageReader(Address addr);
		PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity);
		Procedure GetProcedureAtAddress(Address addr, int cbReturnAddress);
		ProcedureSignature GetCallSignatureAtAddress(Address addrCallInstruction);
		Procedure [] GetProceduresFromVector(Address addrCallingInstruction, int cbReturnAddress);
		PseudoProcedure GetImportThunkAtAddress(Address addrThunk);
		VectorUse VectorUseAt(Address addr);
		SystemService SystemCallAt(Address addr);
		PseudoProcedure TrampolineAt(Address addr);
		ProgramImage Image { get; }
		void AddDiagnostic(Diagnostic diagnostic);
    }
}
