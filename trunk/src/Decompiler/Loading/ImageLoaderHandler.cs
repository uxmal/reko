using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;

namespace Decompiler.Loading
{
    /// <summary>
    /// Custom configuration section that 
    /// </summary>
    public class ImageLoaderHandler : ConfigurationSection
    {
        [ConfigurationProperty("MagicNumber", IsRequired = true)]
        public string MagicNumber
        {
            get { return (string) this["MagicNumber"]; }
            set { this["MagicNumber"] = value; }
        }

        [ConfigurationProperty("Type", IsRequired = true)]
        public string LoaderType
        {
            get { return (string) this["Type"]; }
            set { this["Type"] = value; }
        }
    }
}
