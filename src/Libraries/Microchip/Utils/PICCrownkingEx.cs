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
    using Microchip.V1;

    /// <summary>
    /// Various extensions methods to manipulate PIC definitions.
    /// </summary>
    public static partial class PICCrownkingEx
    {

        /// <summary>
        /// A PICCrownking extension method that gets a PIC descriptor.
        /// </summary>
        /// <param name="db">The PIC database to retrieve definition from.</param>
        /// <param name="sPICName">Name of the PIC.</param>
        /// <returns>
        /// The PIC descriptor or null.
        /// </returns>
        public static IPICDescriptor GetPIC(this PICCrownking db, string sPICName)
            => db.GetPICAsXML(sPICName)?.ToObject<PIC_v1>().PICDescriptorInterface;

        /// <summary>
        /// A PICCrownking extension method that gets a PIC descriptor.
        /// </summary>
        /// <param name="db">The PIC database to retrieve definition from.</param>
        /// <param name="iProcID">Identifier for the processor.</param>
        /// <returns>
        /// The PIC descriptor or null.
        /// </returns>
        public static IPICDescriptor GetPIC(this PICCrownking db, int iProcID)
            => db.GetPICAsXML(iProcID)?.ToObject<PIC_v1>().PICDescriptorInterface;

    }

}
