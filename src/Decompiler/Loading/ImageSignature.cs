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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Loading
{
    public class ImageSignature
    {
        public string? Name;
        public string? Comments;
        public string? EntryPointPattern;
        public string? ImagePattern;

        //$PERF: of course we should compile pattern files into a trie for super performance.
        //$REVIEW: move to ImageSignature class?
        // See https://www.hex-rays.com/products/ida/tech/flirt/in_depth.shtml for implementation
        // ideas.
        public bool Matches(byte[] image, uint entryPointOffset)
        {
            try
            {
                if (entryPointOffset >= image.Length || string.IsNullOrEmpty(this.EntryPointPattern))
                    return false;
                int iImage = (int) entryPointOffset;
                int iPattern = 0;
                while (iPattern < this.EntryPointPattern!.Length - 1 && iImage < image.Length)
                {
                    var msn = this.EntryPointPattern[iPattern];
                    var lsn = this.EntryPointPattern[iPattern + 1];
                    if (msn != '?' && lsn != '?')
                    {
                        if (!BytePattern.TryParseHexDigit(msn, out var ms) ||
                            !BytePattern.TryParseHexDigit(lsn, out var ls))
                            return false;
                        var pat = ms << 4 | ls;
                        var img = image[iImage];
                        if (pat != img)
                            return false;
                    }
                    iImage += 1;
                    iPattern += 2;
                }
                return iPattern == this.EntryPointPattern.Length;
            }
            catch
            {
                Debug.Print("Pattern for '{0}' is unhandled: {1}", this.Name, this.EntryPointPattern);
                return false;
            }
        }

    }
}
