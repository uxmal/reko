using Bridge;
using Retyped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Electron.edge_js.SharpFunction;

namespace Electron.edge_js
{
    public class SharpAssembly
    {
        [Init(InitPosition.Top)]
        public static void InitGlobals()
        {
            // The call below is required to initialize a global var 'Edge'.
            var Edge = node.require.Self("electron-edge-js");
        }

        [Template("Edge")]
        public static object Edge;

        private string assembly;

        public SharpAssembly(string assemblyFile)
        {
            this.assembly = assemblyFile;
        }

        public SharpFunction GetFunction(string typeName, string methodName)
        {
            Action<object, EdgeCallback> clrMethod = Edge.ToDynamic().func(
                new
                {
                    assemblyFile = this.assembly,
                    typeName = typeName,
                    methodName = methodName
                });

            return new SharpFunction(clrMethod);
        }
    }
}
