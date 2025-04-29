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

        // 2) Form1_Load: 폼 로드 시 논리 드라이브(예: C:\, D:\)를 트리뷰에 추가
        private void Form1_Load(object sender, EventArgs e)
        {// 시스템의 모든 논리 드라이브 문자열 배열
            string[] Drv_list;
            TreeNode root;

            Drv_list = Environment.GetLogicalDrives();

            foreach (string Drv in Drv_list)
            {// 트리뷰 최상위에 드라이브 노드 추가
                root = trvDir.Nodes.Add(Drv); 
                root.ImageIndex = 2;

                if (trvDir.SelectedNode == null)
                    trvDir.SelectedNode = null;
                root.SelectedImageIndex = root.ImageIndex;
                // 자식 노드를 미리 하나 추가해 두어 “+” 표시가 나타나도록 함
                root.Nodes.Add("");
            }
        }

        // 3) setPlus: 특정 노드에 하위 폴더가 있으면 “+” 더미 노드 추가
        public void setPlus(TreeNode node)
        {
            string path;
            DirectoryInfo dir;
            DirectoryInfo[] di;

            try
            {
                path = node.FullPath; // 노드의 전체 경로
                dir = new DirectoryInfo(path);
                di = dir.GetDirectories(); // 하위 디렉터리 목록
                if (di.Length > 0) // 하위 디렉터리가 하나라도 있으면 더미로 빈 노드 추가
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

        
        /// trvDir.Nodes 전체를 재귀 탐색해서 FullPath가 path와 일치하는 TreeNode를 반환      
        private TreeNode FindNodeByPath(TreeNodeCollection nodes, string path)
        {
            foreach (TreeNode node in nodes)
            {
                // 트리뷰 노드의 FullPath 속성과 비교
                if (string.Equals(node.FullPath, path, StringComparison.InvariantCultureIgnoreCase))
                    return node;

                // 자식이 “+”만 달려 있는 더미 노드 상태라면 실제 하위 폴더 채우기
                if (node.Nodes.Count == 1 && node.Nodes[0].Text == "")
                    setPlus(node);

                // 재귀 탐색
                TreeNode found = FindNodeByPath(node.Nodes, path);
                if (found != null)
                    return found;
            }
            return null;
        }

        public void OpenItem(ListViewItem item)
        {
            // Tag에 저장해 둔 풀 경로 꺼내기
            string fullPath = item.Tag as string;
            if (Directory.Exists(fullPath))
            {
                // 트리뷰에서 노드 찾아 선택·확장
                TreeNode target = FindNodeByPath(trvDir.Nodes, fullPath);
                if (target != null)
                {
                    trvDir.SelectedNode = target;
                    target.Expand();
                    trvDir.Focus();
                }
                else
                {
                    // 없으면 부모 노드 찾아서 새로 추가
                    string parent = Path.GetDirectoryName(fullPath);
                    TreeNode pnode = FindNodeByPath(trvDir.Nodes, parent);
                    if (pnode != null)
                    {
                        TreeNode node = pnode.Nodes.Add(Path.GetFileName(fullPath));
                        setPlus(node);
                        trvDir.SelectedNode = node;
                        node.Expand();
                        trvDir.Focus();
                    }
                }
                // txtPath도 실제 선택된 폴더로 갱신
                txtPath.Text = fullPath;
            }
            else if (File.Exists(fullPath))
            {
                // 파일은 바로 실행
                Process.Start(fullPath);
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
                e.Node.Nodes.Clear(); // 기존 더미 노드 지움
                path = e.Node.FullPath;
                dir = new DirectoryInfo(path);
                di = dir.GetDirectories();

                foreach(DirectoryInfo dirs in di)
                {
                    node = e.Node.Nodes.Add(dirs.Name);
                    setPlus(node); // 하위에 또 “+” 필요하면 추가
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //해당 폴더의 파일/폴더 목록을 리스트뷰에 표시
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
                    item.Tag = tdls.FullName;
                }

                fiArray = di.GetFiles();
                foreach(FileInfo fls in fiArray)
                {
                    item = lvwFiles.Items.Add(fls.Name);
                    item.SubItems.Add(fls.Length.ToString());
                    item.SubItems.Add(fls.LastWriteTime.ToString());
                    item.ImageIndex = 1;
                    item.Tag = fls.FullName;
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string input = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("검색어를 입력하세요.");
                return;
            }

            string rootPath;
            string pattern;

            if (Path.IsPathRooted(input) && Directory.Exists(input))
            {
                // 절대경로 검색
                rootPath = input;
                pattern = "";
            }
            else
            {
                // 현재 선택된 경로에서 이름 검색
                rootPath = txtPath.Text;
                pattern = input;
            }

            // * 여기서 txtPath를 갱신 *
            txtPath.Text = rootPath;

            lvwFiles.Items.Clear();
            SearchDirectory(rootPath, pattern);
        }


        private void SearchDirectory(string path, string pattern)
        {
            try
            {
                var dirInfo = new DirectoryInfo(path);
                // 와일드카드
                string searchPattern = string.IsNullOrEmpty(pattern)
                    ? "*"
                    : "*" + pattern + "*";

                // 1) 바로 이 폴더 안의 매치된 서브폴더
                foreach (var folder in dirInfo.GetDirectories(searchPattern))
                {
                    var item = new ListViewItem(folder.Name);
                    item.SubItems.Add("");
                    item.SubItems.Add(folder.LastWriteTime.ToString());
                    item.ImageIndex = 0;     // 폴더 아이콘
                    item.Tag = folder.FullName; // 여기에 풀 경로 저장
                    lvwFiles.Items.Add(item);
                }

                // 2) 바로 이 폴더 안의 매치된 파일
                foreach (var file in dirInfo.GetFiles(searchPattern))
                {
                    var item = new ListViewItem(file.Name);
                    item.SubItems.Add(file.Length.ToString());
                    item.SubItems.Add(file.LastWriteTime.ToString());
                    item.ImageIndex = 1;     // 파일 아이콘
                    item.Tag = file.FullName;  //  여기에 풀 경로 저장
                    lvwFiles.Items.Add(item);
                }

                // 3) 재귀: 하위 모든 폴더 탐색
                foreach (var sub in dirInfo.GetDirectories())
                {
                    SearchDirectory(sub.FullName, pattern);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // 무시
            }
            catch (Exception ex)
            {
                MessageBox.Show("검색 중 오류: " + ex.Message);
            }
        }

    }
}
