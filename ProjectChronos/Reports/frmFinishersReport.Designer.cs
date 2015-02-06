namespace ProjectChronos.Reports
{
    partial class frmFinishersReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.chronosDataSet = new ProjectChronos.chronosDataSet();
            this.rptFinisherListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.rpt_FinisherListTableAdapter = new ProjectChronos.chronosDataSetTableAdapters.rpt_FinisherListTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.chronosDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rptFinisherListBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "DataSet1";
            reportDataSource1.Value = this.rptFinisherListBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "ProjectChronos.Reports.rptFinishers.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(841, 475);
            this.reportViewer1.TabIndex = 0;
            // 
            // chronosDataSet
            // 
            this.chronosDataSet.DataSetName = "chronosDataSet";
            this.chronosDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // rptFinisherListBindingSource
            // 
            this.rptFinisherListBindingSource.DataMember = "rpt_FinisherList";
            this.rptFinisherListBindingSource.DataSource = this.chronosDataSet;
            // 
            // rpt_FinisherListTableAdapter
            // 
            this.rpt_FinisherListTableAdapter.ClearBeforeFill = true;
            // 
            // frmFinishersReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 475);
            this.Controls.Add(this.reportViewer1);
            this.Name = "frmFinishersReport";
            this.Text = "Finishers Report";
            this.Load += new System.EventHandler(this.frmFinishersReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chronosDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rptFinisherListBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private chronosDataSet chronosDataSet;
        private System.Windows.Forms.BindingSource rptFinisherListBindingSource;
        private chronosDataSetTableAdapters.rpt_FinisherListTableAdapter rpt_FinisherListTableAdapter;
    }
}