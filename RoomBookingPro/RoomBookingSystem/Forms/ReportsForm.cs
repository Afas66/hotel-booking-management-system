/***************************************************
 * REPORTS FORM
 ***************************************************/
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using RoomBookingSystem.Data;
using RoomBookingSystem.Models;
using System.Text;

namespace RoomBookingSystem.Forms
{
    public partial class ReportsForm : Form
    {
        private readonly DatabaseHelper _db;

        public ReportsForm()
        {
            InitializeComponent();
            _db = new DatabaseHelper();
        }

        private void ReportsForm_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureDataGridView();
                dtpStartDate.Value = DateTime.Now.AddMonths(-1);
                dtpEndDate.Value = DateTime.Now;
            }
            catch (Exception ex)
            {
                ShowError("Failed to initialize form", ex);
            }
        }

        private void ConfigureDataGridView()
        {
            dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReport.ReadOnly = true;
            dgvReport.AllowUserToAddRows = false;
            dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnGenerateRevenue_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"
                    SELECT 
                        DATE_FORMAT(PaymentDate, '%Y-%m') AS Month,
                        SUM(CASE WHEN PaymentFor = 'Monthly Rent' THEN Amount ELSE 0 END) AS RentRevenue,
                        SUM(CASE WHEN PaymentFor = 'Deposit' THEN Amount ELSE 0 END) AS DepositRevenue,
                        SUM(CASE WHEN PaymentFor = 'Utilities' THEN Amount ELSE 0 END) AS UtilityRevenue,
                        SUM(CASE WHEN PaymentFor = 'Maintenance' THEN Amount ELSE 0 END) AS MaintenanceRevenue,
                        SUM(Amount) AS TotalRevenue,
                        COUNT(*) AS TotalTransactions
                    FROM Payments
                    WHERE PaymentDate BETWEEN @StartDate AND @EndDate
                    GROUP BY DATE_FORMAT(PaymentDate, '%Y-%m')
                    ORDER BY Month DESC";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@StartDate", dtpStartDate.Value.Date),
                    new MySqlParameter("@EndDate", dtpEndDate.Value.Date)
                };

                DataTable dt = _db.ExecuteQuery(query, parameters);
                dgvReport.DataSource = dt;
                lblReportTitle.Text = "Monthly Revenue Report";
                lblRecordCount.Text = $"Total Periods: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                ShowError("Failed to generate revenue report", ex);
            }
        }

        private void btnGenerateOccupancy_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"
                    SELECT 
                        a.AreaName,
                        rt.TypeName AS RoomType,
                        COUNT(CASE WHEN b.Status = 'Available' THEN 1 END) AS Available,
                        COUNT(CASE WHEN b.Status = 'Occupied' THEN 1 END) AS Occupied,
                        COUNT(CASE WHEN b.Status = 'Maintenance' THEN 1 END) AS Maintenance,
                        COUNT(CASE WHEN b.Status = 'Reserved' THEN 1 END) AS Reserved,
                        COUNT(*) AS TotalRooms,
                        CONCAT(ROUND(COUNT(CASE WHEN b.Status = 'Occupied' THEN 1 END) / COUNT(*) * 100, 2), '%') AS OccupancyRate
                    FROM Beds b
                    JOIN Areas a ON b.AreaID = a.AreaID
                    JOIN RoomTypes rt ON b.RoomTypeID = rt.RoomTypeID
                    GROUP BY a.AreaName, rt.TypeName
                    ORDER BY a.AreaName, rt.TypeName";

                DataTable dt = _db.ExecuteQuery(query);
                dgvReport.DataSource = dt;
                lblReportTitle.Text = "Occupancy Statistics";
                lblRecordCount.Text = $"Total Categories: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                ShowError("Failed to generate occupancy report", ex);
            }
        }

        private void btnGenerateCustomers_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"
                    SELECT 
                        c.FullName,
                        c.Email,
                        c.Phone,
                        COUNT(bk.BookingID) AS TotalBookings,
                        SUM(CASE WHEN bk.Status = 'Active' THEN 1 ELSE 0 END) AS ActiveBookings,
                        COALESCE(SUM(p.Amount), 0) AS TotalPaid,
                        c.CreatedDate AS RegistrationDate
                    FROM Customers c
                    LEFT JOIN Bookings bk ON c.CustomerID = bk.CustomerID
                    LEFT JOIN Payments p ON bk.BookingID = p.BookingID
                    WHERE c.IsActive = 1
                    GROUP BY c.CustomerID
                    ORDER BY TotalPaid DESC";

                DataTable dt = _db.ExecuteQuery(query);
                dgvReport.DataSource = dt;
                lblReportTitle.Text = "Customer Report";
                lblRecordCount.Text = $"Total Customers: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                ShowError("Failed to generate customer report", ex);
            }
        }

        private void btnGenerateBookings_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"
                    SELECT 
                        c.FullName AS Customer,
                        c.Phone,
                        bd.BedNumber,
                        a.AreaName,
                        bk.CheckInDate,
                        bk.CheckOutDate,
                        bk.MonthlyRent,
                        bk.Status,
                        DATEDIFF(IFNULL(bk.CheckOutDate, CURDATE()), bk.CheckInDate) AS DaysOccupied,
                        u.FullName AS ManagedBy
                    FROM Bookings bk
                    JOIN Customers c ON bk.CustomerID = c.CustomerID
                    JOIN Beds bd ON bk.BedID = bd.BedID
                    JOIN Areas a ON bd.AreaID = a.AreaID
                    LEFT JOIN Users u ON bk.CreatedBy = u.UserID
                    WHERE bk.BookingDate BETWEEN @StartDate AND @EndDate
                    ORDER BY bk.BookingDate DESC";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@StartDate", dtpStartDate.Value.Date),
                    new MySqlParameter("@EndDate", dtpEndDate.Value.Date)
                };

                DataTable dt = _db.ExecuteQuery(query, parameters);
                dgvReport.DataSource = dt;
                lblReportTitle.Text = "Bookings Report";
                lblRecordCount.Text = $"Total Bookings: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                ShowError("Failed to generate bookings report", ex);
            }
        }

        private void btnGenerateMaintenance_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"
                    SELECT 
                        bd.BedNumber,
                        a.AreaName,
                        m.IssueType,
                        m.Description,
                        m.Priority,
                        m.Status,
                        m.ReportedDate,
                        m.CompletedDate,
                        m.Cost,
                        u.FullName AS ReportedBy,
                        m.AssignedTo
                    FROM Maintenance m
                    JOIN Beds bd ON m.BedID = bd.BedID
                    JOIN Areas a ON bd.AreaID = a.AreaID
                    LEFT JOIN Users u ON m.ReportedBy = u.UserID
                    WHERE m.ReportedDate BETWEEN @StartDate AND @EndDate
                    ORDER BY 
                        CASE m.Priority 
                            WHEN 'Critical' THEN 1 
                            WHEN 'High' THEN 2 
                            WHEN 'Medium' THEN 3 
                            ELSE 4 
                        END,
                        m.ReportedDate DESC";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@StartDate", dtpStartDate.Value.Date),
                    new MySqlParameter("@EndDate", dtpEndDate.Value.Date)
                };

                DataTable dt = _db.ExecuteQuery(query, parameters);
                dgvReport.DataSource = dt;
                lblReportTitle.Text = "Maintenance Report";
                lblRecordCount.Text = $"Total Records: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                ShowError("Failed to generate maintenance report", ex);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvReport.DataSource == null)
                {
                    MessageBox.Show("Please generate a report first.", "No Data",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "CSV File|*.csv|Excel File|*.xlsx",
                    FileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    DataTable dt = (DataTable)dgvReport.DataSource;

                    if (sfd.FilterIndex == 1) // CSV
                    {
                        ExportToCSV(dt, sfd.FileName);
                    }
                    else // Excel (requires additional library)
                    {
                        MessageBox.Show("Excel export requires additional setup. Please use CSV export.",
                            "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to export report", ex);
            }
        }

        private void ExportToCSV(DataTable dt, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            // Headers
            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            // Rows
            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                    field?.ToString().Replace(",", ";") ?? "");
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(filePath, sb.ToString());
            ShowSuccess($"Report exported successfully to:\n{filePath}");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ShowError(string message, Exception ex)
        {
            MessageBox.Show($"{message}\n\nError: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
