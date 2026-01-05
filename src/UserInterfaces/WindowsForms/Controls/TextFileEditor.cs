#region License
/* 
 * Copyright (C) 1999-2026 Pavel Tomin.
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

using Reko.Gui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    public partial class TextFileEditor : UserControl, ITextFileEditor
    {
        public TextFileEditor()
        {
            InitializeComponent();
            TextBox = new TextBoxWrapper(textBox);
            SaveButton = new ToolStripButtonWrapper(saveButton);
        }

        public ITextBox TextBox { get; }
        public IButton SaveButton { get; }
        public string Status
        {
            get => statusStrip.Items[0].Text;
            set => statusStrip.Items[0].Text = value;
        }

        public void GoToLine(int line)
        {
            int highlightedLineOffset = 10;
            var lineNumber = line - 1;
            if (lineNumber < 0)
                return;
            int offset = highlightedLineOffset;
            if (offset > lineNumber)
                offset = lineNumber;
            SelectLineStart(lineNumber - offset);
            textBox.ScrollToCaret();
            bool modified = textBox.Modified;
            ResetBackground();
            HighlightLine(lineNumber, Color.Azure);
            textBox.Modified = modified;
            SelectLineStart(lineNumber);
        }

        private void SelectLineStart(int lineNumber)
        {
            var charIndex = textBox.GetFirstCharIndexFromLine(lineNumber);
            if (charIndex < 0)
                return;
            textBox.SelectionStart = charIndex;
            textBox.SelectionLength = 0;
        }

        private void ResetBackground()
        {
            textBox.SelectAll();
            textBox.SelectionBackColor = textBox.BackColor;
        }

        private void HighlightLine(int lineNumber, Color color)
        {
            var lineStart = textBox.GetFirstCharIndexFromLine(lineNumber);
            if (lineStart < 0)
                return;
            var lineEnd = textBox.GetFirstCharIndexFromLine(lineNumber + 1);
            if (lineEnd < 0)
                return;
            (lineStart, lineEnd) = TrimStart(lineStart, lineEnd);
            var lineLength = lineEnd - lineStart;
            textBox.SelectionStart = lineStart;
            textBox.SelectionLength = lineLength;
            textBox.SelectionBackColor = color;
        }

        private (int, int) TrimStart(int start, int end)
        {
            for(; start < end; start++)
            {
                if (!char.IsWhiteSpace(textBox.Text[start]))
                    break;
            }
            return (start, end);
        }

        private void UpdateStatus()
        {
            int caretIndex = textBox.SelectionStart;
            int line = textBox.GetLineFromCharIndex(caretIndex);
            int firstLineChar = textBox.GetFirstCharIndexFromLine(line);
            int col = caretIndex - firstLineChar + 1;
            Status = $"Line: {line + 1} Col: {col}";
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) System.Windows.Forms.Keys.Tab)
            {
                e.Handled = true;
                //$TODO: Make it user-defined
                int numSpaces = 4;
                textBox.SelectedText = new string(' ', numSpaces);
            }
        }

        private void textBox_SelectionChanged(object sender, EventArgs e)
        {
            UpdateStatus();
        }
    }
}
