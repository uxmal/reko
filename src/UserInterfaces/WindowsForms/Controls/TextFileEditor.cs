#region License
/* 
 * Copyright (C) 1999-2021 Pavel Tomin.
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
            var lineNumber = line - 1;
            var charIndex = textBox.GetFirstCharIndexFromLine(lineNumber);
            textBox.SelectionStart = charIndex;
            textBox.ScrollToCaret();
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
