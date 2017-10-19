using Bot.Worker;
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
            PoolingEngine.Instance.QueuePoolingRequest(null);

            var t = DateTime.UtcNow;
            while (DateTime.UtcNow - t < TimeSpan.FromSeconds(5))
            {
               // PoolingEngine.get
            }
        }
    }
}
