#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Decompiler.Core
{
	/// <summary>
	/// Walks code backwards to find "dominating" comparisons against constants,
	/// which may provide vector table limits.
	/// </summary>
	internal class Backwalker
	{
        private Identifier regIdx;
		private bool returnToCaller;
        private static TraceSwitch trace = new TraceSwitch("BackWalker", "Traces the progress backward instruction walking");

		public Backwalker(RtlTransfer xfer, Expression  eval)
		{
            var mem = xfer.Target as MemoryAccess;
            if (mem == null)
                throw new ArgumentException("Expected an indirect JMP or CALL");
            DetermineIndexRegister(mem);
            Index = regIdx;
            Operations = new List<BackwalkOperation>();
		}

        /// <summary>
        /// The register used to perform a table-dispatch switch.
        /// </summary>
        public virtual MachineRegister IndexRegister { get; private set; }
        public Identifier Index { get; private set; }
        public int Stride { get; private set; }
        public Address VectorAddress { get; private set; }
        public List<BackwalkOperation> Operations { get; private set; }

        public virtual List<BackwalkOperation> BackWalk(Address addrFrom, IBackWalkHost host)
        {
            throw new NotImplementedException();
        }

        public List<BackwalkOperation> BackWalk(Block block, IBackWalkHost host)
        {
            // Record the operations done to the idx register.

            if (Stride > 1)
                Operations.Add(new BackwalkOperation(BackwalkOperator.mul, Stride));

            returnToCaller = false;

            regIdx = BackwalkInstructions(regIdx, block.Statements);
            if (regIdx == null)
                return null;	// unable to find guard. //$REVIEW: return warning.

            if (!returnToCaller)
            {
                block = block.Procedure.ControlGraph.Predecessors(block).FirstOrDefault();
                if (block == null)
                    return null;	// seems unguarded to me.	//$REVIEW: emit warning.

                regIdx = BackwalkInstructions(regIdx, block.Statements);
                if (regIdx == null)
                    return null;
            }
            Operations.Reverse();
            Index = regIdx;
            return Operations;
        }

        public Identifier BackwalkInstructions(
            Identifier regIdx,
            StatementList stms)
        {
            for (int i = stms.Count-1; i >= 0; --i)
            {
                var instr = stms[i].Instruction;
                var ass = instr as Assignment;
                if (ass != null)
                {
                    var idDst = ass.Dst;
                    var idSrc = ass.Src as Identifier;
                    var binSrc = ass.Src as BinaryExpression;
                    if (binSrc != null)
                    {
                        idSrc  = binSrc.Left as Identifier;
                        var immSrc = binSrc.Right as Constant;
                        if (binSrc.op == Operator.Add || binSrc.op == Operator.Sub)
                        {
                            if (idDst == regIdx)
                            {
                                regIdx = HandleAddition(regIdx, idDst, idSrc, immSrc, binSrc.op == Operator.Add);
                            }
                        }
                        else if (binSrc.op == Operator.And)
                        {
                            if (idDst == regIdx)
                            {
                                if (immSrc != null && IsEvenPowerOfTwo(immSrc.ToInt32() + 1))
                                {
                                    Operations.Add(new BackwalkOperation(BackwalkOperator.cmp, immSrc.ToInt32() + 1));
                                    returnToCaller = true;
                                }
                                else
                                {
                                    regIdx = null;
                                }
                                return regIdx;
                            }
                        }
                        else if (binSrc.op == Operator.Shl)
                        {
                            if (idDst == regIdx)
                            {
                        if (immSrc != null)
                        {
                            Operations.Add(new BackwalkOperation(BackwalkOperator.mul, 1 << immSrc.ToInt32()));
                        }
                        else
                            return null;
                            }
                        }
                //case Opcode.xor:
                //    if (ropDst != null && ropSrc != null &&
                //        ropSrc.Register == ropDst.Register &&
                //        ropDst.Register == regIdx.GetSubregister(8, 8))
                //    {
                //        operations.Add(new BackwalkOperation(BackwalkOperator.and, 0xFF));
                //        regIdx = regIdx.GetSubregister(0, 8);
                //    }
                //    break;

                    }
                    if (idSrc != null)
                    {
                        if (idDst == regIdx)
                        {
                            regIdx = idSrc;
                        }
                        else 
                        {
                            regIdx = null;
                            throw new NotImplementedException();
                        //    if (ropDst.Register == regIdx.GetSubregister(0, 8) &&
                        //    memSrc != null && memSrc.Offset != null &&
                        //    memSrc.Base != MachineRegister.None)
                        //{
                        //    operations.Add(new BackwalkDereference(memSrc.Offset.ToInt32(), memSrc.Scale));
                        //    regIdx = memSrc.Base;
                        //}
                        }
                    }
                    break;

                //case Opcode.cmp:
                //    if (ropDst != null &&
                //        (ropDst.Register == regIdx || ropDst.Register == regIdx.GetPart(PrimitiveType.Byte)))
                //    {
                //        if (immSrc != null)
                //        {
                //            operations.Add(new BackwalkOperation(BackwalkOperator.cmp, immSrc.Value.ToInt32()));
                //            return regIdx;
                //        }
                //    }
                //    break;
                }
                var bra = instr as Branch;
                if (bra != null)
                {
                    var cond = bra.Condition as TestCondition;
                    if (cond != null && cond.ConditionCode == ConditionCode.UGT)
                    {
                        Operations.Add(new BackwalkBranch(ConditionCode.UGT));
                    }
                }
                else
                {
                    Debug.WriteLine("Backwalking not supported: " + instr);
                    DumpInstructions(stms, i);
                    break;
                }
                if (regIdx == null)
                    break;
            }
            return regIdx;
        }

        public bool CanBackwalk()
        {
            return regIdx != null;
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
		
        public void DetermineIndexRegister(MemoryAccess mem)
        {
            regIdx = null;
            Stride = 0;
            var id = mem.EffectiveAddress as Identifier;    // Mem[reg]
            if (id != null)
            {
                regIdx = id;
                Stride = 1;
                VectorAddress = null;       // Don't know where the table starts!
                return;
            }

            var bin = mem.EffectiveAddress as BinaryExpression;

            var idLeft = bin.Left as Identifier;
            var idRight = bin.Right as Identifier;
            if (idRight != null && idLeft == null)
            {
                var t = idLeft;
                idLeft = idRight;
                idRight = t;
            }
            if (idLeft != null && idRight != null)
                return; ;
            var binRight = bin.Right as BinaryExpression;
            if (binRight != null && binRight.op is MulOperator && binRight.Right is Constant)
            {
                Stride = ((Constant)binRight.Right).ToInt32();   // Mem[x + reg * C]
                regIdx = binRight.Left as Identifier;
                return;
            }
        }

        private Identifier HandleAddition(
			Identifier regIdx,
			Identifier ropDst,
			Identifier ropSrc, 
			Constant immSrc, 
			bool add)
		{
			if (ropSrc != null)
			{
				if (ropSrc == ropDst && add)
				{
					Operations.Add(new BackwalkOperation(BackwalkOperator.mul, 2));
					return ropSrc;
				}		
				else
				{
					return null;
				}
			} 
			else if (immSrc != null)
			{
				Operations.Add(new BackwalkOperation(
					add ? BackwalkOperator.add : BackwalkOperator.sub,
					immSrc.ToInt32()));
				return regIdx;
			}
			else
				return null;
		}

		public static bool IsEvenPowerOfTwo(int n)
		{
			return n != 0 && (n & (n - 1)) == 0;
		}



    }
}
