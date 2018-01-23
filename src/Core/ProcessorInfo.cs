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

using System;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.Core
{
	/// <summary>
	/// Summarizes information about a particular processor.
	/// </summary>
	[XmlRoot("processorinfo")]
	public class ProcessorInfo
	{
		public ProcessorInfo()
		{
		}

		[XmlElement("name")]
		public string Name;

		[XmlElement("heuristics")]
		public Heuristics heuristics;
	}

	public class Heuristics
	{
		[XmlElement("pattern-rules")]
		public PatternRule [] PatternRules;
	}

	public class PatternRule
	{
		[XmlElement("pattern")]
		public string Pattern;
		[XmlElement("action")]
		public string Action;
	}
}
