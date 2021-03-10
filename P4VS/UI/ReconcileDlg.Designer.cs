namespace Perforce.P4VS
{
	partial class ReconcileDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReconcileDlg));
            this.OkBtn = new Perforce.I18nControls.GridButton();
            this.CancelBtn = new Perforce.I18nControls.GridButton();
            this.PromptLbl = new Perforce.I18nControls.GridLabel();
            this.ItemsCB = new Perforce.I18nControls.GridComboBox();
            this.missingText = new Perforce.I18nControls.GridLabel();
            this.notInDepotText = new Perforce.I18nControls.GridLabel();
            this.modifiedText = new Perforce.I18nControls.GridLabel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView3 = new System.Windows.Forms.ListView();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // OkBtn
            // 
            resources.ApplyResources(this.OkBtn, "OkBtn");
            this.OkBtn.CellHeight = 24;
            this.OkBtn.CellWidth = 223;
            this.OkBtn.Column = 0;
            this.OkBtn.ColumnsSpanned = 0;
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Row = 0;
            this.OkBtn.RowsSpanned = 0;
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.YOffset = 0;
            this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // CancelBtn
            // 
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.CellHeight = 24;
            this.CancelBtn.CellWidth = 222;
            this.CancelBtn.Column = 0;
            this.CancelBtn.ColumnsSpanned = 0;
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Row = 0;
            this.CancelBtn.RowsSpanned = 0;
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.YOffset = 0;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // PromptLbl
            // 
            resources.ApplyResources(this.PromptLbl, "PromptLbl");
            this.PromptLbl.CellHeight = 48;
            this.PromptLbl.CellWidth = 369;
            this.PromptLbl.Column = 0;
            this.PromptLbl.ColumnsSpanned = 0;
            this.PromptLbl.Name = "PromptLbl";
            this.PromptLbl.Row = 2;
            this.PromptLbl.RowsSpanned = 0;
            this.PromptLbl.YOffset = 0;
            // 
            // ItemsCB
            // 
            resources.ApplyResources(this.ItemsCB, "ItemsCB");
            this.ItemsCB.CellHeight = 62;
            this.ItemsCB.CellWidth = 369;
            this.ItemsCB.Column = 0;
            this.ItemsCB.ColumnsSpanned = 0;
            this.ItemsCB.DesignSize = new System.Drawing.Size(0, 0);
            this.ItemsCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ItemsCB.FormattingEnabled = true;
            this.ItemsCB.Name = "ItemsCB";
            this.ItemsCB.Row = 3;
            this.ItemsCB.RowsSpanned = 0;
            this.ItemsCB.YOffset = 0;
            this.ItemsCB.SelectedIndexChanged += new System.EventHandler(this.ItemsCB_SelectedIndexChanged);
            // 
            // missingText
            // 
            resources.ApplyResources(this.missingText, "missingText");
            this.missingText.CellHeight = 85;
            this.missingText.CellWidth = 369;
            this.missingText.Column = 0;
            this.missingText.ColumnsSpanned = 0;
            this.missingText.Image = global::Perforce.P4VS.Images.delete_toolbar_icon16x16;
            this.missingText.Name = "missingText";
            this.missingText.Row = 0;
            this.missingText.RowsSpanned = 0;
            this.missingText.YOffset = 0;
            // 
            // notInDepotText
            // 
            resources.ApplyResources(this.notInDepotText, "notInDepotText");
            this.notInDepotText.CellHeight = 33;
            this.notInDepotText.CellWidth = 348;
            this.notInDepotText.Column = 0;
            this.notInDepotText.ColumnsSpanned = 0;
            this.notInDepotText.Image = global::Perforce.P4VS.Images.add_toolbar_icon16x16;
            this.notInDepotText.Name = "notInDepotText";
            this.notInDepotText.Row = 0;
            this.notInDepotText.RowsSpanned = 0;
            this.notInDepotText.YOffset = 0;
            // 
            // modifiedText
            // 
            resources.ApplyResources(this.modifiedText, "modifiedText");
            this.modifiedText.CellHeight = 33;
            this.modifiedText.CellWidth = 348;
            this.modifiedText.Column = 0;
            this.modifiedText.ColumnsSpanned = 0;
            this.modifiedText.Image = global::Perforce.P4VS.Images.checkout_toolbar_icon16x16;
            this.modifiedText.Name = "modifiedText";
            this.modifiedText.Row = 0;
            this.modifiedText.RowsSpanned = 0;
            this.modifiedText.YOffset = 0;
            // 
            // listView1
            // 
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.Name = "listView1";
            this.listView1.OwnerDraw = true;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView1_DrawColumnHeader);
            this.listView1.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView1_DrawItem);
            this.listView1.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView1_DrawSubItem);
            this.listView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView1_ItemChecked);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // listView2
            // 
            this.listView2.CheckBoxes = true;
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            resources.ApplyResources(this.listView2, "listView2");
            this.listView2.Name = "listView2";
            this.listView2.OwnerDraw = true;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView2_ColumnClick);
            this.listView2.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView2_DrawColumnHeader);
            this.listView2.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView2_DrawItem);
            this.listView2.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView2_DrawSubItem);
            this.listView2.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView2_ItemChecked);
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            // 
            // columnHeader6
            // 
            resources.ApplyResources(this.columnHeader6, "columnHeader6");
            // 
            // listView3
            // 
            resources.ApplyResources(this.listView3, "listView3");
            this.listView3.CheckBoxes = true;
            this.listView3.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12});
            this.listView3.Name = "listView3";
            this.listView3.OwnerDraw = true;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
            this.listView3.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView3_ColumnClick);
            this.listView3.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView3_DrawColumnHeader);
            this.listView3.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView3_DrawItem);
            this.listView3.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView3_DrawSubItem);
            this.listView3.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView3_ItemChecked);
            // 
            // columnHeader10
            // 
            resources.ApplyResources(this.columnHeader10, "columnHeader10");
            // 
            // columnHeader11
            // 
            resources.ApplyResources(this.columnHeader11, "columnHeader11");
            // 
            // columnHeader12
            // 
            resources.ApplyResources(this.columnHeader12, "columnHeader12");
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.modifiedText, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ItemsCB, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.listView1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.missingText, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.PromptLbl, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.notInDepotText, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.listView2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.listView3, 0, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.OkBtn, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.CancelBtn, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // ReconcileDlg
            // 
            this.AcceptButton = this.OkBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReconcileDlg";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

		}

        #endregion
        private I18nControls.GridButton CancelBtn;
        private I18nControls.GridButton OkBtn;
        private I18nControls.GridLabel PromptLbl;
        private I18nControls.GridComboBox ItemsCB;
        private I18nControls.GridLabel missingText;
        private I18nControls.GridLabel notInDepotText;
        private I18nControls.GridLabel modifiedText;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}