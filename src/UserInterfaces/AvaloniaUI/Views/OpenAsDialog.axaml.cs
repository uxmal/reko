using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Reko.Gui.Forms;

namespace Reko.UserInterfaces.AvaloniaUI.Views
{
    public partial class OpenAsDialog : Window
    {
        public OpenAsDialog()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
