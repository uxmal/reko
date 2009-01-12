/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Lib;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
	/// <summary>
	/// Serialized representation of a procedure signature.
	/// </summary>
	public class SerializedSignature
	{
        public SerializedSignature()
        {
        }

		[XmlElement("return")]
		public SerializedArgument ReturnValue;

		[XmlAttribute("convention")]
		public string Convention;		

		[XmlAttribute("stackDelta")]
		[DefaultValue(0)]
		public int StackDelta;

		[XmlElement("arg")]
		public SerializedArgument [] Arguments;

        [XmlAttribute("fpuStackDelta")]
        [DefaultValue(0)]
        public int FpuStackDelta;
    }
}
