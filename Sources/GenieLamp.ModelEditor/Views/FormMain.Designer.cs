namespace GenieLamp.ModelEditor.Views
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.actionListMain = new CDiese.Actions.ActionList(this.components);
            this.actionFileNew = new CDiese.Actions.Action(this.components);
            this.actionFileOpen = new CDiese.Actions.Action(this.components);
            this.actionFileExit = new CDiese.Actions.Action(this.components);
            this.imageListMain = new System.Windows.Forms.ImageList(this.components);
            this.dlgOpenProject = new System.Windows.Forms.OpenFileDialog();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.tabControlBottom = new System.Windows.Forms.TabControl();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.edtOutput = new System.Windows.Forms.TextBox();
            this.menuStripMain.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.tabControlBottom.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(1009, 26);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripSeparator1,
            this.toolStripMenuItem3});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(40, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // toolStripMenuItem1
            // 
            this.actionListMain.SetAction(this.toolStripMenuItem1, this.actionFileNew);
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(126, 22);
            this.toolStripMenuItem1.Text = "New";
            // 
            // toolStripMenuItem2
            // 
            this.actionListMain.SetAction(this.toolStripMenuItem2, this.actionFileOpen);
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(126, 22);
            this.toolStripMenuItem2.Text = "Open...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(123, 6);
            // 
            // toolStripMenuItem3
            // 
            this.actionListMain.SetAction(this.toolStripMenuItem3, this.actionFileExit);
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(126, 22);
            this.toolStripMenuItem3.Text = "Exit";
            // 
            // statusStripMain
            // 
            this.statusStripMain.Location = new System.Drawing.Point(0, 674);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(1009, 22);
            this.statusStripMain.TabIndex = 1;
            this.statusStripMain.Text = "statusStrip1";
            // 
            // toolStripMain
            // 
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.Location = new System.Drawing.Point(0, 26);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Size = new System.Drawing.Size(1009, 25);
            this.toolStripMain.TabIndex = 2;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // actionListMain
            // 
            this.actionListMain.Actions.AddRange(new CDiese.Actions.Action[] {
            this.actionFileNew,
            this.actionFileOpen,
            this.actionFileExit});
            this.actionListMain.ImageList = this.imageListMain;
            this.actionListMain.ShowTextOnToolBar = false;
            this.actionListMain.Tag = null;
            // 
            // actionFileNew
            // 
            this.actionFileNew.Checked = false;
            this.actionFileNew.Enabled = true;
            this.actionFileNew.Hint = null;
            this.actionFileNew.ImageIndex = 0;
            this.actionFileNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.actionFileNew.Tag = null;
            this.actionFileNew.Text = "New";
            this.actionFileNew.Visible = true;
            // 
            // actionFileOpen
            // 
            this.actionFileOpen.Checked = false;
            this.actionFileOpen.Enabled = true;
            this.actionFileOpen.Hint = null;
            this.actionFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.actionFileOpen.Tag = null;
            this.actionFileOpen.Text = "Open...";
            this.actionFileOpen.Visible = true;
            this.actionFileOpen.Execute += new System.EventHandler(this.actionFileOpen_Execute);
            // 
            // actionFileExit
            // 
            this.actionFileExit.Checked = false;
            this.actionFileExit.Enabled = true;
            this.actionFileExit.Hint = null;
            this.actionFileExit.Shortcut = System.Windows.Forms.Shortcut.None;
            this.actionFileExit.Tag = null;
            this.actionFileExit.Text = "Exit";
            this.actionFileExit.Visible = true;
            this.actionFileExit.Execute += new System.EventHandler(this.actionFileExit_Execute);
            // 
            // imageListMain
            // 
            this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
            this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListMain.Images.SetKeyName(0, "Action-file-new-icon.png");
            // 
            // dlgOpenProject
            // 
            this.dlgOpenProject.Filter = "GL project files|*.xml";
            // 
            // panelMain
            // 
            this.panelMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelMain.Controls.Add(this.panelRight);
            this.panelMain.Controls.Add(this.splitter1);
            this.panelMain.Controls.Add(this.panelLeft);
            this.panelMain.Controls.Add(this.splitter2);
            this.panelMain.Controls.Add(this.panelBottom);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 51);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1009, 623);
            this.panelMain.TabIndex = 6;
            // 
            // panelRight
            // 
            this.panelRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(284, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(721, 463);
            this.panelRight.TabIndex = 9;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(281, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 463);
            this.splitter1.TabIndex = 8;
            this.splitter1.TabStop = false;
            // 
            // panelLeft
            // 
            this.panelLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(281, 463);
            this.panelLeft.TabIndex = 7;
            // 
            // splitter2
            // 
            this.splitter2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 463);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(1005, 3);
            this.splitter2.TabIndex = 1;
            this.splitter2.TabStop = false;
            // 
            // panelBottom
            // 
            this.panelBottom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelBottom.Controls.Add(this.tabControlBottom);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 466);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1005, 153);
            this.panelBottom.TabIndex = 0;
            // 
            // tabControlBottom
            // 
            this.tabControlBottom.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControlBottom.Controls.Add(this.tabLog);
            this.tabControlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlBottom.Location = new System.Drawing.Point(0, 0);
            this.tabControlBottom.Name = "tabControlBottom";
            this.tabControlBottom.Padding = new System.Drawing.Point(0, 0);
            this.tabControlBottom.SelectedIndex = 0;
            this.tabControlBottom.Size = new System.Drawing.Size(1001, 149);
            this.tabControlBottom.TabIndex = 0;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.edtOutput);
            this.tabLog.Location = new System.Drawing.Point(4, 4);
            this.tabLog.Name = "tabLog";
            this.tabLog.Size = new System.Drawing.Size(993, 120);
            this.tabLog.TabIndex = 0;
            this.tabLog.Text = "Output";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // edtOutput
            // 
            this.edtOutput.BackColor = System.Drawing.SystemColors.Window;
            this.edtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.edtOutput.Location = new System.Drawing.Point(0, 0);
            this.edtOutput.Multiline = true;
            this.edtOutput.Name = "edtOutput";
            this.edtOutput.ReadOnly = true;
            this.edtOutput.Size = new System.Drawing.Size(993, 120);
            this.edtOutput.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1009, 696);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "FormMain";
            this.Text = "Genie Lamp model editor";
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.tabControlBottom.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private CDiese.Actions.Action actionFileNew;
        private CDiese.Actions.Action actionFileOpen;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private CDiese.Actions.Action actionFileExit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ImageList imageListMain;
        private CDiese.Actions.ActionList actionListMain;
        private System.Windows.Forms.OpenFileDialog dlgOpenProject;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.TabControl tabControlBottom;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.TextBox edtOutput;
    }
}

