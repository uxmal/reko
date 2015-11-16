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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Lib;
using Reko.Core.Serialization;
using Reko.Core.Types;

namespace Reko.Environments.Trs80
{
    public class Trs80Platform : Platform
    {
        public Trs80Platform(IServiceProvider services, IProcessorArchitecture arch) : base( services,  arch)
        {

        }

        // http://fjkraan.home.xs4all.nl/comp/trs80-4p/dmkeilImages/trstech.htm
        public override string DefaultCallingConvention
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string PlatformIdentifier
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override BitSet CreateImplicitArgumentRegisters()
        {
            throw new NotImplementedException();
        }

        public override ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention)
        {
            throw new NotImplementedException();
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override ProcedureBase GetTrampolineDestination(ImageReader imageReader, IRewriterHost host)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}
