using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    public partial class StructureFieldPerformanceDialog : Form
    {
        private Random rand;

        public StructureFieldPerformanceDialog()
        {
            InitializeComponent();
        }

        private async void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            var sizes = Sizes();
            this.rand = new Random(0x4711);
            foreach (var nItems in sizes)
            {
                var time = await Time(() => MakeStructureCollection(nItems));
                //var str = MakeStructureCollection(nItems);
                //var maxOffset = str.Last().Offset;
                //var time = await Time(() => FindRandomItem(str, maxOffset));
                dataGridView1.Rows.Add(new object[] { nItems, time, time / (1.0 * nItems * nItems) });
            }
            btnGo.Enabled = true;
        }

        private IEnumerable<int> Sizes()
        {
            double x = 10.0;
            while (x < 100000.0)
            {
                yield return (int) x;
                x = x * 1.8;
            }
        }

        private StructureFieldCollection MakeStructureCollection(int nItems)
        {
            var sfc = new StructureFieldCollection();
            for (int i = 0; i < nItems; ++i)
            {
                sfc.Add(i * 4, PrimitiveType.Int32);
            }
            return sfc;
        }

        private StructureField FindRandomItem(StructureFieldCollection sfc, int maxOffset)
        {
            return sfc.AtOffset(rand.Next(maxOffset));
        }

        private async Task<double> Time(Action task)
        {
            int repetitions = 1000;
            double d = await Task<double>.Run(() =>
            {
                var sw = new Stopwatch();
                double sampleSum = 0.0;
                sw.Restart();
                for (int i = 0; i < repetitions; ++i)
                {
                    task();
                }
                sampleSum += (double) sw.ElapsedMilliseconds;
                return sampleSum;
            });
            return d;
        }
    }
}
