using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Types
{
    /// <summary>
    /// Models C++ '&' references.
    /// </summary>
    public class ReferenceTo : DataType
    {
        public ReferenceTo(DataType referent)
        {
            this.Referent = referent;
        } 

        public DataType Referent { get; set; }

        public override int Size { get { return Referent.Size; } set { Referent.Size = value; } }

        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitReference(this);
        }

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitReference(this);
        }

        public override DataType Clone()
        {
            return new ReferenceTo(Referent.Clone());
        }
    }
}
