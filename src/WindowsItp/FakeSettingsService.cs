using Reko.Gui.Services;
using System;
using System.Collections.Generic;

namespace Reko.WindowsItp
{
    public class FakeSettingsService : ISettingsService
    {
        public object Get(string settingName, object defaultValue)
        {
            return defaultValue;
        }

        public string[] GetList(string settingName)
        {
            throw new NotImplementedException();
        }

        public void SetList(string name, IEnumerable<string> values)
        {
            throw new NotImplementedException();
        }

        public void Set(string name, object value)
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Delete(string name)
        {
            throw new NotImplementedException();
        }
    }

}