using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core.Types
{
    public class DataTypeBuilderUnifier : Unifier
    {
        private ITypeStore store;
        private int nestedCalls;        //$DEBUG

        public DataTypeBuilderUnifier(TypeFactory factory, ITypeStore store)
            : base(factory)
        {
            this.store = store;
        }

        public override DataType UnifyTypeVariables(TypeVariable tA, TypeVariable tB)
        {
            if (++nestedCalls > 300)        //$DEBUG
                nestedCalls.ToString();
            var dt = Unify(tA.Class.DataType, tB.Class.DataType);
            var eq = store.MergeClasses(tA, tB);
            eq.DataType = dt;
            --nestedCalls;
            return eq.Representative;
        }
    }
}
