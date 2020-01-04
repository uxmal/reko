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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ArmNativeRewriterHost : MarshalByRefObject, INativeRewriterHost
    {
        private Dictionary<int, RegisterStorage> regs;
        private Dictionary<int, RegisterStorage> coprocregs;
        private IStorageBinder frame;
        private IRewriterHost host;
        private NativeTypeFactory ntf;
        private NativeRtlEmitter m;

        public ArmNativeRewriterHost(Dictionary<int, RegisterStorage> regs, IStorageBinder frame, IRewriterHost host, NativeTypeFactory ntf, NativeRtlEmitter m)
        {
            Debug.Assert(regs != null);
            this.regs = regs;
            this.coprocregs = new Dictionary<int, RegisterStorage>();
            this.frame = frame;
            this.host = host;
            this.ntf = ntf;
            this.m = m;
        }

        public virtual RegisterStorage GetRegister(int reg)
        {
            return regs[reg];
        }

        public virtual RegisterStorage GetSysRegister(int sysreg)
        {
            return regs[sysreg];
        }

        public virtual RegisterStorage GetCoprocRegister(int coprocreg)
        {
            if (!coprocregs.TryGetValue(coprocreg, out var reg))
            {
                reg = new RegisterStorage($"p{coprocreg}", (int)StorageDomain.Register + 0x800, 0, PrimitiveType.Word32);
                coprocregs.Add(coprocreg, reg);
            }
            return reg;
        }

        public HExpr CreateTemporary(BaseType size)
        {
            var id = frame.CreateTemporary(Interop.DataTypes[size]);
            return m.MapToHandle(id);
        }

        public HExpr EnsureFlagGroup(int baseReg, int bitmask, string name, BaseType size)
        {
            var reg = regs[baseReg];
            var id = frame.EnsureFlagGroup(reg, (uint)bitmask, name, Interop.DataTypes[size]);
            return m.MapToHandle(id);
        }

        public HExpr EnsureRegister(int regKind, int reg)
        {
            RegisterStorage r;
            switch (regKind)
            {
            case 0:   // standard register
                r = GetRegister(reg);
                break;
            case 1:
                r = GetSysRegister(reg);
                break;
            case 2:
                r = GetCoprocRegister(reg);
                break;
            default:
                throw new NotImplementedException($"Unknown register kind {regKind}.");
            }
            var id = frame.EnsureRegister(r);
            return m.MapToHandle(id);
        }

        public HExpr EnsureSequence(int regHi, int regLo, BaseType size)
        {
            var hi = regs[regHi];
            var lo = regs[regLo];
            var id = frame.EnsureSequence(Interop.DataTypes[size], hi, lo);
            return m.MapToHandle(id);
        }

        public void Error(ulong uAddress, string error)
        {
            host.Error(m.CreateAddress(uAddress), error);
        }

        public HExpr EnsurePseudoProcedure(string name, BaseType dt, int arity)
        {
            var exp = host.EnsurePseudoProcedure(name, ntf.GetRekoType((HExpr) dt), arity);
            var pc = new ProcedureConstant(PrimitiveType.Ptr32, exp);
            return m.MapToHandle(pc);
        }
    }
}
