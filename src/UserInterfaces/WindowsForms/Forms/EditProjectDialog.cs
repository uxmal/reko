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

using Reko.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class EditProjectDialog : Form, IDialog
    {
        public EditProjectDialog()
        {
            InitializeComponent();
        }


        public TextBox BinaryFilename
        {
            get { return txtInputFile; }
        }

        public TextBox BaseAddress
        {
            get { return txtLoadAddress; }
            
        }

        public TextBox Disassembly
        {
            get { return txtAssemblerFile; }
        }

        public TextBox IntermediateFilename
        {
            get { return txtIntermediateFile; }
            
        }

        public TextBox TypesFilename
        {
            get { return txtHeaderFile; }
        }

        public TextBox OutputFilename
        {
            get { return txtSourceFile; }
        }

        public Button BrowseBinaryFileButton
        {
            get { return btnBrowseInputFile; }
        }
        public Button BrowseAssemblerFileButton
        {
            get { return btnBrowseAssemblerFile; }
        }
        public Button BrowseIntermediateFileButton
        {
            get { return btnBrowseIntermediateFile; }
        }
        public Button BrowseTypesFileButton
        {
            get { return btnBrowseHeaderFile; }
        }
        public Button BrowseOutputFileButton
        {
            get { return btnBrowseSourceFile; }
        }

        public OpenFileDialog OpenFileDialog
        {
            get { return openFileDialog; }
        }

        public SaveFileDialog SaveFileDialog
        {
            get { return saveFileDialog; }
        }
    }
}
