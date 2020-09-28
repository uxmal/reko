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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Reko.Core
{
    /// <summary>
    /// Reads XML into a Dictionary&lt;string,object&gt; (JSON envy!)
    /// Handles the following XML as follows:
    /// &lt;dict&gt;
    ///   &lt;item key="UK"&gt;United Kingdom&lt/item&gt;
    ///   &lt;item key="FR"&gt;France&lt;/item&gt;
    /// &lt;/dict&gt;
    /// </summary>
    public class XmlOptions
    {
        private static object ReadItem(XmlElement element,
            StringComparer comparer)
        {
            if (element.Name == "item")
            {
                return element.InnerText;
            }
            else if (element.Name == "list")
            {
                return element.ChildNodes
                    .OfType<XmlElement>()
                    .Select(e => ReadItem(e, comparer))
                    .ToList();
            }
            else if (element.Name == "dict")
            {
                return ReadDictionaryElements(element.ChildNodes.OfType<XmlElement>(), comparer);
            }
            throw new NotSupportedException(element.Name);
        }

        private static Dictionary<string, object> ReadDictionaryElements(
            IEnumerable<XmlElement> elements,
            StringComparer comparer)
        {
            return elements.ToDictionary(
                e => e.Attributes["key"]?.Value,
                e => ReadItem(e, comparer),
                comparer);
        }

        public static Dictionary<string, object> LoadIntoDictionary(XmlElement[] options, StringComparer comparer)
        {
            if (options == null)
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            return ReadDictionaryElements(options, comparer);
        }
    }
}
