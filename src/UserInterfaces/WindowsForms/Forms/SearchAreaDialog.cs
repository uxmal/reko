using Reko.Core.Loading;
using Reko.Gui;
using Reko.Gui.Services;
using Reko.Gui.ViewModels.Dialogs;
using System;
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
                OnDataContextChanged();
            }
        }

        public SearchArea Value => throw new NotImplementedException();

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
                    ForeColor = PickForeColor(segment.Segment),
                    BackColor = PickBackColor(segment.Segment),
                    Tag = segment,
                };
                listSegments.Items.Add(item);
            }
        }
    

        private Color PickForeColor(ImageSegment segment)
        {
            throw new NotImplementedException();
        }

        private Color PickBackColor(ImageSegment segment)
        {
            throw new NotImplementedException();
        }
    }
}
