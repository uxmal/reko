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
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Evaluation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        where TBlock : class
        where TInstr : class
	{
        private static readonly TraceSwitch trace = new("BackWalker", "Traces the progress backward instruction walking");

        private readonly IBackWalkHost<TBlock, TInstr> host;
        private readonly ExpressionSimplifier eval;
        private Identifier? UsedAsFlag;
        private TBlock? startBlock;

        public Backwalker(IBackWalkHost<TBlock, TInstr> host, RtlTransfer xfer, ExpressionSimplifier eval)
		{
            this.host = host;
            this.eval = eval;
            var target = xfer.Target;
            if (xfer.Target is MkSequence seq && seq.Expressions.Length == 2)
            {
                target = seq.Expressions[1];
            }
            if (target is MemoryAccess mem)
            {
                Index = DetermineIndexRegister(mem);
            }
            else
            {
                Index = RegisterOf(target as Identifier);
            }
            Operations = new List<BackwalkOperation>();
            JumpSize = target.DataType.Size;
        }

        /// <summary>
        /// The register used to perform a table-dispatch switch.
        /// </summary>
        public RegisterStorage? Index { get; private set; }
        public Expression? IndexExpression { get; set; }
        public Identifier? UsedFlagIdentifier { get; set; }
        public int Stride { get; private set; }
        public Address? VectorAddress { get; private set; }
        public List<BackwalkOperation> Operations { get; private set; }
        public int JumpSize { get; set; }

        /// <summary>
        /// Walks backward along the <paramref name="block"/>, recording the operations 
        /// done to the idx register. The operations are used to reconstruct
        /// the indexing expression, which gives clues to its layout.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public List<BackwalkOperation>? BackWalk(TBlock block)
        {
            this.startBlock = block;
            if (Stride > 1)
                Operations.Add(new BackwalkOperation(BackwalkOperator.mul, Stride));

            bool continueBackwalking = BackwalkInstructions(block);
            if ((Index is null || Index == RegisterStorage.None) && IndexExpression is null)
                return null;	// unable to find guard.

            if (continueBackwalking)
            {
                var blockPrev = host.GetPredecessors(block).SingleOrDefault();
                if (blockPrev is null)
                    return null;	// seems unguarded to me.

                block = blockPrev;
                BackwalkInstructions(block);
                if (Index is null && IndexExpression is null)
                    return null;
            }
            Operations.Reverse();
            return Operations;
        }

        public bool BackwalkInstruction(TInstr instr)
        {
            var ass = host.AsAssignment(instr);
            if (ass.Item1 is not null)
            {
                var (assSrc, _) = ass.Item2!.Accept(eval);
                var assDst = ass.Item1;
                var regSrc = RegisterOf(assSrc);
                if (assSrc is BinaryExpression binSrc)
                {
                    if (RegisterOf(assDst) == Index || assDst == IndexExpression)
                    {
                        regSrc = RegisterOf(binSrc.Left);
                        var immSrc = binSrc.Right as Constant;
                        //$TODO: AddOrSub
                        switch (binSrc.Operator.Type)
                        {
                        case OperatorType.IAdd:
                            Index = HandleAddition(Index, regSrc, immSrc!, true);
                            return true;
                        case OperatorType.ISub:
                            Index = HandleAddition(Index, regSrc, immSrc!, false);
                            return true;
                        case OperatorType.And:
                            if (immSrc is not null && Bits.IsEvenPowerOfTwo(immSrc.ToInt32() + 1))
                            {
                                Operations.Add(new BackwalkOperation(BackwalkOperator.cmp, immSrc.ToInt32() + 1));
                            }
                            else
                            {
                                Index = null;
                            }
                            return false;
                        case OperatorType.IMul:
                        case OperatorType.SMul:
                        case OperatorType.UMul:
                            if (immSrc is not null)
                            {
                                var m = immSrc.ToInt32();
                                Operations.Add(new BackwalkOperation(BackwalkOperator.mul, m));
                                Stride *= m;
                                return true;
                            }
                            break;
                        case OperatorType.Shl:
                            if (immSrc is not null)
                            {
                                var m = 1 << immSrc.ToInt32();
                                Operations.Add(new BackwalkOperation(BackwalkOperator.mul, m));
                                Stride *= m;
                                return true;
                            }
                            break;
                        }
                    }
                    if (Index is not null &&
                        binSrc.Operator.Type == OperatorType.Xor &&
                        binSrc.Left == assDst &&
                        binSrc.Right == assDst &&
                        RegisterOf(assDst) == host.GetSubregister(Index, new BitRange(8, 16)))
                    {
                        Operations.Add(new BackwalkOperation(BackwalkOperator.and, 0xFF));
                        Index = host.GetSubregister(Index, new BitRange(0, 8));
                    }
                }
                if (Index is not null &&
                    assSrc is Constant cSrc &&
                    cSrc.IsIntegerZero &&
                    RegisterOf(assDst) == host.GetSubregister(Index, new BitRange(8, 16)))
                {
                    // mov bh,0 ;; xor bh,bh
                    // jmp [bx...]
                    Operations.Add(new BackwalkOperation(BackwalkOperator.and, 0xFF));
                    Index = host.GetSubregister(Index, new BitRange(0, 8));
                    return true;
                }
                if (assSrc is ConditionOf cof && UsedFlagIdentifier is not null)
                {
                    var grfDef = (((Identifier)assDst).Storage as FlagGroupStorage)!.FlagGroupBits;
                    var grfUse = (UsedFlagIdentifier.Storage as FlagGroupStorage)!.FlagGroupBits;
                    if ((grfDef & grfUse) == 0)
                        return true;
                    var binCmp = cof.Expression as BinaryExpression;
                    binCmp = NegateRight(binCmp);
                    if (binCmp is not null &&
                        (binCmp.Operator is ISubOperator ||
                         binCmp.Operator is USubOperator))
                    {
                        var idLeft = RegisterOf(binCmp.Left  as Identifier);
                        if (idLeft is not null &&
                            (idLeft == Index || idLeft == host.GetSubregister(Index!, new BitRange(0, 8))) ||
                           (IndexExpression is not null && IndexExpression.ToString() == idLeft!.ToString()))    //$HACK: sleazy, but we don't appear to have an expression comparer
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
                var src = assSrc is InvalidConstant ? ass.Item2 : assSrc;
                var cvtSrc = src as Conversion;
                if (cvtSrc is not null)
                    src = cvtSrc.Expression;
                var regDst = RegisterOf(assDst);
                if (src is MemoryAccess memSrc &&
                    (regDst == Index ||
                     (Index is not null && regDst is not null && regDst.Name != "None" && regDst.IsSubRegisterOf(Index))))
                {
                    // R = Mem[xxx]
                    var rIdx = Index;
                    var rDst = RegisterOf(assDst);
                    if ((rDst != host.GetSubregister(rIdx, new BitRange(0, 8)) && cvtSrc is null) &&
                        rDst != rIdx)
                    {
                        Index = RegisterStorage.None;
                        IndexExpression = src;
                        return true;
                    }

                    if (memSrc.EffectiveAddress is not BinaryExpression binEa)
                    {
                        Index = RegisterStorage.None;
                        IndexExpression = null;
                        return false;
                    }
                    var scale = GetMultiplier(binEa.Left);
                    var baseReg = GetBaseRegister(binEa.Left);
                    if (binEa.Right is Constant memOffset && binEa.Operator.Type == OperatorType.IAdd)
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
                    baseReg = GetBaseRegister(binEa.Right);
                    if (!(binEa.Left is not Address addr || VectorAddress is null))
                    {
                        this.VectorAddress = addr;
                        Index = baseReg;
                        return true;
                    }
                    Index = RegisterStorage.None;
                    IndexExpression = ass.Item2;
                    return true;
                }

                if (regSrc is not null && regDst == Index)
                {
                    Index = regSrc;
                    return true;
                }
                UsedAsFlag = null;
                return true;
            }

            var bra = host.AsBranch(instr);
            if (bra is not null)
            {
                bool fallthrough = host.IsFallthrough(instr, startBlock!);
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

        private static BinaryExpression? NegateRight(BinaryExpression? bin)
        {
            if (bin is not null &&
                (bin.Operator.Type == OperatorType.IAdd) &&
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

        private static RegisterStorage RegisterOf(Expression? e)
        {
            if (e is Conversion c)
                e = c.Expression;
            if (e is Identifier id && id.Storage is RegisterStorage reg)
                return reg;
                return RegisterStorage.None;
        }

        public bool BackwalkInstructions(
            IEnumerable<TInstr> backwardStatementSequence)
        {
            foreach (var instr in backwardStatementSequence)
            {
                if (!BackwalkInstruction(instr))
                    return false;
            }
            return true;
        }

        public bool BackwalkInstructions(TBlock block)
        {
            return BackwalkInstructions(host.GetBlockInstructions(block)
                .Select(p => p.Item2).Reverse()!);
        }

        [Conditional("DEBUG")]
        public void DumpBlock(RegisterStorage regIdx, Block block)
        {
            Debug.Print("Backwalking register {0} through block: {1}", regIdx, block.DisplayName);
            foreach (var stm in block.Statements  )
            {
                Debug.Print("    {0}", stm.Instruction);
            }
        }

        private static RegisterStorage? GetBaseRegister(Expression ea)
        {
            if (ea is Identifier id)
                return RegisterOf(id);
            if (ea is not BinaryExpression bin)
                return RegisterStorage.None;
            var e = bin.Left;
            while (e is not null && e is Cast cast)
            {
                e = cast.Expression;
            }
            if (e is null)
                return null;
            if (e is Identifier idBase)
                return RegisterOf(idBase);
            if (bin is null)
                return RegisterStorage.None;
            var scaledExpr = bin.Left as BinaryExpression;
            return RegisterOf(scaledExpr!.Left as Identifier);
        }

        private static int GetMultiplier(Expression exp)
        {
            if (exp is not BinaryExpression bin)
                return 1;
            if (bin.Operator is IMulOperator)
            {
                if (bin.Right is not Constant scale)
                    return 1;
                return scale.ToInt32();
            }
            else
                return 1;
        }

        public bool CanBackwalk()
        {
            return Index is not null;
        }

        [Conditional("DEBUG")]
        private static void DumpInstructions(StatementList instrs, int idx)
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
        public RegisterStorage? DetermineIndexRegister(MemoryAccess mem)
        {
            Stride = 0;
            // Mem[reg]
            if (mem.EffectiveAddress is Identifier id)
            {
                Stride = 1;
                return RegisterOf(id);
            }
            if (mem.EffectiveAddress is not BinaryExpression bin)
                return null;

            var idLeft = bin.Left as Identifier;
            var idRight = bin.Right as Identifier;
            if (idRight is not null && idLeft is null)
            {
                // Rearrange so that the effective address is [id + C]
                (idRight, idLeft) = (idLeft, idRight);
            }
            if (idLeft is not null && idRight is not null)
            {
                // Can't handle [id1 + id2] yet.
                return null;
            }
            if (idLeft is { })
            {
                // We have [id + C]
                Stride = 1;
                if (host.IsStackRegister(idLeft.Storage))
                    return null;
                DetermineVector(mem, bin.Right);
                if (VectorAddress is not null && host.IsValidAddress(VectorAddress.Value))
                    return RegisterOf(idLeft);
                else
                    return null;
            }
            var binLeft = bin.Left as BinaryExpression;
            if (IsScaledIndex(binLeft))
            {
                // We have [(id * C1) + C2]
                return DetermineVectorWithScaledIndex(mem, bin.Right, binLeft!);
            }
            var binRight = bin.Right as BinaryExpression;
            if (IsScaledIndex(binRight))
            {
                // We may have [C1 + (id * C2)]
                return DetermineVectorWithScaledIndex(mem, bin.Left, binRight!);
            }
            return null;
        }

        private static bool IsScaledIndex(BinaryExpression? bin)
        {
            return bin is not null && bin.Operator is IMulOperator && bin.Right is Constant;
        }

        private RegisterStorage DetermineVectorWithScaledIndex(MemoryAccess mem, Expression possibleVector, BinaryExpression scaledIndex)
        {
            Stride = ((Constant)scaledIndex.Right).ToInt32();   // Mem[x + reg * C]
            DetermineVector(mem, possibleVector);
            return RegisterOf(scaledIndex.Left as Identifier);
        }

        private void DetermineVector(MemoryAccess mem, Expression possibleVector)
        {
            if (possibleVector is not Constant vector)
                return;
            if (vector.DataType.Domain == Domain.SignedInt)
                return;
            if (mem.EffectiveAddress is SegmentedPointer segmem)
            {
                var (e, _) = segmem.BasePointer.Accept(eval);
                if (e is Constant selector)
                {
                    VectorAddress = host.MakeSegmentedAddress(selector, vector);
                }
            }
            else
            {
                VectorAddress = host.MakeAddressFromConstant(vector);   //$BUG: 32-bit only, what about 16- and 64-
            }
        }

        private RegisterStorage? HandleAddition(
			RegisterStorage? ropDst,
			RegisterStorage? ropSrc, 
			Constant immSrc, 
			bool add)
		{
			if (ropSrc is not null && immSrc is null)
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
			
            if (immSrc is not null)
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
