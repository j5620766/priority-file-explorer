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

namespace priority_file_explorer_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form_DragEnter);
            this.DragDrop += new DragEventHandler(Form_DragDrop);

            

            InitializeComponent();
        }

        private void Form_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string filePath in files)
            {
                
            }
        }

        private void FlowLayoutPanel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void FlowLayoutPanel1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string file in files)
            {
                // 1. 썸네일용 PictureBox
                PictureBox pb = new PictureBox();
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Size = new Size(64, 64);
                pb.Margin = new Padding(0, 0, 0, 5);
                pb.Tag = file;

                string ext = Path.GetExtension(file).ToLower();

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
                else
                {
                    pb.Image = Icon.ExtractAssociatedIcon(file).ToBitmap();
                }

                // 2. 파일 이름용 Label
                Label lbl = new Label();
                lbl.Text = Path.GetFileName(file);
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.AutoSize = false;
                lbl.Width = 64;
                lbl.Height = 30;
                lbl.MaximumSize = new Size(64, 40);
                lbl.Font = new Font("맑은 고딕", 8);
                lbl.ForeColor = Color.Black;

                // 3. PictureBox + Label을 담을 Panel 생성
                Panel filePanel = new Panel();
                filePanel.Width = 70;
                filePanel.Height = 100;
                filePanel.Margin = new Padding(5);
                filePanel.Controls.Add(pb);
                filePanel.Controls.Add(lbl);

                // 위치 조정
                pb.Location = new Point(3, 0);
                lbl.Location = new Point(0, 68);

                EventHandler MouseDoubleClick = (s, ev) =>
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
                };

                // 클릭 시 배경색 변경
                EventHandler clickHandler = (s, ev) =>
                {
                    try
                    {
                        // 클릭 시 배경색 변경 (파란색)
                        if (filePanel.BackColor == Color.LightBlue)
                        {
                            filePanel.BackColor = Color.Transparent;  // 원래 색으로 돌아감
                        }
                        else
                        {
                            filePanel.BackColor = Color.LightBlue;  // 파란색으로 변경
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("파일 실행 실패: " + ex.Message);
                    }
                };

                // AddClickHandler 부분 제거 (이미 클릭 이벤트는 등록되어 있음)
                flowLayoutPanel1.Controls.Add(filePanel);

                AddDoubleClickHandler(filePanel, MouseDoubleClick);
                AddClickHandler(filePanel, clickHandler);
                

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
    }
}