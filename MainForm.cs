using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using photo_editor;


namespace photo_editor
{
    public partial class MainForm : Form
    {
        public EditPhotoForm edit = new EditPhotoForm();
        private string imageListDirectory;
        private string photoRootDirectory;
        private List<FileInfo> photoFiles;
        private List<ListViewItem> photoDetails;
        private ImageList thumbnails;
        public MainForm()
        {
            InitializeComponent();
            //progressBarMainForm.Visible = false;

            photoRootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            PopulateImageList();
            PopulateTreeView();
        }

        private enum SizeUnits { KB = 1, MG };
        private static string ToSize(Int64 value, SizeUnits unit)
        {
            return (value / (double)Math.Pow(1024, (Int64)unit)).ToString("0.0") + (unit == SizeUnits.KB ? "KB" : "MB");
        }

        private async Task PopulateImageList()
        {
            photoFiles = new List<FileInfo>();
            photoDetails = new List<ListViewItem>();
            thumbnails = new ImageList();
            thumbnails.ImageSize = new Size(64, 64);

            int imageIndex = 0;

            // Start a background thread
            await Task.Run(() =>
            {
                DirectoryInfo homeDir = new DirectoryInfo(photoRootDirectory);
                try
                {
                    foreach (FileInfo file in homeDir.GetFiles("*.jpg"))
                    {
                        try
                        {
                            byte[] bytes = File.ReadAllBytes(file.FullName);
                            MemoryStream ms = new MemoryStream(bytes);
                            Image img = Image.FromStream(ms);

                            ListViewItem photoRow = new ListViewItem(file.Name, imageIndex);
                            photoRow.SubItems.Add(file.LastWriteTime.ToString());
                            // if it is less than 1048576 then show in kilobytes, otherwise in megabytes
                            photoRow.SubItems.Add(file.Length < 1048576 ? ToSize(file.Length, SizeUnits.KB) : ToSize(file.Length, SizeUnits.MG));
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
                }
                catch (UnauthorizedAccessException blockedPath)
                {
                    // ignore this path and move on
                }

                // Run code on UI thread

                Invoke((Action)(() =>
                {
                    listViewMain.Clear();

                    // Create columns (Width of -2 indicates auto-size)
                    listViewMain.Columns.Add("Name", 240, HorizontalAlignment.Left);
                    listViewMain.Columns.Add("Date", 160, HorizontalAlignment.Left);
                    listViewMain.Columns.Add("Size", 80, HorizontalAlignment.Right);

                    // Add the items to the list view
                    listViewMain.Items.AddRange(photoDetails.ToArray());
                    if (detailsToolStripMenuItem.Checked)
                    {
                        // Show default view
                        listViewMain.View = View.Details;

                        // Assign the ImageLists to the ListView
                        listViewMain.SmallImageList = thumbnails;
                    }
                    else if (smallToolStripMenuItem.Checked)
                    {
                        // Show default view
                        listViewMain.View = View.SmallIcon;

                        // Assign the ImageLists to the ListView
                        listViewMain.SmallImageList = thumbnails;

                    }
                    else if (largeToolStripMenuItem.Checked)
                    {
                        // Increase image size
                        thumbnails.ImageSize = new Size(128, 128);

                        // Show default view
                        listViewMain.View = View.LargeIcon;

                        // Assign the ImageLists to the ListView
                        listViewMain.LargeImageList = thumbnails;
                    }
                }));
            });
        }

        private void PopulateTreeView()
        {
            // From
            // https://stackoverflow.com/questions/6239544/populate-treeview-with-file-system-directory-structure
            treeViewMainForm.Nodes.Clear();
            Stack<TreeNode> stack = new Stack<TreeNode>();
            DirectoryInfo rootDirectory = new DirectoryInfo(photoRootDirectory);
            TreeNode node = new TreeNode(rootDirectory.Name) { Tag = rootDirectory };
            node.ExpandAll();
            stack.Push(node);

            while (stack.Count > 0)
            {
                TreeNode currentNode = stack.Pop();
                DirectoryInfo directoryInfo = (DirectoryInfo)currentNode.Tag;
                try
                {
                    foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                    {
                        TreeNode childDirectoryNode = new TreeNode(directory.Name) { Tag = directory };
                        currentNode.Nodes.Add(childDirectoryNode);
                        stack.Push(childDirectoryNode);
                    }
                } catch (UnauthorizedAccessException blockedPath)
                {
                    // ignore this path and move on
                }
            }

            treeViewMainForm.Nodes.Add(node);
        }

   

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog(this);
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listViewMain_ItemActivate(object sender, EventArgs e)
        {
            //String send = sender.ToString();
            //send = send.Remove(0, 72);
            //send = send.Remove(send.Length - 1, 1);
            String send = listViewMain.SelectedItems[0].Text;
            
            Console.WriteLine("current image selected?");
            Console.WriteLine(send);
            //EditPhotoForm edit = new EditPhotoForm();
           
            edit.pic = photoRootDirectory + "\\" + send;
            edit.ShowDialog();
        }

        private async void treeViewMainForm_AfterSelect(object sender, TreeViewEventArgs e)
        {
            photoRootDirectory = ((DirectoryInfo)treeViewMainForm.SelectedNode.Tag).FullName;
            progressBarMainForm.Visible = true;
            await PopulateImageList();
            progressBarMainForm.Visible = false;
        }

        private async void onToolStripMenuItemChange(object sender, EventArgs e)
        {
            // Uncheck all the menu items from viewToolStripMenuItem
            foreach (ToolStripMenuItem item in viewToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }

            // Check the one item that was clicked
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            menuItem.Checked = true;

            // Render image list
            progressBarMainForm.Visible = true;
            await PopulateImageList();
            progressBarMainForm.Visible = false;

        }

        private void locateOnDiskToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listViewMain.SelectedItems.Count == 0)
            {
                MessageBox.Show(this, "First select an image, then choose this option to see its location on disk.", "No image selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            } else
            {
                // Open explorer with the selected image
                Process.Start("explorer.exe", "/select," + photoRootDirectory + "\\" + listViewMain.SelectedItems[0].Text);
            }
        }

        private async void selectRootFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            if (folderBrowserDialog.SelectedPath != "")
            {
                photoRootDirectory = folderBrowserDialog.SelectedPath;
                progressBarMainForm.Visible = true;
                await PopulateImageList();
                PopulateTreeView();
                progressBarMainForm.Visible = false;

            }
        }
    }
}
