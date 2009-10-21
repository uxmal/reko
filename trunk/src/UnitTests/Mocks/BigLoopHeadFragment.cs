using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class BigLoopHeadFragment : ProcedureMock
    {
        protected override void BuildBody()
        {
            Label("loopHeader");
            SideEffect(Fn("DoSomething"));
            SideEffect(Fn("DoSomethingelse"));
            BranchIf(Fn("IsDone"), "done");
            Label("loopBody");
                SideEffect(Fn("LoopWork"));
                Jump("loopHeader");
            Label("done");
            Return();
        }
    }
}
