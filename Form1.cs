using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace delgatetutorial
{
    public partial class Form1 : Form
    {
        // Delegate for updating UI
        private delegate void UpdateUIDelegate(string message);

        // Event for notifying UI about events
        private event EventHandler<string> EventOccurred;

        // Flag to indicate if the event simulation is running
        private bool isRunning = false;

        // Thread for simulating events
        private Thread eventThread;

        private int standbyEventCount = 0;
        private int scheduledEventCount = 0;
        private int nonScheduledEventCount = 0;


        public Form1()
        {
            InitializeComponent();

            // Initialize event handlers
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            // Attach the event handler for event occurrences
            EventOccurred += OnEventOccurred;
        }

        private void OnEventOccurred(object sender, string message)
        {
            if (InvokeRequired)
            {
                // If called from a different thread, invoke on the UI thread
                BeginInvoke(new UpdateUIDelegate(UpdateUI), message);
            }
            else
            {
                // If called from the UI thread, directly update the UI
                UpdateUI(message);
            }
        }

        private void UpdateUI(string message)
        {
            // Update the UI by adding a new item to the list box
            listBoxEvents.Items.Insert(0, $"{DateTime.Now}: {message}");
        }

        private void SimulateEvents()
        {
            Random random = new Random();

            // Loop to simulate events while isRunning is true
            while (isRunning)
            {
                // Generate a random event type
                int eventType = random.Next(3);

                // Trigger the EventOccurred event with the corresponding message
                switch (eventType)
                {
                    case 0:
                        standbyEventCount++;
                        EventOccurred?.Invoke(this, $"Standby event (Count: {standbyEventCount})");
                        break;

                    case 1:
                        scheduledEventCount++;
                        EventOccurred?.Invoke(this, $"Scheduled time event (Count: {scheduledEventCount})");
                        break;

                    case 2:
                        nonScheduledEventCount++;
                        EventOccurred?.Invoke(this, $"Non-scheduled time event (Count: {nonScheduledEventCount})");
                        break;
                }

                // Simulate some delay between events
                Thread.Sleep(2000);
            }
        }

        private void btnStart_Click_1(object sender, EventArgs e)
        {
            // Start event simulation
            if (!isRunning)
            {
                isRunning = true;
                eventThread = new Thread(SimulateEvents);
                eventThread.Start();
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }
        }

        private void btnStop_Click_1(object sender, EventArgs e)
        {
            // Stop event simulation
            if (isRunning)
            {
                isRunning = false;
                eventThread.Join(); // Wait for the thread to finish
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ensure proper cleanup when the form is closing
            if (isRunning)
            {
                isRunning = false;
                eventThread.Join(); // Wait for the thread to finish
            }
        }
    }
}
