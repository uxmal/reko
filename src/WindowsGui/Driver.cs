/* 
 * Copyright (C) 1999-2007 John Källén.
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
using Decompiler.Core.Serialization;
using Decompiler.Gui;
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Decompiler.WindowsGui
{
	public class Driver
	{
		[STAThread]
		public static void Main(string [] args)
		{
			if (args.Length < 1)
			{
				Application.Run(new Forms.MainForm());
			}
			else
			{
				DecompilerDriver dec = new DecompilerDriver();
				dec.DecompileBinary(args[0], null);
			}
		}
	}
}
