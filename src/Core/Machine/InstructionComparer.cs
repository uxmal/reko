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

using Reko.Core.Expressions;
using System;
using System.Collections.Generic;

namespace Reko.Core.Machine
{
    /// <summary>
    /// Compares instructions for equality.
    /// </summary>
    public abstract class InstructionComparer : IEqualityComparer<MachineInstruction>
    {
        private const int RegCode = 1;
        private const int ConstCode = 2;
        private const int AddrCode = 3;
        private const int SeqCode = 4;
        private const int FlagGroupCode = 5;
        private const int BitOperandCode = 6;
        private const int RegisterRangeCode = 7;

        private readonly Normalize norm;

        /// <summary>
        /// Constructs an instruction comparer.
        /// </summary>
        /// <param name="norm">Normalization to use.</param>
        public InstructionComparer(Normalize norm)
        {
            this.norm = norm;
        }

        /// <summary>
        /// Compares the operands of two instructions.
        /// </summary>
        /// <param name="x">First machine instruction.</param>
        /// <param name="y">Second machine instruction.</param>
        /// <returns>True if all operands compare equal; otherwise false.</returns>
        public virtual bool CompareOperands(MachineInstruction x, MachineInstruction y)
        {
            if (x.Operands.Length != y.Operands.Length)
                return false;
            for (int i = 0; i < x.Operands.Length; ++i)
            {
                var op1 = x.Operands[i];
                var op2 = y.Operands[i];
                if (!CompareOperands(op1, op2))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Compares two operands.
        /// </summary>
        /// <param name="op1">First operand.</param>
        /// <param name="op2">Second operand.</param>
        /// <returns>True if operands compare equal; otherwise false.</returns>
        public bool CompareOperands(MachineOperand? op1, MachineOperand? op2)
        {
            if (op1 is null)
                return op2 is null;
            if (op2 is null)
                return false;
            if (op1.GetType() != op2.GetType())
                return false;
            return DoCompareOperands(op1, op2);
        }

        /// <summary>
        /// Compares two operands of the same type.
        /// </summary>
        /// <param name="op1">First operand.</param>
        /// <param name="op2">Second operand.</param>
        /// <returns>True if operands compare equal; otherwise false.</returns>
        public abstract bool DoCompareOperands(MachineOperand op1, MachineOperand op2);

        /// <summary>
        /// Compares the two <see cref="RegisterStorage"/>s.
        /// </summary>
        /// <param name="regA">First register.</param>
        /// <param name="regB">Second register.</param>
        /// <returns>True if registers are equal; otherwise false.</returns>
        public bool CompareRegisters(RegisterStorage? regA, RegisterStorage? regB)
        {
            if (regA is null)
            {
                return regB is null;
            }
            if (regB is null)
            {
                return regA is null;
            }
            return NormalizeRegisters || regA == regB;
        }


        /// <summary>
        /// Compares the two <see cref="SequenceStorage"/>s.
        /// </summary>
        /// <param name="seqA">First sequence storage.</param>
        /// <param name="seqB">Second sequence storage.</param>
        /// <returns>True if the sequences are considered equal;
        /// otherwise false.</returns>
        public bool CompareSequences(SequenceStorage seqA, SequenceStorage seqB)
        {
            if (seqA.Elements.Length != seqB.Elements.Length)
                return false;
            if (NormalizeRegisters)
                return true;
            for (int i = 0; i < seqA.Elements.Length; ++i)
            {
                if (seqA.Elements[i] != seqB.Elements[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Compares the two <see cref="Storage"/>s for equality.
        /// </summary>
        /// <param name="storageA">First storage.</param>
        /// <param name="storageB">Second storage.</param>
        /// <returns>True if the storages compare to equal.</returns>
        public bool CompareStorages(Storage? storageA, Storage? storageB)
        {
            if (storageA is null)
                return storageB is null;
            if (storageB is null)
                return storageA is null;
            if (storageA.GetType() != storageB.GetType())
                return false;
            return storageA switch
            {
                RegisterStorage regA => CompareRegisters(regA, (RegisterStorage) storageB),
                SequenceStorage seqA => CompareSequences(seqA, (SequenceStorage) storageB),
                _ => throw new NotImplementedException($"Storage type {storageA.GetType().Name} not handled yet.")
            };
        }

        /// <summary>
        /// Compares the two <see cref="Address"/>es.
        /// </summary>
        /// <param name="addrA">First constant.</param>
        /// <param name="addrB">Second constant.</param>
        /// <returns>True if addresses are equal; otherwise false.</returns>
        public bool CompareAddresses(Address addrA, Address addrB)
        {
            return NormalizeConstants || addrA == addrB;
        }

        /// <summary>
        /// Compares two <see cref="BitOperand"/>s.
        /// </summary>
        /// <param name="opA">First bit operand.</param>
        /// <param name="opB">Second bit operand.</param>
        /// <returns>True if the operands are considered equal.
        /// </returns>
        public bool CompareBitOperands(BitOperand opA, BitOperand opB)
        {
            if (!CompareOperands(opA.Operand, opB.Operand))
                return false;
            return opA.BitPosition == opB.BitPosition;
        }

        /// <summary>
        /// Compares the two <see cref="Constant"/>s.
        /// </summary>
        /// <param name="constA">First constant.</param>
        /// <param name="constB">Second constant.</param>
        /// <returns>True if constants are equal; otherwise false.</returns>
        public bool CompareConstants(Constant? constA, Constant? constB)
        {
            if (constA is null)
            {
                return constB is null;
            }
            if (constB is null)
            {
                return constA is null;
            }
            return NormalizeConstants || constA.GetValue().Equals(constB.GetValue());
        }

        /// <summary>
        /// Compares the two integer values.
        /// </summary>
        /// <param name="constA">First integer.</param>
        /// <param name="constB">Second integers.</param>
        /// <returns>True if integers are equal; otherwise false.</returns>
        public bool CompareConstants(long constA, long constB)
        {
            return NormalizeConstants || constA == constB;
        }

        /// <summary>
        /// Compares two flag groups.
        /// </summary>
        /// <param name="grfA">First flag group.</param>
        /// <param name="grfB">Second flag group.</param>
        /// <returns>True if the flag groups are equal; otherwose false.
        /// </returns>
        public bool CompareFlagGroups(FlagGroupStorage grfA , FlagGroupStorage grfB)
        {
            return grfA.FlagRegister == grfB.FlagRegister &&
                grfA.FlagGroupBits == grfA.FlagGroupBits;
        }

        /// <summary>
        /// Compares two literal operands.
        /// </summary>
        /// <param name="litA">First literal operand.</param>
        /// <param name="litB">Second literal operands.</param>
        /// <returns>True if the literals are equal; otherwise false.
        /// </returns>
        public bool CompareLiterals(LiteralOperand litA, LiteralOperand litB)
        {
            return litA.Literal == litB.Literal;
        }

        /// <summary>
        /// Compare two <see cref="RegisterRange"/>s.
        /// </summary>
        /// <param name="rr1">First register range operand.</param>
        /// <param name="rr2">Second register range operand.</param>
        /// <returns>True if the operands are equal; otherwise false.</returns>
        public bool CompareRegisterRanges(RegisterRange rr1, RegisterRange rr2)
        {
            if (NormalizeRegisters)
                return true;
            if (rr1.RegisterIndex != rr2.RegisterIndex)
                return false;
            return rr1.Count == rr2.Count;
        }

        /// <inheritdoc/>
        public virtual bool Equals(MachineInstruction? x, MachineInstruction? y)
        {
            if (x is null)
                return y is null;
            if (y is null)
                return false;
            if (x.MnemonicAsInteger != y.MnemonicAsInteger)
                return false;
            return CompareOperands(x, y);
        }

        /// <summary>
        /// Computes a hash code for the given <see cref="Address"/>.
        /// </summary>
        /// <param name="addr">Address whose hash code is to be computed.</param>
        /// <returns>Hashcode of the address, normalized if necessary.</returns>
        public int GetAddressHash(Address addr)
        {
            int h = AddrCode;
            if ((norm & Normalize.Constants) != 0)
                return h;
            var lin = addr.ToLinear();
            return h ^ (int) (lin * 17 ^ (lin >> 32));
        }

        /// <summary>
        /// Computes a hash code for the given <see cref="BitOperand"/>.
        /// </summary>
        /// <param name="op">Bit operand whose hash code is to be computed.</param>
        /// <returns>The hash code of the bit operand, normalized if necessary.</returns>
        public int GetBitOperandHash(BitOperand op)
        {
            int h = BitOperandCode;
            h = h * 17 ^ GetOperandHash(op.Operand);
            h = h * 7 ^ op.BitPosition;
            return h;
        }
        /// <summary>
        /// Computes a hash code for the given <see cref="Constant"/>.
        /// </summary>
        /// <param name="c">Constant whose hash code is to be computed.</param>
        /// <returns>Hash code of the constant, normalized if necessary.</returns>
        public int GetConstantHash(Constant? c)
        {
            int h = ConstCode;
            if ((norm & Normalize.Constants) != 0 || c is null)
                return h;
            if (c.IsReal)
            {
                var r = c.ToDouble();
                var ul = BitConverter.DoubleToInt64Bits(r);
                return h * 0x31 ^ (int) (ul * 17 ^ (ul >> 32));
            }
            if (c.DataType.BitSize <= 64)
            {
                var ul = c.ToUInt64();
                return h * 0x31 ^ (int) (ul * 17 ^ (ul >> 32));
            }
            var big = c.ToBigInteger();
            while (!big.IsZero)
            {
                var i = (int) big;
                h = (h * 17) ^ i;
                big >>= 32;
            }
            return h;
        }

        /// <summary>
        /// Computes a hash code for the given integer.
        /// </summary>
        /// <param name="l">Constant whose hash code is to be computed.</param>
        /// <returns>Hash code of the constant, normalized if necessary.</returns>
        public int GetConstantHash(long l)
        {
            int h = ConstCode;
            if (NormalizeConstants)
                return h;
            var ul = (ulong) l;
            return h * 0x31 ^ (int)(ul * 17 ^ (ul >> 32));
        }

        /// <summary>
        /// Computes a hash code for the given <see cref="LiteralOperand"/>.
        /// </summary>
        /// <param name="lit">Literal operand whose hash is to be computed.</param>
        /// <returns>Hash code of the literal.</returns>
        public int GetLiteralHash(LiteralOperand lit)
        {
            int h = 0;
            foreach (var ch in lit.Literal)
            {
                h = h * 17 ^ (int) ch;
            }
            return h;
        }

        /// <summary>
        /// Computes a hash code for the given <see cref="RegisterStorage"/>.
        /// </summary>
        /// <param name="r">Register whose hash code is to be computed.</param>
        /// <returns>Hash code of the register, normalized if necessary.</returns>
        public int GetRegisterHash(RegisterStorage? r)
        {
            int h = RegCode;
            if ((norm & Normalize.Registers) != 0 || r is null)
                return h;
            return h * 31 ^ r.Number;
        }

        /// <summary>
        /// Computes a hash code for the given <see cref="RegisterRange"/>.
        /// </summary>
        /// <param name="rr">A <see cref="RegisterRange"/> whose hash code is to be computed.</param>
        /// <returns>The hash code of the register range, normalized if necessary.
        /// </returns>
        public int GetRegisterRangeHash(RegisterRange rr)
        {
            int h = RegisterRangeCode;
            if (NormalizeRegisters)
                return h;
            h = h * 17 ^ rr.RegisterIndex;
            h = h * 7 ^ rr.Count;
            return h;
        }

        /// <summary>
        /// Computes a hash code for the given <see cref="SequenceStorage"/>.
        /// </summary>
        /// <param name="s">A sequence whose hash code is to be computed.</param>
        /// <returns>Hash code of the sequence, normalized if necessary.</returns>
        public int GetSequenceHash(SequenceStorage? s)
        {
            if (s is null)
                return 0;
            int h = SeqCode;
            foreach (var op in s.Elements)
            {
                h = h * 17 ^ GetStorageHash(op);
            }
            return h;
        }

        /// <summary>
        /// Computes a hash code for the given <see cref="FlagGroupStorage"/>.
        /// </summary>
        /// <param name="grf">Flag group whose hash code is to be computed.</param>
        /// <returns>Hash code of the flag group.
        /// </returns>
        public int GetFlagGroupHash(FlagGroupStorage grf)
        {
            int hash = FlagGroupCode;
            hash = hash * 17 ^ grf.FlagRegister.Number;
            hash = hash * 7 ^ (int) grf.FlagGroupBits;
            return hash;
        }

        /// <summary>
        /// Computes the hash code for the given <see cref="Storage"/>.
        /// </summary>
        /// <param name="s">Storage whose hash is to be compited. </param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int GetStorageHash(Storage? s)
        {
            if (s is null)
                return 0;
            return s switch
            {
                RegisterStorage reg => GetRegisterHash(reg),
                SequenceStorage seq => GetSequenceHash(seq),
                FlagGroupStorage grf => GetFlagGroupHash(grf),
                _ => throw new NotImplementedException($"Storage type {s.GetType().Name} not handled yet.")
            };
        }

        /// <summary>
        /// Computes a hash code for the given instruction.
        /// </summary>
        /// <param name="instr">Instruction for which to compute
        /// the hash code.</param>
        /// <returns>A hash code.</returns>
        public int GetHashCode(MachineInstruction instr)
        {
            int h = instr.MnemonicAsInteger.GetHashCode();
            return h ^ GetOperandsHash(instr);
        }

        /// <summary>
        /// Computes a hash code for the operands of the given instruction.
        /// </summary>
        /// <param name="instr">Machine instruction whose operands will have their hash compited.</param>
        /// <returns>The hash code of the operands, respecting the <see cref="norm"/>.
        /// </returns>
        public virtual int GetOperandsHash(MachineInstruction instr)
        {
            int h = 0;
            foreach (var op in instr.Operands)
            {
                h = h * 17 ^ GetOperandHash(op);
            }
            return h;
        }

        /// <summary>
        /// Computes a hash code for the operands of the given instruction.
        /// </summary>
        /// <param name="op">Operand having its hash computed.</param>
        /// <returns>The hash code of the operand, respecting the <see cref="norm"/>.
        /// </returns>
        public abstract int GetOperandHash(MachineOperand op);

        /// <summary>
        /// If true, differences between constants will be ignored.
        /// </summary>
        public bool NormalizeConstants => (norm & Normalize.Constants) != 0;

        /// <summary>
        /// If true, register differences will be ignored.
        /// </summary>
        public bool NormalizeRegisters => (norm & Normalize.Registers) != 0;
    }
}
