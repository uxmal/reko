/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Loading;
using Decompiler.Core;
using Decompiler.Configuration;
using Decompiler.Gui;
using Decompiler.Gui.Windows.Forms;
using System;
using System.ComponentModel.Design;
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
                MainFormInteractor interactor = new MainFormInteractor();
                interactor.Run();
            }
			else
			{
                DecompilerHost host = NullDecompilerHost.Instance;
                DecompilerEventListener listener = NullDecompilerEventListener.Instance;

                ServiceContainer sc = new ServiceContainer();
                sc.AddService(typeof (DecompilerEventListener), listener);
                Loader ldr = new Loader(new DecompilerConfiguration(), sc);
				DecompilerDriver dec = new DecompilerDriver(ldr, host, sc);
				dec.Decompile(args[0]);
			}
		}
	}
}
