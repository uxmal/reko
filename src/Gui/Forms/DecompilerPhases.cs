#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Forms
{

    /// <summary>
    /// The separate phases of the Reko decompiler.
    /// </summary>
    public class DecompilerPhases
    {
        public DecompilerPhases(
            InitialPageInteractor initial,
            IPreScanPageInteractor preScanning,
            IScannedPageInteractor scanning,
            IAnalyzedPageInteractor analysis,
            IFinalPageInteractor final)
        {
            Initial = initial;
            PreScanning = preScanning;
            Scanning = scanning;
            Analysis = analysis;
            Final = final;
        }
    
        public InitialPageInteractor Initial { get; }
        public IPreScanPageInteractor PreScanning { get;  }
        public IScannedPageInteractor Scanning { get; }
        public IAnalyzedPageInteractor Analysis { get;  }
        public IFinalPageInteractor Final { get;  }


    }
}
