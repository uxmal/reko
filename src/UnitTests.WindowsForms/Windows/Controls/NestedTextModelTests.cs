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

namespace Reko.UnitTests.Gui.Windows.Controls
{
    [TestFixture]
    public class NestedTextModelTests
    {
        private TextViewModel CreateNode(params string[] lines)
        {
            var fmt = new TextSpanFormatter();
            foreach (var line in lines)
            {
                fmt.WriteLine(line);
            }
            return fmt.GetModel();
        }

        [Test]
        public void Ntm_Add()
        {
            var ntm = new NestedTextModel
            {
                Nodes =
                {
                    CreateNode("hello", "world", "fnord"),
                }
            };

            var spans = ntm.GetLineSpans(4);
            Assert.AreEqual(3, spans.Length);
            var delta = ntm.MoveToLine(ntm.CurrentPosition, 1);
            Assert.AreEqual(0, delta, "Should have been at the end");
        }

        [Test]
        public void Ntm_Add_TwoModels()
        {
            var ntm = new NestedTextModel
            {
                Nodes =
                {
                    CreateNode("hello"),
                    CreateNode("world", "fnord"),
                }
            };

            var spans = ntm.GetLineSpans(2);
            Assert.AreEqual(2, spans.Length);
            spans = ntm.GetLineSpans(1);
            Assert.AreEqual("fnord", spans[0].TextSpans[0].GetText());
        }

        [Test]
        public void Ntm_MoveAcrossBoundary()
        {
            var ntm = new NestedTextModel
            {
                Nodes =
                {
                    CreateNode("hello"),
                    CreateNode("world", "control", "fnord"),
                }
            };
            int delta = ntm.MoveToLine(ntm.CurrentPosition, 2);
            Assert.AreEqual(2, delta);
            var spans = ntm.GetLineSpans(1);
            Assert.AreEqual("control", spans[0].TextSpans[0].GetText());
        }

        [Test]
        public void Ntm_SetPositionAsFraction()
        {
            var ntm = new NestedTextModel
            {
                Nodes =
                {
                    CreateNode("hello", "buy"),
                    CreateNode("world", "control", "fnord"),
                }
            };

            ntm.SetPositionAsFraction(1, 5);
            var spans = ntm.GetLineSpans(1);
            Assert.AreEqual("buy", spans[0].TextSpans[0].GetText());

            ntm.SetPositionAsFraction(0, 5);
            spans = ntm.GetLineSpans(1);
            Assert.AreEqual("hello", spans[0].TextSpans[0].GetText());

            ntm.SetPositionAsFraction(3, 5);
            spans = ntm.GetLineSpans(1);
            Assert.AreEqual("control", spans[0].TextSpans[0].GetText());

            ntm.SetPositionAsFraction(2, 5);
            spans = ntm.GetLineSpans(1);
            Assert.AreEqual("world", spans[0].TextSpans[0].GetText());

            ntm.SetPositionAsFraction(4, 5);
            spans = ntm.GetLineSpans(1);
            Assert.AreEqual("fnord", spans[0].TextSpans[0].GetText());

            ntm.SetPositionAsFraction(5, 5);
            spans = ntm.GetLineSpans(1);
            Assert.AreEqual(0, spans.Length);
        }

        [Test]
        public void Ntm_GetLineSpans()
        {
            var ntm = new NestedTextModel
            {
                Nodes =
                {
                    CreateNode("hello", "buy"),
                    CreateNode("world", "control", "fnord"),
                }
            };

            ntm.MoveToLine(ntm.StartPosition, 1);
            var spans = ntm.GetLineSpans(5);
            Assert.AreEqual(4, spans.Length);
            Assert.AreEqual("buy", spans[0].TextSpans[0].GetText());
            Assert.AreEqual("world", spans[1].TextSpans[0].GetText());
            Assert.AreEqual("control", spans[2].TextSpans[0].GetText());
            Assert.AreEqual("fnord", spans[3].TextSpans[0].GetText());

            ntm.MoveToLine(ntm.StartPosition, 0);
            spans = ntm.GetLineSpans(5);
            Assert.AreEqual(5, spans.Length);
            Assert.AreEqual("hello", spans[0].TextSpans[0].GetText());
            Assert.AreEqual("buy", spans[1].TextSpans[0].GetText());
            Assert.AreEqual("world", spans[2].TextSpans[0].GetText());
            Assert.AreEqual("control", spans[3].TextSpans[0].GetText());
            Assert.AreEqual("fnord", spans[4].TextSpans[0].GetText());

            ntm.MoveToLine(ntm.StartPosition, 3);
            spans = ntm.GetLineSpans(5);
            Assert.AreEqual(2, spans.Length);
            Assert.AreEqual("control", spans[0].TextSpans[0].GetText());
            Assert.AreEqual("fnord", spans[1].TextSpans[0].GetText());

            ntm.MoveToLine(ntm.StartPosition, 2);
            spans = ntm.GetLineSpans(5);
            Assert.AreEqual(3, spans.Length);
            Assert.AreEqual("world", spans[0].TextSpans[0].GetText());
            Assert.AreEqual("control", spans[1].TextSpans[0].GetText());
            Assert.AreEqual("fnord", spans[2].TextSpans[0].GetText());

            ntm.MoveToLine(ntm.StartPosition, 4);
            spans = ntm.GetLineSpans(5);
            Assert.AreEqual(1, spans.Length);
            Assert.AreEqual("fnord", spans[0].TextSpans[0].GetText());

            ntm.SetPositionAsFraction(5, 5);
            spans = ntm.GetLineSpans(5);
            Assert.AreEqual(0, spans.Length);
        }
    }
}
