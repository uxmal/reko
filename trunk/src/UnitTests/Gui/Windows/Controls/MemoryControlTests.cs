using Decompiler.Core;
using Decompiler.Gui.Windows.Controls;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows.Controls
{
	[TestFixture]
	public class MemoryControlTests
	{
        private Form form;
		private byte [] bytes;
        private MemoryControl memctl;

		public MemoryControlTests()
		{
			bytes = GenerateTestMemory();
		}

		[SetUp]
		public void Setup()
		{
            form = new Form();
            form.Size = new Size(300, 200);
            memctl = new MemoryControl();
            memctl.Dock = DockStyle.Fill;
            form.Controls.Add(memctl);
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
