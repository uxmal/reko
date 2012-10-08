using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Scanning
{
    public class VectorWorkItem : WorkItem
    {
        public ImageMapVectorTable Table;
        public ProcessorState State;
        public Address AddrFrom;			// address from which the jump is called.
        public PrimitiveType Stride;
        public ushort SegBase;

        private IScanner scanner;
        private ProgramImage image;
        private Procedure proc;
        private Dictionary<Address, VectorUse> vectorUses;

        public VectorWorkItem(IScanner scanner, ProgramImage image, ImageMapVectorTable table, Procedure proc)
            : base()
        {
            this.scanner = scanner;
            this.image = image;
            this.Table = table;
            this.proc = proc;
            this.vectorUses = new Dictionary<Address, VectorUse>();
        }

        public override void Process()
        {
            var builder = new VectorBuilder(scanner.Architecture, image, new DirectedGraphImpl<object>());
            var vector = builder.Build(Table.TableAddress, AddrFrom, State);
            if (vector.Count == 0)
            {
                Address addrNext = Table.TableAddress + Stride.Size;
                if (image.IsValidAddress(addrNext))
                {
                    // Can't determine the size of the table, but surely it has one entry?
                    image.Map.AddItem(addrNext, new ImageMapItem());
                }
                return;
            }

            Table.Addresses.AddRange(vector);
            for (int i = 0; i < vector.Count; ++i)
            {
                var st = State.Clone();
                if (Table.IsCallTable)
                {
                    scanner.ScanProcedure(vector[i], null, st);
                }
                else
                {
                    scanner.EnqueueJumpTarget(vector[i], proc, st);
                }
            }
            vectorUses[AddrFrom] = new VectorUse(Table.TableAddress, builder.IndexRegister);
            image.Map.AddItem(Table.TableAddress + builder.TableByteSize, new ImageMapItem());
        }

    }
}
