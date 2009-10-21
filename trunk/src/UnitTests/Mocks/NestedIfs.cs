using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;

namespace Decompiler.UnitTests.Mocks
{
    public class NestedIfs : ProcedureMock
    {
        protected override void BuildBody()
        {
            Identifier ax = Local(PrimitiveType.Int16, "ax");
            Identifier cl = Local(PrimitiveType.Byte, "cl");
            BranchIf(base.Lt(ax, 0), "negatory");
            Label("positive");
                Assign(cl, 0);
                BranchIf(Le(ax,12),"small");
                Label("very_positive");
                    Assign(ax,12);
                Label("small");
                Jump("join");
            Label("negatory");
                Assign(cl, 1);
                BranchIf(Ge(ax, -12), "negsmall");
                Label("very_negative");
                    Assign(ax,-12);
                Label("negsmall");
                Jump("join");
            Label("join");
            Return();
        }
    }
}