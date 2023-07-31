using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Services
{
    /// <summary>
    /// General brokerage of events from non-GUI threads.
    /// </summary>
    public interface IEventBus
    {
        event EventHandler<(Program, Address)>? ProcedureFound;

        void RaiseProcedureFound(Program program, Address address);
    }
}
