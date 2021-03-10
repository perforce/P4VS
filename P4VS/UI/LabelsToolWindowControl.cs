
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using NLog;

using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	/// <summary>
	/// Summary description for P4ToolWindowControl.
	/// </summary>
	public class LabelsToolWindowControl : P4ToolWindowControlBase
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private I18nControls.GridSplitContainer LabelsControlSplitter;
		private I18nControls.GridTreeListView LabelList;
		private ColumnHeader LabelClm;
		private ColumnHeader LastAccessedClm;
		private ColumnHeader OwnerCln;
		private I18nControls.GridLabel label1;
		private I18nControls.GridGroupBox dividerGB;
		private I18nControls.GridLabel matchesLbl;
		private I18nControls.GridButton filterBtn;
		private I18nControls.GridLabel label2;
		private I18nControls.GridFilterComboBox LabelNameFilterCB;
		private I18nControls.GridFilterComboBox OwnerCB;
		private I18nControls.GridLabel label3;
		private I18nControls.GridFilterComboBox FileFilterCB;
		private Perforce.I18nControls.GridGroupBox groupBox1;
		private Perforce.I18nControls.GridButton ShowFilesBtn;
		private Perforce.I18nControls.GridButton ShowDetailsBtn;
		private LabelsToolWindowControlDetails DetailsCtrl;
		private LabelsToolWindowFilesControl FilesCtrl;
		private IContainer components;
		private ColumnHeader DateModifiedClm;
		private ColumnHeader DescriptionClm;
		private ColumnHeader LockedClm;
		private UI.ThreadMonitorControl threadMonitor;
		private I18nControls.GridCheckBox LabelNameMatchCaseCB;
		private I18nControls.GridComboBox NameFilterCB;

		private ImageList ButtonImages;
		private ImageList LabelListImages;
		private ContextMenuStrip LabelListCtxMenu;
		private ToolStripMenuItem LabelListCtxMnyRefreshList;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
		private I18nControls.GridLayoutPanel gridLayoutPanel2;
		private I18nControls.GridPanel gridPanel1;
		private I18nControls.GridPanel gridPanel2;

		public P4ScmProvider scm { get; set; }
		//private IContainer components;

	    IList<object> WorkspacesFilesListViewFields = null;

		public LabelsToolWindowControl()
		{
			Scm = P4VsProvider.CurrentScm;

			PreferenceKey = "LabelsToolWindowControl";

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
            base.Initialize();

			this.ButtonImages = new System.Windows.Forms.ImageList(this.components);
			// 
			// ButtonImages
			// 
			this.ButtonImages.TransparentColor = System.Drawing.Color.White;
			this.ButtonImages.Images.Add("TriangleDownGray.png", Images.TriangleDownGray);
			this.ButtonImages.Images.Add("TriangleDownRed.png", Images.TriangleDownRed);
			this.ButtonImages.Images.Add("TriangleRightGray.png", Images.TriangleRightGray);
			this.ButtonImages.Images.Add("TriangleRightRed.png", Images.TriangleRightRed);

			this.ShowFilesBtn.ImageList = this.ButtonImages;
			this.ShowDetailsBtn.ImageList = this.ButtonImages;

			DetailsCtrl.Visible = true;
			FilesCtrl.Visible = false;
			ActiveButton = ShowDetailsBtn;
			ActiveTab = DetailsCtrl;

			filterBtn.Enabled = (Scm != null);

			// field shown in the workspaces files list view
			WorkspacesFilesListViewFields = new List<object>();
			WorkspacesFilesListViewFields.Add(P4FileTreeListViewItem.SubItemFlag.LocalFileName);
			WorkspacesFilesListViewFields.Add(P4FileTreeListViewItem.SubItemFlag.LocalFolder);
			WorkspacesFilesListViewFields.Add(P4FileTreeListViewItem.SubItemFlag.HaveSlashHeadRevision);
			WorkspacesFilesListViewFields.Add(P4FileTreeListViewItem.SubItemFlag.HeadModTime);
			WorkspacesFilesListViewFields.Add(P4FileTreeListViewItem.SubItemFlag.Size);
			WorkspacesFilesListViewFields.Add(P4FileTreeListViewItem.SubItemFlag.FileExtension);
			WorkspacesFilesListViewFields.Add(P4FileTreeListViewItem.SubItemFlag.Type);

			NameFilterCB.SelectedIndex = 0;

#if VS2012
            if (!DesignMode)
            {
                base.InitThemeManager();
            }
#endif
        }

        /// <summary> 
        /// Let this control process the mnemonics.
        /// </summary>
        protected override bool ProcessDialogChar(char charCode)
		{
			  // If we're the top-level form or control, we need to do the mnemonic handling
			  if (charCode != ' ' && ProcessMnemonic(charCode))
			  {
					return true;
			  }
			  return base.ProcessDialogChar(charCode);
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private new void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LabelsToolWindowControl));
            this.LabelListCtxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.LabelListCtxMnyRefreshList = new System.Windows.Forms.ToolStripMenuItem();
            this.LabelListImages = new System.Windows.Forms.ImageList(this.components);
            this.LabelsControlSplitter = new Perforce.I18nControls.GridSplitContainer();
            this.threadMonitor = new Perforce.P4VS.UI.ThreadMonitorControl();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.LabelNameMatchCaseCB = new Perforce.I18nControls.GridCheckBox();
            this.FileFilterCB = new Perforce.I18nControls.GridFilterComboBox();
            this.label3 = new Perforce.I18nControls.GridLabel();
            this.NameFilterCB = new Perforce.I18nControls.GridComboBox();
            this.label2 = new Perforce.I18nControls.GridLabel();
            this.filterBtn = new Perforce.I18nControls.GridButton();
            this.label1 = new Perforce.I18nControls.GridLabel();
            this.matchesLbl = new Perforce.I18nControls.GridLabel();
            this.LabelNameFilterCB = new Perforce.I18nControls.GridFilterComboBox();
            this.dividerGB = new Perforce.I18nControls.GridGroupBox();
            this.OwnerCB = new Perforce.I18nControls.GridFilterComboBox();
            this.LabelList = new Perforce.I18nControls.GridTreeListView();
            this.LabelClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DateModifiedClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.OwnerCln = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LastAccessedClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DescriptionClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LockedClm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gridLayoutPanel2 = new Perforce.I18nControls.GridLayoutPanel();
            this.gridPanel2 = new Perforce.I18nControls.GridPanel();
            this.ShowDetailsBtn = new Perforce.I18nControls.GridButton();
            this.ShowFilesBtn = new Perforce.I18nControls.GridButton();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.DetailsCtrl = new Perforce.P4VS.LabelsToolWindowControlDetails();
            this.FilesCtrl = new Perforce.P4VS.LabelsToolWindowFilesControl();
            this.groupBox1 = new Perforce.I18nControls.GridGroupBox();
            this.LabelListCtxMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LabelsControlSplitter)).BeginInit();
            this.LabelsControlSplitter.Panel1.SuspendLayout();
            this.LabelsControlSplitter.Panel2.SuspendLayout();
            this.LabelsControlSplitter.SuspendLayout();
            this.gridLayoutPanel1.SuspendLayout();
            this.gridLayoutSubpanel1.SuspendLayout();
            this.gridLayoutPanel2.SuspendLayout();
            this.gridPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelListCtxMenu
            // 
            this.LabelListCtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LabelListCtxMnyRefreshList});
            this.LabelListCtxMenu.Name = "LabelListCtxMenu";
            resources.ApplyResources(this.LabelListCtxMenu, "LabelListCtxMenu");
            this.LabelListCtxMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.LabelListCtxMenu_ItemClicked);
            // 
            // LabelListCtxMnyRefreshList
            // 
            this.LabelListCtxMnyRefreshList.Name = "LabelListCtxMnyRefreshList";
            resources.ApplyResources(this.LabelListCtxMnyRefreshList, "LabelListCtxMnyRefreshList");
            // 
            // LabelListImages
            // 
            this.LabelListImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.LabelListImages, "LabelListImages");
            this.LabelListImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // LabelsControlSplitter
            // 
            this.LabelsControlSplitter.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.LabelsControlSplitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabelsControlSplitter.CellHeight = 0;
            this.LabelsControlSplitter.CellWidth = 0;
            this.LabelsControlSplitter.Column = 0;
            this.LabelsControlSplitter.ColumnsSpanned = 0;
            resources.ApplyResources(this.LabelsControlSplitter, "LabelsControlSplitter");
            this.LabelsControlSplitter.Name = "LabelsControlSplitter";
            // 
            // LabelsControlSplitter.Panel1
            // 
            this.LabelsControlSplitter.Panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.LabelsControlSplitter.Panel1.Controls.Add(this.threadMonitor);
            this.LabelsControlSplitter.Panel1.Controls.Add(this.gridLayoutPanel1);
            // 
            // LabelsControlSplitter.Panel2
            // 
            this.LabelsControlSplitter.Panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.LabelsControlSplitter.Panel2.CausesValidation = false;
            this.LabelsControlSplitter.Panel2.Controls.Add(this.gridLayoutPanel2);
            this.LabelsControlSplitter.Row = 0;
            this.LabelsControlSplitter.RowsSpanned = 0;
            this.LabelsControlSplitter.YOffset = 0;
            // 
            // threadMonitor
            // 
            this.threadMonitor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.threadMonitor.CancelPressed = false;
            resources.ApplyResources(this.threadMonitor, "threadMonitor");
            this.threadMonitor.Maximum = 100;
            this.threadMonitor.Name = "threadMonitor";
            this.threadMonitor.Step = 1;
            this.threadMonitor.Value = 50;
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel1);
            this.gridLayoutPanel1.Controls.Add(this.LabelList);
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.EnableDesignerGrid = true;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // gridLayoutSubpanel1
            // 
            resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
            this.gridLayoutSubpanel1.CellHeight = 90;
            this.gridLayoutSubpanel1.CellWidth = 553;
            this.gridLayoutSubpanel1.Column = 0;
            this.gridLayoutSubpanel1.ColumnsSpanned = 0;
            this.gridLayoutSubpanel1.Controls.Add(this.LabelNameMatchCaseCB);
            this.gridLayoutSubpanel1.Controls.Add(this.FileFilterCB);
            this.gridLayoutSubpanel1.Controls.Add(this.label3);
            this.gridLayoutSubpanel1.Controls.Add(this.NameFilterCB);
            this.gridLayoutSubpanel1.Controls.Add(this.label2);
            this.gridLayoutSubpanel1.Controls.Add(this.filterBtn);
            this.gridLayoutSubpanel1.Controls.Add(this.label1);
            this.gridLayoutSubpanel1.Controls.Add(this.matchesLbl);
            this.gridLayoutSubpanel1.Controls.Add(this.LabelNameFilterCB);
            this.gridLayoutSubpanel1.Controls.Add(this.dividerGB);
            this.gridLayoutSubpanel1.Controls.Add(this.OwnerCB);
            this.gridLayoutSubpanel1.EnableDesignerGrid = true;
            this.gridLayoutSubpanel1.EnableDesignerLayout = true;
            this.gridLayoutSubpanel1.EnableParentResize = false;
            this.gridLayoutSubpanel1.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel1.MinimumRowHeight = 10;
            this.gridLayoutSubpanel1.Name = "gridLayoutSubpanel1";
            this.gridLayoutSubpanel1.Row = 0;
            this.gridLayoutSubpanel1.RowsSpanned = 0;
            this.gridLayoutSubpanel1.YOffset = 0;
            // 
            // LabelNameMatchCaseCB
            // 
            resources.ApplyResources(this.LabelNameMatchCaseCB, "LabelNameMatchCaseCB");
            this.LabelNameMatchCaseCB.CellHeight = 23;
            this.LabelNameMatchCaseCB.CellWidth = 167;
            this.LabelNameMatchCaseCB.Column = 4;
            this.LabelNameMatchCaseCB.ColumnsSpanned = 0;
            this.LabelNameMatchCaseCB.Name = "LabelNameMatchCaseCB";
            this.LabelNameMatchCaseCB.Row = 2;
            this.LabelNameMatchCaseCB.RowsSpanned = 0;
            this.LabelNameMatchCaseCB.UseVisualStyleBackColor = true;
            this.LabelNameMatchCaseCB.YOffset = 0;
            // 
            // FileFilterCB
            // 
            resources.ApplyResources(this.FileFilterCB, "FileFilterCB");
            this.FileFilterCB.CellHeight = 30;
            this.FileFilterCB.CellWidth = 390;
            this.FileFilterCB.Column = 1;
            this.FileFilterCB.ColumnsSpanned = 3;
            this.FileFilterCB.FormattingEnabled = true;
            this.FileFilterCB.Name = "FileFilterCB";
            this.FileFilterCB.Row = 0;
            this.FileFilterCB.RowsSpanned = 0;
            this.FileFilterCB.YOffset = 1;
            this.FileFilterCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FileFilterCB_KeyDown);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.CellHeight = 28;
            this.label3.CellWidth = 41;
            this.label3.Column = 2;
            this.label3.ColumnsSpanned = 0;
            this.label3.Name = "label3";
            this.label3.Row = 1;
            this.label3.RowsSpanned = 0;
            this.label3.YOffset = 4;
            // 
            // NameFilterCB
            // 
            resources.ApplyResources(this.NameFilterCB, "NameFilterCB");
            this.NameFilterCB.CellHeight = 28;
            this.NameFilterCB.CellWidth = 92;
            this.NameFilterCB.Column = 3;
            this.NameFilterCB.ColumnsSpanned = 0;
            this.NameFilterCB.DesignSize = new System.Drawing.Size(0, 0);
            this.NameFilterCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NameFilterCB.FormattingEnabled = true;
            this.NameFilterCB.Items.AddRange(new object[] {
            resources.GetString("NameFilterCB.Items"),
            resources.GetString("NameFilterCB.Items1"),
            resources.GetString("NameFilterCB.Items2"),
            resources.GetString("NameFilterCB.Items3")});
            this.NameFilterCB.Name = "NameFilterCB";
            this.NameFilterCB.Row = 1;
            this.NameFilterCB.RowsSpanned = 0;
            this.NameFilterCB.YOffset = 0;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.CellHeight = 28;
            this.label2.CellWidth = 66;
            this.label2.Column = 0;
            this.label2.ColumnsSpanned = 0;
            this.label2.Name = "label2";
            this.label2.Row = 1;
            this.label2.RowsSpanned = 0;
            this.label2.YOffset = 4;
            // 
            // filterBtn
            // 
            resources.ApplyResources(this.filterBtn, "filterBtn");
            this.filterBtn.CellHeight = 30;
            this.filterBtn.CellWidth = 81;
            this.filterBtn.Column = 6;
            this.filterBtn.ColumnsSpanned = 0;
            this.filterBtn.Name = "filterBtn";
            this.filterBtn.Row = 0;
            this.filterBtn.RowsSpanned = 0;
            this.filterBtn.UseVisualStyleBackColor = false;
            this.filterBtn.YOffset = 0;
            this.filterBtn.Click += new System.EventHandler(this.filterBtn_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.CellHeight = 30;
            this.label1.CellWidth = 66;
            this.label1.Column = 0;
            this.label1.ColumnsSpanned = 0;
            this.label1.Name = "label1";
            this.label1.Row = 0;
            this.label1.RowsSpanned = 0;
            this.label1.YOffset = 5;
            // 
            // matchesLbl
            // 
            resources.ApplyResources(this.matchesLbl, "matchesLbl");
            this.matchesLbl.AutoEllipsis = true;
            this.matchesLbl.CellHeight = 28;
            this.matchesLbl.CellWidth = 81;
            this.matchesLbl.Column = 6;
            this.matchesLbl.ColumnsSpanned = 0;
            this.matchesLbl.Name = "matchesLbl";
            this.matchesLbl.Row = 1;
            this.matchesLbl.RowsSpanned = 0;
            this.matchesLbl.YOffset = 4;
            // 
            // LabelNameFilterCB
            // 
            resources.ApplyResources(this.LabelNameFilterCB, "LabelNameFilterCB");
            this.LabelNameFilterCB.CellHeight = 28;
            this.LabelNameFilterCB.CellWidth = 167;
            this.LabelNameFilterCB.Column = 4;
            this.LabelNameFilterCB.ColumnsSpanned = 0;
            this.LabelNameFilterCB.FormattingEnabled = true;
            this.LabelNameFilterCB.Name = "LabelNameFilterCB";
            this.LabelNameFilterCB.Row = 1;
            this.LabelNameFilterCB.RowsSpanned = 0;
            this.LabelNameFilterCB.YOffset = 0;
            this.LabelNameFilterCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LabelNameFilterCB_KeyDown);
            // 
            // dividerGB
            // 
            resources.ApplyResources(this.dividerGB, "dividerGB");
            this.dividerGB.CellHeight = 81;
            this.dividerGB.CellWidth = 10;
            this.dividerGB.Column = 5;
            this.dividerGB.ColumnsSpanned = 0;
            this.dividerGB.Name = "dividerGB";
            this.dividerGB.Row = 0;
            this.dividerGB.RowsSpanned = 2;
            this.dividerGB.TabStop = false;
            this.dividerGB.YOffset = 0;
            // 
            // OwnerCB
            // 
            resources.ApplyResources(this.OwnerCB, "OwnerCB");
            this.OwnerCB.CellHeight = 28;
            this.OwnerCB.CellWidth = 90;
            this.OwnerCB.Column = 1;
            this.OwnerCB.ColumnsSpanned = 0;
            this.OwnerCB.FormattingEnabled = true;
            this.OwnerCB.Name = "OwnerCB";
            this.OwnerCB.Row = 1;
            this.OwnerCB.RowsSpanned = 0;
            this.OwnerCB.YOffset = 0;
            this.OwnerCB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OwnerCB_KeyDown);
            // 
            // LabelList
            // 
            this.LabelList._maxLineOffset = 0;
            this.LabelList.ActionColumn = -1;
            this.LabelList.AllowColumnReorder = true;
            resources.ApplyResources(this.LabelList, "LabelList");
            this.LabelList.BackColor = System.Drawing.SystemColors.Window;
            this.LabelList.CellHeight = 59;
            this.LabelList.CellWidth = 553;
            this.LabelList.Column = 0;
            this.LabelList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LabelClm,
            this.DateModifiedClm,
            this.OwnerCln,
            this.LastAccessedClm,
            this.DescriptionClm,
            this.LockedClm});
            this.LabelList.ColumnsSpanned = 0;
            this.LabelList.ContextMenuStrip = this.LabelListCtxMenu;
            this.LabelList.EnableIconOverlays = false;
            this.LabelList.EnableSorting = true;
            this.LabelList.FullRowSelect = true;
            this.LabelList.GridLines = true;
            this.LabelList.MultiSelectConditions = Perforce.P4VS.TreeListView.MultiSelectCondition.none;
            this.LabelList.Name = "LabelList";
            this.LabelList.OverlayOffset = 3;
            this.LabelList.RootCheckBoxes = false;
            this.LabelList.Row = 1;
            this.LabelList.RowsSpanned = 0;
            this.LabelList.ScrollPosition = 0;
            this.LabelList.SmallImageList = this.LabelListImages;
            this.LabelList.TreeView = false;
            this.LabelList.UseClassicImageList = false;
            this.LabelList.UseCompatibleStateImageBehavior = false;
            this.LabelList.View = System.Windows.Forms.View.Details;
            this.LabelList.YOffset = 0;
            this.LabelList.onMaxScroll += new System.Windows.Forms.ScrollEventHandler(this.LabelList_onMaxScroll);
            this.LabelList.SelectedIndexChanged += new System.EventHandler(this.LabelList_SelectedIndexChanged);
            this.LabelList.DoubleClick += new System.EventHandler(this.LabelList_DoubleClick);
            // 
            // LabelClm
            // 
            resources.ApplyResources(this.LabelClm, "LabelClm");
            // 
            // DateModifiedClm
            // 
            resources.ApplyResources(this.DateModifiedClm, "DateModifiedClm");
            // 
            // OwnerCln
            // 
            resources.ApplyResources(this.OwnerCln, "OwnerCln");
            // 
            // LastAccessedClm
            // 
            resources.ApplyResources(this.LastAccessedClm, "LastAccessedClm");
            // 
            // DescriptionClm
            // 
            resources.ApplyResources(this.DescriptionClm, "DescriptionClm");
            // 
            // LockedClm
            // 
            resources.ApplyResources(this.LockedClm, "LockedClm");
            // 
            // gridLayoutPanel2
            // 
            this.gridLayoutPanel2.Controls.Add(this.gridPanel2);
            this.gridLayoutPanel2.Controls.Add(this.ShowDetailsBtn);
            this.gridLayoutPanel2.Controls.Add(this.ShowFilesBtn);
            this.gridLayoutPanel2.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel2.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.gridLayoutPanel2, "gridLayoutPanel2");
            this.gridLayoutPanel2.EnableDesignerGrid = true;
            this.gridLayoutPanel2.EnableDesignerLayout = true;
            this.gridLayoutPanel2.EnableParentResize = false;
            this.gridLayoutPanel2.MinimumColumnWidth = 10;
            this.gridLayoutPanel2.MinimumRowHeight = 10;
            this.gridLayoutPanel2.Name = "gridLayoutPanel2";
            // 
            // gridPanel2
            // 
            resources.ApplyResources(this.gridPanel2, "gridPanel2");
            this.gridPanel2.CellHeight = 122;
            this.gridPanel2.CellWidth = 206;
            this.gridPanel2.Column = 0;
            this.gridPanel2.ColumnsSpanned = 0;
            this.gridPanel2.Name = "gridPanel2";
            this.gridPanel2.Row = 2;
            this.gridPanel2.RowsSpanned = 0;
            this.gridPanel2.YOffset = 0;
            // 
            // ShowDetailsBtn
            // 
            resources.ApplyResources(this.ShowDetailsBtn, "ShowDetailsBtn");
            this.ShowDetailsBtn.CellHeight = 77;
            this.ShowDetailsBtn.CellWidth = 206;
            this.ShowDetailsBtn.Column = 0;
            this.ShowDetailsBtn.ColumnsSpanned = 0;
            this.ShowDetailsBtn.FlatAppearance.BorderSize = 0;
            this.ShowDetailsBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ShowDetailsBtn.Name = "ShowDetailsBtn";
            this.ShowDetailsBtn.Row = 0;
            this.ShowDetailsBtn.RowsSpanned = 0;
            this.ShowDetailsBtn.UseVisualStyleBackColor = false;
            this.ShowDetailsBtn.YOffset = 0;
            this.ShowDetailsBtn.Click += new System.EventHandler(this.ShowDetailsBtn_Click);
            // 
            // ShowFilesBtn
            // 
            resources.ApplyResources(this.ShowFilesBtn, "ShowFilesBtn");
            this.ShowFilesBtn.CellHeight = 61;
            this.ShowFilesBtn.CellWidth = 206;
            this.ShowFilesBtn.Column = 0;
            this.ShowFilesBtn.ColumnsSpanned = 0;
            this.ShowFilesBtn.FlatAppearance.BorderSize = 0;
            this.ShowFilesBtn.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ShowFilesBtn.Name = "ShowFilesBtn";
            this.ShowFilesBtn.Row = 1;
            this.ShowFilesBtn.RowsSpanned = 0;
            this.ShowFilesBtn.UseVisualStyleBackColor = false;
            this.ShowFilesBtn.YOffset = 0;
            this.ShowFilesBtn.Click += new System.EventHandler(this.ShowFilesBtn_Click);
            // 
            // gridPanel1
            // 
            resources.ApplyResources(this.gridPanel1, "gridPanel1");
            this.gridPanel1.CellHeight = 260;
            this.gridPanel1.CellWidth = 206;
            this.gridPanel1.Column = 2;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Controls.Add(this.DetailsCtrl);
            this.gridPanel1.Controls.Add(this.FilesCtrl);
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 0;
            this.gridPanel1.RowsSpanned = 2;
            this.gridPanel1.YOffset = 0;
            // 
            // DetailsCtrl
            // 
            this.DetailsCtrl.Autoreload = false;
            this.DetailsCtrl.DateModified = "";
            this.DetailsCtrl.Description = "";
            resources.ApplyResources(this.DetailsCtrl, "DetailsCtrl");
            this.DetailsCtrl.IncludeAutoreloadOption = true;
            this.DetailsCtrl.LabelName = "";
            this.DetailsCtrl.LastAccessed = "";
            this.DetailsCtrl.Name = "DetailsCtrl";
            this.DetailsCtrl.Owner = "";
            this.DetailsCtrl.Revision = "";
            // 
            // FilesCtrl
            // 
            resources.ApplyResources(this.FilesCtrl, "FilesCtrl");
            this.FilesCtrl.Name = "FilesCtrl";
            this.FilesCtrl.ParentControl = null;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackColor = System.Drawing.SystemColors.Menu;
            this.groupBox1.CellHeight = 260;
            this.groupBox1.CellWidth = 206;
            this.groupBox1.Column = 1;
            this.groupBox1.ColumnsSpanned = 0;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Row = 0;
            this.groupBox1.RowsSpanned = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.YOffset = 0;
            // 
            // LabelsToolWindowControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.LabelsControlSplitter);
            this.Name = "LabelsToolWindowControl";
            this.Load += new System.EventHandler(this.LabelsToolWindowControl_Load);
            this.LabelListCtxMenu.ResumeLayout(false);
            this.LabelsControlSplitter.Panel1.ResumeLayout(false);
            this.LabelsControlSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LabelsControlSplitter)).EndInit();
            this.LabelsControlSplitter.ResumeLayout(false);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.PerformLayout();
            this.gridLayoutPanel2.ResumeLayout(false);
            this.gridLayoutPanel2.PerformLayout();
            this.gridPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		public override void OnNewConnection(P4ScmProvider newScm)
		{
			if ((newScm != null) && (newScm.Connected))
			{
                if (OwnerCB.Text == "" &&
                OwnerCB.mruValues[1] == null)
                {
                    OwnerCB.Text = Scm.Connection.User;
                }
				filterBtn.Enabled = true;
				filterBtn_Click(this, null);
			}
			else
			{
				filterBtn.Enabled = false;
				matchesLbl.Text = Resources.JobsToolWindowControl_NoConnection;

				LabelList.Items.Clear();
				clearDetails();
				clearFiles();
			}
		}

		private delegate void setFilterBtnDelegate(bool filter);
		private delegate void setMatchesCntDelegate(int value);
		private delegate ListViewItem TreeListViewItemDelegate(TreeListViewItem item);

		private class filterArgs
		{
			public string UserName;
			public string LabelName;
			public bool CaseSensitive;
			public string File;
		}
		private void setFilterButtonEnabled(bool value)
		{
			filterBtn.Enabled = value;
		}
		private void setMatchesCnt(int value)
		{
			if (matchesLbl.InvokeRequired)
			{
				matchesLbl.BeginInvoke(new setMatchesCntDelegate(setMatchesCnt_Int), value);
			}
			else
			{
				setMatchesCnt_Int(value);
			}
		}
		private void setMatchesCnt_Int(int value)
		{
			switch (value)
			{
				case 0:
					matchesLbl.Text = string.Format(Resources.JobsToolWindowControl_NoMatches);
					break;
				case 1:
					matchesLbl.Text = string.Format(Resources.JobsToolWindowControl_1Match);
					break;
				default:
					if (value == maxItems)
					{
						matchesLbl.Text = string.Format(Resources.JobsToolWindowControl_nMatches, value.ToString() + "+");
					}
					else
					{
						matchesLbl.Text = string.Format(Resources.JobsToolWindowControl_nMatches, value);
					}
					break;

			}
		}
		private object SyncRoot = new object();

		public void LoadListProc(object parm)
		{
			if (filterBtn.InvokeRequired)
			{
				this.filterBtn.BeginInvoke(new setFilterBtnDelegate(setFilterButtonEnabled), false);
			}
			else
			{
				filterBtn.Enabled = false;
			}
			try
			{
				lock (SyncRoot)
				{
					if (parm == null)
					{
						return;
					}
					filterArgs args = parm as filterArgs;
					if (args == null)
					{
						return;
					}

					if ((Scm == null) || Scm.Connection.Disconnected)
					{
						return;
					}

                    Scm.Connection.Repository.Connection.getP4Server().ProgramName = "P4VS";
                    Scm.Connection.Repository.Connection.getP4Server().ProgramVersion = Versions.product();

                    threadMonitor.Value = 0;

					try
					{
						IList<P4.Label> labels = Scm.GetLabels(args.LabelName, args.CaseSensitive, args.UserName, args.File, maxItems);
						if (labels == null)
						{
                            P4.P4CommandResult results = Scm.Connection.Repository.Connection.LastResults;
							setMatchesCnt(0);
							if (results.Success == false)
							{
								P4ErrorDlg.Show(results, false);

								foreach (P4.P4ClientError error in results.ErrorList)
								{
									if (error.SeverityLevel <= P4.ErrorSeverity.E_WARN)
									{
										string msg = string.Format("{0}: {1}", error.SeverityLevel, error.ErrorMessage);
										P4VsOutputWindow.AppendMessage(msg);
									}
								}
							}
							return;
						}
						setMatchesCnt(labels.Count);
						
						int cnt = 0;
						int step = labels.Count / 10;
						if (step < 10)
						{
							step = 10;
						}
						foreach (P4.Label label in labels)
						{
							if (threadMonitor.CancelPressed)
							{
								return;
							}

							if (label == null)
							{
								continue;
							}
                            string access = "";
                            string update = "";
                            if (Preferences.LocalSettings.GetBool("P4Date_format", true))
                            {
                                access = label.Access.ToString("yyyy/MM/dd HH:mm:ss");
                                update = label.Update.ToString("yyyy/MM/dd HH:mm:ss");
                            }
                            else
                            {
                                access = label.Access.ToString();
                                update = label.Update.ToString();
                            }

							TreeListViewItem it = new TreeListViewItem(
								(TreeListViewItem)null,
								new string[] {label.Id,
								update,
								label.Owner,
								access,
								label.Description,
								label.Locked?"*":string.Empty});

							it.Tag = null;
							it.ImageIndex = 0;

							if (LabelList.InvokeRequired)
							{
								this.LabelList.Invoke(new TreeListViewItemDelegate(LabelList.Items.Add), it);
							}
							else
							{
								LabelList.Items.Add(it);
							}
							cnt++;
							if (cnt == step)
							{
								threadMonitor.Show(labels.Count);
							}
							if ((cnt % step) == 0)
							{
								threadMonitor.Value = cnt;
							}
						}
					}
					catch (P4.P4Exception ex)
					{
						P4ErrorDlg.Show(ex);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			catch (ThreadAbortException)
			{
				Thread.ResetAbort();
				return;
			}
			catch (Exception ex)
			{
				if (!threadMonitor.CancelPressed)
				{
					P4ErrorDlg.Show(ex.Message, false, false);
				}
			}
			finally
			{
				threadMonitor.Hide(); 
				if (filterBtn.InvokeRequired)
				{
					this.filterBtn.BeginInvoke(new setFilterBtnDelegate(setFilterButtonEnabled), true);
				}
				else
				{
					filterBtn.Enabled = true;
				}
			}
		}

		Thread FillInListProc = null;

		public void LoadList()
		{
			clearDetails();
			clearFiles();

			filterArgs args = new filterArgs();

			if (string.IsNullOrEmpty(OwnerCB.Text))
			{
				args.UserName = null;
			}
			else
			{
				args.UserName = OwnerCB.Text;
			}

			if (string.IsNullOrEmpty(LabelNameFilterCB.Text))
			{
				args.LabelName = null;
			}
			else
			{
				switch (NameFilterCB.SelectedIndex)
				{
					default:
					case 0:
						args.LabelName = string.Format("*{0}*", LabelNameFilterCB.Text);
						break;
					case 1:
						args.LabelName = LabelNameFilterCB.Text;
						break;
					case 2:
						args.LabelName = string.Format("{0}*", LabelNameFilterCB.Text);
						break;
					case 3:
						args.LabelName = string.Format("*{0}", LabelNameFilterCB.Text);
						break;
				}
			}

			args.CaseSensitive = LabelNameMatchCaseCB.Checked;

			if (string.IsNullOrEmpty(FileFilterCB.Text))
			{
				args.File = null;
			}
			else
			{
				args.File = FileFilterCB.Text;
			}

			if ((FillInListProc != null) && (FillInListProc.IsAlive))
			{
				// will call abort on the thread
				threadMonitor.CancelPressed = true;

				// might have been nulled out if the proc completed asynchronously 
				if (FillInListProc != null)
				{
					FillInListProc.Join(1000);
				}
			}

			LabelList.Items.Clear();

			FillInListProc = new Thread(new ParameterizedThreadStart(LoadListProc));
			FillInListProc.IsBackground = true;
			FillInListProc.Name = "LabelsToolWindowControl.LoadListProc";

			FillInListProc.Start(args);
			// LoadListProc((object)args);

            SaveControlSettings();
		}

		private void clearDetails()
		{
			DetailsCtrl.LabelName = string.Empty;
			DetailsCtrl.DateModified = string.Empty;
			DetailsCtrl.LastAccessed = string.Empty;
			DetailsCtrl.Owner = string.Empty;
			DetailsCtrl.Description = string.Empty;
			DetailsCtrl.Locked = false;
			DetailsCtrl.IncludeAutoreloadOption = false;
			DetailsCtrl.Autoreload = false;
			DetailsCtrl.Revision = string.Empty;
			DetailsCtrl.View.Clear();
		}

		private void setDetails()
		{
			if (Label == null)
			{
				clearDetails();
				return;
			}

            string access = "";
            string update = "";
            if (Preferences.LocalSettings.GetBool("P4Date_format", true))
            {
                access = Label.Access.ToString("yyyy/MM/dd HH:mm:ss");
                update = Label.Update.ToString("yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                access = Label.Access.ToString();
                update = Label.Update.ToString();
            }
			DetailsCtrl.LabelName = Label.Id;
			DetailsCtrl.DateModified = update;
			DetailsCtrl.LastAccessed = access;
			DetailsCtrl.Owner = Label.Owner;
			DetailsCtrl.Description = Label.Description;
			DetailsCtrl.Locked = Label.Locked;
			DetailsCtrl.IncludeAutoreloadOption = Label.IncludeAutoreloadOption;
			DetailsCtrl.Autoreload = Label.Autoreload;
			DetailsCtrl.Revision = Label.Revision;

			DetailsCtrl.View.Clear();
			if (Label.ViewMap != null)
			{
				foreach (P4.MapEntry entry in Label.ViewMap)
				{
					DetailsCtrl.View.Add(entry.Left.Path);
				}
			}
		}

		public void clearFiles()
		{
			FilesCtrl.WorkspaceFilesList.Items.Clear();
			FilesCtrl.LabeledRevisionsList.Items.Clear();
		}

		internal void setFiles()
		{
			try
			{
				FilesCtrl.ParentControl = this;

				FilesCtrl.WorkspaceFilesList.Items.Clear();
				FilesCtrl.LabeledRevisionsList.Items.Clear();

				if (Label == null)
				{
					return;
				}
				P4.Options options = new P4.Options();
				options["-Olhp"] = null;
                if (FilesCtrl.maxItems != 0)
                {
#pragma warning disable 1690
    				options["-m"] = FilesCtrl.maxItems.ToString();
#pragma warning restore 1690
                }

				P4.FileSpec fs = P4.FileSpec.DepotSpec("//...");
				fs.Version = new P4.LabelNameVersion(Label.Id);

				IList<P4.FileMetaData> files = Scm.GetFileMetaData(options, fs);

				if (files != null)
				{
					if (FilesCtrl.ShowWorkspaceFilesList)
					{
						try
						{
							FilesCtrl.WorkspaceFilesList.SuspendLayout();

                            foreach (P4.FileMetaData md in files)
                            {
                                if (md.LocalPath == null)
                                {
                                    continue;
                                }
                                P4FileTreeListViewItem it = null;
                                P4.FileMetaData fileData = null;

                                if (Scm.IsFileCached(md.LocalPath.Path))
                                {
                                    fileData = Scm.Fetch(md.LocalPath.Path);
                                }
                                else
                                {
                                    P4.Options o = new P4.GetFileMetaDataCmdOptions(P4.GetFileMetadataCmdFlags.FileSize, null, null, -1, null, null, null);
                                    IList<P4.FileMetaData> mds = Scm.GetFileMetaData(o, new P4.FileSpec(null, null, md.LocalPath, null));
                                    if (mds != null)
                                    {
                                        fileData = mds[0];
                                    }
                                }
                                if ((fileData != null) && (fileData.HaveRev > 0))
                                {
                                    it = new P4FileTreeListViewItem(null, fileData, WorkspacesFilesListViewFields);
                                    FilesCtrl.WorkspaceFilesList.Items.Add(it);
                                }
                            }
							if ((FilesCtrl.maxItems > 0) && (files.Count >= FilesCtrl.maxItems))
							{
								TreeListViewItem it = new TreeListViewItem(null, 
									string.Format("Label contains more than {0} files",FilesCtrl.maxItems), true);
								FilesCtrl.WorkspaceFilesList.Items.Add(it);
							}
						}
						finally
						{
							FilesCtrl.WorkspaceFilesList.ResumeLayout();
						}
					}
					else
					{
						try
						{
							FilesCtrl.LabeledRevisionsList.SuspendLayout();

							foreach (P4.FileMetaData file in files)
							{
								TreeListViewItem it = null;
								string fileName = string.Empty;
								string directoryName = string.Empty;
								try
								{
									fileName = file.DepotPath.Path;
									directoryName = file.DepotPath.GetDirectoryName();
									if (string.IsNullOrEmpty(directoryName) == false)
									{
										directoryName = directoryName.Replace('\\', '/');
									}

									char[] badChars = Path.GetInvalidPathChars();
									if (fileName.IndexOfAny(badChars) >= 0)
									{
										foreach (char c in badChars)
										{
											fileName = fileName.Replace(c, '¿');
										}
									}

									it = new TreeListViewItem(null,
										new string[] {
											Path.GetFileName(fileName),
											(file.HeadRev >= 0)?string.Format("{0}",file.HeadRev):string.Empty,
											file.HeadAction.ToString(),
											file.Type.ToString(),
											directoryName!=null?directoryName:string.Empty,
										});
								}
								catch (Exception ex)
								{
									logger.Trace(ex.Message);
									logger.Trace(ex.StackTrace);
									throw;
								}
								it.ImageIndex = 2;
								switch (file.Action)
								{
									case P4.FileAction.Add:
									case P4.FileAction.Added:
									case P4.FileAction.AddInto:
										it.ImageIndex = 0;
										break;

									case P4.FileAction.Branch:
									case P4.FileAction.BranchFrom:
									case P4.FileAction.BranchInto:
									case P4.FileAction.CopyFrom:
									case P4.FileAction.CopyInto:
										it.ImageIndex = 3;
										break;

									case P4.FileAction.Delete:
									case P4.FileAction.DeleteFrom:
									case P4.FileAction.DeleteInto:
										it.ImageIndex = 4;
										break;

									case P4.FileAction.Edit:
									case P4.FileAction.EditFrom:
									case P4.FileAction.EditIgnored:
									case P4.FileAction.EditInto:
										it.ImageIndex = 5;
										break;

									case P4.FileAction.Integrate:
									case P4.FileAction.MergeFrom:
									case P4.FileAction.MergeInto:
										it.ImageIndex = 6;
										break;

									case P4.FileAction.Move:
									case P4.FileAction.MoveAdd:
									case P4.FileAction.MovedInto:
										it.ImageIndex = 7;
										break;

									case P4.FileAction.MoveDelete:
										it.ImageIndex = 8;
										break;

									case P4.FileAction.Purge:
										it.ImageIndex = 9;
										break;

								}
								FilesCtrl.LabeledRevisionsList.Items.Add(it);
							}
							if ((FilesCtrl.maxItems > 0) && (files.Count >= FilesCtrl.maxItems))
							{
								TreeListViewItem it = new TreeListViewItem(null,
									string.Format("Label contains more than {0} files", FilesCtrl.maxItems), true);
								FilesCtrl.LabeledRevisionsList.Items.Add(it);
							}
						}
						finally
						{
							FilesCtrl.LabeledRevisionsList.ResumeLayout();
						}
					}
				}
				else
				{
                    P4.P4CommandResult results = Scm.Connection.Repository.Connection.LastResults;
					//P4ErrorDlg.Show(results, false);

					foreach (P4.P4ClientError error in results.ErrorList)
					{
						if (error.SeverityLevel < P4.ErrorSeverity.E_WARN)
						{
							string msg = string.Format("{0}: {1}", error.SeverityLevel, error.ErrorMessage);
							P4VsOutputWindow.AppendMessage(msg);
						}
					}
				}
			}
			catch (P4.P4Exception ex)
			{
				P4ErrorDlg.Show(ex);
			}
			catch (Exception ex)
			{
				P4ErrorDlg.Show(ex.Message, false, false);
			}
		}

		private Button ActiveButton = null; //ShowDetailsBtn;
		private UserControl ActiveTab = null; //DetailsTab;

		private void SwitchTab(Button NewButton, UserControl NewTab)
		{
			if (ActiveButton.Text == NewButton.Text)
			{
				// no change
				return;
			}
			ActiveButton.ImageIndex = 2;
			ActiveButton.ForeColor = SystemColors.GrayText;

			ActiveTab.Visible = false;
			//ActiveTab.SendToBack();

			ActiveButton = NewButton;
			ActiveTab = NewTab;

			ActiveButton.ImageIndex = 0;
			ActiveButton.ForeColor = SystemColors.ControlText;

			if (ActiveButton.Text == ShowFilesBtn.Text)
			{
				setFiles();
			}
			//ActiveTab.Visible = true;
			//ActiveTab.BringToFront();
		}

		private void ShowDetailsBtn_Click(object sender, EventArgs e)
		{
			SwitchTab(ShowDetailsBtn, DetailsCtrl);
			DetailsCtrl.Visible = true;
		}

		private void ShowFilesBtn_Click(object sender, EventArgs e)
		{
			SwitchTab(ShowFilesBtn, FilesCtrl);
			FilesCtrl.Visible = true;
		}

		public P4.Label Label { get; set; }

		private void LabelList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((LabelList.SelectedItems == null) || (LabelList.SelectedItems.Count <= 0))
			{
				Label = null;
			}
			else
			{
				TreeListViewItem it = LabelList.SelectedItems[0] as TreeListViewItem;
				Label = it.Tag as P4.Label;

				if (Label == null)
				{
					it.Tag = Scm.GetLabel(it.SubItems[0].Text);

					Label = it.Tag as P4.Label;
				}
			}
			FilesCtrl.maxItems = (int)Preferences.LocalSettings.GetInt("Number_specs", 100);

			setDetails();
			setFiles();
		}

		private void filterBtn_Click(object sender, EventArgs e)
		{
			maxItems = (int)Preferences.LocalSettings.GetInt("Number_specs", 100);
			LoadList();
		}

		private void LabelList_DoubleClick(object sender, EventArgs e)
		{
			if (this.ParentForm is LabelsBrowserDlg)
			{
				LabelsBrowserDlg dlg = this.ParentForm as LabelsBrowserDlg;
				dlg.DialogResult = DialogResult.OK;
				dlg.Close();
			}
		}

		private void FileFilterCB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				filterBtn_Click(this, null)	;	
			}
		}

		private void OwnerCB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				filterBtn_Click(this, null);
			}
		}

		private void LabelNameFilterCB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				filterBtn_Click(this, null);
			}
		}

		int maxItems;

		private void LabelList_onMaxScroll(object sender, ScrollEventArgs e)
		{
			if ((maxItems > 0) && (LabelList.Items.Count >= maxItems))
			{
				maxItems += (int)Preferences.LocalSettings.GetInt("Number_specs", 100);
				LoadList();
			}
		}

		private void LabelListCtxMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem == LabelListCtxMnyRefreshList)
			{
				LoadList();
			}
		}

        private void LabelsToolWindowControl_Load(object sender, EventArgs e)
        {
            clearDetails();
            clearFiles();

            if (OwnerCB.Text == "" &&
                 OwnerCB.mruValues[1] == null)
            {
                if (Scm != null)
                {
                    OwnerCB.Text = Scm.Connection.User;
                }
            }
            filterBtn.PerformClick();
            //LoadList();
        }
	}
}
