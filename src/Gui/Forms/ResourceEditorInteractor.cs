using System;
using System.Windows.Forms;
using Reko.Core;
using System.IO;
using Reko.Gui.Windows.Controls;
using System.Drawing;

namespace Reko.Gui.Forms
{
    public class ResourceEditorInteractor : IWindowPane
    {
        private Program program;
        private IResourceEditor dlg;
        private ProgramResourceInstance resource;
        private IServiceProvider services;

        public ResourceEditorInteractor(Program program, ProgramResourceInstance resource)
        {
            this.program = program;
            this.resource = resource;
        }

        public IWindowFrame Frame { get; set; }

        public void Close()
        {
            if (dlg != null)
                dlg.Dispose();
        }

        public Control CreateControl()
        {
            try
            {
                dlg = services.RequireService<IDialogFactory>().CreateResourceEditor();
                switch (resource.Type)
                {
                case "Windows.BMP": GenerateWindowsBitmap(); break;
                case "Windows.ICO": GenerateWindowsIcon(); break;
                default: return DumpBytes();
                }
            }
            catch
            {
                return DumpBytes();
            }
            return (Control)dlg;
        }

        private void GenerateWindowsBitmap()
        {
            var stm = new MemoryStream(resource.Bytes);
            dlg.Image = Image.FromStream(stm);
        }

        private void GenerateWindowsIcon()
        {
            var stm = new MemoryStream();
            var icon = new Icon(new MemoryStream(resource.Bytes));
            dlg.Image = Bitmap.FromHicon(icon.Handle);
            icon.Dispose();
        }

        Control DumpBytes()
        {
            var mem = new Reko.Core.MemoryArea(Address.Ptr32(0), resource.Bytes);

            var memCtrl = new MemoryControl();
            memCtrl.Services = services;
            memCtrl.ImageMap = new ImageMap(
                mem.BaseAddress,
                new ImageSegment("resource", mem, AccessMode.Read));
            memCtrl.Architecture = program.Architecture;
            memCtrl.Font = new Font("Lucida Console", 10F); //$TODO: use user preference
            return memCtrl;
        }

        public void SetSite(IServiceProvider sp)
        {
            this.services = sp;
        }
    }
}