/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler;
using Decompiler.Core;
using Decompiler.WindowsGui.Forms;
using System;
using System.Windows.Forms;

namespace WindowsDecompiler
{
	public class Driver
	{
		[STAThread]
		public static void Main(string [] args)
		{
			if (args.Length == 0)
			{
				MainForm form = new MainForm();
				MainFormInteractor interactor = new MainFormInteractor(form);
				Application.Run(form);
			}
			else
			{
				DecompilerDriver dec = new DecompilerDriver(args[0], new Program(), new NullDecompilerHost());
				dec.Decompile();
			}
		}
	}
}
