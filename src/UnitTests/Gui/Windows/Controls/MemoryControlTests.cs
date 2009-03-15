using Decompiler.Core;
using Decompiler.WindowsGui.Controls;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.WindowsGui.Controls
{
	[TestFixture]
	public class MemoryControlTests
	{
		private byte [] bytes;

		public MemoryControlTests()
		{
			bytes = GenerateTestMemory();
		}

		[Test]
		public void Initialize()
		{
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
