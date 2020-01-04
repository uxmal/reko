#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.NativeInterface;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.Arch.Arm.AArch32
{
    public class ThumbRewriter : ArmRewriter
    {
        public ThumbRewriter(
            ThumbArchitecture arch,
            EndianImageReader rdr,
            IRewriterHost host,
            IStorageBinder binder) :
            base(arch, rdr, host, binder, new T32Disassembler(arch, rdr).GetEnumerator())
        {
            base.pcValueOffset = 4;
        }

        public override Address ComputePcRelativeOffset(MemoryOperand mop)
        {
            var dst = (int) instr.Address.ToLinear() + 2;
            if (mop.Offset != null)
            {
                dst += mop.Offset.ToInt32();
            }
            return Address.Ptr32((uint) dst).Align(4);
        }

        protected override void ConditionalSkip(bool force)
        {
            if (instr.Mnemonic == Mnemonic.it)
                return;
            base.ConditionalSkip(force);
        }

        protected override void RewriteIt()
        {
            m.Nop();
        }
    }

    public class ThumbRewriterRetired : IEnumerable<RtlInstructionCluster>
    {
        private Dictionary<int, RegisterStorage> regs;
        private INativeArchitecture nArch;
        private EndianImageReader rdr;
        private IStorageBinder binder;
        private IRewriterHost host;

        public ThumbRewriterRetired(Dictionary<int, RegisterStorage> regs, INativeArchitecture nArch, EndianImageReader rdr, AArch32ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.regs = regs;
            this.nArch = nArch;
            this.rdr = rdr;
            this.binder = binder;
            this.host = host;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            var bytes = rdr.Bytes;
            var offset = (int)rdr.Offset;
            var addr = rdr.Address.ToLinear();
            return new Enumerator(regs, this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class Enumerator : IEnumerator<RtlInstructionCluster>
        {
            private INativeRewriter native;
            private byte[] bytes;
            private GCHandle hBytes;
            private RtlEmitter m;
            private NativeTypeFactory ntf;
            private NativeRtlEmitter rtlEmitter;
            private ArmNativeRewriterHost host;
            private IntPtr iRtlEmitter;
            private IntPtr iNtf;
            private IntPtr iHost;

            public Enumerator(Dictionary<int, RegisterStorage> regs, ThumbRewriterRetired outer)
            {
                this.bytes = outer.rdr.Bytes;
                ulong addr = outer.rdr.Address.ToLinear();
                this.hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);

                this.m = new RtlEmitter(new List<RtlInstruction>());
                this.ntf = new NativeTypeFactory();
                this.rtlEmitter = new NativeRtlEmitter(m, ntf, outer.host);
                this.host = new ArmNativeRewriterHost(regs, outer.binder, outer.host, this.ntf, rtlEmitter);

                this.iRtlEmitter = GetCOMInterface(rtlEmitter, IID_IRtlEmitter);
                this.iNtf = GetCOMInterface(ntf, IID_INativeTypeFactory);
                this.iHost = GetCOMInterface(host, IID_INativeRewriterHost);

                this.native = outer.nArch.CreateRewriter(
                    hBytes.AddrOfPinnedObject(),
                    bytes.Length,
                    (int)outer.rdr.Offset,
                    addr,
                    rtlEmitter,
                    ntf,
                    host);
            }

            public RtlInstructionCluster Current { get; private set; }

            object IEnumerator.Current { get { return Current; } }

            private IntPtr GetCOMInterface(object o, Guid iid)
            {
                var iUnknown = Marshal.GetIUnknownForObject(o);
                IntPtr intf;
                var hr2 = Marshal.QueryInterface(iUnknown, ref iid, out intf);
                return intf;
            }

            public void Dispose()
            {
                if (this.native != null)
                {
                    int n = native.GetCount();
                    Marshal.ReleaseComObject(this.native);
                    this.native = null;
                }
                if (iHost != null)
                {
                    Marshal.Release(iHost);
                }
                if (iNtf != null)
                {
                    Marshal.Release(iNtf);
                }
                if (iRtlEmitter != null)
                {
                    Marshal.Release(iRtlEmitter);
                }
                if (this.hBytes != null && this.hBytes.IsAllocated)
                {
                    this.hBytes.Free();
                }
            }

            public bool MoveNext()
            {
                m.Instructions = new List<RtlInstruction>();
                int n = native.GetCount();
                if (native.Next() == 1)
                    return false;
                this.Current = this.rtlEmitter.ExtractCluster();
                return true;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }

        static ThumbRewriterRetired()
        {
            IID_INativeRewriter = typeof(INativeRewriter).GUID;
            IID_INativeRewriterHost = typeof(INativeRewriterHost).GUID;
            IID_INativeTypeFactory = typeof(INativeTypeFactory).GUID;
            IID_IRtlEmitter = typeof(INativeRtlEmitter).GUID;
        }

        private static Guid IID_INativeRewriter;
        private static Guid IID_INativeRewriterHost;
        private static Guid IID_IRtlEmitter;
        private static Guid IID_INativeTypeFactory;
    }
}
