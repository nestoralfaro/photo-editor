using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace photo_editor
{

    public partial class EditPhotoForm : Form
    {
        public string pic;
        public int count = 0;
        public ColorDialog colorDialog1 = new ColorDialog();
        public int progress = 0;
        private CancellationTokenSource cancellationTokenSource;
        

        public EditPhotoForm()
        {
            InitializeComponent();
        }

        private void EditPhotoForm_Load(object sender, EventArgs e)
        {
            Console.WriteLine("pic edit photo form");
            Console.WriteLine(pic);
            pictureBox1.Image = Image.FromFile(pic);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

       

        private async void Color_Click(object sender, EventArgs e)
        {
            // Citation: Code below from https://github.com/fmccown/SimplePhotoEditor
            // Author: Frank McCown
            var Photo = new Bitmap(pictureBox1.Image);
           // var token = cancellationTokenSource.Token;

            int Total = (Photo.Height * Photo.Width) / 100;
            int percent = 0;
            progressForm progress = new progressForm();
            progress.Show();
            Console.WriteLine(Total);
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                await Task.Run(() =>
                {
                    for (int y = 0; y < Photo.Height; y++)
                    {
                        for (int x = 0; x < Photo.Width; x++)
                        {
                            percent++;
                            var color = Photo.GetPixel(x, y);
                            int ave = (color.R + color.G + color.B) / 3;
                            double per = ave / 255.0;
                            int newRed = Convert.ToInt32(colorDialog1.Color.R * per);
                            int newGreen = Convert.ToInt32(colorDialog1.Color.G * per);
                            int newBlue = Convert.ToInt32(colorDialog1.Color.B * per);
                            var newColor = System.Drawing.Color.FromArgb(newRed, newGreen, newBlue);
                            Photo.SetPixel(x, y, newColor);
                            if(percent % Total == 0)
                            {
                                Invoke((Action)(() =>
                                {
                                    progress.IncrementProgress(10);
                                }));
                            }
                        }
                    }
                });
                
                pictureBox1.Image = Photo;

            }
        }
       
        private async void Invert_Click(object sender, EventArgs e)
        {
            cancellationTokenSource = new CancellationTokenSource();
            // Citation: Code below from https://github.com/fmccown/SimplePhotoEditor
            // Author: Frank McCown
            progressForm progress = new progressForm();
            Bitmap photo = new Bitmap(pictureBox1.Image);
            var count = 0;
            var token = cancellationTokenSource.Token;

            double percent = (photo.Height * photo.Width) / 100;
            progress.Show();
            this.Enabled = false;
            await Task.Run(() =>    // Start a background thread
            {
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
                        if (count % percent == 0)
                        {
                            Invoke((Action)(() =>
                            {
                                progress.IncrementProgress(10);
                            }));
                        }
                    }
                }
            }, token);

            this.Enabled = true;
            progress.Hide();
            pictureBox1.Image = photo;

            
        }
    }

}
