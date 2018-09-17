using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Absyn
{
    public class AbsynCompoundAssignment : AbsynAssignment
    {
        public AbsynCompoundAssignment(Expression dst, BinaryExpression src) : base(dst, src)
        {
            this.Src = src;
        }

        public new BinaryExpression Src { get; private set; }


        public override void Accept(IAbsynVisitor visitor)
        {
            visitor.VisitCompoundAssignment(this);
        }

        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitCompoundAssignment(this);
        }
    }
}
