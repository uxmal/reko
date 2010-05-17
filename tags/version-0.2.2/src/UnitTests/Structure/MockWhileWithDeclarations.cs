using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Structure
{
    public class MockWhileWithDeclarations : ProcedureMock
    {
        protected override void BuildBody()
        {
            Identifier i = Local32("i");
            Label("loopHeader");
            Identifier v = Declare(PrimitiveType.Byte, "v", Load(PrimitiveType.Byte, i));
            Assign(i, Add(i, 1));
            BranchIf(Eq(v, 0x20), "exit_loop");

            Store(Word32(0x00300000), v);
            Jump("loopHeader");
            Label("exit_loop");
            Return();
        }
    }
}
