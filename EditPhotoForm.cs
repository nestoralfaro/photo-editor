using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace photo_editor
{

    public partial class EditPhotoForm : Form
    {
        public string pic;
        public ColorDialog colorDialog1 = new ColorDialog();
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

        private void button1_Click(object sender, EventArgs e)
        {
            // Citation Start: Code below from https://stackoverflow.com/questions/33024881/invert-image-faster-in-c-sharp
            Bitmap photo = new Bitmap(pictureBox1.Image);                                       
            for(int y = 0; (y <= (photo.Height - 1)); y++) {                                    
                for (int x = 0; (x <= (photo.Width - 1)); x++)                                      
                {                                                                                      
                    Color inv = photo.GetPixel(x, y);                                           
                    inv = System.Drawing.Color.FromArgb(255, (255 - inv.R), (255 - inv.G), (255 - inv.B));     
                    photo.SetPixel(x, y, inv);                                                  
                }
            }
            pictureBox1.Image = photo;
            //Citation End
        }

       

        private void Color_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Color.BackColor = colorDialog1.Color;
            }
        }
    }

}
