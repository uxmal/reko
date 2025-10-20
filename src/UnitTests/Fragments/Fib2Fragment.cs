using Reko.Core;
using Reko.Core.Expressions;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Fragments
{
    class Fib2Fragment
    {
        public static Program Generate()
        {
            var pb = new ProgramBuilder();
            pb.Add("main", m =>
            {
                var esp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                m.Assign(esp, m.Frame.FramePointer);
                m.Assign(esp, m.ISub(esp, 40));
                m.Assign(esp, m.ISubS(esp, 4));
                m.MStore(esp, m.IAdd(esp, 4));
                m.Assign(esp, m.ISubS(esp, 4));
                m.MStore(esp, m.Word32(10));
                m.Call("twoFib", 4);
                m.Assign(esp, m.IAdd(esp, 52));
                m.Return();
            });

            /// The function "twoFib" is not only recursive, but fetches data from hte caller's stack frame!
            pb.Add("twoFib", m =>
            {
                var esp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
                var eax = m.Reg32("eax");
                var ecx = m.Reg32("ecx");
                var edx = m.Reg32("edx");
                var ebp = m.Reg32("ebp");
                var Z = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("Z"));
                m.Assign(esp, m.ISubS(esp, 4));
                m.MStore(esp, ebp);
                m.Assign(ebp, esp);
                m.Assign(esp, m.ISub(esp, 10));
                m.Assign(Z, m.Cond(Z.DataType, m.ISub(m.Mem32(m.IAdd(ebp, 0x0C)), 0)));
                m.BranchIf(m.Test(ConditionCode.NE, Z), "l080483B8");

                m.Label("l080483A8");
                m.MStore(m.ISub(ebp, 0x08), m.Word32(0));
                m.MStore(m.ISub(ebp, 0x04), m.Word32(1));
                m.Goto("l080483E0");

                m.Label("l080483B8");
                m.Assign(edx, m.ISub(ebp, 0x08));
                m.Assign(esp, m.ISub(esp, 08));
                m.Assign(eax, m.Mem32(m.IAdd(ebp, 0x0C)));
                m.Assign(eax, m.ISub(eax, 1));
                m.Assign(esp, m.ISubS(esp, 4));
                m.MStore(esp, eax);
                m.Assign(esp, m.ISubS(esp, 4));
                m.MStore(esp, edx);
                m.Call("twoFib", 4);

                m.Assign(esp, m.IAdd(esp, 0x0C));
                m.Assign(eax, m.Mem32(m.ISub(ebp, 0x08)));
                m.MStore(m.ISub(ebp, 0x0C), eax);
                m.Assign(eax, m.Mem32(m.ISub(ebp, 0x04)));
                m.MStore(m.ISub(ebp, 0x08), eax);
                m.Assign(edx, m.Mem32(m.ISub(ebp, 0x0C)));
                m.Assign(eax, m.ISub(ebp, 0x04));
                m.MStore(eax, m.IAdd(m.Mem32(eax), edx));

                m.Label("l080483E0");
                m.Assign(eax, m.Mem32(m.ISub(ebp, 0x08)));
                m.Assign(edx, m.Mem32(m.ISub(ebp, 0x04)));
                m.Assign(ecx, m.Mem32(m.IAdd(ebp, 0x08)));
                m.MStore(ecx, eax);
                m.MStore(m.IAdd(ecx, 0x04), edx);
                m.Assign(eax, m.Mem32(m.IAdd(ebp, 0x08)));
                m.Assign(esp, ebp);
                m.Assign(ebp, m.Mem32(esp));
                m.Assign(esp, m.IAdd(esp, 4));
                m.Return();
            });

            return pb.BuildProgram();
        }
        /*
        void twofib(struct Eq_76 * dwArg04, int32 dwArg08)
{
    word32 dwLoc08_55;
        word32 dwLoc0C_54;
    if (dwArg08 == 0x00)
    {
        dwLoc0C_54 = 0x00;
        dwLoc08_55 = 0x01;
    }
    else
    {
        twofib(fp - 0x0C, dwArg08 - 0x01);
    dwLoc0C_54 = dwLoc08;
        dwLoc08_55 = dwLoc08 + dwLoc0C;
    }
dwArg04->dw0000 = dwLoc0C_54;
    dwArg04->dw0004 = dwLoc08_55;
}    }
}
*/
    }
}