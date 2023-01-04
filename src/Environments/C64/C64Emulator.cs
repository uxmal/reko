#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Emulation;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Environments.C64
{
    public class C64Emulator : IPlatformEmulator
    {
        private SegmentMap segmentMap;
        private Dictionary<Address, ImportReference> importReferences;

        public C64Emulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            this.segmentMap = segmentMap;
            this.importReferences = importReferences;
            this.InterceptedCalls = new Dictionary<Address, ExternalProcedure>();
        }

        public Dictionary<Address, ExternalProcedure> InterceptedCalls { get; }

        public ImageSegment? InitializeStack(IProcessorEmulator emulator, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public bool InterceptCall(IProcessorEmulator emulator, uint calledAddress)
        {
            return false;
        }

        public bool EmulateSystemCall(IProcessorEmulator emulator, params MachineOperand[] operands)
        {
            return false;
        }

        public void TearDownStack(ImageSegment? stackSeg)
        {
            throw new NotImplementedException();
        }
    }
}
