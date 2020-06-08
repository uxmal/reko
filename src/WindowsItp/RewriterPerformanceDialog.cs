using Reko.Arch.Arm;
using Reko.Arch.Arm.AArch32;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
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
            private Dictionary<string, PseudoProcedure> ppp;
            private Dictionary<Address, ImportReference> importThunks;

            public RewriterHost(Dictionary<Address, ImportReference> importThunks)
            {
                this.importThunks = importThunks;
                this.ppp = new Dictionary<string, PseudoProcedure>();
            }

            public Expression CallIntrinsic(string name, FunctionType fnType, params Expression[] args)
            {
                if (!ppp.TryGetValue(name, out var intrinsic))
                {
                    intrinsic = new PseudoProcedure(name, fnType);
                    ppp.Add(name, intrinsic);
                }
                return new Application(
                    new ProcedureConstant(PrimitiveType.Ptr32, intrinsic),
                    fnType.ReturnValue.DataType,
                    args);
            }

            public PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity)
            {
                if (ppp.TryGetValue(name, out var p))
                    return p;
                p = new PseudoProcedure(name, returnType, arity);
                ppp.Add(name, p);
                return p;
            }

            public Expression PseudoProcedure(string name, DataType returnType, params Expression[] args)
            {
                var ppp = EnsurePseudoProcedure(name, returnType, args.Length);
                return new Application(
                    new ProcedureConstant(PrimitiveType.Ptr32, ppp),
                    returnType,
                    args);
            }

            public Expression PseudoProcedure(string name, ProcedureCharacteristics c, DataType returnType, params Expression[] args)
            {
                var ppp = EnsurePseudoProcedure(name, returnType, args.Length);
                ppp.Characteristics = c;
                return new Application(
                    new ProcedureConstant(PrimitiveType.Ptr32, ppp),
                    returnType,
                    args);
            }

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
                if (bytes == null)
                    return (new TimeSpan(), new TimeSpan());
                var tsDasm = MeasureTime(DasmInstructions,bytes);
                var tsRewr = MeasureTime(RewriteA32Instructions, bytes);
                return (tsDasm, tsRewr);
            });
            lblDasm.Text = nDasm.ToString();
            lblRw.Text = nRewrite.ToString();
            button1.Enabled = true;
        }

        private TimeSpan MeasureTime(Action<MemoryArea> task, MemoryArea mem)
        {
            var watch = new Stopwatch();
            watch.Start();
            task(mem);
            watch.Stop();
            return watch.Elapsed;
        }

        private void RewriteT32Instructions(MemoryArea mem)
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

        private void RewriteA32Instructions(MemoryArea mem)
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

        private void DasmInstructions(MemoryArea mem)
        {
            var dasm = CreateA32Disassembler(mem);
            var ei = dasm.GetEnumerator();
            var mis = new List<MachineInstruction>();
            while (ei.MoveNext() && ei.Current != null)
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

        private MemoryArea CreateBytes()
        {
            if (!long.TryParse(textBox1.Text, out var cb))
                return null;
            var buf = new byte[cb];  
            var rnd = new Random(0x4242);
            rnd.NextBytes(buf);
            var mem = new MemoryArea(Address.Ptr32(0x00100000), buf);
            return mem;
        }

        private IEnumerable<RtlInstructionCluster> CreateT32Rewriter(MemoryArea mem)
        {
            var arch = new ThumbArchitecture(sc, "arm-thumb");
            var rdr = new LeImageReader(mem, mem.BaseAddress);
            var rw = arch.CreateRewriter(rdr, arch.CreateProcessorState(), new StorageBinder(), new RewriterHost(new Dictionary<Address, ImportReference>()));
            return rw;
        }

        private IEnumerable<RtlInstructionCluster> CreateA32Rewriter(MemoryArea mem)
        {
            var arch = new Arm32Architecture(sc, "arm");
            var rdr = new LeImageReader(mem, mem.BaseAddress);
            var rw = arch.CreateRewriter(rdr, arch.CreateProcessorState(), new StorageBinder(), new RewriterHost(new Dictionary<Address, ImportReference>()));
            return rw;
        }

        private IEnumerable<AArch32Instruction> CreateA32Disassembler(MemoryArea mem)
        {
            var arch = new Arm32Architecture(sc, "arm");
            var rdr = new LeImageReader(mem, mem.BaseAddress);
            var dasm = new A32Disassembler(arch, rdr);
            return dasm;
        }

        private void RewriterPerformanceDialog_Load(object sender, EventArgs e)
        {

        }

    }
}
