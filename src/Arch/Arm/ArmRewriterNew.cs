using Reko.Core.Rtl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm
{
    public class ArmRewriterNew : IEnumerable<RtlInstructionCluster>
    {
        public ArmRewriterNew()
        {
            var x = CreateNativeRewriter(IntPtr.Zero, 3, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        [DllImport("ArmNative.dll",CallingConvention = CallingConvention.Cdecl, EntryPoint = "CreateRewriter")]
        public static extern IntPtr CreateNativeRewriter(IntPtr rawbytes, int length, IntPtr rtlEmitter, IntPtr frame, IntPtr host);
    }
}
