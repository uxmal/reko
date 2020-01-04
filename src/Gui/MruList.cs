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

using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.Gui
{
	/// <summary>
	/// Maintains a most-recently used file lists at a specified location in the user's settings directory.
	/// </summary>
	public class MruList
	{
        private int itemsMax;
        private List<string> items;

		public MruList(int itemsMax)
		{
			this.itemsMax = itemsMax;
			this.items = new List<string>(itemsMax);
		}

        public IList<string> Items => items;

		public void Load(IFileSystemService fsSvc, string fileLocation)
		{
            if (!fsSvc.FileExists(fileLocation))
                return;     // Save ourselves the pain of an exception.
			try
			{
				using (var reader = fsSvc.CreateStreamReader(fileLocation, new UTF8Encoding(false)))
				{
					string line = reader.ReadLine();
					while (line != null && items.Count < items.Capacity)
					{
                        items.Add(line.TrimEnd('\r', '\n'));
                        line = reader.ReadLine();
                    }
				}
			}
			catch (FileNotFoundException)
			{
			}
		}

		public void Save(IFileSystemService fsSvc, string fileLocation)
		{
			using (var writer = fsSvc.CreateStreamWriter(fileLocation, false, new UTF8Encoding(false)))
			{
                foreach (var item in Items)
                {
                    writer.WriteLine(item);
                }
            }
		}

		/// <summary>
		/// By using the selected index, we move the item to the top of the mru, shifting any other items down.
		/// </summary>
		/// <param name="index"></param>

		public void Use(string item)
		{
            var i = items.IndexOf(item);
            if (i >= 0)
            {
                items.RemoveAt(i);
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
