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

    public class StringLiteral : Exp
    {
        public string String;
        public StringLiteral(string s) { this.String = s; }
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
        public Exp High { get; internal set; }
        public Exp Low { get; internal set; }
        public bool Packed { get; internal set; }
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
}
