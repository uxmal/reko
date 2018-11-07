using Reko.Core.Lib;
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
    public partial class SuffixArrayPerformanceDialog : Form
    {
        public SuffixArrayPerformanceDialog()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await RunPerformanceRun();
        }

        private async Task RunPerformanceRun()
        {
            lblSuffixArrayComputation.Text = " Running test...";
            byte[] text = GenerateText();
            var method = SelectMethod();
            var result = await Task.Run(() => RunTest(method, text));
            lblSuffixArrayComputation.Text = $"Built SA + LCP in {result.Item1} msec";
            lblSASequence.Text = RenderSuffixArray(text, result.Item2);
            lblLcp.Text = RenderLcp(text, result.Item3);
        }

        private string RenderSuffixArray(byte[] text, int[] suffixArray)
        {
            var strs =
                from idx in suffixArray.Take(10)
                select
                    string.Join(",", text.Skip(idx).Take(10).Select(b => b.ToString("X2")));
            return string.Join(Environment.NewLine, strs);
        }


        private string RenderLcp(byte [] text, int [] lcp)
        {
            return string.Join(Environment.NewLine, lcp.Take(10));
        }

        private byte[] GenerateText()
        {
            if (txtText.Text.Length > 0)
            {
                return Encoding.ASCII.GetBytes(txtText.Text);
            }
            var text = new byte[Convert.ToInt32(textBox1.Text)];
            var rng = new Random(0x4711);
            rng.NextBytes(text);
            return text;
        }

        private Func<byte[], (int[],int[])> SelectMethod()
        {
            switch (comboBox1.SelectedIndex)
            {
            default: return BuildSuffixArray1;
            case 1: return BuildSuffixArray2;
            }
        }

        private (long, int[], int[]) RunTest(Func<byte[], (int[],int[])> test, byte[] data)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var sa = test(data);
            stopwatch.Stop();
            return (stopwatch.ElapsedMilliseconds, sa.Item1, sa.Item2);
        }

        private (int[],int[]) BuildSuffixArray1(byte[] text)
        {
            var s = SuffixArray.Create(text);
            var sa = s.Save();
            var lcp = s.Lcp;
            return (sa, lcp);
        }

        private (int[],int[]) BuildSuffixArray2(byte [] text)
        {
            var sa = SuffixArray2.Create(text);
            return (sa.SuffixArray, sa.Lcp);
        }

        private void SuffixArrayPerformanceDialog_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }
    }
}
