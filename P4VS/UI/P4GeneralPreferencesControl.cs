
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.Shell.Interop;
using Perforce.P4Scm;

namespace Perforce.P4VS
{
	/// <summary>
	/// Summary description for P4GeneralPreferencesControl.
	/// </summary>
	public class P4GeneralPreferencesControl : System.Windows.Forms.UserControl
	{

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private I18nControls.GridCheckBox revertWarnChk;
        private I18nControls.GridCheckBox writeBindingsChk;
        private I18nControls.GridCheckBox checkoutLockChk;
        private I18nControls.GridRadioButton P4FormatRB;
        private I18nControls.GridCheckBox promptChk;
        private I18nControls.GridRadioButton OSFormatRB;
        private I18nControls.GridCheckBox updateChk;
        private I18nControls.GridCheckBox OpenShelvedFileInEditorChk;
        private I18nControls.GridCheckBox addChk;
        private I18nControls.GridCheckBox setProjLocationChk;
        private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
        private I18nControls.GridLabel displayLbl;
        private I18nControls.GridGroupBox displayGB;
        private I18nControls.GridLayoutSubpanel gridLayoutSubpanel2;
        private I18nControls.GridLabel filesAndFoldersLbl;
        private I18nControls.GridGroupBox filesAndFoldersGB;
        private I18nControls.GridLayoutSubpanel gridLayoutSubpanel3;
        private I18nControls.GridLabel label1;
        private I18nControls.GridGroupBox groupBox1;
        private I18nControls.GridPanel gridPanel1;
        private I18nControls.GridLayoutSubpanel gridLayoutSubpanel4;
        private I18nControls.GridPanel gridPanel2;
        private I18nControls.GridComboBox QueryEditSyncOptionCB;
        private I18nControls.GridLabel gridLabel1;
        private I18nControls.GridLayoutPanel gridLayoutPanel1;
        private I18nControls.GridPanel gridPanel3;
        private I18nControls.GridCheckBox warnSlnWorkspaceChk;
        private I18nControls.GridCheckBox promptDel;
        private I18nControls.GridCheckBox checkoutWriteableChk;
        private I18nControls.GridCheckBox promptSlnSync;































        // The parent page, use to persist data
        private P4GeneralPreferences _customPage;

		public P4GeneralPreferencesControl()
		{
			P4GeneralPreferencesControl.Instance = this;

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
				GC.SuppressFinalize(this);
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(P4GeneralPreferencesControl));
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.promptSlnSync = new Perforce.I18nControls.GridCheckBox();
            this.checkoutWriteableChk = new Perforce.I18nControls.GridCheckBox();
            this.promptDel = new Perforce.I18nControls.GridCheckBox();
            this.warnSlnWorkspaceChk = new Perforce.I18nControls.GridCheckBox();
            this.gridPanel3 = new Perforce.I18nControls.GridPanel();
            this.gridLayoutSubpanel4 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.gridPanel2 = new Perforce.I18nControls.GridPanel();
            this.QueryEditSyncOptionCB = new Perforce.I18nControls.GridComboBox();
            this.gridLabel1 = new Perforce.I18nControls.GridLabel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.gridLayoutSubpanel3 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.label1 = new Perforce.I18nControls.GridLabel();
            this.groupBox1 = new Perforce.I18nControls.GridGroupBox();
            this.gridLayoutSubpanel2 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.filesAndFoldersLbl = new Perforce.I18nControls.GridLabel();
            this.filesAndFoldersGB = new Perforce.I18nControls.GridGroupBox();
            this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.displayLbl = new Perforce.I18nControls.GridLabel();
            this.displayGB = new Perforce.I18nControls.GridGroupBox();
            this.setProjLocationChk = new Perforce.I18nControls.GridCheckBox();
            this.addChk = new Perforce.I18nControls.GridCheckBox();
            this.OpenShelvedFileInEditorChk = new Perforce.I18nControls.GridCheckBox();
            this.updateChk = new Perforce.I18nControls.GridCheckBox();
            this.OSFormatRB = new Perforce.I18nControls.GridRadioButton();
            this.promptChk = new Perforce.I18nControls.GridCheckBox();
            this.P4FormatRB = new Perforce.I18nControls.GridRadioButton();
            this.checkoutLockChk = new Perforce.I18nControls.GridCheckBox();
            this.writeBindingsChk = new Perforce.I18nControls.GridCheckBox();
            this.revertWarnChk = new Perforce.I18nControls.GridCheckBox();
            this.gridLayoutPanel1.SuspendLayout();
            this.gridLayoutSubpanel4.SuspendLayout();
            this.gridLayoutSubpanel3.SuspendLayout();
            this.gridLayoutSubpanel2.SuspendLayout();
            this.gridLayoutSubpanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridLayoutPanel1
            // 
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.Controls.Add(this.promptSlnSync);
            this.gridLayoutPanel1.Controls.Add(this.checkoutWriteableChk);
            this.gridLayoutPanel1.Controls.Add(this.promptDel);
            this.gridLayoutPanel1.Controls.Add(this.warnSlnWorkspaceChk);
            this.gridLayoutPanel1.Controls.Add(this.gridPanel3);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel4);
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel3);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel2);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel1);
            this.gridLayoutPanel1.Controls.Add(this.setProjLocationChk);
            this.gridLayoutPanel1.Controls.Add(this.addChk);
            this.gridLayoutPanel1.Controls.Add(this.OpenShelvedFileInEditorChk);
            this.gridLayoutPanel1.Controls.Add(this.updateChk);
            this.gridLayoutPanel1.Controls.Add(this.OSFormatRB);
            this.gridLayoutPanel1.Controls.Add(this.promptChk);
            this.gridLayoutPanel1.Controls.Add(this.P4FormatRB);
            this.gridLayoutPanel1.Controls.Add(this.checkoutLockChk);
            this.gridLayoutPanel1.Controls.Add(this.writeBindingsChk);
            this.gridLayoutPanel1.Controls.Add(this.revertWarnChk);
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 25;
            this.gridLayoutPanel1.MinimumRowHeight = 0;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // promptSlnSync
            // 
            resources.ApplyResources(this.promptSlnSync, "promptSlnSync");
            this.promptSlnSync.CellHeight = 23;
            this.promptSlnSync.CellWidth = 764;
            this.promptSlnSync.Column = 1;
            this.promptSlnSync.ColumnsSpanned = 0;
            this.promptSlnSync.Name = "promptSlnSync";
            this.promptSlnSync.Row = 17;
            this.promptSlnSync.RowsSpanned = 0;
            this.promptSlnSync.UseVisualStyleBackColor = true;
            this.promptSlnSync.YOffset = 0;
            // 
            // checkoutWriteableChk
            // 
            resources.ApplyResources(this.checkoutWriteableChk, "checkoutWriteableChk");
            this.checkoutWriteableChk.CellHeight = 23;
            this.checkoutWriteableChk.CellWidth = 764;
            this.checkoutWriteableChk.Column = 1;
            this.checkoutWriteableChk.ColumnsSpanned = 0;
            this.checkoutWriteableChk.Name = "checkoutWriteableChk";
            this.checkoutWriteableChk.Row = 7;
            this.checkoutWriteableChk.RowsSpanned = 0;
            this.checkoutWriteableChk.UseVisualStyleBackColor = true;
            this.checkoutWriteableChk.YOffset = 0;
            // 
            // promptDel
            // 
            resources.ApplyResources(this.promptDel, "promptDel");
            this.promptDel.CellHeight = 23;
            this.promptDel.CellWidth = 764;
            this.promptDel.Column = 1;
            this.promptDel.ColumnsSpanned = 0;
            this.promptDel.Name = "promptDel";
            this.promptDel.Row = 8;
            this.promptDel.RowsSpanned = 0;
            this.promptDel.UseVisualStyleBackColor = true;
            this.promptDel.YOffset = 0;
            // 
            // warnSlnWorkspaceChk
            // 
            resources.ApplyResources(this.warnSlnWorkspaceChk, "warnSlnWorkspaceChk");
            this.warnSlnWorkspaceChk.CellHeight = 23;
            this.warnSlnWorkspaceChk.CellWidth = 764;
            this.warnSlnWorkspaceChk.Column = 1;
            this.warnSlnWorkspaceChk.ColumnsSpanned = 0;
            this.warnSlnWorkspaceChk.Name = "warnSlnWorkspaceChk";
            this.warnSlnWorkspaceChk.Row = 16;
            this.warnSlnWorkspaceChk.RowsSpanned = 0;
            this.warnSlnWorkspaceChk.UseVisualStyleBackColor = true;
            this.warnSlnWorkspaceChk.YOffset = 0;
            // 
            // gridPanel3
            // 
            resources.ApplyResources(this.gridPanel3, "gridPanel3");
            this.gridPanel3.CellHeight = 82;
            this.gridPanel3.CellWidth = 25;
            this.gridPanel3.Column = 0;
            this.gridPanel3.ColumnsSpanned = 0;
            this.gridPanel3.Name = "gridPanel3";
            this.gridPanel3.Row = 18;
            this.gridPanel3.RowsSpanned = 0;
            this.gridPanel3.YOffset = 0;
            // 
            // gridLayoutSubpanel4
            // 
            resources.ApplyResources(this.gridLayoutSubpanel4, "gridLayoutSubpanel4");
            this.gridLayoutSubpanel4.CellHeight = 27;
            this.gridLayoutSubpanel4.CellWidth = 764;
            this.gridLayoutSubpanel4.Column = 1;
            this.gridLayoutSubpanel4.ColumnsSpanned = 0;
            this.gridLayoutSubpanel4.Controls.Add(this.gridPanel2);
            this.gridLayoutSubpanel4.Controls.Add(this.QueryEditSyncOptionCB);
            this.gridLayoutSubpanel4.Controls.Add(this.gridLabel1);
            this.gridLayoutSubpanel4.EnableDesignerGrid = false;
            this.gridLayoutSubpanel4.EnableDesignerLayout = false;
            this.gridLayoutSubpanel4.EnableParentResize = false;
            this.gridLayoutSubpanel4.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel4.MinimumRowHeight = 10;
            this.gridLayoutSubpanel4.Name = "gridLayoutSubpanel4";
            this.gridLayoutSubpanel4.Row = 12;
            this.gridLayoutSubpanel4.RowsSpanned = 0;
            this.gridLayoutSubpanel4.YOffset = 0;
            // 
            // gridPanel2
            // 
            resources.ApplyResources(this.gridPanel2, "gridPanel2");
            this.gridPanel2.CellHeight = 21;
            this.gridPanel2.CellWidth = 282;
            this.gridPanel2.Column = 2;
            this.gridPanel2.ColumnsSpanned = 0;
            this.gridPanel2.Name = "gridPanel2";
            this.gridPanel2.Row = 0;
            this.gridPanel2.RowsSpanned = 0;
            this.gridPanel2.YOffset = 0;
            // 
            // QueryEditSyncOptionCB
            // 
            resources.ApplyResources(this.QueryEditSyncOptionCB, "QueryEditSyncOptionCB");
            this.QueryEditSyncOptionCB.CellHeight = 21;
            this.QueryEditSyncOptionCB.CellWidth = 283;
            this.QueryEditSyncOptionCB.Column = 1;
            this.QueryEditSyncOptionCB.ColumnsSpanned = 0;
            this.QueryEditSyncOptionCB.DesignSize = new System.Drawing.Size(0, 0);
            this.QueryEditSyncOptionCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.QueryEditSyncOptionCB.FormattingEnabled = true;
            this.QueryEditSyncOptionCB.Items.AddRange(new object[] {
            resources.GetString("QueryEditSyncOptionCB.Items"),
            resources.GetString("QueryEditSyncOptionCB.Items1"),
            resources.GetString("QueryEditSyncOptionCB.Items2")});
            this.QueryEditSyncOptionCB.Name = "QueryEditSyncOptionCB";
            this.QueryEditSyncOptionCB.Row = 0;
            this.QueryEditSyncOptionCB.RowsSpanned = 0;
            this.QueryEditSyncOptionCB.YOffset = 0;
            // 
            // gridLabel1
            // 
            resources.ApplyResources(this.gridLabel1, "gridLabel1");
            this.gridLabel1.CellHeight = 21;
            this.gridLabel1.CellWidth = 193;
            this.gridLabel1.Column = 0;
            this.gridLabel1.ColumnsSpanned = 0;
            this.gridLabel1.Name = "gridLabel1";
            this.gridLabel1.Row = 0;
            this.gridLabel1.RowsSpanned = 0;
            this.gridLabel1.YOffset = 4;
            // 
            // gridPanel1
            // 
            resources.ApplyResources(this.gridPanel1, "gridPanel1");
            this.gridPanel1.CellHeight = 82;
            this.gridPanel1.CellWidth = 764;
            this.gridPanel1.Column = 1;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 18;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 0;
            // 
            // gridLayoutSubpanel3
            // 
            resources.ApplyResources(this.gridLayoutSubpanel3, "gridLayoutSubpanel3");
            this.gridLayoutSubpanel3.CellHeight = 29;
            this.gridLayoutSubpanel3.CellWidth = 789;
            this.gridLayoutSubpanel3.Column = 0;
            this.gridLayoutSubpanel3.ColumnsSpanned = 1;
            this.gridLayoutSubpanel3.Controls.Add(this.label1);
            this.gridLayoutSubpanel3.Controls.Add(this.groupBox1);
            this.gridLayoutSubpanel3.EnableDesignerGrid = false;
            this.gridLayoutSubpanel3.EnableDesignerLayout = false;
            this.gridLayoutSubpanel3.EnableParentResize = false;
            this.gridLayoutSubpanel3.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel3.MinimumRowHeight = 10;
            this.gridLayoutSubpanel3.Name = "gridLayoutSubpanel3";
            this.gridLayoutSubpanel3.Row = 13;
            this.gridLayoutSubpanel3.RowsSpanned = 0;
            this.gridLayoutSubpanel3.YOffset = 0;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.CellHeight = 0;
            this.label1.CellWidth = 0;
            this.label1.Column = 0;
            this.label1.ColumnsSpanned = 0;
            this.label1.Name = "label1";
            this.label1.Row = 0;
            this.label1.RowsSpanned = 0;
            this.label1.YOffset = 0;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.CellHeight = 106;
            this.groupBox1.CellWidth = 494;
            this.groupBox1.Column = 0;
            this.groupBox1.ColumnsSpanned = 0;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Row = 0;
            this.groupBox1.RowsSpanned = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.YOffset = 0;
            // 
            // gridLayoutSubpanel2
            // 
            resources.ApplyResources(this.gridLayoutSubpanel2, "gridLayoutSubpanel2");
            this.gridLayoutSubpanel2.CellHeight = 28;
            this.gridLayoutSubpanel2.CellWidth = 789;
            this.gridLayoutSubpanel2.Column = 0;
            this.gridLayoutSubpanel2.ColumnsSpanned = 1;
            this.gridLayoutSubpanel2.Controls.Add(this.filesAndFoldersLbl);
            this.gridLayoutSubpanel2.Controls.Add(this.filesAndFoldersGB);
            this.gridLayoutSubpanel2.EnableDesignerGrid = false;
            this.gridLayoutSubpanel2.EnableDesignerLayout = false;
            this.gridLayoutSubpanel2.EnableParentResize = false;
            this.gridLayoutSubpanel2.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel2.MinimumRowHeight = 10;
            this.gridLayoutSubpanel2.Name = "gridLayoutSubpanel2";
            this.gridLayoutSubpanel2.Row = 3;
            this.gridLayoutSubpanel2.RowsSpanned = 0;
            this.gridLayoutSubpanel2.YOffset = 0;
            // 
            // filesAndFoldersLbl
            // 
            resources.ApplyResources(this.filesAndFoldersLbl, "filesAndFoldersLbl");
            this.filesAndFoldersLbl.CellHeight = 0;
            this.filesAndFoldersLbl.CellWidth = 0;
            this.filesAndFoldersLbl.Column = 0;
            this.filesAndFoldersLbl.ColumnsSpanned = 0;
            this.filesAndFoldersLbl.Name = "filesAndFoldersLbl";
            this.filesAndFoldersLbl.Row = 0;
            this.filesAndFoldersLbl.RowsSpanned = 0;
            this.filesAndFoldersLbl.YOffset = 0;
            // 
            // filesAndFoldersGB
            // 
            resources.ApplyResources(this.filesAndFoldersGB, "filesAndFoldersGB");
            this.filesAndFoldersGB.CellHeight = 106;
            this.filesAndFoldersGB.CellWidth = 494;
            this.filesAndFoldersGB.Column = 0;
            this.filesAndFoldersGB.ColumnsSpanned = 0;
            this.filesAndFoldersGB.Name = "filesAndFoldersGB";
            this.filesAndFoldersGB.Row = 0;
            this.filesAndFoldersGB.RowsSpanned = 0;
            this.filesAndFoldersGB.TabStop = false;
            this.filesAndFoldersGB.YOffset = 0;
            // 
            // gridLayoutSubpanel1
            // 
            resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
            this.gridLayoutSubpanel1.CellHeight = 24;
            this.gridLayoutSubpanel1.CellWidth = 789;
            this.gridLayoutSubpanel1.Column = 0;
            this.gridLayoutSubpanel1.ColumnsSpanned = 1;
            this.gridLayoutSubpanel1.Controls.Add(this.displayLbl);
            this.gridLayoutSubpanel1.Controls.Add(this.displayGB);
            this.gridLayoutSubpanel1.EnableDesignerGrid = false;
            this.gridLayoutSubpanel1.EnableDesignerLayout = false;
            this.gridLayoutSubpanel1.EnableParentResize = false;
            this.gridLayoutSubpanel1.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel1.MinimumRowHeight = 10;
            this.gridLayoutSubpanel1.Name = "gridLayoutSubpanel1";
            this.gridLayoutSubpanel1.Row = 0;
            this.gridLayoutSubpanel1.RowsSpanned = 0;
            this.gridLayoutSubpanel1.YOffset = 0;
            // 
            // displayLbl
            // 
            resources.ApplyResources(this.displayLbl, "displayLbl");
            this.displayLbl.CellHeight = 106;
            this.displayLbl.CellWidth = 458;
            this.displayLbl.Column = 0;
            this.displayLbl.ColumnsSpanned = 0;
            this.displayLbl.Name = "displayLbl";
            this.displayLbl.Row = 0;
            this.displayLbl.RowsSpanned = 0;
            this.displayLbl.YOffset = 0;
            // 
            // displayGB
            // 
            resources.ApplyResources(this.displayGB, "displayGB");
            this.displayGB.CellHeight = 106;
            this.displayGB.CellWidth = 323;
            this.displayGB.Column = 1;
            this.displayGB.ColumnsSpanned = 0;
            this.displayGB.Name = "displayGB";
            this.displayGB.Row = 0;
            this.displayGB.RowsSpanned = 0;
            this.displayGB.TabStop = false;
            this.displayGB.YOffset = 0;
            // 
            // setProjLocationChk
            // 
            resources.ApplyResources(this.setProjLocationChk, "setProjLocationChk");
            this.setProjLocationChk.CellHeight = 23;
            this.setProjLocationChk.CellWidth = 764;
            this.setProjLocationChk.Column = 1;
            this.setProjLocationChk.ColumnsSpanned = 0;
            this.setProjLocationChk.Name = "setProjLocationChk";
            this.setProjLocationChk.Row = 15;
            this.setProjLocationChk.RowsSpanned = 0;
            this.setProjLocationChk.UseVisualStyleBackColor = true;
            this.setProjLocationChk.YOffset = 0;
            this.setProjLocationChk.CheckedChanged += new System.EventHandler(this.setProjLocationChk_CheckedChanged);
            // 
            // addChk
            // 
            resources.ApplyResources(this.addChk, "addChk");
            this.addChk.CellHeight = 23;
            this.addChk.CellWidth = 764;
            this.addChk.Checked = true;
            this.addChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.addChk.Column = 1;
            this.addChk.ColumnsSpanned = 0;
            this.addChk.Name = "addChk";
            this.addChk.Row = 9;
            this.addChk.RowsSpanned = 0;
            this.addChk.UseVisualStyleBackColor = true;
            this.addChk.YOffset = 0;
            // 
            // OpenShelvedFileInEditorChk
            // 
            resources.ApplyResources(this.OpenShelvedFileInEditorChk, "OpenShelvedFileInEditorChk");
            this.OpenShelvedFileInEditorChk.CellHeight = 23;
            this.OpenShelvedFileInEditorChk.CellWidth = 764;
            this.OpenShelvedFileInEditorChk.Checked = true;
            this.OpenShelvedFileInEditorChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.OpenShelvedFileInEditorChk.Column = 1;
            this.OpenShelvedFileInEditorChk.ColumnsSpanned = 0;
            this.OpenShelvedFileInEditorChk.Name = "OpenShelvedFileInEditorChk";
            this.OpenShelvedFileInEditorChk.Row = 11;
            this.OpenShelvedFileInEditorChk.RowsSpanned = 0;
            this.OpenShelvedFileInEditorChk.UseVisualStyleBackColor = true;
            this.OpenShelvedFileInEditorChk.YOffset = 0;
            // 
            // updateChk
            // 
            resources.ApplyResources(this.updateChk, "updateChk");
            this.updateChk.CellHeight = 23;
            this.updateChk.CellWidth = 764;
            this.updateChk.Checked = true;
            this.updateChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateChk.Column = 1;
            this.updateChk.ColumnsSpanned = 0;
            this.updateChk.Name = "updateChk";
            this.updateChk.Row = 10;
            this.updateChk.RowsSpanned = 0;
            this.updateChk.UseVisualStyleBackColor = true;
            this.updateChk.YOffset = 0;
            // 
            // OSFormatRB
            // 
            resources.ApplyResources(this.OSFormatRB, "OSFormatRB");
            this.OSFormatRB.CellHeight = 23;
            this.OSFormatRB.CellWidth = 764;
            this.OSFormatRB.Column = 1;
            this.OSFormatRB.ColumnsSpanned = 0;
            this.OSFormatRB.Name = "OSFormatRB";
            this.OSFormatRB.Row = 1;
            this.OSFormatRB.RowsSpanned = 0;
            this.OSFormatRB.UseVisualStyleBackColor = true;
            this.OSFormatRB.YOffset = 0;
            // 
            // promptChk
            // 
            resources.ApplyResources(this.promptChk, "promptChk");
            this.promptChk.CellHeight = 23;
            this.promptChk.CellWidth = 764;
            this.promptChk.Checked = true;
            this.promptChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.promptChk.Column = 1;
            this.promptChk.ColumnsSpanned = 0;
            this.promptChk.Name = "promptChk";
            this.promptChk.Row = 6;
            this.promptChk.RowsSpanned = 0;
            this.promptChk.UseVisualStyleBackColor = true;
            this.promptChk.YOffset = 0;
            // 
            // P4FormatRB
            // 
            resources.ApplyResources(this.P4FormatRB, "P4FormatRB");
            this.P4FormatRB.CellHeight = 23;
            this.P4FormatRB.CellWidth = 764;
            this.P4FormatRB.Checked = true;
            this.P4FormatRB.Column = 1;
            this.P4FormatRB.ColumnsSpanned = 0;
            this.P4FormatRB.Name = "P4FormatRB";
            this.P4FormatRB.Row = 2;
            this.P4FormatRB.RowsSpanned = 0;
            this.P4FormatRB.TabStop = true;
            this.P4FormatRB.UseVisualStyleBackColor = true;
            this.P4FormatRB.YOffset = 0;
            // 
            // checkoutLockChk
            // 
            resources.ApplyResources(this.checkoutLockChk, "checkoutLockChk");
            this.checkoutLockChk.CellHeight = 23;
            this.checkoutLockChk.CellWidth = 764;
            this.checkoutLockChk.Column = 1;
            this.checkoutLockChk.ColumnsSpanned = 0;
            this.checkoutLockChk.Name = "checkoutLockChk";
            this.checkoutLockChk.Row = 5;
            this.checkoutLockChk.RowsSpanned = 0;
            this.checkoutLockChk.UseVisualStyleBackColor = true;
            this.checkoutLockChk.YOffset = 0;
            // 
            // writeBindingsChk
            // 
            resources.ApplyResources(this.writeBindingsChk, "writeBindingsChk");
            this.writeBindingsChk.CellHeight = 23;
            this.writeBindingsChk.CellWidth = 764;
            this.writeBindingsChk.Column = 1;
            this.writeBindingsChk.ColumnsSpanned = 0;
            this.writeBindingsChk.Name = "writeBindingsChk";
            this.writeBindingsChk.Row = 14;
            this.writeBindingsChk.RowsSpanned = 0;
            this.writeBindingsChk.UseVisualStyleBackColor = true;
            this.writeBindingsChk.YOffset = 0;
            // 
            // revertWarnChk
            // 
            resources.ApplyResources(this.revertWarnChk, "revertWarnChk");
            this.revertWarnChk.CellHeight = 23;
            this.revertWarnChk.CellWidth = 764;
            this.revertWarnChk.Checked = true;
            this.revertWarnChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.revertWarnChk.Column = 1;
            this.revertWarnChk.ColumnsSpanned = 0;
            this.revertWarnChk.Name = "revertWarnChk";
            this.revertWarnChk.Row = 4;
            this.revertWarnChk.RowsSpanned = 0;
            this.revertWarnChk.UseVisualStyleBackColor = true;
            this.revertWarnChk.YOffset = 0;
            // 
            // P4GeneralPreferencesControl
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridLayoutPanel1);
            this.Name = "P4GeneralPreferencesControl";
            this.Load += new System.EventHandler(this.P4GeneralPreferencesControl_Load);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutSubpanel4.ResumeLayout(false);
            this.gridLayoutSubpanel4.PerformLayout();
            this.gridLayoutSubpanel3.ResumeLayout(false);
            this.gridLayoutSubpanel3.PerformLayout();
            this.gridLayoutSubpanel2.ResumeLayout(false);
            this.gridLayoutSubpanel2.PerformLayout();
            this.gridLayoutSubpanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public P4GeneralPreferences OptionsPage
		{
			set
			{
				_customPage = value;
			}
		}

		private void P4GeneralPreferencesControl_Load(object sender, EventArgs e)
		{
			P4FormatRB.Checked = Preferences.LocalSettings.GetBool("P4Date_format", true);
			OSFormatRB.Checked = !P4FormatRB.Checked;

			revertWarnChk.Checked = Preferences.LocalSettings.GetBool("Revert_warn", true);
			checkoutLockChk.Checked = Preferences.LocalSettings.GetBool("Checkout_lock", false);
			promptChk.Checked = Preferences.LocalSettings.GetBool("PromptForChanglist", true);
            checkoutWriteableChk.Checked = Preferences.LocalSettings.GetBool("CheckoutWriteable", false);
            promptDel.Checked = Preferences.LocalSettings.GetBool("PromptForDelete", true);
            addChk.Checked = Preferences.LocalSettings.GetBool("Auto_Add", true);
			updateChk.Checked = Preferences.LocalSettings.GetBool("Update_Project", true);
			writeBindingsChk.Checked = Preferences.LocalSettings.GetBool("TagSolutionProjectFiles", false);
			OpenShelvedFileInEditorChk.Checked = Preferences.LocalSettings.GetBool("OpenShelvedFileInEditor", true);
			setProjLocationChk.Checked = Preferences.LocalSettings.GetBool("SetProjectFileLocation", true);
            warnSlnWorkspaceChk.Checked = Preferences.LocalSettings.GetBool("WarnSlnWorkspace", true);
            promptSlnSync.Checked = Preferences.LocalSettings.GetBool("PromptSlnSync", true);
            if (Preferences.LocalSettings.GetBool("QueryEditNeverSync", false))
            {
                QueryEditSyncOptionCB.SelectedIndex = 1;
            }
            else if (Preferences.LocalSettings.GetBool("QueryEditAlwaysSync", false))
            {
                QueryEditSyncOptionCB.SelectedIndex = 2;
            }
            else
            {
                QueryEditSyncOptionCB.SelectedIndex = 0;
            }
		}

		public void OnApply()
		{
			Preferences.LocalSettings["P4Date_format"] = P4FormatRB.Checked;
			//Preferences.LocalSettings["P4Date_format"] = OSFormatRB.Checked; the top two are radio buttons
			// in the same group, so only one of their checked status should affect the saved option.
			Preferences.LocalSettings["Revert_warn"] = revertWarnChk.Checked;
			Preferences.LocalSettings["Checkout_lock"] = checkoutLockChk.Checked;
			Preferences.LocalSettings["PromptForChanglist"] = promptChk.Checked;
            Preferences.LocalSettings["CheckoutWriteable"] = checkoutWriteableChk.Checked;
            Preferences.LocalSettings["PromptForDelete"] = promptDel.Checked;
            Preferences.LocalSettings["Auto_Add"] = addChk.Checked;
			Preferences.LocalSettings["Update_Project"] = updateChk.Checked;
			Preferences.LocalSettings["TagSolutionProjectFiles"] = writeBindingsChk.Checked;
			Preferences.LocalSettings["OpenShelvedFileInEditor"] = OpenShelvedFileInEditorChk.Checked;
			Preferences.LocalSettings["SetProjectFileLocation"] = setProjLocationChk.Checked;
            Preferences.LocalSettings["WarnSlnWorkspace"] = warnSlnWorkspaceChk.Checked;
            Preferences.LocalSettings["PromptSlnSync"] = promptSlnSync.Checked;

            Preferences.LocalSettings["QueryEditNeverSync"] = QueryEditSyncOptionCB.SelectedIndex == 1;
            Preferences.LocalSettings["QueryEditAlwaysSync"] = QueryEditSyncOptionCB.SelectedIndex == 2;

		}

		private void setProjLocationChk_CheckedChanged(object sender, EventArgs e)
		{
            if (P4VsProvider.CurrentScm != null)
            {
                string root = P4VsProvider.CurrentScm.Connection.WorkspaceRoot;
                P4ScmProvider scm = P4VsProvider.CurrentScm;

                try
                {
                    EnvDTE.Property prop = null;
                    EnvDTE.DTE dte2;
                    dte2 = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));

                    if (dte2 != null)
                    {
                        EnvDTE.Properties generalPnS = dte2.get_Properties("Environment", "ProjectsAndSolution");
                        foreach (EnvDTE.Property temp in generalPnS)
                        {
                            prop = temp;
                            if (prop.Name == "ProjectsLocation")
                            {
                                if (Directory.Exists(root))
                                {
                                    prop.Value = root;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    MessageBox.Show(msg, Resources.P4VS, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
		}

		public static P4GeneralPreferencesControl Instance { get; private set; }
    }
}
