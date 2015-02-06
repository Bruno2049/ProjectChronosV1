using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ProjectChronos.Repositories;
using ProjectChronos.Models;

namespace ProjectChronos.Forms
{
    public partial class FinisherList : Form
    {
        FinishersRepo finishers = new FinishersRepo();
        List<Finisher> finisherCollection = new List<Finisher>();

        public FinisherList()
        {
            InitializeComponent();
        }

        private void populateListView()
        {
            finisherCollection = finishers.all();
            if (finisherCollection.Count <= 0)
            {
                MessageBox.Show("There's no finishers yet");
            }
            else
            {
                lstFinishers.Items.Clear();
                foreach (Finisher finisher in finisherCollection)
                {
                    lstFinishers.Items.Add(finisher.id.ToString());
                    lstFinishers.Items[lstFinishers.Items.Count - 1].SubItems.Add(finisher.racerNo.ToString());
                    lstFinishers.Items[lstFinishers.Items.Count - 1].SubItems.Add(finisher.racerName);
                    lstFinishers.Items[lstFinishers.Items.Count - 1].SubItems.Add(finisher.epc);
                   
                    DateTime d;
                    DateTime.TryParse(finisher.time, out d);
                    lstFinishers.Items[lstFinishers.Items.Count - 1].SubItems.Add(d.ToString("HH:mm:ss.fff"));
                }
            }
        }

        private void FinisherList_Load(object sender, EventArgs e)
        {
            populateListView();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Reports.frmFinishersReport rpt = new Reports.frmFinishersReport();
            rpt.Show();
        }
    }
}
