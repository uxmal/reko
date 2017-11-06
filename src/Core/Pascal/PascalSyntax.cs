using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Pascal
{
    public class PascalSyntax
    {
    }

    public class Exp : PascalSyntax
    {

    }

    public class Number : Exp
    {
        private long n;

        public Number(long n) { this.n = n; }

        public override string ToString()
        {
            return n.ToString();
        }
    }

    public class RealLiteral : Exp
    {
        public double Real;
        public RealLiteral(double real) { this.Real = real; }
    }

    public class StringLiteral : Exp
    {
        public string String;
        public StringLiteral(string s) { this.String = s; }
    }

    public class Id : Exp
    {
        public string Name;
        public Id(string name) { this.Name = name; }
    }

    public class BinExp : Exp
    {
        private TokenType minus;
        private Exp term1;
        private Exp right;

        public BinExp(TokenType minus, Exp left, Exp right)
        {
            this.minus = minus;
            this.term1 = left;
            this.right = right;
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
    }

    public class PascalType : PascalSyntax
    {

    }

    public class Primitive : PascalType
    {
        public Serialization.SerializedType type;
    }

    public class Pointer : PascalType
    {
        public PascalType pointee;

        public Pointer(PascalType type)
        {
            this.pointee = type;
        }
    }

    public class TypeReference : PascalType
    {
        public string TypeName;

        public TypeReference(string name)
        {
            this.TypeName = name;
        }
    }

    public class Array : PascalType
    {
        public PascalType ElementType { get; internal set; }
        public List<ArrayDimension> Dimensions { get; internal set; }
        public bool Packed { get; internal set; }
    }

    public class SetType : PascalType
    {
        public string EnumName { get; internal set; }
        public List<string> Names { get; internal set; }
    }

    public class StringType  : PascalType
    {
        public Exp Size;
    }

    public class Record : PascalType
    {
        public List<Field> Fields;
        public bool Packed { get; internal set; }
    }

    public class Field
    {
        public string Name;
        public PascalType Type;

        public Field(string name, PascalType type) { this.Name = name;  this.Type = type; }
    }

    public class ParameterDeclaration
    {
        public List<string> ParameterNames { get; internal set; }
        public PascalType Type { get; internal set; }
    }

    public class EnumType : PascalType
    {
        public List<string> Names;
    }

    public class RangeType : PascalType
    {
        public Exp Low;
        public Exp High;
    }

    public class ArrayDimension
    {
        public Exp Low;
        public Exp High;
    }

    public class Block : PascalSyntax
    { }


    public class InlineMachineCode : Block
    {
        public List<Exp> Opcodes { get; set; }
    }
}
