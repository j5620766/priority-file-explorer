﻿using System;
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

namespace Explorer
{
    public partial class Form1 : Form
    {
        Dictionary<string, int> priorityInfo = new Dictionary<string, int>();

        bool is_highlighted;

        string priorityFilePath = "priority.txt"; // bin\Debug에 위치
        string highlightFilePath = "highlight.txt"; // bin\Debug에 위치

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

            this.KeyPreview = true; // 키 입력 감지를 위해 활성화

            LoadPriorityData();
            LoadHighlightInfo();
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
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SavePriorityData();
            SaveHighlightInfo();
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

        private void SaveHighlightInfo()
        {
            File.WriteAllText(highlightFilePath, is_highlighted.ToString());
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

                    PriorityHighlight();

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

                    PriorityHighlight();
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
            if (listView1.SelectedItems.Count == 1)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];

                // 간단하게 InputBox로 숫자 입력받기
                string input = Microsoft.VisualBasic.Interaction.InputBox(
                    "우선순위를 입력하세요. (0~100)",
                    "우선순위 설정",
                    selectedItem.SubItems.Count >= 4 ? selectedItem.SubItems[3].Text : "0");

                // 취소를 누르거나 아무것도 입력하지 않고 확인을 눌렀을 때 그냥 종료
                if (input == "")
                    return;

                if (int.TryParse(input, out int priority))
                {
                    if (priority >= 0 && priority <= 100)
                    {
                        if (selectedItem.SubItems.Count < 4)
                            selectedItem.SubItems.Add(priority.ToString());
                        else
                            selectedItem.SubItems[3].Text = priority.ToString();

                        string fullPath = textBox1.Text + "\\" + selectedItem.Text;
                        priorityInfo[fullPath] = priority;

                        selectedItem.UseItemStyleForSubItems = false;

                        // 우선순위 설정 및 수정 시 강조 표시 즉시 적용
                        if (is_highlighted && priority > 0)
                            foreach (ListViewItem.ListViewSubItem sub in selectedItem.SubItems)
                                sub.BackColor = Color.Yellow; // 우선순위 항목은 노란색으로 강조 표시
                        else
                            foreach (ListViewItem.ListViewSubItem sub in selectedItem.SubItems)
                                sub.BackColor = Color.White; // 우선순위 0이면 강조 해제
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

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // 파일 선택 후 P 키를 누르면 즉시 우선순위 설정 가능
            if (e.KeyCode == Keys.P)
                if (listView1.Focused || listView1.ContainsFocus)
                    우선순위설정ToolStripMenuItem_Click(sender, e);
        }

        private void 우선순위강조ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            item.Checked = !item.Checked;
            is_highlighted = item.Checked;

            PriorityHighlight();
        }

        private void PriorityHighlight()
        {
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.SubItems.Count >= 4)
                {
                    if (int.TryParse(item.SubItems[3].Text, out int priority))
                    {
                        item.UseItemStyleForSubItems = false;

                        if (is_highlighted && priority > 0)
                            foreach (ListViewItem.ListViewSubItem sub in item.SubItems)
                                sub.BackColor = Color.Yellow;
                        else
                            foreach (ListViewItem.ListViewSubItem sub in item.SubItems)
                                sub.BackColor = Color.White;
                    }
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

                File.WriteAllText(priorityFilePath, ""); // "priorty.txt" 파일 초기화

                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.SubItems.Count >= 4)
                        item.SubItems[3].Text = "0"; // 우선순위 초기화
                    else
                        item.SubItems.Add("0");

                    item.UseItemStyleForSubItems = false;

                    foreach (ListViewItem.ListViewSubItem sub in item.SubItems)
                        sub.BackColor = Color.White; // 우선순위 강조 초기화
                }
            }
        }
    }
}
