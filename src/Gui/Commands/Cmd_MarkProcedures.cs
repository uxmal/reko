#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Gui.Commands
{
    /// <summary>
    /// User-interface command to start a recursive scan at a given
    /// address to yield one or more procedures.
    /// </summary>
    /// <remarks>
    /// This is expected to run on the GUI thread, since it may 
    /// pop up a dialog.
    /// </remarks>
    public class Cmd_MarkProcedures : Command
    {
        private IDecompilerService decSvc;
        private IEnumerable<ProgramAddress> addresses;

        public Cmd_MarkProcedures(IServiceProvider services, IEnumerable<ProgramAddress> addresses)
            : base(services)
        {
            this.decSvc = Services.RequireService<IDecompilerService>();
            this.addresses = addresses;
        }

        public override void DoIt()
        {
            var brSvc = Services.RequireService<IProjectBrowserService>();
            var procsSvc = Services.RequireService<IProcedureListService>();

            //$TODO if arch > 1 pick arch.
            try
            {
                //$HACK: we have to stop events while this is happening. This is gross
                // but the future EventBus implementation will clear this up.
                foreach (var program in addresses.Select(a => a.Program).Distinct())
                {
                    program.ImageMap.PauseEventHandler();
                }
                var sArch = DetermineArchitecture();
                var userProcs =
                    from hit in addresses
                    let uProc = DoScanProcedure(hit, sArch)
                    where uProc != null
                    select new
                    {
                        hit.Program,
                        hit.Address,
                        UserProc = new Procedure_v1
                        {
                            Address = hit.Address.ToString(),
                            Name = uProc.Name
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
            procsSvc.Load(decSvc.Decompiler.Project);
            procsSvc.Show();
        }

        private ProcedureBase DoScanProcedure(ProgramAddress paddr, string sArch)
        {
            //$TODO: do this in a worker procedure.
            if (sArch == null)
                return null;
            if (!paddr.Program.Architectures.TryGetValue(sArch, out var arch))
                return null;
            var proc = decSvc.Decompiler.ScanProcedure(paddr, arch);
            return proc;
        }

        /// <summary>
        /// Determine which processor architecture to use. If more than 
        /// one architecture is available, present user with an option
        /// to pick one.
        /// </summary>
        /// <returns>Null if no architecture could be determined,
        /// or the name of the chosen architecture.
        /// </returns>
        private string DetermineArchitecture()
        {
            var archs = addresses.SelectMany(a => a.Program.Architectures.Values)
                .GroupBy(a => a.Name)
                .OrderBy(g => g.Key)
                .Select(g => new ListOption
                {
                    Text = g.First().Description,
                    Value = g.Key,
                })
                .ToArray();
            if (archs.Length == 0)
                return null;
            if (archs.Length == 1)
                return (string)archs[0].Value;
            var dlgFactory = Services.RequireService<IDialogFactory>();
            var uiSvc = Services.RequireService<IDecompilerShellUiService>();
            using (var dlg = dlgFactory.CreateSelectItemDialog("Select a processor architecture", archs, false))
            {
                if (uiSvc.ShowModalDialog(dlg) != DialogResult.OK)
                    return null;
                return (string)(((ListOption) dlg.SelectedItem).Value);
            }
        }
    }
}