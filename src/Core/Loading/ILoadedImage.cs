#region License
/* Copyright (C) 1999-2021 John Källén.
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
        /// - A simple executable file test.dll in the folder c:\myprograms:
        ///     archive:///c:/myprograms/test.dll
        /// - An executable file test.so inside a TAR archive:
        ///     archive:///home/username/archive.tar#lib/test.so
        /// - An executable file test.exe inside two nested archives:
        ///     archive:///home/username/outer.tar#archives/inner.tar#test.exe
        /// </remarks>
        ImageLocation Location { get; }

        T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context);
    }

    public interface ILoadedImageVisitor<T, C>
    {
        T VisitProgram(Program program, C context);
        T VisitProject(Project project, C context);
        T VisitArchive(IArchive archive, C context);
        T VisitBlob(Blob blob, C context);
    }
}
