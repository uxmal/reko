#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
 .
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Services
{
    /// <summary>
    /// Abstract base class for implementations of <see cref="ISettingsService"/> that need
    /// knowledge of where the settings directory is.
    /// </summary>
    public abstract class SettingsService : ISettingsService
    {
        public static string SettingsDirectory { get; }
        

        public abstract void Delete(string name);
        public abstract object? Get(string settingName, object? defaultValue);
        public abstract string[] GetList(string settingName);
        public abstract void Load();
        public abstract void Save();
        public abstract void Set(string name, object? value);
        public abstract void SetList(string name, IEnumerable<string> values);

        static SettingsService()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dir = Path.Combine(dir, "reko");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            SettingsDirectory = dir;
        }
    }
}
