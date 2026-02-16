using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using RoomBookingSystem.Data;
using RoomBookingSystem.Models;

namespace RoomBookingSystem.Forms
{
    public partial class RoomManagementForm : Form
    {
        private readonly DatabaseHelper _db;
        private int _selectedBedId = 0;

        public RoomManagementForm()
        {
            InitializeComponent();
            _db = new DatabaseHelper();
        }

        private void RoomManagementForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadAreas();
                LoadRoomTypes();
                LoadRooms();
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
            dgvRooms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRooms.MultiSelect = false;
            dgvRooms.ReadOnly = true;
            dgvRooms.AllowUserToAddRows = false;
            dgvRooms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void PopulateStatusComboBox()
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new string[] { "Available", "Occupied", "Maintenance", "Reserved" });

            cmbFilterStatus.Items.Clear();
            cmbFilterStatus.Items.Add("All");
            cmbFilterStatus.Items.AddRange(new string[] { "Available", "Occupied", "Maintenance", "Reserved" });
            cmbFilterStatus.SelectedIndex = 0;
        }

        private void LoadAreas()
        {
            try
            {
                string query = "SELECT AreaID, AreaName, AreaCode FROM Areas WHERE IsActive = 1 ORDER BY AreaName";
                DataTable dt = _db.ExecuteQuery(query);

                cmbArea.DisplayMember = "AreaName";
                cmbArea.ValueMember = "AreaID";
                cmbArea.DataSource = dt;
                cmbArea.SelectedIndex = -1;

                // Load filter dropdown
                DataTable filterDt = dt.Copy();
                DataRow newRow = filterDt.NewRow();
                newRow["AreaID"] = 0;
                newRow["AreaName"] = "All Areas";
                filterDt.Rows.InsertAt(newRow, 0);

                cmbFilterArea.DisplayMember = "AreaName";
                cmbFilterArea.ValueMember = "AreaID";
                cmbFilterArea.DataSource = filterDt;
                cmbFilterArea.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowError("Failed to load areas", ex);
            }
        }

        private void LoadRoomTypes()
        {
            try
            {
                string query = "SELECT RoomTypeID, TypeName, BasePrice FROM RoomTypes WHERE IsActive = 1 ORDER BY TypeName";
                DataTable dt = _db.ExecuteQuery(query);

                cmbRoomType.DisplayMember = "TypeName";
                cmbRoomType.ValueMember = "RoomTypeID";
                cmbRoomType.DataSource = dt;
                cmbRoomType.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ShowError("Failed to load room types", ex);
            }
        }

        private void LoadRooms()
        {
            try
            {
                string query = @"
                    SELECT 
                        b.BedID,
                        b.BedNumber,
                        a.AreaName,
                        rt.TypeName AS RoomType,
                        b.Price,
                        b.Status,
                        b.Description,
                        b.LastMaintenance
                    FROM Beds b
                    JOIN Areas a ON b.AreaID = a.AreaID
                    JOIN RoomTypes rt ON b.RoomTypeID = rt.RoomTypeID
                    WHERE 1=1";

                List<MySqlParameter> parameters = new List<MySqlParameter>();

                // Filter by area
                if (cmbFilterArea.SelectedValue != null && Convert.ToInt32(cmbFilterArea.SelectedValue) > 0)
                {
                    query += " AND b.AreaID = @AreaID";
                    parameters.Add(new MySqlParameter("@AreaID", cmbFilterArea.SelectedValue));
                }

                // Filter by status
                if (cmbFilterStatus.SelectedIndex > 0)
                {
                    query += " AND b.Status = @Status";
                    parameters.Add(new MySqlParameter("@Status", cmbFilterStatus.SelectedItem.ToString()));
                }

                // Search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    query += " AND (b.BedNumber LIKE @Search OR b.Description LIKE @Search)";
                    parameters.Add(new MySqlParameter("@Search", $"%{txtSearch.Text}%"));
                }

                query += " ORDER BY a.AreaName, b.BedNumber";

                DataTable dt = _db.ExecuteQuery(query, parameters.ToArray());
                dgvRooms.DataSource = dt;

                if (dgvRooms.Columns.Contains("BedID"))
                {
                    dgvRooms.Columns["BedID"].Visible = false;
                }

                // Color code status
                foreach (DataGridViewRow row in dgvRooms.Rows)
                {
                    string status = row.Cells["Status"].Value?.ToString() ?? "";
                    switch (status)
                    {
                        case "Available":
                            row.DefaultCellStyle.BackColor = Color.LightGreen;
                            break;
                        case "Occupied":
                            row.DefaultCellStyle.BackColor = Color.LightCoral;
                            break;
                        case "Maintenance":
                            row.DefaultCellStyle.BackColor = Color.LightYellow;
                            break;
                        case "Reserved":
                            row.DefaultCellStyle.BackColor = Color.LightBlue;
                            break;
                    }
                }

                lblRecordCount.Text = $"Total Rooms: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                ShowError("Failed to load rooms", ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                // Check for duplicate bed number
                string checkQuery = "SELECT COUNT(*) FROM Beds WHERE BedNumber = @BedNumber";
                MySqlParameter[] checkParams = { new MySqlParameter("@BedNumber", txtBedNumber.Text.Trim()) };
                object result = _db.ExecuteScalar(checkQuery, checkParams);

                if (result != null && Convert.ToInt32(result) > 0)
                {
                    MessageBox.Show("A room with this number already exists.", "Duplicate Room",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = @"
                    INSERT INTO Beds 
                    (AreaID, RoomTypeID, BedNumber, Price, Status, Description, LastMaintenance)
                    VALUES 
                    (@AreaID, @RoomTypeID, @BedNumber, @Price, @Status, @Description, @LastMaintenance)";

                MySqlParameter[] parameters = GetRoomParameters();
                int insertResult = _db.ExecuteNonQuery(query, parameters);

                if (insertResult > 0)
                {
                    ShowSuccess("Room added successfully!");
                    LoadRooms();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to add room", ex);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedBedId == 0)
                {
                    MessageBox.Show("Please select a room to update.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateInput())
                    return;

                string query = @"
                    UPDATE Beds 
                    SET AreaID = @AreaID,
                        RoomTypeID = @RoomTypeID,
                        BedNumber = @BedNumber,
                        Price = @Price,
                        Status = @Status,
                        Description = @Description,
                        LastMaintenance = @LastMaintenance
                    WHERE BedID = @BedID";

                var parameters = GetRoomParameters();
                Array.Resize(ref parameters, parameters.Length + 1);
                parameters[parameters.Length - 1] = new MySqlParameter("@BedID", _selectedBedId);

                int updateResult = _db.ExecuteNonQuery(query, parameters);

                if (updateResult > 0)
                {
                    ShowSuccess("Room updated successfully!");
                    LoadRooms();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to update room", ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedBedId == 0)
                {
                    MessageBox.Show("Please select a room to delete.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check for active bookings
                string checkQuery = "SELECT COUNT(*) FROM Bookings WHERE BedID = @BedID AND Status = 'Active'";
                MySqlParameter[] checkParams = { new MySqlParameter("@BedID", _selectedBedId) };
                object result = _db.ExecuteScalar(checkQuery, checkParams);

                if (result != null && Convert.ToInt32(result) > 0)
                {
                    MessageBox.Show("Cannot delete room with active bookings.", "Active Bookings",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (ConfirmAction("Are you sure you want to delete this room?"))
                {
                    string query = "DELETE FROM Beds WHERE BedID = @BedID";
                    MySqlParameter[] parameters = { new MySqlParameter("@BedID", _selectedBedId) };

                    int deleteResult = _db.ExecuteNonQuery(query, parameters);

                    if (deleteResult > 0)
                    {
                        ShowSuccess("Room deleted successfully!");
                        LoadRooms();
                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to delete room", ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadRooms();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            txtSearch.Clear();
            cmbFilterArea.SelectedIndex = 0;
            cmbFilterStatus.SelectedIndex = 0;
            LoadRooms();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbRoomType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRoomType.SelectedValue != null && cmbRoomType.SelectedValue != DBNull.Value)
            {
                DataRowView drv = (DataRowView)cmbRoomType.SelectedItem;
                if (drv != null && drv.Row.Table.Columns.Contains("BasePrice"))
                {
                    decimal basePrice = Convert.ToDecimal(drv.Row["BasePrice"]);
                    if (nudPrice.Value == 0)
                    {
                        nudPrice.Value = basePrice;
                    }
                }
            }
        }

        private void dgvRooms_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvRooms.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dgvRooms.SelectedRows[0];

                    _selectedBedId = Convert.ToInt32(row.Cells["BedID"].Value);
                    txtBedNumber.Text = row.Cells["BedNumber"].Value?.ToString() ?? "";
                    nudPrice.Value = Convert.ToDecimal(row.Cells["Price"].Value);
                    cmbStatus.SelectedItem = row.Cells["Status"].Value?.ToString() ?? "";
                    txtDescription.Text = row.Cells["Description"].Value?.ToString() ?? "";

                    // Set area
                    string areaName = row.Cells["AreaName"].Value?.ToString();
                    for (int i = 0; i < cmbArea.Items.Count; i++)
                    {
                        DataRowView drv = (DataRowView)cmbArea.Items[i];
                        if (drv.Row["AreaName"].ToString() == areaName)
                        {
                            cmbArea.SelectedIndex = i;
                            break;
                        }
                    }

                    // Set room type
                    string roomType = row.Cells["RoomType"].Value?.ToString();
                    for (int i = 0; i < cmbRoomType.Items.Count; i++)
                    {
                        DataRowView drv = (DataRowView)cmbRoomType.Items[i];
                        if (drv.Row["TypeName"].ToString() == roomType)
                        {
                            cmbRoomType.SelectedIndex = i;
                            break;
                        }
                    }

                    if (row.Cells["LastMaintenance"].Value != DBNull.Value)
                    {
                        dtpLastMaintenance.Value = Convert.ToDateTime(row.Cells["LastMaintenance"].Value);
                        chkLastMaintenance.Checked = true;
                    }
                    else
                    {
                        chkLastMaintenance.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load room details", ex);
            }
        }

        private void ClearForm()
        {
            _selectedBedId = 0;
            txtBedNumber.Clear();
            cmbArea.SelectedIndex = -1;
            cmbRoomType.SelectedIndex = -1;
            nudPrice.Value = 0;
            cmbStatus.SelectedIndex = 0; // Available
            txtDescription.Clear();
            chkLastMaintenance.Checked = false;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtBedNumber.Text))
            {
                MessageBox.Show("Please enter room/bed number.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBedNumber.Focus();
                return false;
            }

            if (cmbArea.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an area.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbArea.Focus();
                return false;
            }

            if (cmbRoomType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a room type.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRoomType.Focus();
                return false;
            }

            if (nudPrice.Value <= 0)
            {
                MessageBox.Show("Please enter a valid price.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudPrice.Focus();
                return false;
            }

            if (cmbStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please select room status.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbStatus.Focus();
                return false;
            }

            return true;
        }

        private MySqlParameter[] GetRoomParameters()
        {
            return new MySqlParameter[]
            {
        new MySqlParameter("@AreaID", cmbArea.SelectedValue),
        new MySqlParameter("@RoomTypeID", cmbRoomType.SelectedValue),
        new MySqlParameter("@BedNumber", txtBedNumber.Text.Trim()),
        new MySqlParameter("@Price", nudPrice.Value),
        new MySqlParameter("@Status", cmbStatus.SelectedItem.ToString()),
        new MySqlParameter("@Description", string.IsNullOrWhiteSpace(txtDescription.Text) ? (object)DBNull.Value : txtDescription.Text.Trim()),
        new MySqlParameter("@LastMaintenance", chkLastMaintenance.Checked ? (object)dtpLastMaintenance.Value : DBNull.Value)
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
