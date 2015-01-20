using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.OdbgScript
{
    public enum eContextData
    {
        UE_EAX,
        UE_EBX,
        UE_ECX,
        UE_EDX,
        UE_ESI,
        UE_EDI,
        UE_EBP,
        UE_ESP,
        UE_EIP,
        UE_DR0,
        UE_DR1,
        UE_DR2,
        UE_DR3,
        UE_DR6,
        UE_DR7,
        UE_EFLAGS,
        UE_RSP,
        UE_RBP,

        UE_CSP,
        UE_CIP,
    }
}
