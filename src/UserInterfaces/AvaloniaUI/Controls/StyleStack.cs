#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
        private IUiPreferencesService uiPrefSvc;
        private List<string[]> stack;
        private IBrush? fg;
        private IBrush? bg;
        private Typeface? font;
        private double fontSize;
        private IBrush ctrlForeColor;
        private IBrush ctrlBackColor;

        public StyleStack(IUiPreferencesService uiPrefSvc,  IBrush ctrlForeColor, IBrush ctrlBackColor)
        {
            if (uiPrefSvc is null) throw new ArgumentNullException(nameof(uiPrefSvc));
            this.uiPrefSvc = uiPrefSvc;
            this.stack = new List<string[]>();
        }


        public void PushStyle(string styleSelector)
        {
            if (styleSelector == null)
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
                var ff = (IBrush) styles.Select(s => s.Foreground).LastOrDefault(f => f != null);
                if (ff != null)
                    return ff;
            }
            return CacheBrush(ref fg, this.ctrlForeColor);
        }

        public Cursor GetCursor(Control ctrl)
        {
            Cursor cu;
            for (int i = stack.Count - 1; i>=0; --i)
            {
                var styles = GetStyles(stack[i]);
                cu = (Cursor?)styles.Select(s => s.Cursor).LastOrDefault(c => c != null);
                if (cu != null)
                    return cu;
            }
            return Cursor.Default;
        }

        public IBrush GetForeground(IBrush fgColor)
        {
           for(int i = stack.Count - 1; i >= 0; --i)
            {
                var styles = GetStyles(stack[i]);
                var fg = styles.Select(s => s.Foreground).LastOrDefault(f => f != null);
                if (fg != null)
                    return (IBrush)fg;
            }
            return fgColor;
        }

        public IBrush GetBackground(IBrush bgColor)
        {
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var styles = GetStyles(stack[i]);
                var back = styles.Select(s => s.Background).LastOrDefault(b => b != null);
                if (back != null)
                    return (IBrush) back;
            }
            return CacheBrush(ref bg, bgColor);
        }

        public Typeface GetFont(Typeface defaultFont)
        {
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var styles = GetStyles(stack[i]);
                var oFont = styles.Select(s => s.Font).LastOrDefault(f => f != null);
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
                var oFont = styles.Select(s => s.FontSize).LastOrDefault(f => f != null);
                if (oFont is double fontSize)
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
