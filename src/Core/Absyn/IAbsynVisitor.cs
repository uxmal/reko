#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using System;

namespace Reko.Core.Absyn
{
	/// <summary>
	/// Interface for visiting abstract syntax nodes.
	/// </summary>
	public interface IAbsynVisitor
	{
		void VisitAssignment(AbsynAssignment ass);
        void VisitCompoundAssignment(AbsynCompoundAssignment compound);
        void VisitBreak(AbsynBreak brk);
        void VisitCase(AbsynCase absynCase);
		void VisitContinue(AbsynContinue cont);
        void VisitDeclaration(AbsynDeclaration decl);
        void VisitDefault(AbsynDefault decl);
		void VisitDoWhile(AbsynDoWhile loop);
        void VisitFor(AbsynFor forLoop);
        void VisitGoto(AbsynGoto gotoStm);
		void VisitIf(AbsynIf ifStm);
		void VisitLabel(AbsynLabel lbl);
        void VisitLineComment(AbsynLineComment comment);
        void VisitReturn(AbsynReturn ret);
		void VisitSideEffect(AbsynSideEffect side);
        void VisitSwitch(AbsynSwitch absynSwitch);
        void VisitWhile(AbsynWhile loop);
    }

    public interface IAbsynVisitor<T>
    {
        T VisitAssignment(AbsynAssignment ass);
        T VisitBreak(AbsynBreak brk);
        T VisitCase(AbsynCase absynCase);
        T VisitCompoundAssignment(AbsynCompoundAssignment compound);
        T VisitContinue(AbsynContinue cont);
        T VisitDeclaration(AbsynDeclaration decl);
        T VisitDefault(AbsynDefault decl);
        T VisitDoWhile(AbsynDoWhile loop);
        T VisitFor(AbsynFor forLoop);
        T VisitGoto(AbsynGoto gotoStm);
        T VisitIf(AbsynIf ifStm);
        T VisitLabel(AbsynLabel lbl);
        T VisitLineComment(AbsynLineComment comment);
        T VisitReturn(AbsynReturn ret);
        T VisitSideEffect(AbsynSideEffect side);
        T VisitSwitch(AbsynSwitch absynSwitch);
        T VisitWhile(AbsynWhile loop);
    }

    public interface IAbsynVisitor<T, C>
    {
        T VisitAssignment(AbsynAssignment ass, C context);
        T VisitBreak(AbsynBreak brk, C context);
        T VisitCase(AbsynCase absynCase, C context);
        T VisitCompoundAssignment(AbsynCompoundAssignment compound, C context);
        T VisitContinue(AbsynContinue cont, C context);
        T VisitDeclaration(AbsynDeclaration decl, C context);
        T VisitDefault(AbsynDefault decl, C context);
        T VisitDoWhile(AbsynDoWhile loop, C context);
        T VisitFor(AbsynFor forLoop, C context);
        T VisitGoto(AbsynGoto gotoStm, C context);
        T VisitIf(AbsynIf ifStm, C context);
        T VisitLabel(AbsynLabel lbl, C context);
        T VisitLineComment(AbsynLineComment comment, C context);
        T VisitReturn(AbsynReturn ret, C context);
        T VisitSideEffect(AbsynSideEffect side, C context);
        T VisitSwitch(AbsynSwitch absynSwitch, C context);
        T VisitWhile(AbsynWhile loop, C context);
    }
}
