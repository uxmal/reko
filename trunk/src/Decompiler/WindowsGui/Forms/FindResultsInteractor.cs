using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace Decompiler.WindowsGui.Forms
{
    public class FindResultsInteractor : IFindResultsService
    {
        private ListView listView;

        public void Attach(ListView listView)
        {
            this.listView = listView;
        }

        public void ShowResults(IList<FindResult> results)
        {
            foreach (FindResult result in results)
            {
                AddResult(result);
            }
        }

        public void AddResult(FindResult result)
        {
            ListViewItem item = new ListViewItem();
            item.Text = result.ToString();
            item.Tag = result;
            listView.Items.Add(item);
        }
    }
}
