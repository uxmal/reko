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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public class M68kProcedureSerializer 
        : ProcedureSerializer
    {
        public M68kProcedureSerializer(M68kArchitecture arch, ISerializedTypeVisitor<DataType> typeLoader, string defaultCc)
            : base(arch, typeLoader, defaultCc)
        {
        }

        public override ProcedureSignature Deserialize(SerializedSignature ss, Frame frame)
        {
            if (ss == null)
                return null;
            var argser = new ArgumentSerializer(this, Architecture, frame, ss.Convention);
            Identifier ret = null;

            if (ss.ReturnValue != null)
            {
                ret = argser.DeserializeReturnValue(ss.ReturnValue);
            }

            var args = new List<Identifier>();
            if (ss.Arguments != null)
            {
                for (int iArg = 0; iArg < ss.Arguments.Length; ++iArg)
                {
                    var sArg = ss.Arguments[iArg];
                    Identifier arg = argser.Deserialize(sArg);
                    args.Add(arg);
                }
            }

            var sig = new ProcedureSignature(ret, args.ToArray());
            return sig;
        }

        public override Storage GetReturnRegister(Argument_v1 sArg, int bitSize)
        {
            throw new NotImplementedException();
        }
    }
}
