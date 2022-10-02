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
            Console.WriteLine(pic);
            pictureBox1.Image = Image.FromFile(pic);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

       

        private void Color_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Color.BackColor = colorDialog1.Color;
            }
        }

        private async void Invert_Click(object sender, EventArgs e)
        {
            cancellationTokenSource = new CancellationTokenSource();
            // Citation: Code below from https://github.com/fmccown/SimplePhotoEditor
            // Author: Frank McCown
            progressForm progress = new progressForm();
            Bitmap photo = new Bitmap(pictureBox1.Image);
            var percent = 0;
            
            var token = cancellationTokenSource.Token;
            progress.Show();
            this.Enabled = false;
            await Task.Run(() =>    // Start a background thread
            {
                for (int y = 0; y < photo.Height; y++)
                {
                    for (int x = 0; x < photo.Width; x++)
                    {
                        if (token.IsCancellationRequested)
                            break;
                        percent++;
                        var color = photo.GetPixel(x, y);
                        int newRed = Math.Abs(color.R - 255);
                        int newGreen = Math.Abs(color.G - 255);
                        int newBlue = Math.Abs(color.B - 255);
                        Color tempColor = System.Drawing.Color.FromArgb(newRed, newGreen, newBlue);
                        photo.SetPixel(x, y, tempColor);
                        if (percent % 3555 == 0)
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
