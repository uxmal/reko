using Bridge;
using Electron.edge_js;
using Newtonsoft.Json;
//using Reko.Gui.Electron.Adapter;
using Retyped;
using Retyped.Primitive;
using System;
using System.Threading.Tasks;
using static Electron.edge_js.SharpFunction;

using path = Retyped.node.path;

namespace Electron
{
    public class App
    {
        // Relative to project root directory, NOT index.html
        private const string ASSEMBLIES_RELPATH = "generated/assemblies/";

        private const string AdapterType = "Reko.Gui.Electron.Adapter.ElectronDecompilerDriver";

        /**
         * Bridge.NET fails with this approach. It will error out saying there's a circular dependency in System.dll
         * Patched: it will create an undefined reference to 'Reko' in JS (won't output metadata)
         * 
         * To try anyways add, as copy local:
         * - Electron.Adapter
         * - Accessibility
         * - Microsoft.CSharp
         * - System.Xml.dll
         * - System.dll
         * - System.Configuration.dll
         * - System.Data.SqlXml
         * - System.Deployment
         * - System.Drawing
         * - System.Dynamic
         * - System.Runtime.Serialization.Formatters.Soap
         * - System.Security
         * - System.Windows.Forms
         * - System.Xml
         */
        //private static readonly string AdapterType = typeof(ElectronDecompilerDriver).FullName;

        private static readonly string AppConfig = path.resolve(ASSEMBLIES_RELPATH + "/reko.config");
        private static readonly string AdapterAssembly = path.resolve(ASSEMBLIES_RELPATH + "/Reko.Gui.Electron.Adapter.dll");

        private static void OnNotify(object data, EdgeCallback cb)
        {
            cb(null, true);
        }

        [Init(InitPosition.Top)]
        public static void InitGlobals()
        {
            // The call below is required to initialize a global var 'Electron'.
            var Electron = (electron.Electron.AllElectron)node.require.Self("electron");
            
            // Keep a global reference of the window object, if you don't, the window will
            // be closed automatically when the      object is garbage collected.
            electron.Electron.BrowserWindow win = null;
        }

        [Template("Electron")]
        public static electron.Electron.AllElectron Electron;

        [Template("win")]
        public static electron.Electron.BrowserWindow Win;

        public static void Main()
        {
            var app = Electron.app;

            Console.WriteLine("Hello World");

            SharpAssembly x = new SharpAssembly(AdapterAssembly);
            SharpFunction CreateReko = x.GetFunction(AdapterType, "CreateReko");

            CreateReko.InvokeAsync(new {
                appConfig = AppConfig,
                fileName = "E:/dec/Aberaham_0.dir/Aberaham.exe",
                notify = new Action<object, EdgeCallback>(OnNotify)
            }).then<object>((result) => {
                Console.WriteLine(Retyped.Primitive.Object.keys(result));
                return null;
            }, (error) => {
                es5.Error obj = (es5.Error)error;
                throw new Exception(obj.message);
            });
        }
    }
}
 