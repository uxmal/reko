#region License
/* 
 * Copyright (c) 2017-2020 Christian Hostelet.
 *
 * The contents of this file are subject to the terms of the Common Development
 * and Distribution License (the License), or the GPL v2, or (at your option)
 * any later version. 
 * You may not use this file except in compliance with the License.
 *
 * You can obtain a copy of the License at http://www.gnu.org/licenses/gpl-2.0.html.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *
 * If applicable, add the following below the header, with the fields
 * enclosed by brackets [] replaced by your own identifying information:
 * "Portions Copyrighted (c) [year] [name of copyright owner]"
 *
 */

#endregion

namespace Reko.Libraries.Microchip
{
    using System.Collections;

    public static class PICRemapMemDomains
    {
        private readonly static Hashtable subdom2dom = new Hashtable(20)
        {
            { PICMemorySubDomain.SFR,      PICMemoryDomain.Data },
            { PICMemorySubDomain.GPR,      PICMemoryDomain.Data },
            { PICMemorySubDomain.DPR,      PICMemoryDomain.Data },
            { PICMemorySubDomain.NNMR,     PICMemoryDomain.Data },
            { PICMemorySubDomain.Emulator, PICMemoryDomain.Data },
            { PICMemorySubDomain.Linear,   PICMemoryDomain.Data },
            { PICMemorySubDomain.DMA,      PICMemoryDomain.Data },
            { PICMemorySubDomain.Code,             PICMemoryDomain.Prog },
            { PICMemorySubDomain.ExtCode,          PICMemoryDomain.Prog },
            { PICMemorySubDomain.EEData,           PICMemoryDomain.Prog },
            { PICMemorySubDomain.DeviceConfig,     PICMemoryDomain.Prog },
            { PICMemorySubDomain.DeviceConfigInfo, PICMemoryDomain.Prog },
            { PICMemorySubDomain.DeviceInfoAry,    PICMemoryDomain.Prog },
            { PICMemorySubDomain.UserID,           PICMemoryDomain.Prog },
            { PICMemorySubDomain.DeviceID,         PICMemoryDomain.Prog },
            { PICMemorySubDomain.RevisionID,       PICMemoryDomain.Prog },
            { PICMemorySubDomain.Debugger,         PICMemoryDomain.Prog },
            { PICMemorySubDomain.Calib,            PICMemoryDomain.Prog },
            { PICMemorySubDomain.Test,             PICMemoryDomain.Prog },
        };

        public static PICMemoryDomain GetDomain(this PICMemorySubDomain subdom)
            => (PICMemoryDomain)(subdom2dom[subdom] ?? PICMemoryDomain.Unknown);

    }

}
