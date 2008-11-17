using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Gui
{
    public interface IProgressBar
    {
        int Value { get; set; }
        int Minimum { get; set; }
        int Maximum { get; set; }
    }
}
