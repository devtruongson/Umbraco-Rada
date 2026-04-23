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
// Aliases to resolve conflicts between System.Drawing and SixLabors.ImageSharp
using SDPoint   = System.Drawing.Point;
using SDSize    = System.Drawing.Size;
using SDImage   = System.Drawing.Image;
using SDBitmap  = System.Drawing.Bitmap;

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

        private TrackBar   trackQuality;
        private Label      lblQuality;
        private CheckBox   chkLossyPng;
        private Button     btnRunAll;
        private ListView   listFiles;
        private PictureBox pictOrig, pictOpt;
        private Label      lblOrigInfo, lblOptInfo;
        private Label      lblStatus;
        private Button     btnReplace, btnReplaceAll;

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
            Size            = new SDSize(1020, 700);
            MinimumSize     = new SDSize(820, 560);
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = System.Drawing.Color.White;
            Font            = new System.Drawing.Font("Segoe UI", 9.5F);

            // Header
            var header = new Panel { Dock = DockStyle.Top, Height = 50,
                BackColor = System.Drawing.Color.FromArgb(45, 45, 48) };
            header.Controls.Add(new Label {
                Text = "Optimize Images",
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 13F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(14, 0, 0, 0)
            });

            // Toolbar
            var toolbar = new Panel { Dock = DockStyle.Top, Height = 52,
                BackColor = System.Drawing.Color.FromArgb(245, 245, 245) };

            var lblQL = new Label {
                Text = "JPEG Quality:",
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(55, 55, 55),
                Location = new SDPoint(14, 17)
            };

            trackQuality = new TrackBar {
                Minimum = 40, Maximum = 100, Value = 80, TickFrequency = 10,
                Location = new SDPoint(118, 8), Width = 220,
                BackColor = System.Drawing.Color.FromArgb(245, 245, 245)
            };

            lblQuality = new Label {
                Text = "80%", AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(0, 120, 212),
                Location = new SDPoint(346, 17)
            };

            chkLossyPng = new CheckBox {
                Text = "Lossy PNG (like TinyPNG)",
                Location = new SDPoint(400, 16),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9F),
                ForeColor = System.Drawing.Color.FromArgb(55, 55, 55),
                Checked = true
            };

            btnRunAll = new Button {
                Text = "Optimize All", Location = new SDPoint(610, 12),
                Size = new SDSize(130, 30), FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(0, 120, 212),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRunAll.FlatAppearance.BorderSize = 0;

            trackQuality.ValueChanged += (s, e) => lblQuality.Text = trackQuality.Value + "%";
            btnRunAll.Click += RunOptimization;
            toolbar.Controls.AddRange(new Control[] { lblQL, trackQuality, lblQuality, chkLossyPng, btnRunAll });

            // Bottom bar
            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50,
                BackColor = System.Drawing.Color.FromArgb(245, 245, 245) };

            lblStatus = new Label {
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                ForeColor = System.Drawing.Color.FromArgb(60, 60, 60)
            };

            btnReplaceAll = new Button {
                Text = "Replace All", Dock = DockStyle.Right, Width = 115,
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(52, 73, 94),
                ForeColor = System.Drawing.Color.White,
                Enabled = false, Cursor = Cursors.Hand
            };
            btnReplaceAll.FlatAppearance.BorderSize = 0;

            btnReplace = new Button {
                Text = "Replace", Dock = DockStyle.Right, Width = 95,
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(39, 174, 96),
                ForeColor = System.Drawing.Color.White,
                Enabled = false, Cursor = Cursors.Hand
            };
            btnReplace.FlatAppearance.BorderSize = 0;

            btnReplace.Click    += ReplaceSelected;
            btnReplaceAll.Click += ReplaceAll;
            panelBottom.Controls.AddRange(new Control[] { lblStatus, btnReplaceAll, btnReplace });

            // Main split: list | preview
            // Do NOT set Panel1MinSize, Panel2MinSize, or SplitterDistance here —
            // SplitContainer width is 0 at construction and those setters call
            // SplitterDistance internally, causing InvalidOperationException.
            var split = new SplitContainer {
                Dock = DockStyle.Fill, Orientation = Orientation.Vertical
            };
            this.Shown += (s, e) =>
            {
                try
                {
                    split.Panel1MinSize    = 200;
                    split.Panel2MinSize    = 200;
                    split.SplitterDistance = 340;
                }
                catch { }
            };

            // Left: ListView
            listFiles = new ListView {
                Dock = DockStyle.Fill, View = View.Details,
                FullRowSelect = true, GridLines = true, MultiSelect = false,
                Font = new System.Drawing.Font("Segoe UI", 9F)
            };
            listFiles.Columns.Add("File",      155);
            listFiles.Columns.Add("Original",   75);
            listFiles.Columns.Add("Optimized",  75);
            listFiles.Columns.Add("Saved",       55);
            listFiles.SelectedIndexChanged += OnListSelectionChanged;
            split.Panel1.Controls.Add(listFiles);

            // Right: before / after preview
            var splitPrev = new SplitContainer {
                Dock = DockStyle.Fill, Orientation = Orientation.Vertical
            };
            split.Panel2.Controls.Add(splitPrev);

            var panelOrig = BuildPreviewPanel("ORIGINAL",
                System.Drawing.Color.FromArgb(70, 70, 70),
                System.Drawing.Color.FromArgb(220, 220, 220),
                out pictOrig, out lblOrigInfo);
            splitPrev.Panel1.Controls.Add(panelOrig);

            var panelOpt = BuildPreviewPanel("OPTIMIZED",
                System.Drawing.Color.FromArgb(39, 174, 96),
                System.Drawing.Color.FromArgb(20, 20, 20),
                out pictOpt, out lblOptInfo);
            splitPrev.Panel2.Controls.Add(panelOpt);
            lblOptInfo.Text = "— run optimization first";

            // Assemble
            var sep = new Panel { Dock = DockStyle.Top, Height = 1,
                BackColor = System.Drawing.Color.FromArgb(220, 220, 220) };
            Controls.Add(split);
            Controls.Add(panelBottom);
            Controls.Add(sep);
            Controls.Add(toolbar);
            Controls.Add(header);

            FormClosed += (s, e) => { ClearPicture(pictOrig); ClearPicture(pictOpt); };
        }

        private static Panel BuildPreviewPanel(string title,
            System.Drawing.Color headerColor, System.Drawing.Color bgColor,
            out PictureBox pict, out Label info)
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = bgColor };

            var hdr = new Label {
                Text = title, Dock = DockStyle.Top, Height = 28,
                BackColor = headerColor, ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 8.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pict = new PictureBox {
                Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = bgColor
            };

            info = new Label {
                Dock = DockStyle.Bottom, Height = 26,
                BackColor = headerColor, ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 8.5F),
                TextAlign = ContentAlignment.MiddleCenter, Text = "—"
            };

            panel.Controls.Add(pict);
            panel.Controls.Add(hdr);
            panel.Controls.Add(info);
            return panel;
        }

        // ── Population ────────────────────────────────────────────────────────

        private void PopulateList()
        {
            listFiles.Items.Clear();
            foreach (var item in _items)
            {
                var lvi = new ListViewItem(item.DisplayName);
                lvi.SubItems.Add(FmtSize(item.OriginalSize));
                lvi.SubItems.Add("—");
                lvi.SubItems.Add("—");
                lvi.Tag = item;
                listFiles.Items.Add(lvi);
            }
        }

        // ── Optimization ──────────────────────────────────────────────────────

        private void RunOptimization(object sender, EventArgs e)
        {
            btnRunAll.Enabled = false;
            btnRunAll.Text    = "Working…";
            Cursor            = Cursors.WaitCursor;

            long totalOrig = 0, totalOpt = 0;
            int  improved  = 0;

            try
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    var lvi  = listFiles.Items[i];

                    if (item.IsAzure && _azure != null && !File.Exists(item.OriginalPath))
                    {
                        lvi.SubItems[2].Text = "Downloading…";
                        listFiles.Refresh();
                        _azure.GetBlobClient(item.BlobName).DownloadTo(item.OriginalPath);
                        item.OriginalSize    = new FileInfo(item.OriginalPath).Length;
                        lvi.SubItems[1].Text = FmtSize(item.OriginalSize);
                    }

                    lvi.SubItems[2].Text = "Optimizing…";
                    listFiles.Refresh();

                    try
                    {
                        string srcExt  = Path.GetExtension(item.OriginalPath).ToLower();
                        string outExt  = srcExt == ".bmp" ? ".png" : srcExt;
                        string outPath = Path.Combine(TempDir,
                            Path.GetFileNameWithoutExtension(item.OriginalPath) + "_opt" + outExt);

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
                            // Optimized is not smaller — keep original, mark as no gain
                            File.Copy(item.OriginalPath, outPath, overwrite: true);
                            item.OptimizedSize     = item.OriginalSize;
                            lvi.SubItems[2].Text   = FmtSize(item.OptimizedSize);
                            lvi.SubItems[3].Text   = "Already optimal";
                            lvi.ForeColor          = System.Drawing.Color.DimGray;
                        }
                        else
                        {
                            lvi.SubItems[2].Text = FmtSize(item.OptimizedSize);
                            lvi.SubItems[3].Text = "-" + pct.ToString("F0") + "%";
                            lvi.ForeColor        = System.Drawing.Color.FromArgb(0, 110, 0);
                            improved++;
                        }

                        totalOrig += item.OriginalSize;
                        totalOpt  += item.OptimizedSize;
                    }
                    catch (Exception ex)
                    {
                        item.Error           = ex.Message;
                        lvi.SubItems[2].Text = "Error";
                        lvi.SubItems[3].Text = "—";
                        lvi.ForeColor        = System.Drawing.Color.Crimson;
                    }
                }

                if (listFiles.Items.Count > 0)
                    listFiles.Items[0].Selected = true;

                btnReplaceAll.Enabled = improved > 0;
            }
            finally
            {
                btnRunAll.Enabled = true;
                btnRunAll.Text    = "Optimize All";
                Cursor            = Cursors.Default;
                UpdateStatus();
            }
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
                        // Lossy: quantize to 256-color palette (like TinyPNG) — 60-80% smaller
                        img.Mutate(ctx => ctx.Quantize(
                            new WuQuantizer(new QuantizerOptions
                            {
                                MaxColors = 256,
                                Dither    = KnownDitherings.Bayer8x8
                            })));
                        img.SaveAsPng(dst, new PngEncoder {
                            ColorType        = PngColorType.Palette,
                            CompressionLevel = PngCompressionLevel.BestCompression
                        });
                    }
                    else
                    {
                        // Lossless: best deflate compression only
                        img.SaveAsPng(dst, new PngEncoder {
                            CompressionLevel = PngCompressionLevel.BestCompression
                        });
                    }
                }
                else if (ext == ".gif")
                    img.SaveAsGif(dst, new GifEncoder());
                else
                {
                    // JPEG: quality + 4:2:0 chroma subsampling for best compression
                    img.SaveAsJpeg(dst, new JpegEncoder {
                        Quality   = quality,
                        ColorType = JpegColorType.YCbCrRatio420
                    });
                }
            }
        }

        // ── Preview ───────────────────────────────────────────────────────────

        private void OnListSelectionChanged(object sender, EventArgs e)
        {
            if (listFiles.SelectedItems.Count == 0) return;
            var item = listFiles.SelectedItems[0].Tag as OptimizeItem;
            ShowPreview(item);
            btnReplace.Enabled = item != null && item.IsOptimized && !item.Replaced
                              && item.OptimizedSize < item.OriginalSize;
        }

        private void ShowPreview(OptimizeItem item)
        {
            ClearPicture(pictOrig);
            ClearPicture(pictOpt);
            if (item == null) return;

            if (File.Exists(item.OriginalPath))
            {
                pictOrig.Image   = LoadBitmapSafe(item.OriginalPath);
                lblOrigInfo.Text = FmtSize(item.OriginalSize);
            }

            if (item.IsOptimized && File.Exists(item.OptimizedPath))
            {
                pictOpt.Image = LoadBitmapSafe(item.OptimizedPath);
                long   saved = item.OriginalSize - item.OptimizedSize;
                double pct   = item.OriginalSize > 0
                    ? saved * 100.0 / item.OriginalSize : 0;
                lblOptInfo.Text = pct > 0
                    ? FmtSize(item.OptimizedSize) + "   saved " + FmtSize(saved)
                      + " (" + pct.ToString("F1") + "% smaller)"
                    : FmtSize(item.OptimizedSize) + "   (no improvement)";
            }
            else
            {
                lblOptInfo.Text = item.Error != null
                    ? "Error: " + item.Error
                    : "— run optimization first";
            }
        }

        private static SDBitmap LoadBitmapSafe(string path)
        {
            using (var ms = new MemoryStream(File.ReadAllBytes(path)))
            using (var tmp = SDImage.FromStream(ms))
            {
                var bmp = new SDBitmap(tmp.Width, tmp.Height, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(bmp))
                    g.DrawImage(tmp, 0, 0, tmp.Width, tmp.Height);
                return bmp;
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
                    string srcExt  = Path.GetExtension(item.OriginalPath).ToLower();
                    string dest    = item.OutExtension != srcExt
                        ? Path.ChangeExtension(item.OriginalPath, item.OutExtension)
                        : item.OriginalPath;

                    if (dest != item.OriginalPath && File.Exists(item.OriginalPath))
                        File.Delete(item.OriginalPath);

                    File.Copy(item.OptimizedPath, dest, overwrite: true);
                    item.OriginalPath = dest;
                }

                item.Replaced        = true;
                lvi.SubItems[3].Text = "Done";
                lvi.ForeColor        = System.Drawing.Color.FromArgb(0, 100, 180);
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
            int  total    = _items.Count;
            int  opt      = 0, replaced = 0;
            long savedBytes = 0;

            foreach (var it in _items)
            {
                if (it.IsOptimized) { opt++; savedBytes += it.OriginalSize - it.OptimizedSize; }
                if (it.Replaced)    replaced++;
            }

            if (opt == 0)
                lblStatus.Text = total + " file(s) ready — click Optimize All to start";
            else if (replaced > 0)
                lblStatus.Text = "Replaced " + replaced + "/" + total
                    + "  ·  Total saved " + FmtSize(savedBytes);
            else
                lblStatus.Text = "Optimized " + opt + "/" + total
                    + "  ·  Could save " + FmtSize(savedBytes)
                    + "  ·  Click Replace / Replace All to apply";
        }

        private static string FmtSize(long bytes)
        {
            if (bytes >= 1048576) return (bytes / 1048576.0).ToString("F2") + " MB";
            if (bytes >= 1024)    return (bytes / 1024.0).ToString("F0") + " KB";
            return bytes + " B";
        }
    }
}
