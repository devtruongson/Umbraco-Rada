using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class PreviewForm : Form
    {
        private PictureBox pictureBox;
        private Label lblInfo;
        private Panel panelTop;
        private TrackBar zoomTrack;
        private Label lblZoom;
        private Image originalImage;

        public PreviewForm(string filePath)
        {
            BuildUI();
            LoadImage(filePath);
        }

        private void BuildUI()
        {
            this.Text = "Preview";
            this.Size = new Size(900, 700);
            this.MinimumSize = new Size(600, 500);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.StartPosition = FormStartPosition.CenterParent;

            // Top panel
            panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                BackColor = Color.FromArgb(45, 45, 48),
                Padding = new Padding(10, 0, 10, 0)
            };

            lblInfo = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Left,
                Width = 560,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblZoom = new Label
            {
                AutoSize = false,
                Width = 55,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                TextAlign = ContentAlignment.MiddleRight,
                Text = "100%",
                Dock = DockStyle.Right
            };

            zoomTrack = new TrackBar
            {
                Minimum = 10,
                Maximum = 300,
                Value = 100,
                TickFrequency = 50,
                Width = 200,
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(45, 45, 48)
            };
            zoomTrack.ValueChanged += (s, e) =>
            {
                lblZoom.Text = zoomTrack.Value + "%";
                ApplyZoom();
            };

            panelTop.Controls.Add(lblInfo);
            panelTop.Controls.Add(lblZoom);
            panelTop.Controls.Add(zoomTrack);

            // ScrollablePanel for the image
            var scroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(20, 20, 20)
            };

            pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Location = new Point(0, 0)
            };

            scroll.Controls.Add(pictureBox);

            this.Controls.Add(scroll);
            this.Controls.Add(panelTop);
            this.FormClosed += (s, e) => originalImage?.Dispose();
        }

        private void LoadImage(string filePath)
        {
            var info = new FileInfo(filePath);
            originalImage = Image.FromFile(filePath);

            lblInfo.Text = $"  {info.Name}   |   {originalImage.Width} × {originalImage.Height} px   |   {info.Length / 1024.0:F1} KB";
            this.Text = "Preview — " + info.Name;

            ApplyZoom();
        }

        private void ApplyZoom()
        {
            if (originalImage == null) return;
            double scale = zoomTrack.Value / 100.0;
            int w = (int)(originalImage.Width * scale);
            int h = (int)(originalImage.Height * scale);

            pictureBox.Size = new Size(Math.Max(w, 1), Math.Max(h, 1));
            pictureBox.Image = originalImage;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewForm));
            this.SuspendLayout();
            // 
            // PreviewForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PreviewForm";
            this.ResumeLayout(false);

        }
    }
}
