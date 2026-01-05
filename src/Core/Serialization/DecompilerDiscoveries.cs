#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Collections;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
	/// <summary>
	/// Container class for discoveries made by the decompiler as it does its work.
	/// </summary>
	public class DecompilerDiscoveries
	{
		public DecompilerDiscoveries()
		{
			Procedures = new ArrayList();
			IndirectCalls = new ArrayList();
		}

		[XmlElement("procedure", typeof (SerializedProcedure))]
		public ArrayList Procedures;

		[XmlElement("call", typeof (SerializedCall))]
		public ArrayList IndirectCalls;

	}
}
