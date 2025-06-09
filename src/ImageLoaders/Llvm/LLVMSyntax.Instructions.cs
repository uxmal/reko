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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Output;

#nullable disable

namespace Reko.ImageLoaders.LLVM
{
    public abstract class Instruction : LLVMSyntax
    {
        public abstract T Accept<T>(InstructionVisitor<T> visitor);
    }

    public abstract class Terminator : Instruction
    {
    }

    public abstract class MemoryInstruction : Instruction
    {
        public LocalId Result;
    }

    public abstract class OtherInstruction : Instruction
    {
        public LocalId Result;
    }


    public class Binary : Instruction
    {
        public LocalId Result;
        public TokenType Operator;
        public bool NoUnsignedWrap;
        public bool NoSignedWrap;
        public LLVMType Type;
        public Value Left;
        public Value Right;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitBinary(this);
        }

        public override void Write(Formatter w)
        {
            Result.Write(w);
            w.Write(" = ");
            w.Write(Operator.ToString());
            w.Write(" ");
            if (NoUnsignedWrap)
            {
                w.WriteKeyword("nuw");
                w.Write(' ');
            }
            if (NoSignedWrap)
            {
                w.WriteKeyword("nsw");
                w.Write(' ');
            }
            Type.Write(w);
            w.Write(" ");
            Left.Write(w);
            w.Write(", ");
            Right.Write(w);
        }
    }

    public class BitwiseBinary : Instruction
    {
        public LocalId Result;
        public TokenType Operator;
        public bool NoUnsignedWrap;
        public bool NoSignedWrap;
        public LLVMType Type;
        public Value Left;
        public Value Right;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitBitwiseBinary(this);
        }

        public override void Write(Formatter w)
        {
            Result.Write(w);
            w.Write(" = ");
            w.Write(Operator.ToString());
            w.Write(" ");
            if (NoUnsignedWrap)
            {
                w.WriteKeyword("nuw");
                w.Write(' ');
            }
            if (NoSignedWrap)
            {
                w.WriteKeyword("nsw");
                w.Write(' ');
            }
            Type.Write(w);
            w.Write(" ");
            Left.Write(w);
            w.Write(", ");
            Right.Write(w);
        }
    }

    public class BrInstr : Terminator
    {
        public LLVMType Type;
        public Value Cond;
        public LocalId IfTrue;
        public LocalId IfFalse;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitBr(this);
        }

        public override void Write(Formatter w)
        {
            w.WriteKeyword("br");
            w.Write(' ');
            if (Cond is not null)
            {
                Type.Write(w);
                w.Write(' ');
                Cond.Write(w);
                w.Write(", ");
                w.WriteKeyword("label");
                w.Write(' ');
                IfTrue.Write(w);
                w.Write(", ");
                w.WriteKeyword("label");
                w.Write(' ');
                IfFalse.Write(w);
            }
            else
            {
                w.WriteKeyword("label");
                w.Write(' ');
                IfTrue.Write(w);
            }
        }
    }

    public class PhiInstruction : OtherInstruction
    {
        public LLVMType Type;
        public List<(Value, LocalId)> Arguments;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitPhi(this);
        }

        public override void Write(Formatter w)
        {
            Result.Write(w);
            w.Write(" = ");
            w.WriteKeyword("phi");
            w.Write(' ');
            Type.Write(w);
            var sep = " ";
            foreach (var arg in Arguments)
            {
                w.Write(sep);
                sep = ", ";
                w.Write("[");
                w.Write(arg.Item1);
                w.Write(", ");
                w.Write(arg.Item2);
                w.Write("]");
            }
        }
    }

    public class RetInstr : Terminator
    {
        public LLVMType Type;
        public Value Value;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitRet(this);
        }

        public override void Write(Formatter w)
        {
            w.WriteKeyword("ret");
            w.Write(" ");
            if (this.Type == LLVMType.Void)
            {
                w.WriteKeyword("void");
            }
            else
            {
                Type.Write(w);
                w.Write(' ');
                Value.Write(w);
            }
        }
    }

    public class Alloca : MemoryInstruction
    {
        public LLVMType Type;
        public LLVMType ElCountType;
        public Value ElementCount;
        public int Alignment;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitAlloca(this);
        }

        public override void Write(Formatter w)
        {
            Result.Write(w);
            w.Write(" = ");
            w.WriteKeyword("alloca");
            w.Write(' ');
            Type.Write(w);
            if (ElCountType is not null && ElementCount is not null)
            {
                w.Write(", ");
                Type.Write(w);
                w.Write(' ');
                ElementCount.Write(w);
            }
            if (Alignment != 0)
            {
                w.Write(", ");
                w.WriteKeyword("align");
                w.Write(Alignment);
            }
        }
    }

    public class Extractvalue : MemoryInstruction
    {
        public LLVMType AggregateType;
        public Value Value;
        public List<int> Indices;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitExtractvalue(this);
        }

        public override void Write(Formatter w)
        {
            Result.Write(w);
            w.Write(" = ");
            w.WriteKeyword("extractvalue");
            w.Write(" ");
            AggregateType.Write(w);
            w.Write(' ');
            Value.Write(w);
            foreach (var idx in Indices)
            {
                w.Write(", ");
                w.Write(idx);
            }
        }
    }

    public class GetElementPtr : MemoryInstruction
    {
        public List<(LLVMType, Value)> Indices;

        public LLVMType BaseType;
        public LLVMType PtrType;
        public Value PtrValue;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitGetelementptr(this);
        }

        public override void Write(Formatter w)
        {
            Result.Write(w);
            w.Write(" = ");
            w.WriteKeyword("getelementptr");
            w.Write(" ");
            BaseType.Write(w);
            w.Write(", ");
            PtrType.Write(w);
            w.Write(" ");
            PtrValue.Write(w);
            foreach (var index in Indices)
            {
                w.Write(", ");
                index.Item1.Write(w);
                w.Write(" ");
                index.Item2.Write(w);
            }
        }
    }

    public class CmpInstruction : OtherInstruction
    {
        public TokenType Operator;
        public TokenType ConditionCode;
        public LLVMType Type;
        public Value Op1;
        public Value Op2;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitCmp(this);
        }

        public override void Write(Formatter w)
        {
            Result.Write(w);
            w.Write(" = ");
            w.WriteKeyword(Operator.ToString());
            w.Write(' ');
            w.WriteKeyword(ConditionCode.ToString());
            w.Write(' ');
            Type.Write(w);
            w.Write(' ');
            Op1.Write(w);
            w.Write(", ");
            Op2.Write(w);
        }
    }

    public class LLVMCall : OtherInstruction
    {
        public List<Argument> Arguments;
        public ParameterAttributes res_attrs;
        public LLVMType FnType;
        public Value FnPtr;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitCall(this);
        }

        public override void Write(Formatter w)
        {
            if (Result is not null)
            {
                Result.Write(w);
                w.Write(" = ");
            }
            w.WriteKeyword("call");
            w.Write(" ");
            if (res_attrs is not null)
            {
                res_attrs.Write(w);
                w.Write(' ');
            }
            FnType.Write(w);
            w.Write(" ");
            FnPtr.Write(w);
            w.Write("(");
            var sep = "";
            foreach (var arg in Arguments)
            {
                w.Write(sep);
                sep = ", ";
                arg.Type.Write(w);
                w.Write(" ");
                if (arg.Attributes is not null)
                {
                    arg.Attributes.Write(w);
                    w.Write(" ");
                }
                arg.Value.Write(w);
            }
            w.Write(")");
        }
    }

    public class Argument
    {
        public LLVMType Type;
        public Value Value;
        public ParameterAttributes Attributes;
    }

    public class Fence : MemoryInstruction
    {
        public TokenType Type;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitFence(this);
        }

        public override void Write(Formatter w)
        {
            w.WriteKeyword("fence");
            w.Write(' ');
            w.WriteKeyword(Type.ToString());
        }
    }

    public class Load : MemoryInstruction
    {
        public LLVMType DstType;
        public Value Dst;
        public LLVMType SrcType;
        public Value Src;
        public int Alignment;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitLoad(this);
        }

        public override void Write(Formatter w)
        {
            Dst.Write(w);
            w.Write(" = ");
            w.WriteKeyword("load");
            w.Write(' ');
            DstType.Write(w);
            w.Write(", ");
            SrcType.Write(w);
            w.Write(' ');
            Src.Write(w);
            if (Alignment != 0)
            {
                w.Write(", ");
                w.WriteKeyword("align");
                w.Write(" {0}", Alignment);
            }
        }
    }

    public class Conversion : OtherInstruction
    {
        public TokenType Operator;
        public LLVMType TypeFrom;
        public Value Value;
        public LLVMType TypeTo;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitConversion(this);
        }

        public override void Write(Formatter w)
        {
            Result.Write(w);
            w.Write(" = ");
            w.WriteKeyword(Operator.ToString());
            w.Write(' ');
            TypeFrom.Write(w);
            w.Write(' ');
            Value.Write(w);
            w.Write(' ');
            w.WriteKeyword("to");
            w.Write(' ');
            TypeTo.Write(w);
        }
    }

    public class Select : OtherInstruction
    {
        public LLVMType CondType;
        public Value Cond;
        public LLVMType TrueType;
        public Value TrueValue;
        public LLVMType FalseType;
        public Value FalseValue;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitSelect(this);
        }

        public override void Write(Formatter w)
        {
            Result.Write(w);
            w.Write(" = ");
            w.WriteKeyword("select");
            w.Write(" ");
            CondType.Write(w);
            w.Write(" ");
            Cond.Write(w);
            w.Write(", ");
            TrueType.Write(w);
            w.Write(" ");
            TrueValue.Write(w);
            w.Write(", ");
            FalseType.Write(w);
            w.Write(" ");
            FalseValue.Write(w);
        }
    }

    public class Store : MemoryInstruction
    {
        public bool Volatile;
        public int Alignment;
        public Value Dst;
        public LLVMType DstType;
        public Value Src;
        public LLVMType SrcType;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitStore(this);
        }

        public override void Write(Formatter w)
        {
            w.WriteKeyword("store");
            w.Write(' ');
            SrcType.Write(w);
            w.Write(' ');
            Src.Write(w);
            w.Write(", ");
            DstType.Write(w);
            w.Write(' ');
            Dst.Write(w);
            if (Alignment != 0)
            {
                w.Write(", ");
                w.WriteKeyword("align");
                w.Write(' ');
                w.Write(Alignment);
            }
        }
    }

    public class Switch : Terminator
    {
        public LLVMType Type;
        public Value Value;
        public LocalId Default;
        public List<(LLVMType, Value, LocalId)> Destinations;

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitSwitch(this);
        }

        public override void Write(Formatter w)
        {
            w.WriteKeyword("switch");
            w.Write(' ');
            Type.Write(w);
            w.Write(' ');
            Value.Write(w);
            w.Write(", ");
            w.WriteKeyword("label");
            w.Write(' ');
            Default.Write(w);
            w.WriteLine(" [");
            foreach (var dest in Destinations)
            {
                w.Indent();
                dest.Item1.Write(w);
                w.Write(' ');
                dest.Item2.Write(w);
                w.Write(", ");
                w.WriteKeyword("label");
                w.Write(' ');
                dest.Item3.Write(w);
                w.WriteLine();
            }
            w.Write("]");
        }
    }

    public class Unreachable : Terminator
    {
        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitUnreachable(this);
        }

        public override void Write(Formatter w)
        {
            w.WriteKeyword("unreachable");
        }
    }

}
