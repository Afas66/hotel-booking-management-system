/***************************************************
 * PAYMENT MANAGEMENT FORM
 ***************************************************/
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using RoomBookingSystem.Data;
using RoomBookingSystem.Models;
namespace RoomBookingSystem.Forms
{
    public partial class PaymentManagementForm : Form
    {
        private readonly DatabaseHelper _db;
        private int _selectedPaymentId = 0;

        public PaymentManagementForm()
        {
            InitializeComponent();
            _db = new DatabaseHelper();
        }

        private void PaymentManagementForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadActiveBookings();
                LoadPayments();
                ConfigureDataGridView();
                PopulateComboBoxes();
                ClearForm();
            }
            catch (Exception ex)
            {
                ShowError("Failed to initialize form", ex);
            }
        }

        private void ConfigureDataGridView()
        {
            dgvPayments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPayments.MultiSelect = false;
            dgvPayments.ReadOnly = true;
            dgvPayments.AllowUserToAddRows = false;
            dgvPayments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void PopulateComboBoxes()
        {
            // Payment Methods
            cmbPaymentMethod.Items.Clear();
            cmbPaymentMethod.Items.AddRange(new string[] {
                "Cash", "Bank Transfer", "Credit Card", "Debit Card", "Mobile Payment"
            });
            cmbPaymentMethod.SelectedIndex = 0;

            // Payment Types
            cmbPaymentFor.Items.Clear();
            cmbPaymentFor.Items.AddRange(new string[] {
                "Deposit", "Monthly Rent", "Utilities", "Maintenance", "Other"
            });
            cmbPaymentFor.SelectedIndex = 1; // Monthly Rent
        }

        private void LoadActiveBookings()
        {
            try
            {
                string query = @"
                    SELECT 
                        bk.BookingID,
                        CONCAT(c.FullName, ' - ', bd.BedNumber, ' (', a.AreaName, ')') AS BookingInfo,
                        bk.MonthlyRent
                    FROM Bookings bk
                    JOIN Customers c ON bk.CustomerID = c.CustomerID
                    JOIN Beds bd ON bk.BedID = bd.BedID
                    JOIN Areas a ON bd.AreaID = a.AreaID
                    WHERE bk.Status IN ('Active', 'Pending')
                    ORDER BY c.FullName";

                DataTable dt = _db.ExecuteQuery(query);

                cmbBooking.DisplayMember = "BookingInfo";
                cmbBooking.ValueMember = "BookingID";
                cmbBooking.DataSource = dt;
                cmbBooking.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ShowError("Failed to load bookings", ex);
            }
        }

        private void LoadPayments()
        {
            try
            {
                string query = @"
                    SELECT 
                        p.PaymentID,
                        c.FullName AS Customer,
                        bd.BedNumber AS Room,
                        p.PaymentDate,
                        p.Amount,
                        p.PaymentMethod,
                        p.PaymentFor,
                        p.TransactionReference,
                        u.FullName AS ReceivedBy
                    FROM Payments p
                    JOIN Bookings bk ON p.BookingID = bk.BookingID
                    JOIN Customers c ON bk.CustomerID = c.CustomerID
                    JOIN Beds bd ON bk.BedID = bd.BedID
                    LEFT JOIN Users u ON p.ReceivedBy = u.UserID
                    ORDER BY p.PaymentDate DESC";

                DataTable dt = _db.ExecuteQuery(query);
                dgvPayments.DataSource = dt;

                if (dgvPayments.Columns.Contains("PaymentID"))
                {
                    dgvPayments.Columns["PaymentID"].Visible = false;
                }

                // Calculate total
                decimal total = 0;
                foreach (DataRow row in dt.Rows)
                {
                    total += Convert.ToDecimal(row["Amount"]);
                }

                lblRecordCount.Text = $"Total Payments: {dt.Rows.Count} | Total Amount: ${total:N2}";
            }
            catch (Exception ex)
            {
                ShowError("Failed to load payments", ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                string query = @"
                    INSERT INTO Payments 
                    (BookingID, PaymentDate, Amount, PaymentMethod, PaymentFor, 
                     TransactionReference, Notes, ReceivedBy)
                    VALUES 
                    (@BookingID, @PaymentDate, @Amount, @PaymentMethod, @PaymentFor, 
                     @TransactionReference, @Notes, @ReceivedBy)";

                MySqlParameter[] parameters = GetPaymentParameters();
                int result = _db.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    ShowSuccess("Payment recorded successfully!");
                    LoadPayments();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to record payment", ex);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedPaymentId == 0)
                {
                    MessageBox.Show("Please select a payment to update.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateInput())
                    return;

                string query = @"
                    UPDATE Payments 
                    SET PaymentDate = @PaymentDate,
                        Amount = @Amount,
                        PaymentMethod = @PaymentMethod,
                        PaymentFor = @PaymentFor,
                        TransactionReference = @TransactionReference,
                        Notes = @Notes
                    WHERE PaymentID = @PaymentID";

                var parameters = GetPaymentParameters();
                Array.Resize(ref parameters, parameters.Length + 1);
                parameters[parameters.Length - 1] = new MySqlParameter("@PaymentID", _selectedPaymentId);

                int result = _db.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    ShowSuccess("Payment updated successfully!");
                    LoadPayments();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to update payment", ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedPaymentId == 0)
                {
                    MessageBox.Show("Please select a payment to delete.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (ConfirmAction("Are you sure you want to delete this payment record?\nThis action cannot be undone."))
                {
                    string query = "DELETE FROM Payments WHERE PaymentID = @PaymentID";
                    MySqlParameter[] parameters = { new MySqlParameter("@PaymentID", _selectedPaymentId) };

                    int result = _db.ExecuteNonQuery(query, parameters);

                    if (result > 0)
                    {
                        ShowSuccess("Payment deleted successfully!");
                        LoadPayments();
                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to delete payment", ex);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbBooking_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBooking.SelectedValue != null && cmbBooking.SelectedValue != DBNull.Value)
            {
                DataRowView drv = (DataRowView)cmbBooking.SelectedItem;
                if (drv != null && drv.Row.Table.Columns.Contains("MonthlyRent"))
                {
                    decimal monthlyRent = Convert.ToDecimal(drv.Row["MonthlyRent"]);
                    if (nudAmount.Value == 0)
                    {
                        nudAmount.Value = monthlyRent;
                    }
                }
            }
        }

        private void dgvPayments_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvPayments.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dgvPayments.SelectedRows[0];

                    _selectedPaymentId = Convert.ToInt32(row.Cells["PaymentID"].Value);
                    dtpPaymentDate.Value = Convert.ToDateTime(row.Cells["PaymentDate"].Value);
                    nudAmount.Value = Convert.ToDecimal(row.Cells["Amount"].Value);
                    cmbPaymentMethod.SelectedItem = row.Cells["PaymentMethod"].Value?.ToString();
                    cmbPaymentFor.SelectedItem = row.Cells["PaymentFor"].Value?.ToString();
                    txtTransactionRef.Text = row.Cells["TransactionReference"].Value?.ToString() ?? "";

                    // Disable booking selection for updates
                    cmbBooking.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load payment details", ex);
            }
        }

        private void ClearForm()
        {
            _selectedPaymentId = 0;
            cmbBooking.SelectedIndex = -1;
            dtpPaymentDate.Value = DateTime.Now;
            nudAmount.Value = 0;
            cmbPaymentMethod.SelectedIndex = 0;
            cmbPaymentFor.SelectedIndex = 1;
            txtTransactionRef.Clear();
            txtNotes.Clear();

            cmbBooking.Enabled = true;
        }

        private bool ValidateInput()
        {
            if (cmbBooking.SelectedIndex == -1 && _selectedPaymentId == 0)
            {
                MessageBox.Show("Please select a booking.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbBooking.Focus();
                return false;
            }

            if (nudAmount.Value <= 0)
            {
                MessageBox.Show("Please enter a valid amount.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudAmount.Focus();
                return false;
            }

            return true;
        }

        private MySqlParameter[] GetPaymentParameters()
        {
            return new MySqlParameter[]
            {
        new MySqlParameter("@BookingID", cmbBooking.SelectedValue ?? _selectedPaymentId),
        new MySqlParameter("@PaymentDate", dtpPaymentDate.Value.Date),
        new MySqlParameter("@Amount", nudAmount.Value),
        new MySqlParameter("@PaymentMethod", cmbPaymentMethod.SelectedItem.ToString()),
        new MySqlParameter("@PaymentFor", cmbPaymentFor.SelectedItem.ToString()),
        new MySqlParameter("@TransactionReference", string.IsNullOrWhiteSpace(txtTransactionRef.Text) ? (object)DBNull.Value : txtTransactionRef.Text.Trim()),
        new MySqlParameter("@Notes", string.IsNullOrWhiteSpace(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text.Trim()),
        new MySqlParameter("@ReceivedBy", User.CurrentUser?.UserID ?? 1)
            };
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

        private bool ConfirmAction(string message)
        {
            return MessageBox.Show(message, "Confirm Action",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}
