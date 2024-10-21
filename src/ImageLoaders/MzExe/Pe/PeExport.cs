using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.MzExe.Pe;

public class PeExport
{
    public PeExport(uint rvaAddr, string? name)
    {
        RvaAddress = rvaAddr;
        Name = name;
    }

    public uint RvaAddress { get; }
    public string? Name { get; }
}
