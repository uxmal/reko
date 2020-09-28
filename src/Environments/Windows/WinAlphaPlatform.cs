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
    public class WinAlphaPlatform : Platform
    {
        private AlphaCallingConvention cc;

        public WinAlphaPlatform(IServiceProvider services, IProcessorArchitecture arch) 
            : base(services, arch, "winAlpha")
        {
            arch.StackRegister = arch.GetRegister("r30");
        }

        public override string DefaultCallingConvention
        {
            get { return null; }
        }

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotImplementedException();
        }

        public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            return new HashSet<RegisterStorage>();  //$TODO ret reguster, gp register?
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            return new HashSet<RegisterStorage>(new[] {
                "r0", "r1", "r2", "r3", "r4", "r5", "r6", "r7", "r8",
                   "r16", "r17", "r18", "r9", "r20", "r21",
                   "r22", "r23", "r24", "r25", "r26", "r27", "r28"}.Select(n => Architecture.GetRegister(n)));
        }

        public override ImageSymbol FindMainProcedure(Program program, Address addrStart)
        {
            Services.RequireService<DecompilerEventListener>().Warn(new NullCodeLocation(program.Name),
                           "Win32 Alpha main procedure finder not supported.");
            return null;
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
            if (this.cc == null)
            {
                this.cc = new AlphaCallingConvention(this.Architecture);
            }
            return cc;
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            return null;
        }
    }
}
