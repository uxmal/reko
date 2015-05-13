#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.Mos6502;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Environments.C64
{
    /// <summary>
    /// Implementation of the C64 platform.
    /// </summary>
    public class C64Platform : Platform
    {
        private Mos6502ProcessorArchitecture arch;

        public C64Platform(IServiceProvider services, Mos6502ProcessorArchitecture arch)
            : base(services, arch)
        {
            this.arch = arch;
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override BitSet CreateImplicitArgumentRegisters()
        {
            return Architecture.CreateRegisterBitset();
        }

        public override SystemService FindService(RtlInstruction rtl, ProcessorState state)
        {
            return base.FindService(rtl, state);
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override ProcedureBase GetTrampolineDestination(ImageReader imageReader, IRewriterHost host)
        {
            return null;
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr16(c.ToUInt16());
        }
    }
}
