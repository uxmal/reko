#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Core.Rtl;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Reko.Core.Lib;

namespace Reko.Scanning
{
	/// <summary>
	/// Walks code backwards to find "dominating" comparisons against constants,
	/// which may provide vector table limits.
	/// </summary>
    /// <remarks>
    /// This is a godawful hack; a proper range analysis would be much
    /// better. Have a spare few months?
    /// </remarks>
	public class Backwalker<TBlock, TInstr>
	{
        private IBackWalkHost<TBlock, TInstr> host;
        private ExpressionSimplifier eval;
        private Identifier UsedAsFlag;

        private static TraceSwitch trace = new TraceSwitch("BackWalker", "Traces the progress backward instruction walking");
        private TBlock startBlock;

        public Backwalker(IBackWalkHost<TBlock, TInstr> host, RtlTransfer xfer, ExpressionSimplifier eval)
		{
            this.host = host;
            this.eval = eval;
            var target = xfer.Target;
            if (xfer.Target is MkSequence seq && seq.Expressions.Length == 2)
            {
                target = seq.Expressions[1];
            }
            var mem = target as MemoryAccess;
            if (mem == null)
            {
                Index = RegisterOf(target as Identifier);
            }
            else
            {
                Index = DetermineIndexRegister(mem);
            }
            Operations = new List<BackwalkOperation>();
            JumpSize = target.DataType.Size;
        }

        /// <summary>
        /// The register used to perform a table-dispatch switch.
        /// </summary>
        public RegisterStorage Index { get; private set; }
        public Expression IndexExpression { get; set; }
        public Identifier UsedFlagIdentifier { get; set; }
        public int Stride { get; private set; }
        public Address VectorAddress { get; private set; }
        public List<BackwalkOperation> Operations { get; private set; }
        public int JumpSize { get; set; }

        /// <summary>
        /// Walks backward along the <paramref name="block"/>, recording the operations 
        /// done to the idx register. The operations are used to reconstruct
        /// the indexing expression, which gives clues to its layout.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public List<BackwalkOperation> BackWalk(TBlock block)
        {
            this.startBlock = block;
            if (Stride > 1)
                Operations.Add(new BackwalkOperation(BackwalkOperator.mul, Stride));

            bool continueBackwalking = BackwalkInstructions(Index, block);
            if ((Index == null || Index == RegisterStorage.None) && IndexExpression == null)
                return null;	// unable to find guard.

            if (continueBackwalking)
            {
                block = host.GetSinglePredecessor(block);
                if (block == null)
                    return null;	// seems unguarded to me.

                BackwalkInstructions(Index, block);
                if (Index == null && IndexExpression == null)
                    return null;
            }
            Operations.Reverse();
            return Operations;
        }

        public bool BackwalkInstruction(TInstr instr)
        {
            var ass = host.AsAssignment(instr);
            if (ass != null)
            {
                var assSrc = ass.Item2.Accept(eval);
                var assDst = ass.Item1;
                var regSrc = RegisterOf(assSrc);
                if (assSrc is BinaryExpression binSrc)
                {
                    if (RegisterOf(assDst) == Index || assDst == IndexExpression)
                    {
                        regSrc = RegisterOf(binSrc.Left);
                        var immSrc = binSrc.Right as Constant;
                        if (binSrc.Operator == Operator.IAdd || binSrc.Operator == Operator.ISub)
                        {
                            Index = HandleAddition(Index, Index, regSrc, immSrc, binSrc.Operator == Operator.IAdd);
                            return true;
                        }
                        if (binSrc.Operator == Operator.And)
                        {
                            if (immSrc != null && Bits.IsEvenPowerOfTwo(immSrc.ToInt32() + 1))
                            {
                                Operations.Add(new BackwalkOperation(BackwalkOperator.cmp, immSrc.ToInt32() + 1));
                            }
                            else
                            {
                                Index = null;
                            }
                            return false;
                        }
                        if (binSrc.Operator is IMulOperator && immSrc != null)
                        {
                            var m = immSrc.ToInt32();
                            Operations.Add(new BackwalkOperation(BackwalkOperator.mul, m));
                            Stride *= m;
                            return true;
                        }
                        if (binSrc.Operator is ShlOperator && immSrc != null)
                        {
                            var m = 1 << immSrc.ToInt32();
                            Operations.Add(new BackwalkOperation(BackwalkOperator.mul, m));
                            Stride *= m;
                            return true;
                        }
                    }
                    if (Index != null &&
                        binSrc.Operator == Operator.Xor &&
                        binSrc.Left == assDst &&
                        binSrc.Right == assDst &&
                        RegisterOf(assDst) == host.GetSubregister(Index, 8, 8))
                    {
                        Operations.Add(new BackwalkOperation(BackwalkOperator.and, 0xFF));
                        Index = host.GetSubregister(Index, 0, 8);
                    }
                }
                var cSrc = assSrc as Constant;
                if (Index != null &&
                    cSrc != null &&
                    cSrc.IsIntegerZero &&
                    RegisterOf(assDst) == host.GetSubregister(Index, 8, 8))
                {
                    // mov bh,0 ;; xor bh,bh
                    // jmp [bx...]
                    Operations.Add(new BackwalkOperation(BackwalkOperator.and, 0xFF));
                    Index = host.GetSubregister(Index, 0, 8);
                    return true;
                }
                if (assSrc is ConditionOf cof && UsedFlagIdentifier != null)
                {
                    var grfDef = (((Identifier)assDst).Storage as FlagGroupStorage).FlagGroupBits;
                    var grfUse = (UsedFlagIdentifier.Storage as FlagGroupStorage).FlagGroupBits;
                    if ((grfDef & grfUse) == 0)
                        return true;
                    var binCmp = cof.Expression as BinaryExpression;
                    binCmp = NegateRight(binCmp);
                    if (binCmp != null &&
                        (binCmp.Operator is ISubOperator ||
                         binCmp.Operator is USubOperator))
                    {
                        var idLeft = RegisterOf(binCmp.Left  as Identifier);
                        if (idLeft != null &&
                            (idLeft == Index || idLeft == host.GetSubregister(Index, 0, 8)) ||
                           (IndexExpression != null && IndexExpression.ToString() == idLeft.ToString()))    //$HACK: sleazy, but we don't appear to have an expression comparer
                        {
                            if (binCmp.Right is Constant immSrc)
                            {
                                // Found the bound of the table.
                                Operations.Add(new BackwalkOperation(BackwalkOperator.cmp, immSrc.ToInt32()));
                                return false;
                            }
                        }
                    }
                    if (cof.Expression is Identifier idCof)
                    {
                        IndexExpression = idCof;
                        Index = null;
                        UsedFlagIdentifier = null;
                        UsedAsFlag = idCof;
                        return true;
                    }
                }

                //$BUG: this is rubbish, the simplifier should _just_
                // perform simplification, no substitutions.
                var src = assSrc == Constant.Invalid ? ass.Item2 : assSrc;
                var castSrc = src as Cast;
                if (castSrc != null)
                    src = castSrc.Expression;
                var regDst = RegisterOf(assDst);
                if (src is MemoryAccess memSrc &&
                    (regDst == Index ||
                     (Index != null && regDst != null && regDst.Name != "None" && regDst.IsSubRegisterOf(Index))))
                {
                    // R = Mem[xxx]
                    var rIdx = Index;
                    var rDst = RegisterOf(assDst);
                    if ((rDst != host.GetSubregister(rIdx, 0, 8) && castSrc == null) &&
                        rDst != rIdx)
                    {
                        Index = RegisterStorage.None;
                        IndexExpression = src;
                        return true;
                    }

                    var binEa = memSrc.EffectiveAddress as BinaryExpression;
                    if (binEa == null)
                    {
                        Index = RegisterStorage.None;
                        IndexExpression = null;
                        return false;
                    }
                    var scale = GetMultiplier(binEa.Left);
                    var baseReg = GetBaseRegister(binEa.Left);
                    if (binEa.Right is Constant memOffset && binEa.Operator == Operator.IAdd)
                    {
                        var mOff = memOffset.ToInt32();
                        if (mOff > 0x200)
                        {
                            Operations.Add(new BackwalkDereference(memOffset.ToInt32(), scale));
                            Index = baseReg;
                            return true;
                        }
                    }

                    // Some architectures have pc-relative addressing, which the rewriters
                    // should convert to an _address_.
                    var addr = binEa.Left as Address;
                    baseReg = GetBaseRegister(binEa.Right);
                    if (addr != null && VectorAddress == null)
                    {
                        this.VectorAddress = addr;
                        Index = baseReg;
                        return true;
                    }
                    Index = RegisterStorage.None;
                    IndexExpression = ass.Item2;
                    return true;
                }

                if (regSrc != null && regDst == Index)
                {
                    Index = regSrc;
                    return true;
                }
                UsedAsFlag = null;
                return true;
            }

            var bra = host.AsBranch(instr);
            if (bra != null)
            {
                bool fallthrough = host.IsFallthrough(instr, startBlock);
                return VisitBranch(bra, fallthrough);
            }

            Debug.WriteLine("Backwalking not supported: " + instr);
            return true;
        }

        private bool VisitBranch(Expression bra, bool fallthrough)
        {
            if (bra is TestCondition cond)
            {
                ConditionCode cc = cond.ConditionCode;
                switch (cc)
                {
                case ConditionCode.UGE:
                case ConditionCode.UGT:
                case ConditionCode.GT:
                    break;
                    //$TODO: verify the branch direction here.
                case ConditionCode.ULE:
                    fallthrough = !fallthrough;
                    cc = ConditionCode.UGT;
                    break;
                case ConditionCode.ULT:
                    fallthrough = !fallthrough;
                    cc = ConditionCode.UGE;
                    break;
                default: return true;
                }
                if (fallthrough)
                {
                    Operations.Add(new BackwalkBranch(cc));
                    UsedFlagIdentifier = (Identifier)cond.Expression;
                }
            }
            return true;
        }

        private BinaryExpression NegateRight(BinaryExpression bin)
        {
            if (bin != null &&
                (bin.Operator == Operator.IAdd) &&
                bin.Right is Constant cRight)
            {
                return new BinaryExpression(
                    Operator.ISub,
                    bin.Left.DataType,
                    bin.Left,
                    cRight.Negate());
            }
            return bin;
        }

        private RegisterStorage RegisterOf(Expression e)
        {
            if (e is Cast c)
                e = c.Expression;
            if (e is Identifier id && id.Storage is RegisterStorage reg)
                return reg;
                return RegisterStorage.None;
        }

        public bool BackwalkInstructions(
            RegisterStorage regIdx,
            IEnumerable<TInstr> backwardStatementSequence)
        {
            foreach (var instr in backwardStatementSequence)
            {
                if (!BackwalkInstruction(instr))
                    return false;
            }
            return true;
        }

        public bool BackwalkInstructions(
            RegisterStorage regIdx,
            TBlock block)
        {
            return BackwalkInstructions(regIdx, host.GetBlockInstructions(block).Reverse());
        }

        [Conditional("DEBUG")]
        public void DumpBlock(RegisterStorage regIdx, Block block)
        {
            Debug.Print("Backwalking register {0} through block: {1}", regIdx, block.Name);
            foreach (var stm in block.Statements  )
            {
                Debug.Print("    {0}", stm.Instruction);
            }
        }

        private RegisterStorage GetBaseRegister(Expression ea)
        {
            if (ea is Identifier id)
                return RegisterOf(id);
            var bin = ea as BinaryExpression;
            if (bin == null)
                return RegisterStorage.None;
            var e = bin.Left;
            while (e != null && e is Cast)
            {
                e = ((Cast)e).Expression;
            }
            if (e == null)
                return null;
            id = e as Identifier;
            if (id != null)
                return RegisterOf(id);
            var scaledExpr = bin.Left as BinaryExpression;
            if (bin == null)
                return RegisterStorage.None;
            return RegisterOf(scaledExpr.Left as Identifier);
        }

        private int GetMultiplier(Expression exp)
        {
            var bin = exp as BinaryExpression;
            if (bin == null)
                return 1;
            if (bin.Operator is IMulOperator)
            {
                var scale = bin.Right as Constant;
                if (scale == null)
                    return 1;
                return scale.ToInt32();
            }
            else
                return 1;
        }

        public bool CanBackwalk()
        {
            return Index != null;
        }

        [Conditional("DEBUG")]
        private void DumpInstructions(StatementList instrs, int idx)
        {
            for (int i = 0; i < instrs.Count; ++i)
            {
                Debug.WriteLineIf(trace.TraceInfo,
                    string.Format("{0} {1}",
                    idx == i ? '*' : ' ',
                    instrs[i]));
            }
        }
		
        /// <summary>
        /// Given a memory access, attempts to determine the index register.
        /// </summary>
        /// <param name="mem"></param>
        /// <returns></returns>
        public RegisterStorage DetermineIndexRegister(MemoryAccess mem)
        {
            Stride = 0;
            // Mem[reg]
            if (mem.EffectiveAddress is Identifier id)
            {
                Stride = 1;
                return RegisterOf(id);
            }
            var bin = mem.EffectiveAddress as BinaryExpression;
            if (bin == null)
                return null;

            var idLeft = bin.Left as Identifier;
            var idRight = bin.Right as Identifier;
            if (idRight != null && idLeft == null)
            {
                // Rearrange so that the effective address is [id + C]
                var t = idLeft;
                idLeft = idRight;
                idRight = t;
            }
            if (idLeft != null && idRight != null)
            {
                // Can't handle [id1 + id2] yet.
                return null;
            }
            if (idLeft != null)
            {
                // We have [id + C]
                Stride = 1;
                if (host.IsStackRegister(idLeft.Storage))
                    return null;
                DetermineVector(mem, bin.Right);
                if (VectorAddress != null && host.IsValidAddress(VectorAddress))
                    return RegisterOf(idLeft);
                else
                    return null;
            }
            var binLeft = bin.Left as BinaryExpression;
            if (IsScaledIndex(binLeft))
            {
                // We have [(id * C1) + C2]
                return DetermineVectorWithScaledIndex(mem, bin.Right, binLeft);
            }
            var binRight = bin.Right as BinaryExpression;
            if (IsScaledIndex(binRight))
            {
                // We may have [C1 + (id * C2)]
                return DetermineVectorWithScaledIndex(mem, bin.Left, binRight);
            }
            return null;
        }

        private bool IsScaledIndex(BinaryExpression bin)
        {
            return bin != null && bin.Operator is IMulOperator && bin.Right is Constant;
        }

        private RegisterStorage DetermineVectorWithScaledIndex(MemoryAccess mem, Expression possibleVector, BinaryExpression scaledIndex)
        {
            Stride = ((Constant)scaledIndex.Right).ToInt32();   // Mem[x + reg * C]
            DetermineVector(mem, possibleVector);
            return RegisterOf(scaledIndex.Left as Identifier);
        }

        private void DetermineVector(MemoryAccess mem, Expression possibleVector)
        {
            var vector = possibleVector as Constant;
            if (vector == null)
                return;
            if (vector.DataType is PrimitiveType pt && pt.Domain == Domain.SignedInt)
                return;
            if (mem is SegmentedAccess segmem)
            {
                if (segmem.BasePointer.Accept(eval) is Constant selector)
                {
                    VectorAddress = host.MakeSegmentedAddress(selector, vector);
                }
            }
            else
            {
                VectorAddress = host.MakeAddressFromConstant(vector);   //$BUG: 32-bit only, what about 16- and 64-
            }
        }

        private RegisterStorage HandleAddition(
			RegisterStorage regIdx,
			RegisterStorage ropDst,
			RegisterStorage ropSrc, 
			Constant immSrc, 
			bool add)
		{
			if (ropSrc != null && immSrc == null)
			{
				if (ropSrc == ropDst && add)
				{
					Operations.Add(new BackwalkOperation(BackwalkOperator.mul, 2));
                    Stride *= 2;
					return ropSrc;
				}		
				else
				{
					return null;
				}
			} 
			
            if (immSrc != null)
			{
                if (!add && UsedAsFlag == IndexExpression)
                {
                    Operations.Add(
                        new BackwalkOperation(
                            BackwalkOperator.cmp,
                            immSrc.ToInt32()));
                }
                else
                {
                    Operations.Add(new BackwalkOperation(
                        add ? BackwalkOperator.add : BackwalkOperator.sub,
                        immSrc.ToInt32()));
                }
				return ropSrc;
			}
			else
				return null;
		}
    }
}
