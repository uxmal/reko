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

using Reko.Core;
using Reko.Core.Absyn;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Structure;

/// <summary>
/// Emitter class for generating <see cref="AbsynStatement"/>s.
/// </summary>
public class AbsynStatementEmitter : InstructionVisitor, IAbsynVisitor
{
    private readonly List<AbsynStatement> stms;
    private readonly ExpressionEmitter m;

    /// <summary>
    /// Constructs an instance of the <see cref="AbsynStatementEmitter"/> class.
    /// </summary>
    /// <param name="stms">List of <see cref="AbsynStatement"/>s to populate.
    /// </param>
    public AbsynStatementEmitter(List<AbsynStatement> stms)
    {
        this.stms = stms;
        this.m = new ExpressionEmitter();
    }

    /// <summary>
    /// Emit an assignment statement.
    /// </summary>
    /// <param name="dst">LValue of the assignment.</param>
    /// <param name="src">RValue of the assignment.</param>
    /// <returns>The generated <see cref="AbsynAssignment"/>.
    /// </returns>
    public AbsynAssignment EmitAssign(Expression dst, Expression src)
    {
        var ass = new AbsynAssignment(dst, src);
        stms.Add(ass);
        return ass;
    }


    #region InstructionVisitor Members

    void InstructionVisitor.VisitAssignment(Assignment a)
    {
        stms.Add(new AbsynAssignment(a.Dst, a.Src));
    }

    void InstructionVisitor.VisitBranch(Branch b)
    {
        throw new NotImplementedException();
    }

    void InstructionVisitor.VisitCallInstruction(CallInstruction ci)
    {
        stms.Add(new AbsynSideEffect(m.Fn(ci.Callee, VoidType.Instance)));
    }

    void InstructionVisitor.VisitComment(CodeComment comment)
    {
        stms.Add(new AbsynLineComment(comment.Text));
    }

    void InstructionVisitor.VisitDefInstruction(DefInstruction def)
    {
        stms.Add(new AbsynLineComment(def.ToString()));
    }

    void InstructionVisitor.VisitGotoInstruction(GotoInstruction g)
    {
        throw new NotImplementedException();
    }

    void InstructionVisitor.VisitPhiAssignment(PhiAssignment phi)
    {
        stms.Add(new AbsynLineComment(phi.ToString()));
    }

    void InstructionVisitor.VisitReturnInstruction(ReturnInstruction ret)
    {
        stms.Add(new AbsynReturn(ret.Expression));
    }

    void InstructionVisitor.VisitSideEffect(SideEffect side)
    {
        stms.Add(new AbsynSideEffect(side.Expression));
    }

    void InstructionVisitor.VisitStore(Store store)
    {
        stms.Add(new AbsynAssignment(store.Dst, store.Src));
    }

    void InstructionVisitor.VisitSwitchInstruction(SwitchInstruction si)
    {
        throw new NotImplementedException();
    }

    void InstructionVisitor.VisitUseInstruction(UseInstruction u)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region IAbsynVisitor Members

    /// <inheritdoc/>
    public void VisitAssignment(AbsynAssignment ass)
    {
        stms.Add(ass);
    }

    /// <inheritdoc/>
    public void VisitBreak(AbsynBreak brk)
    {
        stms.Add(brk);
    }

    /// <inheritdoc/>
    public void VisitCase(AbsynCase absynCase)
    {
        stms.Add(absynCase);
    }

    /// <inheritdoc/>
    public void VisitCompoundAssignment(AbsynCompoundAssignment compound)
    {
        stms.Add(compound);
    }

    /// <inheritdoc/>
    public void VisitContinue(AbsynContinue cont)
    {
        stms.Add(cont);
    }

    /// <inheritdoc/>
    public void VisitDeclaration(AbsynDeclaration decl)
    {
        stms.Add(decl);
    }

    /// <inheritdoc/>
    public void VisitDefault(AbsynDefault def)
    {
        stms.Add(def);
    }

    /// <inheritdoc/>
    public void VisitDoWhile(AbsynDoWhile loop)
    {
        stms.Add(loop);
    }

    /// <inheritdoc/>
    public void VisitFor(AbsynFor forLoop)
    {
        stms.Add(forLoop);
    }

    /// <inheritdoc/>
    public void VisitGoto(AbsynGoto gotoStm)
    {
        stms.Add(gotoStm);
    }

    /// <inheritdoc/>
    public void VisitIf(AbsynIf ifStm)
    {
        stms.Add(ifStm);
    }

    /// <inheritdoc/>
    public void VisitLabel(AbsynLabel lbl)
    {
        stms.Add(lbl);
    }

    /// <inheritdoc/>
    public void VisitLineComment(AbsynLineComment comment)
    {
        stms.Add(comment);
    }

    /// <inheritdoc/>
    public void VisitReturn(AbsynReturn ret)
    {
        stms.Add(ret);
    }

    /// <inheritdoc/>
    public void VisitSideEffect(AbsynSideEffect side)
    {
        stms.Add(side);
    }

    /// <inheritdoc/>
    public void VisitSwitch(AbsynSwitch absynSwitch)
    {
        stms.Add(absynSwitch);
    }

    /// <inheritdoc/>
    public void VisitWhile(AbsynWhile loop)
    {
        stms.Add(loop);
    }

    #endregion

}
