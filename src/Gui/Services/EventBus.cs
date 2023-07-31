using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.Gui.Services
{
    public class EventBus : IEventBus
    {
        public event EventHandler<(Program, Address)>? ProcedureFound;

        private readonly SynchronizationContext ctx;

        public EventBus(SynchronizationContext ctx)
        {
            this.ctx = ctx;
        }

        public void RaiseProcedureFound(Program program, Address addr)
        {
            ctx.Post(_ =>
            {
                this.ProcedureFound?.Invoke(this, (program, addr));
            }, null);
        }
    }
}
