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
        // Function signature
        public delegate void CancelClickedEvent();

        // CancelClicked event requires a CancelClickedEvent callback
        public event CancelClickedEvent CancelClicked;
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
            // Notify EditPhotoForm (or any observer) that the cancel button was clicked
            CancelClicked();
        }
    }
}
