using Reko.Arch.Arm;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = Reko.Core.Expressions.Application;

namespace Reko.WindowsItp
{
    public partial class RewriterPerformanceDialog : Form
    {
        public RewriterPerformanceDialog()
        {
            InitializeComponent();
        }

        private class RewriterHost : IRewriterHost
        {
            private Dictionary<string, PseudoProcedure> ppp;
            private Dictionary<Address, ImportReference> importThunks;

            public RewriterHost(Dictionary<Address, ImportReference> importThunks)
            {
                this.importThunks = importThunks;
                this.ppp = new Dictionary<string, PseudoProcedure>();
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

            public FunctionType GetCallSignatureAtAddress(Address addrCallInstruction)
            {
                throw new NotImplementedException();
            }

            public Expression GetImport(Address addrThunk, Address addrInstruction)
            {
                throw new NotImplementedException();
            }

            public ExternalProcedure GetImportedProcedure(Address addrThunk, Address addrInstruction)
            {
                if (importThunks.TryGetValue(addrThunk, out var p))
                    throw new NotImplementedException();
                else
                    return null;
            }


            public ExternalProcedure GetInterceptedCall(Address addrImportThunk)
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
            var bytes = CreateBytes();
            var rw = CreateRewriter(bytes);

            button1.Enabled = false;
            TimeSpan nInstrs = await Task.Run<TimeSpan>(() =>
            {
                int instrs = 0;
                var watch = new Stopwatch();
                var addrEnd = bytes.EndAddress;
                try
                {
                    watch.Start();
                    var ei = rw.GetEnumerator();
                    while (ei.MoveNext() && ei.Current.Address < addrEnd)
                    {
                        var i = ei.Current;
                        ++instrs;
                    }
                }
                catch (Exception ex)
                {

                }
                watch.Stop();
                return watch.Elapsed;
            });
            lblTime.Text = nInstrs.ToString();
            button1.Enabled = true;
        }

        private MemoryArea CreateBytes()
        {
            var buf = new byte[10000001];    // 1e6 bytes
            var rnd = new Random(0x42);
            rnd.NextBytes(buf);
            var mem = new MemoryArea(Address.Ptr32(0x00100000), buf);
            return mem;
        }

        private IEnumerable<RtlInstructionCluster> CreateRewriter(MemoryArea mem)
        {
            var arch = new ThumbArchitecture("arm-thumb");
            var rdr = new LeImageReader(mem, mem.BaseAddress);
            var rw = arch.CreateRewriter(rdr, arch.CreateProcessorState(), new StorageBinder(), new RewriterHost(new Dictionary<Address, ImportReference>()));
            return rw;
        }

        private void RewriterPerformanceDialog_Load(object sender, EventArgs e)
        {

        }

    }
}
