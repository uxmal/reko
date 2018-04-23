using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    public partial class EventBusForm : Form
    {
        private ManualResetEvent evt;
        private Engine engine;
        private Thread thread;

        public EventBusForm()
        {
            InitializeComponent();
            this.evt = new ManualResetEvent(false);
            this.engine = new Engine(evt);
            engine.SomethingChanged += Engine_SomethingChanged;
            this.thread = new Thread(new ThreadStart(engine.Grind));
            this.thread.Start();
            this.FormClosed += EventBusForm_FormClosed;
        }

        private void EventBusForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            engine.Cancel = true;
        }

        private void Engine_SomethingChanged(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
                return;

            this.Invoke(new Action(() =>
            {
                listBox1.Items.Add($"Event {DateTime.Now}");
                Thread.Sleep(1000);
            }));

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            evt.Set();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            evt.Reset();
        }

        private class Engine
        {
            private ManualResetEvent evt;

            public Engine(ManualResetEvent evt)
            {
                this.evt = evt;
            }

            public bool Cancel { get; set; }

            public void Grind()
            {
                Debug.Print("Stariting grind");
                while (!Cancel && evt.WaitOne())
                {
                    // spam with events.
                    Debug.Print("Firing event");
                    SomethingChanged?.Invoke(this, EventArgs.Empty);
                    Thread.Sleep(200);
                }
                Debug.Print("Ending grind");

            }

            public event EventHandler SomethingChanged;

        }
    }
}
