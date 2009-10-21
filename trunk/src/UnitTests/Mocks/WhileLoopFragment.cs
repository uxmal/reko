using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class WhileLoopFragment : ProcedureMock
    {
        protected override void BuildBody()
        {
            Identifier i = Local(PrimitiveType.Int32, "i");
            Identifier sum = Local(PrimitiveType.Int32, "sum");
            Assign(i, 0);
            Assign(sum, 0);

            Label("loopHeader");
            BranchIf(Ge(i, 100), "done");
                Assign(sum, Add(sum, i));
                Assign(i, Add(i, 1));
                Jump("loopHeader");

            Label("done");
            Return(sum);
        }
    }
}
