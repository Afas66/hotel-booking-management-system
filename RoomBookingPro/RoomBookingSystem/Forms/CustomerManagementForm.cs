using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using RoomBookingSystem.Data;
using RoomBookingSystem.Models;

namespace RoomBookingSystem.Forms
{
    public partial class CustomerManagementForm : Form
    {
        private readonly DatabaseHelper _db;
        private int _selectedCustomerId = 0;

        public CustomerManagementForm()
        {
            InitializeComponent();
            _db = new DatabaseHelper();
        }

        private void CustomerManagementForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCustomers();
                ConfigureDataGridView();
                ClearForm();
            }
            catch (Exception ex)
            {
                ShowError("Failed to initialize form", ex);
            }
        }

        private void ConfigureDataGridView()
        {
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.ReadOnly = true;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadCustomers(string searchTerm = "")
        {
            try
            {
                string query = @"
                    SELECT 
                        CustomerID,
                        FullName,
                        Email,
                        Phone,
                        Address,
                        IDNumber,
                        EmergencyContact,
                        EmergencyPhone,
                        DateOfBirth,
                        Gender,
                        Occupation,
                        CASE WHEN IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                        CreatedDate
                    FROM Customers
                    WHERE IsActive = 1";

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query += @" AND (FullName LIKE @search 
                                  OR Phone LIKE @search 
                                  OR Email LIKE @search 
                                  OR IDNumber LIKE @search)";
                }

                query += " ORDER BY FullName";

                MySqlParameter[] parameters = null;
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    parameters = new MySqlParameter[] {
                        new MySqlParameter("@search", $"%{searchTerm}%")
                    };
                }

                DataTable dt = _db.ExecuteQuery(query, parameters);
                dgvCustomers.DataSource = dt;

                // Hide CustomerID column
                if (dgvCustomers.Columns.Contains("CustomerID"))
                {
                    dgvCustomers.Columns["CustomerID"].Visible = false;
                }

                lblRecordCount.Text = $"Total Customers: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                ShowError("Failed to load customers", ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                string query = @"
                    INSERT INTO Customers 
                    (FullName, Email, Phone, Address, IDNumber, EmergencyContact, 
                     EmergencyPhone, DateOfBirth, Gender, Occupation, IsActive)
                    VALUES 
                    (@FullName, @Email, @Phone, @Address, @IDNumber, @EmergencyContact, 
                     @EmergencyPhone, @DateOfBirth, @Gender, @Occupation, 1)";

                MySqlParameter[] parameters = GetCustomerParameters();
                int result = _db.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    MessageBox.Show("Customer added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomers();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to add customer. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062) // Duplicate entry
            {
                MessageBox.Show("A customer with this email or phone already exists.", "Duplicate Entry",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                ShowError("Failed to add customer", ex);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomerId == 0)
                {
                    MessageBox.Show("Please select a customer to update.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateInput())
                    return;

                string query = @"
                    UPDATE Customers 
                    SET FullName = @FullName,
                        Email = @Email,
                        Phone = @Phone,
                        Address = @Address,
                        IDNumber = @IDNumber,
                        EmergencyContact = @EmergencyContact,
                        EmergencyPhone = @EmergencyPhone,
                        DateOfBirth = @DateOfBirth,
                        Gender = @Gender,
                        Occupation = @Occupation
                    WHERE CustomerID = @CustomerID";

                var parameters = GetCustomerParameters();
                Array.Resize(ref parameters, parameters.Length + 1);
                parameters[parameters.Length - 1] = new MySqlParameter("@CustomerID", _selectedCustomerId);

                int result = _db.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    MessageBox.Show("Customer updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomers();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to update customer. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("A customer with this email or phone already exists.", "Duplicate Entry",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                ShowError("Failed to update customer", ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomerId == 0)
                {
                    MessageBox.Show("Please select a customer to delete.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if customer has active bookings
                string checkQuery = "SELECT COUNT(*) FROM Bookings WHERE CustomerID = @CustomerID AND Status = 'Active'";
                MySqlParameter[] checkParams = { new MySqlParameter("@CustomerID", _selectedCustomerId) };
                object result = _db.ExecuteScalar(checkQuery, checkParams);

                if (result != null && Convert.ToInt32(result) > 0)
                {
                    MessageBox.Show("Cannot delete customer with active bookings. Please complete or cancel bookings first.",
                        "Active Bookings", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult dialogResult = MessageBox.Show(
                    "Are you sure you want to delete this customer?\nThis action cannot be undone.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    // Soft delete
                    string query = "UPDATE Customers SET IsActive = 0 WHERE CustomerID = @CustomerID";
                    MySqlParameter[] parameters = { new MySqlParameter("@CustomerID", _selectedCustomerId) };

                    int deleteResult = _db.ExecuteNonQuery(query, parameters);

                    if (deleteResult > 0)
                    {
                        MessageBox.Show("Customer deleted successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCustomers();
                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to delete customer", ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadCustomers(txtSearch.Text.Trim());
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            txtSearch.Clear();
            LoadCustomers();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvCustomers.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dgvCustomers.SelectedRows[0];

                    _selectedCustomerId = Convert.ToInt32(row.Cells["CustomerID"].Value);
                    txtFullName.Text = row.Cells["FullName"].Value?.ToString() ?? "";
                    txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
                    txtPhone.Text = row.Cells["Phone"].Value?.ToString() ?? "";
                    txtAddress.Text = row.Cells["Address"].Value?.ToString() ?? "";
                    txtIDNumber.Text = row.Cells["IDNumber"].Value?.ToString() ?? "";
                    txtEmergencyContact.Text = row.Cells["EmergencyContact"].Value?.ToString() ?? "";
                    txtEmergencyPhone.Text = row.Cells["EmergencyPhone"].Value?.ToString() ?? "";
                    txtOccupation.Text = row.Cells["Occupation"].Value?.ToString() ?? "";

                    if (row.Cells["DateOfBirth"].Value != DBNull.Value)
                    {
                        dtpDateOfBirth.Value = Convert.ToDateTime(row.Cells["DateOfBirth"].Value);
                        chkDateOfBirth.Checked = true;
                    }
                    else
                    {
                        chkDateOfBirth.Checked = false;
                    }

                    string gender = row.Cells["Gender"].Value?.ToString() ?? "";
                    cmbGender.SelectedItem = gender;
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load customer details", ex);
            }
        }

        private void ClearForm()
        {
            _selectedCustomerId = 0;
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtIDNumber.Clear();
            txtEmergencyContact.Clear();
            txtEmergencyPhone.Clear();
            txtOccupation.Clear();
            chkDateOfBirth.Checked = false;
            dtpDateOfBirth.Value = DateTime.Now;
            cmbGender.SelectedIndex = -1;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Please enter customer's full name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Please enter customer's phone number.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private MySqlParameter[] GetCustomerParameters()
        {
            return new MySqlParameter[]
            {
        new MySqlParameter("@FullName", txtFullName.Text.Trim()),
        new MySqlParameter("@Email", string.IsNullOrWhiteSpace(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text.Trim()),
        new MySqlParameter("@Phone", txtPhone.Text.Trim()),
        new MySqlParameter("@Address", string.IsNullOrWhiteSpace(txtAddress.Text) ? (object)DBNull.Value : txtAddress.Text.Trim()),
        new MySqlParameter("@IDNumber", string.IsNullOrWhiteSpace(txtIDNumber.Text) ? (object)DBNull.Value : txtIDNumber.Text.Trim()),
        new MySqlParameter("@EmergencyContact", string.IsNullOrWhiteSpace(txtEmergencyContact.Text) ? (object)DBNull.Value : txtEmergencyContact.Text.Trim()),
        new MySqlParameter("@EmergencyPhone", string.IsNullOrWhiteSpace(txtEmergencyPhone.Text) ? (object)DBNull.Value : txtEmergencyPhone.Text.Trim()),
        new MySqlParameter("@DateOfBirth", chkDateOfBirth.Checked ? (object)dtpDateOfBirth.Value : DBNull.Value),
        new MySqlParameter("@Gender", cmbGender.SelectedItem != null ? (object)cmbGender.SelectedItem.ToString() : DBNull.Value),
        new MySqlParameter("@Occupation", string.IsNullOrWhiteSpace(txtOccupation.Text) ? (object)DBNull.Value : txtOccupation.Text.Trim())
            };
        }

        private void ShowError(string message, Exception ex)
        {
            string errorMessage = $"{message}\n\nError: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"\n\nDetails: {ex.InnerException.Message}";
            }
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
