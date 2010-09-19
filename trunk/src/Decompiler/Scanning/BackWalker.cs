#region License
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
#endregion

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Scanning
{
    public class BackWalker2
    {
        private IProcessorArchitecture arch;
        private ProgramImage img;
        private bool returnToCaller;
        private Identifier regIdxDetected;
        private static TraceSwitch trace = new TraceSwitch("IntelBackWalker", "Traces the progress of x86 backward instruction walking");

        public BackWalker2(IProcessorArchitecture arch, ProgramImage img)
        {
            this.arch = arch;
            this.img = img;
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

        public static bool IsEvenPowerOfTwo(int n)
        {
            return n != 0 && (n & (n - 1)) == 0;
        }

        private MachineRegister HandleAddition(
            MachineRegister regIdx,
            List<BackwalkOperation> operations,
            RegisterOperand ropDst,
            RegisterOperand ropSrc,
            ImmediateOperand immSrc,
            bool add)
        {
            if (ropSrc != null)
            {
                if (ropSrc.Register == ropDst.Register && add)
                {
                    operations.Add(new BackwalkOperation(BackwalkOperator.mul, 2));
                    return regIdx;
                }
                else
                {
                    return null;
                }
            }
            else if (immSrc != null)
            {
                operations.Add(new BackwalkOperation(
                    add ? BackwalkOperator.add : BackwalkOperator.sub,
                    immSrc.Value.ToInt32()));
                return regIdx;
            }
            else
                return MachineRegister.None;
        }

        public Identifier BackwalkInstructions(
            Identifier regIdx,
            StatementList instrs,
            int i,
            List<BackwalkOperation> operations)
        {
            var bw = new Visitor(regIdx, operations);
            for (; i >= 0; --i)
            {
                Instruction instr = instrs[i].Instruction;
                instr.Accept(bw);
                if (bw.IndexRegister == null)
                    break;
            }
            return bw.IndexRegister;
#if OLD
            switch (instr.code)
            {
            case Opcode.add:
            case Opcode.sub:
                if (ropDst != null && ropDst.Register == regIdx)
                {
                    regIdx = HandleAddition(regIdx, operations, ropDst, ropSrc, immSrc, instr.code == Opcode.add);
                }
                break;
            case Opcode.adc:
                if (ropDst != null && ropDst.Register == regIdx)
                {
                    return MachineRegister.None;
                }
                break;
            case Opcode.and:
                if (ropDst != null && ropDst.Register == regIdx && immSrc != null)
                {
                    if (IsEvenPowerOfTwo(immSrc.Value.ToInt32() + 1))
                    {
                        operations.Add(new BackwalkOperation(BackwalkOperator.cmp, immSrc.Value.ToInt32() + 1));
                        returnToCaller = true;
                    }
                    else
                    {
                        regIdx = null;
                    }
                    return regIdx;
                }
                break;
            case Opcode.inc:
            case Opcode.dec:
                if (ropDst != null && ropDst.Register == regIdx)
                {
                    BackwalkOperator op = instr.code == Opcode.inc ? BackwalkOperator.add : BackwalkOperator.sub;
                    operations.Add(new BackwalkOperation(op, 1));
                }
                break;
            case Opcode.cmp:
                if (ropDst != null &&
                    (ropDst.Register == regIdx || ropDst.Register == regIdx.GetPart(PrimitiveType.Byte)))
                {
                    if (immSrc != null)
                    {
                        operations.Add(new BackwalkOperation(BackwalkOperator.cmp, immSrc.Value.ToInt32()));
                        return regIdx;
                    }
                }
                break;
            case Opcode.ja:
                operations.Add(new BackwalkBranch(ConditionCode.UGT));
                break;
            case Opcode.mov:
                if (ropDst != null)
                {
                    if (ropDst.Register == regIdx)
                    {
                        if (ropSrc != null)
                            regIdx = ropSrc.Register;
                        else
                            regIdx = MachineRegister.None;	// haven't seen an immediate compare yet.
                    }
                    else if (ropDst.Register == regIdx.GetSubregister(0, 8) &&
                        memSrc != null && memSrc.Offset != null &&
                        memSrc.Base != MachineRegister.None)
                    {
                        operations.Add(new BackwalkDereference(memSrc.Offset.ToInt32(), memSrc.Scale));
                        regIdx = memSrc.Base;
                    }
                }
                break;
            case Opcode.movzx:
                if (ropDst != null && ropDst.Register == regIdx)
                {
                    if (memSrc != null)
                    {
                        return MachineRegister.None;
                    }
                }
                break;
            case Opcode.pop:
                if (ropDst != null && ropDst.Register == regIdx)
                {
                    return MachineRegister.None;
                }
                break;
            case Opcode.push:
                break;
            case Opcode.shl:
                if (ropDst != null && ropDst.Register == regIdx)
                {
                    if (immSrc != null)
                    {
                        operations.Add(new BackwalkOperation(BackwalkOperator.mul, 1 << immSrc.Value.ToInt32()));
                    }
                    else
                        return MachineRegister.None;
                }
                break;
            case Opcode.test:
                return MachineRegister.None;
            case Opcode.xchg:
                if (ropDst != null && ropSrc != null)
                {
                    if (ropDst.Register == regIdx ||
                        ropSrc.Register == regIdx)
                    {
                        regIdx = ropDst.Register;
                    }
                }
                else
                {
                    regIdx = MachineRegister.None;
                }
                break;
            case Opcode.xor:
                if (ropDst != null && ropSrc != null &&
                    ropSrc.Register == ropDst.Register &&
                    ropDst.Register == regIdx.GetSubregister(8, 8))
                {
                    operations.Add(new BackwalkOperation(BackwalkOperator.and, 0xFF));
                    regIdx = regIdx.GetSubregister(0, 8);
                }
                break;
            default:
                System.Diagnostics.Debug.WriteLine("Backwalking not supported: " + instr.code);
                DumpInstructions(instrs, i);
                break;
            }
#endif
            return regIdx;
        }


        private class Visitor : InstructionVisitorBase
        {
            Identifier regIdx;
            List<BackwalkOperation> operations;
            public Visitor(Identifier regIdx, List<BackwalkOperation> operations)
            {
                this.regIdx = regIdx;
                this.operations = operations;
            }

            public Identifier IndexRegister { get { return regIdx; } }
        }



        public Identifier FindIndexRegister(MemoryAccess mem)
        {
            throw new NotImplementedException();
            //if (mem.Base != MachineRegister.None)
            //{
            //    if (mem.Index != MachineRegister.None)
            //    {
            //        return MachineRegister.None; // Address expression too complex. //$REVIEW: Emit warning and barf.
            //    }
            //    return mem.Base;
            //}
            //else 
            //    return mem.Index;
        }

        public List<BackwalkOperation> Backwalk(Address addrCallJump, Block block)
        {
            var operations = new List<BackwalkOperation>();
            int i = FindLastCallJump(addrCallJump, block);
            if (i < 0)
                throw new InvalidOperationException(string.Format("Expected an indirect call or jump at address {0}.", addrCallJump));
            var mem = GetIndirectTarget(block.Statements[i].Instruction);
            if (mem == null)
                throw new InvalidOperationException(string.Format("Expected an indirect call or jump at address {0}.", addrCallJump));
            var regIdx = FindIndexRegister(mem);
            if (regIdx == null)
                return operations;

            // Record operations done to the IDX regiister.
            int scale = FindIndexRegisterScale(mem);
            if (scale > 1)
                operations.Add(new BackwalkOperation(BackwalkOperator.mul, scale));

            returnToCaller = false;
            regIdx = BackwalkInstructions(regIdx, block.Statements, i, operations);
            if (regIdx == null)
                return null;

            if (!returnToCaller)
            {
                Block pred = GetSinglePredcessesor(block);
                if (pred == null)
                {
                    return null;	// seems unguarded to me.	//$REVIEW: emit warning.
                }

                regIdx = BackwalkInstructions(regIdx, pred.Statements, pred.Statements.Count - 1, operations);
                if (regIdx == null)
                {
                    return null;
                }
            }
            operations.Reverse();
            regIdxDetected = regIdx;
            return operations;
        }

        private Block GetSinglePredcessesor(Block block)
        {
            throw new NotImplementedException();
        }

        private Identifier FindIndexRegister(object mem)
        {
            throw new NotImplementedException();
        }

        private int FindIndexRegisterScale(object mem)
        {
            throw new NotImplementedException();
        }

        private MemoryAccess GetIndirectTarget(Instruction instruction)
        {
            throw new NotImplementedException();
        }

        private int FindLastCallJump(Address addrCallJump, Block block)
        {
            throw new NotImplementedException();
        }

        public Identifier IndexRegister
        {
            get { return regIdxDetected; }
        }

        public Address MakeAddress(PrimitiveType size, ImageReader rdr, ushort segBase)
        {
            if (arch.WordWidth == PrimitiveType.Word16)
            {
                if (size == PrimitiveType.Word16)
                {
                    return new Address(segBase, rdr.ReadLeUint16());
                }
                else
                {
                    ushort off = rdr.ReadLeUint16();
                    ushort seg = rdr.ReadLeUint16();
                    return new Address(seg, off);
                }
            }
            else if (arch.WordWidth == PrimitiveType.Word32)
            {
                Debug.Assert(segBase == 0);
                return new Address(rdr.ReadLeUint32());
            }
            else
                throw new ApplicationException("Unexpected word width: " + size);
        }
    }
}