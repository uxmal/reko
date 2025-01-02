#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Types;
using Reko.Core.Machine;
using Reko.Core.Expressions;

namespace Reko.Environments.Windows
{
    public class PowerPcCallingConvention : AbstractCallingConvention
    {
        private IProcessorArchitecture arch;

        public PowerPcCallingConvention(IProcessorArchitecture arch) : base("")
        {
            this.arch = arch;
        }

        public override void Generate(
            ICallingConventionEmitter ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            //$TODO: finding it hard to locate information about the calling
            // convention on PowerPC Win32. May have to reverse engineer it.
        }

        public override bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                //$TODO: see comment above.
                return true;
            }
            //$TODO: handle stack args.
            return false;
        }

        public override bool IsOutArgument(Storage stg)
        {
            //$TODO
            return false;
        }
    }
}
