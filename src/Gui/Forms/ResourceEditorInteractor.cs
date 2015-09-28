using System;
using System.Windows.Forms;
using Reko.Core;
using System.IO;

namespace Reko.Gui.Forms
{
    public class ResourceEditorInteractor : IWindowPane
    {
        private IResourceEditor dlg;
        private ProgramResourceInstance resource;
        private IServiceProvider services;

        public ResourceEditorInteractor(ProgramResourceInstance resource)
        {
            this.resource = resource;
        }

        public void Close()
        {
            if (dlg != null)
                dlg.Dispose();
        }

        public Control CreateControl()
        {
            dlg = services.RequireService<IDialogFactory>().CreateResourceEditor();
            var stm = new MemoryStream();
            var bw = new BinaryWriter(stm);
            bw.Write('B');
            bw.Write('M');
            bw.Write(14 + resource.Bytes.Length);
            bw.Write(0);
            bw.Write(14);
            bw.Write(resource.Bytes, 0, resource.Bytes.Length);
            bw.Flush();
            try {
                dlg.Image = System.Drawing.Bitmap.FromStream(stm);
            } catch
            {
            }
            return (Control)dlg;
        }

        public void SetSite(IServiceProvider sp)
        {
            this.services = sp;
        }
    }
}