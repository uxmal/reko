#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Reko.UserInterfaces.AvaloniaUI.Views
{
    public class VisualizerControl : Control
    {
        public static readonly DirectProperty<VisualizerControl, object?> VisualizerProperty =
            AvaloniaProperty.RegisterDirect<VisualizerControl, object?>(
                nameof(Visualizer),
                v => v.Visualizer,
                (v, vv) => v.Visualizer = vv);

        public object? Visualizer
        {   get => visualizer;
            set => this.visualizer = value; }
        private object? visualizer;

        public override void Render(DrawingContext context)
        {
            context.FillRectangle(Brushes.Red, new Rect(0, 0, 100, 100));
            base.Render(context);
        }
    }
}
