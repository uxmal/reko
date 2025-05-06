#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Code;
using Reko.Core.Expressions;
using System;

namespace Reko.Core.FingerPrinting;

/// <summary>
/// Computes the hash of all IR instructions in a <see cref="Procedure"/>.
/// </summary>
public class IRHashComputer : IFeatureComputer,
    InstructionVisitor<int>,
    ExpressionVisitor<int>
{
    private const int AddressCode = '@';
    private const int ApplicationCode = '(';
    private const int ArrayAccessCode = '[';
    private const int AssignmentCode = '=';
    private const int BinaryExpressionCode = '2';
    private const int BranchCode = 'b';
    private const int CallInstructionCode = 'c';
    private const int CastCode = '<';
    private const int CommentCode = '\'';
    private const int ConditionalExpressionCode = '?';
    private const int ConditionOfCode = 'C';
    private const int ConstantCode = '0';
    private const int ConversionCode = '>';
    private const int DefInstructionCode = 'd';
    private const int DereferenceCode = '*';
    private const int FieldAccessCode = '.';
    private const int GotoInstructionCode = 'g';
    private const int IdentifierCode = 'i';
    private const int MemberPointerSelectorCode = 'm';
    private const int MemoryAccessCode = 'm';
    private const int MkSequenceCode = 's';
    private const int OutArgumentCode = 'o';
    private const int PhiAssignmentCode = 'F';
    private const int PhiFunctionCode = 'f';
    private const int PointerAdditionCode = 'p';
    private const int ProcedureConstantCode = 'P';
    private const int ReturnCode = 'r';
    private const int ScopeResolutionCode = '#';
    private const int SegmentedAddressCode = ':';
    private const int SideEffectCode = '_';
    private const int SliceCode = 'S';
    private const int StoreCode = ']';
    private const int StringConstantCode = '"';
    private const int SwitchCode = 'w';
    private const int TestConditionCode = 't';
    private const int UnaryExpressionCode = '1';
    private const int UseInstructionCode = 'u';

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Type => "irhash";

    /// <summary>
    /// Computes a hash of all IR instructions in a <see cref="Procedure"/>.
    /// </summary>
    /// <param name="proc"></param>
    /// <param name="program"></param>
    /// <returns></returns>
    public IFeature ComputeFeature(Procedure proc, Program program)
    {
        int hash = 0;
        foreach (var block in proc.ControlGraph.Blocks)
        {
            int blockHash = ComputeBlockHash(block);
            hash = hash * 17 ^ blockHash;
        }
        return new IRHashFeature(hash);
    }

    private int ComputeBlockHash(Block block)
    {
        int hash = 0;
        foreach (var statement in block.Statements)
        {
            int statementHash = statement.Instruction.Accept(this);
            hash = 31 * hash ^ statementHash;
        }
        return hash;
    }

    /// <summary>
    /// Computes the hash of an address.
    /// </summary>
    /// <param name="addr">Value to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitAddress(Address addr)
    {
        return (int)addr.Offset % 64;
    }

    /// <summary>
    /// Computes the hash of an address.
    /// </summary>
    /// <param name="appl">Application to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitApplication(Application appl)
    {
        var hash = ApplicationCode ^ appl.Procedure.Accept(this);
        foreach (var arg in appl.Arguments)
        {
            hash = hash * 17 + arg.Accept(this);
        }
        return hash;
    }

    /// <summary>
    /// Computes the hash of an array access.
    /// </summary>
    /// <param name="acc">Array access to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitArrayAccess(ArrayAccess acc)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of an assignment.
    /// </summary>
    /// <param name="ass">Assignment to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitAssignment(Assignment ass)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a binary expression.
    /// </summary>
    /// <param name="binExp">Binary expression to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitBinaryExpression(BinaryExpression binExp)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a branch.
    /// </summary>
    /// <param name="branch">Branch to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitBranch(Branch branch)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a call instruction.
    /// </summary>
    /// <param name="call">Call instruction to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitCallInstruction(CallInstruction call)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a cast.
    /// </summary>
    /// <param name="cast">Cast to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitCast(Cast cast)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a comment.
    /// </summary>
    /// <param name="comment">Comment to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitComment(CodeComment comment)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a ternary conditional expression.
    /// </summary>
    /// <param name="cond">Conditional expression to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitConditionalExpression(ConditionalExpression cond)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="ConditionOf"/> expression.
    /// </summary>
    /// <param name="cof"><see cref="ConditionOf"/> expression to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitConditionOf(ConditionOf cof)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a constant.
    /// </summary>
    /// <param name="c">Constant to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitConstant(Constant c)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a conversion .
    /// </summary>
    /// <param name="conversion">Conversion to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitConversion(Conversion conversion)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="DefInstruction"/>.
    /// </summary>
    /// <param name="def"><see cref="DefInstruction"/> to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitDefInstruction(DefInstruction def)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="Dereference"/>.
    /// </summary>
    /// <param name="deref">Dereference to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitDereference(Dereference deref)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="FieldAccess"/>.
    /// </summary>
    /// <param name="acc">Field access to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitFieldAccess(FieldAccess acc)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a goto instruction.
    /// </summary>
    /// <param name="gotoInstruction">Goto instruction to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitGotoInstruction(GotoInstruction gotoInstruction)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of an identifier.
    /// </summary>
    /// <param name="id">Identifier to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitIdentifier(Identifier id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="MemberPointerSelector"/>.
    /// </summary>
    /// <param name="mps"><see cref="MemberPointerSelector"/> to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitMemberPointerSelector(MemberPointerSelector mps)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="MemoryAccess"/>.
    /// </summary>
    /// <param name="access"><see cref="MemoryAccess"/> to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitMemoryAccess(MemoryAccess access)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="MkSequence">sequence expression</see>.
    /// </summary>
    /// <param name="seq"><see cref="MkSequence">sequence expression</see> to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitMkSequence(MkSequence seq)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of an <see cref="OutArgument"/>.
    /// </summary>
    /// <param name="outArgument"><see cref="OutArgument"/> to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitOutArgument(OutArgument outArgument)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="PhiAssignment"/>.
    /// </summary>
    /// <param name="phi"><see cref="PhiAssignment"/> to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitPhiAssignment(PhiAssignment phi)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="PhiFunction"/>.
    /// </summary>
    /// <param name="phi"><see cref="PhiFunction"/> to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitPhiFunction(PhiFunction phi)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="PointerAddition"/>.
    /// </summary>
    /// <param name="pa"><see cref="PointerAddition"/> to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitPointerAddition(PointerAddition pa)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="ProcedureConstant"/>.
    /// </summary>
    /// <param name="pc"><see cref="ProcedureConstant"/> to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitProcedureConstant(ProcedureConstant pc)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a return instruction.
    /// </summary>
    /// <param name="ret">Return instruction to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitReturnInstruction(ReturnInstruction ret)
    {
        var hash = ReturnCode;
        if (ret.Expression is not null)
        {
            hash = hash * 17 ^ ret.Expression.Accept(this);
        }
        return hash;
    }

    /// <summary>
    /// Computes the hash of a <see cref="ScopeResolution"/>.
    /// </summary>
    /// <param name="scopeResolution"><see cref="ScopeResolution" />to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitScopeResolution(ScopeResolution scopeResolution)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of an <see cref="SegmentedPointer" />.
    /// </summary>
    /// <param name="address"><see cref="SegmentedPointer"/> to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitSegmentedAddress(SegmentedPointer address)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="SideEffect"/>.
    /// </summary>
    /// <param name="side"><see cref="SideEffect"/> to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitSideEffect(SideEffect side)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="Slice"/> expression.
    /// </summary>
    /// <param name="slice"><see cref="Slice"/> expression to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitSlice(Slice slice)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="Store"/> instruction.
    /// </summary>
    /// <param name="store">Application to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitStore(Store store)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="StringConstant"/>,
    /// </summary>
    /// <param name="str"><see cref="StringConstant"/> to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitStringConstant(StringConstant str)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a switch instruction.
    /// </summary>
    /// <param name="si">Switch instruction to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitSwitchInstruction(SwitchInstruction si)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of an <see cref="TestCondition"/>.
    /// </summary>
    /// <param name="tc">Test condtiion to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitTestCondition(TestCondition tc)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a unary expression.
    /// </summary>
    /// <param name="unary">Unary expression to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitUnaryExpression(UnaryExpression unary)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Computes the hash of a <see cref="UseInstruction"/>.
    /// </summary>
    /// <param name="use">Application to compute the hash of.</param>
    /// <returns>The computed hash.</returns>
    public int VisitUseInstruction(UseInstruction use)
    {
        throw new NotImplementedException();
    }


}
