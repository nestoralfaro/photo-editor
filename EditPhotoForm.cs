﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace photo_editor
{
    public partial class EditPhotoForm : Form
    {
        public string pic;
        public int count = 0;
        private Bitmap photo;
        public ColorDialog colorDialog1 = new ColorDialog();
        public int progress = 0;
        private CancellationTokenSource cancellationTokenSource;
        private bool isOperationBrightness;
        private int brightness;
        private int brightness2;

        String filePath;
        public EditPhotoForm(String photoName)
        {
            filePath = photoName;
            InitializeComponent();
        }

        // cancel clicked callback
        private void Progress_CancelClicked()
        {
            cancellationTokenSource.Cancel();
            if (isOperationBrightness)
            {
                brightnessTrackBar.Value = brightness2;
                
            }
        }

        private void EditPhotoForm_Load(object sender, EventArgs e)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(pic);
            MemoryStream ms = new MemoryStream(bytes);
            pictureBox1.Image = Image.FromStream(ms);
            // pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }
        private async void Color_Click(object sender, EventArgs e)
        {
            // Citation: Code below from https://github.com/fmccown/SimplePhotoEditor
            // Author: Frank McCown
            photo = new Bitmap(pictureBox1.Image);
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            int Total = (photo.Height * photo.Width) / 100;
            int percent = 0;
            progressForm progress = new progressForm();
            progress.CancelClicked += Progress_CancelClicked;
            
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                isOperationBrightness = false;
                progress.Show();

                await Task.Run(() =>
                {
                    for (int y = 0; y < photo.Height; y++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        for (int x = 0; x < photo.Width; x++)
                        {
                            percent++;
                            var color = photo.GetPixel(x, y);
                            int ave = (color.R + color.G + color.B) / 3;
                            double per = ave / 255.0;
                            int newRed = Convert.ToInt32(colorDialog1.Color.R * per);
                            int newGreen = Convert.ToInt32(colorDialog1.Color.G * per);
                            int newBlue = Convert.ToInt32(colorDialog1.Color.B * per);
                            var newColor = System.Drawing.Color.FromArgb(newRed, newGreen, newBlue);
                            photo.SetPixel(x, y, newColor);
                            if(percent % Total == 0)
                            {
                                Invoke((Action)(() =>
                                {
                                    progress.IncrementProgress(1);
                                }));
                            }
                        }
                    }
                });
                if (!token.IsCancellationRequested)
                {
                    pictureBox1.Image = photo;
                    Save.Enabled = true;
                }
                progress.Close();
            }
        }

        private async void Invert_Click(object sender, EventArgs e)
        {
            cancellationTokenSource = new CancellationTokenSource();
            // Citation: Code below from https://github.com/fmccown/SimplePhotoEditor
            // Author: Frank McCown
            progressForm progress = new progressForm();
            progress.CancelClicked += Progress_CancelClicked;
            photo = new Bitmap(pictureBox1.Image);

            var count = 0;
            var token = cancellationTokenSource.Token;

            double onePercent = (photo.Height * photo.Width) / 100;    // Calulates 1 percent of the number of pixels in the photo
            progress.Show();    
            this.Enabled = false;                                       // Disable EditPhotoForm while performing operation
            await Task.Run(() =>    // Start a background thread
            {
                isOperationBrightness = false;
                for (int y = 0; y < photo.Height; y++)
                {
                    for (int x = 0; x < photo.Width; x++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }
                        count++;
                        var color = photo.GetPixel(x, y);
                        int newRed = Math.Abs(color.R - 255);
                        int newGreen = Math.Abs(color.G - 255);
                        int newBlue = Math.Abs(color.B - 255);
                        Color tempColor = System.Drawing.Color.FromArgb(newRed, newGreen, newBlue);
                        photo.SetPixel(x, y, tempColor);
                        if (count % onePercent == 0)
                        {
                            Invoke((Action)(() =>
                            {
                                progress.IncrementProgress(1);
                            }));
                        }
                    }
                }
            }, token);
            this.Enabled = true;            // Enable EditPhotoForm 
            progress.Hide();                // Hide progress bar form
            if (!token.IsCancellationRequested)
            {
                pictureBox1.Image = photo;
                Save.Enabled = true;
            }
        }

        private async void brightnessTrackBar_MouseUp(object sender, MouseEventArgs e)
        {

            cancellationTokenSource = new CancellationTokenSource();
            progressForm pf = new progressForm();
            // register listener for when the cancel button in progressForm was Clicked and execute Pf_CancelThread callback
            pf.CancelClicked += Progress_CancelClicked;
            photo = new Bitmap(pictureBox1.Image);
            var token = cancellationTokenSource.Token;
            int onePercent = (photo.Height * photo.Width) / 100;
            int counter = 0;
            // Start asynchronous task
            pf.Show();

            await Task.Run(() =>
            {
                // Citation: Code below from https://github.com/fmccown/SimplePhotoEditor
                // Author: Frank McCown
                isOperationBrightness = true;
                // show progress form bar
                Invoke((Action)(() =>
                {
                    this.Enabled = false;
                    brightness = brightnessTrackBar.Value;
                    
                }));

                    int amount = Convert.ToInt32(2 * (50 - brightness) * 0.01 * 255);
                    for (int y = 0; y < photo.Height; y++)
                    {
                        for (int x = 0; x < photo.Width; x++)
                        {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }
                            counter++;
                            var color = photo.GetPixel(x, y);
                            int newRed = Math.Max(0, Math.Min(color.R - amount, 255));
                            int newGreen = Math.Max(0, Math.Min(color.G - amount, 255));
                            int newBlue = Math.Max(0, Math.Min(color.B - amount, 255));
                            var newColor = System.Drawing.Color.FromArgb(newRed, newGreen, newBlue);
                            photo.SetPixel(x, y, newColor);
                            if (counter % onePercent == 0)
                            {
                                Invoke((Action)(() =>
                                {
                                    pf.IncrementProgress(1);
                                }));
                            }
                        }
                    }

                    // close progress form bar
                    Invoke((Action)(() =>
                    {
                        this.Enabled = true;

                    }));
                
            });

            if (!token.IsCancellationRequested)
            {
                pictureBox1.Image = photo;
                Save.Enabled = true;
                brightness2 = brightness;
            }
            pf.Close();

        }

        private void Save_Click(object sender, EventArgs e)
        {
            // if changes have been made
            if (photo != null)
            {
                photo.Save(filePath, ImageFormat.Jpeg); 
            }
        }

        private void SaveAs_Click(object sender, EventArgs e)   // Allows user to specify file name
        {
            SaveAs.Image = photo;
            SaveFileDialog Save = new SaveFileDialog();
            Save.Filter = "Jpeg Image|*.jpg";   // Specifies types of files user is allowed to choose
            Save.Title = "Save As...";
            Save.ShowDialog();

            if(Save.FileName != ""){
                System.IO.FileStream fs = (System.IO.FileStream)Save.OpenFile();

                if(Save.FilterIndex == 1)
                {
                    this.SaveAs.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                    SaveAs.Image = null;
                }
                fs.Close();
            }
    }
}
}