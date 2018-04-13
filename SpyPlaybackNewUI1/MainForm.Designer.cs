namespace BotsinaWPF
{
    partial class MainForm
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
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InspectorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AutomationId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Names = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnCreateTestSteps = new System.Windows.Forms.Button();
            this.btnQuickSave2 = new System.Windows.Forms.Button();
            this.btnPlayback2 = new System.Windows.Forms.Button();
            this.rtxtScript = new System.Windows.Forms.RichTextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnCreateTestScript = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnPlayback = new System.Windows.Forms.Button();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.TestSteps = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Index1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AutomationId1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Name1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Actions = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.InputValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnAttach = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ConsolePanelPush = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ResultPanelPush = new System.Windows.Forms.RichTextBox();
            this.clbTestScriptList = new System.Windows.Forms.CheckedListBox();
            this.btnMoveUpCLB = new System.Windows.Forms.Button();
            this.btnMoveDownCLB = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1297, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click_1);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.InspectorToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // InspectorToolStripMenuItem
            // 
            this.InspectorToolStripMenuItem.Name = "InspectorToolStripMenuItem";
            this.InspectorToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.InspectorToolStripMenuItem.Text = "Inspector";
            this.InspectorToolStripMenuItem.Click += new System.EventHandler(this.InspectorToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(6, 57);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(110, 23);
            this.button4.TabIndex = 5;
            this.button4.Text = "Remove";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(326, 73);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(122, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "Add";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.Index,
            this.AutomationId,
            this.Names,
            this.Type});
            this.dataGridView1.Location = new System.Drawing.Point(12, 149);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(452, 442);
            this.dataGridView1.TabIndex = 0;
            // 
            // Select
            // 
            this.Select.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            this.Select.Width = 43;
            // 
            // Index
            // 
            this.Index.HeaderText = "Index";
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            this.Index.Width = 53;
            // 
            // AutomationId
            // 
            this.AutomationId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.AutomationId.HeaderText = "AutomationId";
            this.AutomationId.Name = "AutomationId";
            this.AutomationId.ReadOnly = true;
            // 
            // Names
            // 
            this.Names.HeaderText = "Names";
            this.Names.Name = "Names";
            this.Names.ReadOnly = true;
            this.Names.Width = 120;
            // 
            // Type
            // 
            this.Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(626, 41);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(666, 552);
            this.tabControl1.TabIndex = 16;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnCreateTestSteps);
            this.tabPage2.Controls.Add(this.btnQuickSave2);
            this.tabPage2.Controls.Add(this.btnPlayback2);
            this.tabPage2.Controls.Add(this.rtxtScript);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(658, 526);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Playback Script";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnCreateTestSteps
            // 
            this.btnCreateTestSteps.Location = new System.Drawing.Point(112, 6);
            this.btnCreateTestSteps.Name = "btnCreateTestSteps";
            this.btnCreateTestSteps.Size = new System.Drawing.Size(100, 23);
            this.btnCreateTestSteps.TabIndex = 3;
            this.btnCreateTestSteps.Text = "Create Test Steps";
            this.btnCreateTestSteps.UseVisualStyleBackColor = true;
            this.btnCreateTestSteps.Click += new System.EventHandler(this.btnCreateTestSteps_Click);
            // 
            // btnQuickSave2
            // 
            this.btnQuickSave2.Location = new System.Drawing.Point(218, 6);
            this.btnQuickSave2.Name = "btnQuickSave2";
            this.btnQuickSave2.Size = new System.Drawing.Size(100, 23);
            this.btnQuickSave2.TabIndex = 2;
            this.btnQuickSave2.Text = "Quick Save";
            this.btnQuickSave2.UseVisualStyleBackColor = true;
            this.btnQuickSave2.Click += new System.EventHandler(this.btnQuickSave2_Click);
            // 
            // btnPlayback2
            // 
            this.btnPlayback2.Location = new System.Drawing.Point(6, 6);
            this.btnPlayback2.Name = "btnPlayback2";
            this.btnPlayback2.Size = new System.Drawing.Size(100, 23);
            this.btnPlayback2.TabIndex = 1;
            this.btnPlayback2.Text = "Playback";
            this.btnPlayback2.UseVisualStyleBackColor = true;
            this.btnPlayback2.Click += new System.EventHandler(this.btnPlayback2_Click);
            // 
            // rtxtScript
            // 
            this.rtxtScript.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtScript.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtxtScript.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtScript.Location = new System.Drawing.Point(6, 35);
            this.rtxtScript.Name = "rtxtScript";
            this.rtxtScript.Size = new System.Drawing.Size(719, 485);
            this.rtxtScript.TabIndex = 0;
            this.rtxtScript.Text = "";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnMoveUp);
            this.tabPage1.Controls.Add(this.btnCreateTestScript);
            this.tabPage1.Controls.Add(this.btnMoveDown);
            this.tabPage1.Controls.Add(this.btnPlayback);
            this.tabPage1.Controls.Add(this.dataGridView2);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(658, 526);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Playback Table";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveUp.Location = new System.Drawing.Point(426, 57);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(110, 23);
            this.btnMoveUp.TabIndex = 5;
            this.btnMoveUp.Text = "Move Up";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnCreateTestScript
            // 
            this.btnCreateTestScript.Location = new System.Drawing.Point(122, 6);
            this.btnCreateTestScript.Name = "btnCreateTestScript";
            this.btnCreateTestScript.Size = new System.Drawing.Size(110, 23);
            this.btnCreateTestScript.TabIndex = 4;
            this.btnCreateTestScript.Text = "Create Test Script";
            this.btnCreateTestScript.UseVisualStyleBackColor = true;
            this.btnCreateTestScript.Click += new System.EventHandler(this.btnCreateTestScript_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveDown.Location = new System.Drawing.Point(542, 57);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(110, 23);
            this.btnMoveDown.TabIndex = 6;
            this.btnMoveDown.Text = "Move Down";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnPlayback
            // 
            this.btnPlayback.Location = new System.Drawing.Point(6, 6);
            this.btnPlayback.Name = "btnPlayback";
            this.btnPlayback.Size = new System.Drawing.Size(110, 23);
            this.btnPlayback.TabIndex = 3;
            this.btnPlayback.Text = "Playback";
            this.btnPlayback.UseVisualStyleBackColor = true;
            this.btnPlayback.Click += new System.EventHandler(this.btnPlayback_Click);
            // 
            // dataGridView2
            // 
            this.dataGridView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TestSteps,
            this.Index1,
            this.AutomationId1,
            this.Name1,
            this.Type1,
            this.Actions,
            this.InputValue});
            this.dataGridView2.Location = new System.Drawing.Point(6, 86);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView2.Size = new System.Drawing.Size(646, 434);
            this.dataGridView2.TabIndex = 1;
            // 
            // TestSteps
            // 
            this.TestSteps.HeaderText = "Test Steps";
            this.TestSteps.Name = "TestSteps";
            // 
            // Index1
            // 
            this.Index1.HeaderText = "Index";
            this.Index1.Name = "Index1";
            this.Index1.Visible = false;
            // 
            // AutomationId1
            // 
            this.AutomationId1.HeaderText = "AutomationId";
            this.AutomationId1.Name = "AutomationId1";
            // 
            // Name1
            // 
            this.Name1.HeaderText = "Name";
            this.Name1.Name = "Name1";
            // 
            // Type1
            // 
            this.Type1.HeaderText = "Type";
            this.Type1.Name = "Type1";
            // 
            // Actions
            // 
            this.Actions.HeaderText = "Actions";
            this.Actions.Name = "Actions";
            // 
            // InputValue
            // 
            this.InputValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.InputValue.HeaderText = "Input Value";
            this.InputValue.Name = "InputValue";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnAttach);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(12, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(452, 102);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Spy";
            // 
            // btnAttach
            // 
            this.btnAttach.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAttach.Location = new System.Drawing.Point(291, 14);
            this.btnAttach.Name = "btnAttach";
            this.btnAttach.Size = new System.Drawing.Size(75, 23);
            this.btnAttach.TabIndex = 9;
            this.btnAttach.Text = "Attach";
            this.btnAttach.UseVisualStyleBackColor = true;
            this.btnAttach.Click += new System.EventHandler(this.btnAttach_Click);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(79, 16);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(200, 20);
            this.textBox2.TabIndex = 8;
   
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "AUT\'s Name";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "All",
            "Interactive Controls",
            "TextBox",
            "Button",
            "RadioButton",
            "ComboBox",
            "ComboBoxEdit",
            "DataGrid"});
            this.comboBox1.Location = new System.Drawing.Point(326, 47);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 6;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged_1);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(79, 46);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(200, 20);
            this.textBox1.TabIndex = 5;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged_1);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(288, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Type";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Search";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(373, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Spy";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.AutoSize = true;
            this.groupBox2.Controls.Add(this.ConsolePanelPush);
            this.groupBox2.Location = new System.Drawing.Point(12, 597);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(454, 141);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Console Panel";
            // 
            // ConsolePanelPush
            // 
            this.ConsolePanelPush.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConsolePanelPush.Location = new System.Drawing.Point(3, 16);
            this.ConsolePanelPush.Name = "ConsolePanelPush";
            this.ConsolePanelPush.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.ConsolePanelPush.Size = new System.Drawing.Size(448, 122);
            this.ConsolePanelPush.TabIndex = 0;
            this.ConsolePanelPush.Text = "";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.ResultPanelPush);
            this.groupBox3.Location = new System.Drawing.Point(470, 597);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(822, 141);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Result Panel";
            // 
            // ResultPanelPush
            // 
            this.ResultPanelPush.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultPanelPush.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResultPanelPush.Location = new System.Drawing.Point(3, 16);
            this.ResultPanelPush.Name = "ResultPanelPush";
            this.ResultPanelPush.Size = new System.Drawing.Size(816, 122);
            this.ResultPanelPush.TabIndex = 0;
            this.ResultPanelPush.Text = "";
            this.ResultPanelPush.TextChanged += new System.EventHandler(this.ResultPanelPush_TextChanged);
            // 
            // clbTestScriptList
            // 
            this.clbTestScriptList.FormattingEnabled = true;
            this.clbTestScriptList.Location = new System.Drawing.Point(468, 149);
            this.clbTestScriptList.Name = "clbTestScriptList";
            this.clbTestScriptList.Size = new System.Drawing.Size(154, 439);
            this.clbTestScriptList.TabIndex = 26;
            this.clbTestScriptList.SelectedIndexChanged += new System.EventHandler(this.clbTestScriptList_SelectedIndexChanged);
            // 
            // btnMoveUpCLB
            // 
            this.btnMoveUpCLB.Location = new System.Drawing.Point(468, 120);
            this.btnMoveUpCLB.Name = "btnMoveUpCLB";
            this.btnMoveUpCLB.Size = new System.Drawing.Size(75, 23);
            this.btnMoveUpCLB.TabIndex = 27;
            this.btnMoveUpCLB.Text = "Move Up";
            this.btnMoveUpCLB.UseVisualStyleBackColor = true;
            this.btnMoveUpCLB.Click += new System.EventHandler(this.btnMoveUpCLB_Click);
            // 
            // btnMoveDownCLB
            // 
            this.btnMoveDownCLB.Location = new System.Drawing.Point(547, 120);
            this.btnMoveDownCLB.Name = "btnMoveDownCLB";
            this.btnMoveDownCLB.Size = new System.Drawing.Size(75, 23);
            this.btnMoveDownCLB.TabIndex = 28;
            this.btnMoveDownCLB.Text = "Move Down";
            this.btnMoveDownCLB.UseVisualStyleBackColor = true;
            this.btnMoveDownCLB.Click += new System.EventHandler(this.btnMoveDownCLB_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(547, 63);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 29;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1297, 750);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnMoveDownCLB);
            this.Controls.Add(this.btnMoveUpCLB);
            this.Controls.Add(this.clbTestScriptList);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Spy & Playback";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem InspectorToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn AutomationId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Names;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnCreateTestScript;
        private System.Windows.Forms.Button btnPlayback;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewTextBoxColumn TestSteps;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index1;
        private System.Windows.Forms.DataGridViewTextBoxColumn AutomationId1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type1;
        private System.Windows.Forms.DataGridViewComboBoxColumn Actions;
        private System.Windows.Forms.DataGridViewTextBoxColumn InputValue;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnCreateTestSteps;
        private System.Windows.Forms.Button btnQuickSave2;
        private System.Windows.Forms.Button btnPlayback2;
        private System.Windows.Forms.RichTextBox rtxtScript;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAttach;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox ConsolePanelPush;
        private System.Windows.Forms.CheckedListBox clbTestScriptList;
        private System.Windows.Forms.Button btnMoveUpCLB;
        private System.Windows.Forms.Button btnMoveDownCLB;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RichTextBox ResultPanelPush;
    }
}

