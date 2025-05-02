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
		public string? InstructionAddress;

        /// <summary>
        /// The function signature used for the call.
        /// </summary>
		[XmlElement("signature")]
		public SerializedSignature? Signature;

        /// <summary>
        /// Optional comment.
        /// </summary>
        [XmlElement("comment")]
        public string? Comment;

        /// <summary>
        /// True if the call is a "noreturn" call. This means that the function
        /// diverges and does not return to the caller.
        /// </summary>
        [XmlElement("noreturn")]
        [DefaultValue(false)]
        public bool NoReturn;

        /// <summary>
        /// Creates an uninitialized serialized call.
        /// </summary>
		public SerializedCall_v1()
		{
		}

        /// <summary>
        /// Constructs a serialized call with the specified address and signature.
        /// </summary>
        /// <param name="addr">Address at which the call takes place.</param>
        /// <param name="sig">Function signature of the call.</param>
		public SerializedCall_v1(Address addr, SerializedSignature sig)
		{
			InstructionAddress = addr.ToString();
			Signature = sig;
		}
	}
}
