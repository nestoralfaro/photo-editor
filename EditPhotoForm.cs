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
        public EditPhotoForm()
        {
            InitializeComponent();
        }

        private void pictureBox1_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {

        }
    }

    public class Photo
    {

        public string test;
    }
}
