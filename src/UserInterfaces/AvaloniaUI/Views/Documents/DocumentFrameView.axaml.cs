using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace Reko.UserInterfaces.AvaloniaUI.Views.Documents
{
    public partial class DocumentFrameView : UserControl
    {
        public DocumentFrameView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
        }
    }
}
