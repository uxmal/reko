using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Gui.Commands
{
    public class Cmd_MarkAndScanProcedure : Command
    {
        private Program program;
        private Address address;

        public Cmd_MarkAndScanProcedure(
            IServiceProvider services,
            Program program,
            Address address)
            : base(services)
        {
            this.program = program;
            this.address = address;
        }

        public override void DoIt()
        {
            var decompiler = Services.RequireService<IDecompilerService>().Decompiler;
            var brSvc = Services.RequireService<IProjectBrowserService>();
            var proc = decompiler.ScanProcedure(program, address);
            var userp = new Reko.Core.Serialization.Procedure_v1
            {
                Address = address.ToString(),
                Name = proc.Name,
            };
            var ups = program.UserProcedures;
            if (!ups.ContainsKey(address))
            {
                ups.Add(address, userp);
            }
            brSvc.Reload();
        }
    }
}
