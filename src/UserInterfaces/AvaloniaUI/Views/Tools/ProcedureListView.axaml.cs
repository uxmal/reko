using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Reko.UserInterfaces.AvaloniaUI.Views.Tools
{
    public class ProcedureListView : UserControl
    {
        public ProcedureListView()
        {
            InitializeComponent();
            var slob = this.FindControl<DataGrid>("slob");
            slob.AttachedToVisualTree += Slob_AttachedToVisualTree;
        }

        private void Slob_AttachedToVisualTree(object? sender, global::Avalonia.VisualTreeAttachmentEventArgs e)
        {
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
