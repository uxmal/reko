/* 
 * Copyright (C) 1999-2010 John Källén.
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
using System.Xml.Serialization;
using System.Text;

namespace Decompiler.Core.Serialization
{
    public class ArraySizeCharacteristic
    {
        private string argReference;
        private ArraySizeFactor [] factors;

        [XmlAttribute("argument")]
        public string Argument
        {
            get { return argReference; }
            set { argReference = value; }
        }

        [XmlElement("factor")]
        public ArraySizeFactor [] Factors
        {
            get { return factors; }
            set { factors = value; }
        }
    }

    public class ArraySizeFactor
    {
        private string constant;
        private string argument;

        [XmlAttribute("argument")]
        public string Argument
        {
            get { return argument; }
            set { argument = value; }
        }

        [XmlAttribute("constant")]
        public string Constant
        {
            get { return constant; }
            set { constant = value; }
        }
    }
}
