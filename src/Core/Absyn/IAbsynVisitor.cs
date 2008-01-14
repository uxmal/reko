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

using System;

namespace Decompiler.Core.Absyn
{
	/// <summary>
	/// Interface for visiting abstract syntax nodes.
	/// </summary>
	public interface IAbsynVisitor
	{
		void VisitAssignment(AbsynAssignment expr);
		void VisitBreak(AbsynBreak brk);
		void VisitCompoundStatement(AbsynCompoundStatement compound);
		void VisitContinue(AbsynContinue cont);
		void VisitDeclaration(AbsynDeclaration decl);
		void VisitDoWhile(AbsynDoWhile loop);
		void VisitGoto(AbsynGoto loop);
		void VisitIf(AbsynIf ifStm);
		void VisitLabel(AbsynLabel lbl);
		void VisitReturn(AbsynReturn ret);
		void VisitSideEffect(AbsynSideEffect side);
		void VisitWhile(AbsynWhile loop);
	}
}
