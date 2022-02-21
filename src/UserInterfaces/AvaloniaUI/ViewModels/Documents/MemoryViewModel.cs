using Dock.Model.ReactiveUI.Controls;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents
{
    public class MemoryViewModel : Document
    {
        public MemoryViewModel()
        {
            var rnd = new Random();
            var bytes = new byte[12000];
            rnd.NextBytes(bytes);
            this.MemoryArea = new ByteMemoryArea(Address.Ptr32(0x0011200), bytes);
        }

        public MemoryArea? MemoryArea { get; set; }
    }
}
