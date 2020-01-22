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
    using System.Collections.Generic;

    /// <summary>
    /// Values that represent the PIC families.
    /// </summary>
    public enum PICFamily
    {
        Unknown = -1,
        PIC10,
        PIC12,
        PIC16,
        PIC18,
        PIC24,
        dsPIC30,
        dsPIC33,
        PIC32
    }

    /// <summary>
    /// Values that represent PIC Execution modes.
    /// </summary>
    public enum PICExecMode
    {
        /// <summary> PIC is executing the traditional/legacy instruction set. </summary>
        Traditional,
        /// <summary> PIC is executing the PIC18 extended instruction set with Indexed Literal Offset Addressing mode. </summary>
        Extended
    };

    /// <summary>
    /// Values that represent PIC Instruction-Set identifiers.
    /// </summary>
    public enum InstructionSetID
    {
        /// <summary> PIC Instruction Set is undefined. </summary>
        UNDEFINED,
        /// <summary> PIC Instruction Set is PIC16 like pic16f77 - basic mid-range. Identified as InstructionSet="pic16f77" in XML definition.</summary>
        PIC16,
        /// <summary> PIC Instruction Set is PIC16 like pic16f1946 - mid-range enhanced 5-bit BSR. Identified as InstructionSet="cpu_mid_v10" in XML definition.</summary>
        PIC16_ENHANCED,
        /// <summary> PIC Instruction Set is PIC16 like pic16f15313 - mid-range enhanced 6-bit BSR. Identified as InstructionSet="cpu_p16f1_v1" in XML definition.</summary>
        PIC16_FULLFEATURED,
        /// <summary> PIC Instruction Set is traditional PIC18 like pic18f1220 - without any extended mode. Identified as InstructionSet="pic18" in XML definition.</summary>
        PIC18,
        /// <summary> PIC Instruction Set is PIC18 like pic18f1230 - with extended execution mode capabilities. Identified as InstructionSet="egg" in XML definition.</summary>
        PIC18_EXTENDED,
        /// <summary> PIC Instruction Set is PIC18 enhanced like pic18f25k42 - same as PIC18_EXTD + some instructions for bigger RAM size. Identified as InstructionSet="cpu_pic18f_v6" in XML definition.</summary>
        PIC18_ENHANCED,
    }

    public static class PICFamiliesEx
    {
        private static readonly Dictionary<InstructionSetID, PICFamily> families = new Dictionary<InstructionSetID, PICFamily>()
        {
            { InstructionSetID.UNDEFINED,           PICFamily.Unknown },
            { InstructionSetID.PIC16,               PICFamily.PIC16 },
            { InstructionSetID.PIC16_ENHANCED,      PICFamily.PIC16 },
            { InstructionSetID.PIC16_FULLFEATURED,  PICFamily.PIC16 },
            { InstructionSetID.PIC18,               PICFamily.PIC18 },
            { InstructionSetID.PIC18_EXTENDED,      PICFamily.PIC18 },
            { InstructionSetID.PIC18_ENHANCED,      PICFamily.PIC18 },
        };

        public static PICFamily GetFamily(this InstructionSetID instrID)
        {
            if (families.TryGetValue(instrID, out PICFamily res))
                return res;
            return PICFamily.Unknown;
        }

    }

}
