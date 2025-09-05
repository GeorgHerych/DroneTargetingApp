using System;
using System.Drawing;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Tesseract;
using System.Drawing.Imaging;
using System.IO;

namespace DroneTargetingApp
{
    //public partial class Form1 : Form
    //{
    //    private System.Windows.Forms.Timer captureTimer;
    //    private OpenCvSharp.Point? targetPos = null;
    //    private Rectangle captureArea;
    //    private float? height, windSpeed, windDirection;
    //    private PictureBox pictureBox;
    //    string inputHeight;
    //    string inputMass;
        
    //    public Form1()
    //    {
    //        //this.BackColor = Color.Magenta;
    //        //this.TransparencyKey = Color.Magenta;
    //        InitializeComponent();
    //        InitializeUI();
    //        InitializeCapture();
    //    }

    //    private void InitializeUI()
    //    {
    //        this.WindowState = FormWindowState.Maximized;
    //        this.Text = "Drone Targeting App";

    //        Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
    //        captureArea = new Rectangle(0, 0, screenBounds.Width - 100, screenBounds.Height - 100);

    //        pictureBox = new PictureBox
    //        {
    //            Location = new System.Drawing.Point(0, 0),
    //            Size = new System.Drawing.Size(screenBounds.Width - 100, screenBounds.Height - 100),
    //            SizeMode = PictureBoxSizeMode.StretchImage,
    //        };
    //        pictureBox.MouseClick += PictureBox_MouseClick!;
    //        this.Controls.Add(pictureBox);

    //    }

    //    private void InitializeCapture()
    //    {
    //        captureTimer = new System.Windows.Forms.Timer();
    //        captureTimer.Interval = 33;
    //        captureTimer.Tick += CaptureTimer_Tick!;
    //        captureTimer.Start();
    //    }

    //    private void CaptureTimer_Tick(object sender, EventArgs e)
    //    {
    //        try
    //        {
    //            using (Bitmap screenshot = CaptureScreen())
    //            {
    //                using (Mat img = OpenCvSharp.Extensions.BitmapConverter.ToMat(screenshot))
    //                {
    //                    ExtractTelemetry(img);

    //                    if (targetPos.HasValue)
    //                    {
    //                        Cv2.DrawMarker(img, new OpenCvSharp.Point(targetPos.Value.X, targetPos.Value.Y),
    //                            Scalar.Red, MarkerTypes.Cross, 20, 2);

    //                        var aimPoint = CalculateAimPoint(targetPos.Value, height, windSpeed, windDirection);
    //                        if (aimPoint.HasValue)
    //                        {
    //                            Cv2.DrawMarker(img, new OpenCvSharp.Point(aimPoint.Value.X, aimPoint.Value.Y),
    //                                Scalar.Green, MarkerTypes.Cross, 20, 2);
    //                        }
    //                    }

    //                    PictureBox pictureBox = this.Controls.OfType<PictureBox>().First();
    //                    pictureBox.Image?.Dispose();
    //                    pictureBox.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(img);
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Помилка: {ex.Message}");
    //        }
    //    }

    //    private Bitmap CaptureScreen()
    //    {
    //        Bitmap bmp = new Bitmap(captureArea.Width, captureArea.Height, PixelFormat.Format32bppArgb);
    //        using (Graphics g = Graphics.FromImage(bmp))
    //        {
    //            g.CopyFromScreen(captureArea.Left, captureArea.Top, 0, 0, captureArea.Size);
    //        }
    //        return bmp;
    //    }

    //    private void ExtractTelemetry(Mat img)
    //    {
    //        OpenCvSharp.Rect roi = new OpenCvSharp.Rect(0, 0, 300, 100);
    //        using (Mat telemetryRoi = new Mat(img, roi))
    //        {
    //            using (Mat gray = new Mat())
    //            {
    //                Cv2.CvtColor(telemetryRoi, gray, ColorConversionCodes.BGR2GRAY);
    //                Cv2.Threshold(gray, gray, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

    //                using (Bitmap roiBitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(gray))
    //                {
    //                    using (var stream = new MemoryStream())
    //                    {
    //                        roiBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
    //                        stream.Position = 0;
    //                        using (var pix = Pix.LoadFromMemory(stream.ToArray()))
    //                        {
    //                            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
    //                            {
    //                                using (var page = engine.Process(pix))
    //                                {
    //                                    string text = page.GetText();
    //                                    Console.WriteLine($"Розпізнаний текст: {text}");

    //                                    foreach (var line in text.Split('\n'))
    //                                    {
    //                                        try
    //                                        {
    //                                            if (line.Contains("Height"))
    //                                                height = float.Parse(line.Split(':')[1].Replace("m", "").Trim());
    //                                            if (line.Contains("Wind Speed"))
    //                                                windSpeed = float.Parse(line.Split(':')[1].Replace("m/s", "").Trim());
    //                                            if (line.Contains("Wind Direction"))
    //                                                windDirection = float.Parse(line.Split(':')[1].Replace("deg", "").Trim());
    //                                        }
    //                                        catch { }
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    private void PictureBox_MouseClick(object sender, MouseEventArgs e)
    //    {
    //        PictureBox pictureBox = sender as PictureBox;
    //        float scaleX = (float)captureArea.Width / pictureBox.Width;
    //        float scaleY = (float)captureArea.Height / pictureBox.Height;
    //        targetPos = new OpenCvSharp.Point((int)(e.X * scaleX), (int)(e.Y * scaleY));
    //        Console.WriteLine($"Ціль: ({targetPos.Value.X}, {targetPos.Value.Y})");
    //    }

    //    private OpenCvSharp.Point? CalculateAimPoint(OpenCvSharp.Point target, float? height, float? windSpeed, float? windDirection)
    //    {
    //        if (!height.HasValue || !windSpeed.HasValue || !windDirection.HasValue)
    //            return null;

    //        const float g = 9.81f;
    //        float t = (float)Math.Sqrt(2 * height.Value / g);
    //        float windRad = (float)(windDirection.Value * Math.PI / 180);

    //        float offsetX = windSpeed.Value * t * (float)Math.Cos(windRad);
    //        float offsetY = windSpeed.Value * t * (float)Math.Sin(windRad);

    //        const float pixelsPerMeter = 10f;
    //        int aimX = target.X - (int)(offsetX * pixelsPerMeter);
    //        int aimY = target.Y - (int)(offsetY * pixelsPerMeter);

    //        return new OpenCvSharp.Point(aimX, aimY);
    //    }

    //    private void ResetButton_Click(object sender, EventArgs e)
    //    {
    //        targetPos = null;
    //    }

    //    private void StopButton_Click(object sender, EventArgs e)
    //    {
    //        captureTimer.Stop();
    //    }

    //    protected override void OnFormClosing(FormClosingEventArgs e)
    //    {
    //        captureTimer?.Stop();
    //        base.OnFormClosing(e);
    //    }

    //    private void MassBox_TextChanged(object sender, EventArgs e)
    //    {
    //        inputMass = textBox1.Text.Trim();
    //        if (float.TryParse(inputMass, out float value) && value >= 0)
    //        {
    //            height = value;
    //            Console.WriteLine($"Маса встановлена: {height} грамм");
    //        }
    //        else
    //        {
    //            MessageBox.Show("Будь ласка, введіть коректне значення маси.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //        }
    //    }

    //    private void InputMass_Click(object sender, EventArgs e)
    //    {
    //        if (string.IsNullOrEmpty(inputMass))
    //        {
    //            MessageBox.Show("Будь ласка, введіть масу дрона.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //            return;
    //        }

    //    }

    //    private void HeightAdjustment_Click(object sender, EventArgs e)
    //    {
    //        if(string.IsNullOrEmpty(inputHeight))
    //        {
    //            MessageBox.Show("Будь ласка, введіть висоту польоту для корегування.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //            return;
    //        }
    //    }

    //    private void HeightBox_TextChanged(object sender, EventArgs e)
    //    {
    //        inputHeight = textBox2.Text.Trim();
    //        if (float.TryParse(inputHeight, out float value) && value >= 0)
    //        {
    //            height = value;
    //            Console.WriteLine($"Висота встановлена: {height} м");
    //        }
    //        else
    //        {
    //            MessageBox.Show("Будь ласка, введіть коректне значення висоти.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //        }
    //    }
    //}
}