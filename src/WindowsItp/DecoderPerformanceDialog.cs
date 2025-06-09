using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.WindowsItp.Decoders;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    public partial class DecoderPerformanceDialog : Form
    {
        private List<RtlInstructionCluster> rtls;
        private ServiceContainer sc;

        public DecoderPerformanceDialog()
        {
            InitializeComponent();
            this.sc = new ServiceContainer();
        }

        private async void btnDoit_Click(object sender, EventArgs e)
        {
            lblTest.Text = "Running...";
            try
            {
                byte[] buf = ReadBytes();
                if (buf is null)
                    return;
                Func<long> test = SelectTest(buf);
                long msec = await Task.Run(test);
                double instrs_msec = msec / (double) (buf.Length / 4000);
                lblTest.Text = $"Done in {msec}ms; {instrs_msec,3} usec/instr";
            }
            catch
            {
                lblTest.Text = "Threw an exception innit?";
            }
        }

        private Func<long> SelectTest(byte[] buf)
        {
            var rewriter = chkRewriter.Checked;
            Func<long> test;
            if (rdbRealDasm.Checked)
            {
                var arch = new Reko.Arch.Arm.Arm32Architecture(sc, "arm32", new Dictionary<string, object>());
                //var arch = new Reko.Arch.X86.X86ArchitectureFlat32("x86-protected-32");

                if (rewriter)
                {
                    test = () => PerformanceTest_A32Rewriter(arch, buf);
                }
                else
                {
                    test = () => PerformanceTest_A32Dasm(arch, buf);
                }
            }
            else
            {
                DecoderBuilder m;
                if (rdbInterpretedDasm.Checked)
                {
                    m = new Decoders.FormatDecoderBuilder();
                }
                else
                {
                    m = new Decoders.ThreadedDecoderBuilder();
                }
                var root = m.Mask(29, 3,
                    m.Instr(Mnemonic.add, "r8,r4,r0"),
                    m.Instr(Mnemonic.sub, "r8,r4,r0"),
                    m.Instr(Mnemonic.mul, "r8,r4,r0"),
                    m.Instr(Mnemonic.div, "r8,r4,r0"),

                    m.Instr(Mnemonic.and, "r8,r4,r0"),
                    m.Instr(Mnemonic.or, "r8,r4,r0"),
                    m.Instr(Mnemonic.not, "r8,r4"),
                    m.Instr(Mnemonic.xor, "r8,r4,r0"));

                if (rewriter)
                {
                    test = () => PerformanceTest_SimulatedRewriter(buf, root);
                }
                else
                {
                    test = () => PerformanceTest_Simulated(buf, root);
                }
            }
            return test;
        }

        private byte[] ReadBytes()
        {
            if (rdbLoadFile.Checked)
            {
                string filename = txtFilename.Text;
                if (string.IsNullOrEmpty(filename))
                    return null;
                var buf = System.IO.File.ReadAllBytes(filename);
                return buf;
            }
            else
            {
                if (!int.TryParse(txtRandomSize.Text, out var bufsize))
                    return null;
                var rnd = new Random(4711);
                var buf = new byte[bufsize];
                rnd.NextBytes(buf);
                return buf;
            }
        }

        private long PerformanceTest_Simulated(byte[] buf, Decoder root)
        {
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

        private long PerformanceTest_SimulatedRewriter(byte[] buf, Decoder root)
        {
#if NYI
            var rdr = new BeImageReader(buf);
            var dasm = new Disassembler(rdr, root);
            var rewriter = new Rewriter(dasm);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (var instr in rewriter)
            {
            }
            sw.Stop();
            var time = sw.ElapsedMilliseconds;
            return time;
#endif
            return 1;
        }

        private long PerformanceTest_A32Dasm(IProcessorArchitecture arch, byte[] buf)
        {
            var mem = new ByteMemoryArea(Address.Ptr32(0x00100000), buf);
            var rdr = arch.CreateImageReader(mem, mem.BaseAddress);
            var dasm = arch.CreateDisassembler(rdr);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (var instr in dasm)
            {
            }
            sw.Stop();
            var time = sw.ElapsedMilliseconds;
            return time;

        }

        private long PerformanceTest_A32Rewriter(IProcessorArchitecture arch,   byte[] buf)
        {
            var mem = new ByteMemoryArea(Address.Ptr32(0x00100000), buf);
            var rdr = arch.CreateImageReader(mem, mem.BaseAddress);
            var dasm = arch.CreateRewriter(rdr, arch.CreateProcessorState(), new StorageBinder(),
                new RewriterPerformanceDialog.RewriterHost(new Dictionary<Address, ImportReference>()));
            this.rtls = new List<RtlInstructionCluster>();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (var rtl in dasm)
            {
                rtls.Add(rtl);
            }
            sw.Stop();
            var time = sw.ElapsedMilliseconds;
            return time;

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}