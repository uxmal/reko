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

using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Drawing;
using Reko.Core;
using Reko.Gui;
using Reko.Gui.Services;
using Reko.Gui.TextViewing;
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.Diagnostics;
using System.Drawing;
using P2 = Microsoft.Msagl.Core.Geometry.Point;

namespace Reko.UserInterfaces.WindowsForms
{
    public class CfgBlockNode
    {
        public IUiPreferencesService UiPreferences { get; set; }
        public Block Block { get; set; }
        public ITextViewModel TextModel { get; set; }
        public TextViewLayout Layout { get; set; }

        /// <summary>
        /// Compute the size of the block
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public ICurve GetNodeBoundary(Node node)
        {
            const double radiusRatio = 0.3;
            var extent = Layout.CalculateExtent();
            double height = extent.Height;
            double width = extent.Width;
            return CurveFactory.CreateRectangleWithRoundedCorners(width, height, width * radiusRatio, height * radiusRatio, new P2());
        }

        public bool DrawNode(Node node, object graphics)
        {
            Graphics g = (Graphics)graphics;

            var m = g.Transform;
            var saveM = g.Transform.Clone();
            //      g.SetClip(FillTheGraphicsPath(node.GeometryNode.BoundaryCurve));

            // This is supposed to flip the text around its center

            var c = (float)node.GeometryNode.Center.Y;
            using (var m2 = new System.Drawing.Drawing2D.Matrix(1, 0, 0, -1, 0, 2 * c))
            {
                m.Multiply(m2);
            }

            m.Translate(
                (float)node.GeometryNode.Center.X,
                (float)node.GeometryNode.Center.Y);

            g.Transform = m;

            var styleStack = GetStyleStack();
            var painter = new TextViewPainter(
                Layout, g,
                SystemColors.WindowText,
                SystemColors.Window,
                SystemFonts.DefaultFont, styleStack);
            var ptr = new TextPointer(TextModel.StartPosition, 0, 0);
            painter.SetSelection(ptr, ptr);
            painter.PaintGdiPlus();
            g.Transform = saveM;
            g.ResetClip();

            saveM.Dispose();
       //     m.Dispose();
            return true;//returning false would enable the default rendering
        }

        private StyleStack GetStyleStack()
        {
            return new StyleStack(this.UiPreferences);
        }

        static System.Drawing.Drawing2D.GraphicsPath FillTheGraphicsPath(ICurve iCurve)
        {
            var curve = ((RoundedRect)iCurve).Curve;
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            foreach (ICurve seg in curve.Segments)
                AddSegmentToPath(seg, ref path);
            return path;
        }

        private static void AddSegmentToPath(ICurve seg, ref System.Drawing.Drawing2D.GraphicsPath p)
        {
            const float radiansToDegrees = (float)(180.0 / Math.PI);
            LineSegment line = seg as LineSegment;
            if (line is not null)
                p.AddLine(PointF(line.Start), PointF(line.End));
            else {
                CubicBezierSegment cb = seg as CubicBezierSegment;
                if (cb is not null)
                    p.AddBezier(PointF(cb.B(0)), PointF(cb.B(1)), PointF(cb.B(2)), PointF(cb.B(3)));
                else {
                    Ellipse ellipse = seg as Ellipse;
                    if (ellipse is not null)
                        p.AddArc((float)(ellipse.Center.X - ellipse.AxisA.Length), (float)(ellipse.Center.Y - ellipse.AxisB.Length),
                            (float)(2 * ellipse.AxisA.Length), (float)(2 * ellipse.AxisB.Length), (float)(ellipse.ParStart * radiansToDegrees),
                            (float)((ellipse.ParEnd - ellipse.ParStart) * radiansToDegrees));

                }
            }
        }

        static internal PointF PointF(P2 p) { return new PointF((float)p.X, (float)p.Y); }

    }

}
