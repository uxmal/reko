#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.Gui.Windows.Forms
{
    public class UnscannedBlocksResult : CodeAddressSearchResult
    {
        public UnscannedBlocksResult(
            IServiceProvider services, 
            IEnumerable<ProgramAddress> addresses) : 
            base(services, addresses)
        {

        }

        public override bool QueryStatus(CommandID cmdID, CommandStatus status, CommandText txt)
        {
            if (cmdID.Guid == CmdSets.GuidReko)
            {
                switch (cmdID.ID)
                {
                case CmdIds.ActionMarkProcedure:
                case CmdIds.ActionScanHeuristically:
                    status.Status = MenuStatus.Enabled|MenuStatus.Visible;
                    return true;
                }
            }
            return base.QueryStatus(cmdID, status, txt);
        }

        public override bool Execute(CommandID cmdID)
        {
            if (cmdID.Guid == CmdSets.GuidReko)
            {
                switch (cmdID.ID)
                {
                case CmdIds.ActionScanHeuristically: if (!View.IsFocused) return false; ScanHeuristically(); return true;
                }
                return false;
            }
            return base.Execute(cmdID);
        }

        public void ScanHeuristically()
        {
            var decSvc = services.RequireService<IDecompilerService>();
            var selected = base.SelectedHits();
            foreach (var hit in selected)
            {
                var scanner = new Scanner(
                    hit.Program, 
                    null,
                    null,
                    services.RequireService<WindowsDecompilerEventListener>());
                //var hs = new HeuristicScanner(hit.Program,)
            }
        }
    }
}
