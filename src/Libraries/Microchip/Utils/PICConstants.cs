#region License
/* 
 *
 * Copyrighted (c) 2017-2023 Christian Hostelet.
 *
 * The contents of this file are subject to the terms of the Common Development
 * and Distribution License (the License), or the GPL v2, or (at your option)
 * any later version. 
 * You may not use this file except in compliance with the License.
 *
 * You can obtain a copy of the License at http://www.netbeans.org/cddl.html
 * or http://www.gnu.org/licenses/gpl-2.0.html.
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
 * When distributing Covered Code, include this CDDL Header Notice in each file
 * and include the License file at http://www.netbeans.org/cddl.txt.
 * If applicable, add the following below the CDDL Header, with the fields
 * enclosed by brackets [] replaced by your own identifying information:
 * "Portions Copyrighted (c) [year] [name of copyright owner]"
 *
 */

#endregion

namespace Reko.Libraries.Microchip
{
    /// <summary>
    /// A class defining some constants related to Microchip PIC database.
    /// </summary>
    public static class PICConstants
    {
        /// <summary>
        /// The filename of the local database file (ZIP archive).
        /// </summary>
        public const string LocalDBFilename = "picdb.zip";

        /// <summary>
        /// The filename of the parts information file inside the PIC database.
        /// </summary>
        public const string PartsInfoFilename = "partsinfo.xml";

        /// <summary>
        /// Relative pathname for PIC16 definition files in the PIC database.
        /// </summary>
        public const string ContentPIC16Path = EDCContent + "16xxxx";
        /// <summary>
        /// Relative pathname for PIC16 definition files in the PIC database.
        /// </summary>
        public const string ContentPIC18Path = EDCContent + "18xxxx";

        private const string EDCContent = @"content/edc/";

    }

}
