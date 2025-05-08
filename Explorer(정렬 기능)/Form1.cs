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
using System.Collections;

namespace Explorer
{
    public partial class Form1 : Form
    {
        Dictionary<string, int> priorityInfo = new Dictionary<string, int>();
        string priorityFilePath = "priority.txt"; // bin\Debug에 위치
        private int sortColumn = -1;   // 마지막으로 클릭한 열
        private bool sortAscending = true; // 오름차순 정렬

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] Drv_list;
            TreeNode root;

            Drv_list = Environment.GetLogicalDrives(); // 전체 드라이브 가져옴

            foreach (string Drv in Drv_list)
            {
                root = treeView1.Nodes.Add(Drv);
                root.ImageIndex = 2; // 이미지 리스트 번호

                // 처음 실행 시 첫 번째 드라이브가 선택되도록 설정
                if (treeView1.SelectedNode == null)
                    treeView1.SelectedNode = root;

                root.SelectedImageIndex = root.ImageIndex;
                root.Nodes.Add(""); // 드라이브 옆에 '+' 표시가 나오도록 설정
            }

            listView1.Columns.Clear();

            자세히ToolStripMenuItem.Checked = true;
            listView1.View = View.Details;

            listView1.Columns.Add("이름", 200);
            listView1.Columns.Add("크기", 80);
            listView1.Columns.Add("수정한 날짜", 150);
            listView1.Columns.Add("우선순위", 100);

            LoadPriorityData();
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
                        int priority;

                        if (int.TryParse(parts[1], out priority))
                            priorityInfo[path] = priority;
                    }
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SavePriorityData();
        }

        private void SavePriorityData()
        {
            using (StreamWriter writer = new StreamWriter(priorityFilePath))
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.SubItems.Count >= 4)
                    {
                        string fullPath = textBox1.Text + "\\" + item.Text;
                        string priority = item.SubItems[3].Text;

                        writer.WriteLine($"{fullPath},{priority}");
                    }
                }
            }
        }

        // 트리뷰에서 실행
        public void setPlus(TreeNode node)
        {
            DirectoryInfo[] di;
            DirectoryInfo dir;
            string path;

            try
            {
                path = node.FullPath; // 해당 노드의 전체 경로
                dir = new DirectoryInfo(path);
                di = dir.GetDirectories(); // 해당 경로에 있는 디렉터리 리스트

                // 하위 폴더가 하나라도 있으면 '+' 표시가 나오도록 설정
                if (di.Length > 0)
                    node.Nodes.Add("");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // 리스트뷰에서 실행
        public void OpenFiles()
        {
            ListView.SelectedListViewItemCollection siList;
            siList = listView1.SelectedItems;

            foreach (ListViewItem item in siList)
                OpenItem(item);
        }

        public void OpenItem(ListViewItem item)
        {
            TreeNode node;
            TreeNode child;

            // item이 폴더인 경우
            if (item.Tag.ToString() == "D")
            {
                // 현재 선택된 트리의 노드를 확장
                node = treeView1.SelectedNode;
                node.Expand();

                // 확장된 노드의 자식 노드들 중 이름이 같은 노드를 찾아서 트리뷰에서 선택하고 포커스 이동
                child = node.FirstNode;
                while (child != null)
                {
                    if (child.Text == item.Text)
                    {
                        treeView1.SelectedNode = child;
                        treeView1.Focus();

                        break;
                    }

                    child = child.NextNode;
                }
            }
            else
            {
                // item이 파일인 경우
                Process.Start(textBox1.Text + "\\" + item.Text);
            }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            DirectoryInfo[] di; // 하위 디렉터리
            DirectoryInfo dir; // 디렉터리 정보
            string path; // 경로 지정 변수
            TreeNode node;

            try
            {
                e.Node.Nodes.Clear(); // 확장할 노드의 하위 노드 초기화

                path = e.Node.FullPath;
                dir = new DirectoryInfo(path);
                di = dir.GetDirectories();

                foreach (DirectoryInfo dirs in di)
                {
                    // 하위 디렉터리의 노드 추가 및 해당 노드의 setPlus 함수 실행
                    node = e.Node.Nodes.Add(dirs.Name);
                    setPlus(node);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // 트리뷰에서 다른 노드를 선택하기 직전에 실행
        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            DirectoryInfo[] diArray;
            DirectoryInfo di;
            FileInfo[] fiArray;
            ListViewItem item;

            try
            {
                textBox1.Text = e.Node.FullPath.Substring(0, 2) + e.Node.FullPath.Substring(3); // 선택된 노드 경로

                listView1.Items.Clear(); // 기존 정보 초기화

                di = new DirectoryInfo(e.Node.FullPath);
                diArray = di.GetDirectories();

                foreach (DirectoryInfo tdis in diArray)
                {
                    item = listView1.Items.Add(tdis.Name);
                    item.SubItems.Add(""); // 디렉터리는 크기 없음
                    item.SubItems.Add(tdis.LastWriteTime.ToString());
                    item.ImageIndex = 0;
                    item.Tag = "D";

                    string path = textBox1.Text + "\\" + tdis.Name;

                    if (priorityInfo.ContainsKey(path))
                        item.SubItems.Add(priorityInfo[path].ToString());
                    else
                        item.SubItems.Add("0"); // 기본 우선순위

                    // 우선순위 항목은 노란색으로 강조 표시
                    if (priorityInfo.ContainsKey(path) && priorityInfo[path] > 0)
                        item.BackColor = Color.Yellow;

                }

                fiArray = di.GetFiles();

                foreach (FileInfo fis in fiArray)
                {
                    item = listView1.Items.Add(fis.Name);
                    item.SubItems.Add(fis.Length.ToString());
                    item.SubItems.Add(fis.LastWriteTime.ToString());
                    item.ImageIndex = 1;
                    item.Tag = "F";

                    string path = textBox1.Text + "\\" + fis.Name;

                    if (priorityInfo.ContainsKey(path))
                        item.SubItems.Add(priorityInfo[path].ToString());
                    else
                        item.SubItems.Add("0"); // 기본 우선순위

                    // 우선순위 항목은 노란색으로 강조 표시
                    if (priorityInfo.ContainsKey(path) && priorityInfo[path] > 0)
                        item.BackColor = Color.Yellow;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            OpenFiles();
        }

        private void 열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFiles();
        }

        private void mnuView_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            자세히ToolStripMenuItem.Checked = false;
            간단히ToolStripMenuItem.Checked = false;
            작은아이콘ToolStripMenuItem.Checked = false;
            큰아이콘ToolStripMenuItem.Checked = false;

            switch (item.Text)
            {
                case "자세히":
                    자세히ToolStripMenuItem.Checked = true;
                    listView1.View = View.Details;
                    break;
                case "간단히":
                    간단히ToolStripMenuItem.Checked = true;
                    listView1.View = View.List;
                    break;
                case "작은 아이콘":
                    작은아이콘ToolStripMenuItem.Checked = true;
                    listView1.View = View.SmallIcon;
                    break;
                case "큰 아이콘":
                    큰아이콘ToolStripMenuItem.Checked = true;
                    listView1.View = View.LargeIcon;
                    break;
            }
        }

        private void 우선순위설정ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];

                // 간단하게 InputBox로 숫자 입력받기
                string input = Microsoft.VisualBasic.Interaction.InputBox(
                    "우선순위를 입력하세요. (0~100)",
                    "우선순위 설정",
                    selectedItem.SubItems.Count > 3 ? selectedItem.SubItems[3].Text : "0");

                // 취소를 누르거나 아무것도 입력하지 않고 확인을 눌렀을 때 그냥 종료
                if (input == "")
                    return;

                int priority;

                if (int.TryParse(input, out priority))
                {
                    if (priority >= 0 && priority <= 100)
                    {
                        if (selectedItem.SubItems.Count < 4)
                            selectedItem.SubItems.Add(priority.ToString());
                        else
                            selectedItem.SubItems[3].Text = priority.ToString();

                        string fullPath = textBox1.Text + "\\" + selectedItem.Text;
                        priorityInfo[fullPath] = priority;

                        // 우선순위 설정 및 수정 시 강조 표시 즉시 적용
                        if (priority > 0)
                            selectedItem.BackColor = Color.Yellow; // 우선순위 항목은 노란색으로 강조 표시
                        else
                            selectedItem.BackColor = Color.White; // 우선순위 0이면 강조 해제

                        // 우선 순위 설정 시 정렬 즉시 적용
                        listView1.ListViewItemSorter = new ListViewItemComparer(sortColumn, sortAscending); 
                        listView1.Sort();
                    }
                    else
                    {
                        MessageBox.Show("잘못 입력하셨습니다.");
                    }
                }
                else
                {
                    MessageBox.Show("잘못 입력하셨습니다.");
                }
            }

        }
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // 같은 열을 다시 클릭하면 정렬 방향을 뒤집고,
            // 다른 열을 클릭하면 해당 열을 기준으로 오름차순 시작
            if (e.Column == sortColumn)
                sortAscending = !sortAscending;
            else
            {
                sortColumn = e.Column;
                sortAscending = true;
            }

            listView1.ListViewItemSorter = new ListViewItemComparer(sortColumn, sortAscending);
            listView1.Sort();
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                string path = textBox1.Text.Trim();

                if (!Directory.Exists(path))
                {
                    MessageBox.Show("해당 경로가 존재하지 않습니다.");
                    return;
                }
                if (!SelectNodeByPath(path))
                    RefreshListView(path);
            }
        }
        private bool SelectNodeByPath(string fullPath)
        {
            string[] parts = fullPath.TrimEnd('\\').Split('\\');
            if (parts.Length == 0) return false;
            TreeNode current = treeView1.Nodes
                .Cast<TreeNode>()
                .FirstOrDefault(n => n.Text.StartsWith(parts[0],
                         StringComparison.OrdinalIgnoreCase));

            if (current == null) return false;

            treeView1.SelectedNode = current;
            for (int i = 1; i < parts.Length; i++)
            {
                current.Expand();
                current = current.Nodes
                    .Cast<TreeNode>()
                    .FirstOrDefault(n => n.Text.Equals(parts[i],
                         StringComparison.OrdinalIgnoreCase));

                if (current == null) return false;
                treeView1.SelectedNode = current;
            }

            current.EnsureVisible();
            treeView1.Focus();
            return true;
        }
        private void RefreshListView(string path)
        {
            try
            {
                listView1.Items.Clear();
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (var dir in di.GetDirectories())
                {
                    var item = listView1.Items.Add(dir.Name);
                    item.SubItems.Add("");
                    item.SubItems.Add(dir.LastWriteTime.ToString());
                    item.ImageIndex = 0;
                    item.Tag = "D";

                    string full = path + "\\" + dir.Name;
                    if (priorityInfo.ContainsKey(full))
                    {
                        item.SubItems.Add(priorityInfo[full].ToString());
                        if (priorityInfo[full] > 0) item.BackColor = Color.Yellow;
                    }
                    else item.SubItems.Add("0");
                }
                foreach (var file in di.GetFiles())
                {
                    var item = listView1.Items.Add(file.Name);
                    item.SubItems.Add(file.Length.ToString());
                    item.SubItems.Add(file.LastWriteTime.ToString());
                    item.ImageIndex = 1;
                    item.Tag = "F";

                    string full = path + "\\" + file.Name;
                    if (priorityInfo.ContainsKey(full))
                    {
                        item.SubItems.Add(priorityInfo[full].ToString());
                        if (priorityInfo[full] > 0) item.BackColor = Color.Yellow;
                    }
                    else item.SubItems.Add("0");
                }
                textBox1.Text = path;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

class ListViewItemComparer : IComparer
{
    private readonly int col;
    private readonly bool ascending;

    public ListViewItemComparer(int column, bool asc)
    {
        col = column;
        ascending = asc;
    }

    public int Compare(object x, object y)
    {
        var a = x as ListViewItem;
        var b = y as ListViewItem;

        int priA = 0, priB = 0;
        if (a.SubItems.Count >= 4)
            int.TryParse(a.SubItems[3].Text, out priA);
        if (b.SubItems.Count >= 4)
            int.TryParse(b.SubItems[3].Text, out priB);

        int result = priB.CompareTo(priA); // 우선순위는 내림차순으로 정렬
        if (result != 0) return result; // 우선순위가 다르면 그걸로 정렬

        // 선택된 열이 없을 경우 우선순위만 적용
        if (col == -1)
            return 0;

        switch (col)
        {
            case 1: // 크기
                long sizeA = 0, sizeB = 0;
                long.TryParse(a.SubItems[col].Text, out sizeA);
                long.TryParse(b.SubItems[col].Text, out sizeB);
                result = sizeA.CompareTo(sizeB);
                break;

            case 2: // 수정 시각
                DateTime timeA, timeB;
                DateTime.TryParse(a.SubItems[col].Text, out timeA);
                DateTime.TryParse(b.SubItems[col].Text, out timeB);
                result = timeA.CompareTo(timeB);
                break;

            default: // 이름 (대/소문자 무시)
                result = String.Compare(
                    a.SubItems[col].Text,
                    b.SubItems[col].Text,
                    StringComparison.CurrentCultureIgnoreCase);
                break;
        }
        return ascending ? result : -result; // 내림차순일 땐 부호 반전
    }
}
