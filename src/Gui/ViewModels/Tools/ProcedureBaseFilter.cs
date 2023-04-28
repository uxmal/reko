using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.ViewModels.Tools
{
    public enum ProcedureBaseFilter
    {
        // Show all procedures
        All,
        // Show procedures with no callers
        Roots,
        // Show procedures with no calls
        Leaves
    }
}
