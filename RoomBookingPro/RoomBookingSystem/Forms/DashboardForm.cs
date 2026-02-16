using RoomBookingSystem.Data;
using RoomBookingSystem.Models;
using System.Data;

namespace RoomBookingSystem.Forms
{
    public partial class DashboardForm : Form
    {
        private readonly DatabaseHelper _db;

        public DashboardForm()
        {
            InitializeComponent();
            _db = new DatabaseHelper();
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            // Display welcome message
            lblWelcome.Text = $"Welcome, {User.CurrentUser?.FullName}!";
            
            // Load dashboard statistics
            LoadStats();
        }

        private void LoadStats()
        {
            try
            {
                // Get dashboard statistics using stored procedure
                string query = "CALL sp_GetDashboardStats()";
                DataTable dt = _db.ExecuteQuery(query);
                
                if (dt.Rows.Count > 0)
                {
                    lblAvailableRooms.Text = $"Available Rooms: {dt.Rows[0]["AvailableRooms"]}";
                    lblOccupiedRooms.Text = $"Occupied Rooms: {dt.Rows[0]["OccupiedRooms"]}";
                    lblActiveBookings.Text = $"Active Bookings: {dt.Rows[0]["ActiveBookings"]}";
                    lblTotalCustomers.Text = $"Total Customers: {dt.Rows[0]["TotalCustomers"]}";
                    lblMonthlyRevenue.Text = $"Monthly Revenue: ${Convert.ToDecimal(dt.Rows[0]["MonthlyRevenue"]):N2}";
                    lblPendingMaintenance.Text = $"Pending Maintenance: {dt.Rows[0]["PendingMaintenance"]}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}\n\nYou can still use the system.", 
                    "Dashboard Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Set default values if database query fails
                lblAvailableRooms.Text = "Available Rooms: --";
                lblOccupiedRooms.Text = "Occupied Rooms: --";
                lblActiveBookings.Text = "Active Bookings: --";
                lblTotalCustomers.Text = "Total Customers: --";
                lblMonthlyRevenue.Text = "Monthly Revenue: $0.00";
                lblPendingMaintenance.Text = "Pending Maintenance: --";
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadStats();
            MessageBox.Show("Dashboard refreshed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            CustomerManagementForm form = new CustomerManagementForm();
            form.ShowDialog();
            LoadStats(); // Refresh stats when returning
        }

        private void btnRooms_Click(object sender, EventArgs e)
        {
            RoomManagementForm form = new RoomManagementForm();
            form.ShowDialog();
            LoadStats();
        }

        private void btnBookings_Click(object sender, EventArgs e)
        {
            BookingManagementForm form = new BookingManagementForm();
            form.ShowDialog();
            LoadStats();
        }

        private void btnPayments_Click(object sender, EventArgs e)
        {
            PaymentManagementForm form = new PaymentManagementForm();
            form.ShowDialog();
            LoadStats();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            ReportsForm form = new ReportsForm();
            form.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                User.CurrentUser = null;
                this.Close();
            }
        }
    }
}
