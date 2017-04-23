#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core;
using Reko.Core.NativeInterface;
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

                var rtlEmitter = new RtlNativeEmitter();
                var oRtlEmitter = Marshal.GetIUnknownForObject(rtlEmitter);
                IntPtr iRtlEmitter;
                var hr = Marshal.QueryInterface(oRtlEmitter, ref IID_INativeRewriterHost, out iRtlEmitter);

                var host = new ArmNativeRewriterHost();
                var oHost = Marshal.GetIUnknownForObject(host);
                IntPtr iHost;
                hr = Marshal.QueryInterface(oHost, ref IID_INativeRewriterHost, out iHost);

                this.native = CreateNativeRewriter(hBytes.AddrOfPinnedObject(), bytes.Length, iRtlEmitter, iHost);
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

        static ArmRewriterNew()
        {
            IID_INativeRewriterHost = typeof(INativeRewriterHost)
                .GetCustomAttributes(typeof(GuidAttribute), false)
                .Select(a => new Guid(((GuidAttribute)a).Value))
                .First();
            IID_IRtlEmitter = typeof(IRtlNativeEmitter)
                .GetCustomAttributes(typeof(GuidAttribute), false)
                .Select(a => new Guid(((GuidAttribute)a).Value))
                .First();
        }

        private static  Guid IID_INativeRewriterHost;
        private static  Guid IID_IRtlEmitter;


        [DllImport("ArmNative.dll",CallingConvention = CallingConvention.Cdecl, EntryPoint = "CreateRewriter")]
        public static extern INativeRewriter CreateNativeRewriter(IntPtr rawbytes, int length, IntPtr rtlEmitter, IntPtr host);
    }
}
