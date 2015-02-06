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
    public partial class frmRacerListForm : Form
    {
        private RacersRepo racersRepo = new RacersRepo();

        public frmRacerListForm()
        {
            InitializeComponent();
        }

        private void frmRacerListForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Racer racer = new Racer();
            racer.racerNo = Int32.Parse(txtRacerBib.Text);
            racer.racerName = txtRacerName.Text;
            racer.epc1 = txtRacerEpc1.Text;
            racer.epc2 = txtRacerEpc2.Text;
            int result = racersRepo.create(racer);

            if (result > 0)
            {
                MessageBox.Show(this,"Racer successfully added.", null, MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(this, "Error adding racer", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
    