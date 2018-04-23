#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Gui.Commands
{
    public class Cmd_MarkProcedures : Command
    {
        private IEnumerable<ProgramAddress> addresses;

        public Cmd_MarkProcedures(IServiceProvider services, IEnumerable<ProgramAddress> addresses)
            : base(services)
        {
            this.addresses = addresses;
        }

        public override void DoIt()
        {
            var decSvc = Services.RequireService<IDecompilerService>();
            var brSvc = Services.RequireService<IProjectBrowserService>();
            try
            {
                //$HACK: we have to stop events while this is happening. This is gross
                // but the future EventBus implementation will clear this up.
                foreach (var program in addresses.Select(a => a.Program).Distinct())
                {
                    program.ImageMap.PauseEventHandler();
                }
                var userProcs =
                    from hit in addresses
                    //$TODO: do this in a worker procedure.
                    let proc = decSvc.Decompiler.ScanProcedure(hit)
                    select new
                    {
                        Program = hit.Program,
                        Address = hit.Address,
                        UserProc = new Core.Serialization.Procedure_v1
                        {
                            Address = hit.Address.ToString(),
                            Name = proc.Name
                        }
                    };
                foreach (var up in userProcs)
                {
                    up.Program.EnsureUserProcedure(up.Address, up.UserProc.Name);
                }
            }
            finally
            {
                //$HACK: we have to stop events while this is happening. This is gross
                // but the future EventBus implementation will clear this up.
                foreach (var program in addresses.Select(a => a.Program).Distinct())
                {
                    program.ImageMap.UnpauseEventHandler();
                }
            }

            //$REVIEW: browser service should listen to changes in UserProcedures, no?
            brSvc.Reload();
        }
    }
}
