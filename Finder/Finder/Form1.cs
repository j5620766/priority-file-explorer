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
using System.Diagnostics;

namespace Finder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.Form1_Load);
            this.trvDir.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.trvDir_BeforeExpand);
            this.trvDir.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.trvDir_BeforeSelect);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] Drv_list;
            TreeNode root;

            Drv_list = Environment.GetLogicalDrives();

            foreach (string Drv in Drv_list)
            {
                root = trvDir.Nodes.Add(Drv);
                root.ImageIndex = 2;

                if (trvDir.SelectedNode == null)
                    trvDir.SelectedNode = null;
                root.SelectedImageIndex = root.ImageIndex;
                root.Nodes.Add("");
            }
        }

        public void setPlus(TreeNode node)
        {
            string path;
            DirectoryInfo dir;
            DirectoryInfo[] di;

            try
            {
                path = node.FullPath;
                dir = new DirectoryInfo(path);
                di = dir.GetDirectories();
                if (di.Length > 0)
                    node.Nodes.Add("");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void OpenFiles()
        {
            ListView.SelectedListViewItemCollection siList;
            siList = lvwFiles.SelectedItems;

            foreach (ListViewItem item in siList)
            {
                OpenItem(item);
            }
        }

        public void OpenItem(ListViewItem item)
        {
            TreeNode node;
            TreeNode child;

            if(item.Tag.ToString() =="D")
            {
                node = trvDir.SelectedNode;
                node.Expand();

                child = node.FirstNode;

                while (child != null)
                {
                    if (child.Text == item.Text)
                    {
                        trvDir.SelectedNode = child;
                        trvDir.Focus();
                        break;
                    }
                    child = child.NextNode;
                }
            }
            else
            {
                Process.Start(txtPath.Text + "WW" + item.Text);
            }
          
        }

        private void trvDir_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            string path;
            DirectoryInfo dir;
            DirectoryInfo[] di;
            TreeNode node;

            try
            {
                e.Node.Nodes.Clear();
                path = e.Node.FullPath;
                dir = new DirectoryInfo(path);
                di = dir.GetDirectories();

                foreach(DirectoryInfo dirs in di)
                {
                    node = e.Node.Nodes.Add(dirs.Name);
                    setPlus(node);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void trvDir_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            DirectoryInfo di;
            DirectoryInfo[] diarray;
            ListViewItem item;
            FileInfo[] fiArray;

            try
            {
                di = new DirectoryInfo(e.Node.FullPath);
                txtPath.Text = e.Node.FullPath.Substring(0, 2) + e.Node.FullPath.Substring(3);
                lvwFiles.Items.Clear();

                diarray = di.GetDirectories();
                foreach(DirectoryInfo tdls in diarray)
                {
                    item = lvwFiles.Items.Add(tdls.Name);
                    item.SubItems.Add("");
                    item.SubItems.Add(tdls.LastWriteTime.ToString());
                    item.ImageIndex = 0;
                    item.Tag = "D";
                }

                fiArray = di.GetFiles();
                foreach(FileInfo fls in fiArray)
                {
                    item = lvwFiles.Items.Add(fls.Name);
                    item.SubItems.Add(fls.Length.ToString());
                    item.SubItems.Add(fls.LastWriteTime.ToString());
                    item.ImageIndex = 1;
                    item.Tag = "F";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lvwFiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lvwFiles_DoubleClick(object sender, EventArgs e)
        {
            OpenFiles();
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            OpenFiles();
        }

        private void mnuDetail_Click(object sender, EventArgs e)
        {
            mnuDetail.Checked=true;
            lvwFiles.View = View.Details;
        }

        private void mnuList_Click(object sender, EventArgs e)
        {
            mnuList.Checked=true;
            lvwFiles.View = View.List;
        }

        private void mnuSmall_Click(object sender, EventArgs e)
        {
            mnuSmall.Checked=true;
            lvwFiles.View =View.SmallIcon;
        }

        private void mnuLarge_Click(object sender, EventArgs e)
        {
            mnuLarge.Checked=true;
            lvwFiles.View=View.LargeIcon;
        }
    }
}
