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
using static System.Net.WebRequestMethods;

namespace priority_file_explorer_
{
    public partial class Form1 : Form
    {
        private Stack<string> pathHistory = new Stack<string>();
        private string currentPath = "";
        private Panel selectedPanel = null;
        public Form1()
        {
            InitializeComponent();
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

            foreach (string path in paths)
            {
                if (System.IO.File.Exists(path) || Directory.Exists(path))
                {
                    flowLayoutPanel1.Controls.Add(CreateFilePanel(path));
                }
            }
        }

        void AddClickHandler(Control parent, EventHandler handler)
        {
            parent.Click += handler;  // 부모 컨트롤에도 클릭 이벤트 추가

            foreach (Control child in parent.Controls)
            {
                child.Click += handler;  // 자식 컨트롤에도 클릭 이벤트 추가
            }
        }
        void AddDoubleClickHandler(Control parent, EventHandler handler)
        {
            parent.DoubleClick += handler;  // 부모 컨트롤에도 클릭 이벤트 추가

            foreach (Control child in parent.Controls)
            {
                child.DoubleClick += handler;  // 자식 컨트롤에도 클릭 이벤트 추가
            }
        }

        // FileDialog를 열어 파일 추가
        private void 파일추가ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "파일 열기";
            openFileDialog.Filter = "모든 파일 (*.*)|*.*"; // 파일 필터 설정 (예: 텍스트 파일만 보여주고 싶으면 "텍스트 파일 (*.txt)|*.txt")
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] files = openFileDialog.FileNames;

                foreach (string file in files)
                {
                    flowLayoutPanel1.Controls.Add(CreateFilePanel(file));

                }
            }
        }

        // 패널에 파일을 생성
        private Panel CreateFilePanel(string file)
        {
            PictureBox pb = CreateThumbnail(file);
            Label lbl = CreateFileLabel(file);

            Panel panel = new Panel();

            panel.Width = 70;
            panel.Height = 100;
            panel.Margin = new Padding(5);

            pb.Location = new Point(3, 0);
            lbl.Location = new Point(0, 68);

            panel.Controls.Add(pb);
            panel.Controls.Add(lbl);

            
            // 더블클릭 시 파일 실행
            EventHandler doubleClickHandler = (s, e) =>
            {
                string path = (string)((Control)s).Tag;

                if (Directory.Exists(path))  // 폴더일 경우
                {
                    NavigateToFolder(path); // 🔥 내부 탐색 함수 호출
                }
                else if (System.IO.File.Exists(path))  // 파일일 경우
                {
                    try
                    {
                        var psi = new ProcessStartInfo(file)
                        {
                            UseShellExecute = true
                        };
                        Process.Start(psi);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("파일 실행 실패: " + ex.Message);
                    }
                }
            };

            // 파일 클릭 시 해당 파일 색깔 변경
            EventHandler clickHandler = (s, ev) =>
            {
                try
                {
                    Control clickedControl = (Control)s;

                    Panel parentPanel = clickedControl as Panel ?? clickedControl.Parent as Panel;

                    if (parentPanel == null)
                        return;

                    if (selectedPanel != null && selectedPanel != parentPanel)  // 이전에 클릭한 파일 색깔 복원
                    {
                        selectedPanel.BackColor = Color.Transparent;
                    }

                    parentPanel.BackColor = Color.LightBlue;
                    selectedPanel = parentPanel;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("클릭 실패: " + ex.Message);
                }
            };
            AddDoubleClickHandler(panel, doubleClickHandler);
            AddClickHandler(panel, clickHandler);

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
        private Label CreateFileLabel(string file)
        {
            Label lbl = new Label();
            lbl.Text = Path.GetFileName(file);
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.AutoSize = false;
            lbl.Width = 64;
            lbl.Height = 30;
            lbl.MaximumSize = new Size(64, 40);
            lbl.Font = new Font("맑은 고딕", 8);
            lbl.ForeColor = Color.Black;
            return lbl;
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
    }
}