#region License
/* 
 * Copyright (C) 1999-2025 Pavel Tomin.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.Collections
{
    /// <summary>
    /// A collectionof <see cref="Annotation"/>s.
    /// </summary>
    public class AnnotationList : IEnumerable<Annotation>
    {
        /// <summary>
        /// Event that is raised when the list of annotations changes.
        /// </summary>
        public event EventHandler? AnnotationChanged;

        private readonly Dictionary<Address, string> annotations;

        /// <summary>
        /// Constructs an empty <see cref="AnnotationList"/>.
        /// </summary>
        public AnnotationList()
        {
            annotations = [];
        }

        /// <summary>
        /// Constructs an <see cref="AnnotationList"/> from a collection of 
        /// <see cref="Annotation"/>s.
        /// </summary>
        /// <param name="annotations"></param>
        public AnnotationList(IEnumerable<Annotation> annotations) : this()
        {
            foreach (var annotation in annotations)
                this.annotations.Add(annotation.Address, annotation.Text);
        }

        /// <summary>
        /// Stores an annotation for the specified address.
        /// </summary>
        /// <param name="addr">Address at which to place the annotation.</param>
        public string? this[Address addr]
        {
            get
            {
                if (!annotations.TryGetValue(addr, out var text))
                    return null;
                return text;
            }
            set
            {
                if (value is null)
                    annotations.Remove(addr);
                else
                    annotations[addr] = value;
                AnnotationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Removes the annotation at the specified address.
        /// </summary>
        /// <param name="addr">Address to remove.</param>
        public void Remove(Address addr)
        {
            annotations.Remove(addr);
            AnnotationChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public IEnumerator<Annotation> GetEnumerator()
        {
            return annotations
                .Select(a => new Annotation(a.Key, a.Value))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
