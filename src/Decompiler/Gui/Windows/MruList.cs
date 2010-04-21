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

using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Decompiler.Gui.Windows
{
	/// <summary>
	/// Maintains a most-recently used file lists at a specified location in the user's settings directory.
	/// </summary>
	public class MruList
	{
		private int itemsMax;
		private ArrayList items;

		public MruList(int itemsMax)
		{
			this.itemsMax = itemsMax;
			this.items = new ArrayList(itemsMax);
		}

		public IList Items
		{
			get { return items; }
		}

		public void Load(string fileLocation)
		{
			try
			{
				using (StreamReader rdr = new StreamReader(fileLocation, new UTF8Encoding(false)))
				{
					string line = rdr.ReadLine();
					while (line != null && items.Count < items.Capacity)
					{
						this.items.Add(line.TrimEnd('\r', '\n'));
						line = rdr.ReadLine();
					}
				}
			}
			catch (FileNotFoundException)
			{
			}
		}

		public void Save(string fileLocation)
		{
			using (StreamWriter wrt = new StreamWriter(fileLocation, false, new UTF8Encoding(false)))
			{
				foreach (string line in items)
				{
					wrt.WriteLine(line);
				}
			}
		}

		/// <summary>
		/// By using the selected index, we move the item to the top of the mru, shifting any other items down.
		/// </summary>
		/// <param name="index"></param>

		public void Use(string item)
		{
			for (int i = 0; i < items.Count; ++i)
			{
				if ((string) items[i] == item)
				{
					items.RemoveAt(i);
					break;
				}
			}

			items.Insert(0, item);

			int itemsToKill = items.Count - itemsMax;
			if (itemsToKill > 0)
			{
				items.RemoveRange(itemsMax, itemsToKill);
			}
		}

	}
}
