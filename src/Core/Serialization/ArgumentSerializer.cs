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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;

namespace Reko.Core.Serialization
{
	public class ArgumentSerializer 
	{
		private IProcessorArchitecture arch;

        public ArgumentSerializer(IProcessorArchitecture arch)
		{
			this.arch = arch;
        }
        
        public Argument_v1 Serialize(Identifier arg)
        {
            if (arg == null)
                return null;
            if (arg.DataType == null)
                throw new ArgumentNullException("arg.DataType");
            Argument_v1 sarg = new Argument_v1 
            {
			    Name = arg.Name,
			    Kind = arg.Storage != null ? arg.Storage.Serialize() : null,
                OutParameter = arg.Storage is OutArgumentStorage,
                Type = arg.DataType.Accept(new DataTypeSerializer()),
            };
            return sarg;
        }
    }
}
