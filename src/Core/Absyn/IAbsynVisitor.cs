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
	public interface IAbsynVisitor
	{
        /// <summary>
        /// Called when visiting an <see cref="AbsynAssignment"/>.
        /// </summary>
        /// <param name="ass">The visited assignment.</param>
		void VisitAssignment(AbsynAssignment ass);

        /// <summary>
        /// Called when visiting an <see cref="AbsynBreak"/>.
        /// </summary>
        /// <param name="brk">The visited break statement.</param>
        void VisitBreak(AbsynBreak brk);

        /// <summary>
        /// Called when visiting an <see cref="AbsynCase"/> statement.
        /// </summary>
        /// <param name="absynCase">The visited case statement.</param>
        /// 
        void VisitCase(AbsynCase absynCase);
        /// <summary>
        /// Called when visiting an <see cref="AbsynCompoundAssignment"/>;
        /// </summary>
        /// <param name="compound">The visited compound assignment.</param>
        void VisitCompoundAssignment(AbsynCompoundAssignment compound);

        /// <summary>
        /// Called when visiting an <see cref="AbsynContinue"/> statement.
        /// </summary>
        /// <param name="cont">The visited continue statement.</param>
		void VisitContinue(AbsynContinue cont);

        /// <summary>
        /// Called when visiting an <see cref="AbsynDeclaration"/> statement.
        /// </summary>
        /// <param name="decl">The visited declaration statement.</param>
        void VisitDeclaration(AbsynDeclaration decl);

        /// <summary>
        /// Called when visiting an <see cref="AbsynDefault"/> statement.
        /// </summary>
        /// <param name="decl">The visited default statement.</param>
        void VisitDefault(AbsynDefault decl);

        /// <summary>
        /// Called when visiting an <see cref="AbsynDoWhile"/> statement.
        /// </summary>
        /// <param name="loop">The visited do-while loop.</param>
		void VisitDoWhile(AbsynDoWhile loop);

        /// <summary>
        /// Called when visiting an <see cref="AbsynFor"/> statement.
        /// </summary>
        /// <param name="forLoop">The visited for loop.</param>
        void VisitFor(AbsynFor forLoop);

        /// <summary>
        /// Called when visiting an <see cref="AbsynGoto"/> statement.
        /// </summary>
        /// <param name="gotoStm">The visited goto statement.</param>
        void VisitGoto(AbsynGoto gotoStm);

        /// <summary>
        /// Called when visiting an <see cref="AbsynIf"/> statement.
        /// </summary>
        /// <param name="ifStm">The visited if statement.</param>
		void VisitIf(AbsynIf ifStm);

        /// <summary>
        /// Called when visiting an <see cref="AbsynLabel"/> statement.
        /// </summary>
        /// <param name="lbl">The visited label.</param>
		void VisitLabel(AbsynLabel lbl);

        /// <summary>
        /// Called when visiting an <see cref="AbsynLineComment"/> statement.
        /// </summary>
        /// <param name="comment">The visited comment.</param>
        void VisitLineComment(AbsynLineComment comment);

        /// <summary>
        /// Called when visiting an <see cref="AbsynReturn"/> statement.
        /// </summary>
        /// <param name="ret">The visited return statement.</param>
        void VisitReturn(AbsynReturn ret);

        /// <summary>
        /// Called when visiting an <see cref="AbsynSideEffect"/> statement.
        /// </summary>
        /// <param name="side">The visited side effect statement.</param>
		void VisitSideEffect(AbsynSideEffect side);

        /// <summary>
        /// Called when visiting an <see cref="AbsynSwitch"/> statement.
        /// </summary>
        /// <param name="absynSwitch">The visited switch statement.</param>
        void VisitSwitch(AbsynSwitch absynSwitch);

        /// <summary>
        /// Called when visiting an <see cref="AbsynWhile"/> statement.
        /// </summary>
        /// <param name="loop">The visited while loop.</param>
        void VisitWhile(AbsynWhile loop);
    }

    /// <summary>
    /// Interface for visiting abstract syntax nodes. All visitor methods
    /// can return an <typeparamref name="T"/> instance.
    /// </summary>
    public interface IAbsynVisitor<T>
    {
        /// <summary>
        /// Called when visiting an <see cref="AbsynAssignment"/>.
        /// </summary>
        /// <param name="ass">The visited assignment.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitAssignment(AbsynAssignment ass);

        /// <summary>
        /// Called when visiting an <see cref="AbsynBreak"/>.
        /// </summary>
        /// <param name="brk">The visited break statement.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitBreak(AbsynBreak brk);

        /// <summary>
        /// Called when visiting an <see cref="AbsynCase"/> statement.
        /// </summary>
        /// <param name="absynCase">The visited case statement.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitCase(AbsynCase absynCase);
        /// <summary>
        /// Called when visiting an <see cref="AbsynCompoundAssignment"/>;
        /// </summary>
        /// <param name="compound">The visited compound assignment.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitCompoundAssignment(AbsynCompoundAssignment compound);

        /// <summary>
        /// Called when visiting an <see cref="AbsynContinue"/> statement.
        /// </summary>
        /// <param name="cont">The visited continue statement.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitContinue(AbsynContinue cont);

        /// <summary>
        /// Called when visiting an <see cref="AbsynDeclaration"/> statement.
        /// </summary>
        /// <param name="decl">The visited declaration statement.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitDeclaration(AbsynDeclaration decl);

        /// <summary>
        /// Called when visiting an <see cref="AbsynDefault"/> statement.
        /// </summary>
        /// <param name="decl">The visited default statement.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitDefault(AbsynDefault decl);

        /// <summary>
        /// Called when visiting an <see cref="AbsynDoWhile"/> statement.
        /// </summary>
        /// <param name="loop">The visited do-while loop.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitDoWhile(AbsynDoWhile loop);

        /// <summary>
        /// Called when visiting an <see cref="AbsynFor"/> statement.
        /// </summary>
        /// <param name="forLoop">The visited for loop.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitFor(AbsynFor forLoop);

        /// <summary>
        /// Called when visiting an <see cref="AbsynGoto"/> statement.
        /// </summary>
        /// <param name="gotoStm">The visited goto statement.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitGoto(AbsynGoto gotoStm);

        /// <summary>
        /// Called when visiting an <see cref="AbsynIf"/> statement.
        /// </summary>
        /// <param name="ifStm">The visited if statement.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitIf(AbsynIf ifStm);

        /// <summary>
        /// Called when visiting an <see cref="AbsynLabel"/> statement.
        /// </summary>
        /// <param name="lbl">The visited label.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitLabel(AbsynLabel lbl);

        /// <summary>
        /// Called when visiting an <see cref="AbsynLineComment"/> statement.
        /// </summary>
        /// <param name="comment">The visited comment.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitLineComment(AbsynLineComment comment);

        /// <summary>
        /// Called when visiting an <see cref="AbsynReturn"/> statement.
        /// </summary>
        /// <param name="ret">The visited return statement.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitReturn(AbsynReturn ret);

        /// <summary>
        /// Called when visiting an <see cref="AbsynSideEffect"/> statement.
        /// </summary>
        /// <param name="side">The visited side effect statement.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitSideEffect(AbsynSideEffect side);

        /// <summary>
        /// Called when visiting an <see cref="AbsynSwitch"/> statement.
        /// </summary>
        /// <param name="absynSwitch">The visited switch statement.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitSwitch(AbsynSwitch absynSwitch);

        /// <summary>
        /// Called when visiting an <see cref="AbsynWhile"/> statement.
        /// </summary>
        /// <param name="loop">The visited while loop.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitWhile(AbsynWhile loop);
    }

    /// <summary>
    /// Interface for visiting abstract syntax nodes. All visitor methods
    /// are passed a context object of type <typeparamref name="C"/>.
    /// All visitor methods can return an <typeparamref name="T"/> instance.
    /// </summary>
    public interface IAbsynVisitor<T, C>
    {
        /// <summary>
        /// Called when visiting an <see cref="AbsynAssignment"/>.
        /// </summary>
        /// <param name="ass">The visited assignment.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitAssignment(AbsynAssignment ass, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynBreak"/>.
        /// </summary>
        /// <param name="brk">The visited break statement.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitBreak(AbsynBreak brk, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynCase"/> statement.
        /// </summary>
        /// <param name="absynCase">The visited case statement.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitCase(AbsynCase absynCase, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynCompoundAssignment"/>;
        /// </summary>
        /// <param name="compound">The visited compound assignment.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitCompoundAssignment(AbsynCompoundAssignment compound, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynContinue"/> statement.
        /// </summary>
        /// <param name="cont">The visited continue statement.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitContinue(AbsynContinue cont, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynDeclaration"/> statement.
        /// </summary>
        /// <param name="decl">The visited declaration statement.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitDeclaration(AbsynDeclaration decl, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynDefault"/> statement.
        /// </summary>
        /// <param name="decl">The visited default statement.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitDefault(AbsynDefault decl, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynDoWhile"/> statement.
        /// </summary>
        /// <param name="loop">The visited do-while loop.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitDoWhile(AbsynDoWhile loop, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynFor"/> statement.
        /// </summary>
        /// <param name="forLoop">The visited for loop.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitFor(AbsynFor forLoop, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynGoto"/> statement.
        /// </summary>
        /// <param name="gotoStm">The visited goto statement.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitGoto(AbsynGoto gotoStm, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynIf"/> statement.
        /// </summary>
        /// <param name="ifStm">The visited if statement.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitIf(AbsynIf ifStm, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynLabel"/> statement.
        /// </summary>
        /// <param name="lbl">The visited label.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitLabel(AbsynLabel lbl, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynLineComment"/> statement.
        /// </summary>
        /// <param name="comment">The visited comment.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitLineComment(AbsynLineComment comment, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynReturn"/> statement.
        /// </summary>
        /// <param name="ret">The visited return statement.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitReturn(AbsynReturn ret, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynSideEffect"/> statement.
        /// </summary>
        /// <param name="side">The visited side effect statement.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitSideEffect(AbsynSideEffect side, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynSwitch"/> statement.
        /// </summary>
        /// <param name="absynSwitch">The visited switch statement.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitSwitch(AbsynSwitch absynSwitch, C context);

        /// <summary>
        /// Called when visiting an <see cref="AbsynWhile"/> statement.
        /// </summary>
        /// <param name="loop">The visited while loop.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitWhile(AbsynWhile loop, C context);
    }
}
