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
    }

}
