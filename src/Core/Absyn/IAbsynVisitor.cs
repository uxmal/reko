#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
    /// <remarks>
    /// Implementors will be called on the different methods
    /// depending on the concrete type of a visited <see cref="AbsynStatement"/>.
    /// </remarks>
	public interface IAbsynVisitor
	{
        /// <summary>
        /// Called when an assignment statement is visited.
        /// </summary>
		void VisitAssignment(AbsynAssignment ass);

        /// <summary>
        /// Called when an compound statement is visited.
        /// </summary>
        void VisitCompoundAssignment(AbsynCompoundAssignment compound);

        /// <summary>
        /// Called when a break statement is visited.
        /// </summary>
        void VisitBreak(AbsynBreak brk);

        /// <summary>
        /// Called when a case statement is visited.
        /// </summary>
        void VisitCase(AbsynCase absynCase);

        /// <summary>
        /// Called when a continue statement is visited.
        /// </summary>
		void VisitContinue(AbsynContinue cont);

        /// <summary>
        /// Called when a declaration statement is visited.
        /// </summary>
        void VisitDeclaration(AbsynDeclaration decl);

        /// <summary>
        /// Called when a default statement is visited.
        /// </summary>
        void VisitDefault(AbsynDefault decl);

        /// <summary>
        /// Called when a do-while statement is visited.
        /// </summary>
		void VisitDoWhile(AbsynDoWhile loop);

        /// <summary>
        /// Called when a for statement is visited.
        /// </summary>
        void VisitFor(AbsynFor forLoop);

        /// <summary>
        /// Called when a goto statement is visited.
        /// </summary>
        void VisitGoto(AbsynGoto gotoStm);

        /// <summary>
        /// Called when an if statement is visited.
        /// </summary>
		void VisitIf(AbsynIf ifStm);

        /// <summary>
        /// Called when a goto label statement is visited.
        /// </summary>
		void VisitLabel(AbsynLabel lbl);

        /// <summary>
        /// Called when a line comment is visited.
        /// </summary>
        void VisitLineComment(AbsynLineComment comment);

        /// <summary>
        /// Called when a return statement is visited.
        /// </summary>
        void VisitReturn(AbsynReturn ret);

        /// <summary>
        /// Called when a side effect statement is visited.
        /// </summary>
		void VisitSideEffect(AbsynSideEffect side);

        /// <summary>
        /// Called when a switch statement is visited.
        /// </summary>
        void VisitSwitch(AbsynSwitch absynSwitch);

        /// <summary>
        /// Called when a continue statement is visited.
        /// </summary>
        void VisitWhile(AbsynWhile loop);
    }

    /// <summary>
    /// Interface for visiting abstract syntax nodes.
    /// </summary>
    /// <remarks>
    /// Implementors will be called on the different methods
    /// depending on the concrete type of a visited <see cref="AbsynStatement"/>.
    /// All methods return a value of type <typeparamref name="T"/>, or a subclass
    /// thereof.
    /// </remarks>
    public interface IAbsynVisitor<T>
    {
        /// <summary>
        /// Called when an assignment statement is visited.
        /// </summary>
		T VisitAssignment(AbsynAssignment ass);

        /// <summary>
        /// Called when an compound statement is visited.
        /// </summary>
        T VisitCompoundAssignment(AbsynCompoundAssignment compound);

        /// <summary>
        /// Called when a break statement is visited.
        /// </summary>
        T VisitBreak(AbsynBreak brk);

        /// <summary>
        /// Called when a case statement is visited.
        /// </summary>
        T VisitCase(AbsynCase absynCase);

        /// <summary>
        /// Called when a continue statement is visited.
        /// </summary>
		T VisitContinue(AbsynContinue cont);

        /// <summary>
        /// Called when a declaration statement is visited.
        /// </summary>
        T VisitDeclaration(AbsynDeclaration decl);

        /// <summary>
        /// Called when a default statement is visited.
        /// </summary>
        T VisitDefault(AbsynDefault decl);

        /// <summary>
        /// Called when a do-while statement is visited.
        /// </summary>
		T VisitDoWhile(AbsynDoWhile loop);

        /// <summary>
        /// Called when a for statement is visited.
        /// </summary>
        T VisitFor(AbsynFor forLoop);

        /// <summary>
        /// Called when a goto statement is visited.
        /// </summary>
        T VisitGoto(AbsynGoto gotoStm);

        /// <summary>
        /// Called when an if statement is visited.
        /// </summary>
		T VisitIf(AbsynIf ifStm);

        /// <summary>
        /// Called when a goto label statement is visited.
        /// </summary>
		T VisitLabel(AbsynLabel lbl);

        /// <summary>
        /// Called when a line comment is visited.
        /// </summary>
        T VisitLineComment(AbsynLineComment comment);

        /// <summary>
        /// Called when a return statement is visited.
        /// </summary>
        T VisitReturn(AbsynReturn ret);

        /// <summary>
        /// Called when a side effect statement is visited.
        /// </summary>
		T VisitSideEffect(AbsynSideEffect side);

        /// <summary>
        /// Called when a switch statement is visited.
        /// </summary>
        T VisitSwitch(AbsynSwitch absynSwitch);

        /// <summary>
        /// Called when a continue statement is visited.
        /// </summary>
        T VisitWhile(AbsynWhile loop);
    }

    /// <summary>
    /// Interface for visiting abstract syntax nodes.
    /// </summary>
    /// <remarks>
    /// Implementors will be called on the different methods
    /// depending on the concrete type of a visited <see cref="AbsynStatement"/>.
    /// All methods return a value of type <typeparamref name="T"/>, or a subclass
    /// thereof. The context parameter us used to pass in contextual information
    /// (e.g. the parent node in the abstract syntax tree) to the visitor.
    /// </remarks>
    public interface IAbsynVisitor<T, C>
    {
        /// <summary>
        /// Called when an assignment statement is visited.
        /// </summary>
		T VisitAssignment(AbsynAssignment ass, C context);

        /// <summary>
        /// Called when an compound statement is visited.
        /// </summary>
        T VisitCompoundAssignment(AbsynCompoundAssignment compound, C context);

        /// <summary>
        /// Called when a break statement is visited.
        /// </summary>
        T VisitBreak(AbsynBreak brk, C context);

        /// <summary>
        /// Called when a case statement is visited.
        /// </summary>
        T VisitCase(AbsynCase absynCase, C context);

        /// <summary>
        /// Called when a continue statement is visited.
        /// </summary>
		T VisitContinue(AbsynContinue cont, C context);

        /// <summary>
        /// Called when a declaration statement is visited.
        /// </summary>
        T VisitDeclaration(AbsynDeclaration decl, C context);

        /// <summary>
        /// Called when a default statement is visited.
        /// </summary>
        T VisitDefault(AbsynDefault decl, C context);

        /// <summary>
        /// Called when a do-while statement is visited.
        /// </summary>
		T VisitDoWhile(AbsynDoWhile loop, C context);

        /// <summary>
        /// Called when a for statement is visited.
        /// </summary>
        T VisitFor(AbsynFor forLoop, C context);

        /// <summary>
        /// Called when a goto statement is visited.
        /// </summary>
        T VisitGoto(AbsynGoto gotoStm, C context);

        /// <summary>
        /// Called when an if statement is visited.
        /// </summary>
		T VisitIf(AbsynIf ifStm, C context);

        /// <summary>
        /// Called when a goto label statement is visited.
        /// </summary>
		T VisitLabel(AbsynLabel lbl, C context);

        /// <summary>
        /// Called when a line comment is visited.
        /// </summary>
        T VisitLineComment(AbsynLineComment comment, C context);

        /// <summary>
        /// Called when a return statement is visited.
        /// </summary>
        T VisitReturn(AbsynReturn ret, C context);

        /// <summary>
        /// Called when a side effect statement is visited.
        /// </summary>
		T VisitSideEffect(AbsynSideEffect side, C context);

        /// <summary>
        /// Called when a switch statement is visited.
        /// </summary>
        T VisitSwitch(AbsynSwitch absynSwitch, C context);

        /// <summary>
        /// Called when a continue statement is visited.
        /// </summary>
        T VisitWhile(AbsynWhile loop, C context);
    }
}
