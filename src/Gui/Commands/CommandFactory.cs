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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Gui.Commands
{
    public class CommandFactory : ICommandFactory
    {
        private IServiceProvider services;

        public CommandFactory(IServiceProvider services)
        {
            this.services = services;
        }

        public ICommand MarkProcedure(ProgramAddress address)
        {
            return new Cmd_MarkProcedures(services, new ProgramAddress[] { address });
        }

        public ICommand MarkProcedures(IEnumerable<ProgramAddress> addresses)
        {
            return new Cmd_MarkProcedures(services, addresses);
        }

        public ICommand ViewFindPattern()
        {
            throw new NotImplementedException();
        }

        public ICommand ViewWhatPointsHere(Program program, Address address)
        {
            return new Cmd_ViewWhatPointsHere(services, program, new [] { address });
        }

        public ICommand ViewWhatPointsHere(Program program, IEnumerable<Address> range)
        {
            return new Cmd_ViewWhatPointsHere(services, program, range);
        }

        public ICommand EditSignature(Program program, Procedure procedure, Address addr)
        {
            return new Cmd_EditSignature(services, program, procedure, addr);
        }
    }
}
