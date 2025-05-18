using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using static System.Net.WebRequestMethods;

namespace priority_file_explorer_
{
    public partial class Form1 : Form
    {

        private enum ViewMode
        {
            SmallIcon,   // 작은 아이콘 + 이름 + 크기 + 날짜
            LargeIcon,   // 큰 아이콘 + 이름 + 크기 + 날짜
            SimpleList,  // 아이콘 없이 이름만 (간단히)
            Detailed     // 중간 크기 아이콘 + 이름 + 크기 + 날짜 (자세히)
        }
        private ViewMode currentMode; // 현재 보기 모드
        private Stack<string> pathHistory = new Stack<string>();
        private string currentPath = "";
        private Panel selectedPanel = null;
        private List<string> virtualRootEntries = new List<string>();   // 맨 처음 화면
        private string storageFolder = @"C:\MyExplorerData";

        string virtualRootSaveFile = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "MyExplorerData", "virtual_root.txt"


);

        public Form1()
        {
            InitializeComponent();

            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;  // 한 줄씩 아래로 출력
            flowLayoutPanel1.WrapContents = false;                   // 줄 바꿈 없음
            flowLayoutPanel1.AutoScroll = true;
            
            this.FormClosing += Form1_FormClosing;
            this.Load += Form1_Load;

            if (!Directory.Exists(storageFolder))
                Directory.CreateDirectory(storageFolder);


            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.SizeChanged += flowLayoutPanel1_SizeChanged;
            this.trvDir.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.trvDir_BeforeExpand);
            this.trvDir.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.trvDir_BeforeSelect);

            flowLayoutPanel1.ContextMenuStrip=cmnListView;

            // 기본 보기 모드 설정: SmallIcon
            currentMode = ViewMode.SmallIcon;

            // FlowLayoutPanel 전체에 ContextMenuStrip 할당 (원한다면)
            flowLayoutPanel1.ContextMenuStrip = cmnListView;

            // ContextMenuStrip 내부 메뉴 항목 클릭 시 호출될 핸들러를 연결
            mnuOpen.Click += mnuOpen_Click;
            mnuDetail.Click += mnuDetail_Click;
            mnuList.Click += mnuList_Click;
            mnuSmall.Click += mnuSmall_Click;
            mnuLarge.Click += mnuLarge_Click;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(virtualRootSaveFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(virtualRootSaveFile));

                System.IO.File.WriteAllLines(virtualRootSaveFile, virtualRootEntries);
            }
            catch (Exception ex)
            {
                MessageBox.Show("virtual root 저장 실패: " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (System.IO.File.Exists(virtualRootSaveFile))
                {
                    string[] paths = System.IO.File.ReadAllLines(virtualRootSaveFile);
                    virtualRootEntries = paths.ToList();
                    currentPath = "VIRTUAL_ROOT";
                    NavigateToFolder("VIRTUAL_ROOT", addToHistory: false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("virtual root 불러오기 실패: " + ex.Message);
            }

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

        // FlowLayoutPanel1(메인 패널) 위로 드래그된 데이터가 파일인지 아닌지 체크
        private void FlowLayoutPanel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        // FlowLayoutPanel1에 드래그 한 파일을 놓으면 패널에 파일 출력
        private void FlowLayoutPanel1_DragDrop(object sender, DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            AddFilesToCurrentPath(paths);
        }

        void AddClickHandler(Control parent, EventHandler handler)
        {
            parent.Click += handler;

            foreach (Control child in parent.Controls)
            {
                // 자식 컨트롤도 클릭 이벤트 달고
                child.Click += handler;

                // 자식의 자식이 있다면 → 재귀적으로 계속 연결
                if (child.HasChildren)
                {
                    AddClickHandler(child, handler);
                }
            }
        }
        void AddMouseDoubleClickHandler(Control parent, MouseEventHandler handler)
        {
            parent.MouseDoubleClick += handler;

            foreach (Control child in parent.Controls)
            {
                AddMouseDoubleClickHandler(child, handler);
            }
        }
        void AddMouseRightClickHandler(Control parent, MouseEventHandler handler)
        {
            parent.MouseUp += handler;

            foreach (Control child in parent.Controls)
            {
                AddMouseRightClickHandler(child, handler);
            }
        }

        // FileDialog를 열어 파일 추가
        private void 파일추가ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "파일 열기";
            openFileDialog.Filter = "모든 파일 (*.*)|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                AddFilesToCurrentPath(openFileDialog.FileNames);
            }
        }

        // 패널에 파일을 생성
        private Panel CreateFilePanel(string file)
        {
            PictureBox pb = CreateThumbnail(file);
            Panel infoPanel = CreateFileInfoPanel(file);

            Panel panel = new Panel();
            panel.Width = flowLayoutPanel1.Width - 2; // 스크롤바 감안해서 너비 맞춤
            panel.Height = 26;
            panel.Margin = new Padding(2);
            panel.BackColor = Color.Transparent;

            // 썸네일 좌측 정렬
            pb.Size = new Size(20, 20);
            pb.Location = new Point(8, 4);

            // 파일 정보 라벨 출력
            infoPanel.Location = new Point(40, 6);

            panel.Controls.Add(pb);
            panel.Controls.Add(infoPanel);

            // 더블클릭: 폴더 탐색 / 파일 실행
            MouseEventHandler doubleClickHandler = (s, e) =>
            {
                Control clicked = (Control)s;

                // 상위로 올라가서 filePanel 찾기
                while (clicked != null && !(clicked.Parent is FlowLayoutPanel))
                    clicked = clicked.Parent;

                if (clicked is Panel filePanel)
                {
                    string path = filePanel.Tag as string;

                    if (Directory.Exists(path))
                    {
                        NavigateToFolder(path);
                    }
                    else if (System.IO.File.Exists(path))
                    {
                        try
                        {
                            var psi = new ProcessStartInfo(path) { UseShellExecute = true };
                            Process.Start(psi);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("파일 실행 실패: " + ex.Message);
                        }
                    }
                }
            };

            // 클릭: 배경 강조
            EventHandler clickHandler = (s, e) =>
            {
                Control clicked = (Control)s;

                // 최상위 filePanel을 찾는다: flowLayoutPanel1 바로 아래에 있는 Panel
                while (clicked != null && !(clicked.Parent is FlowLayoutPanel))
                {
                    clicked = clicked.Parent;
                }

                if (clicked != null)
                {
                    // 선택 효과 처리
                    if (selectedPanel != null && selectedPanel != clicked)
                        selectedPanel.BackColor = Color.Transparent;

                    clicked.BackColor = Color.LightBlue;
                    selectedPanel = clicked as Panel;
                }
            };

            MouseEventHandler rightClickHandler = (s, e) =>
            {
                if (e.Button != MouseButtons.Right) return;

                Control clicked = (Control)s;

                // 최상위 패널 추적: flowLayoutPanel1 바로 아래에 있는 Panel
                while (clicked != null && !(clicked.Parent is FlowLayoutPanel))
                {
                    clicked = clicked.Parent;
                }

                if (clicked != null)
                {
                    selectedPanel = clicked as Panel;

                    // 우클릭 메뉴 보여주기 (마우스 위치 기준)
                    rightClickMenu.Show(clicked, clicked.PointToClient(Cursor.Position));
                }
            };

            panel.Tag = file; // 반드시 필요!
            AddMouseDoubleClickHandler(panel, doubleClickHandler);
            AddClickHandler(panel, clickHandler);
            AddMouseRightClickHandler(panel, rightClickHandler);

            return panel;
        }


        // 파일의 썸네일 생성
        private PictureBox CreateThumbnail(string file)
        {
            PictureBox pb = new PictureBox();
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Size = new Size(64, 64);
            pb.Margin = new Padding(0, 0, 0, 5);
            pb.Tag = file;

            string ext = Path.GetExtension(file).ToLower();

            // 이미지 파일이면 해당 이미지를 썸네일로, 이미지가 아닌 파일이면 기본 아이콘을 썸네일로 설정
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif")
            {
                try
                {
                    Image img = Image.FromFile(file);
                    pb.Image = new Bitmap(img, pb.Size);
                }
                catch
                {
                    pb.Image = SystemIcons.Warning.ToBitmap();
                }
            }
            else if (Directory.Exists(file)) // 폴더일 경우
            {
                pb.Image = Properties.Resources.folder;  // 또는 사용자 정의 폴더 아이콘
            }
            else
            {
                pb.Image = Icon.ExtractAssociatedIcon(file).ToBitmap();
            }

            return pb;
        }

        // 파일의 이름 생성
        private Panel CreateFileInfoPanel(string path)
        {
            FileInfo fileInfo = new FileInfo(path);

            Panel infoPanel = new Panel();
            infoPanel.Height = 30;
            infoPanel.Width = 800;
            infoPanel.BackColor = Color.Transparent;

            // 이름
            Label nameLabel = new Label();
            nameLabel.Text = Path.GetFileName(path);
            nameLabel.SetBounds(0, -6, 260, 30);
            nameLabel.TextAlign = ContentAlignment.MiddleLeft;

            // 날짜
            Label dateLabel = new Label();
            dateLabel.Text = fileInfo.Exists ? fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm") : "";
            dateLabel.SetBounds(270, -6, 140, 30);
            dateLabel.TextAlign = ContentAlignment.MiddleLeft;

            // 유형
            Label typeLabel = new Label();
            typeLabel.Text = Directory.Exists(path) ? "파일 폴더" : fileInfo.Extension.ToUpper() + " 파일";
            typeLabel.SetBounds(430, -6, 100, 30);
            typeLabel.TextAlign = ContentAlignment.MiddleLeft;

            // 크기
            Label sizeLabel = new Label();
            sizeLabel.Text = Directory.Exists(path) ? "" : GetSizeText(fileInfo.Length);
            sizeLabel.SetBounds(560, -6, 90, 30);
            sizeLabel.TextAlign = ContentAlignment.MiddleRight;

            // 추가
            infoPanel.Controls.Add(nameLabel);
            infoPanel.Controls.Add(dateLabel);
            infoPanel.Controls.Add(typeLabel);
            infoPanel.Controls.Add(sizeLabel);

            return infoPanel;
        }



        private void NavigateToFolder(string path, bool addToHistory = true)
        {
            if (addToHistory && !string.IsNullOrEmpty(currentPath))
            {
                pathHistory.Push(currentPath);
            }

            currentPath = path;
            flowLayoutPanel1.Controls.Clear();

            try
            {
                if (path == "VIRTUAL_ROOT")
                {
                    foreach (string entry in virtualRootEntries)
                    {
                        if (System.IO.File.Exists(entry) || Directory.Exists(entry))
                        {
                            flowLayoutPanel1.Controls.Add(CreateFilePanel(entry));
                        }
                    }
                    return;
                }

                string[] entries = Directory.GetFileSystemEntries(path);
                foreach (string entry in entries)
                {
                    flowLayoutPanel1.Controls.Add(CreateFilePanel(entry));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("폴더 열기 실패: " + ex.Message);
            }
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            if (pathHistory.Count > 0)
            {
                string previousPath = pathHistory.Pop();
                NavigateToFolder(previousPath, addToHistory: false); // 
            }
            else
            {
                MessageBox.Show("더 이상 뒤로 갈 폴더가 없습니다.");
            }
        }

        private string GetSizeText(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{(bytes / 1024.0):F1} KB";
            return $"{(bytes / (1024.0 * 1024)):F1} MB";
        }

        private void AddFilesToCurrentPath(string[] paths)
        {
            if (string.IsNullOrEmpty(currentPath))
            {
                currentPath = "VIRTUAL_ROOT";
                pathHistory.Clear();
                virtualRootEntries.Clear();
                flowLayoutPanel1.Controls.Clear();

                foreach (string path in paths)
                {
                    if ((System.IO.File.Exists(path) || Directory.Exists(path)) && !virtualRootEntries.Contains(path))
                    {
                        virtualRootEntries.Add(path);
                        flowLayoutPanel1.Controls.Add(CreateFilePanel(path));
                    }
                }

                return;
            }

            if (currentPath == "VIRTUAL_ROOT")
            {
                foreach (string path in paths)
                {
                    if ((System.IO.File.Exists(path) || Directory.Exists(path)) && !virtualRootEntries.Contains(path))
                    {
                        virtualRootEntries.Add(path);
                        flowLayoutPanel1.Controls.Add(CreateFilePanel(path));
                    }
                }

                return;
            }

            foreach (string path in paths)
            {
                if (System.IO.File.Exists(path))
                {
                    string fileName = Path.GetFileName(path);
                    string destPath = Path.Combine(currentPath, fileName);

                    try
                    {
                        if (!Directory.Exists(currentPath))
                            Directory.CreateDirectory(currentPath);

                        System.IO.File.Copy(path, destPath, overwrite: true);
                        flowLayoutPanel1.Controls.Add(CreateFilePanel(destPath));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"파일 복사 실패: {ex.Message}");
                    }
                }
                else if (Directory.Exists(path))
                {
                    flowLayoutPanel1.Controls.Add(CreateFilePanel(path));
                }
            }
        }

        private void 삭제ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedPanel == null) return;

            string path = selectedPanel.Tag as string;

            if (currentPath == "VIRTUAL_ROOT")
            {
                // 리스트에서만 제거
                virtualRootEntries.Remove(path);
                flowLayoutPanel1.Controls.Remove(selectedPanel);
            }
            else
            {
                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                        flowLayoutPanel1.Controls.Remove(selectedPanel);
                    }
                    else if (Directory.Exists(path))
                    {
                        Directory.Delete(path, recursive: true); // 폴더 및 하위 항목 포함 삭제
                        flowLayoutPanel1.Controls.Remove(selectedPanel);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("삭제 실패: " + ex.Message);
                }
            }

            selectedPanel = null;
        }

        // 여기서부터 Finder 관련 기능 
        /// FlowLayoutPanel에 넣을 “한 줄짜리” 패널을 생성.
        /// FileSystemInfo (FileInfo/DirectoryInfo)를 받아 아이콘, 이름, 크기, 날짜 레이블을 나란히 배치.
        
        private Panel CreateListItemPanel(FileSystemInfo info)
        {
            // 1) 아이콘 PictureBox
            PictureBox pic = new PictureBox();
            pic.SizeMode = PictureBoxSizeMode.StretchImage;
            pic.Size = new Size(16, 16);
            pic.Margin = new Padding(3, 6, 3, 3);
            pic.Tag = info.FullName;

            

            // 폴더인지 파일인지 확인해서 아이콘 설정
            if (info is DirectoryInfo)
            {
                // 폴더 아이콘을 리소스로 추가해 두셨다면 대체
                // pb.Image = Properties.Resources.folder_small;
                pic.Image = SystemIcons.WinLogo.ToBitmap(); // (예시) 기본 아이콘
            }
            else
            {
                try
                {
                    pic.Image = Icon.ExtractAssociatedIcon(info.FullName).ToBitmap();
                }
                catch
                {
                    pic.Image = SystemIcons.Application.ToBitmap();
                }
            }

            // 2) 이름 Label
            Label lblName = new Label();
            lblName.Text = info.Name;
            lblName.AutoSize = false;
            lblName.Width = 200;
            lblName.Height = 20;
            lblName.TextAlign = ContentAlignment.MiddleLeft;
            lblName.Margin = new Padding(3, 3, 3, 3);
            lblName.Tag = info.FullName;

            // 3) 크기 Label (파일만 표시, 폴더는 빈 문자열)
            Label lblSize = new Label();
            if (info is FileInfo fi)
            {
                lblSize.Text = GetSizeText(fi.Length);
            }
            else
            {
                lblSize.Text = "";
            }
            lblSize.AutoSize = false;
            lblSize.Width = 80;
            lblSize.Height = 20;
            lblSize.TextAlign = ContentAlignment.MiddleRight;
            lblSize.Margin = new Padding(3, 3, 3, 3);
            lblSize.Tag = info.FullName;

            // 4) 수정 날짜 Label
            Label lblDate = new Label();
            lblDate.Text = info.LastWriteTime.ToString("yyyy-MM-dd HH:mm");
            lblDate.AutoSize = false;
            lblDate.Width = 120;
            lblDate.Height = 20;
            lblDate.TextAlign = ContentAlignment.MiddleLeft;
            lblDate.Margin = new Padding(3, 3, 3, 3);
            lblDate.Tag = info.FullName;

            // 5) 전체를 담을 Panel 생성
            Panel itemPanel = new Panel();
            itemPanel.Width = flowLayoutPanel1.Width - 4; // 스크롤바 공간 고려
            itemPanel.Height = 28;
            itemPanel.Margin = new Padding(2);
            itemPanel.BackColor = Color.Transparent;
            itemPanel.Tag = info.FullName; // 클릭 시 경로를 알기 위해 저장

            // 컨트롤을 절대 좌표로 배치
            pic.Location = new Point(4, 4);
            lblName.Location = new Point(pic.Right + 4, 4);
            lblSize.Location = new Point(lblName.Right + 8, 4);
            lblDate.Location = new Point(lblSize.Right + 8, 4);

            // 순서대로 Panel에 추가
            itemPanel.Controls.Add(pic);
            itemPanel.Controls.Add(lblName);
            itemPanel.Controls.Add(lblSize);
            itemPanel.Controls.Add(lblDate);
            // 클릭 시 선택 효과, 더블클릭 시 열기/탐색 기능 연결

            // 클릭: 배경색 변경(선택 느낌)
            EventHandler clickHandler = (s, e) =>
            {
                Control clicked = s as Control;
                Panel parent = (clicked is Panel) ? (Panel)clicked : clicked.Parent as Panel;
                if (parent == null) return;

                if (selectedPanel != null && selectedPanel != parent)
                    selectedPanel.BackColor = Color.Transparent;

                parent.BackColor = Color.LightBlue;
                selectedPanel = parent;
            };

            // 더블클릭: 파일 실행 또는 폴더 탐색
            MouseEventHandler dblClickHandler = (s, e) =>
            {
                if (e.Button != MouseButtons.Left || e.Clicks < 2) return;
                Control clicked = s as Control;
                Panel parent = (clicked is Panel) ? (Panel)clicked : clicked.Parent as Panel;
                if (parent == null) return;

                string path = parent.Tag as string;
                if (Directory.Exists(path))
                {
                    NavigateToFolder(path);
                }
                else if (File.Exists(path))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("파일 실행 실패: " + ex.Message);
                    }
                }
            };

            // Panel과 자식 컨트롤에 모두 이벤트 연결
            itemPanel.Click += clickHandler;
            itemPanel.MouseDoubleClick += dblClickHandler;
            foreach (Control child in itemPanel.Controls)
            {
                child.Click += clickHandler;
                child.MouseDoubleClick += dblClickHandler;
            }

            return itemPanel;
        }
        private void ClearViewChecks()
        {
            mnuDetail.Checked = false;
            mnuList.Checked = false;
            mnuSmall.Checked = false;
            mnuLarge.Checked = false;
        }
        private void Child_DoubleClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        // FlowLayoutPanel 크기 변경 시, 자식 패널 너비를 맞춰 주는 핸들러
        private void flowLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control ctrl in flowLayoutPanel1.Controls)
            {
                ctrl.Width = flowLayoutPanel1.ClientSize.Width - 4;
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
            if (selectedPanel == null) return;
            string fullPath = selectedPanel.Tag as string;
            if (Directory.Exists(fullPath))
            {
                NavigateToFolder(fullPath);
            }
            else if (File.Exists(fullPath))
            {
                Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
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
                    //부모 경로에서 새로 추가할지 말지 결정 ==> 이미 똑같은 이름의 폴더가 있다면, 추가하지 않기 위해서
                    string parent = Path.GetDirectoryName(fullPath);
                    TreeNode pnode = FindNodeByPath(trvDir.Nodes, parent);
                    if (pnode != null)
                    {
                        string nodeName = Path.GetFileName(fullPath);
                        // 같은 이름의 노드가 있는지 확인

                        var existing = pnode.Nodes.Cast<TreeNode>()
                            .FirstOrDefault(n => string.Equals(n.Text, nodeName, StringComparison.InvariantCultureIgnoreCase));
                        if (existing != null)
                        {
                            target = existing;
                        }
                        else
                        {
                            target = pnode.Nodes.Add(nodeName);
                            setPlus(target);
                        }

                        trvDir.SelectedNode = target;
                        target.Expand();
                        trvDir.Focus();
                    }

                    /* if (pnode != null)
                     {
                         TreeNode node = pnode.Nodes.Add(Path.GetFileName(fullPath));
                         setPlus(node);
                         trvDir.SelectedNode = node;
                         node.Expand();
                         trvDir.Focus();
                     }*/
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

                foreach (DirectoryInfo dirs in di)
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

        //해당 폴더의 파일/폴더 목록을 FlowLayoutPannel 표시
        private void trvDir_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                // 1) 현재 Path 텍스트 업데이트
                string selectedPath = e.Node.FullPath;
                txtPath.Text = selectedPath;

                // 2) FlowLayoutPanel 클리어
                flowLayoutPanel1.Controls.Clear();

                DirectoryInfo di = new DirectoryInfo(selectedPath);

                // 3) 하위 폴더부터 추가
                foreach (DirectoryInfo tdls in di.GetDirectories())
                {
                    Panel folderPanel = CreateListItemPanel(tdls);
                    flowLayoutPanel1.Controls.Add(folderPanel);
                }

                // 4) 하위 파일 추가
                foreach (FileInfo fls in di.GetFiles())
                {
                    Panel filePanel = CreateListItemPanel(fls);
                    flowLayoutPanel1.Controls.Add(filePanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("폴더 열기 실패: " + ex.Message);
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
            ClearViewChecks();
            mnuDetail.Checked = true;
            currentMode = ViewMode.Detailed;
            ApplyViewModeToAllItems();
        }

        private void mnuList_Click(object sender, EventArgs e)
        {
            ClearViewChecks();
            mnuList.Checked = true;

            currentMode = ViewMode.SimpleList;
            ApplyViewModeToAllItems();
        }

        private void mnuSmall_Click(object sender, EventArgs e)
        {
            ClearViewChecks();
            mnuSmall.Checked = true;

            currentMode = ViewMode.SmallIcon;
            ApplyViewModeToAllItems();
        }

        private void mnuLarge_Click(object sender, EventArgs e)
        {
            ClearViewChecks();
            mnuLarge.Checked = true;

            currentMode = ViewMode.LargeIcon;
            ApplyViewModeToAllItems();
        }
        
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string input = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("검색어를 입력하세요.");
                return;
            }

            string rootPath, pattern;
            if (Path.IsPathRooted(input) && Directory.Exists(input))
            {
                rootPath = input;
                pattern = "*";
            }
            else
            {
                rootPath = txtPath.Text;
                pattern = "*" + input + "*";
            }

            txtPath.Text = rootPath;

            // 1) FlowLayoutPanel 클리어
            flowLayoutPanel1.Controls.Clear();

            // 2) 백그라운드에서 검색 수행
            var results = await Task.Run(() => GetSearchResults(rootPath, pattern));

            // 3) 검색 결과를 FlowLayoutPanel에 채우기
            foreach (var info in results)
            {
                Panel itemPanel = CreateListItemPanel(info);
                flowLayoutPanel1.Controls.Add(itemPanel);
            }
        }
        private List<FileSystemInfo> GetSearchResults(string root, string pattern)
        {
            var output = new List<FileSystemInfo>();
            var stack = new Stack<DirectoryInfo>();
            stack.Push(new DirectoryInfo(root));

            while (stack.Count > 0)
            {
                var dir = stack.Pop();

                // 파일 스트리밍
                try
                {
                    foreach (var file in dir.EnumerateFiles(pattern))
                        output.Add(file);
                }
                catch { /* 액세스 거부, TooLong 등 무시 */ }

                // 하위 폴더 스트리밍
                try
                {
                    foreach (var sub in dir.EnumerateDirectories())
                    {
                        // 순환 참조 방지: 재분석 지점(링크) 건너뛰기
                        if ((sub.Attributes & FileAttributes.ReparsePoint) != 0)
                            continue;

                        output.Add(sub);
                        stack.Push(sub);
                    }
                }
                catch { /* 액세스 거부, TooLong 등 무시 */ }
            }

            return output;
        }



        private void SearchDirectory(string path, string pattern)
        {
            try
            {
                var dirInfo = new DirectoryInfo(path);

                // 와일드카드가 비어 있으면 모든 항목(*) 검색
                string searchPattern = string.IsNullOrEmpty(pattern)
                    ? "*"
                    : "*" + pattern + "*";

            
                // 1) 현재 폴더 내의 패턴에 맞는 하위 폴더 검색
                foreach (var folder in dirInfo.GetDirectories(searchPattern))
                {
                   
                    Panel folderPanel = CreateListItemPanel(folder);
                    flowLayoutPanel1.Controls.Add(folderPanel);
                }

            
                // 2) 현재 폴더 내의 패턴에 맞는 파일 검색
                foreach (var file in dirInfo.GetFiles(searchPattern))
                {
                   
                    Panel filePanel = CreateListItemPanel(file);
                    flowLayoutPanel1.Controls.Add(filePanel);
                }

                // 3) 재귀: 하위 모든 폴더 탐색
                foreach (var sub in dirInfo.GetDirectories())
                {
                    SearchDirectory(sub.FullName, pattern);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // 접근 거부 등 예외는 무시
            }
            catch (Exception ex)
            {
                MessageBox.Show("검색 중 오류: " + ex.Message);
            }
        }

        /// <summary>
        /// FlowLayoutPanel 에 들어있는 모든 항목(Panel)에 대해
        /// 현재 설정된 currentMode 에 맞춰 아이콘 크기/레이블 보이기 여부 등을 적용한다.
        /// </summary>
        private void ApplyViewModeToAllItems()
        {
            foreach (Control ctl in flowLayoutPanel1.Controls)
            {
                if (ctl is Panel itemPanel && itemPanel.Tag is string fullPath)
                {
                    // Panel.Tag 에 저장된 fullPath(절대경로)를 기반으로 FileSystemInfo 객체를 만든다.
                    FileSystemInfo info = Directory.Exists(fullPath)
                        ? (FileSystemInfo)new DirectoryInfo(fullPath)
                        : new FileInfo(fullPath);

                    ApplyViewModeToPanel(itemPanel, info);
                }
            }
        }

        /// <summary>
        /// 단일 Panel(파일/폴더 한 줄) 컨트롤에 대해 currentMode 에 맞춰
        /// 아이콘 크기, 라벨(이름·크기·날짜) 보이기/숨기기, 위치 재조정 등을 수행한다.
        /// </summary>
        private void ApplyViewModeToPanel(Panel itemPanel, FileSystemInfo info)
        {
            // 1) Panel 내부의 PictureBox(아이콘)와 Label(이름·크기·날짜) 컨트롤을 찾아낸다.
            //    - 우리는 CreateListItemPanel에서 PictureBox 한 개, Label 세 개를 생성해 두었으므로 그 순서대로 찾으면 됩니다.
            PictureBox pic = itemPanel.Controls.OfType<PictureBox>().FirstOrDefault();
            // 이름 레이블(텍스트가 info.Name 과 동일)
            Label lblName = itemPanel.Controls
                .OfType<Label>()
                .FirstOrDefault(l => l.Text == info.Name);

            // 다른 레이블은 Tag 속성에 fullPath를 넣어두었으므로, Tag 가 같은 것 중 뒤에 두 개(Label 순서: 이름, 크기, 날짜)
            var allLabels = itemPanel.Controls.OfType<Label>()
                              .Where(l => l.Tag as string == info.FullName)
                              .ToList();
            Label lblSize = (allLabels.Count >= 2) ? allLabels[1] : null;
            Label lblDate = (allLabels.Count >= 3) ? allLabels[2] : null;

            // 2) currentMode 에 따라 아이콘 크기와 라벨 Visible 여부를 조정
            switch (currentMode)
            {
                case ViewMode.SmallIcon:
                    if (pic != null) { pic.Size = new Size(16, 16); pic.Visible = true; }
                    if (lblSize != null) lblSize.Visible = true;
                    if (lblDate != null) lblDate.Visible = true;
                    break;

                case ViewMode.LargeIcon:
                    if (pic != null) { pic.Size = new Size(64, 64); pic.Visible = true; }
                    if (lblSize != null) lblSize.Visible = true;
                    if (lblDate != null) lblDate.Visible = true;
                    break;

                case ViewMode.SimpleList:
                    if (pic != null) pic.Visible = false;
                    if (lblSize != null) lblSize.Visible = false;
                    if (lblDate != null) lblDate.Visible = false;
                    break;

                case ViewMode.Detailed:
                    if (pic != null) { pic.Size = new Size(32, 32); pic.Visible = true; }
                    if (lblSize != null) lblSize.Visible = true;
                    if (lblDate != null) lblDate.Visible = true;
                    break;
            }

            // 3) 변경된 크기/가시성에 맞춰 위치를 재배치한다.
            //    - 아이콘(pic) 재배치
            if (pic != null)
                pic.Location = new Point(4, 4);

            //    - 이름 레이블(lblName)은 아이콘 우측(또는 좌측 여백)에서 시작
            if (lblName != null)
            {
                int xAfterIcon = (pic != null && pic.Visible) ? pic.Right : 4;
                lblName.Location = new Point(xAfterIcon + 4, 4);
            }

            //    - 크기 레이블(lblSize)은 이름 오른쪽 + 간격
            if (lblSize != null)
                lblSize.Location = new Point(lblName.Right + 8, 4);

            //    - 날짜 레이블(lblDate)은 크기 레이블 오른쪽 + 간격
            if (lblDate != null)
                lblDate.Location = new Point(lblSize.Right + 8, 4);
        }

    }
}