using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectChronos.Reports
{
    public partial class frmFinishersReport : Form
    {
        public frmFinishersReport()
        {
            InitializeComponent();
        }

        private void frmFinishersReport_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'chronosDataSet.rpt_FinisherList' table. You can move, or remove it, as needed.
            this.rpt_FinisherListTableAdapter.Fill(this.chronosDataSet.rpt_FinisherList);

            this.reportViewer1.RefreshReport();
        }
    }
}
