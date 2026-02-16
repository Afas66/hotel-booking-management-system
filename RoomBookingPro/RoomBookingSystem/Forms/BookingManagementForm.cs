/***************************************************
 * BOOKING MANAGEMENT FORM
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
    public partial class BookingManagementForm : Form
    {
        private readonly DatabaseHelper _db;
        private int _selectedBookingId = 0;

        public BookingManagementForm()
        {
            InitializeComponent();
            _db = new DatabaseHelper();
        }

        private void BookingManagementForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCustomers();
                LoadAvailableRooms();
                LoadBookings();
                ConfigureDataGridView();
                PopulateStatusComboBox();
                ClearForm();
            }
            catch (Exception ex)
            {
                ShowError("Failed to initialize form", ex);
            }
        }

        private void ConfigureDataGridView()
        {
            dgvBookings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBookings.MultiSelect = false;
            dgvBookings.ReadOnly = true;
            dgvBookings.AllowUserToAddRows = false;
            dgvBookings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void PopulateStatusComboBox()
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new string[] { "Pending", "Active", "Completed", "Cancelled" });
            cmbStatus.SelectedIndex = 0;
        }

        private void LoadCustomers()
        {
            try
            {
                string query = "SELECT CustomerID, FullName, Phone FROM Customers WHERE IsActive = 1 ORDER BY FullName";
                DataTable dt = _db.ExecuteQuery(query);

                cmbCustomer.DisplayMember = "FullName";
                cmbCustomer.ValueMember = "CustomerID";
                cmbCustomer.DataSource = dt;
                cmbCustomer.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ShowError("Failed to load customers", ex);
            }
        }

        private void LoadAvailableRooms()
        {
            try
            {
                string query = @"
                    SELECT 
                        b.BedID,
                        CONCAT(b.BedNumber, ' - ', a.AreaName, ' (', rt.TypeName, ') - $', b.Price) AS RoomInfo,
                        b.Price
                    FROM Beds b
                    JOIN Areas a ON b.AreaID = a.AreaID
                    JOIN RoomTypes rt ON b.RoomTypeID = rt.RoomTypeID
                    WHERE b.Status = 'Available'
                    ORDER BY a.AreaName, b.BedNumber";

                DataTable dt = _db.ExecuteQuery(query);

                cmbRoom.DisplayMember = "RoomInfo";
                cmbRoom.ValueMember = "BedID";
                cmbRoom.DataSource = dt;
                cmbRoom.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ShowError("Failed to load available rooms", ex);
            }
        }

        private void LoadBookings()
        {
            try
            {
                string query = @"
                    SELECT 
                        bk.BookingID,
                        c.FullName AS Customer,
                        c.Phone,
                        bd.BedNumber AS Room,
                        a.AreaName AS Area,
                        bk.CheckInDate,
                        bk.CheckOutDate,
                        bk.MonthlyRent,
                        bk.DepositAmount,
                        bk.Status,
                        u.FullName AS CreatedBy
                    FROM Bookings bk
                    JOIN Customers c ON bk.CustomerID = c.CustomerID
                    JOIN Beds bd ON bk.BedID = bd.BedID
                    JOIN Areas a ON bd.AreaID = a.AreaID
                    LEFT JOIN Users u ON bk.CreatedBy = u.UserID
                    ORDER BY bk.BookingDate DESC";

                DataTable dt = _db.ExecuteQuery(query);
                dgvBookings.DataSource = dt;

                if (dgvBookings.Columns.Contains("BookingID"))
                {
                    dgvBookings.Columns["BookingID"].Visible = false;
                }

                lblRecordCount.Text = $"Total Bookings: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                ShowError("Failed to load bookings", ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                // Check if room is still available
                int bedId = Convert.ToInt32(cmbRoom.SelectedValue);
                string checkQuery = "SELECT Status FROM Beds WHERE BedID = @BedID";
                MySqlParameter[] checkParams = { new MySqlParameter("@BedID", bedId) };
                object statusResult = _db.ExecuteScalar(checkQuery, checkParams);

                if (statusResult?.ToString() != "Available")
                {
                    MessageBox.Show("Selected room is no longer available.", "Room Unavailable",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LoadAvailableRooms();
                    return;
                }

                // Start transaction
                using (MySqlConnection conn = _db.GetConnection())
                {
                    conn.Open();
                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Insert booking
                            string bookingQuery = @"
                                INSERT INTO Bookings 
                                (CustomerID, BedID, CheckInDate, CheckOutDate, MonthlyRent, 
                                 DepositAmount, Status, Notes, CreatedBy)
                                VALUES 
                                (@CustomerID, @BedID, @CheckInDate, @CheckOutDate, @MonthlyRent, 
                                 @DepositAmount, @Status, @Notes, @CreatedBy);
                                SELECT LAST_INSERT_ID();";

                            using (MySqlCommand cmd = new MySqlCommand(bookingQuery, conn, transaction))
                            {
                                cmd.Parameters.AddRange(GetBookingParameters());
                                object bookingIdObj = cmd.ExecuteScalar();
                                int bookingId = Convert.ToInt32(bookingIdObj);

                                // Update room status
                                string updateRoomQuery = @"
                                    UPDATE Beds 
                                    SET Status = CASE 
                                        WHEN @BookingStatus = 'Active' THEN 'Occupied'
                                        WHEN @BookingStatus = 'Pending' THEN 'Reserved'
                                        ELSE Status 
                                    END
                                    WHERE BedID = @BedID";

                                using (MySqlCommand updateCmd = new MySqlCommand(updateRoomQuery, conn, transaction))
                                {
                                    updateCmd.Parameters.AddWithValue("@BookingStatus", cmbStatus.SelectedItem.ToString());
                                    updateCmd.Parameters.AddWithValue("@BedID", bedId);
                                    updateCmd.ExecuteNonQuery();
                                }

                                transaction.Commit();
                                ShowSuccess($"Booking created successfully! Booking ID: {bookingId}");
                                LoadBookings();
                                LoadAvailableRooms();
                                ClearForm();
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to create booking", ex);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedBookingId == 0)
                {
                    MessageBox.Show("Please select a booking to update.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateInput(isUpdate: true))
                    return;

                using (MySqlConnection conn = _db.GetConnection())
                {
                    conn.Open();
                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Get current booking info
                            string getCurrentQuery = "SELECT BedID, Status FROM Bookings WHERE BookingID = @BookingID";
                            MySqlCommand getCurrentCmd = new MySqlCommand(getCurrentQuery, conn, transaction);
                            getCurrentCmd.Parameters.AddWithValue("@BookingID", _selectedBookingId);

                            int currentBedId = 0;
                            string currentStatus = "";
                            using (var reader = getCurrentCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    currentBedId = reader.GetInt32("BedID");
                                    currentStatus = reader.GetString("Status");
                                }
                            }

                            string newStatus = cmbStatus.SelectedItem.ToString();

                            // Update booking
                            string updateQuery = @"
                                UPDATE Bookings 
                                SET CheckOutDate = @CheckOutDate,
                                    MonthlyRent = @MonthlyRent,
                                    DepositAmount = @DepositAmount,
                                    Status = @Status,
                                    Notes = @Notes
                                WHERE BookingID = @BookingID";

                            using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@CheckOutDate", chkCheckOut.Checked ? dtpCheckOut.Value : DBNull.Value);
                                cmd.Parameters.AddWithValue("@MonthlyRent", nudMonthlyRent.Value);
                                cmd.Parameters.AddWithValue("@DepositAmount", nudDeposit.Value);
                                cmd.Parameters.AddWithValue("@Status", newStatus);
                                cmd.Parameters.AddWithValue("@Notes", txtNotes.Text.Trim());
                                cmd.Parameters.AddWithValue("@BookingID", _selectedBookingId);
                                cmd.ExecuteNonQuery();
                            }

                            // Update room status if booking status changed
                            if (currentStatus != newStatus)
                            {
                                string newRoomStatus = newStatus switch
                                {
                                    "Active" => "Occupied",
                                    "Pending" => "Reserved",
                                    "Completed" => "Available",
                                    "Cancelled" => "Available",
                                    _ => "Available"
                                };

                                string updateRoomQuery = "UPDATE Beds SET Status = @Status WHERE BedID = @BedID";
                                using (MySqlCommand updateCmd = new MySqlCommand(updateRoomQuery, conn, transaction))
                                {
                                    updateCmd.Parameters.AddWithValue("@Status", newRoomStatus);
                                    updateCmd.Parameters.AddWithValue("@BedID", currentBedId);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            ShowSuccess("Booking updated successfully!");
                            LoadBookings();
                            LoadAvailableRooms();
                            ClearForm();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to update booking", ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedBookingId == 0)
                {
                    MessageBox.Show("Please select a booking to cancel.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (ConfirmAction("Are you sure you want to cancel this booking?"))
                {
                    using (MySqlConnection conn = _db.GetConnection())
                    {
                        conn.Open();
                        using (MySqlTransaction transaction = conn.BeginTransaction())
                        {
                            try
                            {
                                // Get bed ID
                                string getBedQuery = "SELECT BedID FROM Bookings WHERE BookingID = @BookingID";
                                MySqlCommand getBedCmd = new MySqlCommand(getBedQuery, conn, transaction);
                                getBedCmd.Parameters.AddWithValue("@BookingID", _selectedBookingId);
                                int bedId = Convert.ToInt32(getBedCmd.ExecuteScalar());

                                // Cancel booking
                                string cancelQuery = "UPDATE Bookings SET Status = 'Cancelled' WHERE BookingID = @BookingID";
                                MySqlCommand cancelCmd = new MySqlCommand(cancelQuery, conn, transaction);
                                cancelCmd.Parameters.AddWithValue("@BookingID", _selectedBookingId);
                                cancelCmd.ExecuteNonQuery();

                                // Update room status to available
                                string updateRoomQuery = "UPDATE Beds SET Status = 'Available' WHERE BedID = @BedID";
                                MySqlCommand updateCmd = new MySqlCommand(updateRoomQuery, conn, transaction);
                                updateCmd.Parameters.AddWithValue("@BedID", bedId);
                                updateCmd.ExecuteNonQuery();

                                transaction.Commit();
                                ShowSuccess("Booking cancelled successfully!");
                                LoadBookings();
                                LoadAvailableRooms();
                                ClearForm();
                            }
                            catch
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to cancel booking", ex);
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

        private void cmbRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRoom.SelectedValue != null && cmbRoom.SelectedValue != DBNull.Value)
            {
                DataRowView drv = (DataRowView)cmbRoom.SelectedItem;
                if (drv != null && drv.Row.Table.Columns.Contains("Price"))
                {
                    decimal price = Convert.ToDecimal(drv.Row["Price"]);
                    nudMonthlyRent.Value = price;
                    nudDeposit.Value = price; // Default deposit = one month rent
                }
            }
        }

        private void dgvBookings_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvBookings.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dgvBookings.SelectedRows[0];

                    _selectedBookingId = Convert.ToInt32(row.Cells["BookingID"].Value);

                    // Load customer (read-only for edit)
                    string customer = row.Cells["Customer"].Value?.ToString();
                    cmbCustomer.Text = customer;

                    // Load room (read-only for edit)
                    string room = row.Cells["Room"].Value?.ToString();
                    cmbRoom.Text = room;

                    dtpCheckIn.Value = Convert.ToDateTime(row.Cells["CheckInDate"].Value);

                    if (row.Cells["CheckOutDate"].Value != DBNull.Value)
                    {
                        dtpCheckOut.Value = Convert.ToDateTime(row.Cells["CheckOutDate"].Value);
                        chkCheckOut.Checked = true;
                    }
                    else
                    {
                        chkCheckOut.Checked = false;
                    }

                    nudMonthlyRent.Value = Convert.ToDecimal(row.Cells["MonthlyRent"].Value);
                    nudDeposit.Value = Convert.ToDecimal(row.Cells["DepositAmount"].Value);
                    cmbStatus.SelectedItem = row.Cells["Status"].Value?.ToString();

                    // Disable customer and room selection for updates
                    cmbCustomer.Enabled = false;
                    cmbRoom.Enabled = false;
                    dtpCheckIn.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load booking details", ex);
            }
        }

        private void ClearForm()
        {
            _selectedBookingId = 0;
            cmbCustomer.SelectedIndex = -1;
            cmbRoom.SelectedIndex = -1;
            dtpCheckIn.Value = DateTime.Now;
            dtpCheckOut.Value = DateTime.Now.AddMonths(1);
            chkCheckOut.Checked = false;
            nudMonthlyRent.Value = 0;
            nudDeposit.Value = 0;
            cmbStatus.SelectedIndex = 0;
            txtNotes.Clear();

            cmbCustomer.Enabled = true;
            cmbRoom.Enabled = true;
            dtpCheckIn.Enabled = true;
        }

        private bool ValidateInput(bool isUpdate = false)
        {
            if (!isUpdate)
            {
                if (cmbCustomer.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a customer.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbCustomer.Focus();
                    return false;
                }

                if (cmbRoom.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a room.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbRoom.Focus();
                    return false;
                }
            }

            if (chkCheckOut.Checked && dtpCheckOut.Value <= dtpCheckIn.Value)
            {
                MessageBox.Show("Check-out date must be after check-in date.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpCheckOut.Focus();
                return false;
            }

            if (nudMonthlyRent.Value <= 0)
            {
                MessageBox.Show("Please enter a valid monthly rent.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudMonthlyRent.Focus();
                return false;
            }

            return true;
        }

        private MySqlParameter[] GetBookingParameters()
        {
            return new MySqlParameter[]
            {
        new MySqlParameter("@CustomerID", cmbCustomer.SelectedValue),
        new MySqlParameter("@BedID", cmbRoom.SelectedValue),
        new MySqlParameter("@CheckInDate", dtpCheckIn.Value.Date),
        new MySqlParameter("@CheckOutDate", chkCheckOut.Checked ? (object)dtpCheckOut.Value.Date : DBNull.Value),
        new MySqlParameter("@MonthlyRent", nudMonthlyRent.Value),
        new MySqlParameter("@DepositAmount", nudDeposit.Value),
        new MySqlParameter("@Status", cmbStatus.SelectedItem.ToString()),
        new MySqlParameter("@Notes", string.IsNullOrWhiteSpace(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text.Trim()),
        new MySqlParameter("@CreatedBy", User.CurrentUser?.UserID ?? 1)
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