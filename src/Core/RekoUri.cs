#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Special-case class for Reko-style URIs.
    /// </summary>
    /// <remarks>
    /// Reko-style URI's are similar to the 'file:' scheme, but extend it by
    /// allowing 0 or more fragments to follow the file name. Fragments are separated
    /// by '#' characters (U+0023). The fragments of the URI are URL-encoded, which allows
    /// files and fragments to contain the '#' character.
    /// <para>
    /// I'm not certain the Reko-style URIs are conformant to the URI spec,
    /// so they are in this class for now. They could be moved to 
    /// <see cref="System.Uri"/> in the future.
    /// </para>
    /// </remarks>
    public class RekoUri : IComparable<RekoUri>
    {
        //$TODO: can this class be replaced with System.Uri?
        private string sUri;

        public RekoUri(string uri)
        {
            this.sUri = uri;
        }

        public int CompareTo(RekoUri that) => this.sUri.CompareTo(that.sUri);

        public bool EndsWith(string s) => sUri.EndsWith(s);

        public bool EndsWith(string s, StringComparison c) => sUri.EndsWith(s, c);

        public string ExtractString() => sUri;

        public override bool Equals(object obj)
        {
            if (obj is RekoUri that)
            {
                return this.sUri == that.sUri;
            }
            return false;
        }

        public override int GetHashCode() => sUri.GetHashCode();

        public override string ToString() => sUri;
    }
}
