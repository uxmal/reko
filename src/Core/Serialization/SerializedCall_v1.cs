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

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
	/// <summary>
	/// Used to represent the signature of a procedure call occurring at a specified address.
	/// </summary>
	/// If while rewriting the code a call is found at the InstructionAddress, the rewriter will
	/// emit an Application rather than a call instruction. This allows the user to coerce a 
	/// particular call instruction to a specified signature.
	public class SerializedCall_v1
	{
		/// <summary>
		/// The address of the call instruction whose signature we wish to override.
		/// </summary>
		[XmlElement("address")]
		public string InstructionAddress;

		[XmlElement("signature")]
		public SerializedSignature Signature;

        [XmlElement("comment")]
        public string Comment;

        [XmlElement("noreturn")]
        [DefaultValue(false)]
        public bool NoReturn;

		public SerializedCall_v1()
		{
		}

		public SerializedCall_v1(Address addr, SerializedSignature sig)
		{
			InstructionAddress = addr.ToString();
			Signature = sig;
		}
	}
}
