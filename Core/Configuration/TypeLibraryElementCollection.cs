#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System.Configuration;
using System.Linq;
using System.Text;

namespace Decompiler.Core.Configuration
{
    public class TypeLibraryElementCollection  : ConfigurationElementCollection
    {
        public TypeLibraryElementCollection()
        {
        }

        public TypeLibraryElement this[int index]
        {
            get { return (TypeLibraryElement) this.BaseGet(index); }
        }


        public void Add(TypeLibraryElement element)
        {
            LockItem = false;  // the workaround
            BaseAdd(element);
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override bool IsElementName(string elementName)
        {
            throw new NotImplementedException();
        }

        protected override ConfigurationElement CreateNewElement()
        {
 	        return new TypeLibraryElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TypeLibraryElement) element).Name;
        }

        protected override string ElementName { get { return "TypeLibrary"; } }
    }
}
