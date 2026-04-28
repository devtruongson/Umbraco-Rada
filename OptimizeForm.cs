using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Azure.Storage.Blobs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
// Aliases: resolve System.Drawing vs SixLabors.ImageSharp name conflicts
using SDPoint  = System.Drawing.Point;
using SDSize   = System.Drawing.Size;
using SDImage  = System.Drawing.Image;
using SDBitmap = System.Drawing.Bitmap;

namespace WindowsFormsApp1
{
    public class OptimizeItem
    {
        public string DisplayName   { get; set; }
        public string OriginalPath  { get; set; }
        public string OptimizedPath { get; set; }
        public string OutExtension  { get; set; }
        public long   OriginalSize  { get; set; }
        public long   OptimizedSize { get; set; }
        public bool   IsOptimized   { get; set; }
        public bool   IsAzure       { get; set; }
        public string BlobName      { get; set; }
        public int    GridRowIndex  { get; set; }
        public bool   Replaced      { get; set; }
        public string Error         { get; set; }
    }

    public class OptimizeForm : Form
    {
        private readonly List<OptimizeItem> _items;
        private readonly BlobContainerClient _azure;

        private static readonly string TempDir =
            Path.Combine(Path.GetTempPath(), "UmbOptimize");

        // Controls
        private Label       lblHeaderCount;
        private TrackBar    trackQuality;
        private Label       lblQuality;
        private CheckBox    chkLossyPng;
        private Button      btnRunAll;
        private ProgressBar progressBar;
        private ListView    listFiles;
        private PictureBox  pictOrig, pictOpt;
        private Label       lblOrigInfo, lblOptInfo;
        private Label       lblOrigTitle, lblOptTitle;
        private Label       lblStatus;
        private Button      btnReplace, btnReplaceAll;

        public List<OptimizeItem> Items => _items;

        public OptimizeForm(List<OptimizeItem> items, BlobContainerClient azure = null)
        {
            _items = items;
            _azure = azure;
            Directory.CreateDirectory(TempDir);
            BuildUI();
            PopulateList();
            UpdateStatus();
        }

        // ── UI ────────────────────────────────────────────────────────────────

        private void BuildUI()
        {
            Text            = "Optimize Images";
            Size            = new SDSize(1060, 720);
            MinimumSize     = new SDSize(860, 580);
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = System.Drawing.Color.White;
            Font            = new System.Drawing.Font("Segoe UI", 9.5F);
            KeyPreview      = true;

            // ── Header ────────────────────────────────────────────────────────
            var header = new Panel { Dock = DockStyle.Top, Height = 52,
                BackColor = System.Drawing.Color.FromArgb(30, 30, 30) };

            var lblTitle = new Label {
                Text = "Optimize Images",
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 13F, FontStyle.Bold),
                Location = new SDPoint(16, 0), Height = 52, AutoSize = false,
                Width = 260, TextAlign = ContentAlignment.MiddleLeft
            };

            lblHeaderCount = new Label {
                ForeColor = System.Drawing.Color.FromArgb(160, 160, 160),
                Font = new System.Drawing.Font("Segoe UI", 9.5F),
                Location = new SDPoint(276, 0), Height = 52, AutoSize = false,
                Width = 300, TextAlign = ContentAlignment.MiddleLeft,
                Text = _items.Count + " file(s) selected"
            };

            header.Controls.Add(lblTitle);
            header.Controls.Add(lblHeaderCount);

            // ── Toolbar ───────────────────────────────────────────────────────
            var toolbar = new Panel { Dock = DockStyle.Top, Height = 54,
                BackColor = System.Drawing.Color.FromArgb(248, 248, 248) };

            var lblQL = new Label {
                Text = "JPEG Quality:", AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(55, 55, 55),
                Location = new SDPoint(14, 18) };

            trackQuality = new TrackBar {
                Minimum = 40, Maximum = 100, Value = 80, TickFrequency = 10,
                Location = new SDPoint(118, 10), Width = 200,
                BackColor = System.Drawing.Color.FromArgb(248, 248, 248) };

            lblQuality = new Label {
                Text = "80%", AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(0, 120, 212),
                Location = new SDPoint(325, 18) };

            var sep1 = new Label {
                Text = "|", AutoSize = true,
                ForeColor = System.Drawing.Color.FromArgb(200, 200, 200),
                Location = new SDPoint(364, 16) };

            chkLossyPng = new CheckBox {
                Text = "Lossy PNG  (TinyPNG-style, −60–80%)",
                Location = new SDPoint(380, 17), AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9F),
                ForeColor = System.Drawing.Color.FromArgb(55, 55, 55),
                Checked = true };

            btnRunAll = new Button {
                Text = "▶  Optimize All",
                Location = new SDPoint(700, 13), Size = new SDSize(150, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand };
            btnRunAll.FlatAppearance.BorderSize = 0;

            trackQuality.ValueChanged += (s, e) => lblQuality.Text = trackQuality.Value + "%";
            btnRunAll.Click += RunOptimization;
            toolbar.Controls.AddRange(new Control[] {
                lblQL, trackQuality, lblQuality, sep1, chkLossyPng, btnRunAll });

            // ── Progress bar (hidden until optimization runs) ─────────────────
            progressBar = new ProgressBar {
                Dock = DockStyle.Top, Height = 5,
                Style = ProgressBarStyle.Continuous,
                ForeColor = System.Drawing.Color.FromArgb(0, 120, 212),
                Visible = false };

            // ── Bottom bar ────────────────────────────────────────────────────
            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 52,
                BackColor = System.Drawing.Color.FromArgb(248, 248, 248) };

            var sepBottom = new Panel { Dock = DockStyle.Top, Height = 1,
                BackColor = System.Drawing.Color.FromArgb(220, 220, 220) };

            lblStatus = new Label {
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(14, 0, 0, 0),
                Font = new System.Drawing.Font("Segoe UI", 9F),
                ForeColor = System.Drawing.Color.FromArgb(60, 60, 60) };

            btnReplaceAll = new Button {
                Text = "Replace All", Dock = DockStyle.Right, Width = 120,
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(52, 73, 94),
                ForeColor = System.Drawing.Color.White,
                Enabled = false, Cursor = Cursors.Hand };
            btnReplaceAll.FlatAppearance.BorderSize = 0;

            btnReplace = new Button {
                Text = "Replace", Dock = DockStyle.Right, Width = 100,
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(39, 174, 96),
                ForeColor = System.Drawing.Color.White,
                Enabled = false, Cursor = Cursors.Hand };
            btnReplace.FlatAppearance.BorderSize = 0;

            btnReplace.Click    += ReplaceSelected;
            btnReplaceAll.Click += ReplaceAll;
            panelBottom.Controls.AddRange(new Control[] { lblStatus, btnReplaceAll, btnReplace, sepBottom });

            // ── Main split: list | preview ────────────────────────────────────
            var split = new SplitContainer {
                Dock = DockStyle.Fill, Orientation = Orientation.Vertical };
            this.Shown += (s, e) => {
                try { split.Panel1MinSize = 200; split.Panel2MinSize = 200;
                      split.SplitterDistance = 360; } catch { } };

            // ── Left: file list ───────────────────────────────────────────────
            var leftPanel = new Panel { Dock = DockStyle.Fill };

            listFiles = new ListView {
                Dock = DockStyle.Fill, View = View.Details,
                FullRowSelect = true, GridLines = false, MultiSelect = false,
                HideSelection = false,
                Font = new System.Drawing.Font("Segoe UI", 9F),
                BackColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None };
            listFiles.Columns.Add("File",      160);
            listFiles.Columns.Add("Original",   78);
            listFiles.Columns.Add("Optimized",  78);
            listFiles.Columns.Add("Saved",       60);

            var lblHint = new Label {
                Text = "↑↓ arrow keys or click to preview",
                Dock = DockStyle.Bottom, Height = 24,
                BackColor = System.Drawing.Color.FromArgb(245, 245, 245),
                ForeColor = System.Drawing.Color.FromArgb(140, 140, 140),
                Font = new System.Drawing.Font("Segoe UI", 8F),
                TextAlign = ContentAlignment.MiddleCenter };

            // Mini toolbar above the list (Remove button)
            var listToolbar = new Panel {
                Dock = DockStyle.Top, Height = 30,
                BackColor = System.Drawing.Color.FromArgb(240, 240, 240) };
            var sepListToolbar = new Panel { Dock = DockStyle.Bottom, Height = 1,
                BackColor = System.Drawing.Color.FromArgb(210, 210, 210) };

            var btnRemove = new Button {
                Text = "✕  Remove Selected",
                Location = new SDPoint(6, 4), Size = new SDSize(140, 22),
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(240, 240, 240),
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 40),
                Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand };
            btnRemove.FlatAppearance.BorderSize = 1;
            btnRemove.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 150, 150);
            btnRemove.Click += RemoveSelected;
            listToolbar.Controls.Add(btnRemove);
            listToolbar.Controls.Add(sepListToolbar);

            // Add order: Fill first (processed last in layout), then Bottom, then Top
            leftPanel.Controls.Add(listFiles);
            leftPanel.Controls.Add(lblHint);
            leftPanel.Controls.Add(listToolbar);
            split.Panel1.Controls.Add(leftPanel);

            // Click: use MouseClick + HitTest — fixes the "need two clicks" bug
            listFiles.MouseClick += (s, e) => {
                var hit = listFiles.HitTest(e.X, e.Y);
                if (hit.Item != null) SelectAndPreview(hit.Item);
            };
            // Keyboard: Up/Down via BeginInvoke so selection is committed first
            listFiles.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                    this.BeginInvoke(new Action(() => {
                        if (listFiles.SelectedItems.Count > 0)
                            SelectAndPreview(listFiles.SelectedItems[0]);
                    }));
            };

            // ── Right: before / after preview ────────────────────────────────
            var splitPrev = new SplitContainer {
                Dock = DockStyle.Fill, Orientation = Orientation.Vertical };
            split.Panel2.Controls.Add(splitPrev);

            var panelOrig = BuildPreviewPanel("ORIGINAL",
                System.Drawing.Color.FromArgb(65, 65, 68),
                System.Drawing.Color.FromArgb(215, 215, 215),
                out pictOrig, out lblOrigTitle, out lblOrigInfo);
            splitPrev.Panel1.Controls.Add(panelOrig);

            var panelOpt = BuildPreviewPanel("OPTIMIZED",
                System.Drawing.Color.FromArgb(39, 174, 96),
                System.Drawing.Color.FromArgb(22, 22, 22),
                out pictOpt, out lblOptTitle, out lblOptInfo);
            splitPrev.Panel2.Controls.Add(panelOpt);
            lblOptInfo.Text = "Run optimization first";

            // ── Assemble (order matters for Dock) ─────────────────────────────
            var toolSep = new Panel { Dock = DockStyle.Top, Height = 1,
                BackColor = System.Drawing.Color.FromArgb(220, 220, 220) };
            Controls.Add(split);
            Controls.Add(panelBottom);
            Controls.Add(progressBar);
            Controls.Add(toolSep);
            Controls.Add(toolbar);
            Controls.Add(header);

            FormClosed += (s, e) => { ClearPicture(pictOrig); ClearPicture(pictOpt); };
        }

        private static Panel BuildPreviewPanel(string title,
            System.Drawing.Color hdrColor, System.Drawing.Color bgColor,
            out PictureBox pict, out Label titleLabel, out Label info)
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = bgColor };

            titleLabel = new Label {
                Text = title, Dock = DockStyle.Top, Height = 30,
                BackColor = hdrColor, ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 8.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter };

            pict = new PictureBox {
                Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = bgColor };

            info = new Label {
                Dock = DockStyle.Bottom, Height = 28,
                BackColor = hdrColor, ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 8.5F),
                TextAlign = ContentAlignment.MiddleCenter, Text = "—" };

            panel.Controls.Add(pict);
            panel.Controls.Add(titleLabel);
            panel.Controls.Add(info);
            return panel;
        }

        // ── Population ────────────────────────────────────────────────────────

        private void PopulateList()
        {
            listFiles.Items.Clear();
            foreach (var item in _items)
            {
                var lvi = new ListViewItem(TruncateName(item.DisplayName, 22));
                lvi.SubItems.Add(FmtSize(item.OriginalSize));
                lvi.SubItems.Add("—");
                lvi.SubItems.Add("—");
                lvi.Tag = item;
                listFiles.Items.Add(lvi);
            }
        }

        private static string TruncateName(string name, int max)
        {
            if (name.Length <= max) return name;
            string ext = Path.GetExtension(name);
            int keep = max - ext.Length - 2;
            return keep > 0 ? name.Substring(0, keep) + "…" + ext : name.Substring(0, max) + "…";
        }

        // ── Optimization ──────────────────────────────────────────────────────

        private void RunOptimization(object sender, EventArgs e)
        {
            btnRunAll.Enabled   = false;
            btnRunAll.Text      = "Working…";
            btnReplaceAll.Enabled = false;
            btnReplace.Enabled  = false;
            Cursor              = Cursors.WaitCursor;
            progressBar.Maximum = _items.Count;
            progressBar.Value   = 0;
            progressBar.Visible = true;

            long totalOrig = 0, totalOpt = 0;
            int  improved  = 0;
            int  firstGoodIndex = -1;

            try
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    var lvi  = listFiles.Items[i];

                    // Download Azure blob to temp first
                    if (item.IsAzure && _azure != null && !File.Exists(item.OriginalPath))
                    {
                        SetItemStatus(lvi, "Downloading…", System.Drawing.Color.FromArgb(100, 100, 100));
                        _azure.GetBlobClient(item.BlobName).DownloadTo(item.OriginalPath);
                        item.OriginalSize    = new FileInfo(item.OriginalPath).Length;
                        lvi.SubItems[1].Text = FmtSize(item.OriginalSize);
                    }

                    SetItemStatus(lvi, "Optimizing…", System.Drawing.Color.FromArgb(100, 100, 100));

                    try
                    {
                        string srcExt  = Path.GetExtension(item.OriginalPath).ToLower();
                        string outExt  = srcExt == ".bmp" ? ".png" : srcExt;
                        string outPath = Path.Combine(TempDir,
                            Path.GetFileNameWithoutExtension(item.OriginalPath)
                            + "_opt_" + i + outExt);

                        OptimizeFile(item.OriginalPath, outPath, trackQuality.Value,
                            chkLossyPng.Checked);

                        item.OptimizedPath = outPath;
                        item.OutExtension  = outExt;
                        item.OptimizedSize = new FileInfo(outPath).Length;
                        item.IsOptimized   = true;
                        item.Error         = null;

                        long   saved = item.OriginalSize - item.OptimizedSize;
                        double pct   = item.OriginalSize > 0
                            ? saved * 100.0 / item.OriginalSize : 0;

                        if (item.OptimizedSize >= item.OriginalSize)
                        {
                            File.Copy(item.OriginalPath, outPath, overwrite: true);
                            item.OptimizedSize   = item.OriginalSize;
                            lvi.SubItems[2].Text = FmtSize(item.OptimizedSize);
                            lvi.SubItems[3].Text = "Already optimal";
                            lvi.ForeColor        = System.Drawing.Color.FromArgb(150, 150, 150);
                        }
                        else
                        {
                            lvi.SubItems[2].Text = FmtSize(item.OptimizedSize);
                            lvi.SubItems[3].Text = "-" + pct.ToString("F0") + "%";
                            lvi.ForeColor        = System.Drawing.Color.FromArgb(0, 130, 0);
                            improved++;
                            if (firstGoodIndex < 0) firstGoodIndex = i;
                        }

                        totalOrig += item.OriginalSize;
                        totalOpt  += item.OptimizedSize;
                    }
                    catch (Exception ex)
                    {
                        item.Error = ex.Message;
                        lvi.SubItems[2].Text = "Error";
                        lvi.SubItems[3].Text = "—";
                        lvi.ForeColor        = System.Drawing.Color.Crimson;
                    }

                    progressBar.Value = i + 1;
                    progressBar.Refresh();
                }

                btnReplaceAll.Enabled = improved > 0;
                UpdateReplaceAllText(improved);

                // Auto-select: prefer first improved item, else first item
                int selectIdx = firstGoodIndex >= 0 ? firstGoodIndex : 0;
                if (listFiles.Items.Count > 0)
                {
                    listFiles.Items[selectIdx].Selected = true;
                    listFiles.Items[selectIdx].EnsureVisible();
                    SelectAndPreview(listFiles.Items[selectIdx]);
                }
            }
            finally
            {
                btnRunAll.Enabled  = true;
                btnRunAll.Text     = "▶  Optimize All";
                progressBar.Visible = false;
                Cursor = Cursors.Default;
                UpdateStatus();
            }
        }

        private static void SetItemStatus(ListViewItem lvi, string text,
            System.Drawing.Color color)
        {
            lvi.SubItems[2].Text = text;
            lvi.ForeColor        = color;
            lvi.ListView?.Refresh();
        }

        private static void OptimizeFile(string src, string dst, int quality, bool lossyPng)
        {
            string ext = Path.GetExtension(src).ToLower();
            using (var img = SixLabors.ImageSharp.Image.Load(src))
            {
                if (ext == ".png" || ext == ".bmp")
                {
                    if (lossyPng)
                    {
                        img.Mutate(ctx => ctx.Quantize(
                            new WuQuantizer(new QuantizerOptions {
                                MaxColors = 256,
                                Dither    = KnownDitherings.Bayer8x8 })));
                        img.SaveAsPng(dst, new PngEncoder {
                            ColorType        = PngColorType.Palette,
                            CompressionLevel = PngCompressionLevel.BestCompression });
                    }
                    else
                    {
                        img.SaveAsPng(dst, new PngEncoder {
                            CompressionLevel = PngCompressionLevel.BestCompression });
                    }
                }
                else if (ext == ".gif")
                    img.SaveAsGif(dst, new GifEncoder());
                else
                    img.SaveAsJpeg(dst, new JpegEncoder {
                        Quality   = quality,
                        ColorType = JpegColorType.YCbCrRatio420 });
            }
        }

        // ── Preview ───────────────────────────────────────────────────────────

        private void SelectAndPreview(ListViewItem lvi)
        {
            var item = lvi.Tag as OptimizeItem;
            ShowPreview(item);
            btnReplace.Enabled = item != null && item.IsOptimized
                              && !item.Replaced
                              && item.OptimizedSize < item.OriginalSize;
        }

        private void ShowPreview(OptimizeItem item)
        {
            ClearPicture(pictOrig);
            ClearPicture(pictOpt);
            if (item == null) return;

            // Original
            if (File.Exists(item.OriginalPath))
            {
                try
                {
                    pictOrig.Image   = LoadBitmapSafe(item.OriginalPath);
                    lblOrigInfo.Text = FmtSize(item.OriginalSize)
                        + "  ·  " + GetDimensions(item.OriginalPath);
                }
                catch { lblOrigInfo.Text = FmtSize(item.OriginalSize); }
            }
            else
            {
                lblOrigInfo.Text = FmtSize(item.OriginalSize)
                    + (item.IsAzure ? "  — run Optimize All to download" : "  — file not found");
            }

            // Optimized
            if (item.IsOptimized && File.Exists(item.OptimizedPath))
            {
                try
                {
                    pictOpt.Image = LoadBitmapSafe(item.OptimizedPath);
                    long   saved = item.OriginalSize - item.OptimizedSize;
                    double pct   = item.OriginalSize > 0
                        ? saved * 100.0 / item.OriginalSize : 0;

                    if (pct > 0)
                    {
                        lblOptInfo.Text = FmtSize(item.OptimizedSize)
                            + "   ▼ " + FmtSize(saved) + " smaller  (" + pct.ToString("F1") + "%)";
                        lblOptTitle.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
                    }
                    else
                    {
                        lblOptInfo.Text = "Already at optimal size";
                        lblOptTitle.BackColor = System.Drawing.Color.FromArgb(130, 130, 130);
                    }
                }
                catch
                {
                    pictOpt.Image   = null;
                    lblOptInfo.Text = "Preview error";
                }
            }
            else
            {
                lblOptInfo.Text = item.Error != null
                    ? "⚠ " + item.Error
                    : "Run Optimize All first";
                lblOptTitle.BackColor = System.Drawing.Color.FromArgb(65, 65, 68);
            }
        }

        private static string GetDimensions(string path)
        {
            try
            {
                using (var ms = new MemoryStream(File.ReadAllBytes(path)))
                using (var img = SDImage.FromStream(ms))
                    return img.Width + " × " + img.Height + " px";
            }
            catch { return ""; }
        }

        private static SDBitmap LoadBitmapSafe(string path)
        {
            // Fast path: System.Drawing
            try
            {
                byte[] data = File.ReadAllBytes(path);
                using (var ms = new MemoryStream(data))
                using (var tmp = SDImage.FromStream(ms))
                {
                    var bmp = new SDBitmap(tmp.Width, tmp.Height, PixelFormat.Format32bppArgb);
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.Clear(System.Drawing.Color.White); // white bg so transparent PNGs are visible
                        g.DrawImage(tmp, 0, 0, tmp.Width, tmp.Height);
                    }
                    return bmp;
                }
            }
            catch { }

            // Fallback: decode via ImageSharp → re-encode as standard RGBA PNG → load
            byte[] pngBytes;
            using (var img = SixLabors.ImageSharp.Image.Load(path))
            using (var ms2 = new MemoryStream())
            {
                img.SaveAsPng(ms2, new PngEncoder { ColorType = PngColorType.RgbWithAlpha });
                pngBytes = ms2.ToArray();
            }
            using (var ms3 = new MemoryStream(pngBytes))
            using (var tmp3 = SDImage.FromStream(ms3))
            {
                var bmp3 = new SDBitmap(tmp3.Width, tmp3.Height, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(bmp3))
                {
                    g.Clear(System.Drawing.Color.White);
                    g.DrawImage(tmp3, 0, 0, tmp3.Width, tmp3.Height);
                }
                return bmp3;
            }
        }

        private static void ClearPicture(PictureBox pb)
        {
            var old = pb.Image;
            pb.Image = null;
            old?.Dispose();
        }

        // ── Replace ───────────────────────────────────────────────────────────

        private void ReplaceSelected(object sender, EventArgs e)
        {
            if (listFiles.SelectedItems.Count == 0) return;
            var item = listFiles.SelectedItems[0].Tag as OptimizeItem;
            if (item == null || !item.IsOptimized || item.Replaced) return;
            DoReplace(item, listFiles.SelectedItems[0]);
            UpdateStatus();
        }

        private void ReplaceAll(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    if (!item.IsOptimized || item.Replaced) continue;
                    if (item.OptimizedSize >= item.OriginalSize) continue;
                    DoReplace(item, listFiles.Items[i]);
                }
            }
            finally { Cursor = Cursors.Default; }
            UpdateStatus();
        }

        private void RemoveSelected(object sender, EventArgs e)
        {
            if (listFiles.SelectedItems.Count == 0) return;
            var lvi  = listFiles.SelectedItems[0];
            var item = lvi.Tag as OptimizeItem;
            if (item == null) return;

            int idx = listFiles.Items.IndexOf(lvi);
            _items.Remove(item);
            listFiles.Items.Remove(lvi);

            // Clean up temp optimized file
            if (!string.IsNullOrEmpty(item.OptimizedPath) && File.Exists(item.OptimizedPath))
                try { File.Delete(item.OptimizedPath); } catch { }

            lblHeaderCount.Text = _items.Count + " file(s) selected";

            if (listFiles.Items.Count > 0)
            {
                int nextIdx = Math.Min(idx, listFiles.Items.Count - 1);
                listFiles.Items[nextIdx].Selected = true;
                listFiles.Items[nextIdx].EnsureVisible();
                SelectAndPreview(listFiles.Items[nextIdx]);
            }
            else
            {
                ClearPicture(pictOrig);
                ClearPicture(pictOpt);
                lblOrigInfo.Text = "—";
                lblOptInfo.Text  = "—";
                btnReplace.Enabled    = false;
                btnReplaceAll.Enabled = false;
            }

            UpdateStatus();
            UpdateReplaceAllText(0);
        }

        private void DoReplace(OptimizeItem item, ListViewItem lvi)
        {
            try
            {
                if (item.IsAzure && _azure != null)
                {
                    using (var stream = File.OpenRead(item.OptimizedPath))
                        _azure.GetBlobClient(item.BlobName).Upload(stream, overwrite: true);
                }
                else
                {
                    string srcExt = Path.GetExtension(item.OriginalPath).ToLower();
                    string dest   = item.OutExtension != srcExt
                        ? Path.ChangeExtension(item.OriginalPath, item.OutExtension)
                        : item.OriginalPath;

                    if (dest != item.OriginalPath && File.Exists(item.OriginalPath))
                        File.Delete(item.OriginalPath);

                    File.Copy(item.OptimizedPath, dest, overwrite: true);
                    item.OriginalPath = dest;
                }

                item.Replaced        = true;
                lvi.SubItems[3].Text = "✓ Replaced";
                lvi.ForeColor        = System.Drawing.Color.FromArgb(0, 100, 200);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to replace " + item.DisplayName + ":\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (listFiles.SelectedItems.Count > 0 &&
                listFiles.SelectedItems[0].Tag == item)
                btnReplace.Enabled = false;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private void UpdateStatus()
        {
            int  total = _items.Count, opt = 0, replaced = 0;
            long saved = 0;

            foreach (var it in _items)
            {
                if (it.IsOptimized) { opt++; saved += it.OriginalSize - it.OptimizedSize; }
                if (it.Replaced)    replaced++;
            }

            if (opt == 0)
            {
                lblStatus.Text = total + " file(s) ready  —  adjust quality then click Optimize All";
            }
            else if (replaced == total || (replaced > 0 && replaced == opt))
            {
                lblStatus.Text = "Done!  Replaced " + replaced + "/" + total
                    + " file(s)  ·  Saved " + FmtSize(saved) + " total";
                lblStatus.ForeColor = System.Drawing.Color.FromArgb(0, 130, 0);
            }
            else if (replaced > 0)
            {
                lblStatus.Text = replaced + " replaced  ·  " + (opt - replaced)
                    + " pending  ·  " + FmtSize(saved) + " saved so far";
                lblStatus.ForeColor = System.Drawing.Color.FromArgb(0, 100, 200);
            }
            else
            {
                int improved = 0;
                foreach (var it in _items)
                    if (it.IsOptimized && it.OptimizedSize < it.OriginalSize) improved++;

                lblStatus.Text = improved + "/" + total + " can be reduced  ·  potential saving: "
                    + FmtSize(saved)
                    + "  —  click Replace / Replace All to apply";
                lblStatus.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            }

            UpdateReplaceAllText(0); // refresh button text
        }

        private void UpdateReplaceAllText(int improved)
        {
            int count = 0;
            foreach (var it in _items)
                if (it.IsOptimized && !it.Replaced && it.OptimizedSize < it.OriginalSize) count++;
            btnReplaceAll.Text    = count > 0 ? "Replace All (" + count + ")" : "Replace All";
            btnReplaceAll.Enabled = count > 0;
        }

        private static string FmtSize(long bytes)
        {
            if (bytes >= 1048576) return (bytes / 1048576.0).ToString("F2") + " MB";
            if (bytes >= 1024)    return (bytes / 1024.0).ToString("F0") + " KB";
            return bytes + " B";
        }
    }
}
