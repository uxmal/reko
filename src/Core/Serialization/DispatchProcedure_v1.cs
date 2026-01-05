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

using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// A dispatch procedure is a procedure where a single entry entry point
    /// specified by the address provides multiple services, depending on an
    /// input variable.
    /// </summary>
    public class DispatchProcedure_v1 : ProcedureBase_v1
    {
        /// <summary>
        /// Address at which the dispatch procedure is located.
        /// </summary>
        [XmlAttribute("address")]
        public string? Address;

        /// <summary>
        /// Sub-services provided by the dispatch procedure.
        /// </summary>
        [XmlElement("service")]
        public SerializedService[]? Services;
    }
}
