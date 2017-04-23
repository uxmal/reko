using Reko.Core;
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
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("12506D0F-1C67-4828-9601-96F8ED4D162D")]
    public interface INativeRewriter
    {
        void Next();
    }

    public class ArmRewriterNew : IEnumerable<RtlInstructionCluster>
    {
        private byte[] bytes;
        private int offset;
        private Address addr;

        public ArmRewriterNew(byte[] bytes, int offset, Address addr)
        {
            this.bytes = bytes;
            this.offset = offset;
            this.addr = addr;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            return new Enumerator(bytes, offset, addr.ToLinear());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public class Enumerator : IEnumerator<RtlInstructionCluster>
        {
            private INativeRewriter native;
            private byte[] bytes;
            private GCHandle hBytes;

            public Enumerator(byte[] bytes, int offset, ulong addr)
            {
                this.bytes = bytes;
                this.hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);

                this.native = CreateNativeRewriter(hBytes.AddrOfPinnedObject(), bytes.Length, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            }

            public RtlInstructionCluster Current { get; private set; }

            object IEnumerator.Current { get { return Current; } }

            public void Dispose()
            {
                if (this.hBytes != null && this.hBytes.IsAllocated)
                {
                    this.hBytes.Free();
                }
            }

            public bool MoveNext()
            {
                native.Next();
                return false;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }

        [DllImport("ArmNative.dll",CallingConvention = CallingConvention.Cdecl, EntryPoint = "CreateRewriter")]
        public static extern INativeRewriter CreateNativeRewriter(IntPtr rawbytes, int length, IntPtr rtlEmitter, IntPtr frame, IntPtr host);
    }
}
