﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ScreenCaptureApp
{
    public partial class ScreenCaptureForm : Form
    {
        private PictureBox pictureBox;
        private System.Windows.Forms.Timer captureTimer;
        private Button startButton;
        private Button stopButton;
        private Button saveButton;
        private Rectangle captureArea;

        public ScreenCaptureForm()
        {
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            InitializeComponents();
        }

        private void InitializeComponents()
        {

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Screen Capture App";
            this.KeyPreview = true;

            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            captureArea = new Rectangle(0, 0, screenBounds.Width, screenBounds.Height);

            pictureBox = new PictureBox
            {
                Location = new Point(0, 0),
                Size = new Size(screenBounds.Width, screenBounds.Height - 100),
                SizeMode = PictureBoxSizeMode.StretchImage,
            };
            this.Controls.Add(pictureBox);

            startButton = new Button
            {
                Text = "Старт",
                Location = new Point(10, screenBounds.Height - 90),
                Size = new Size(100, 30),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            startButton.Click += StartButton_Click;
            this.Controls.Add(startButton);

            stopButton = new Button
            {
                Text = "Стоп",
                Location = new Point(120, screenBounds.Height - 90),
                Size = new Size(100, 30),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            stopButton.Click += StopButton_Click;
            this.Controls.Add(stopButton);

            saveButton = new Button
            {
                Text = "Зберегти",
                Location = new Point(230, screenBounds.Height - 90),
                Size = new Size(100, 30),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            captureTimer = new System.Windows.Forms.Timer();
            captureTimer.Interval = 33; // ~30 FPS
            captureTimer.Tick += CaptureTimer_Tick;
        }

        private void CaptureTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                using (Bitmap screenshot = CaptureScreen())
                {
                    pictureBox.Image?.Dispose();
                    pictureBox.Image = (Bitmap)screenshot.Clone();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        private Bitmap CaptureScreen()
        {
            Bitmap bmp = new Bitmap(captureArea.Width, captureArea.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(captureArea.Left, captureArea.Top, 0, 0, captureArea.Size);
            }
            return bmp;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            captureTimer.Start();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            captureTimer.Stop();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null) return;

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PNG Image|*.png";
                saveDialog.Title = "Зберегти скріншот";
                saveDialog.FileName = $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox.Image.Save(saveDialog.FileName, ImageFormat.Png);
                    MessageBox.Show("Скріншот збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            base.OnKeyDown(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            captureTimer?.Stop();
            pictureBox.Image?.Dispose();
            base.OnFormClosing(e);
        }
    }
}