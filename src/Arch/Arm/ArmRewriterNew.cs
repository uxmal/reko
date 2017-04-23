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
    public enum BaseType
    {
        Void,

        Bool,

        Byte,
        SByte,
        Char8,

        Int16,
        UInt16,
        Ptr16,
        Word16,

        Int32,
        UInt32,
        Ptr32,
        Word32,

        Int64,
        UInt64,
        Ptr64,
        Word64,

        Real32,
        Real64,
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("12506D0F-1C67-4828-9601-96F8ED4D162D")]
    public interface INativeRewriter
    {
        void Next();
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("56E6600F-619E-441F-A2C3-A37F07BA0DA0")]
    [ComVisible(true)]
    public interface INativeRewriterHost
    {
        [PreserveSig] int EnsureRegister(int reg);
        //int EnsureSequence(int regHi, int regLo, BaseType size);
        //int EnsureFlagGroup(int baseReg, int bitmask, string name, BaseType size);
        //int CreateTemporary(int /*BaseType*/ size);
        //void Error(ulong uAddress, string error);
        //int PseudoProcedure(string name, BaseType x);
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



                var host = new ArmNativeHost();
                var factory = Marshal.GetIUnknownForObject(host);
                var iid = new Guid("56E6600F-619E-441F-A2C3-A37F07BA0DA0");
                IntPtr ifac;
                var hr = Marshal.QueryInterface(factory, ref iid, out ifac);

                this.native = CreateNativeRewriter(hBytes.AddrOfPinnedObject(), bytes.Length, IntPtr.Zero, ifac);
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
        public static extern INativeRewriter CreateNativeRewriter(IntPtr rawbytes, int length, IntPtr rtlEmitter, IntPtr host);
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    class ArmNativeHost : MarshalByRefObject, INativeRewriterHost
    {
        public int CreateTemporary(int /*BaseType*/ size)
        {
            throw new NotImplementedException();
        }

        public int EnsureFlagGroup(int baseReg, int bitmask, string name, BaseType size)
        {
            throw new NotImplementedException();
        }

        public int EnsureRegister(int reg)
        {
            return 42;
        }

        public int EnsureSequence(int regHi, int regLo, BaseType size)
        {
            throw new NotImplementedException();
        }

        public void Error(ulong uAddress, string error)
        {
            throw new NotImplementedException();
        }

        public int PseudoProcedure(string name, BaseType x)
        {
            throw new NotImplementedException();
        }
    }
}
