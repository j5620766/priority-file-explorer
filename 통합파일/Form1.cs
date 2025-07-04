﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace priority_file_explorer_
{
    public partial class Form1 : Form
    {
        private Stack<string> pathHistory = new Stack<string>();
        private string currentPath = "";
        private Panel selectedPanel = null;
        private List<string> virtualRootEntries = new List<string>();   // 맨 처음 화면
        private string storageFolder = @"C:\MyExplorerData";
        private bool usePri = true;

        string virtualRootSaveFile = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "MyExplorerData", "virtual_root.txt"
);

        Dictionary<string, int> priorityInfo = new Dictionary<string, int>();

        bool is_highlighted;

        string priorityFilePath = "priority.txt"; // bin\Debug에 위치
        string highlightFilePath = "highlight.txt"; // bin\Debug에 위치

        private enum SortField { Name, Date, Type, Size, Priority }
        private SortField currentSortField = SortField.Name;
        private bool ascending = true;

        private CancellationTokenSource _searchCts;          // ← 탐색 취소용
        private const int BatchSize = 3;

        public Form1()
        {
            InitializeComponent();

            this.trvDir.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.trvDir_BeforeExpand);
            this.trvDir.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.trvDir_BeforeSelect);

            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;  // 한 줄씩 아래로 출력
            flowLayoutPanel1.WrapContents = false;                   // 줄 바꿈 없음
            flowLayoutPanel1.AutoScroll = true;

            this.FormClosing += Form1_FormClosing;

            if (!Directory.Exists(storageFolder))
                Directory.CreateDirectory(storageFolder);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(virtualRootSaveFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(virtualRootSaveFile));

                File.WriteAllLines(virtualRootSaveFile, virtualRootEntries);
            }
            catch (Exception ex)
            {
                MessageBox.Show("virtual root 저장 실패: " + ex.Message);
            }

            SavePriorityData();
            SaveHighlightInfo();
        }

        private void SavePriorityData()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(priorityFilePath))
                {
                    foreach (var pair in priorityInfo)
                        writer.WriteLine($"{pair.Key},{pair.Value}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("우선순위 저장 실패: " + ex.Message);
            }
        }

        private void SaveHighlightInfo()
        {
            File.WriteAllText(highlightFilePath, is_highlighted.ToString());
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(virtualRootSaveFile))
                {
                    string[] paths = File.ReadAllLines(virtualRootSaveFile);
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
            {
                root = trvDir.Nodes.Add(Drv);
                root.Tag = Drv; // 실제 경로 저장
                root.ImageIndex = 2;
                if (trvDir.SelectedNode == null)
                    trvDir.SelectedNode = null;
                root.SelectedImageIndex = root.ImageIndex;
                root.Nodes.Add(""); // 더미 노드
            }

            LoadPriorityData();
            LoadHighlightInfo();

            // 파일 로드 후 즉시 재정렬
            SortFilePanels();
        }

        private void LoadPriorityData()
        {
            priorityInfo = new Dictionary<string, int>();

            if (File.Exists(priorityFilePath))
            {
                string[] lines = File.ReadAllLines(priorityFilePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');

                    if (parts.Length == 2)
                    {
                        string path = parts[0];

                        if (int.TryParse(parts[1], out int priority))
                            priorityInfo[path] = priority;
                    }
                }
            }
        }

        private void LoadHighlightInfo()
        {
            if (File.Exists(highlightFilePath))
            {
                string info = File.ReadAllText(highlightFilePath).Trim().ToLower();

                if (info == "true")
                    is_highlighted = true;
                else
                    is_highlighted = false;

                우선순위강조ToolStripMenuItem.Checked = is_highlighted;
            }

            PriorityHighlight();
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
                    else if (File.Exists(path))
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
                    if (selectedPanel != null && selectedPanel != clicked)
                    {
                        string path = selectedPanel.Tag as string;

                        if (is_highlighted && path != null && priorityInfo.ContainsKey(path) && priorityInfo[path] > 0)
                        {
                            int priority = priorityInfo[path];
                            int b = (int)(255 - (priority / 100.0) * 255);
                            b = Math.Min(200, b);
                            selectedPanel.BackColor = Color.FromArgb(255, 255, b);
                        }
                        else
                        {
                            selectedPanel.BackColor = Color.Transparent;
                        }
                    }

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
            else {
                try {
                    if (File.Exists(file)) {
                        Icon icon = Icon.ExtractAssociatedIcon(file);

                        if (icon != null)
                            pb.Image = icon.ToBitmap();
                        else
                            pb.Image = SystemIcons.Warning.ToBitmap();  // fallback
                    } else {
                        pb.Image = SystemIcons.Warning.ToBitmap(); // path가 아예 없을 때도 대비
                    }
                } catch {
                    pb.Image = SystemIcons.Warning.ToBitmap(); // icon 추출 자체 실패
                }
            }

            return pb;
        }

        // 파일의 이름 생성
        private Panel CreateFileInfoPanel(string path)
        {
            FileInfo fileInfo = new FileInfo(path);

            Font infoFont = new Font("Segoe UI", 9f);

            Panel infoPanel = new Panel();
            infoPanel.Height = 30;
            infoPanel.Width = 800;
            infoPanel.BackColor = Color.Transparent;

            // 이름
            Label nameLabel = new Label();
            nameLabel.Text = Path.GetFileName(path);
            nameLabel.SetBounds(0, -4, 260, infoFont.Height);
            nameLabel.TextAlign = ContentAlignment.MiddleLeft;
            nameLabel.Font = infoFont;
            nameLabel.AutoSize = false;
            nameLabel.AutoEllipsis = true;
            nameLabel.MaximumSize = new Size(260, infoFont.Height);
            nameLabel.Dock = DockStyle.Left;


            // 날짜
            Label dateLabel = new Label();
            dateLabel.Text =
            File.Exists(path) ? new FileInfo(path).LastWriteTime.ToString("yyyy-MM-dd HH:mm") :
            Directory.Exists(path) ? new DirectoryInfo(path).LastWriteTime.ToString("yyyy-MM-dd HH:mm") :
            "";
            dateLabel.SetBounds(270, -6, 140, 30);
            dateLabel.TextAlign = ContentAlignment.MiddleLeft;
            dateLabel.Font = infoFont;

            // 유형
            Label typeLabel = new Label();
            typeLabel.Text = Directory.Exists(path) ? "파일 폴더" : fileInfo.Extension.ToUpper() + " 파일";
            typeLabel.SetBounds(430, -6, 100, 30);
            typeLabel.TextAlign = ContentAlignment.MiddleLeft;
            typeLabel.Font = infoFont;

            // 크기
            Label sizeLabel = new Label();
            sizeLabel.Text = Directory.Exists(path) ? "" : GetSizeText(fileInfo.Length);
            sizeLabel.SetBounds(560, -6, 90, 30);
            sizeLabel.TextAlign = ContentAlignment.MiddleRight;
            sizeLabel.Font = infoFont;

            // 우선순위
            Label priorityLabel = new Label();
            int priority = priorityInfo.ContainsKey(path) ? priorityInfo[path] : 0;
            priorityLabel.Text = priority.ToString();
            priorityLabel.SetBounds(750, -6, 90, 30);
            priorityLabel.TextAlign = ContentAlignment.MiddleLeft;
            priorityLabel.Font = infoFont;

            infoPanel.Controls.Add(priorityLabel);


            // 추가
            infoPanel.Controls.Add(nameLabel);
            infoPanel.Controls.Add(dateLabel);
            infoPanel.Controls.Add(typeLabel);
            infoPanel.Controls.Add(sizeLabel);

            return infoPanel;
        }

        private void NavigateToFolder(string path, bool addToHistory = true)
        {
            _searchCts?.Cancel();
            _searchCts = null;

            if (addToHistory && !string.IsNullOrEmpty(currentPath))
            {
                pathHistory.Push(currentPath);
            }

            currentPath = path;
            RenderPathBar(currentPath);
            flowLayoutPanel1.Controls.Clear();

            try
            {
                if (path == "VIRTUAL_ROOT")
                {
                    this.Text = "파일 탐색기";

                    foreach (string entry in virtualRootEntries)
                    {
                        if (File.Exists(entry) || Directory.Exists(entry))
                        {
                            flowLayoutPanel1.Controls.Add(CreateFilePanel(entry));
                        }
                    }

                    PriorityHighlight();

                    return;
                }

                string folderName = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar));
                if (string.IsNullOrEmpty(folderName))
                    folderName = path;

                this.Text = folderName;

                string[] entries = Directory.GetFileSystemEntries(path);
                foreach (string entry in entries)
                {
                    flowLayoutPanel1.Controls.Add(CreateFilePanel(entry));
                }

                PriorityHighlight();
            }
            catch (UnauthorizedAccessException) { /* ignore */ }
            catch (PathTooLongException) { /* ignore */ }
            catch (Exception ex) { Debug.WriteLine(ex); }
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
                    if ((File.Exists(path) || Directory.Exists(path)) && !virtualRootEntries.Contains(path))
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
                    if ((File.Exists(path) || Directory.Exists(path)) && !virtualRootEntries.Contains(path))
                    {
                        virtualRootEntries.Add(path);
                        flowLayoutPanel1.Controls.Add(CreateFilePanel(path));
                    }
                }

                return;
            }

            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    string fileName = Path.GetFileName(path);
                    string destPath = Path.Combine(currentPath, fileName);

                    try
                    {
                        if (!Directory.Exists(currentPath))
                            Directory.CreateDirectory(currentPath);

                        File.Copy(path, destPath, overwrite: true);
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
                    if (File.Exists(path))
                    {
                        File.Delete(path);
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

        private void 우선순위설정ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedPanel == null) return;

            string path = selectedPanel.Tag as string;
            if (string.IsNullOrEmpty(path)) return;

            string currentPriority = priorityInfo.ContainsKey(path) ? priorityInfo[path].ToString() : "0";

            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "우선순위를 입력하세요. (0~100)", "우선순위 설정", currentPriority);

            if (input == "") return;

            if (int.TryParse(input, out int priority))
            {
                if (priority >= 0 && priority <= 100)
                {
                    priorityInfo[path] = priority;

                    // UI 갱신
                    foreach (Control ctrl in selectedPanel.Controls)
                    {
                        if (ctrl is Panel infoPanel)
                        {
                            foreach (Control sub in infoPanel.Controls)
                            {
                                if (sub is Label label && int.TryParse(label.Text, out _)) // 숫자만 있는 라벨 필터링
                                {
                                    label.Text = priority.ToString();

                                    break;
                                }
                            }
                        }
                    }

                    // 강조 반영
                    if (is_highlighted && priority > 0)
                    {
                        int b = (int)(255 - (priority / 100.0) * 255);
                        b = Math.Min(200, b);
                        selectedPanel.BackColor = Color.FromArgb(255, 255, b);
                    }
                    else
                    {
                        selectedPanel.BackColor = Color.Transparent;
                    }

                    // 우선순위 설정 시 즉시 재정렬
                    SortFilePanels();
                }
                else
                {
                    MessageBox.Show("0에서 100 사이의 값을 입력하세요.");
                }
            }
            else
            {
                MessageBox.Show("숫자를 입력하세요.");
            }
        }


        private void 우선순위강조ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            menuItem.Checked = !menuItem.Checked;
            is_highlighted = menuItem.Checked;

            PriorityHighlight();
        }

        private void PriorityHighlight()
        {
            foreach (Control ctrl in flowLayoutPanel1.Controls)
            {
                if (ctrl is Panel panel && panel.Tag is string path)
                {
                    int priority = priorityInfo.ContainsKey(path) ? priorityInfo[path] : 0;

                    if (is_highlighted && priority > 0)
                    {
                        int b = (int)(255 - (priority / 100.0) * 255);
                        b = Math.Min(200, b); // 최소 밝기 확보
                        panel.BackColor = Color.FromArgb(255, 255, b);
                    }
                    else
                        panel.BackColor = Color.Transparent;
                }
            }
        }

        private void 우선순위초기화ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("모든 우선순위를 초기화하시겠습니까?", "확인",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                priorityInfo.Clear();

                foreach (Control ctrl in flowLayoutPanel1.Controls)
                {
                    if (ctrl is Panel panel && panel.Tag is string path)
                    {
                        // 우선순위 라벨 텍스트 0으로 변경
                        foreach (Control sub in panel.Controls)
                        {
                            if (sub is Panel infoPanel)
                            {
                                foreach (Control labelCtrl in infoPanel.Controls)
                                {
                                    if (labelCtrl is Label label && int.TryParse(label.Text, out _))
                                    {
                                        label.Text = "0";

                                        break;
                                    }
                                }
                            }
                        }

                        // 강조 제거
                        panel.BackColor = Color.Transparent;
                    }
                }
            }

            // 재정렬
            SortFilePanels();
        }

        private void ToggleSort(SortField field) {
            if (currentSortField == field)
                ascending = !ascending;
            else {
                currentSortField = field;
                ascending = true;
            }

            SortFilePanels();
            UpdateSortLabels();
        }


        private void SortFilePanels()
        {
            List<Panel> panelList = flowLayoutPanel1.Controls
                .OfType<Panel>()
                .ToList();

            panelList.Sort((a, b) =>
            {
                string pathA = a.Tag as string;
                string pathB = b.Tag as string;

                FileInfo infoA = new FileInfo(pathA);
                FileInfo infoB = new FileInfo(pathB);

                int cmp = 0;

                if (usePri) {
                    int priA = priorityInfo.ContainsKey(pathA) ? priorityInfo[pathA] : 0;
                    int priB = priorityInfo.ContainsKey(pathB) ? priorityInfo[pathB] : 0;

                    cmp = priB.CompareTo(priA);

                    if (cmp != 0)
                        return cmp;
                }

                switch (currentSortField)
                {
                    case SortField.Name:
                        cmp = string.Compare(infoA.Name, infoB.Name, StringComparison.OrdinalIgnoreCase);
                        break;

                    case SortField.Date:
                        cmp = DateTime.Compare(infoA.LastWriteTime, infoB.LastWriteTime);
                        break;

                    case SortField.Type:
                        cmp = string.Compare(infoA.Extension, infoB.Extension, StringComparison.OrdinalIgnoreCase);
                        break;

                    case SortField.Size:
                        long sizeA = Directory.Exists(pathA) ? 0 : infoA.Length;
                        long sizeB = Directory.Exists(pathB) ? 0 : infoB.Length;
                        cmp = sizeA.CompareTo(sizeB);
                        break;

                    case SortField.Priority:
                        cmp = 0;
                        break;
                }

                return ascending ? cmp : -cmp;
            });

            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.Controls.Clear();
            foreach (var panel in panelList)
                flowLayoutPanel1.Controls.Add(panel);
            flowLayoutPanel1.ResumeLayout();
        }


        private void label1_Click(object sender, EventArgs e)
        {
            ToggleSort(SortField.Name);
        }
        private void label2_Click(object sender, EventArgs e)
        {
            ToggleSort(SortField.Date);
        }
        private void label3_Click(object sender, EventArgs e)
        {
            ToggleSort(SortField.Type);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            ToggleSort(SortField.Size);
        }


        private void UpdateSortLabels() {
            // 초기화
            label1.Text = "이름";
            label2.Text = "날짜";
            label3.Text = "유형";
            label4.Text = "크기";

            string direction = ascending ? "△" : "▽";

            switch (currentSortField) {
                case SortField.Name:
                    label1.Text += $" ({direction})";
                    break;
                case SortField.Date:
                    label2.Text += $" ({direction})";
                    break;
                case SortField.Type:
                    label3.Text += $" ({direction})";
                    break;
                case SortField.Size:
                    label4.Text += $" ({direction})";
                    break;
            }
        }


        // 상단 패널 하단에 줄 긋기
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.Gray, 1)) // 선 색상과 두께
            {
                int y = panel2.Height - 1; // 맨 아래쪽에 선 그리기
                e.Graphics.DrawLine(pen, 0, y, panel2.Width, y);
            }
        }

        void RenderPathBar(string fullPath)
        {
            pathBar.Controls.Clear();

            string[] parts = fullPath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            // 직접 누적 경로 초기화
            string cumulativePath = parts[0].EndsWith(":") ? parts[0] + "\\" : parts[0]; // 예: "C:/"로 시작 보장

            int x = 10;

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];

                // 첫 부분("C:")은 이미 누적됨, 이후부터 누적
                if (i > 0)
                    cumulativePath = Path.Combine(cumulativePath, part);

                // 버튼 생성
                Button btn = new Button
                {
                    Text = part,
                    Height = 24,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Location = new Point(x, 5)
                };

                string targetPath = cumulativePath;
                btn.Click += (s, e) =>
                {
                    if (Directory.Exists(targetPath))
                    {
                        NavigateToFolder(targetPath);
                    }
                    else
                    {
                        MessageBox.Show("폴더가 존재하지 않음: " + targetPath);
                    }
                };

                pathBar.Controls.Add(btn);
                x += btn.Width;

                // ▶ 구분자 추가
                if (i < parts.Length - 1)
                {
                    Label arrow = new Label
                    {
                        Text = "▶",
                        Location = new Point(x + 2, 8),
                        AutoSize = true,
                        ForeColor = Color.Gray
                    };
                    pathBar.Controls.Add(arrow);
                    x += arrow.Width + 5;
                }
            }
        }

        private void 우선순위정렬ToolStripMenuItem_Click(object sender, EventArgs e) {
            if (usePri) {
                우선순위정렬ToolStripMenuItem.Checked = false;
                usePri = false;
                SortFilePanels();
            }
            else {
                우선순위정렬ToolStripMenuItem.Checked = true;
                usePri = true;
                SortFilePanels();
            }
        }

        private IEnumerable<FileSystemInfo> EnumerateRecursively(
        string root, string pattern, CancellationToken token)
        {
            var stack = new Stack<DirectoryInfo>();
            stack.Push(new DirectoryInfo(root));

            while (stack.Count > 0)
            {
                token.ThrowIfCancellationRequested();
                DirectoryInfo dir = stack.Pop();

                /* 1) 파일 */
                IEnumerable<FileInfo> files;
                try { files = dir.EnumerateFiles(pattern, SearchOption.TopDirectoryOnly); }
                catch { continue; }

                foreach (var f in files)
                {
                    yield return f;
                    token.ThrowIfCancellationRequested();
                }

                /* 2) 하위 폴더 */
                DirectoryInfo[] subs;
                try { subs = dir.GetDirectories(); }
                catch { continue; }

                foreach (var sub in subs)
                {
                    if ((sub.Attributes & FileAttributes.ReparsePoint) == 0)
                        stack.Push(sub);

                    if (WildcardMatch(sub.Name, pattern))
                        yield return sub;
                }
            }
        }

        /* ---------- ④ btnSearch_Click : 배치-단위로 패널 추가 ---------- */
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            /* (1) 이전 검색 중단 + 새 토큰 생성 */
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            CancellationToken token = _searchCts.Token;

            /* (2) rootPath / pattern 결정 – 기존 로직 재사용 */
            string input = txtPath.Text.Trim();
            if (string.IsNullOrEmpty(input)) { MessageBox.Show("검색어를 입력하세요."); return; }

            string rootPath, pattern;
            if (Path.IsPathRooted(input) && Directory.Exists(input))
            {
                rootPath = input.EndsWith("\\") ? input : input + "\\";
                pattern = "*";
            }
            else
            {
                if (string.IsNullOrEmpty(currentPath) || currentPath == "VIRTUAL_ROOT")
                { MessageBox.Show("검색할 폴더를 먼저 선택하세요."); return; }

                rootPath = currentPath;
                pattern = "*" + input + "*";
            }
            string searchPathLabel = Path.GetFileName(rootPath.TrimEnd('\\'));
            if (string.IsNullOrEmpty(searchPathLabel))
                searchPathLabel = rootPath;

            RenderPathBar(searchPathLabel + " ▶ 검색 중...");

            /* (3) UI 초기화 */
            flowLayoutPanel1.Controls.Clear();

            /* (4) 백그라운드 검색 */
            await Task.Run(() =>
            {
                var batch = new List<FileSystemInfo>(BatchSize);

                foreach (var info in EnumerateRecursively(rootPath, pattern, token))
                {
                    batch.Add(info);

                    if (batch.Count == BatchSize)
                    {
                        SendBatch(batch);
                        batch = new List<FileSystemInfo>(BatchSize);
                    }
                }
                if (batch.Count > 0) SendBatch(batch);

                /* 로컬 함수 – UI 스레드로 전달 */
                void SendBatch(List<FileSystemInfo> snapshot)
                {
                    try {
                        if (!this.IsHandleCreated || this.IsDisposed) return;

                        this.Invoke((MethodInvoker)(() =>
                        {
                            foreach (var f in snapshot) {
                                string path = f.FullName;
                                if (File.Exists(path) || Directory.Exists(path)) {
                                    try {
                                        var panel = CreateFilePanel(path);
                                        if (panel != null)
                                            flowLayoutPanel1.Controls.Add(panel);
                                    } catch (Exception ex) {
                                        Debug.WriteLine("패널 생성 중 오류: " + ex.Message);
                                    }
                                }
                            }
                        }));
                    } catch (ObjectDisposedException) { } catch (InvalidOperationException) { }
                }
            }, token)
            /* (5) 마무리 */
            .ContinueWith(t =>
            {
                if (IsDisposed) return;                // 폼이 이미 닫혔으면 무시
                this.Invoke((MethodInvoker)(() =>
                {
                    string folderName = Path.GetFileName(rootPath.TrimEnd('\\'));
                    if (string.IsNullOrEmpty(folderName))
                        folderName = rootPath;

                    RenderPathBar(folderName + "의 검색 결과");
                }));
            });
        }


        private List<FileSystemInfo> GetSearchResults(string root, string pattern)
        {
            var output = new List<FileSystemInfo>();
            var stack = new Stack<DirectoryInfo>();
            stack.Push(new DirectoryInfo(root));

            while (stack.Count > 0)
            {
                var dir = stack.Pop();

                // 1. 패턴에 맞는 파일 추가
                try
                {
                    foreach (var file in dir.EnumerateFiles(pattern))
                        output.Add(file);
                }
                catch { /* 액세스 거부, TooLong 등 무시 */ }

                // 2. 모든 하위 폴더 탐색, 결과에는 패턴에 맞는 폴더만 추가
                try
                {
                    foreach (var sub in dir.EnumerateDirectories())
                    {
                        // 결과에는 패턴에 맞는 폴더만 추가
                        if (WildcardMatch(sub.Name, pattern))
                            output.Add(sub);

                        // 하위 폴더는 무조건 탐색
                        if ((sub.Attributes & FileAttributes.ReparsePoint) == 0)
                            stack.Push(sub);
                    }
                }
                catch { /* 액세스 거부, TooLong 등 무시 */ }
            }

            return output;
        }

        // 와일드카드 패턴 매칭 함수 (간단 버전)
        private bool WildcardMatch(string input, string pattern)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(
                input,
                "^" + System.Text.RegularExpressions.Regex.Escape(pattern).Replace("\\*", ".*") + "$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
        }

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
            catch (UnauthorizedAccessException) { }
            catch (PathTooLongException) { }
            catch (Exception ex)
            {

            }
        }

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

        private void trvDir_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                e.Node.Nodes.Clear();
                string path = e.Node.Tag as string;
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return;

                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    TreeNode node = e.Node.Nodes.Add(subDir.Name);
                    node.Tag = subDir.FullName; // 실제 경로 저장
                    setPlus(node);
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (PathTooLongException) { }
            catch (Exception ex)
            {
                
            }
        }

        //해당 폴더의 파일/폴더 목록을 FlowLayoutPannel 표시
        private void trvDir_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                // 1) 실제 경로는 Tag에서 가져옴
                string selectedPath = e.Node.Tag as string;
                if (string.IsNullOrEmpty(selectedPath) || !Directory.Exists(selectedPath))
                    return;

                // 2) 폴더 열기
                NavigateToFolder(selectedPath);
            }
            catch (UnauthorizedAccessException) { /* ignore */ }
            catch (PathTooLongException) { /* ignore */ }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }


    }
}
