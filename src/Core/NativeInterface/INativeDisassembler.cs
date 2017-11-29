using System;
using System.Runtime.InteropServices;

namespace Reko.Core.NativeInterface
{
    [ComVisible(true)]
    [Guid("10475E6B-D167-4DB3-B211-610F6073A313")]
    public interface INativeDisassembler
    {
        IntPtr NextInstruction();
        void Render(IntPtr instr, INativeInstructionWriter writer);
        void DestroyInstruction(IntPtr instr);
    }
}