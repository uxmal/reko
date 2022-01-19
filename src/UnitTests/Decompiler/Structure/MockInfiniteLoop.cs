using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Decompiler.Structure
{
    public class MockInfiniteLoop : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            Label("Infinite");
            SideEffect(Fn("DispatchEvents"));
            Goto("Infinite");
        }
    }
}
