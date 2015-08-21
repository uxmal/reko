﻿using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Scanning.Fragments
{
    public class RtlEvenOdd
    {
        public static RtlTrace[] Create(FakeArchitecture arch)
        {
            var frame = arch.CreateFrame();
            var r1 = frame.EnsureRegister(arch.GetRegister(1));
            var sp = frame.EnsureRegister(arch.StackRegister);
            return new RtlTrace[]
            {
                new RtlTrace(0x1000)    // main
                {
                    m => {m.Assign(r1, 3); },
                    m => { m.Assign(sp, m.ISub(sp, 4)); m.Assign(m.LoadDw(sp), r1); },
                    m => { m.Call(Address.Ptr32(0x1200), 4); },
                    m => { m.Assign(r1, 3); },
                    m => { m.Assign(sp, m.ISub(sp, 4)); m.Assign(m.LoadDw(sp), r1); },
                    m => { m.Call(Address.Ptr32(0x1100), 4); },
                    m => { m.Return(4, 4); }
                },

                new RtlTrace(0x1100)    // odd
                {
                    m => { m.Assign(r1, m.LoadDw(m.IAdd(sp, 4))); },
                    m => { m.Branch(m.Eq0(r1), Address.Ptr32(0x1120), RtlClass.ConditionalTransfer); },
                    m => { m.Assign(r1, m.LoadDw(m.IAdd(sp, 4))); },
                    m => { m.Assign(r1, m.ISub(r1, 1)); },
                    m => { m.Assign(m.LoadDw(m.IAdd(sp, 4)), r1); },
                    m => { m.Goto(Address.Ptr32(0x1200)); }
                },
                new RtlTrace(0x1120)
                {
                    m => { m.Assign(r1, Constant.Word32(0)); },
                    m => { m.Return(4, 4); }
                },

                new RtlTrace(0x1200)    // event
                {
                    m => { m.Assign(r1, m.LoadDw(m.IAdd(sp, 4))); },
                    m => { m.Branch(m.Eq0(r1), Address.Ptr32(0x1220), RtlClass.ConditionalTransfer); },
                    m => { m.Assign(r1, m.LoadDw(m.IAdd(sp, 4))); },
                    m => { m.Assign(r1, m.ISub(r1, 1)); },
                    m => { m.Assign(m.LoadDw(m.IAdd(sp, 4)), r1); },
                    m => { m.Goto(Address.Ptr32(0x1100)); }
                },
                new RtlTrace(0x1220)
                {
                    m => { m.Assign(r1, Constant.Word32(1)); },
                    m => { m.Return(4, 4); }
                },
            };
        }
    }
}