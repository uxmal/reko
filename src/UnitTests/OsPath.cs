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

namespace Reko.UnitTests
{
    /// <summary>
    /// Encapsulates platform differences to make unit tests pass.
    /// </summary>
    public static class OsPath
    {
        /// <summary>
        /// Create an absolute path obeying the rules of the platform.
        /// </summary>
        /// <param name="pathSegments"></param>
        /// <returns></returns>
        public static string Absolute(params string [] pathSegments)
        {
            switch (Environment.OSVersion.Platform)
            {
            case PlatformID.Unix:
                return "/" + string.Join("/", pathSegments);
            case PlatformID.Win32NT:
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
            case PlatformID.WinCE:
                return "c:\\" + string.Join("\\", pathSegments);
            default:
                throw new NotSupportedException("Platform " + Environment.OSVersion.Platform + " not supported");
            }
        }

        /// <summary>
        /// Create a relative path obeying the rules of the platform.
        /// </summary>
        /// <param name="pathSegments"></param>
        /// <returns></returns>
        public static string Relative(params string[] pathSegments)
        {
            switch (Environment.OSVersion.Platform)
            {
            case PlatformID.Unix:
                return string.Join("/", pathSegments);
            case PlatformID.Win32NT:
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
            case PlatformID.WinCE:
                return string.Join("\\", pathSegments);
            default:
                throw new NotSupportedException("Platform " + Environment.OSVersion.Platform + " not supported");
            }
        }

    }
}
