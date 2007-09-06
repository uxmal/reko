using Decompiler.WindowsGui.Controls;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.WindowsGui.Controls
{
	[TestFixture]
	public class MemoryControlTests
	{
		public MemoryControlTests()
		{
			byte [] bytes = GenerateTestMemory();

		}

		[Test]
		public void Initialize()
		{
			MemoryControl ctl = new MemoryControl();
		}

		private byte [] GenerateTestMemory()
		{
			System.IO.MemoryStream stm = new System.IO.MemoryStream();
			for (int i = 0; i < 1024; ++i)
			{
				stm.WriteByte((byte)i);
			}
			return stm.ToArray();
		}
	}
}
