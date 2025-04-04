#region License
/* Copyright (C) 1999-2025 John Källén.
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

namespace Reko.Core.Loading
{
    /// <summary>
    /// This interface is exposed by loaded objects whose format Reko has discovered.
    /// </summary>
    public interface ILoadedImage
    {
        /// <summary>
        /// The (absolute) file location from which this object was loaded.
        /// </summary>
        /// <remarks>
        /// Reko introduces the 'archive:' schema to model file paths within
        /// (possibly nested) archives. This is accomplished by using the '#'
        /// character to separate path segments within the respective
        /// archives.
        /// The syntax is as follows:
        /// <list type="bullet">
        /// <item>
        /// A simple executable file test.dll in the folder c:\myprograms:
        ///     <para><c>archive:///c:/myprograms/test.dll</c></para>
        /// </item> 
        /// <item>
        /// An executable file test.so inside a TAR archive:
        /// <para><c>archive:///home/username/archive.tar#lib/test.so</c></para>
        /// </item>
        /// <item>An executable file test.exe inside two nested archives:
        /// <para><c>archive:///home/username/outer.tar#archives/inner.tar#test.exe</c></para>
        /// </item> 
        /// </list>
        /// </remarks>
        ImageLocation Location { get; }

        /// <summary>
        /// Accepts a class implementing the <see cref="ILoadedImageVisitor{T, C}"/>
        /// interface as a visitor to this object.
        /// </summary>
        /// <typeparam name="T">Return type from the visit.</typeparam>
        /// <typeparam name="C">Context provided by the caller.</typeparam>
        /// <param name="visitor">Visitor object.</param>
        /// <param name="context">Any relevant context.</param>
        /// <returns></returns>
        T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context);
    }

    /// <summary>
    /// Visitor interface for loaded images.
    /// </summary>
    /// <typeparam name="T">Visitor return type.</typeparam>
    /// <typeparam name="C">Type of any context provided by the caller.</typeparam>
    public interface ILoadedImageVisitor<T, C>
    {
        /// <summary>
        /// This method is called when the visitor visits a <see cref="Program"/>.
        /// </summary>
        /// <param name="program">The visited <see cref="Program"/>.</param>
        /// <param name="context">Relevant context, if any.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        T VisitProgram(Program program, C context);

        /// <summary>
        /// This method is called when the visitor visits a <see cref="Project"/>.
        /// </summary>
        /// <param name="project">The visited <see cref="Project"/>.</param>
        /// <param name="context">Relevant context, if any.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        T VisitProject(Project project, C context);

        /// <summary>
        /// This method is called when the visitor visits an <see cref="IArchive"/>.
        /// </summary>
        /// <param name="archive">The visited <see cref="IArchive"/>.</param>
        /// <param name="context">Relevant context, if any.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        T VisitArchive(IArchive archive, C context);

        /// <summary>
        /// This method is called when the visitor visits a <see cref="Blob"/>.
        /// </summary>
        /// <param name="blob">The visited <see cref="Blob"/>.</param>
        /// <param name="context">Relevant context, if any.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        T VisitBlob(Blob blob, C context);
    }
}
