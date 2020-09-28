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
using Reko.Core.CLanguage;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Environments.Windows
{
    public class WinAArch64Platform : Platform
    {
        private RegisterStorage framePointer;
        private RegisterStorage linkRegister;

        public WinAArch64Platform(IServiceProvider services, IProcessorArchitecture arch) :
            base(services, arch, "winArm64")
        {
            this.framePointer = arch.GetRegister("x29");
            this.linkRegister = arch.GetRegister("x30");
        }

        public override string DefaultCallingConvention => "";

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotImplementedException();
        }

        // https://docs.microsoft.com/en-us/cpp/build/arm64-windows-abi-conventions?view=vs-2020
        public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            return new HashSet<RegisterStorage>
            {
                framePointer, linkRegister
            };
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            //$TODO: make this happen.
            return new HashSet<RegisterStorage>();
        }

        public override ImageSymbol FindMainProcedure(Program program, Address addrStart)
        {
            {
                Services.RequireService<DecompilerEventListener>().Warn(new NullCodeLocation(program.Name),
                    "Win32 AArch main procedure finder not supported.");
                return null;
            }
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override int GetByteSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public override CallingConvention GetCallingConvention(string ccName)
        {
            return new AArch64CallingConvention(this.Architecture);
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}
