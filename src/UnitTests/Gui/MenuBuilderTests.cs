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

using Reko.Gui;
using NUnit.Framework;
using System;
using System.ComponentModel.Design;
using System.IO;

namespace Reko.UnitTests.Gui
{
	[TestFixture]
	public class MenuBuilderTests
	{
		private string nl = Environment.NewLine;

		[Test]
		public void Mb_Create()
		{
			StringWriter sb = new StringWriter();
			MenuBuilder mb = new FakeMenuBuilder(sb);
			mb.AddMenuItem("&Hello", new CommandID(CommandGroup.Decompiler, CmdID.NextPhase));
			mb.AddMenuItem("&Goodbye", new CommandID(CommandGroup.Decompiler, CmdID.FinishPhases));
			string exp =
				@"&Hello" + nl +
				@"&Goodbye" + nl;
			Assert.AreEqual(exp, sb.ToString());

		}
	}

	public class FakeMenuBuilder : MenuBuilder
	{
		private StringWriter sb;

		public FakeMenuBuilder(StringWriter sb)
		{
			this.sb = sb;
		}

		public override void AddMenuItem(string text, CommandID cmdID)
		{
			sb.WriteLine(text);
		}

		public override void AddSeparator()
		{
			sb.WriteLine("---");
		}

		public override void BeginSubMenu()
		{
		}

		public override void EndSubMenu()
		{
		}
	}
}
