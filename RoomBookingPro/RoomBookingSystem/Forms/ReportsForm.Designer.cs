namespace RoomBookingSystem.Forms
{
    partial class ReportsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.dgvReport = new System.Windows.Forms.DataGridView();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.btnGenerateRevenue = new System.Windows.Forms.Button();
            this.btnGenerateOccupancy = new System.Windows.Forms.Button();
            this.btnGenerateCustomers = new System.Windows.Forms.Button();
            this.btnGenerateBookings = new System.Windows.Forms.Button();
            this.btnGenerateMaintenance = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblReportTitle = new System.Windows.Forms.Label();
            this.lblRecordCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvReport
            // 
            this.dgvReport.AllowUserToAddRows = false;
            this.dgvReport.AllowUserToDeleteRows = false;
            this.dgvReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReport.Location = new System.Drawing.Point(25, 140);
            this.dgvReport.Name = "dgvReport";
            this.dgvReport.ReadOnly = true;
            this.dgvReport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReport.Size = new System.Drawing.Size(1150, 420);
            this.dgvReport.TabIndex = 0;
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStartDate.Location = new System.Drawing.Point(90, 25);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(150, 23);
            this.dtpStartDate.TabIndex = 1;
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEndDate.Location = new System.Drawing.Point(310, 25);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(150, 23);
            this.dtpEndDate.TabIndex = 2;
            // 
            // btnGenerateRevenue
            // 
            this.btnGenerateRevenue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnGenerateRevenue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerateRevenue.ForeColor = System.Drawing.Color.White;
            this.btnGenerateRevenue.Location = new System.Drawing.Point(25, 90);
            this.btnGenerateRevenue.Name = "btnGenerateRevenue";
            this.btnGenerateRevenue.Size = new System.Drawing.Size(140, 35);
            this.btnGenerateRevenue.TabIndex = 3;
            this.btnGenerateRevenue.Text = "Revenue Report";
            this.btnGenerateRevenue.UseVisualStyleBackColor = false;
            this.btnGenerateRevenue.Click += new System.EventHandler(this.btnGenerateRevenue_Click);
            // 
            // btnGenerateOccupancy
            // 
            this.btnGenerateOccupancy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnGenerateOccupancy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerateOccupancy.ForeColor = System.Drawing.Color.White;
            this.btnGenerateOccupancy.Location = new System.Drawing.Point(180, 90);
            this.btnGenerateOccupancy.Name = "btnGenerateOccupancy";
            this.btnGenerateOccupancy.Size = new System.Drawing.Size(140, 35);
            this.btnGenerateOccupancy.TabIndex = 4;
            this.btnGenerateOccupancy.Text = "Occupancy Report";
            this.btnGenerateOccupancy.UseVisualStyleBackColor = false;
            this.btnGenerateOccupancy.Click += new System.EventHandler(this.btnGenerateOccupancy_Click);
            // 
            // btnGenerateCustomers
            // 
            this.btnGenerateCustomers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.btnGenerateCustomers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerateCustomers.ForeColor = System.Drawing.Color.White;
            this.btnGenerateCustomers.Location = new System.Drawing.Point(335, 90);
            this.btnGenerateCustomers.Name = "btnGenerateCustomers";
            this.btnGenerateCustomers.Size = new System.Drawing.Size(140, 35);
            this.btnGenerateCustomers.TabIndex = 5;
            this.btnGenerateCustomers.Text = "Customer Report";
            this.btnGenerateCustomers.UseVisualStyleBackColor = false;
            this.btnGenerateCustomers.Click += new System.EventHandler(this.btnGenerateCustomers_Click);
            // 
            // btnGenerateBookings
            // 
            this.btnGenerateBookings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            this.btnGenerateBookings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerateBookings.ForeColor = System.Drawing.Color.White;
            this.btnGenerateBookings.Location = new System.Drawing.Point(490, 90);
            this.btnGenerateBookings.Name = "btnGenerateBookings";
            this.btnGenerateBookings.Size = new System.Drawing.Size(140, 35);
            this.btnGenerateBookings.TabIndex = 6;
            this.btnGenerateBookings.Text = "Bookings Report";
            this.btnGenerateBookings.UseVisualStyleBackColor = false;
            this.btnGenerateBookings.Click += new System.EventHandler(this.btnGenerateBookings_Click);
            // 
            // btnGenerateMaintenance
            // 
            this.btnGenerateMaintenance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnGenerateMaintenance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerateMaintenance.ForeColor = System.Drawing.Color.White;
            this.btnGenerateMaintenance.Location = new System.Drawing.Point(645, 90);
            this.btnGenerateMaintenance.Name = "btnGenerateMaintenance";
            this.btnGenerateMaintenance.Size = new System.Drawing.Size(160, 35);
            this.btnGenerateMaintenance.TabIndex = 7;
            this.btnGenerateMaintenance.Text = "Maintenance Report";
            this.btnGenerateMaintenance.UseVisualStyleBackColor = false;
            this.btnGenerateMaintenance.Click += new System.EventHandler(this.btnGenerateMaintenance_Click);
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(160)))), ((int)(((byte)(133)))));
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(25, 575);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(120, 35);
            this.btnExport.TabIndex = 8;
            this.btnExport.Text = "Export to CSV";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(1085, 575);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 35);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblReportTitle
            // 
            this.lblReportTitle.AutoSize = true;
            this.lblReportTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblReportTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblReportTitle.Location = new System.Drawing.Point(25, 110);
            this.lblReportTitle.Name = "lblReportTitle";
            this.lblReportTitle.Size = new System.Drawing.Size(183, 20);
            this.lblReportTitle.TabIndex = 10;
            this.lblReportTitle.Text = "Select a report to generate";
            // 
            // lblRecordCount
            // 
            this.lblRecordCount.AutoSize = true;
            this.lblRecordCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblRecordCount.Location = new System.Drawing.Point(160, 583);
            this.lblRecordCount.Name = "lblRecordCount";
            this.lblRecordCount.Size = new System.Drawing.Size(90, 15);
            this.lblRecordCount.TabIndex = 11;
            this.lblRecordCount.Text = "Total Records: 0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 15);
            this.label1.TabIndex = 12;
            this.label1.Text = "Start Date:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(250, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 15);
            this.label2.TabIndex = 13;
            this.label2.Text = "End Date:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtpStartDate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpEndDate);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(25, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(480, 60);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Date Range Filter";
            // 
            // ReportsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 630);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblRecordCount);
            this.Controls.Add(this.lblReportTitle);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnGenerateMaintenance);
            this.Controls.Add(this.btnGenerateBookings);
            this.Controls.Add(this.btnGenerateCustomers);
            this.Controls.Add(this.btnGenerateOccupancy);
            this.Controls.Add(this.btnGenerateRevenue);
            this.Controls.Add(this.dgvReport);
            this.Name = "ReportsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reports";
            this.Load += new System.EventHandler(this.ReportsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvReport;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Button btnGenerateRevenue;
        private System.Windows.Forms.Button btnGenerateOccupancy;
        private System.Windows.Forms.Button btnGenerateCustomers;
        private System.Windows.Forms.Button btnGenerateBookings;
        private System.Windows.Forms.Button btnGenerateMaintenance;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblReportTitle;
        private System.Windows.Forms.Label lblRecordCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
