using Decompiler.Gui.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public partial class UserPreferencesDialog : Form
    {
        public UserPreferencesDialog()
        {
            InitializeComponent();

            new UserPreferencesInteractor().Attach(this);
        }

        public IServiceProvider Services { get; set; }

        public TreeView WindowTree     { get { return treeView1; } }
        public Button WindowFontButton { get { return btnWindowFont; } }
        public Button WindowFgButton   { get { return btnWindowFgColor; } }
        public Button WindowBgButton   { get { return btnWindowBgColor; } }

        public ListBox ImagebarList    { get { return lbxUiElements; } }
        public Button ImagebarFgButton { get { return btnElementFgColor; } }
        public Button ImagebarBgButton { get { return btnElementBgColor; } }

        public TextView CodeControl { get { return codeCtl; } }
        public MemoryControl MemoryControl { get { return memCtl; } }
        public DisassemblyControl DisassemblyControl { get { return dasmCtl; } }

        public ColorDialog ColorPicker { get { return colorPicker; } }
        public FontDialog FontPicker { get { return fontPicker; } }
    }
}
