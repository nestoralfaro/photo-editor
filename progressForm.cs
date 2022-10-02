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
    public partial class progressForm : Form
    {
        public progressForm()
        {
            InitializeComponent();
        }

        

        public void IncrementProgress(int i)
        {
            progressBar1.Increment(i);
        }

        private void Cancel_Click(object sender, EventArgs e)
        {

        }
    }
}
