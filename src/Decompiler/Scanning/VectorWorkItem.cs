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

        public ImageMapVectorTable table;
        public ProcessorState state;
        public Address addrFrom;			// address from which the jump is called.
        public PrimitiveType stride;
        public ushort segBase;

        private IScanner scanner;
        private ProgramImage image;
        private Procedure proc;
        private Dictionary<Address, VectorUse> vectorUses;

        public VectorWorkItem(IScanner scanner, ProgramImage image, ImageMapVectorTable table, Procedure proc)
            : base()
        {
            this.scanner = scanner;
            this.image = image;
            this.table = table;
            this.proc = proc;
        }

        public override void Process()
        {
            VectorBuilder builder = new VectorBuilder(scanner.Architecture, image, new DirectedGraphImpl<object>());
            var vector = builder.Build(table.TableAddress, addrFrom, state);
            if (vector.Count == 0)
            {
                Address addrNext = table.TableAddress + stride.Size;
                if (image.IsValidAddress(addrNext))
                {
                    // Can't determine the size of the table, but surely it has one entry?
                    image.Map.AddItem(addrNext, new ImageMapItem());
                }
                return;
            }

            table.Addresses.AddRange(vector);
            for (int i = 0; i < vector.Count; ++i)
            {
                ProcessorState st = state.Clone();
                if (table.IsCallTable)
                {
                    scanner.ScanProcedure(vector[i], null, st);
                }
                else
                {
                    scanner.EnqueueJumpTarget(vector[i], proc, st);
                }
            }
            vectorUses[addrFrom] = new VectorUse(table.TableAddress, builder.IndexRegister);
            image.Map.AddItem(table.TableAddress + builder.TableByteSize, new ImageMapItem());
        }

    }
}
