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

using Reko.Core.Expressions;
using System;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Serializes an <see cref="Identifier"/> to a serialized argument.
    /// </summary>
	public class ArgumentSerializer 
	{
		private IProcessorArchitecture arch;

        /// <summary>
        /// Constructs an instance of <see cref="ArgumentSerializer"/>.
        /// </summary>
        /// <param name="arch">Processor architecture.</param>
        public ArgumentSerializer(IProcessorArchitecture arch)
		{
			this.arch = arch;
        }
        
        /// <summary>
        /// Given an <see cref="Identifier"/> serializes it to
        /// an <see cref="Argument_v1"/> serialization class.
        /// </summary>
        /// <param name="arg">Argument identifier to serialize</param>
        /// <param name="isOutParameter">Argument is an out parameter.</param>
        /// <returns></returns>
        public static Argument_v1? Serialize(Identifier? arg, bool isOutParameter)
        {
            if (arg is null)
                return null;
            if (arg.DataType is null)
                throw new ArgumentNullException("arg.DataType");
            var sarg = new Argument_v1 
            {
			    Name = arg.Name,
			    Kind = arg.Storage?.Serialize(),
                OutParameter = isOutParameter,
                Type = arg.DataType.Accept(new DataTypeSerializer()),
            };
            return sarg;
        }
    }
}
