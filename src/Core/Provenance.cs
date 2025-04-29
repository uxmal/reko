#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

namespace Reko.Core
{
    /// <summary>
    /// The provenance of a basic block or procedure is evidence of how it
    /// arrived to be deduced.
    /// </summary>
    public class Provenance
    {
        /// <summary>
        /// The origin of the object.
        /// </summary>
        public ProvenanceType ProvenanceType { get; }

        /// <summary>
        /// Optional comment about the origin.
        /// </summary>
        public string? Comment { get; }
    }

    /// <summary>
    /// An enumeration of the possible origins of objects discovered during analysis.
    /// </summary>
    public enum ProvenanceType
    {
        /// <summary>
        /// Default uninitialized value.
        /// </summary>
        None,


        /// <summary>
        /// The object's presence is determined by the image. Confidence is very high.
        /// </summary>
        Image,
        /// <summary>
        /// An image defined this as the entry point. Confidence is very high.
        /// </summary>
        ImageEntrypoint,
        /// <summary>
        /// The presence of this object is determined from envionment metadata. Confidence is very high.
        /// </summary>
        Environment,

        /// <summary>
        /// The presece of this object reached here because user's input dictated it.
        /// We blindly trust the user.
        /// </summary>
        UserInput,

        /// <summary>
        /// The object was discovered by recursive scanning. Confidence is high.
        /// </summary>
        Scanning,
        /// <summary>
        /// The object was discovered by using a heuristic or guessing. Confidence is low.
        /// </summary>
        Heuristic,          
    }

    /// <summary>
    /// A generic version of <see cref="Provenance"/> that allows for tracing the origin
    /// of an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Provenance<T> : Provenance
    {

        /// <summary>
        /// Creates an instance of a "tracking" provenance.
        /// </summary>
        /// <param name="from">The object from which this provenance was discovered.
        /// </param>
        public Provenance(T from)
        {
            this.ReachedFrom = from;
        }

        /// <summary>
        /// The object from which this provenance was discovered.
        /// </summary>
        public T ReachedFrom { get; }

    }
}
