using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ProjectChronos.Models;
using ProjectChronos.Repositories;

namespace ProjectChronos.Forms
{
    public partial class frmRacerList : Form
    {
        RacersRepo racers = new RacersRepo();
        List<Racer> racersCollection = new List<Racer>();

        public frmRacerList()
        {
            InitializeComponent();
        }

        private void populateListView()
        {
            racersCollection = racers.all();
            if (racersCollection.Count <= 0)
            {
                MessageBox.Show("There's not yet item on the racers list, please add data");
            }
            else
            {
                lstRacers.Items.Clear();
                foreach (Racer racer in racersCollection)
                {
                    lstRacers.Items.Add(racer.id.ToString());
                    lstRacers.Items[lstRacers.Items.Count - 1].SubItems.Add(racer.racerNo.ToString());
                    lstRacers.Items[lstRacers.Items.Count - 1].SubItems.Add(racer.racerName);
                    lstRacers.Items[lstRacers.Items.Count - 1].SubItems.Add(racer.epc1);
                    lstRacers.Items[lstRacers.Items.Count - 1].SubItems.Add(racer.epc2);
                }
            }
        }

        private void frmRacerList_Load(object sender, EventArgs e)
        {
            populateListView();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmRacerListForm frm = new frmRacerListForm();
            if(frm.ShowDialog() == DialogResult.OK)
            {
                populateListView();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstRacers.SelectedItems.Count > 0)
            {
                int id = Int32.Parse(lstRacers.SelectedItems[0].Text);
                int result = racers.delete(id);
                if (result > 0)
                {
                    MessageBox.Show(this, "Racer successfully deleted.", null, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    populateListView();
                }
                else
                {
                    MessageBox.Show(this, "Error deleting racer", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
