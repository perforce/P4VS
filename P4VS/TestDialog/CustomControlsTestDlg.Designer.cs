namespace Perforce.P4VS.TestDialog
{
    partial class CustomControlsTestDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomControlsTestDlg));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabMultiSelectTreeListView = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SameTypeCB = new System.Windows.Forms.CheckBox();
            this.SameLevelCB = new System.Windows.Forms.CheckBox();
            this.SameParentCB = new System.Windows.Forms.CheckBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.treeListView1 = new Perforce.P4VS.TreeListView();
            this.NameClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SizeClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CommentClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabControl1.SuspendLayout();
            this.TabMultiSelectTreeListView.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TabMultiSelectTreeListView);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(427, 507);
            this.tabControl1.TabIndex = 0;
            // 
            // TabMultiSelectTreeListView
            // 
            this.TabMultiSelectTreeListView.Controls.Add(this.groupBox1);
            this.TabMultiSelectTreeListView.Controls.Add(this.treeListView1);
            this.TabMultiSelectTreeListView.Location = new System.Drawing.Point(4, 22);
            this.TabMultiSelectTreeListView.Name = "TabMultiSelectTreeListView";
            this.TabMultiSelectTreeListView.Padding = new System.Windows.Forms.Padding(3);
            this.TabMultiSelectTreeListView.Size = new System.Drawing.Size(419, 481);
            this.TabMultiSelectTreeListView.TabIndex = 0;
            this.TabMultiSelectTreeListView.Text = "MultiSelect TreeListView";
            this.TabMultiSelectTreeListView.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.SameTypeCB);
            this.groupBox1.Controls.Add(this.SameLevelCB);
            this.groupBox1.Controls.Add(this.SameParentCB);
            this.groupBox1.Location = new System.Drawing.Point(8, 381);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(403, 90);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Multiselect Conditions";
            // 
            // SameTypeCB
            // 
            this.SameTypeCB.AutoSize = true;
            this.SameTypeCB.Location = new System.Drawing.Point(6, 66);
            this.SameTypeCB.Name = "SameTypeCB";
            this.SameTypeCB.Size = new System.Drawing.Size(80, 17);
            this.SameTypeCB.TabIndex = 0;
            this.SameTypeCB.Text = "Same Type";
            this.SameTypeCB.UseVisualStyleBackColor = true;
            this.SameTypeCB.CheckedChanged += new System.EventHandler(this.SameTypeCB_CheckedChanged);
            // 
            // SameLevelCB
            // 
            this.SameLevelCB.AutoSize = true;
            this.SameLevelCB.Location = new System.Drawing.Point(6, 43);
            this.SameLevelCB.Name = "SameLevelCB";
            this.SameLevelCB.Size = new System.Drawing.Size(82, 17);
            this.SameLevelCB.TabIndex = 0;
            this.SameLevelCB.Text = "Same Level";
            this.SameLevelCB.UseVisualStyleBackColor = true;
            this.SameLevelCB.CheckedChanged += new System.EventHandler(this.SameLevelCB_CheckedChanged);
            // 
            // SameParentCB
            // 
            this.SameParentCB.AutoSize = true;
            this.SameParentCB.Location = new System.Drawing.Point(7, 20);
            this.SameParentCB.Name = "SameParentCB";
            this.SameParentCB.Size = new System.Drawing.Size(87, 17);
            this.SameParentCB.TabIndex = 0;
            this.SameParentCB.Text = "Same Parent";
            this.SameParentCB.UseVisualStyleBackColor = true;
            this.SameParentCB.CheckedChanged += new System.EventHandler(this.SameParentCB_CheckedChanged);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.png");
            this.imageList1.Images.SetKeyName(1, "file_depot_icon.png");
            this.imageList1.Images.SetKeyName(2, "label.png");
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(419, 481);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // treeListView1
            // 
            this.treeListView1._maxLineOffset = 0;
            this.treeListView1.ActionColumn = -1;
            this.treeListView1.AllowColumnReorder = true;
            this.treeListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameClm,
            this.SizeClm,
            this.CommentClm});
            this.treeListView1.EnableIconOverlays = false;
            this.treeListView1.EnableSorting = true;
            this.treeListView1.FullRowSelect = true;
            this.treeListView1.GridLines = true;
            this.treeListView1.Location = new System.Drawing.Point(6, 6);
            this.treeListView1.Name = "treeListView1";
            this.treeListView1.OverlayOffset = 0;
            this.treeListView1.RootCheckBoxes = false;
            this.treeListView1.ScrollPosition = 0;
            this.treeListView1.Size = new System.Drawing.Size(407, 369);
            this.treeListView1.SmallImageList = this.imageList1;
            this.treeListView1.TabIndex = 0;
            this.treeListView1.TreeView = true;
            this.treeListView1.UseClassicImageList = false;
            this.treeListView1.UseCompatibleStateImageBehavior = false;
            this.treeListView1.View = System.Windows.Forms.View.Details;
            // 
            // NameClm
            // 
            this.NameClm.Text = "Name";
            this.NameClm.Width = 109;
            // 
            // SizeClm
            // 
            this.SizeClm.Text = "Size";
            this.SizeClm.Width = 71;
            // 
            // CommentClm
            // 
            this.CommentClm.Text = "Comment";
            this.CommentClm.Width = 221;
            // 
            // CustomControlsTestDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 507);
            this.Controls.Add(this.tabControl1);
            this.Name = "CustomControlsTestDlg";
            this.Text = "Test Custom Controls";
            this.tabControl1.ResumeLayout(false);
            this.TabMultiSelectTreeListView.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabMultiSelectTreeListView;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox SameTypeCB;
        private System.Windows.Forms.CheckBox SameLevelCB;
        private System.Windows.Forms.CheckBox SameParentCB;
        private TreeListView treeListView1;
        private System.Windows.Forms.ColumnHeader NameClm;
        private System.Windows.Forms.ColumnHeader SizeClm;
        private System.Windows.Forms.ColumnHeader CommentClm;
        private System.Windows.Forms.ImageList imageList1;
    }
}