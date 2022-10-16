using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Reko.Gui.ViewModels.Documents;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents;
using System;

namespace Reko.UserInterfaces.AvaloniaUI.Views.Documents
{
    public partial class BaseAddressFinderDocumentView : UserControl
    {
        public BaseAddressFinderDocumentView()
        {
            InitializeComponent();
        }

        public BaseAddressFinderViewModel? ViewModel =>
            ((BaseAddressFinderDocumentViewModel?) DataContext)?.ViewModel;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.ViewModel;
            if (vm is null)
                return;
            await vm.StartStopFinder();
        }

        private void btnChangeAddress_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
