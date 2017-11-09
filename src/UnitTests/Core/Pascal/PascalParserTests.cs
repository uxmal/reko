#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using NUnit.Framework;
using Reko.Core.Pascal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Pascal
{

    [TestFixture]
    public class PascalParserTests
    {
        private PascalParser parser;

        private void Given_Parser(string src)
        {
            var lexer = new PascalLexer(new StringReader(src));
            this.parser = new PascalParser(lexer);
        }


        [Test]
        public void PParser_Function_WithInline()
        {
            var src = "my_unit; interface FuNction foo(quux : ^Integer; var bar : Integer) : boolean; INLINE $BADD,$FACE; end.";
            Given_Parser(src);
            var decls = parser.ParseUnit();
            Assert.AreEqual("function foo(quux : integer^; var bar : integer) : boolean; inline $BADD, $FACE", decls[0].ToString());
        }

        [Test]
        public void PParser_Regress()
        {
            var q = new Queue<Token>();
            try
            {

                using (var rdr = new StreamReader(@"C:\dev\uxmal\reko\master\src\Environments\MacOS\Mac MPW Interfaces 1991 PASCAL.pas"))
                {
                    var lexer = new PascalLexer(rdr);
                    var parser = new PascalParser(lexer);
                    parser.Parse();
                    //Token tok;
                    //do
                    //{
                    //    tok = lexer.Read();
                    //    q.Enqueue(tok);
                    //    if (q.Count > 100)
                    //        q.Dequeue();
                    //} while (tok.Type != TokenType.EOF);
                }
            }
            catch (Exception ex)
            {
                foreach (var tok in q)
                {
                    Debug.WriteLine(tok);
                }
                throw;
            }
            foreach (var tok in q)
            {
                Debug.WriteLine(tok);
            }
        }
    }
}
