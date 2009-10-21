using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class DoWhileIfFragment : ProcedureMock
    {
        protected override void BuildBody()
        {
            Label("loopHeader");
                SideEffect(Fn("DoStuff"));
                BranchIf(Not(Fn("NeedsBork")), "dontneedbork");
                Label("needbork");
                    SideEffect(Fn("Bork"));
                Label("dontneedbork");
            BranchIf(Not(Fn("Done")), "loopHeader");
            Return();
        }
    }
}
