using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace photo_editor
{
    public partial class MainForm : Form
    {
        private string photoRootDirectory;
        private List<FileInfo> photoFiles;
        private List<ListViewItem> photoDetails;
        private ImageList thumbnails;
        public MainForm()
        {
            InitializeComponent();

            photoRootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            Console.WriteLine("this is the photorootdire");
            Console.WriteLine(photoRootDirectory);

            PopulateImageList();
            PopulateTreeView();

        }

        private void PopulateImageList()
        {
            photoFiles = new List<FileInfo>();

            photoDetails = new List<ListViewItem>();
            thumbnails = new ImageList();
            thumbnails.ImageSize = new Size(32, 32);

            int imageIndex = 0;

            // Create columns (Width of -2 indicates auto-size)
            listViewMain.Columns.Add("Name", -2, HorizontalAlignment.Left);
            listViewMain.Columns.Add("Date", -2, HorizontalAlignment.Left);
            listViewMain.Columns.Add("Size", 40, HorizontalAlignment.Right);

            DirectoryInfo homeDir = new DirectoryInfo(photoRootDirectory);
            foreach (FileInfo file in homeDir.GetFiles("*.jpg"))
            //foreach (FileInfo file in homeDir.EnumerateFiles("*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".jpg") || s.EndsWith(".jpg")))
            {
                try
                {
                    byte[] bytes = File.ReadAllBytes(file.FullName);
                    MemoryStream ms = new MemoryStream(bytes);
                    Image img = Image.FromStream(ms);

                    ListViewItem photoRow = new ListViewItem(file.Name, imageIndex);
                    photoRow.SubItems.Add(file.LastWriteTime.ToString());
                    photoRow.SubItems.Add(file.Length.ToString());
                    thumbnails.Images.Add(img);
                    photoDetails.Add(photoRow);
                    photoFiles.Add(file);
                }
                catch
                {
                    Console.WriteLine("This is not an image file");
                }

                //photoFiles.Add(file);
                ++imageIndex;
            }

            // Add the items to the list view
            listViewMain.Items.AddRange(photoDetails.ToArray());

            // Show default view
            listViewMain.View = View.Details;

            // Assign the ImageLists to the ListView
            listViewMain.SmallImageList = thumbnails;
        }

        private void PopulateTreeView()
        {
            // From
            // https://stackoverflow.com/questions/6239544/populate-treeview-with-file-system-directory-structure
            treeViewMainForm.Nodes.Clear();
            Stack<TreeNode> stack = new Stack<TreeNode>();
            DirectoryInfo rootDirectory = new DirectoryInfo(photoRootDirectory);
            TreeNode node = new TreeNode(rootDirectory.Name) { Tag = rootDirectory };
            stack.Push(node);

            while (stack.Count > 0)
            {
                TreeNode currentNode = stack.Pop();
                DirectoryInfo directoryInfo = (DirectoryInfo)currentNode.Tag;
                foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                {
                    TreeNode childDirectoryNode = new TreeNode(directory.Name) { Tag = directory };
                    currentNode.Nodes.Add(childDirectoryNode);
                    stack.Push(childDirectoryNode);
                }
            }

            treeViewMainForm.Nodes.Add(node);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
