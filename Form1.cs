using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff", ".tif" };

        private static readonly string[] ExcludedBlobPrefixes = { "cache/", "cache\\" };
        private static readonly string[] ExcludedExtensions = { ".pdf" };
        private static readonly Color ColorDownload = Color.FromArgb(52, 73, 94);
        private static readonly Color ColorReplace  = Color.FromArgb(52, 73, 94);
        private static readonly Color ColorPreview  = Color.FromArgb(52, 152, 219);

        private const int MaxRecent = 10;
        private readonly string _recentFile;
        private readonly string _azureConnFile;

        private BlobContainerClient _azureContainer;
        private bool _isAzureMode = false;

        public Form1()
        {
            InitializeComponent();
            StyleGrid();

            string appData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WindowsFormsApp1");
            Directory.CreateDirectory(appData);
            _recentFile    = Path.Combine(appData, "recent.txt");
            _azureConnFile = Path.Combine(appData, "azure_connections.txt");
        }

        private void Form1_Load(object sender, EventArgs e) { }


        private void StyleGrid()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridView1.ColumnHeadersHeight = 40;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
        }

        private void dataGridView1_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int col = e.ColumnIndex;
            if (col == dataGridView1.Columns["colDownload"].Index)
                e.ToolTipText = "Download this file to your computer";
            else if (col == dataGridView1.Columns["colReplace"].Index)
                e.ToolTipText = "Upload a local file to replace this one";
            else if (col == dataGridView1.Columns["colPreview"].Index)
                e.ToolTipText = "Preview this image in a viewer";
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int col = e.ColumnIndex;
            Color bg = Color.Empty;
            if (col == dataGridView1.Columns["colDownload"].Index) bg = ColorDownload;
            else if (col == dataGridView1.Columns["colReplace"].Index) bg = ColorReplace;
            else if (col == dataGridView1.Columns["colPreview"].Index) bg = ColorPreview;
            if (bg != Color.Empty)
            {
                e.CellStyle.BackColor = bg;
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.SelectionBackColor = ControlPaint.Dark(bg, 0.1f);
                e.CellStyle.SelectionForeColor = Color.White;
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a folder to list files";
                dialog.ShowNewFolderButton = false;
                if (dialog.ShowDialog() != DialogResult.OK) return;
                SwitchToLocal(dialog.SelectedPath);
            }
        }

        private void SwitchToLocal(string folderPath)
        {
            _isAzureMode    = false;
            _azureContainer = null;
            lblFolderPath.Text = folderPath;
            LoadLocalFiles(folderPath);
            SaveRecent(folderPath);
        }

        private void LoadLocalFiles(string folderPath)
        {
            dataGridView1.Rows.Clear();
            foreach (string filePath in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
            {
                string ext = Path.GetExtension(filePath).ToLower();
                if (Array.IndexOf(ExcludedExtensions, ext) >= 0) continue;
                var info = new FileInfo(filePath);
                dataGridView1.Rows.Add(info.Name, info.FullName, (info.Length / 1048576.0).ToString("F3"));
            }
        }


        private void btnConnectAzure_Click(object sender, EventArgs e)
        {
            LoadLastAzureConnection(out string savedContainer);
            using (var form = new AzureConnectForm("", savedContainer))
            {
                if (form.ShowDialog() != DialogResult.OK) return;
                ConnectToAzure(form.ConnectionString, form.ContainerName);
            }
        }

        private void ConnectToAzure(string connectionString, string containerName)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                var container = new BlobContainerClient(connectionString, containerName);
                if (!container.Exists().Value)
                {
                    MessageBox.Show($"Container '{containerName}' does not exist.",
                        "Azure", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _azureContainer = container;
                _isAzureMode    = true;
                lblFolderPath.Text = $"☁  Azure  /  {containerName}";
                LoadAzureBlobs();
                SaveLastAzureConnection(containerName);
                SaveRecent("azure://" + containerName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed:\n" + ex.Message, "Azure Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { Cursor = Cursors.Default; }
        }

        private bool ShouldIncludeBlob(string blobName)
        {
            foreach (string prefix in ExcludedBlobPrefixes)
                if (blobName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return false;
            string ext = Path.GetExtension(blobName).ToLower();
            if (Array.IndexOf(ExcludedExtensions, ext) >= 0) return false;
            return true;
        }

        private void LoadAzureBlobs()
        {
            dataGridView1.Rows.Clear();
            Cursor = Cursors.WaitCursor;
            try
            {
                foreach (BlobItem blob in _azureContainer.GetBlobs())
                {
                    if (!ShouldIncludeBlob(blob.Name)) continue;
                    double sizeMb = (blob.Properties.ContentLength ?? 0) / 1048576.0;
                    dataGridView1.Rows.Add(
                        Path.GetFileName(blob.Name),
                        blob.Name,
                        sizeMb.ToString("F3"));
                }
            }
            finally { Cursor = Cursors.Default; }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string path = dataGridView1.Rows[e.RowIndex].Cells["colPath"].Value?.ToString();
            if (string.IsNullOrEmpty(path)) return;

            int col = e.ColumnIndex;

            if (_isAzureMode)
            {
                if (col == dataGridView1.Columns["colDownload"].Index) DownloadBlob(path);
                else if (col == dataGridView1.Columns["colReplace"].Index) ReplaceBlob(path, e.RowIndex);
                else if (col == dataGridView1.Columns["colPreview"].Index) PreviewBlob(path);
            }
            else
            {
                if (!File.Exists(path)) return;
                if (col == dataGridView1.Columns["colDownload"].Index) DownloadFile(path);
                else if (col == dataGridView1.Columns["colReplace"].Index) ReplaceFile(path, e.RowIndex);
                else if (col == dataGridView1.Columns["colPreview"].Index) PreviewFile(path);
            }
        }

        private void DownloadFile(string filePath)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.FileName = Path.GetFileName(filePath);
                dialog.Filter = "All files (*.*)|*.*";
                if (dialog.ShowDialog() != DialogResult.OK) return;
                File.Copy(filePath, dialog.FileName, overwrite: true);
                MessageBox.Show("File saved successfully.", "Download",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ReplaceFile(string originalPath, int rowIndex)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Select replacement file";
                dialog.Filter = "All files (*.*)|*.*";
                if (dialog.ShowDialog() != DialogResult.OK) return;
                if (MessageBox.Show(
                    $"Replace\n{originalPath}\nwith\n{dialog.FileName}?",
                    "Confirm Replace", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                File.Copy(dialog.FileName, originalPath, overwrite: true);
                UpdateRowSize(rowIndex, originalPath);
                MessageBox.Show("File replaced successfully.", "Replace",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void PreviewFile(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            if (Array.IndexOf(ImageExtensions, ext) < 0)
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                return;
            }
            try { new PreviewForm(filePath).Show(this); }
            catch { Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true }); }
        }

        private void UpdateRowSize(int rowIndex, string filePath)
        {
            var info = new FileInfo(filePath);
            dataGridView1.Rows[rowIndex].Cells["colSize"].Value = (info.Length / 1048576.0).ToString("F3");
        }

        private void DownloadBlob(string blobName)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.FileName = Path.GetFileName(blobName);
                dialog.Filter = "All files (*.*)|*.*";
                if (dialog.ShowDialog() != DialogResult.OK) return;
                Cursor = Cursors.WaitCursor;
                try
                {
                    _azureContainer.GetBlobClient(blobName).DownloadTo(dialog.FileName);
                    MessageBox.Show("Downloaded successfully.", "Download",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Download failed:\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally { Cursor = Cursors.Default; }
            }
        }

        private void ReplaceBlob(string blobName, int rowIndex)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Select local file to upload";
                dialog.Filter = "All files (*.*)|*.*";
                if (dialog.ShowDialog() != DialogResult.OK) return;
                if (MessageBox.Show(
                    $"Upload and replace blob:\n{blobName}\n\nwith local file:\n{dialog.FileName}?",
                    "Confirm Upload", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

                Cursor = Cursors.WaitCursor;
                try
                {
                    var client = _azureContainer.GetBlobClient(blobName);
                    using (var stream = File.OpenRead(dialog.FileName))
                        client.Upload(stream, overwrite: true);

                    long newLen = client.GetProperties().Value.ContentLength;
                    dataGridView1.Rows[rowIndex].Cells["colSize"].Value = (newLen / 1048576.0).ToString("F3");
                    MessageBox.Show("Blob replaced successfully.", "Replace",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Upload failed:\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally { Cursor = Cursors.Default; }
            }
        }

        private void PreviewBlob(string blobName)
        {
            string ext = Path.GetExtension(blobName).ToLower();
            if (Array.IndexOf(ImageExtensions, ext) < 0)
            {
                MessageBox.Show("Preview only supports image files (PNG, JPG, BMP, GIF, TIFF).",
                    "Unsupported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string tempPath = Path.Combine(Path.GetTempPath(), "WFApp1_" + Path.GetFileName(blobName));
            Cursor = Cursors.WaitCursor;
            try
            {
                _azureContainer.GetBlobClient(blobName).DownloadTo(tempPath);
                Cursor = Cursors.Default;
                new PreviewForm(tempPath).Show(this);
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show("Preview failed:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUserGuide_Click(object sender, EventArgs e)
        {
            new UserGuideForm().ShowDialog(this);
        }

        private void btnRecent_Click(object sender, EventArgs e)
        {
            var entries = LoadRecent();
            contextMenuRecent.Items.Clear();

            if (entries.Count == 0)
            {
                contextMenuRecent.Items.Add("(No recent folders)").Enabled = false;
            }
            else
            {
                foreach (string entry in entries)
                {
                    var item = new ToolStripMenuItem(entry);
                    string captured = entry;

                    if (captured.StartsWith("azure://"))
                    {
                        item.ForeColor = Color.FromArgb(0, 100, 180);
                        item.Click += (s, ev) => ReconnectAzureRecent(captured);
                    }
                    else
                    {
                        item.Click += (s, ev) =>
                        {
                            if (!Directory.Exists(captured))
                            {
                                MessageBox.Show("Folder no longer exists:\n" + captured,
                                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                RemoveRecent(captured);
                                return;
                            }
                            SwitchToLocal(captured);
                        };
                    }
                    contextMenuRecent.Items.Add(item);
                }

                contextMenuRecent.Items.Add(new ToolStripSeparator());
                var clearItem = new ToolStripMenuItem("Clear history");
                clearItem.Click += (s, ev) => { if (File.Exists(_recentFile)) File.Delete(_recentFile); };
                contextMenuRecent.Items.Add(clearItem);
            }

            var btn = (Control)sender;
            contextMenuRecent.Show(btn, 0, btn.Height);
        }

        private void ReconnectAzureRecent(string recentEntry)
        {
            string containerName = recentEntry.Substring("azure://".Length);
            using (var form = new AzureConnectForm("", containerName))
            {
                if (form.ShowDialog() != DialogResult.OK) return;
                ConnectToAzure(form.ConnectionString, form.ContainerName);
            }
        }

        // ── Recent persistence ────────────────────────────────────────────────

        private List<string> LoadRecent()
        {
            var list = new List<string>();
            if (!File.Exists(_recentFile)) return list;
            foreach (string line in File.ReadAllLines(_recentFile))
            {
                string t = line.Trim();
                if (t.Length > 0 && !list.Contains(t)) list.Add(t);
            }
            return list;
        }

        private void SaveRecent(string entry)
        {
            var list = LoadRecent();
            list.Remove(entry);
            list.Insert(0, entry);
            if (list.Count > MaxRecent) list.RemoveRange(MaxRecent, list.Count - MaxRecent);
            File.WriteAllLines(_recentFile, list);
        }

        private void RemoveRecent(string entry)
        {
            var list = LoadRecent();
            list.Remove(entry);
            File.WriteAllLines(_recentFile, list);
        }

        // ── Azure credentials persistence ─────────────────────────────────────

        private void SaveLastAzureConnection(string containerName)
        {
            File.WriteAllLines(_azureConnFile, new[] { containerName });
        }

        private void LoadLastAzureConnection(out string containerName)
        {
            containerName = "";
            if (!File.Exists(_azureConnFile)) return;
            string[] lines = File.ReadAllLines(_azureConnFile);
            if (lines.Length >= 1) containerName = lines[0].Trim();
        }
    }
}
