using RoomBookingSystem.Data;
using System.Drawing.Drawing2D;

namespace RoomBookingSystem.Forms
{
    public partial class SplashScreenForm : Form
    {
        private readonly DatabaseHelper _db;
        private int progress = 0;

        public SplashScreenForm()
        {
            InitializeComponent();
            _db = new DatabaseHelper();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(800, 500);
            this.BackColor = Color.FromArgb(41, 128, 185);
            
            // Make corners rounded
            ApplyRoundedCorners();
        }

        private void ApplyRoundedCorners()
        {
            GraphicsPath path = new GraphicsPath();
            int radius = 20;
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(this.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, this.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            this.Region = new Region(path);
        }

        private void SplashScreenForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progress += 2;
            progressBar1.Value = Math.Min(progress, 100);
            
            if (progress <= 20)
            {
                lblStatus.Text = "Initializing system...";
            }
            else if (progress <= 40)
            {
                lblStatus.Text = "Connecting to database...";
            }
            else if (progress <= 60)
            {
                lblStatus.Text = "Loading modules...";
            }
            else if (progress <= 80)
            {
                lblStatus.Text = "Preparing interface...";
            }
            else
            {
                lblStatus.Text = "Ready!";
            }
            
            if (progress >= 100)
            {
                timer1.Stop();
                
                // Test database connection
                if (_db.TestConnection())
                {
                    this.Hide();
                    LoginForm loginForm = new LoginForm();
                    loginForm.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Cannot connect to database!\n\nPlease ensure:\n1. MySQL is running\n2. Database 'RoomBookingDB' exists\n3. Connection string is correct in DatabaseHelper.cs\n\nRun the 'database_schema.sql' file to create the database.",
                        "Database Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
        }
    }
}
