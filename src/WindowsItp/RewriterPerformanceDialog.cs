using Reko.Arch.Arm;
using Reko.Arch.Arm.AArch32;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = Reko.Core.Expressions.Application;

namespace Reko.WindowsItp
{
    public partial class RewriterPerformanceDialog : Form
    {
        private ServiceContainer sc;

        public RewriterPerformanceDialog()
        {
            InitializeComponent();
            sc = new ServiceContainer();
        }

        public class RewriterHost : IRewriterHost
        {
            private Dictionary<string, IntrinsicProcedure> intrinsics;
            private Dictionary<Address, ImportReference> importThunks;

            public RewriterHost(Dictionary<Address, ImportReference> importThunks)
            {
                this.importThunks = importThunks;
                this.intrinsics = new Dictionary<string, IntrinsicProcedure>();
            }

            public Constant GlobalRegisterValue => null;

            public IProcessorArchitecture GetArchitecture(string archLabel)
            {
                throw new NotImplementedException();
            }

            public FunctionType GetCallSignatureAtAddress(Address addrCallInstruction)
            {
                throw new NotImplementedException();
            }

            public Expression GetImport(Address addrThunk, Address addrInstruction)
            {
                return null;
            }

            public ExternalProcedure GetImportedProcedure(IProcessorArchitecture arch, Address addrThunk, Address addrInstruction)
            {
                if (importThunks.TryGetValue(addrThunk, out var p))
                    throw new NotImplementedException();
                else
                    return null;
            }


            public ExternalProcedure GetInterceptedCall(IProcessorArchitecture arch, Address addrImportThunk)
            {
                throw new NotImplementedException();
            }

            public bool TryRead(IProcessorArchitecture arch, Address addr, PrimitiveType dt, out Constant value)
            {
                throw new NotImplementedException();
            }

            public void Error(Address address, string format, params object[] args)
            {
                //$TODO: ERROR
            }

            public void Warn(Address address, string format, params object[] args)
            {
                Debug.Write($"W: {address}: ");
                Debug.Print(format, args);
            }
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            var(nDasm, nRewrite) = await Task.Run<(TimeSpan,TimeSpan)>(() =>
            {
                var bytes = CreateBytes();
                if (bytes is null)
                    return (new TimeSpan(), new TimeSpan());
                var tsDasm = MeasureTime(DasmInstructions,bytes);
                var tsRewr = MeasureTime(RewriteA32Instructions, bytes);
                return (tsDasm, tsRewr);
            });
            lblDasm.Text = nDasm.ToString();
            lblRw.Text = nRewrite.ToString();
            button1.Enabled = true;
        }

        private TimeSpan MeasureTime(Action<ByteMemoryArea> task, ByteMemoryArea mem)
        {
            var watch = new Stopwatch();
            watch.Start();
            task(mem);
            watch.Stop();
            return watch.Elapsed;
        }

        private void RewriteT32Instructions(ByteMemoryArea mem)
        {
            var rw = CreateT32Rewriter(mem);
            int instrs = 0;
            var length = mem.Length;
            var startAddress = mem.BaseAddress;

            var ei = rw.GetEnumerator();
            while (SafeMoveNext(ei) && (ei.Current.Address - startAddress) < length)
            {
                var i = ei.Current;
                ++instrs;
            }
        }

        private void RewriteA32Instructions(ByteMemoryArea mem)
        {
            var rw = CreateA32Rewriter(mem);
            var length = mem.Length;
            var startAddress = mem.BaseAddress;

            var rtl = new List<RtlInstructionCluster>();
            var ei = rw.GetEnumerator();
            while (SafeMoveNext(ei) && (ei.Current.Address - startAddress) < length)
            {
                rtl.Add(ei.Current);
            }
        }

        private void DasmInstructions(ByteMemoryArea mem)
        {
            var dasm = CreateA32Disassembler(mem);
            var ei = dasm.GetEnumerator();
            var mis = new List<MachineInstruction>();
            while (ei.MoveNext() && ei.Current is not null)
            {
                mis.Add(ei.Current);
            }
        }

        private bool SafeMoveNext(IEnumerator<RtlInstructionCluster> ei)
        {
            for (; ; )
            {
                try
                {
                    return ei.MoveNext();
                }
                catch
                {

                }
            }
        }

        private ByteMemoryArea CreateBytes()
        {
            if (!long.TryParse(textBox1.Text, out var cb))
                return null;
            var buf = new byte[cb];  
            var rnd = new Random(0x4242);
            rnd.NextBytes(buf);
            var mem = new ByteMemoryArea(Address.Ptr32(0x00100000), buf);
            return mem;
        }

        private IEnumerable<RtlInstructionCluster> CreateT32Rewriter(ByteMemoryArea mem)
        {
            var arch = new ThumbArchitecture(sc, "arm-thumb", new Dictionary<string, object>());
            var rdr = new LeImageReader(mem, mem.BaseAddress);
            var rw = arch.CreateRewriter(rdr, arch.CreateProcessorState(), new StorageBinder(), new RewriterHost(new Dictionary<Address, ImportReference>()));
            return rw;
        }

        private IEnumerable<RtlInstructionCluster> CreateA32Rewriter(ByteMemoryArea mem)
        {
            var arch = new Arm32Architecture(sc, "arm", new Dictionary<string, object>());
            var rdr = new LeImageReader(mem, mem.BaseAddress);
            var rw = arch.CreateRewriter(rdr, arch.CreateProcessorState(), new StorageBinder(), new RewriterHost(new Dictionary<Address, ImportReference>()));
            return rw;
        }

        private IEnumerable<AArch32Instruction> CreateA32Disassembler(ByteMemoryArea mem)
        {
            var arch = new Arm32Architecture(sc, "arm", new Dictionary<string, object>());
            var rdr = new LeImageReader(mem, mem.BaseAddress);
            var dasm = new A32Disassembler(arch, rdr);
            return dasm;
        }

        private void RewriterPerformanceDialog_Load(object sender, EventArgs e)
        {

        }

    }
}
