using Decompiler.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Structure
{
    public class MockInfiniteLoop : ProcedureMock
    {
        protected override void BuildBody()
        {
            Label("Infinite");
            SideEffect(Fn("DispatchEvents"));
            Jump("Infinite");
        }
    }
}
