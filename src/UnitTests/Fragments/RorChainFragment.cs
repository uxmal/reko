using Reko.Arch.Z80;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Fragments
{
    public class RorChainFragment
    {
        public Procedure ShiftHlRightByC()
        {
            var arch = new Z80ProcessorArchitecture("z80");
            var m = new ProcedureBuilder(arch);
            var sp = m.Frame.EnsureRegister(Registers.sp);
            var a = m.Frame.EnsureRegister(Registers.a);
            var c = m.Frame.EnsureRegister(Registers.c);
            var h = m.Frame.EnsureRegister(Registers.h);
            var l = m.Frame.EnsureRegister(Registers.l);
            var C = m.Frame.EnsureFlagGroup(arch.GetFlagGroup("C"));
            var Z = m.Frame.EnsureFlagGroup(arch.GetFlagGroup("Z"));
            var SZC = m.Frame.EnsureFlagGroup(arch.GetFlagGroup("SZC"));
            var SZP = m.Frame.EnsureFlagGroup(arch.GetFlagGroup("SZP"));
            var rorc = new PseudoProcedure(
                PseudoProcedure.RorC,
                new FunctionType(
                    a,
                    a,
                    new Identifier("", PrimitiveType.Byte, null),
                    C));
            m.Assign(sp, m.Frame.FramePointer);
            m.Label("m1Loop");
            m.Assign(a, h);
            m.Assign(a, m.Or(a, a));
            m.Assign(SZC, m.Cond(a));
            m.Assign(C, Constant.False());
            m.Assign(a, m.Fn(rorc, a, Constant.Byte(1), C));
            m.Assign(C, m.Cond(a));
            m.Assign(h, a);
            m.Assign(a, l);
            m.Assign(a, m.Fn(rorc, a, Constant.Byte(1), C));
            m.Assign(C, m.Cond(a));
            m.Assign(l, a);
            m.Assign(c, m.ISub(c, 1));
            m.Assign(SZP, m.Cond(a));
            m.BranchIf(m.Test(ConditionCode.NE, Z), "m1Loop");
        
            m.Label("m2Done");
            m.Return();

            return m.Procedure;
        }
    }
}
