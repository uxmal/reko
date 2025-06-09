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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Reko.Gui;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.UserInterfaces.AvaloniaUI.Controls
{
    public class StyleStack 
    {
        private readonly IUiPreferencesService uiPrefSvc;
        private readonly List<string[]> stack;
        private readonly Dictionary<string, AvaloniaProperty> classToBrushMap;
        private IBrush? fg;
        private IBrush? bg;
        private Typeface? font;
        private double fontSize;
        private IBrush ctrlForeColor;

        public StyleStack(IUiPreferencesService uiPrefSvc, Dictionary<string, AvaloniaProperty> brushes)
        {
            if (uiPrefSvc is null) throw new ArgumentNullException(nameof(uiPrefSvc));
            this.uiPrefSvc = uiPrefSvc;
            this.stack = new List<string[]>();
            this.classToBrushMap = brushes;
        }


        public void PushStyle(string styleSelector)
        {
            if (styleSelector is null)
                stack.Add(Array.Empty<string>());
            else
                stack.Add(styleSelector.Split(' '));
        }

        internal void PopStyle()
        {
            stack.RemoveAt(stack.Count - 1);
        }

        private IBrush CacheBrush(ref IBrush? brInstance, IBrush brNew)
        {
            brInstance = brNew;
            return brNew;
        }

        private IEnumerable<UiStyle> GetStyles(string[] styles)
        {
            foreach (var styleName in styles)
            {
                if (uiPrefSvc.Styles.TryGetValue(styleName, out UiStyle style))
                    yield return style;
            }
        }

        public IBrush GetForeground()
        {
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var styles = GetStyles(stack[i]);
                var ff = (IBrush?) styles.Select(s => s.Foreground).LastOrDefault(f => f is not null);
                if (ff is not null)
                    return ff;
            }
            return CacheBrush(ref fg, this.ctrlForeColor);
        }

        public Cursor GetCursor(Control ctrl)
        {
            for (int i = stack.Count - 1; i>=0; --i)
            {
                var styles = GetStyles(stack[i]);
                var cu = styles.Select(s => s.Cursor).LastOrDefault(c => c is not null);
                if (cu is StandardCursorType stdCursor)
                    return new Cursor(stdCursor);
            }
            return Cursor.Default;
        }

        public IBrush GetForeground(IBrush fgColor)
        {
           for(int i = stack.Count - 1; i >= 0; --i)
            {
                var styles = GetStyles(stack[i]);
                var fg = styles.Select(s => s.Foreground).LastOrDefault(f => f is not null);
                if (fg is IBrush fgBrush)
                {
                    return fgBrush;
                }
                else if (fg is Color color)
                {
                    return new SolidColorBrush(color);
                }
            }
            return fgColor;
        }

        public IBrush GetBackground(IBrush bgColor)
        {
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var styles = GetStyles(stack[i]);
                var back = styles.Select(s => s.Background).LastOrDefault(b => b is not null);
                if (back is IBrush bgBrush)
                {
                    return bgBrush;
                }
                else if (back is Color color)
                {
                    return new SolidColorBrush(color);
                }
            }
            return CacheBrush(ref bg, bgColor);
        }

        public Typeface GetFont(Typeface defaultFont)
        {
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var styles = GetStyles(stack[i]);
                var oFont = styles.Select(s => s.Font).LastOrDefault(f => f is not null);
                if (oFont is Typeface font)
                    return font;
            }
            return defaultFont;
        }

        public double GetFontSize(double defaultFontSize)
        {
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var styles = GetStyles(stack[i]);
                var oFont = styles.Select(s => s.FontSize).LastOrDefault(f => f is not null);
                if (oFont is double fontSize && fontSize > 0)
                    return fontSize;
            }
            return defaultFontSize;
        }

        public int? GetWidth()
        {
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var styles = GetStyles(stack[i]);
                var width = styles.Select(s => s.Width).LastOrDefault(w => w.HasValue);
                if (width.HasValue)
                    return width;
            }
            return null;
        }

        public float GetNumber(Func<UiStyle, float> fn)
        {
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var styles = GetStyles(stack[i]);
                var value = styles.Select(fn).LastOrDefault(n => n != 0);
                if (value != 0)
                    return value;
            }
            return 0;
        }

        /// <summary>
        /// Given a rectangle <paramref name="rc"/>, creates a padded rectangle
        /// <paramref name="rcPadded"/> and 
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="rcPadded"></param>
        public void PadRectangle(ref Rect rc, ref Rect rcPadded)
        {
            float top = GetNumber(s => s.PaddingTop);
            float left = GetNumber(s => s.PaddingLeft);
            float bottom = GetNumber(s => s.PaddingBottom);
            float right = GetNumber(s => s.PaddingRight);
            rcPadded = new Rect(
                rc.X + left,
                rc.Y + top,
                rc.Width + left + right,
                rc.Height + top + bottom);
        }
    }
}
