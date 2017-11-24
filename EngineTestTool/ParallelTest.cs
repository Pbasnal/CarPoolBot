using Bot.Data;
using Bot.Data.Models;
using Bot.MessagingFramework;
using Bot.Worker;
using Bot.Worker.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EngineTestTool
{
    public partial class ParallelTest : Form
    {
        public ParallelTest()
        {
            InitializeComponent();
        }

        private void StartProcessButton_Click(object sender, EventArgs e)
        {
            MessageBus.Instance.Publish(new ProcessTripOwnerRequestMessage
            {
                TripOwnerRequest = new TripRequest()
            });

            var t = DateTime.UtcNow;
            while (DateTime.UtcNow - t < TimeSpan.FromSeconds(5))
            {
               // PoolingEngine.get
            }
        }
    }
}
