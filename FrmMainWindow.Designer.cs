
namespace dbShowDepends
{
    partial class FrmMainWindow
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.cbConnection = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tscbDatabaseName = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tstbObjectName = new System.Windows.Forms.ToolStripTextBox();
            this.tsbFindObject = new System.Windows.Forms.ToolStripButton();
            this.tsbClearSearchText = new System.Windows.Forms.ToolStripButton();
            this.tsbSearchBySource = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeObj = new System.Windows.Forms.TreeView();
            this.objectTreeViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.pnlTreeObjLabel = new System.Windows.Forms.Panel();
            this.labelTreeObj = new System.Windows.Forms.Label();
            this.listBoxViewHistory = new System.Windows.Forms.ListBox();
            this.pnlViewHistoryLabel = new System.Windows.Forms.Panel();
            this.labelViewHistory = new System.Windows.Forms.Label();
            this.scintillaTextBox = new ScintillaNET.Scintilla();
            this.pnlSourceText = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.bsConnections = new System.Windows.Forms.BindingSource(this.components);
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.pnlTreeObjLabel.SuspendLayout();
            this.pnlViewHistoryLabel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scintillaTextBox)).BeginInit();
            this.pnlSourceText.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsConnections)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.actionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(942, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem,
            this.setupConnectionsToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.actionsToolStripMenuItem.Text = "Actions";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // setupConnectionsToolStripMenuItem
            // 
            this.setupConnectionsToolStripMenuItem.Name = "setupConnectionsToolStripMenuItem";
            this.setupConnectionsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.setupConnectionsToolStripMenuItem.Text = "Setup connections";
            this.setupConnectionsToolStripMenuItem.Click += new System.EventHandler(this.setupConnectionsToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel3,
            this.cbConnection,
            this.toolStripLabel1,
            this.tscbDatabaseName,
            this.toolStripLabel2,
            this.tstbObjectName,
            this.tsbFindObject,
            this.tsbClearSearchText,
            this.tsbSearchBySource});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(942, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(69, 22);
            this.toolStripLabel3.Text = "Connection";
            // 
            // cbConnection
            // 
            this.cbConnection.DropDownWidth = 250;
            this.cbConnection.Name = "cbConnection";
            this.cbConnection.Size = new System.Drawing.Size(121, 25);
            this.cbConnection.SelectedIndexChanged += new System.EventHandler(this.cbConnection_SelectedIndexChanged);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(55, 22);
            this.toolStripLabel1.Text = "Database";
            // 
            // tscbDatabaseName
            // 
            this.tscbDatabaseName.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.tscbDatabaseName.Name = "tscbDatabaseName";
            this.tscbDatabaseName.Size = new System.Drawing.Size(250, 25);
            this.tscbDatabaseName.DropDown += new System.EventHandler(this.tscbDatabaseName_DropDown);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(77, 22);
            this.toolStripLabel2.Text = "Object Name";
            // 
            // tstbObjectName
            // 
            this.tstbObjectName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tstbObjectName.Name = "tstbObjectName";
            this.tstbObjectName.Size = new System.Drawing.Size(250, 25);
            this.tstbObjectName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tstbObjectName_KeyDown);
            // 
            // tsbFindObject
            // 
            this.tsbFindObject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFindObject.Image = ((System.Drawing.Image)(resources.GetObject("tsbFindObject.Image")));
            this.tsbFindObject.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tsbFindObject.Name = "tsbFindObject";
            this.tsbFindObject.Size = new System.Drawing.Size(23, 22);
            this.tsbFindObject.Text = "Найти объекты";
            this.tsbFindObject.Click += new System.EventHandler(this.tsbFindObject_Click);
            // 
            // tsbClearSearchText
            // 
            this.tsbClearSearchText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClearSearchText.Image = global::dbShowDepends.Properties.Resources.btnClear;
            this.tsbClearSearchText.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tsbClearSearchText.Name = "tsbClearSearchText";
            this.tsbClearSearchText.Size = new System.Drawing.Size(23, 22);
            this.tsbClearSearchText.Text = "Очистить строку поиска";
            this.tsbClearSearchText.Click += new System.EventHandler(this.tsbClearSearchText_Click);
            // 
            // tsbSearchBySource
            // 
            this.tsbSearchBySource.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSearchBySource.Image = global::dbShowDepends.Properties.Resources.btnSearchBySource2;
            this.tsbSearchBySource.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tsbSearchBySource.Name = "tsbSearchBySource";
            this.tsbSearchBySource.Size = new System.Drawing.Size(23, 22);
            this.tsbSearchBySource.Text = "Искать в исходниках";
            this.tsbSearchBySource.Click += new System.EventHandler(this.tsbFindObject_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 418);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(942, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(45, 17);
            this.toolStripStatusLabel1.Text = "Готово";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 49);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.scintillaTextBox);
            this.splitContainer1.Panel2.Controls.Add(this.pnlSourceText);
            this.splitContainer1.Size = new System.Drawing.Size(942, 369);
            this.splitContainer1.SplitterDistance = 312;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeObj);
            this.splitContainer2.Panel1.Controls.Add(this.pnlTreeObjLabel);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.listBoxViewHistory);
            this.splitContainer2.Panel2.Controls.Add(this.pnlViewHistoryLabel);
            this.splitContainer2.Size = new System.Drawing.Size(312, 369);
            this.splitContainer2.SplitterDistance = 262;
            this.splitContainer2.TabIndex = 3;
            // 
            // treeObj
            // 
            this.treeObj.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeObj.ImageIndex = 0;
            this.treeObj.ImageList = this.objectTreeViewImageList;
            this.treeObj.Location = new System.Drawing.Point(0, 21);
            this.treeObj.Name = "treeObj";
            this.treeObj.SelectedImageIndex = 0;
            this.treeObj.Size = new System.Drawing.Size(312, 241);
            this.treeObj.TabIndex = 1;
            this.treeObj.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeObj_AfterExpand);
            this.treeObj.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeObj_AfterSelect);
            // 
            // objectTreeViewImageList
            // 
            this.objectTreeViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("objectTreeViewImageList.ImageStream")));
            this.objectTreeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.objectTreeViewImageList.Images.SetKeyName(0, "ImageEmpty.gif");
            this.objectTreeViewImageList.Images.SetKeyName(1, "ImageEmptySelected.gif");
            // 
            // pnlTreeObjLabel
            // 
            this.pnlTreeObjLabel.Controls.Add(this.labelTreeObj);
            this.pnlTreeObjLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTreeObjLabel.Location = new System.Drawing.Point(0, 0);
            this.pnlTreeObjLabel.Name = "pnlTreeObjLabel";
            this.pnlTreeObjLabel.Size = new System.Drawing.Size(312, 21);
            this.pnlTreeObjLabel.TabIndex = 0;
            // 
            // labelTreeObj
            // 
            this.labelTreeObj.AutoSize = true;
            this.labelTreeObj.Location = new System.Drawing.Point(3, 5);
            this.labelTreeObj.Name = "labelTreeObj";
            this.labelTreeObj.Size = new System.Drawing.Size(106, 13);
            this.labelTreeObj.TabIndex = 0;
            this.labelTreeObj.Text = "Список найденного";
            // 
            // listBoxViewHistory
            // 
            this.listBoxViewHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxViewHistory.FormattingEnabled = true;
            this.listBoxViewHistory.IntegralHeight = false;
            this.listBoxViewHistory.Location = new System.Drawing.Point(0, 21);
            this.listBoxViewHistory.Name = "listBoxViewHistory";
            this.listBoxViewHistory.Size = new System.Drawing.Size(312, 82);
            this.listBoxViewHistory.TabIndex = 1;
            this.listBoxViewHistory.Click += new System.EventHandler(this.listBoxViewHistory_Click);
            // 
            // pnlViewHistoryLabel
            // 
            this.pnlViewHistoryLabel.Controls.Add(this.labelViewHistory);
            this.pnlViewHistoryLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlViewHistoryLabel.Location = new System.Drawing.Point(0, 0);
            this.pnlViewHistoryLabel.Name = "pnlViewHistoryLabel";
            this.pnlViewHistoryLabel.Size = new System.Drawing.Size(312, 21);
            this.pnlViewHistoryLabel.TabIndex = 0;
            // 
            // labelViewHistory
            // 
            this.labelViewHistory.AutoSize = true;
            this.labelViewHistory.Location = new System.Drawing.Point(3, 5);
            this.labelViewHistory.Name = "labelViewHistory";
            this.labelViewHistory.Size = new System.Drawing.Size(209, 13);
            this.labelViewHistory.TabIndex = 0;
            this.labelViewHistory.Text = "История переходов (Click для перехода)";
            // 
            // scintillaTextBox
            // 
            this.scintillaTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintillaTextBox.Location = new System.Drawing.Point(0, 21);
            this.scintillaTextBox.Name = "scintillaTextBox";
            this.scintillaTextBox.Size = new System.Drawing.Size(626, 348);
            this.scintillaTextBox.TabIndex = 0;
            this.scintillaTextBox.Text = "select * from ...";
            // 
            // pnlSourceText
            // 
            this.pnlSourceText.Controls.Add(this.label1);
            this.pnlSourceText.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSourceText.Location = new System.Drawing.Point(0, 0);
            this.pnlSourceText.Name = "pnlSourceText";
            this.pnlSourceText.Size = new System.Drawing.Size(626, 21);
            this.pnlSourceText.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Текст";
            // 
            // FrmMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(942, 440);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMainWindow";
            this.Text = "dbShowDepends";
            this.Load += new System.EventHandler(this.FrmMainWindow_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.pnlTreeObjLabel.ResumeLayout(false);
            this.pnlTreeObjLabel.PerformLayout();
            this.pnlViewHistoryLabel.ResumeLayout(false);
            this.pnlViewHistoryLabel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scintillaTextBox)).EndInit();
            this.pnlSourceText.ResumeLayout(false);
            this.pnlSourceText.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsConnections)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox tscbDatabaseName;
        private System.Windows.Forms.ToolStripButton tsbFindObject;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ImageList objectTreeViewImageList;
        private System.Windows.Forms.ToolStripTextBox tstbObjectName;
        private System.Windows.Forms.ToolStripButton tsbClearSearchText;
        private System.Windows.Forms.ToolStripMenuItem setupConnectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox cbConnection;
        private System.Windows.Forms.BindingSource bsConnections;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView treeObj;
        private System.Windows.Forms.Panel pnlTreeObjLabel;
        private System.Windows.Forms.Label labelTreeObj;
        private System.Windows.Forms.Panel pnlViewHistoryLabel;
        private System.Windows.Forms.Label labelViewHistory;
        private System.Windows.Forms.ListBox listBoxViewHistory;
        private ScintillaNET.Scintilla scintillaTextBox;
        private System.Windows.Forms.ToolStripButton tsbSearchBySource;
        private System.Windows.Forms.Panel pnlSourceText;
        private System.Windows.Forms.Label label1;
    }
}

