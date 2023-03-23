using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.WebAssembly
{
    public class TypeOperand : MachineOperand
    {
        public TypeOperand(int typeID)
        {
            this.TypeID = typeID;
        }

        public DataType Width { 
            get => VoidType.Instance;
            set => throw new NotSupportedException();
        }
        
        public int TypeID { get; }

        public void Render(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            var platform = (WasmPlatform?) options.Platform;
            if (TypeID < 0)
            {
                if (platform is null)
                    renderer.WriteFormat("0x{0}", -TypeID);
                else
                    renderer.WriteString(WasmPlatform.RenderValueType(-TypeID));
            }

            throw new NotImplementedException();
        }

        public string ToString(MachineInstructionRendererOptions options)
        {
            var s = new StringRenderer();
            Render(s, options);
            return s.ToString();
        }
    }
}
