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

using Reko.Core;
using Reko.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UserInterfaces.WindowsForms
{
    public class WindowsFormsSettingsService : ISettingsService
    {
        private IServiceProvider services;

        public WindowsFormsSettingsService(IServiceProvider services)
        {
            this.services = services;
        }

        private IRegistryKey GetRegistryKey(string appRelativeKeyName, bool writeable)
        {
            var regSvc = services.RequireService<IRegistryService>();
            var keyName = "Software\\jklSoft\\Reko";
            if (!string.IsNullOrEmpty(appRelativeKeyName))
                keyName = string.Join("\\", keyName, appRelativeKeyName);
            return regSvc.CurrentUser.OpenSubKey(keyName, writeable);
        }

        public object Get(string settingName, object defaultValue)
        {
            string keyName; string valName;
            SplitIntoKeyValueName(settingName, out keyName, out valName);
            var key = GetRegistryKey(keyName, false);
            return key.GetValue(valName, defaultValue);
        }

        public string[] GetList(string settingName)
        {
            byte[] bytes = (byte[]) Get(settingName, new byte[] { });
            return Encoding.UTF8.GetString(bytes).Split((char) 0);
        }

        public void SetList(string name, IEnumerable<string> settings)
        {
            Set(name, Encoding.UTF8.GetBytes(
                string.Join("\0", settings)));
        }

        public void Set(string name, object value)
        {
            string keyName; string valName;
            SplitIntoKeyValueName(name, out keyName, out valName); 
            var key = GetRegistryKey(keyName, true);
            key.SetValue(valName, value);
        }

        public void Delete(string name)
        {
            string keyName; string valName;
            SplitIntoKeyValueName(name, out keyName, out valName); 
            var key = GetRegistryKey(keyName, true);
            key.DeleteValue(valName);
        }

        private void SplitIntoKeyValueName(string name, out string keyName, out string valName)
        {
            if (string.IsNullOrEmpty(name))
            {
                keyName = "";
                valName = null;
            }
            else
            {
                int i = name.LastIndexOf('/');
                if (i < 0)
                {
                    keyName = "";
                    valName = name;
                }
                else
                {
                    keyName = name.Remove(i).Replace('/','\\');
                    valName = name.Substring(i + 1);
                }
            }
        }


        public void Load()
        {
            // Doesn't need an implementation, since we load live values from the registry.
        }

        public void Save()
        {
            // Doesn't need an implementation, since we save values to the registry immediately in the Set method.
        }
    }
}
