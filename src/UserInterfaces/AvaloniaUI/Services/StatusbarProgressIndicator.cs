using Reko.Core.Output;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    /// <summary>
    /// Shows a background process' progress in the Avalonia
    /// status bar.
    /// </summary>
    public class StatusbarProgressIndicator : IProgressIndicator
    {
        private readonly IStatusBarService sbSvc;
        private readonly SynchronizationContext uiSyncCtx;
        private int position;
        private int total;


        public StatusbarProgressIndicator(IStatusBarService sbSvc)
        {
            this.sbSvc = sbSvc;
            var uiSyncCtx = SynchronizationContext.Current;
            if (uiSyncCtx is null)
                throw new InvalidOperationException("No SynchronizationContext.");
            this.uiSyncCtx = uiSyncCtx;
        }

        public void Advance(int count)
        {
            uiSyncCtx?.Post(delegate
            {
                this.position += count;
                if (total > 0)
                {
                    var percentDone = Math.Min(
                        100,
                        (int) ((position * 100L) / total));
                }
            },
            null);
        }

        public void ShowProgress(string caption, int numerator, int denominator)
        {
            uiSyncCtx?.Post(delegate
            {
                sbSvc.SetSubtext(caption);
                this.position = numerator;
                this.total = denominator;
                if (denominator > 0)
                {
                    var percentDone = Math.Min(
                        100,
                        (int) ((numerator * 100L) / denominator));
                    sbSvc.ShowProgress(percentDone);
                }
            },
            null);
        }

        public void ShowStatus(string newStatus)
        {
            uiSyncCtx?.Post(delegate
            {
                sbSvc.SetSubtext(newStatus);
            }, null);
        }

        /// <summary>
        /// Intended to be call by a worker thread; uses Post to make sure the delegate is invoked
        /// on the UI thread.
        /// </summary>
        /// <param name="newCaption"></param>
        public void SetCaption(string newCaption)
        {
            uiSyncCtx?.Post(delegate
            {
                sbSvc.SetText(newCaption);
                sbSvc.HideProgress();
            }, null);
        }

        public void Finish()
        {
        }
    }
}
