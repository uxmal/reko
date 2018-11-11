using NUnit.Framework;
using Reko.Core;
using Reko.WindowsItp.Decoders;
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
    public partial class DecoderPerformanceDialog : Form
    {
        public DecoderPerformanceDialog()
        {
            InitializeComponent();
        }

        private async void btnDoit_Click(object sender, EventArgs e)
        {
            lblTest.Text = "Running...";
            try
            {
                var bufsize = 250_000;
                //var m = new Decoders.FormatDecoderBuilder();
                var m = new Decoders.ThreadedDecoderBuilder();
                long msec = await Task.Run(()=>PerformanceTest_Run(bufsize, m));
                double instrs_msec = msec / (double) (bufsize / 4);
                lblTest.Text = $"Done in {msec}ms; {instrs_msec,3} msec/instr";
            }
            catch
            {

            }
        }

        private long PerformanceTest_Run(int bufsize, DecoderBuilder m)
        {
            var root = m.Mask(29, 3,
                m.Instr(Opcode.add, "r8,r4,r0"),
                m.Instr(Opcode.sub, "r8,r4,r0"),
                m.Instr(Opcode.mul, "r8,r4,r0"),
                m.Instr(Opcode.div, "r8,r4,r0"),

                m.Instr(Opcode.and, "r8,r4,r0"),
                m.Instr(Opcode.or, "r8,r4,r0"),
                m.Instr(Opcode.not, "r8,r4"),
                m.Instr(Opcode.xor, "r8,r4,r0"));
            var rnd = new Random(4711);
            var buf = new byte[bufsize];
            rnd.NextBytes(buf);
            GC.Collect();
            var rdr = new BeImageReader(buf);
            var dasm = new Disassembler(rdr, root);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (var instr in dasm)
            {
            }
            sw.Stop();
            var time = sw.ElapsedMilliseconds;
            return time;
        }
    }
}
