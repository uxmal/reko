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
            var bw = new BinaryWriter(stm);
            var icon = new Icon(new MemoryStream(resource.Bytes));
            dlg.Image = Bitmap.FromHicon(icon.Handle);
            icon.Dispose();
        }

        Control DumpBytes()
        {
            var mem = new MemoryControl();
            mem.Services = services;
            mem.ProgramImage = new Reko.Core.LoadedImage(Address.Ptr32(0), resource.Bytes);
            mem.ImageMap = mem.ProgramImage.CreateImageMap();
            mem.Architecture = program.Architecture;
            mem.Font = new Font("Lucida Console", 10F); //$TODO: use user preference
            return mem;
        }

        public void SetSite(IServiceProvider sp)
        {
            this.services = sp;
        }
    }
}