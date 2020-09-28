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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    /// <summary>
    /// Supports nesting text models.
    /// </summary>
    public class NestedTextModel : TextViewModel
    {
        private Location position;

        public NestedTextModel()
        {
            this.Nodes = new NodeCollection(this);
        }

        private class Location
        {
            public readonly int iModel;
            public readonly object InnerLocation;
            public Location(int i, object pos) { this.iModel = i;  this.InnerLocation = pos; }

            public override string ToString()
            {
                return $"Location: {iModel}:{InnerLocation}";
            }
        }

        public NodeCollection Nodes { get; private set; }

        public object CurrentPosition {
            get { return position; }
            private set { position = (Location) value; }
        }

        public object EndPosition { get; internal set; }

        public object StartPosition { get; internal set; }

        public int LineCount { get { return CountLines();  } }

        public int CountLines()
        {
            return Nodes.Sum(n => n.cLines);
        }

        public int ComparePositions(object a, object b)
        {
            Location aLoc = (Location)a;
            Location bLoc = (Location)b;
            int cmp = aLoc.iModel.CompareTo(bLoc.iModel);
            if (cmp == 0)
            {
                return Nodes[aLoc.iModel].Model.ComparePositions(
                    aLoc.InnerLocation,
                    bLoc.InnerLocation);
            }
            else
            {
                return cmp;
            }
        }

        public LineSpan[] GetLineSpans(int count)
        {
            var spans = new List<LineSpan>();
            if (!(CurrentPosition is Location loc))
                return spans.ToArray();
            for (int i = loc.iModel; count > 0 && i < Nodes.Count; ++i)
            {
                var model = Nodes[i].Model;
                if (position.iModel == i)
                    model.MoveToLine(position.InnerLocation, 0);
                else
                    model.MoveToLine(model.StartPosition, 0);
                var sub = model.GetLineSpans(count);
                count = count - sub.Length;
                spans.AddRange(sub.Select(ls => new LineSpan(
                    new Location(i, ls.Position),
                    ls.TextSpans)));
                position = new Location(i, model.CurrentPosition);
            }
            return spans.ToArray();
        }

        public Tuple<int, int> GetPositionAsFraction()
        {
            if (position.iModel >= Nodes.Count)
                return Tuple.Create(1, 1);
            int c = 0;
            for (int i = 0; i < position.iModel; ++i)
            {
                c += Nodes[i].Model.LineCount;
            }
            var node = Nodes[position.iModel];
            var frac = node.Model.GetPositionAsFraction();
            return Tuple.Create(
                c + (node.cLines * frac.Item1) / frac.Item2,
                CountLines());
        }

        public int MoveToLine(object position, int offset)
        {
            this.position = (Location)position;
            int total = 0;
            if (offset >= 0)
            {
                object subPos = this.position.InnerLocation;
                TextViewModel model = this.Nodes[this.position.iModel].Model;
                for (int i = this.position.iModel; i < this.Nodes.Count; ++i)
                {
                    int moved = this.Nodes[i].Model.MoveToLine(subPos, offset);
                    total += moved;
                    if (moved == offset)
                    {
                        this.position = new Location(i, this.Nodes[i].Model.CurrentPosition);
                        return total;
                    }
                    offset -= moved;
                }
                this.position = new Location(
                    this.Nodes.Count - 1,
                    this.Nodes[this.Nodes.Count - 1].Model.CurrentPosition);
            }
            return total;
        }

        public void SetPositionAsFraction(int numer, int denom)
        {
            long total = CountLines();
            long iPos = (numer * total) / denom;
            for (int i = 0; i < Nodes.Count; ++i)
            {
                var node = Nodes[i];
                if (iPos < node.cLines)
                {
                    node.Model.SetPositionAsFraction((int)iPos, node.cLines);
                    this.position = new Location(i, node.Model.CurrentPosition);
                    return;
                }
                iPos -= node.cLines;
            }
            var model = Nodes[Nodes.Count - 1].Model;
            model.SetPositionAsFraction(1, 1);
            this.position = new Location(Nodes.Count - 1, model.CurrentPosition);
        }

        public void SetPositionAsNode(TextModelNode node, int numer, int denom)
        {
            for (int i = 0; i < Nodes.Count; ++i)
            {
                if (Nodes[i] == node)
                {
                    node.Model.SetPositionAsFraction(numer, denom);
                    this.position = new Location(i, node.Model.CurrentPosition);
                    return;
                }
            }
        }

        public Tuple<TextModelNode, int, int> GetPositionAsNode()
        {
            var node = Nodes[position.iModel];
            node.Model.MoveToLine(position.InnerLocation, 0);
            var frac = node.Model.GetPositionAsFraction();
            return Tuple.Create(node, frac.Item1, frac.Item2);
        }

        public class NodeCollection : Collection<TextModelNode>
        {
            private NestedTextModel outer;
            private bool updatePositions = true;

            private static readonly Location emptyLocation = new Location(0, 0);

            public NodeCollection(NestedTextModel outer)
            {
                this.outer = outer;
            }

            public void Add(TextViewModel model)
            {
                Add(new TextModelNode(model));
            }

            protected override void ClearItems()
            {
                updatePositions = false;
                base.ClearItems();
                updatePositions = true;
                UpdatePositions();
            }

            protected override void InsertItem(int index, TextModelNode item)
            {
                base.InsertItem(index, item);
                if (Count == 1)
                {
                    outer.CurrentPosition = new Location(0, item.Model.StartPosition);
                }
                UpdatePositions();
            }

            protected override void RemoveItem(int index)
            {
                base.RemoveItem(index);
                UpdatePositions();
            }

            public void AddRange(IEnumerable<TextViewModel> models)
            {
                updatePositions = false;
                foreach (var m in models)
                {
                    Add(m);
                }
                updatePositions = true;
                UpdatePositions();
            }

            private void UpdatePositions()
            {
                if (!updatePositions)
                    return;
                if (Count == 0)
                {
                    outer.StartPosition = emptyLocation;
                    outer.EndPosition = emptyLocation;
                    outer.position = emptyLocation;
                }
                else
                {
                    outer.StartPosition = new Location(0, this[0].Model.StartPosition);
                    outer.EndPosition = new Location(Count - 1, this[Count - 1].Model.EndPosition);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{{Nested: {0} nodes: {1}}}",
                this.Nodes.Count,
                string.Join(",", this.Nodes.Select(n => n.cLines)));
        }
    }

    public class TextModelNode
    {
        public TextModelNode(TextViewModel model)
        {
            this.Model = model;
            this.cLines = model.LineCount;
        }

        public TextViewModel Model { get; private set; }
        public TextModelNode Parent { get; set; }
        public NestedTextModel NestedTextModel { get; set; }

        internal int cLines;
    }
}
