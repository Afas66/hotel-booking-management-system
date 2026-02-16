using RoomBookingSystem.Data;
using RoomBookingSystem.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace RoomBookingSystem.Forms
{
    public partial class LoginForm : Form // inheritance //
    {
        private readonly DatabaseHelper _db;

        public LoginForm()
        {
            InitializeComponent();
            _db = new DatabaseHelper();
            this.AcceptButton = btnLogin; // Enter key triggers login //(Authentication & validation)//
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Set focus to username textbox
            txtUsername.Focus();
            
            // Display hint for demo
            lblHint.Text = "Demo: admin / admin123";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            // Validation
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter username.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter password.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            // Authenticate user
            if (AuthenticateUser(username, password))
            {
                // Update last login
                UpdateLastLogin(User.CurrentUser!.UserID);

                // Log audit
                LogAudit(User.CurrentUser.UserID, "Login", "Users", User.CurrentUser.UserID);

                // Show dashboard and handle the form lifecycle properly
                DashboardForm dashboard = new DashboardForm();
                this.Hide(); // Hide login form
                
                dashboard.ShowDialog(); // Show dashboard as dialog
                
                // When dashboard closes, close the application
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.\nPlease try again.",
                    "Login Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            try
            {
                string query = @"SELECT UserID, Username, FullName, Email, Phone, Role, IsActive 
                                FROM Users 
                                WHERE Username = @username AND Password = @password AND IsActive = TRUE";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@username", username),
                    new MySqlParameter("@password", password)
                };

                DataTable dt = _db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    User.CurrentUser = new User
                    {
                        UserID = Convert.ToInt32(row["UserID"]),
                        Username = row["Username"]?.ToString() ?? string.Empty,
                        FullName = row["FullName"]?.ToString() ?? string.Empty,
                        Email = row["Email"]?.ToString() ?? string.Empty,
                        Phone = row["Phone"]?.ToString() ?? string.Empty,
                        Role = row["Role"]?.ToString() ?? string.Empty,
                        IsActive = Convert.ToBoolean(row["IsActive"])
                    };
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void UpdateLastLogin(int userId)
        {
            try
            {
                string query = "UPDATE Users SET LastLogin = @lastLogin WHERE UserID = @userId";
                MySqlParameter[] parameters = {
                    new MySqlParameter("@lastLogin", DateTime.Now),
                    new MySqlParameter("@userId", userId)
                };
                _db.ExecuteNonQuery(query, parameters);
            }
            catch
            {
                // Silently fail - not critical
            }
        }

        private void LogAudit(int userId, string action, string tableName, int recordId)
        {
            try
            {
                string query = @"INSERT INTO AuditLog (UserID, Action, TableName, RecordID, Timestamp) 
                                VALUES (@userId, @action, @tableName, @recordId, @timestamp)";
                MySqlParameter[] parameters = {
                    new MySqlParameter("@userId", userId),
                    new MySqlParameter("@action", action),
                    new MySqlParameter("@tableName", tableName),
                    new MySqlParameter("@recordId", recordId),
                    new MySqlParameter("@timestamp", DateTime.Now)
                };
                _db.ExecuteNonQuery(query, parameters);
            }
            catch
            {
                // Silently fail - audit is not critical for login
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Confirm Exit",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '●';
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin.PerformClick();
                e.Handled = true;
            }
        }
    }
}
