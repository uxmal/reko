/* 
 * Copyright (C) 1999-2009 John Källén.
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
    public class SerializedProcedureBase
    {
        /// <summary>
        /// The name of a procedure.
        /// </summary>
        [XmlAttribute("name")]
        public string Name;

        /// <summary>
        /// Procedure signature. If non-null, the user has specified a signature. If null, the
        /// signature is unknown.
        /// </summary>
        [XmlElement("signature")]
        public SerializedSignature Signature;

        [XmlElement("characteristics")]
        public ProcedureCharacteristics Characteristics;


    }
}
