#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Linq;
using System.Runtime.Versioning;
using System.Text;

namespace Reko.UserInterfaces.WindowsForms
{
    [SupportedOSPlatform("windows")]
    public class WindowsFormsRegistryService : IRegistryService
    {
        private RegistryKey hkcu;

        public WindowsFormsRegistryService()
        {
            this.hkcu = new RegistryKey(Microsoft.Win32.Registry.CurrentUser);
        }

        public IRegistryKey CurrentUser { get { return hkcu; } }

        public class RegistryKey : IRegistryKey
        {
            private Microsoft.Win32.RegistryKey key;

            public RegistryKey(Microsoft.Win32.RegistryKey key)
            {
                if (key is null)
                    throw new ArgumentNullException(nameof(key));
                this.key = key;
            }

            public object GetValue(string name, object defaultValue)
            {
                return key.GetValue(name, defaultValue);
            }

            public IRegistryKey OpenSubKey(string keyName, bool writeable)
            {
                var subkey = key.OpenSubKey(keyName, writeable);
                if (subkey is null)
                {
                    subkey = key.CreateSubKey(keyName);
                }
                return new RegistryKey(subkey);
            }

            public void SetValue(string name, object value)
            {
                if (value is null)
                    key.DeleteValue(name, false);
                else
                    key.SetValue(name, value);
            }

            public void DeleteValue(string name)
            {
                key.DeleteValue(name, false);
            }

            public void Dispose()
            {
                if (key is not null)
                    key.Dispose();
            }
        }
    }
}
