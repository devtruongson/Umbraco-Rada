using System;
using System.Drawing;
using System.Windows.Forms;
using Azure.Storage.Blobs;

namespace WindowsFormsApp1
{
    public class AzureConnectForm : Form
    {
        public string ConnectionString => txtConnString.Text.Trim();
        public string ContainerName    => txtContainer.Text.Trim();

        private TextBox txtConnString;
        private TextBox txtContainer;
        private Label   lblStatus;
        private Button  btnTest;
        private Button  btnOK;
        private Button  btnCancel;

        public AzureConnectForm(string savedConnString = "", string savedContainer = "")
        {
            BuildUI();
            txtConnString.Text = savedConnString;
            txtContainer.Text  = savedContainer;
        }

        private void BuildUI()
        {
            this.Text            = "Connect to Azure Blob Storage";
            this.Size            = new Size(660, 290);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.BackColor       = Color.White;
            this.Font            = new Font("Segoe UI", 9.5F);

            int lx = 20, tx = 160, w = 460;

            var lblConn = new Label { Text = "Connection String:", Location = new Point(lx, 24), AutoSize = true };
            txtConnString = new TextBox { Location = new Point(tx, 20), Size = new Size(w, 22) };

            var lblCont = new Label { Text = "Container Name:", Location = new Point(lx, 64), AutoSize = true };
            txtContainer = new TextBox { Location = new Point(tx, 60), Size = new Size(w, 22) };

            lblStatus = new Label
            {
                Location  = new Point(lx, 98),
                Size      = new Size(600, 50),
                ForeColor = Color.Gray,
                Text      = ""
            };

            btnTest = new Button
            {
                Text      = "Test Connection",
                Location  = new Point(lx, 170),
                Size      = new Size(140, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White
            };
            btnTest.FlatAppearance.BorderSize = 0;
            btnTest.Click += BtnTest_Click;

            btnOK = new Button
            {
                Text         = "Connect",
                Location     = new Point(408, 170),
                Size         = new Size(100, 32),
                DialogResult = DialogResult.OK,
                FlatStyle    = FlatStyle.Flat,
                BackColor    = Color.FromArgb(39, 174, 96),
                ForeColor    = Color.White
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += (s, e) =>
            {
                if (ConnectionString.Length == 0 || ContainerName.Length == 0)
                {
                    lblStatus.ForeColor = Color.Crimson;
                    lblStatus.Text = "Please fill in both Connection String and Container Name.";
                    this.DialogResult = DialogResult.None;
                }
            };

            btnCancel = new Button
            {
                Text         = "Cancel",
                Location     = new Point(516, 170),
                Size         = new Size(100, 32),
                DialogResult = DialogResult.Cancel,
                FlatStyle    = FlatStyle.Flat,
                BackColor    = Color.FromArgb(160, 160, 160),
                ForeColor    = Color.White
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
            this.Controls.AddRange(new Control[] {
                lblConn, txtConnString,
                lblCont, txtContainer,
                lblStatus, btnTest, btnOK, btnCancel
            });
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            if (ConnectionString.Length == 0 || ContainerName.Length == 0)
            {
                lblStatus.ForeColor = Color.Crimson;
                lblStatus.Text = "Fill in both fields before testing.";
                return;
            }
            btnTest.Enabled = false;
            lblStatus.ForeColor = Color.Gray;
            lblStatus.Text = "Testing connection…";
            this.Refresh();
            try
            {
                var container = new BlobContainerClient(ConnectionString, ContainerName);
                bool exists = container.Exists().Value;
                lblStatus.ForeColor = exists ? Color.FromArgb(39, 174, 96) : Color.Crimson;
                lblStatus.Text = exists
                    ? "Connection successful!  Container found."
                    : "Connection OK but container does not exist.";
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Crimson;
                lblStatus.Text = "Error: " + ex.Message;
            }
            finally { btnTest.Enabled = true; }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AzureConnectForm
            // 
            this.ClientSize = new System.Drawing.Size(1209, 481);
            this.Name = "AzureConnectForm";
            this.Load += new System.EventHandler(this.AzureConnectForm_Load);
            this.ResumeLayout(false);

        }

        private void AzureConnectForm_Load(object sender, EventArgs e)
        {

        }
    }
}
