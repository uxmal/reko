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

using NUnit.Framework;
using Reko.UserInterfaces.WindowsForms;
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    public class TextSpanFormatterTests
    {
        private readonly string nl = Environment.NewLine;

        private string Flatten(TextSpanFormatter tsf)
        {
            var model = tsf.GetModel();
            var sb= new StringBuilder();
            var lines = model.GetLineSpans(model.LineCount);
            foreach (var line in lines)
            {
                foreach (var span in line.TextSpans)
                {
                    EmitSpanWrapper(span, sb);
                    sb.Append(span.GetText());
                    EmitSpanWrapper(span, sb);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void EmitSpanWrapper(TextSpan span, StringBuilder sb)
        {
            if (span.Style == "kw")
                sb.Append("'");
            if (span.Style == "cmt")
                sb.Append("rem ");
            if (span.Style == "link")
                sb.Append("_");
        }

        [Test]
        public void TSF_Empty()
        {
            var tsf = new TextSpanFormatter();
            Assert.AreEqual("", Flatten(tsf));
        }

        [Test]
        public void TSF_Text()
        {
            var tsf = new TextSpanFormatter();
            tsf.Write("hello");
            Assert.AreEqual("hello"+ nl, Flatten(tsf));
        }

        [Test]
        public void TSF_Link()
        {
            var tsf = new TextSpanFormatter();
            tsf.Write("go to ");
            tsf.WriteHyperlink("Hell", "Aitch-ee-double-hockeysticks");
            Assert.AreEqual("go to _Hell_" + nl, Flatten(tsf));
        }
    }
}
