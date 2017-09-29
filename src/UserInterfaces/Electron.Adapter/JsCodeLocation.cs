using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Electron.Adapter
{
    public class JsCodeLocation : ICodeLocation
    {
        public static JsCodeLocation ForAddress(Program program, Address addr)
        {
            return new JsCodeLocation { Text = string.Format("{0}!{1}", program.Name, addr) };
        }

        public static JsCodeLocation ForProcedure(Program program, Procedure proc)
        {
            return new JsCodeLocation { Text = string.Format("{0}:{1}", program.Name, proc.Name) }; 
        }

        public static JsCodeLocation ForBlock(Program program, Block block)
        {
            var proc = block.Procedure;
            return new JsCodeLocation { Text = string.Format("{0}:{1}:{2}", program.Name, proc.Name, block.Name) };
        }

        public static JsCodeLocation ForStatement(Program program, Statement stm)
        {
            var block = stm.Block;
            var proc = block.Procedure;

            return new JsCodeLocation {
                Text = string.Format("{0}:{1}:{2}:{3}", program.Name, proc.Name, block.Name, block.Statements.IndexOf(stm))
            };
        }

        public string Text { get; set; }
        string ICodeLocation.Text
        {
            get { return this.Text; }
        }

        void ICodeLocation.NavigateTo()
        {
            throw new NotImplementedException();
        }
    }
}
