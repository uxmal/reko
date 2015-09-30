using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Types
{
    public class DataTypeBuilderUnifier : Unifier
    {
        private ITypeStore store;

        public DataTypeBuilderUnifier(TypeFactory factory, ITypeStore store)
            : base(factory)
        {
            this.store = store;
        }

        public override DataType UnifyTypeVariables(TypeVariable tA, TypeVariable tB)
        {
            var dt = Unify(tA.Class.DataType, tB.Class.DataType);
            var eq = store.MergeClasses(tA, tB);
            eq.DataType = dt;
            return eq.Representative;
        }
    }
}
