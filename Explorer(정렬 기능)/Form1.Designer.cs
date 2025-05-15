namespace Explorer
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.PriorityOnOff = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.colorFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colorFileSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colorFileDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.열기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.자세히ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.간단히ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.작은아이콘ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.큰아이콘ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.우선순위설정ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.PriorityOnOff);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1723, 94);
            this.panel1.TabIndex = 0;
            // 
            // PriorityOnOff
            // 
            this.PriorityOnOff.Location = new System.Drawing.Point(1224, 24);
            this.PriorityOnOff.Name = "PriorityOnOff";
            this.PriorityOnOff.Size = new System.Drawing.Size(157, 47);
            this.PriorityOnOff.TabIndex = 2;
            this.PriorityOnOff.Text = "Priority OFF";
            this.PriorityOnOff.UseVisualStyleBackColor = true;
            this.PriorityOnOff.Click += new System.EventHandler(this.PriorityOnOff_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(139, 24);
            this.textBox1.Margin = new System.Windows.Forms.Padding(6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(1051, 35);
            this.textBox1.TabIndex = 1;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "전체 경로";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.listView1);
            this.panel2.Controls.Add(this.splitter1);
            this.panel2.Controls.Add(this.treeView1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 94);
            this.panel2.Margin = new System.Windows.Forms.Padding(6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1723, 990);
            this.panel2.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colorFileName,
            this.colorFileSize,
            this.colorFileDate});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HideSelection = false;
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(227, 0);
            this.listView1.Margin = new System.Windows.Forms.Padding(6);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1496, 990);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // colorFileName
            // 
            this.colorFileName.Text = "이름";
            // 
            // colorFileSize
            // 
            this.colorFileSize.Text = "크기";
            // 
            // colorFileDate
            // 
            this.colorFileDate.Text = "날짜";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.열기ToolStripMenuItem,
            this.자세히ToolStripMenuItem,
            this.간단히ToolStripMenuItem,
            this.작은아이콘ToolStripMenuItem,
            this.큰아이콘ToolStripMenuItem,
            this.우선순위설정ToolStripMenuItem});
            this.contextMenuStrip1.Margin = new System.Windows.Forms.Padding(1);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowCheckMargin = true;
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(241, 232);
            // 
            // 열기ToolStripMenuItem
            // 
            this.열기ToolStripMenuItem.Name = "열기ToolStripMenuItem";
            this.열기ToolStripMenuItem.Size = new System.Drawing.Size(300, 38);
            this.열기ToolStripMenuItem.Text = "열기";
            this.열기ToolStripMenuItem.Click += new System.EventHandler(this.열기ToolStripMenuItem_Click);
            // 
            // 자세히ToolStripMenuItem
            // 
            this.자세히ToolStripMenuItem.Name = "자세히ToolStripMenuItem";
            this.자세히ToolStripMenuItem.Size = new System.Drawing.Size(300, 38);
            this.자세히ToolStripMenuItem.Text = "자세히";
            this.자세히ToolStripMenuItem.Click += new System.EventHandler(this.mnuView_Click);
            // 
            // 간단히ToolStripMenuItem
            // 
            this.간단히ToolStripMenuItem.Name = "간단히ToolStripMenuItem";
            this.간단히ToolStripMenuItem.Size = new System.Drawing.Size(300, 38);
            this.간단히ToolStripMenuItem.Text = "간단히";
            this.간단히ToolStripMenuItem.Click += new System.EventHandler(this.mnuView_Click);
            // 
            // 작은아이콘ToolStripMenuItem
            // 
            this.작은아이콘ToolStripMenuItem.Name = "작은아이콘ToolStripMenuItem";
            this.작은아이콘ToolStripMenuItem.Size = new System.Drawing.Size(300, 38);
            this.작은아이콘ToolStripMenuItem.Text = "작은 아이콘";
            this.작은아이콘ToolStripMenuItem.Click += new System.EventHandler(this.mnuView_Click);
            // 
            // 큰아이콘ToolStripMenuItem
            // 
            this.큰아이콘ToolStripMenuItem.Name = "큰아이콘ToolStripMenuItem";
            this.큰아이콘ToolStripMenuItem.Size = new System.Drawing.Size(300, 38);
            this.큰아이콘ToolStripMenuItem.Text = "큰 아이콘";
            this.큰아이콘ToolStripMenuItem.Click += new System.EventHandler(this.mnuView_Click);
            // 
            // 우선순위설정ToolStripMenuItem
            // 
            this.우선순위설정ToolStripMenuItem.Name = "우선순위설정ToolStripMenuItem";
            this.우선순위설정ToolStripMenuItem.Size = new System.Drawing.Size(300, 38);
            this.우선순위설정ToolStripMenuItem.Text = "우선순위 설정";
            this.우선순위설정ToolStripMenuItem.Click += new System.EventHandler(this.우선순위설정ToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "mountain.jpg");
            this.imageList1.Images.SetKeyName(1, "blossom.jpg");
            this.imageList1.Images.SetKeyName(2, "sea.jpg");
            this.imageList1.Images.SetKeyName(3, "sky.jpg");
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(221, 0);
            this.splitter1.Margin = new System.Windows.Forms.Padding(6);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(6, 990);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(6);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(221, 990);
            this.treeView1.TabIndex = 1;
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.treeView1.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeSelect);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1723, 1084);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colorFileName;
        private System.Windows.Forms.ColumnHeader colorFileSize;
        private System.Windows.Forms.ColumnHeader colorFileDate;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 열기ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 자세히ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 간단히ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 작은아이콘ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 큰아이콘ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 우선순위설정ToolStripMenuItem;
        private System.Windows.Forms.Button PriorityOnOff;
    }
}

