﻿namespace priority_file_explorer_
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.파일ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.파일ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.파일추가ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btn_back = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.삭제ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.우선순위설정ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.우선순위강조ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.우선순위초기화ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.rightClickMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Window;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.파일ToolStripMenuItem,
            this.파일ToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1935, 40);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 파일ToolStripMenuItem
            // 
            this.파일ToolStripMenuItem.Name = "파일ToolStripMenuItem";
            this.파일ToolStripMenuItem.Size = new System.Drawing.Size(90, 36);
            this.파일ToolStripMenuItem.Text = "       ";
            // 
            // 파일ToolStripMenuItem1
            // 
            this.파일ToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.파일추가ToolStripMenuItem});
            this.파일ToolStripMenuItem1.Name = "파일ToolStripMenuItem1";
            this.파일ToolStripMenuItem1.Size = new System.Drawing.Size(82, 36);
            this.파일ToolStripMenuItem1.Text = "파일";
            // 
            // 파일추가ToolStripMenuItem
            // 
            this.파일추가ToolStripMenuItem.Name = "파일추가ToolStripMenuItem";
            this.파일추가ToolStripMenuItem.Size = new System.Drawing.Size(251, 44);
            this.파일추가ToolStripMenuItem.Text = "파일 추가";
            this.파일추가ToolStripMenuItem.Click += new System.EventHandler(this.파일추가ToolStripMenuItem_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AllowDrop = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.Window;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 90);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1935, 1078);
            this.flowLayoutPanel1.TabIndex = 2;
            this.flowLayoutPanel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.FlowLayoutPanel1_DragDrop);
            this.flowLayoutPanel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.FlowLayoutPanel1_DragEnter);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btn_back
            // 
            this.btn_back.Location = new System.Drawing.Point(0, 0);
            this.btn_back.Margin = new System.Windows.Forms.Padding(4);
            this.btn_back.Name = "btn_back";
            this.btn_back.Size = new System.Drawing.Size(97, 44);
            this.btn_back.TabIndex = 0;
            this.btn_back.Text = "←";
            this.btn_back.UseVisualStyleBackColor = true;
            this.btn_back.Click += new System.EventHandler(this.btn_back_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 52);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1935, 46);
            this.panel1.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1432, 10);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 24);
            this.label5.TabIndex = 5;
            this.label5.Text = "우선순위";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1209, 10);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 24);
            this.label4.TabIndex = 4;
            this.label4.Text = "크기";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(877, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 24);
            this.label3.TabIndex = 3;
            this.label3.Text = "유형";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(574, 10);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "수정한 날짜";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "이름";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // rightClickMenu
            // 
            this.rightClickMenu.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.rightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.삭제ToolStripMenuItem,
            this.우선순위설정ToolStripMenuItem,
            this.우선순위강조ToolStripMenuItem,
            this.우선순위초기화ToolStripMenuItem});
            this.rightClickMenu.Margin = new System.Windows.Forms.Padding(1);
            this.rightClickMenu.Name = "rightClickMenu";
            this.rightClickMenu.ShowCheckMargin = true;
            this.rightClickMenu.ShowImageMargin = false;
            this.rightClickMenu.Size = new System.Drawing.Size(265, 156);
            // 
            // 삭제ToolStripMenuItem
            // 
            this.삭제ToolStripMenuItem.Name = "삭제ToolStripMenuItem";
            this.삭제ToolStripMenuItem.Size = new System.Drawing.Size(264, 38);
            this.삭제ToolStripMenuItem.Text = "삭제";
            this.삭제ToolStripMenuItem.Click += new System.EventHandler(this.삭제ToolStripMenuItem_Click);
            // 
            // 우선순위설정ToolStripMenuItem
            // 
            this.우선순위설정ToolStripMenuItem.Name = "우선순위설정ToolStripMenuItem";
            this.우선순위설정ToolStripMenuItem.Size = new System.Drawing.Size(264, 38);
            this.우선순위설정ToolStripMenuItem.Text = "우선순위 설정";
            this.우선순위설정ToolStripMenuItem.Click += new System.EventHandler(this.우선순위설정ToolStripMenuItem_Click);
            // 
            // 우선순위강조ToolStripMenuItem
            // 
            this.우선순위강조ToolStripMenuItem.Name = "우선순위강조ToolStripMenuItem";
            this.우선순위강조ToolStripMenuItem.Size = new System.Drawing.Size(264, 38);
            this.우선순위강조ToolStripMenuItem.Text = "우선순위 강조";
            this.우선순위강조ToolStripMenuItem.Click += new System.EventHandler(this.우선순위강조ToolStripMenuItem_Click);
            // 
            // 우선순위초기화ToolStripMenuItem
            // 
            this.우선순위초기화ToolStripMenuItem.Name = "우선순위초기화ToolStripMenuItem";
            this.우선순위초기화ToolStripMenuItem.Size = new System.Drawing.Size(264, 38);
            this.우선순위초기화ToolStripMenuItem.Text = "우선순위 초기화";
            this.우선순위초기화ToolStripMenuItem.Click += new System.EventHandler(this.우선순위초기화ToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1935, 1166);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btn_back);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.rightClickMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 파일ToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btn_back;
        private System.Windows.Forms.ToolStripMenuItem 파일ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 파일추가ToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ContextMenuStrip rightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem 삭제ToolStripMenuItem;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolStripMenuItem 우선순위설정ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 우선순위강조ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 우선순위초기화ToolStripMenuItem;
    }
}

