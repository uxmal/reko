#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
	public class SerializedSequence : SerializedKind
	{
		[XmlElement("reg")]
		public Register_v1 [] Registers;

		public SerializedSequence()
		{
		}

		public SerializedSequence(SequenceStorage seq)
		{
			Registers = new Register_v1[2];
			Registers[0] = new Register_v1(seq.Head.Name);
			Registers[1] = new Register_v1(seq.Tail.Name);
		}

		public override Identifier Deserialize(ArgumentDeserializer sser)
		{
			return sser.Deserialize(this);
		}
	}
}
