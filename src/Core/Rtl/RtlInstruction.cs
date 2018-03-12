#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core.NativeInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.Core.Rtl
{
    /// <summary>
    /// RtlInstructions are the low-level register-transfer instructions
    /// emitted by the Instruction rewriters. They exist briefly while 
    /// the binary program is being scanned, and are then converted to
    /// IL code (see Reko.Core.Code).
    /// </summary>
    public abstract class RtlInstruction
    {
        /// <summary>
        /// The RtlClass of this instruction.
        /// </summary>
        public virtual RtlClass Class { get; internal set; }

        /// <summary>
        /// If true, the next statement needs a label. This is required in cases where the original machine code 
        /// maps to many RtlInstructions, some of which are branches (see the X86 REP instruction for a particularly
        /// hideous example).
        /// </summary>
        public bool NextStatementRequiresLabel { get; set; }
        
        public abstract T Accept<T>(RtlInstructionVisitor<T> visitor);

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            Write(sw);
            return sw.ToString();
        }

        public void Write(TextWriter writer)
        {
            WriteInner(writer);
        }

        protected abstract void WriteInner(TextWriter writer);

        public static string FormatClass(RtlClass rtlClass)
        {
            var sb = new StringBuilder();
            switch (rtlClass & (RtlClass.Transfer|RtlClass.Linear|RtlClass.Terminates|RtlClass.System))
            {
            case RtlClass.Linear: sb.Append('L'); break;
            case RtlClass.Transfer: sb.Append('T'); break;
            case RtlClass.Terminates: sb.Append('H'); break;
            case RtlClass.System: sb.Append('S'); break;
            default: sb.Append('-'); break;
            }
            sb.Append((rtlClass & RtlClass.Delay) != 0 ? 'D' : '-');
            sb.Append((rtlClass & RtlClass.Annul) != 0 ? 'A' : '-');
            return sb.ToString();
        }
    }

    /// <summary>
    /// Classifies an RTL instruction based on certain architectural features involving
    /// delay slots on architectures like MIPS and SPARC.
    /// </summary>
    [Flags]
    [NativeInterop]
    public enum RtlClass
    {
        None,
        Linear = 1,         // Non-transfer instruction, e.g. ALU operation.
        Transfer = 2,       // Transfer instruction.
        Conditional = 4,    // Instruction is gated on a condition.
        Call = 8,           // Instruction saves its continuation.
        Delay = 16,         // Next instruction is in the delay slot and may be executed.
        Annul = 32,         // Next instruction is annulled (see SPARC architecture)
        Terminates = 64,    // Instruction terminates execution (e.g. x86 and ARM HLT)
        System = 128,       // Privileged instruction
        Invalid = 256,      // Invalid instruction
        ConditionalTransfer = Conditional | Transfer,
    }

    /// <summary>
    /// A RtlInstructionCluster contains the RtlInstructions that are generated when 
    /// a machine instruction is rewritten.
    /// </summary>
    public class RtlInstructionCluster
    {
        public RtlInstructionCluster(Address addr, int instrLength, params RtlInstruction [] instrs)
        {
            this.Address = addr;
            this.Length = (byte)instrLength;
            this.Instructions = instrs;
        }

        /// <summary>
        /// The address of the original machine instruction.
        /// </summary>
        public Address Address { get; private set; }

        public RtlClass Class { get; set; }

        public RtlInstruction[] Instructions { get; private set; }

        /// <summary>
        /// The length of the original machine instruction, in bytes.
        /// </summary>
        public byte Length { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1}): {2} instructions", Address, Length, Instructions.Length);
        }

        public void Write(TextWriter writer)
        {
            writer.WriteLine("{0}({1}):", Address, Length);
            foreach (var ri in Instructions)
            {
                ri.Write(writer);
                writer.WriteLine();
            }
        }
    }
}
