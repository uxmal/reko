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

using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
	public class FlagGroup_v1 : SerializedKind
	{
		[XmlText]
		public string Name;

		public FlagGroup_v1()
		{
		}

		public FlagGroup_v1(string name)
		{
			this.Name = name;
		}

		public override Identifier Deserialize(ArgumentDeserializer sser)
		{
			return sser.Deserialize(this);
		}
	}
}
