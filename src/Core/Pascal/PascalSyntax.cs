#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System.Globalization;
using System.IO;

#nullable disable

namespace Reko.Core.Pascal
{
    public abstract class PascalSyntax
    {

        public abstract T Accept<T>(IPascalSyntaxVisitor<T> visitor);

        public sealed override string ToString()
        {
            var sw = new StringWriter();
            this.Write(sw);
            return sw.ToString();
        }

        public abstract void Write(TextWriter writer);

        public static void WriteList(TextWriter writer, string sep, IEnumerable<PascalSyntax> items)
        {
            WriteList(writer, sep, items, (w, i) => i.Write(w));
        }

        public static void WriteList<T>(TextWriter writer, string sep, IEnumerable<T> items, Action<TextWriter, T> wr)
        {
            var s = "";
            foreach (var item in items)
            {
                writer.Write(s);
                s = sep;
                wr(writer, item);
            }
        }

    }

    public abstract class Declaration : PascalSyntax
    {
        protected Declaration(string name)
        {
            this.Name = name;
        }

        public string Name;
    }

    public class ConstantDeclaration : Declaration
    {
        public ConstantDeclaration(string name, Exp exp, PascalType type = null) : base(name)
        {
            this.Exp = exp;
            this.Type = type;
        }

        public Exp Exp { get; }
        public PascalType Type { get; }


        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitConstantDeclaration(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("const");
            writer.Write(' ');
            writer.Write(Name);
            if (this.Type != null)
            {
                writer.Write(" : ");
                Type.Write(writer);
            }
            writer.Write(" = ");
            Exp.Write(writer);
        }
    }

    public class TypeDeclaration : Declaration
    {
        public PascalType Type;

        public TypeDeclaration(string name, PascalType type) : base(name) { this.Type = type; }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitTypeDeclaration(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("type");
            writer.Write(" ");
            writer.Write(Name);
            writer.Write(" = ");
            Type.Write(writer);
        }
    }

    /// <summary>
    /// A Callable is either a FUNCTION or PROCEDURE.
    /// </summary>
    public class CallableDeclaration : Declaration
    {
        public PascalType ReturnType;  // Null means no return value.
        public List<ParameterDeclaration> Parameters;

        public CallableDeclaration(string name, PascalType retType, List<ParameterDeclaration> parameters)
            : base(name)
        {
            this.ReturnType = retType;
            this.Parameters = parameters;
        }

        public Block Body { get; set; }
        public string CallingConvention { get; set; }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitCallableDeclaration(this);
        }

        public override void Write(TextWriter writer)
        {
            if (ReturnType != null)
            {
                writer.Write("function");
            }
            else
            {
                writer.Write("procedure");
            }
            writer.Write(" ");
            writer.Write(Name);
            if (Parameters.Count > 0)
            {
                writer.Write("(");
                WriteList(writer, "; ", Parameters, ParameterDeclaration.Write);
                writer.Write(")");
            }
            if (ReturnType != null)
            {
                writer.Write(" : ");
                ReturnType.Write(writer);
            }
            if (CallingConvention != null)
            {
                writer.Write("; {0}", CallingConvention);
            }
            if (Body != null)
            {
                writer.Write("; ");
                Body.Write(writer);
            }
        }
    }

    public abstract class Exp : PascalSyntax
    {
    }

    public class BooleanLiteral : Exp
    {
        public bool Value;

        public BooleanLiteral(bool value) { this.Value = value; }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitBooleanLiteral(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write(Value ? "true" : "false");
        }
    }

    public class NumericLiteral : Exp
    {
        public long Value;

        public NumericLiteral(long n) { this.Value = n; }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitNumericLiteral(this);
        }

        public Exp ToList()
        {
            throw new NotImplementedException();
        }

        public override void Write(TextWriter writer)
        {
            writer.Write(Value);
        }

    }
    public class RealLiteral : Exp
    {
        public double Value;
        public RealLiteral(double real) { this.Value = real; }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitRealLiteral(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write(Value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public class StringLiteral : Exp
    {
        public string String;
        public StringLiteral(string s) { this.String = s; }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitStringLiteral(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write('\'');
            writer.Write(String.Replace("'", "''"));
            writer.Write('\'');
        }
    }

    public class Id : Exp
    {
        public string Name;
        public Id(string name) { this.Name = name; }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitIdentifier(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write(Name);
        }
    }

    public class BinExp : Exp
    {
        public TokenType Op;
        public Exp Left;
        public Exp Right;

        public BinExp(TokenType op, Exp left, Exp right)
        {
            this.Op = op;
            this.Left = left;
            this.Right = right;
        }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitBinExp(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("(");
            Left.Write(writer);
            switch (Op)
            {
            case TokenType.Plus: writer.Write(" + "); break;
            case TokenType.Minus: writer.Write(" - "); break;
            case TokenType.Star: writer.Write(" * "); break;
            case TokenType.Slash: writer.Write(" / "); break;
            default: writer.Write(Op.ToString()); break;
            }
            Right.Write(writer);
            writer.Write(")");
        }
    }

    public class UnaryExp : Exp
    {
        public TokenType op;
        public Exp exp;
        public UnaryExp(TokenType op, Exp exp)
        {
            this.op = op;
            this.exp = exp;
        }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitUnaryExp(this);
        }


        public override void Write(TextWriter writer)
        {
            if (op == TokenType.Minus)
            {
                writer.Write("-");
            }
            exp.Write(writer);
        }
    }

    public abstract class PascalType : PascalSyntax
    {

    }

    public class Primitive : PascalType
    {
        public readonly Serialization.SerializedType Type;

        public Primitive(Serialization.SerializedType type)
        {
            this.Type = type;
        }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitPrimitiveType(this);
        }

        public override void Write(TextWriter writer)
        {
            var pt = (Serialization.PrimitiveType_v1)Type;
            if (pt.Domain == Types.Domain.Boolean)
                writer.Write("boolean");
            if (pt.ByteSize == 2)
                writer.Write("integer");
            if (pt.ByteSize == 4)
                writer.Write("longint");
        }

        public static Primitive Char()
        {
            return new Primitive( Serialization.PrimitiveType_v1.Char8());
        }

        public static Primitive Integer()
        {
            return new Primitive(Serialization.PrimitiveType_v1.Int16());
        }
    }

    public class Pointer : PascalType
    {
        public PascalType pointee;

        public Pointer(PascalType type)
        {
            this.pointee = type;
        }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitPointerType(this);
        }

        public override void Write(TextWriter writer)
        {
            pointee.Write(writer);
            writer.Write("^");
        }
    }

    public class File : PascalType
    {
        public File() { }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitFile(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("file");
        }
    }

    public class TypeReference : PascalType
    {
        public string TypeName;

        public TypeReference(string name)
        {
            this.TypeName = name;
        }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitTypeReference(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write(TypeName);
        }
    }

    public class Array : PascalType
    {
        public PascalType ElementType { get; internal set; }
        public List<ArrayDimension> Dimensions { get; internal set; }
        public bool Packed;

        public Array(PascalType elemType, List<ArrayDimension> dims)
        {
            this.ElementType = elemType;
            this.Dimensions = dims;
        }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitArrayType(this);
        }

        public override void Write(TextWriter writer)
        {
            if (Packed)
            {
                writer.Write("packed");
                writer.Write(" ");
            }
            writer.Write("array");
            writer.Write("[");
            WriteList(writer, ", ", Dimensions, WriteDimension);
            writer.Write("]");

        }

        private void WriteDimension(TextWriter writer, ArrayDimension dim)
        {
            dim.Low.Write(writer);
            if (dim.High != null)
            {
                writer.Write("..");
                dim.High.Write(writer);
            }
        }
    }

    public class SetType : PascalType
    {
        public string EnumName { get; internal set; }
        public List<string> Names { get; internal set; }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitSetType(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("set");
            writer.Write(" ");
            writer.Write("of");
            writer.Write(" ");
            if (EnumName != null)
            {
                writer.Write(EnumName);
            }
            else
            {
                writer.Write("(");
                WriteList(writer, ", ", Names, (w, s) => w.Write(s));
                writer.Write(")");
            }
        }
    }

    public class StringType : PascalType
    {
        public Exp Size;

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitStringType(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("string");
            if (Size != null)
            {
                writer.Write("[");
                Size.Write(writer);
                writer.Write("]");
            }
        }
    }

    public class Record : PascalType
    {
        public List<Field> Fields;
        public bool Packed;
        public VariantPart VariantPart;

        public override void Write(TextWriter writer)
        {
            writer.Write("record");
            writer.Write(" ");
            WriteList(writer, "; ", Fields, WriteField);
            if (VariantPart != null)
            {
                if (Fields.Count > 0)
                    writer.Write("; ");
                VariantPart.Write(writer);
            }
            writer.Write(" end");
        }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitRecord(this);
        }

        public static void WriteField(TextWriter writer, Field field)
        {
            writer.Write(string.Join(", ", field.Names));
            writer.Write(" : ");
            field.Type.Write(writer);
        }
    }

    public class ObjectType : PascalType
    {
        public ObjectType(List<Declaration> members)
        {
            this.Members = members;
        }

        public List<Declaration> Members { get; }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitObject(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("object");
            writer.Write(" ");
            WriteList(writer, "; ", Members);
            writer.Write(" end");
        }
    }

    public class VariantPart
    {
        public string VariantTag;
        public PascalType TagType;
        public List<Variant> Variants;

        public void Write(TextWriter writer)
        {
            writer.Write("case ");
            if (VariantTag != null)
            {
                writer.Write(VariantTag);
                writer.Write(" :");
            }
            TagType.Write(writer);
            writer.Write(" of ");
            PascalSyntax.WriteList(writer, "; ", Variants, WriteVariant);
        }

        private void WriteVariant(TextWriter writer, Variant variant)
        {
            PascalSyntax.WriteList(writer, ",", variant.TagValues);
            writer.Write(" : (");
            PascalSyntax.WriteList(writer, ";", variant.Fields, Record.WriteField);
            writer.Write(")");
        }
    }

    public class Variant
    { 
        public List<Exp> TagValues;
        public List<Field> Fields;
        public VariantPart VariantPart;
    }

    public class Field : PascalSyntax
    {
        public List<string> Names;
        public PascalType Type;

        public Field(List<string> names, PascalType type) { this.Names = names; this.Type = type; }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void Write(TextWriter writer)
        {
            throw new NotImplementedException();
        }
    }

    public class ParameterDeclaration
    {
        public List<string> ParameterNames { get; internal set; }
        public PascalType Type { get; internal set; }

        public bool ByReference;

        public static void Write(TextWriter writer, ParameterDeclaration decl)
        {
            if (decl.ParameterNames.Count == 1 && decl.ParameterNames[0] == "...")
            {
                writer.Write("...");
                return;
            }
            if (decl.ByReference)
            {
                writer.Write("var");
                writer.Write(" ");
            }
            PascalSyntax.WriteList(writer, ", ", decl.ParameterNames, (w, s) => w.Write(s));
            writer.Write(" : ");
            if (decl.Type == null)
                writer.Write("(NULL)");
            else 
                decl.Type.Write(writer);
        }
    }

    public class EnumType : PascalType
    {
        public List<string> Names;
        
        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitEnumType(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("(");
            WriteList(writer, ", ", Names, (w, s) => w.Write(s));
            writer.Write(")");
        }
    }

    public class RangeType : PascalType
    {
        public Exp Low;
        public Exp High;

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitRangeType(this);
        }

        public override void Write(TextWriter writer)
        {
            Low.Write(writer);
            if (High != null)
            {
                writer.Write("..");
                High.Write(writer);
            }
        }
    }

    public class CallableType : PascalType
    {
        public CallableType(List<ParameterDeclaration> procParameters, PascalType returnType = null)
        {
            this.Parameters = procParameters;
            this.ReturnType = returnType;
        }

        public List<ParameterDeclaration> Parameters { get; }
        public PascalType ReturnType { get; }
        public string CallingConvention { get; set; }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitCallableType(this);
        }
        public override void Write(TextWriter writer)
        {
            if (ReturnType is null)
            {
                writer.Write("procedure");
            }
            else
            {
                writer.Write("function");
            }
            if (Parameters.Count > 0)
            {
                writer.Write("(");
                WriteList(writer, "; ", Parameters, ParameterDeclaration.Write);
                writer.Write(")");
            }
            if (ReturnType != null)
            {
                writer.Write(" : ");
                ReturnType.Write(writer);
            }
            if (CallingConvention != null)
            {
                writer.Write("; {0}", CallingConvention);
            }
        }
    }

    public class ArrayDimension
    {
        public Exp Low;
        public Exp High;
    }

    public abstract class Block : PascalSyntax
    {
    }


    public class InlineMachineCode : Block
    {
        public List<Exp> Opcodes { get; set; }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitInlineMachineCode(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("inline");
            writer.Write(" ");
            WriteList(writer, ", ", Opcodes, WriteOpcode);
        }

        private void WriteOpcode(TextWriter w, Exp opcode)
        {
            var i = opcode as NumericLiteral;
            if (i != null)
            {
                w.Write("${0:X4}", i.Value);
            }
            else
            {
                opcode.Write(w);
            }
        }
    }

    public interface IPascalSyntaxVisitor<T>
    {
        T VisitArrayType(Array array);
        T VisitBinExp(BinExp binExp);
        T VisitBooleanLiteral(BooleanLiteral booleanLiteral);
        T VisitCallableDeclaration(CallableDeclaration cd);
        T VisitCallableType(CallableType callableType);
        T VisitConstantDeclaration(ConstantDeclaration cd);
        T VisitEnumType(EnumType enumType);
        T VisitFile(File file);
        T VisitIdentifier(Id id);
        T VisitInlineMachineCode(InlineMachineCode code);
        T VisitNumericLiteral(NumericLiteral number);
        T VisitObject(ObjectType objectType);
        T VisitPointerType(Pointer pointer);
        T VisitPrimitiveType(Primitive primitive);
        T VisitRangeType(RangeType rangeType);
        T VisitRealLiteral(RealLiteral realLiteral);
        T VisitRecord(Record record);
        T VisitSetType(SetType setType);
        T VisitStringLiteral(StringLiteral str);
        T VisitStringType(StringType strType);
        T VisitTypeDeclaration(TypeDeclaration td);
        T VisitTypeReference(TypeReference typeref);
        T VisitUnaryExp(UnaryExp unaryExp);
    }
}