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

namespace Reko.Gui
{
    /// <summary>
    /// Use to navigate to the MixedCodeDataViewer to a specific statement.
    /// </summary>
    public class StatementNavigator : ICodeLocation
    {
        private Program program;
        private IServiceProvider services;

        public StatementNavigator(Program program, Statement stm, IServiceProvider services)
        {
            this.program = program;
            this.Statement = stm;
            this.services = services;
            this.Text = program.SegmentMap.MapLinearAddressToAddress(stm.LinearAddress).ToString();
        }

        public Statement Statement { get; private set; }

        public string Text { get; private set; }

        public void NavigateTo()
        {
            var codeSvc = services.GetService<ICodeViewerService>();
            if (codeSvc != null)
                codeSvc.DisplayStatement(program, Statement);
        }
    }
}
