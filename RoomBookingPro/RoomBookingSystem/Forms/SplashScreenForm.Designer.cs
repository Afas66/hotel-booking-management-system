namespace RoomBookingSystem.Forms
{
    partial class SplashScreenForm
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

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            lblTitle = new Label();
            lblSubtitle = new Label();
            progressBar1 = new ProgressBar();
            lblStatus = new Label();
            lblVersion = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(120, 150);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(508, 72);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Room Booking Pro";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 14F);
            lblSubtitle.ForeColor = Color.White;
            lblSubtitle.Location = new Point(230, 230);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(375, 32);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Professional Management System";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(150, 350);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(500, 30);
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.TabIndex = 2;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 10F);
            lblStatus.ForeColor = Color.White;
            lblStatus.Location = new Point(150, 390);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(157, 23);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "Initializing system...";
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.Font = new Font("Segoe UI", 9F);
            lblVersion.ForeColor = Color.White;
            lblVersion.Location = new Point(350, 460);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(91, 20);
            lblVersion.TabIndex = 4;
            lblVersion.Text = "Version 1.0.0";
            // 
            // timer1
            // 
            timer1.Interval = 50;
            timer1.Tick += timer1_Tick;
            // 
            // SplashScreenForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(41, 128, 185);
            ClientSize = new Size(800, 500);
            Controls.Add(lblVersion);
            Controls.Add(lblStatus);
            Controls.Add(progressBar1);
            Controls.Add(lblSubtitle);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.None;
            Name = "SplashScreenForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Loading...";
            Load += SplashScreenForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Timer timer1;
    }
}
