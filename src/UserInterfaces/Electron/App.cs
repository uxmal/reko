using Bridge;
using ElectronUI.edge_js;
using Newtonsoft.Json;
//using Reko.Gui.Electron.Adapter;
using Retyped;
using Retyped.Primitive;
using System;
using System.Threading.Tasks;
using static ElectronUI.edge_js.SharpFunction;

using path = Retyped.node.path;
using lit = Retyped.electron.Literals;
using static Retyped.electron.NodeJS;
using static Retyped.electron.Electron;

namespace ElectronUI
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
            node.require.Self("./bridge.js");

            // The call below is required to initialize a global var 'Electron'.
            var _Electron = (AllElectron)node.require.Self("electron");

            // Keep a global reference of the window object, if you don't, the window will
            // be closed automatically when the      object is garbage collected.
            BrowserWindow _MainWindow = null;
        }

        [Template("_Electron")]
        public static AllElectron Electron;

        [Template("_MainWindow")]
        public static BrowserWindow MainWindow;

        private static void AfterInit()
        {
            Console.WriteLine("Hello World");
            MainWindow.focus();

            SharpAssembly x = new SharpAssembly(AdapterAssembly);
            SharpFunction CreateReko = x.GetFunction(AdapterType, "CreateReko");

            CreateReko.InvokeAsync(new {
                appConfig = AppConfig,
                fileName = "E:/dec/Aberaham_0.dir/Aberaham.exe",
                notify = new Action<object, EdgeCallback>(OnNotify)
            }).then<object>((result) => {
                Console.WriteLine("-- Obtained Methods from C# Assembly --");
                foreach (string prop in Retyped.Primitive.Object.keys(result)) {
                    object value = Script.Get(result, prop);
                    Console.WriteLine($"{prop} => {value}");
                }
                return null;
            }, (error) => {
                es5.Error obj = (es5.Error)error;
                throw new Exception(obj.message);
            });
        }

        private static void Ready()
        {
            Console.WriteLine("Ready!");

            MainWindow = new BrowserWindow(new BrowserWindowConstructorOptions() {
                width = 800,
                height = 600,
                title = "Reko Decompiler"
            });

            if (!MainWindow.webContents.isDevToolsOpened()) {
                MainWindow.webContents.openDevTools();
            }

            MainWindow.loadURL($"file://{node.__dirname}/../../app/index.html");

            MainWindow.webContents.on(lit.crashed, (ev, input) => {
                throw new Exception(ev.ToString());
            });

            MainWindow.webContents.on(lit.did_finish_load, () => AfterInit());
        }

        public static void Main()
        {
            var app = Electron.app;

            app.on(lit.window_all_closed, () => {
                if(node.process2.platform != "darwin") {
                    app.quit();
                }
            });

            app.on(lit.ready, () => Ready());
        }
    }
}
 