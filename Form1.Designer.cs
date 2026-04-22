namespace WindowsFormsApp1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelSep = new System.Windows.Forms.Panel();
            this.guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            this.btnRecent = new Guna.UI2.WinForms.Guna2Button();
            this.guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblFolderPath = new System.Windows.Forms.Label();
            this.contextMenuRecent = new System.Windows.Forms.ContextMenuStrip();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDownload = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colReplace = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colPreview = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();

            // ── panelTop ──────────────────────────────────────────────────────
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 86;
            this.panelTop.BackColor = System.Drawing.Color.White;
            this.panelTop.Controls.Add(this.guna2HtmlLabel1);
            this.panelTop.Controls.Add(this.guna2Button1);
            this.panelTop.Controls.Add(this.btnRecent);
            this.panelTop.Controls.Add(this.lblFolderPath);

            // ── panelSep (thin separator line) ────────────────────────────────
            this.panelSep.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSep.Height = 1;
            this.panelSep.BackColor = System.Drawing.Color.FromArgb(220, 220, 220);

            // ── guna2Button1 — Open Folder ────────────────────────────────────
            this.guna2Button1.BorderRadius = 6;
            this.guna2Button1.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.FillColor = System.Drawing.Color.FromArgb(169, 169, 169);
            this.guna2Button1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(141, 141, 141);
            this.guna2Button1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2Button1.ForeColor = System.Drawing.Color.White;
            this.guna2Button1.Location = new System.Drawing.Point(12, 14);
            this.guna2Button1.Name = "guna2Button1";
            this.guna2Button1.Size = new System.Drawing.Size(148, 36);
            this.guna2Button1.TabIndex = 0;
            this.guna2Button1.Text = "Open Folder";
            this.guna2Button1.Click += new System.EventHandler(this.guna2Button1_Click);

            // ── btnRecent ─────────────────────────────────────────────────────
            this.btnRecent.BorderRadius = 6;
            this.btnRecent.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnRecent.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnRecent.DisabledState.FillColor = System.Drawing.Color.FromArgb(169, 169, 169);
            this.btnRecent.DisabledState.ForeColor = System.Drawing.Color.FromArgb(141, 141, 141);
            this.btnRecent.FillColor = System.Drawing.Color.FromArgb(80, 80, 85);
            this.btnRecent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRecent.ForeColor = System.Drawing.Color.White;
            this.btnRecent.Location = new System.Drawing.Point(168, 14);
            this.btnRecent.Name = "btnRecent";
            this.btnRecent.Size = new System.Drawing.Size(120, 36);
            this.btnRecent.TabIndex = 1;
            this.btnRecent.Text = "Recent  ▾";
            this.btnRecent.Click += new System.EventHandler(this.btnRecent_Click);

            // ── guna2HtmlLabel1 ───────────────────────────────────────────────
            this.guna2HtmlLabel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.guna2HtmlLabel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.guna2HtmlLabel1.Location = new System.Drawing.Point(820, 20);
            this.guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            this.guna2HtmlLabel1.Size = new System.Drawing.Size(340, 26);
            this.guna2HtmlLabel1.TabIndex = 2;
            this.guna2HtmlLabel1.Text = "Umbraco Optimate Media";
            this.guna2HtmlLabel1.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;

            // ── lblFolderPath ─────────────────────────────────────────────────
            this.lblFolderPath.Anchor = System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.lblFolderPath.AutoSize = false;
            this.lblFolderPath.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblFolderPath.ForeColor = System.Drawing.Color.Gray;
            this.lblFolderPath.Location = new System.Drawing.Point(12, 60);
            this.lblFolderPath.Size = new System.Drawing.Size(1160, 18);
            this.lblFolderPath.Name = "lblFolderPath";
            this.lblFolderPath.Text = "No folder selected";
            this.lblFolderPath.TabIndex = 3;

            // ── contextMenuRecent ─────────────────────────────────────────────
            this.contextMenuRecent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.contextMenuRecent.Name = "contextMenuRecent";

            // ── dataGridView1 ─────────────────────────────────────────────────
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colFileName,
                this.colPath,
                this.colSize,
                this.colDownload,
                this.colReplace,
                this.colPreview
            });
            this.dataGridView1.GridColor = System.Drawing.Color.FromArgb(230, 230, 230);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 38;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);

            // colFileName — fixed
            this.colFileName.HeaderText = "File Name";
            this.colFileName.Name = "colFileName";
            this.colFileName.ReadOnly = true;
            this.colFileName.MinimumWidth = 160;
            this.colFileName.Width = 220;

            // colPath — Fill (stretches with window)
            this.colPath.HeaderText = "Path";
            this.colPath.Name = "colPath";
            this.colPath.ReadOnly = true;
            this.colPath.MinimumWidth = 160;
            this.colPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;

            // colSize — fixed
            this.colSize.HeaderText = "Size (MB)";
            this.colSize.Name = "colSize";
            this.colSize.ReadOnly = true;
            this.colSize.Width = 95;

            // colDownload — fixed
            this.colDownload.HeaderText = "";
            this.colDownload.Name = "colDownload";
            this.colDownload.Text = "Download";
            this.colDownload.UseColumnTextForButtonValue = true;
            this.colDownload.Width = 100;
            this.colDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            // colReplace — fixed
            this.colReplace.HeaderText = "";
            this.colReplace.Name = "colReplace";
            this.colReplace.Text = "Replace";
            this.colReplace.UseColumnTextForButtonValue = true;
            this.colReplace.Width = 100;
            this.colReplace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            // colPreview — fixed
            this.colPreview.HeaderText = "";
            this.colPreview.Name = "colPreview";
            this.colPreview.Text = "Preview";
            this.colPreview.UseColumnTextForButtonValue = true;
            this.colPreview.Width = 100;
            this.colPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            // ── Form1 ─────────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1184, 680);
            this.MinimumSize = new System.Drawing.Size(860, 520);
            // Order: Fill first, then Docked-Top panels (processed last = laid out first)
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panelSep);
            this.Controls.Add(this.panelTop);
            this.Name = "Form1";
            this.Text = "File Optimaze";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelSep;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private Guna.UI2.WinForms.Guna2Button btnRecent;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private System.Windows.Forms.Label lblFolderPath;
        private System.Windows.Forms.ContextMenuStrip contextMenuRecent;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSize;
        private System.Windows.Forms.DataGridViewButtonColumn colDownload;
        private System.Windows.Forms.DataGridViewButtonColumn colReplace;
        private System.Windows.Forms.DataGridViewButtonColumn colPreview;
    }
}
