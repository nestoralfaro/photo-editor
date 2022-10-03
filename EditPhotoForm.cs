using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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

        // cancel clicked callback
        private void Progress_CancelClicked()
        {
            cancellationTokenSource.Cancel();
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
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            int Total = (photo.Height * photo.Width) / 100;
            int percent = 0;
            progressForm progress = new progressForm();
            progress.CancelClicked += Progress_CancelClicked;
            Console.WriteLine(Total);
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                progress.Show();

                await Task.Run(() =>
                {
                    for (int y = 0; y < photo.Height; y++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        for (int x = 0; x < Photo.Width; x++)
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
                                    progress.IncrementProgress(10);
                                }));
                            }
                        }
                    }
                });
                
                if (!token.IsCancellationRequested)pictureBox1.Image = Photo;

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
            if (!token.IsCancellationRequested) pictureBox1.Image = photo;

            
        }

        private async void brightnessTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            progressForm pf = new progressForm();
            // register listener for when the cancel button in progressForm was Clicked and execute Pf_CancelThread callback
            pf.CancelClicked += Progress_CancelClicked;
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            // Start asynchronous task
            await Task.Run(() =>
            {
                // show progress form bar
                Invoke((Action)(() =>
                {
                    pf.Show();
                }));

                // change image brightness (call ChangeBrightness()?)
                while (!token.IsCancellationRequested)
                {
                    /*
                     * If the cancellation has not been requested
                     * (the cancel button has not been clicked)
                     * then do the work necessary to change background color.
                     * Notice that this part does not need to be in a while loop
                     * it is just an illustration so that you can only do the
                     * work while the token has not been requested to cancel
                     */
                }

                // close progress form bar
                Invoke((Action)(() =>
                {
                    pf.Close();
                }));
            });
        }
    }
    }

}
