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
#if DEBUG
using DocoptNet;
using Reko.Core;
using Reko.ImageLoaders.Hunk;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Reko.Tools.HunkTool
{
	[TestFixture]
    public class SignatureGeneratorTests
    {
        private static string nl = Environment.NewLine;

		private static byte[] MakeBytes(string str)
        {
            return Regex.Replace(str, "[ \t\r\n]+", "")
                .Chunks(2)
                .Select(s => byte.Parse(
					string.Join("", s),
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture))
                .ToArray();
        }

		[Test]
		public void hunktool_signature()
        {
            // # 48  UNIT  
            //#3   CODE  size 00000000  alloc size 00000028  file header @  'romhunks'
            //       48 E7 00 3A 26 6F 00 14 20 6F 00 18 43 EF 00 1C
            //       45 FA 00 12 2C 79 00 00 00 00 4E AE 00 00 4C DF
            //       5C 00 4E 75 16 C0 4E 75
            //	ext       ext
            //		00000000  _sprintf                          def
            //		00000016  _AbsExecBase                      absref32
            //		0000001C  _LVORawDoFmt                      relref16
            //	symbol    symbol
            //		00000000  _sprintf                          
            //		00000024  stuffChar  

            var unit = new Unit
            {
                segments = new List<List<Hunk>>
                {
                    new List<Hunk>
                    {
                        new Hunk
                        {
                            HunkType = HunkType.HUNK_CODE,
                            Data = MakeBytes(
                                "48 E7 00 3A 26 6F 00 14 20 6F 00 18 43 EF 00 1C" +
                                "45 FA 00 12 2C 79 00 00 00 00 4E AE 00 00 4C DF" +
                                "5C 00 4E 75 16 C0 4E 75"),
                        },
                        new ExtHunk
                        {
                            ext_def = new List<ExtObject>
                            {
                                new ExtObject { def = 0, type = ExtType.EXT_DEF, name = "_sprintf" }
                            },
                            ext_ref = new List<ExtObject>
                            {
                                new ExtObject {type = ExtType.EXT_ABSREF32, name= "_AbsExecBase", refs = new List<uint> { 0x16 } },
                                new ExtObject {type = ExtType.EXT_RELREF16, name= "_LVORawDoFmt", refs = new List<uint> { 0x1C } }
                            }
                        }
                    }
                }
            };

            var file = new HunkFile
            {
				type = FileType.TYPE_UNIT,
                units = new List<Unit> { unit }
            };
            
            var sw = new StringWriter();
            var segs = new SignatureGenerator(new Dictionary<string, ValueObject>());
            segs.Output = sw;
            segs.handle_hunk_file("foo.lib", file);
            var sGen = sw.ToString();
            var sExp =
"48E7003A266F0014206F001843EF001C45FA00122C79........4EAE....4CDF _sprintf" + nl;
            if (sExp != sGen)
                Debug.Print(sGen);
            Assert.AreEqual(sExp, sGen);
        }
    }
}
#endif
