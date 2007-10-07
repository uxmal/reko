using System;

namespace Decompiler.Gui
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
