// ================================================================
// COMPLETE TEMPLATE FOR ALL REMAINING FORMS
// Copy each section to its own .cs file
// ================================================================

// ==================== DASHBOARD FORM ====================
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
            lblWelcome.Text = $"Welcome, {User.CurrentUser?.FullName} ({User.CurrentUser?.Role})";
            
            // Load dashboard statistics
            LoadStats();
            
            // Apply role-based permissions
            ApplyPermissions();
        }

        private void LoadStats()
        {
            try
            {
                string query = "CALL sp_GetDashboardStats()";
                DataTable dt = _db.ExecuteQuery(query);
                
                if (dt.Rows.Count > 0)
                {
                    lblAvailableRooms.Text = dt.Rows[0]["AvailableRooms"].ToString();
                    lblOccupiedRooms.Text = dt.Rows[0]["OccupiedRooms"].ToString();
                    lblActiveBookings.Text = dt.Rows[0]["ActiveBookings"].ToString();
                    lblTotalCustomers.Text = dt.Rows[0]["TotalCustomers"].ToString();
                    lblMonthlyRevenue.Text = "$" + Convert.ToDecimal(dt.Rows[0]["MonthlyRevenue"]).ToString("N2");
                    lblPendingMaintenance.Text = dt.Rows[0]["PendingMaintenance"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}");
            }
        }

        private void ApplyPermissions()
        {
            // Only admins can access user management
            btnUserManagement.Visible = User.IsAdmin();
        }

        // Navigation button handlers
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadStats();
            MessageBox.Show("Dashboard refreshed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

// ==================== CUSTOMER MANAGEMENT FORM ====================
using RoomBookingSystem.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace RoomBookingSystem.Forms
{
    public partial class CustomerManagementForm : Form
    {
        private readonly DatabaseHelper _db;
        private int? selectedCustomerId = null;

        public CustomerManagementForm()
        {
            InitializeComponent();
            _db = new DatabaseHelper();
        }

        private void CustomerManagementForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            LoadGenderComboBox();
        }

        // CREATE
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                string query = @"INSERT INTO Customers 
                                (FullName, Email, Phone, Address, IDNumber, EmergencyContact, 
                                EmergencyPhone, DateOfBirth, Gender, Occupation) 
                                VALUES (@name, @email, @phone, @address, @id, @ec, @ep, @dob, @gender, @occ)";

                MySqlParameter[] parameters = GetCustomerParameters();
                
                int result = _db.ExecuteNonQuery(query, parameters);
                
                if (result > 0)
                {
                    MessageBox.Show("Customer added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomers();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // READ
        private void LoadCustomers()
        {
            try
            {
                string query = @"SELECT CustomerID, FullName, Email, Phone, Address, IDNumber, 
                                Gender, Occupation, DATE_FORMAT(DateOfBirth, '%Y-%m-%d') AS DateOfBirth,
                                EmergencyContact, EmergencyPhone, 
                                CASE WHEN IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status
                                FROM Customers 
                                ORDER BY FullName";
                
                DataTable dt = _db.ExecuteQuery(query);
                dgvCustomers.DataSource = dt;
                
                // Format columns
                if (dgvCustomers.Columns.Contains("CustomerID"))
                    dgvCustomers.Columns["CustomerID"].Visible = false;
                    
                lblTotalRecords.Text = $"Total Customers: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}");
            }
        }

        // UPDATE
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedCustomerId == null)
            {
                MessageBox.Show("Please select a customer to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInput()) return;

            try
            {
                string query = @"UPDATE Customers SET 
                                FullName=@name, Email=@email, Phone=@phone, Address=@address, 
                                IDNumber=@id, EmergencyContact=@ec, EmergencyPhone=@ep, 
                                DateOfBirth=@dob, Gender=@gender, Occupation=@occ 
                                WHERE CustomerID=@customerId";

                List<MySqlParameter> paramList = new List<MySqlParameter>(GetCustomerParameters());
                paramList.Add(new MySqlParameter("@customerId", selectedCustomerId));
                
                int result = _db.ExecuteNonQuery(query, paramList.ToArray());
                
                if (result > 0)
                {
                    MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomers();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DELETE
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedCustomerId == null)
            {
                MessageBox.Show("Please select a customer to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this customer?\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM Customers WHERE CustomerID=@id";
                    MySqlParameter[] parameters = { new MySqlParameter("@id", selectedCustomerId) };
                    
                    int deleteResult = _db.ExecuteNonQuery(query, parameters);
                    
                    if (deleteResult > 0)
                    {
                        MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCustomers();
                        ClearForm();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting customer: {ex.Message}\n\nNote: Cannot delete customers with active bookings.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // HELPER METHODS
        private MySqlParameter[] GetCustomerParameters()
        {
            return new MySqlParameter[]
            {
                new MySqlParameter("@name", txtFullName.Text.Trim()),
                new MySqlParameter("@email", txtEmail.Text.Trim()),
                new MySqlParameter("@phone", txtPhone.Text.Trim()),
                new MySqlParameter("@address", txtAddress.Text.Trim()),
                new MySqlParameter("@id", txtIDNumber.Text.Trim()),
                new MySqlParameter("@ec", txtEmergencyContact.Text.Trim()),
                new MySqlParameter("@ep", txtEmergencyPhone.Text.Trim()),
                new MySqlParameter("@dob", dtpDateOfBirth.Value.Date),
                new MySqlParameter("@gender", cmbGender.SelectedItem?.ToString() ?? ""),
                new MySqlParameter("@occ", txtOccupation.Text.Trim())
            };
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Full Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Phone number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return false;
            }

            if (cmbGender.SelectedIndex == -1)
            {
                MessageBox.Show("Please select gender.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbGender.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            selectedCustomerId = null;
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtIDNumber.Clear();
            txtEmergencyContact.Clear();
            txtEmergencyPhone.Clear();
            txtOccupation.Clear();
            dtpDateOfBirth.Value = DateTime.Now.AddYears(-25);
            cmbGender.SelectedIndex = -1;
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void LoadGenderComboBox()
        {
            cmbGender.Items.AddRange(new string[] { "Male", "Female", "Other" });
        }

        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCustomers.Rows[e.RowIndex];
                
                selectedCustomerId = Convert.ToInt32(row.Cells["CustomerID"].Value);
                txtFullName.Text = row.Cells["FullName"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
                txtPhone.Text = row.Cells["Phone"].Value?.ToString() ?? "";
                txtAddress.Text = row.Cells["Address"].Value?.ToString() ?? "";
                txtIDNumber.Text = row.Cells["IDNumber"].Value?.ToString() ?? "";
                txtEmergencyContact.Text = row.Cells["EmergencyContact"].Value?.ToString() ?? "";
                txtEmergencyPhone.Text = row.Cells["EmergencyPhone"].Value?.ToString() ?? "";
                txtOccupation.Text = row.Cells["Occupation"].Value?.ToString() ?? "";
                
                if (DateTime.TryParse(row.Cells["DateOfBirth"].Value?.ToString(), out DateTime dob))
                    dtpDateOfBirth.Value = dob;
                
                string gender = row.Cells["Gender"].Value?.ToString() ?? "";
                cmbGender.SelectedItem = gender;
                
                btnAdd.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadCustomers();
                return;
            }

            try
            {
                string query = @"SELECT * FROM Customers 
                                WHERE FullName LIKE @search OR Phone LIKE @search OR Email LIKE @search
                                ORDER BY FullName";
                
                MySqlParameter[] parameters = {
                    new MySqlParameter("@search", "%" + txtSearch.Text.Trim() + "%")
                };
                
                dgvCustomers.DataSource = _db.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Search error: {ex.Message}");
            }
        }
    }
}

// Continue with remaining forms...
// (RoomManagementForm, BookingManagementForm, PaymentManagementForm, ReportsForm)
// Follow similar CRUD pattern as CustomerManagementForm

