using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class DoWhileFragment : ProcedureMock
    {
        protected override void BuildBody()
        {
            Label("loopBody");
            SideEffect(Fn("Frobulate"));
            BranchIf(Not(Fn("DoneFrobbing")), "loopBody");
            Return();
        }
    }
}
