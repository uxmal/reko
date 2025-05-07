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

using System;
using Reko.Arch.Motorola.M68k.Machine;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Motorola.M68k
{
    internal class M68kInstructionComparer : InstructionComparer
    {
        private const int MemCode = 0x10;
        private const int IndIdxCode = 0x11;
        private const int PreDecCode = 0x12;
        private const int PostIncCode = 0x13;
        private const int RegSetCode = 0x14;
        private const int IndexedCode = 0x15;
        private const int BitfieldCode = 0x16;
        private const int DoubleCode = 0x17;

        public M68kInstructionComparer(Normalize norm) : base(norm)
        {
        }

        public override bool DoCompareOperands(MachineOperand opA, MachineOperand opB)
        {
            switch (opA)
            {
            case RegisterStorage regA:
                return CompareRegisters(regA, (RegisterStorage) opB);
            case Constant immA:
                var immB = (Constant) opB;
                return CompareConstants(immA, immB);
            case PredecrementMemoryOperand preA:
                var preB = (PredecrementMemoryOperand) opB;
                return CompareRegisters(preA.Register, preB.Register);
            case PostIncrementMemoryOperand postA:
                var postB = (PostIncrementMemoryOperand) opB;
                return CompareRegisters(postA.Register, postB.Register);
            case RegisterSetOperand regsetA:
                var regsetB = (RegisterSetOperand) opB;
                return NormalizeRegisters || regsetA.BitSet == regsetB.BitSet;
            case DoubleRegisterOperand dA:
                return CompareDoubleRegisters(dA, (DoubleRegisterOperand) opB);
            case MemoryOperand memA:
                var memB = (MemoryOperand) opB;
                if (!NormalizeRegisters && !CompareRegisters(memA.Base, memB.Base))
                    return false;
                return NormalizeConstants || CompareConstants(memA.Offset, memB.Offset);
            case Address addrA:
                var addrB = (Address) opB;
                return NormalizeConstants || addrA == addrB;
            case IndirectIndexedOperand idxA:
                var idxB = (IndirectIndexedOperand) opB;
                if (!NormalizeRegisters)
                {
                    if (!CompareRegisters(idxA.ARegister, idxB.ARegister))
                        return false;
                    if (!CompareRegisters(idxA.XRegister, idxB.XRegister))
                        return false;
                }
                if (!NormalizeConstants)
                {
                    if (idxA.Imm8 != idxB.Imm8)
                        return false;
                    if (idxA.Scale != idxB.Scale)
                        return false;
                }
                return true;
            case IndexedOperand ixA:
                return CompareIndexedOperands(ixA, (IndexedOperand) opB);
            case BitfieldOperand bfA:
                return CompareBitFields(bfA, (BitfieldOperand) opB);
            }
            throw new NotImplementedException(opA.GetType().FullName);
        }

        private bool CompareDoubleRegisters(DoubleRegisterOperand dA, DoubleRegisterOperand dB)
        {
            return 
                CompareRegisters(dA.Register1, dB.Register1) &&
                CompareRegisters(dA.Register2, dB.Register2);
        }

        private bool CompareIndexedOperands(IndexedOperand ixA, IndexedOperand ixB)
        {
            if (!CompareConstants(ixA.BaseDisplacement, ixB.BaseDisplacement))
                return false;
            if (!CompareConstants(ixA.OuterDisplacement, ixB.OuterDisplacement))
                return false;
            if (!CompareRegisters(ixA.Base, ixB.Base))
                return false;
            if (!CompareRegisters(ixA.Index, ixB.Index))
                return false;
            if (ixA.IndexScale != ixB.IndexScale)
                return false;
            return 
                ixA.preindex == ixB.preindex &&
                ixA.postindex == ixB.postindex;
        }

        private bool CompareBitFields(BitfieldOperand bfA, BitfieldOperand bfB)
        {
            return
                CompareOperands(bfA.BitOffset, bfB.BitOffset) &&
                CompareOperands(bfA.BitWidth, bfB.BitWidth);
        }

        public override int GetOperandHash(MachineOperand op)
        {
            switch (op)
            {
            case RegisterStorage rop: return GetRegisterHash(rop);
            case Constant immop: return GetConstantHash(immop);
            case Address addrOp: return GetAddressHash(addrOp);
            case DoubleRegisterOperand dop: return GetDoubleHash(dop);
            case MemoryOperand memOp: return GetMemoryHash(memOp);
            case IndirectIndexedOperand ind: return GetIndirectIndexedHash(ind);
            case PredecrementMemoryOperand pre: return GetPredecrementHash(pre);
            case PostIncrementMemoryOperand post: return GetPostincrementhash(post);
            case RegisterSetOperand regset: return GetRegisterSetHash(regset);
            case IndexedOperand indexOp: return GetIndexedHash(indexOp);
            case BitfieldOperand bfop: return GetBitfieldHash(bfop);
                }
            throw new NotImplementedException(op.GetType().FullName);
                }

        private int GetDoubleHash(DoubleRegisterOperand dop)
                {
            int hash = DoubleCode;
            hash = hash * 7 ^ GetRegisterHash(dop.Register1);
            hash = hash * 11 ^ GetRegisterHash(dop.Register2);
            return hash;
            throw new NotImplementedException();
                }

        private int GetBitfieldHash(BitfieldOperand bfop)
                {
            int h = BitfieldCode;
            h = h * 17 ^ GetOperandHash(bfop.BitOffset);
            h = h * 19 ^ GetOperandHash(bfop.BitWidth);
                return h;
                }

        private int GetIndexedHash(IndexedOperand indexOp)
                {
            int h = IndexedCode;
                if (!NormalizeRegisters)
                {
                    if (indexOp.Base is not null)
                    {
                        h = h * 7 ^ GetRegisterHash(indexOp.Base);
                    }
                    if (indexOp.Index is not null)
                    {
                        h = h * 11 ^ GetRegisterHash(indexOp.Index);
                    h = h * 13 ^ indexOp.index_reg_width?.BitSize ?? 0;
                    }
                }
                if (!NormalizeConstants)
                {
                    if (indexOp.BaseDisplacement is not null)
                    {
                    h = h * 17 ^ GetConstantHash(indexOp.BaseDisplacement);
                    }
                    if (indexOp.IndexScale != 0)
                    {
                        h = h * 19 ^ indexOp.IndexScale;
                    }
                }
                return h;
            }

        private int GetRegisterSetHash(RegisterSetOperand regset)
                {
            int h = RegSetCode;
            h = h * 17 ^ (NormalizeRegisters? 0 : (int)regset.BitSet);
            return h;
        }

        private int GetPostincrementhash(PostIncrementMemoryOperand post)
        {
            int h = PostIncCode;
            h = h * 7 ^ GetRegisterHash(post.Register);
                return h;
    }

        private int GetPredecrementHash(PredecrementMemoryOperand pre)
                {
            int h = PreDecCode;
            h = h * 7 ^ GetRegisterHash(pre.Register);
                return h;
                }

        private int GetIndirectIndexedHash(IndirectIndexedOperand ind)
                {
            int h = IndIdxCode;
            if (!NormalizeConstants)
                    {
                h = ind.Imm8.GetHashCode();
                h = h * 11 ^ ind.Scale.GetHashCode();
                h = h * 13 ^ ind.Imm8.GetHashCode();
                    }
            if (!NormalizeRegisters)
                    {
                h = h * 5 ^ ind.ARegister.GetHashCode();
                h = h * 17 ^ ind.XRegister.GetHashCode();
                    }
            return h;
                }

        private int GetMemoryHash(MemoryOperand memOp)
                {
            int h = MemCode;
            if (!NormalizeConstants && memOp.Offset is not null)
                    {
                h = memOp.Offset.GetHashCode();
                    }
            if (!NormalizeRegisters)
                    {
                h = h * 9 ^ memOp.Base.GetHashCode();
                    }
                return h;
            }
        }
}