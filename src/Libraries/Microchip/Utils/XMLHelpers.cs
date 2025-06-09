#region License
/* 
 *
 * Copyrighted (c) 2017-2025 Christian Hostelet.
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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using System.IO;
    using System.Text;

    #region Interface

    /// <summary>
    /// Interface for attribute value transformer.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IAttributeValueTransformer<T>
    {
        /// <summary>
        /// Convert from a string representation of the object.
        /// </summary>
        /// <param name="paramString">The parameter value as a string.</param>
        /// <returns>
        /// Value converted.
        /// </returns>
        T ConvertFrom(string paramString);

        /// <summary>
        /// Convert this object into a string representation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A string that represents this object.
        /// </returns>
        string ToString(T obj);

    }

    #endregion


    /// <summary>
    /// A class providing some <see cref="XElement"/> extensions methods for manipulating Microchip Crownking definitions.
    /// </summary>
    public static class XMLHelpers
    {
        #region Helpers

        private static XmlSerializer getSerializer<T>(ref XElement xelem, string sdescendantName = null)
        {
            if (sdescendantName is null)
                return new XmlSerializer(typeof(T), xelem.Name.NamespaceName);

            // Look for a descendant of this element making it the root element.
            xelem = xelem.Descendants().Where((x) => x.Name.LocalName == sdescendantName).FirstOrDefault();
            if (xelem is null)
                return null;

            var snamespace = xelem.GetDefaultNamespace().NamespaceName;
            var stypename = typeof(T).ToString();

            var xAttribs = new XmlAttributes
            {
                XmlType = new XmlTypeAttribute { Namespace = snamespace, TypeName = stypename },
                XmlRoot = new XmlRootAttribute { Namespace = snamespace, DataType = stypename }
            };

            var xOverrides = new XmlAttributeOverrides();
            xOverrides.Add(typeof(T), xAttribs);

            var extraTypes = new Type[] { typeof(T) };
            return new XmlSerializer(typeof(T), xOverrides, extraTypes, xAttribs.XmlRoot, snamespace);
        }

        private static float str2Float(this string str) => Convert.ToSingle(str, CultureInfo.InvariantCulture);

        #endregion

        #region Extensions methods

        #region XLINQ extensions methods

        /// <summary>
        /// An XElement extension method that query if <paramref name="xelem"/> has an attribute of given name in given
        /// namespace.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="sLocalName">The local name of the attribute.</param>
        /// <param name="ns">The namespace.</param>
        /// <returns>
        /// True if attribute exists, false if not.
        /// </returns>
        public static bool HasAttribute(this XElement xelem, string sLocalName, XNamespace ns)
            => xelem.Attribute(ns + sLocalName) is not null;

        /// <summary>
        /// An XElement extension method that query if <paramref name="xelem"/> has an attribute of given name in <paramref name="xelem"/> namespace.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="sLocalName">The local name of the attribute.</param>
        /// <returns>
        /// True if attribute exists, false if not.
        /// </returns>
        public static bool HasAttribute(this XElement xelem, string sLocalName)
            => xelem.Attribute(xelem.Name.Namespace + sLocalName) is not null;

        /// <summary>
        /// Gets the value of the attribute with specified local name within node's namespace URI.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute's local name to look for.</param>
        /// <param name="ns">The namespace.</param>
        /// <returns>
        /// A string.
        /// </returns>
        public static string Get(this XElement xelem, string strLocalName, XNamespace ns)
            => xelem.Attribute(ns + strLocalName).Value;

        /// <summary>
        /// Gets the value of the attribute with specified local name within node's namespace URI.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute's local name to look for.</param>
        /// <returns>
        /// A string.
        /// </returns>
        public static string Get(this XElement xelem, string strLocalName)
            => xelem.Attribute(xelem.Name.Namespace + strLocalName).Value;

        /// <summary>
        /// Gets the value of the attribute with the specified local name and namespace URI or returns
        /// default value if not found.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute's local name to look for.</param>
        /// <param name="strNSURI">The namespace URI.</param>
        /// <param name="strDefault">The default value.</param>
        /// <returns>
        /// The attribute's value or the default one.
        /// </returns>
        public static string GetElse(this XElement xelem, string strLocalName, XNamespace ns, string strDefault)
            => (xelem.HasAttribute(strLocalName, ns) ? xelem.Get(strLocalName, ns) : strDefault);

        /// <summary>
        /// Gets the value of given locally named attribute within node's namespace URI or returns a default value if not found.
        /// </summary>
        /// <param name="xelem">This <see cref="XmlElement"/> node to act on.</param>
        /// <param name="strLocalName">The attribute's local name to look for.</param>
        /// <param name="strDefault">The default value.</param>
        /// <returns>
        /// The attribute's value or the default one.
        /// </returns>
        public static string GetElse(this XElement xelem, string strLocalName, string strDefault)
            => xelem.GetElse(strLocalName, xelem.Name.Namespace, strDefault);

        /// <summary>
        /// Gets the value of specified attribute as an integer within node's namespace URI.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <returns>
        /// The integer value of the attribute.
        /// </returns>
        public static int GetAsInteger(this XElement xelem, string strLocalName)
            => xelem.Get(strLocalName).ToInt32Ex();

        /// <summary>
        /// Gets the value of attribute with given name and namespace URI as an integer.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <param name="ns">The namespace URI.</param>
        /// <returns>
        /// The integer value of the attribute.
        /// </returns>
        public static int GetAsInteger(this XElement xelem, string strLocalName, XNamespace ns)
            => xelem.Get(strLocalName, ns).ToInt32Ex();

        /// <summary>
        /// Gets the value of specified attribute as a nullable integer within node's namespace URI.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <returns>
        /// The nullable integer value of the attribute.
        /// </returns>
        public static int? GetAsNullableInteger(this XElement xelem, string strLocalName)
            => xelem.Get(strLocalName).ToNullableInt32Ex();

        /// <summary>
        /// Gets the value of attribute with given name and namespace URI as a nullable integer.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <param name="ns">The namespace URI.</param>
        /// <returns>
        /// The nullable integer value of the attribute.
        /// </returns>
        public static int? GetAsNullableInteger(this XElement xelem, string strLocalName, XNamespace ns)
            => xelem.Get(strLocalName, ns).ToNullableInt32Ex();

        /// <summary>
        /// Gets the value of attribute  with given name and namespace URI as an integer or a default value.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <param name="ns">The namespace URI.</param>
        /// <param name="nDefault">The default value.</param>
        /// <returns>
        /// The integer value of the attribute or specified default value.
        /// </returns>
        public static int GetAsIntegerElse(this XElement xelem, string strLocalName, XNamespace ns, int nDefault)
            => (xelem.HasAttribute(strLocalName, ns) ? xelem.Get(strLocalName, ns).ToInt32Ex() : nDefault);

        /// <summary>
        /// Gets the value of attribute with given name in node's namespace URI as an integer or a default
        /// value.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <param name="nDefault">The default value.</param>
        /// <returns>
        /// The integer value of the attribute or specified default value.
        /// </returns>
        public static int GetAsIntegerElse(this XElement xelem, string strLocalName, int nDefault)
            => (xelem.HasAttribute(strLocalName) ? xelem.Get(strLocalName).ToInt32Ex() : nDefault);

        /// <summary>
        /// Gets the value of specified attribute as a boolean within node's namespace URI.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <returns>
        /// The boolean value of the attribute.
        /// </returns>
        public static bool GetAsBoolean(this XElement xelem, string strLocalName)
            => xelem.Get(strLocalName).ToBooleanEx();

        /// <summary>
        /// Gets the value of attribute  with given name and namespace URI as a boolean.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <param name="ns">The namespace URI.</param>
        /// <returns>
        /// The boolean value of the attribute.
        /// </returns>
        public static bool GetAsBoolean(this XElement xelem, string strLocalName, XNamespace ns)
            => xelem.Get(strLocalName, ns).ToBooleanEx();

        /// <summary>
        /// Gets the value of specified attribute as a nullable boolean with node's namespace URI.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <returns>
        /// The nullable boolean value of the attribute.
        /// </returns>
        public static bool? GetAsNullableBoolean(this XElement xelem, string strLocalName)
            => xelem.Get(strLocalName)?.ToBooleanEx();

        /// <summary>
        /// Gets the value of attribute with given name and namespace URI as a nullable boolean.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <param name="ns">The namespace URI.</param>
        /// <returns>
        /// The nullable boolean value of the attribute.
        /// </returns>
        public static bool? GetAsNullableBoolean(this XElement xelem, string strLocalName, XNamespace ns)
            => xelem.Get(strLocalName, ns)?.ToBooleanEx();

        /// <summary>
        /// Gets the value of attribute  with given name and namespace URI as a boolean or a default value.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <param name="ns">The namespace URI.</param>
        /// <param name="bDefault">The default value.</param>
        /// <returns>
        /// The boolean value of the attribute or specified default value.
        /// </returns>
        public static bool GetAsBooleanElse(this XElement xelem, string strLocalName, XNamespace ns, bool bDefault)
            => (xelem.HasAttribute(strLocalName, ns) ? xelem.Get(strLocalName, ns).ToBooleanEx() : bDefault);

        /// <summary>
        /// Gets the value of attribute with given name in node's namespace URI as a boolean or a default
        /// value.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <param name="bDefault">The default value.</param>
        /// <returns>
        /// The boolean value of the attribute or specified default value.
        /// </returns>
        public static bool GetAsBooleanElse(this XElement xelem, string strLocalName, bool bDefault)
            => (xelem.HasAttribute(strLocalName) ? xelem.Get(strLocalName).ToBooleanEx() : bDefault);

        /// <summary>
        /// Gets the value of specified attribute as a float within node's namespace URI.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <returns>
        /// The float value of the attribute.
        /// </returns>
        public static float GetAsFloat(this XElement xelem, string strLocalName)
            => xelem.Get(strLocalName).str2Float();

        /// <summary>
        /// Gets the value of attribute  with given name and namespace URI as a float.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <param name="ns">The namespace URI.</param>
        /// <returns>
        /// The float value of the attribute.
        /// </returns>
        public static float GetAsFloat(this XElement xelem, string strLocalName, XNamespace ns)
            => xelem.Get(strLocalName, ns).str2Float();

        /// <summary>
        /// Gets the value of specified attribute as a nullable float with node's namespace URI.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <returns>
        /// The nullable float value of the attribute.
        /// </returns>
        public static float? GetAsNullableFloat(this XElement xelem, string strLocalName)
            => xelem.Get(strLocalName)?.str2Float();

        /// <summary>
        /// Gets the value of attribute with given name and namespace URI as a nullable float.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <param name="ns">The namespace URI.</param>
        /// <returns>
        /// The nullable float value of the attribute.
        /// </returns>
        public static float? GetAsNullableFloat(this XElement xelem, string strLocalName, XNamespace ns)
            => xelem?.Get(strLocalName, ns)?.str2Float();

        /// <summary>
        /// Gets the value of attribute  with given name and namespace URI as a float or a default value.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <param name="ns">The namespace URI.</param>
        /// <param name="fDefault">The default value.</param>
        /// <returns>
        /// The float value of the attribute or specified default value.
        /// </returns>
        public static float GetAsFloatElse(this XElement xelem, string strLocalName, XNamespace ns, float fDefault)
            => (xelem.HasAttribute(strLocalName, ns) ? xelem.Get(strLocalName, ns).str2Float() : fDefault);

        /// <summary>
        /// Gets the value of attribute with given name in node's namespace URI as a float or a default
        /// value.
        /// </summary>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="strLocalName">The attribute local name to look for.</param>
        /// <param name="fDefault">The default value.</param>
        /// <returns>
        /// The float value of the attribute or specified default value.
        /// </returns>
        public static float GetAsFloatElse(this XElement xelem, string strLocalName, float fDefault)
            => (xelem.HasAttribute(strLocalName) ? xelem.Get(strLocalName).str2Float() : fDefault);

        /// <summary>
        /// Gets the value of the attribute with the specified local name and <paramref name="xelem"/> namespace.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="attrLocalName">Name of the attribute local.</param>
        /// <param name="transformer">The transformer to/from T object.</param>
        /// <returns>
        /// The attribute's value as a string.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        public static T Get<T>(this XElement xelem, string attrLocalName, IAttributeValueTransformer<T> transformer)
        {
            if (transformer is null)
                throw new ArgumentNullException(nameof(transformer));
            return transformer.ConvertFrom(xelem.Get(attrLocalName));
        }

        /// <summary>
        /// Gets the value of the attribute with the specified local name and namespace.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="attrLocalName">Name of the attribute local.</param>
        /// <param name="ns">The namespace.</param>
        /// <param name="transformer">The transformer to/from T object.</param>
        /// <returns>
        /// The attribute's value as a string.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        public static T Get<T>(this XElement xelem, string attrLocalName, XNamespace ns, IAttributeValueTransformer<T> transformer)
        {
            if (transformer is null)
                throw new ArgumentNullException(nameof(transformer));
            return transformer.ConvertFrom(xelem.Get(attrLocalName, ns));
        }

        /// <summary>
        /// Gets the value of the attribute with the specified local name and namespace URI.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="attrLocalName">Name of the attribute local.</param>
        /// <param name="transformer">The transformer to/from T object.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The attribute's value as a string.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        public static T Get<T>(this XElement xelem, string attrLocalName, IAttributeValueTransformer<T> transformer, T defaultValue)
        {
            if (transformer is null)
                throw new ArgumentNullException(nameof(transformer));
            return (xelem.HasAttribute(attrLocalName) ? transformer.ConvertFrom(xelem.Get(attrLocalName)) : defaultValue);
        }

        /// <summary>
        /// Gets the value of the attribute with the specified local name and namespace URI.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="xelem">The element to act on.</param>
        /// <param name="attrLocalName">Name of the attribute local.</param>
        /// <param name="ns">The namespace.</param>
        /// <param name="transformer">The transformer to/from T object.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The attribute's value as a string.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        public static T Get<T>(this XElement xelem, string attrLocalName, XNamespace ns, IAttributeValueTransformer<T> transformer, T defaultValue)
        {
            if (transformer is null)
                throw new ArgumentNullException(nameof(transformer));
            return (xelem.HasAttribute(attrLocalName, ns) ? transformer.ConvertFrom(xelem.Get(attrLocalName, ns)) : defaultValue);
        }

        /// <summary>
        /// Sets the string value of attribute with the specified local name within the element's namespace URI .
        /// </summary>
        /// <param name="xelem">The <see cref="XmlElement"/> node to act on.</param>
        /// <param name="localName">The local name of the new element.</param>
        /// <param name="value">The string value.</param>
        public static void PutAttribute(this XElement xelem, string localName, string value)
            => xelem.SetAttributeValue(xelem.Name.Namespace + localName, value);

        /// <summary>
        /// Sets the string value of attribute with the specified local name with the specified namespace
        /// </summary>
        /// <param name="xelem">The <see cref="XmlElement"/> node to act on.</param>
        /// <param name="localName">The local name of the new element.</param>
        /// <param name="ns">The namespace.</param>
        /// <param name="value">The string value.</param>
        public static void PutAttribute(this XElement xelem, string localName, XNamespace ns, string value)
            => xelem.SetAttributeValue(ns + localName, value);

        /// <summary>
        /// Sets the boolean value of attribute with the specified local name within the element's namespace URI .
        /// </summary>
        /// <param name="xelem">The <see cref="XmlElement"/> node to act on.</param>
        /// <param name="localName">The local name of the new element.</param>
        /// <param name="value">The boolean value.</param>
        public static void PutAttribute(this XElement xelem, string localName, bool value)
            => xelem.SetAttributeValue(xelem.Name.Namespace + localName, value.ToString());

        /// <summary>
        /// Sets the boolean value of attribute with the specified local name within the specified
        /// namespace.
        /// </summary>
        /// <param name="xelem">The <see cref="XmlElement"/> node to act on.</param>
        /// <param name="localName">The local name of the new element.</param>
        /// <param name="ns">The namespace.</param>
        /// <param name="value">The boolean value.</param>
        public static void PutAttribute(this XElement xelem, string localName, XNamespace ns, bool value)
            => xelem.SetAttributeValue(ns + localName, value.ToString());

        /// <summary>
        /// Sets the integer value of attribute with the specified local name within the node's namespace URI .
        /// </summary>
        /// <param name="xelem">The <see cref="XmlElement"/> node to act on.</param>
        /// <param name="localName">The local name of the new element.</param>
        /// <param name="value">The integer value.</param>
        public static void PutAttribute(this XElement xelem, string localName, int value)
            => xelem.SetAttributeValue(xelem.Name.Namespace + localName, value.ToString());

        /// <summary>
        /// Sets the integer value of attribute with the specified local name within the specified
        /// namespace.
        /// </summary>
        /// <param name="xelem">The <see cref="XmlElement"/> node to act on.</param>
        /// <param name="localName">The local name of the new element.</param>
        /// <param name="ns">The namespace.</param>
        /// <param name="value">The integer value.</param>
        public static void PutAttribute(this XElement xelem, string localName, XNamespace ns, int value)
            => xelem.SetAttributeValue(ns + localName, value.ToString());

        /// <summary>
        /// Sets the value of attribute with the specified local name within the node's namespace URI .
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="xelem">This <see cref="XmlElement"/> node to act on.</param>
        /// <param name="localName">The local name of the new element.</param>
        /// <param name="value">The string value.</param>
        /// <param name="transformer">The transformer.</param>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        public static void PutAttribute<T>(this XElement xelem, string localName, T value, IAttributeValueTransformer<T> transformer)
        {
            if (transformer is null)
                throw new ArgumentNullException(nameof(transformer));
            xelem.SetAttributeValue(xelem.Name.Namespace + localName, transformer.ToString(value));
        }

        /// <summary>
        /// Sets the value of attribute with the specified local name within the specified namespace.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="xelem">This <see cref="XmlElement"/> node to act on.</param>
        /// <param name="localName">The local name of the new element.</param>
        /// <param name="ns">The namespace.</param>
        /// <param name="value">The string value.</param>
        /// <param name="transformer">The transformer.</param>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        public static void PutAttribute<T>(this XElement xelem, string localName, XNamespace ns, T value, IAttributeValueTransformer<T> transformer)
        {
            if (transformer is null)
                throw new ArgumentNullException(nameof(transformer));
            xelem.SetAttributeValue(ns + localName, transformer.ToString(value));
        }

        /// <summary>
        /// An XDocument extension method that creates and attaches a new <see cref="XElement"/> for
        /// serializing the specified object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="xdoc">The <see cref="XDocument"/> to act on.</param>
        /// <param name="parent">(Optional) The parent <see cref="XNode"/> node.</param>
        /// <returns>
        /// A new XElement.
        /// </returns>
        public static XElement AttachNewElement<T>(this XDocument xdoc, XNode parent = null)
        {
            var xelem = new XElement(typeof(T).Name);
            if (parent is null)
                xdoc.Add(xelem);
            else
                parent.AddAfterSelf(xelem);
            return xelem;
        }

        /// <summary>
        /// Enumerates descendant elements in this XML document given descendant local name.
        /// </summary>
        /// <param name="xDoc">The <see cref="XDocument"/> tree to act on.</param>
        /// <param name="nodeLocalName">The local name of the node.</param>
        /// <returns>
        /// An enumerator that allows <code>foreach</code> to be used to process descendant elements in this
        /// collection.
        /// </returns>
        public static IEnumerable<XElement> DescendantElements(this XDocument xDoc, string nodeLocalName)
            => xDoc.Descendants().Where(p => p.Name.LocalName == nodeLocalName);

        /// <summary>
        /// Enumerates descendant elements in this XML element given descendant local name.
        /// </summary>
        /// <param name="xElem">The <see cref="XElement"/> tree to act on.</param>
        /// <param name="nodeName">Local name of the node.</param>
        /// <returns>
        /// An enumerator that allows <code lang="C#">foreach</code> to be used to process descendant elements in this
        /// collection.
        /// </returns>
        public static IEnumerable<XElement> DescendantElements(this XElement xElem, string nodeName)
            => xElem.Descendants().Where(p => p.Name.LocalName == nodeName);

        /// <summary>
        /// An XElement extension method that converts this XML tree (or starting at its descendant) to an object.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the converted object. Must be serializable.</typeparam>
        /// <param name="xelem">The <see cref="XElement"/> tree to act on.</param>
        /// <param name="sDescendantLocalName">(Optional) Local name of the descendant.</param>
        /// <returns>
        /// The given XML data converted to an object of type T.
        /// </returns>
        public static T ToObject<T>(this XElement xelem, string sDescendantLocalName = null)
        {
            if (xelem is null)
                return default(T);
            try
            {
                XmlSerializer xmlsr = getSerializer<T>(ref xelem, sDescendantLocalName);
                if (xmlsr is null)
                    return default(T);
                return (T) xmlsr.Deserialize(xelem.CreateReader());
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(ex.StackTrace);
                return default(T);
            }
        }

        /// <summary>
        /// An XDocument extension method that converts this XML tree (or starting at its descendant) to
        /// an object.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="xdoc">The <see cref="XDocument"/> to act on.</param>
        /// <param name="sDescendantLocalName">(Optional) Local name of the descendant.</param>
        /// <returns>
        /// The given XML data converted to an object of type T.
        /// </returns>
        public static T ToObject<T>(this XDocument xdoc, string sDescendantLocalName = null)
            => xdoc.Root.ToObject<T>();

        /// <summary>
        /// An extension method that converts an object of type T to an XElement tree.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="obj">The object to convert.</param>
        /// <param name="sPrefix">(Optional) The prefix.</param>
        /// <param name="sNamespace">(Optional) The namespace.</param>
        /// <returns>
        /// The given object converted to an XElement.
        /// </returns>
        public static XElement ToXElement<T>(this T obj, string sPrefix = null, string sNamespace = null)
        {
            try
            {
                var ns = new XmlSerializerNamespaces();
                if (!string.IsNullOrEmpty(sPrefix) && !string.IsNullOrEmpty(sNamespace))
                    ns.Add(sPrefix, sNamespace);
                else
                    ns.Add("", "");
                var xdoc = new XDocument();
                using (XmlWriter wr = xdoc.CreateWriter())
                {
                    new XmlSerializer(obj.GetType()).Serialize(wr, obj, ns);
                }
                return xdoc.Root;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"{ex.StackTrace}");
                return default(XElement);
            }
        }

        public static T FromXElement<T>(this XElement xElement, string sNamespae = null)
        {
            var result = default(T);
            try
            {
                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xElement.ToString()), false))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T), sNamespae);
                    result = (T) xmlSerializer.Deserialize(memoryStream);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Failed to translate from object type '{typeof(T).FullName}' to XElement");
                Trace.TraceError($"Exception is: {ex.StackTrace}");
            }
            return result;

        }

        #endregion

        #endregion

    }
}
