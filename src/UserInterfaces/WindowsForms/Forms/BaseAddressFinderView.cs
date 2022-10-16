using Reko.Core;
using Reko.Core.Loading;
using Reko.Gui;
using Reko.Gui.ViewModels.Documents;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class BaseAddressFinderView : UserControl, IWindowPane
    {
        private BaseAddressFinderViewModel viewModel;

        public BaseAddressFinderView()
        {
            InitializeComponent();
        }

        public IWindowFrame Frame { get; set; }

        public BaseAddressFinderViewModel ViewModel
        {
            get { return viewModel; }
            set
            {
                this.viewModel = value;
                CreateBindings();
            }
        }


        public void Close()
        {
        }

        public void SetSite(IServiceProvider services)
        {
        }

        object IWindowPane.CreateControl()
        {
            return this;
        }


        private void CreateBindings()
        {
            this.btnStartStop.DataBindings.Add(
                nameof(btnStartStop.Text),
                viewModel,
                nameof(viewModel.StartStopButtonText));

            this.chkGuessStrings.DataBindings.Add(
                nameof(chkGuessStrings.Checked),
                viewModel,
                nameof(viewModel.ByString));
            this.chkGuessPrologs.DataBindings.Add(
                nameof(chkGuessPrologs.Checked),
                viewModel,
                nameof(viewModel.ByProlog));

            viewModel.Results.CollectionChanged += Results_CollectionChanged;
        }

        private void Results_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
            case NotifyCollectionChangedAction.Reset:
            default:
                _ = this;
                break;
            }
            throw new NotImplementedException();
        }

        private async void btnStartStop_Click(object sender, EventArgs e)
        {
            await viewModel?.StartStopFinder();
        }
    }
}
