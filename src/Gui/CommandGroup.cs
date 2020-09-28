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

namespace Reko.Gui
{
	public class CommandGroup
	{
		public static readonly Guid File = new Guid("{163D1008-E14A-42ac-B80B-E8663D5DBCBB}");

		public static readonly Guid Decompiler = new Guid("{2576B920-0744-4717-952E-C1A424F5CDA2}");
	}

	public class CmdID
	{
		public const int File = 0x0100;

		public const int NextPhase = 0x0300;
		public const int FinishPhases = 0x0301;
	}
}
