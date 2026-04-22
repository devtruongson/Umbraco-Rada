using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff", ".tif" };
        private static readonly Color ColorDownload = Color.FromArgb(52, 73, 94);
        private static readonly Color ColorReplace  = Color.FromArgb(52, 73, 94);
        private static readonly Color ColorPreview  = Color.FromArgb(52, 152, 219);

        private const int MaxRecent = 10;
        private readonly string _recentFile;

        public Form1()
        {
            InitializeComponent();
            StyleGrid();

            string appData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WindowsFormsApp1");
            Directory.CreateDirectory(appData);
            _recentFile = Path.Combine(appData, "recent.txt");
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
                OpenFolder(dialog.SelectedPath);
            }
        }

        private void OpenFolder(string folderPath)
        {
            lblFolderPath.Text = folderPath;
            LoadFiles(folderPath);
            SaveRecent(folderPath);
        }

        private void LoadFiles(string folderPath)
        {
            dataGridView1.Rows.Clear();
            foreach (string filePath in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
            {
                var info = new FileInfo(filePath);
                double sizeMb = info.Length / 1048576.0;
                dataGridView1.Rows.Add(info.Name, info.FullName, sizeMb.ToString("F3"));
            }
        }

        private void btnRecent_Click(object sender, EventArgs e)
        {
            var folders = LoadRecent();
            contextMenuRecent.Items.Clear();

            if (folders.Count == 0)
            {
                contextMenuRecent.Items.Add("(No recent folders)").Enabled = false;
            }
            else
            {
                foreach (string folder in folders)
                {
                    var item = new ToolStripMenuItem(folder);
                    string captured = folder;
                    item.Click += (s, ev) =>
                    {
                        if (!Directory.Exists(captured))
                        {
                            MessageBox.Show("Folder no longer exists:\n" + captured,
                                "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            RemoveRecent(captured);
                            return;
                        }
                        OpenFolder(captured);
                    };
                    contextMenuRecent.Items.Add(item);
                }

                contextMenuRecent.Items.Add(new ToolStripSeparator());
                var clearItem = new ToolStripMenuItem("Clear history");
                clearItem.Click += (s, ev) =>
                {
                    if (File.Exists(_recentFile)) File.Delete(_recentFile);
                };
                contextMenuRecent.Items.Add(clearItem);
            }

            var btn = (Control)sender;
            contextMenuRecent.Show(btn, 0, btn.Height);
        }

        private List<string> LoadRecent()
        {
            var list = new List<string>();
            if (!File.Exists(_recentFile)) return list;
            foreach (string line in File.ReadAllLines(_recentFile))
            {
                string trimmed = line.Trim();
                if (trimmed.Length > 0 && !list.Contains(trimmed))
                    list.Add(trimmed);
            }
            return list;
        }

        private void SaveRecent(string folderPath)
        {
            var list = LoadRecent();
            list.Remove(folderPath);          
            list.Insert(0, folderPath);      
            if (list.Count > MaxRecent) list.RemoveRange(MaxRecent, list.Count - MaxRecent);
            File.WriteAllLines(_recentFile, list);
        }

        private void RemoveRecent(string folderPath)
        {
            var list = LoadRecent();
            list.Remove(folderPath);
            File.WriteAllLines(_recentFile, list);
        }



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string filePath = dataGridView1.Rows[e.RowIndex].Cells["colPath"].Value?.ToString();
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

            int col = e.ColumnIndex;
            if (col == dataGridView1.Columns["colDownload"].Index) DownloadFile(filePath);
            else if (col == dataGridView1.Columns["colReplace"].Index) ReplaceFile(filePath, e.RowIndex);
            else if (col == dataGridView1.Columns["colPreview"].Index) PreviewFile(filePath);
        }

        private void DownloadFile(string filePath)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.FileName = Path.GetFileName(filePath);
                dialog.Filter = "All files (*.*)|*.*";
                if (dialog.ShowDialog() != DialogResult.OK) return;
                File.Copy(filePath, dialog.FileName, overwrite: true);
                MessageBox.Show("File saved successfully.", "Download", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("File replaced successfully.", "Replace", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void PreviewFile(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            if (Array.IndexOf(ImageExtensions, ext) < 0)
            {
                Process.Start(new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                });
                return;
            }
            try { new PreviewForm(filePath).Show(this); }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot preview file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.Start(new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                });
            }
        }

        private void UpdateRowSize(int rowIndex, string filePath)
        {
            var info = new FileInfo(filePath);
            dataGridView1.Rows[rowIndex].Cells["colSize"].Value = (info.Length / 1048576.0).ToString("F3");
        }
    }
}
