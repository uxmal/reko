#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using Decompiler.Core.Machine;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public class InstructionFormatter : M68kOperandVisitor<M68kOperand>
    {
        private StringWriter writer;

        public InstructionFormatter(StringBuilder sb)
        {
            writer = new StringWriter(sb);
        }

        public void Write(M68kInstruction instr)
        {
            writer.Write(instr.code);
            if (instr.dataWidth != null)
            {
                writer.Write(DataSizeSuffix(instr.dataWidth));
            }
            writer.Write('\t');
            if (instr.op1 != null)
            {
                WriteOperand(instr.op1);
                if (instr.op2 != null)
                {
                    writer.Write(',');
                    WriteOperand(instr.op2);
                }
            }
        }

        private string DataSizeSuffix(PrimitiveType dataWidth)
        {
            switch (dataWidth.BitSize)
            {
                case 8: return ".b";
                case 16: return ".w";
                case 32: return ".l";
                default: throw new InvalidOperationException(string.Format("Unsupported data width {0}.", dataWidth.BitSize));
            }
        }

        private void WriteOperand(MachineOperand op)
        {
            RegisterOperand reg = op as RegisterOperand;
            if (reg != null)
            {
                writer.Write(reg.Register.Name);
                return;
            }
            ImmediateOperand imm = op as ImmediateOperand;
            if (imm != null)
            {
                writer.Write("#$");
                writer.Write(MachineOperand.FormatValue(imm.Value));
                return;
            }
            MemoryOperand mop = op as MemoryOperand;
            if (mop != null)
            {
                if (mop.Offset != null)
                {
                    writer.Write('$');
                    writer.Write(mop.Offset.IsNegative
                        ? MachineOperand.FormatSignedValue(mop.Offset)
                        : MachineOperand.FormatUnsignedValue(mop.Offset));
                }
                writer.Write("(");
                writer.Write(mop.Base.Name);
                writer.Write(")");
                return;
            }
            M68kOperand m68kop = op as M68kOperand;
            m68kop.Accept<M68kOperand>(this);
        }

        public M68kOperand Visit(M68kImmediateOperand imm)
        {
            writer.Write("#$");
            writer.Write(MachineOperand.FormatValue(imm.Constant));
            return imm;
        }

        public M68kOperand Visit(PredecrementMemoryOperand pre)
        {
            writer.Write("-(");
            writer.Write(pre.Register.Name);
            writer.Write(")");
            return pre;
        }

        public M68kOperand Visit(PostIncrementMemoryOperand post)
        {
            writer.Write("(");
            writer.Write(post.Register.Name);
            writer.Write(")+");
            return post;
        }

        public M68kOperand Visit(AddressOperand addr)
        {
            writer.Write("$");
            writer.Write("{0:X8}", addr.Address.Offset);
            return addr;
        }

        public M68kOperand Visit(RegisterSetOperand registerSet)
        {
            uint bitSet = registerSet.BitSet;
            WriteRegisterSet(bitSet, 15, -1, "d", writer);
            WriteRegisterSet(bitSet, 7, -1, "a", writer);
            return registerSet;
        }

        public M68kOperand Visit(IndirectIndexedOperand op)
        {
            writer.Write("(");
            if (op.Imm8 < 0)
            {
                writer.Write("-{0:X2},", -op.Imm8);
            }
            else if (op.Imm8 > 0)
            {
                writer.Write("{0:X2},", op.Imm8);
            }
            writer.Write(op.ARegister.Name);
            writer.Write(",");
            writer.Write(op.XRegister.Name);
            if (op.Scale > 1)
                writer.Write("*{0}", op.Scale);
            writer.Write(")");
            return op;
        }
        private static bool bit(uint data, int pos) { return (data & (1 << pos)) != 0; }

        public void WriteRegisterSet(uint data, int bitPos, int incr, string regType, TextWriter writer)
        {
            string sep = "";
            for (int i = 0; i < 8; i++, bitPos += incr)
            {
                if (bit(data, bitPos))
                {
                    int first = i;
                    int run_length = 0;
                    while (i < 7 && bit(data, bitPos + incr))
                    {
                        bitPos += incr;
                        ++i;
                        ++run_length;
                    }
                    writer.Write(sep);
                    writer.Write("{0}{1}", regType, first);
                    if (run_length > 0)
                        writer.Write("-{0}{0}", regType, first + run_length);
                    sep = "/";
                }
            }
        }

        public M68kOperand Visit(IndexedOperand op)
        {
            //@base = EXT_BASE_DISPLACEMENT_PRESENT(extension) ? (EXT_BASE_DISPLACEMENT_LONG(extension) ? read_imm_32() : read_imm_16()) : 0;
            //outer = EXT_OUTER_DISPLACEMENT_PRESENT(extension) ? (EXT_OUTER_DISPLACEMENT_LONG(extension) ? read_imm_32() : read_imm_16()) : 0;
            //if (EXT_BASE_REGISTER_PRESENT(extension))
            //    base_reg = string.Format("A{0}", instruction & 7);
            //else
            //    base_reg = "";
            //if (EXT_INDEX_REGISTER_PRESENT(extension))
            //{
            //    index_reg = string.Format("{0}{1}.{2}", EXT_INDEX_AR(extension) ? 'A' : 'D', EXT_INDEX_REGISTER(extension), EXT_INDEX_LONG(extension) ? 'l' : 'w');
            //    if (EXT_INDEX_SCALE(extension) != 0)
            //        index_reg += string.Format("*{0}", 1 << EXT_INDEX_SCALE(extension));
            //}
            //else
            //    index_reg = "";
            //preindex = (extension & 7) > 0 && (extension & 7) < 4;
            //postindex = (extension & 7) > 4;

            var mode = writer;
            mode.Write("(");
            if (op.preindex || op.postindex)
                mode.Write("[");
            var sep = "";
            if (op.@base != null)
            {
                writer.Write(MachineOperand.FormatValue(op.@base));    
                sep = ",";
            }
            if (op.base_reg != null)
            {
                mode.Write(sep);
                mode.Write(op.base_reg);
                sep = ",";
            }
            if (op.postindex)
            {
                mode.Write("]");
                sep = ",";
            }
            if (op.index_reg != null)
            {
                mode.Write(sep);
                if (op.index_scale > 1)
                    mode.Write("*{0}", op.index_scale);
                sep = ",";
            }
            if (op.preindex)
            {
                mode.Write("]");
                sep = ",";
            }
            if (op.outer != null)
            {
                mode.Write(sep);
                mode.Write(MachineOperand.FormatSignedValue(op.outer));
            }
            mode.Write(")");
            return op;
        }
    }
}
