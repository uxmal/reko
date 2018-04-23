using Reko.Gui;
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
        private EventBus eventBus;

        public int EventsReceived { get; set; }

        public EventBusForm()
        {
            InitializeComponent();
            this.evt = new ManualResetEvent(false);
            this.engine = new Engine(evt);
            this.eventBus = new EventBus();
            this.thread = new Thread(new ThreadStart(engine.Grind));
            this.thread.Start();
            this.FormClosed += EventBusForm_FormClosed;
        }

        private void EventBusForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            engine.Cancel();
            MessageBox.Show($"Events sent {engine.EventsSent}, events received {this.EventsReceived}");
        }

        private void Engine_SomethingChanged(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
                return;

            this.Invoke(new Action(() =>
            {
                listBox1.Items.Add($"Event {DateTime.Now}");
                ++EventsReceived;
                Thread.Sleep(1000);
            }));

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add("Started");
            evt.Set();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add("Paused");
            evt.Reset();
        }

        private class Engine
        {
            private ManualResetEvent evt;
            private bool cancel;

            public int EventsSent { get; set; }

            public Engine(ManualResetEvent evt)
            {
                this.evt = evt;
            }

            public void Cancel()
            {
                this.cancel = true;
            }

            public void Grind()
            {
                Debug.Print("Stariting grind");
                while (!cancel && evt.WaitOne())
                {
                    // spam with events.
                    Debug.Print("Firing event");
                    SomethingChanged?.Invoke(this, EventArgs.Empty);
                    ++EventsSent;
                    Thread.Sleep(200);
                }
                Debug.Print("Ending grind");

            }

            public event EventHandler SomethingChanged;

        }

        private void chkUseEventBus_CheckedChanged(object sender, EventArgs e)
        {
            this.engine.SomethingChanged -= this.Engine_SomethingChanged;
            this.eventBus.RegisterSingleEventMailbox(h => engine.SomethingChanged += h, this.Engine_SomethingChanged);
        }
    }
}
