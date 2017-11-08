using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Name;
    }

    public class ConstantDeclaration : Declaration
    {
        public Exp Exp;

        public ConstantDeclaration(string name, Exp exp) { this.Name = name;  this.Exp = exp; }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitConstantDeclaration(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("const");
            writer.Write(' ');
            writer.Write(Name);
            writer.Write(" = ");
            Exp.Write(writer);
        }
    }

    public class TypeDeclaration : Declaration
    {
        public PascalType Type;

        public TypeDeclaration(string name, PascalType type) { this.Name = name;  this.Type = type; }

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

    public class CallableDeclaration : Declaration
    {
        public PascalType ReturnType;
        public List<ParameterDeclaration> Parameters;

        public Block Body { get; internal set; }

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
        public long n;

        public NumericLiteral(long n) { this.n = n; }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitNumericLiteral(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write(n);
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
            writer.Write(Value);
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
        private TokenType op;
        private Exp term1;
        private Exp right;

        public BinExp(TokenType op, Exp left, Exp right)
        {
            this.op = op;
            this.term1 = left;
            this.right = right;
        }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitBinExp(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("(");
            term1.Write(writer);
            switch (op)
            {
            case TokenType.Minus: writer.Write(" - "); break;
            default: writer.Write(op.ToString()); break;
            }
            right.Write(writer);
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
        public Serialization.SerializedType Type;

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

        public override void Write(TextWriter writer)
        {
            writer.Write("record");
            writer.Write(" ");
            WriteList(writer, "; ", Fields, WriteField);
        }

        public override T Accept<T>(IPascalSyntaxVisitor<T> visitor)
        {
            return visitor.VisitRecord(this);
        }

        private void WriteField(TextWriter writer, Field field)
        {
            writer.Write(field.Name);
            writer.Write(" : ");
            field.Type.Write(writer);
        }
    }

    public class Field
    {
        public string Name;
        public PascalType Type;

        public Field(string name, PascalType type) { this.Name = name; this.Type = type; }
    }

    public class ParameterDeclaration
    {
        public List<string> ParameterNames { get; internal set; }
        public PascalType Type { get; internal set; }

        public bool ByReference;

        public static void Write(TextWriter writer, ParameterDeclaration decl)
        {
            if (decl.ByReference)
            {
                writer.Write("var");
                writer.Write(" ");
            }
            PascalSyntax.WriteList(writer, ", ", decl.ParameterNames, (w, s) => w.Write(s));
            writer.Write(" : ");
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
                w.Write("${0:X4}", i.n);
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
        T VisitConstantDeclaration(ConstantDeclaration cd);
        T VisitEnumType(EnumType enumType);
        T VisitIdentifier(Id id);
        T VisitInlineMachineCode(InlineMachineCode code);
        T VisitNumericLiteral(NumericLiteral number);
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