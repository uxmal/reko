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
        /// <returns>Value returned by visitor.</returns>
        T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context);
    }


    /// <summary>
    /// Visitor interface for loaded images.
    /// </summary>
    /// <typeparam name="T">Visitor's return value.</typeparam>
    /// <typeparam name="C">Context parameter.</typeparam>
    public interface ILoadedImageVisitor<T, C>
    {
        /// <summary>
        /// Called when visiting a <see cref="Program"/>.
        /// </summary>
        /// <param name="program">Program being visited.</param>
        /// <param name="context">Context passed by caller.</param>
        /// <returns>Visitor return value.</returns>
        T VisitProgram(Program program, C context);

        /// <summary>
        /// Called when visiting a Reko <see cref="Project"/>.
        /// </summary>
        /// <param name="project">Project being visited.</param>
        /// <param name="context">Context passed by caller.</param>
        /// <returns>Visitor return value.</returns>
        T VisitProject(Project project, C context);

        /// <summary>
        /// Called when visiting a <see cref="IArchive"/>.
        /// </summary>
        /// <param name="archive">Archivebeing visited.</param>
        /// <param name="context">Context passed by caller.</param>
        /// <returns>Visitor return value.</returns>
        /// 
        T VisitArchive(IArchive archive, C context);

        /// <summary>
        /// Called when visiting a <see cref="Blob"/>.
        /// </summary>
        /// <param name="blob">Blob being visited.</param>
        /// <param name="context">Context passed by caller.</param>
        /// <returns>Visitor return value.</returns>
        T VisitBlob(Blob blob, C context);
    }
}
