using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class UserGuideForm : Form
    {
        public UserGuideForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text            = "User Guide — Umbraco Optimate Media";
            this.Size            = new Size(700, 560);
            this.MinimumSize     = new Size(560, 460);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.BackColor       = Color.White;
            this.Font            = new Font("Segoe UI", 9.5F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            // ── Header bar ──────────────────────────────────────────────────
            var header = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 56,
                BackColor = Color.FromArgb(45, 45, 48)
            };

            var title = new Label
            {
                Text      = "User Guide",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(16, 0, 0, 0)
            };
            header.Controls.Add(title);

            // ── Content ──────────────────────────────────────────────────────
            var rtb = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                ReadOnly    = true,
                BorderStyle = BorderStyle.None,
                BackColor   = Color.White,
                Font        = new Font("Segoe UI", 10F),
                ScrollBars  = RichTextBoxScrollBars.Vertical,
                Padding     = new Padding(16)
            };

            // ── Close button ─────────────────────────────────────────────────
            var btnClose = new Button
            {
                Text      = "Close",
                Dock      = DockStyle.Bottom,
                Height    = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9.5F),
                Cursor    = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(rtb);
            this.Controls.Add(header);
            this.Controls.Add(btnClose);

            FillContent(rtb);
        }

        private void FillContent(RichTextBox rtb)
        {
            rtb.SuspendLayout();

            Section(rtb, "Toolbar Buttons");
            Row(rtb, "Open Folder",
                "Browse and select a local folder. All files inside (including subfolders) are listed in the table. Files with excluded extensions (e.g. .pdf) are hidden automatically.");
            Row(rtb, "Recent",
                "Quickly reopen a recently used local folder or Azure container. Up to 10 entries are remembered. Select \"Clear history\" to reset the list.");
            Row(rtb, "Connect Azure",
                "Connect to an Azure Blob Storage container. Enter the Connection String and Container Name, then click \"Test Connection\" to verify before connecting. The connection string is never saved — only the container name is remembered for convenience.");
            Row(rtb, "User Guide",
                "Opens this help window.");

            Spacer(rtb);
            Section(rtb, "File Table — Action Buttons");
            Row(rtb, "Download",
                "Save the selected file (local) or blob (Azure) to a location on your computer.");
            Row(rtb, "Replace",
                "Choose a local file to overwrite the current entry. In local mode the file is copied in-place; in Azure mode the file is uploaded and replaces the blob.");
            Row(rtb, "Preview",
                "Open the built-in image viewer for PNG, JPG, JPEG, BMP, GIF, and TIFF files. Zoom from 10 % to 300 % using the slider. In local mode, non-image files are opened with the default system application.");

            Spacer(rtb);
            Section(rtb, "Automatic Filters");
            Row(rtb, "cache/ folder",
                "Blobs whose path starts with \"cache/\" are excluded from the Azure listing.");
            Row(rtb, ".pdf files",
                "Files and blobs with the .pdf extension are hidden in both local and Azure mode.");

            Spacer(rtb);
            Section(rtb, "Azure Mode Tips");
            Bullet(rtb, "After connecting, the table header shows the cloud icon and container name.");
            Bullet(rtb, "Preview downloads the blob to a temporary file before displaying it.");
            Bullet(rtb, "Replace uploads your local file directly — no intermediate copy is kept.");
            Bullet(rtb, "Use \"Recent\" to quickly reconnect to a container (you will still be prompted for the connection string each session).");

            rtb.SelectionStart = 0;
            rtb.ResumeLayout();
        }

        private void Section(RichTextBox rtb, string text)
        {
            rtb.SelectionStart  = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionFont   = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            rtb.SelectionColor  = Color.FromArgb(0, 120, 212);
            rtb.AppendText(text + "\n");
            rtb.SelectionFont  = new Font("Segoe UI", 10F);
            rtb.SelectionColor = Color.Black;
        }

        private void Row(RichTextBox rtb, string label, string description)
        {
            rtb.SelectionStart  = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionFont   = new Font("Segoe UI", 10F, FontStyle.Bold);
            rtb.SelectionColor  = Color.FromArgb(45, 45, 48);
            rtb.AppendText("  " + label + "  ");

            rtb.SelectionFont  = new Font("Segoe UI", 10F);
            rtb.SelectionColor = Color.FromArgb(80, 80, 80);
            rtb.AppendText("— " + description + "\n");
        }

        private void Bullet(RichTextBox rtb, string text)
        {
            rtb.SelectionStart  = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionFont   = new Font("Segoe UI", 10F);
            rtb.SelectionColor  = Color.FromArgb(80, 80, 80);
            rtb.AppendText("  • " + text + "\n");
        }

        private void Spacer(RichTextBox rtb)
        {
            rtb.AppendText("\n");
        }
    }
}
