using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectChronos
{
    public partial class FormAddReader : Form
    {
        public FormAddReader()
        {
            InitializeComponent();
        }

        

        private void FormAddReader_Load(object sender, EventArgs e)
        {
            txtAddress.Select();
            txtAddress.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
