using Reko.Core.Loading;
using Reko.Gui;
using Reko.Gui.Services;
using Reko.Gui.ViewModels.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class SearchAreaDialog : Form, IDialog<SearchArea>
    {
        private SearchAreaViewModel viewModel;

        public SearchAreaDialog()
        {
            InitializeComponent();
        }

        public SearchAreaViewModel DataContext
        {
            get { return viewModel; }
            set
            {
                this.viewModel = value;
                this.lblFreeFormError.DataBindings.Add(
                    nameof(lblFreeFormError.Text),
                    viewModel,
                    nameof(viewModel.FreeFormError));
                OnDataContextChanged();
            }
        }

        public SearchArea Value { get; set; }

        private void OnDataContextChanged()
        {
            foreach (var segment in viewModel.SegmentList)
            {
                var item = new ListViewItem(new[]
                {
                    Text = segment.Name,
                    Text = segment.Address,
                    Text = segment.Access,
                })
                {
                    Tag = segment,
                };
                listSegments.Items.Add(item);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Value = new SearchArea(viewModel.Areas);
        }

        private void listSegments_ItemChecked(object sender, EventArgs e)
        {
            IEnumerable en = listSegments.CheckedItems
                .OfType<ListViewItem>()
                .Select(i => i.Tag);
            viewModel.ChangeAreas(en);
        }

        private void txtFreeForm_TextChanged(object sender, EventArgs e)
        {
            viewModel.FreeFormAreas = txtFreeForm.Text;
        }
    }
}
