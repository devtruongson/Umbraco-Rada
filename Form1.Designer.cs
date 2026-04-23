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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panelTop = new System.Windows.Forms.Panel();
            this.guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            this.btnRecent = new Guna.UI2.WinForms.Guna2Button();
            this.btnConnectAzure = new Guna.UI2.WinForms.Guna2Button();
            this.btnUserGuide = new Guna.UI2.WinForms.Guna2Button();
            this.btnOptimize = new Guna.UI2.WinForms.Guna2Button();
            this.lblFolderPath = new System.Windows.Forms.Label();
            this.panelSep = new System.Windows.Forms.Panel();
            this.contextMenuRecent = new System.Windows.Forms.ContextMenuStrip(this.components);
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
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.White;
            this.panelTop.Controls.Add(this.guna2HtmlLabel1);
            this.panelTop.Controls.Add(this.guna2Button1);
            this.panelTop.Controls.Add(this.btnRecent);
            this.panelTop.Controls.Add(this.btnConnectAzure);
            this.panelTop.Controls.Add(this.btnUserGuide);
            this.panelTop.Controls.Add(this.btnOptimize);
            this.panelTop.Controls.Add(this.lblFolderPath);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1184, 86);
            this.panelTop.TabIndex = 4;
            // 
            // guna2HtmlLabel1
            // 
            this.guna2HtmlLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2HtmlLabel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.guna2HtmlLabel1.Location = new System.Drawing.Point(1804, 20);
            this.guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            this.guna2HtmlLabel1.Size = new System.Drawing.Size(232, 27);
            this.guna2HtmlLabel1.TabIndex = 2;
            this.guna2HtmlLabel1.Text = "Umbraco Optimate Media";
            this.guna2HtmlLabel1.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // guna2Button1
            // 
            this.guna2Button1.BorderRadius = 6;
            this.guna2Button1.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2Button1.ForeColor = System.Drawing.Color.White;
            this.guna2Button1.Location = new System.Drawing.Point(12, 14);
            this.guna2Button1.Name = "guna2Button1";
            this.guna2Button1.Size = new System.Drawing.Size(148, 36);
            this.guna2Button1.TabIndex = 0;
            this.guna2Button1.Text = "Open Folder";
            this.guna2Button1.Click += new System.EventHandler(this.guna2Button1_Click);
            // 
            // btnRecent
            // 
            this.btnRecent.BorderRadius = 6;
            this.btnRecent.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnRecent.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnRecent.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnRecent.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnRecent.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(85)))));
            this.btnRecent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRecent.ForeColor = System.Drawing.Color.White;
            this.btnRecent.Location = new System.Drawing.Point(168, 14);
            this.btnRecent.Name = "btnRecent";
            this.btnRecent.Size = new System.Drawing.Size(120, 36);
            this.btnRecent.TabIndex = 1;
            this.btnRecent.Text = "Recent  ▾";
            this.btnRecent.Click += new System.EventHandler(this.btnRecent_Click);
            // 
            // btnConnectAzure
            // 
            this.btnConnectAzure.BorderRadius = 6;
            this.btnConnectAzure.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnConnectAzure.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnConnectAzure.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnConnectAzure.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnConnectAzure.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnConnectAzure.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnConnectAzure.ForeColor = System.Drawing.Color.White;
            this.btnConnectAzure.Location = new System.Drawing.Point(296, 14);
            this.btnConnectAzure.Name = "btnConnectAzure";
            this.btnConnectAzure.Size = new System.Drawing.Size(148, 36);
            this.btnConnectAzure.TabIndex = 4;
            this.btnConnectAzure.Text = "Connect Azure";
            this.btnConnectAzure.Click += new System.EventHandler(this.btnConnectAzure_Click);
            // 
            // btnUserGuide
            // 
            this.btnUserGuide.BorderRadius = 6;
            this.btnUserGuide.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnUserGuide.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnUserGuide.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnUserGuide.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnUserGuide.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(105)))));
            this.btnUserGuide.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnUserGuide.ForeColor = System.Drawing.Color.White;
            this.btnUserGuide.Location = new System.Drawing.Point(452, 14);
            this.btnUserGuide.Name = "btnUserGuide";
            this.btnUserGuide.Size = new System.Drawing.Size(120, 36);
            this.btnUserGuide.TabIndex = 5;
            this.btnUserGuide.Text = "? User Guide";
            this.btnUserGuide.Click += new System.EventHandler(this.btnUserGuide_Click);
            //
            // btnOptimize
            //
            this.btnOptimize.BorderRadius = 6;
            this.btnOptimize.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnOptimize.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnOptimize.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnOptimize.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnOptimize.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            this.btnOptimize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnOptimize.ForeColor = System.Drawing.Color.White;
            this.btnOptimize.Location = new System.Drawing.Point(580, 14);
            this.btnOptimize.Name = "btnOptimize";
            this.btnOptimize.Size = new System.Drawing.Size(148, 36);
            this.btnOptimize.TabIndex = 6;
            this.btnOptimize.Text = "⚡ Optimize Images";
            this.btnOptimize.Visible = false;
            this.btnOptimize.Click += new System.EventHandler(this.btnOptimize_Click);
            //
            // lblFolderPath
            // 
            this.lblFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFolderPath.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblFolderPath.ForeColor = System.Drawing.Color.Gray;
            this.lblFolderPath.Location = new System.Drawing.Point(12, 60);
            this.lblFolderPath.Name = "lblFolderPath";
            this.lblFolderPath.Size = new System.Drawing.Size(2144, 18);
            this.lblFolderPath.TabIndex = 3;
            this.lblFolderPath.Text = "No folder selected";
            // 
            // panelSep
            // 
            this.panelSep.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.panelSep.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSep.Location = new System.Drawing.Point(0, 86);
            this.panelSep.Name = "panelSep";
            this.panelSep.Size = new System.Drawing.Size(1184, 1);
            this.panelSep.TabIndex = 3;
            // 
            // contextMenuRecent
            // 
            this.contextMenuRecent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.contextMenuRecent.Name = "contextMenuRecent";
            this.contextMenuRecent.Size = new System.Drawing.Size(61, 4);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFileName,
            this.colPath,
            this.colSize,
            this.colDownload,
            this.colReplace,
            this.colPreview});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.dataGridView1.Location = new System.Drawing.Point(0, 87);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 38;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1184, 593);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.dataGridView1_CellToolTipTextNeeded);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // colFileName
            // 
            this.colFileName.HeaderText = "File Name";
            this.colFileName.MinimumWidth = 160;
            this.colFileName.Name = "colFileName";
            this.colFileName.ReadOnly = true;
            this.colFileName.Width = 220;
            // 
            // colPath
            // 
            this.colPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colPath.HeaderText = "Path";
            this.colPath.MinimumWidth = 160;
            this.colPath.Name = "colPath";
            this.colPath.ReadOnly = true;
            // 
            // colSize
            // 
            this.colSize.HeaderText = "Size (MB)";
            this.colSize.Name = "colSize";
            this.colSize.ReadOnly = true;
            this.colSize.Width = 95;
            // 
            // colDownload
            // 
            this.colDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colDownload.HeaderText = "";
            this.colDownload.Name = "colDownload";
            this.colDownload.Text = "Download";
            this.colDownload.UseColumnTextForButtonValue = true;
            // 
            // colReplace
            // 
            this.colReplace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colReplace.HeaderText = "";
            this.colReplace.Name = "colReplace";
            this.colReplace.Text = "Replace";
            this.colReplace.UseColumnTextForButtonValue = true;
            // 
            // colPreview
            // 
            this.colPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colPreview.HeaderText = "";
            this.colPreview.Name = "colPreview";
            this.colPreview.Text = "Preview";
            this.colPreview.UseColumnTextForButtonValue = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1184, 680);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panelSep);
            this.Controls.Add(this.panelTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(860, 520);
            this.Name = "Form1";
            this.Text = "File Optimaze";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelSep;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private Guna.UI2.WinForms.Guna2Button btnRecent;
        private Guna.UI2.WinForms.Guna2Button btnConnectAzure;
        private Guna.UI2.WinForms.Guna2Button btnUserGuide;
        private Guna.UI2.WinForms.Guna2Button btnOptimize;
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
