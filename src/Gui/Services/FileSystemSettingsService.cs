#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Reko.Gui.Services
{
    /// <summary>
    /// An implementation of <see cref="ISettingsService"/> that saves user settings in the
    /// file system (thus providing crossplatform support).
    /// </summary>
    public class FileSystemSettingsService : SettingsService
    {
        private readonly IFileSystemService fsSvc;
        private readonly string settingsDirectory;
        private Dictionary<string, object> settings;

        public FileSystemSettingsService(IFileSystemService fsSvc) : this(fsSvc, SettingsService.SettingsDirectory)
        {
        }

        public FileSystemSettingsService(IFileSystemService fsSvc, string settingsDirectory)
        {
            this.fsSvc = fsSvc;
            this.settingsDirectory = settingsDirectory;
            this.settings = new();
        }

        public override void Delete(string name)
        {
            throw new NotImplementedException();
        }

        public override object? Get(string settingName, object? defaultValue)
        {
            if (!this.settings.TryGetValue(settingName, out var value))
                return defaultValue;
            if (value is JsonElement je)
            {
                return je.GetString();
            }
            return value;
        }

        public override string[] GetList(string settingName)
        {
            throw new NotImplementedException();
        }

        public override void Load()
        {
            var settingsFilePath = SettingsFilePath();
            // No file settings exist: first time use.
            if (!fsSvc.FileExists(settingsFilePath))
                return;
            using Stream stm = fsSvc.CreateFileStream(settingsFilePath, FileMode.Open);
            this.settings = JsonSerializer.Deserialize<Dictionary<string, object>>(stm)
                ?? new();
        }

        public override void Save()
        {
            var settingsFilePath = SettingsFilePath();
            using Stream stm = fsSvc.CreateFileStream(settingsFilePath, FileMode.Create);
            Save(stm);
        }

        public void Save(Stream stm)
        {
            JsonSerializer.Serialize(stm, this.settings, new JsonSerializerOptions()
            {
                WriteIndented = true,
            });
        }

        public override void Set(string name, object value)
        {
            throw new NotImplementedException();
        }

        public override void SetList(string name, IEnumerable<string> values)
        {
            settings[name] = values.ToList();
        }

        private string SettingsFilePath()
        {
            return Path.Combine(
                this.settingsDirectory,
                "usersettings.json");
        }
    }
}
