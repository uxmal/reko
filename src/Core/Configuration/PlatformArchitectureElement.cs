using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Configuration
{
    /// <summary>
    /// Contains processor-specific settings for a particular 
    /// platform.
    /// </summary>
    public interface IPlatformArchitectureElement
    {
        string Name { get; }
        List<string> TrashedRegisters { get; }
    }

    public class PlatformArchitectureElement : IPlatformArchitectureElement
    {
        public PlatformArchitectureElement()
        {
            this.TrashedRegisters = new List<string>();
        }

        public string Name { get; internal set; }
        public List<string> TrashedRegisters { get; internal set; }
    }
}
