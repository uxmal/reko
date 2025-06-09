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

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Reko.Loading
{
    public class FlirtoidSignatureLoader : SignatureLoader
    {
        public override IEnumerable<ImageSignature> Load(string filename)
        {
            // Expect rows of the format "<even number of hex digits or dots><spaces><name>
            var re = new Regex("([.0-9a-f]+)[ \t]+([^ ]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            using (var stm = CreateReader(filename))
            {
                var rdr = new StreamReader(stm, Encoding.UTF8);
                string? line = rdr.ReadLine();
                while (line is not null)
                {
                    var m = re.Match(line);
                    if (m.Success && (m.Groups[1].Value.Length & 1) == 0)
                    {
                        yield return new ImageSignature
                        {
                            ImagePattern = m.Groups[1].Value,
                            Name = m.Groups[2].Value,
                        };
                    }
                    line = rdr.ReadLine();
                }
            }
        }

        public virtual Stream CreateReader(string filename)
        {
            return File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}
