#region License
/* 
* Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.Mono
{
    public class MonoSettingsService : ISettingsService
    {
        private IServiceProvider services;

        public MonoSettingsService(IServiceProvider services)
        {
            this.services = services;
        }

        public object Get(string settingName, object defaultValue)
        {
            throw new NotImplementedException();
        }

        public string[] GetList(string settingName)
        {
            throw new NotImplementedException();
        }

        public void SetList(string name, IEnumerable<string> settings)
        {
            throw new NotImplementedException();
        }

        public void Set(string name, object value)
        {
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
                    keyName = name.Remove(i).Replace('/', '\\');
                    valName = name.Substring(i + 1);
                }
            }
        }

        public void Load()
        {
            var configFile = Path.Combine(
                Environment.GetEnvironmentVariable("HOME"),
                ".config/.decompilerrc");
            //$TODO: load from configFile.
        }

        public void Save()
        {
            var configDir = Path.Combine(
                Environment.GetEnvironmentVariable("HOME"),
                ".config"); 
            //$REVIEW: if the .config dir doesn't exist, should we create it?
            if (Directory.Exists(configDir))
            {
                var configFile = Path.Combine(configDir, ".decompilerrc");
                throw new NotImplementedException("Write file into configFile");
            }
        }
    }
}
