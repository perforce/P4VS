
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
using System.Runtime.InteropServices;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;

namespace Perforce.P4VS
{
	/// <summary>
    /// Summary description for P4DiffMergePreferencesControl.
	/// </summary>
	public class P4DiffMergePreferencesControl : System.Windows.Forms.UserControl
	{

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private I18nControls.GridLabel diffLocationLbl;
		private I18nControls.GridTextBox diffLocationTB;
		private I18nControls.GridLabel diffAppLbl;
		private I18nControls.GridButton diffBrowseBtn;
		private I18nControls.GridLabel diffArgumentsLbl;
		private I18nControls.GridTextBox diffArgumentsTB;
		private I18nControls.GridLabel mergeArgumentsLbl;
		private I18nControls.GridTextBox mergeArgumentsTB;
		private I18nControls.GridButton mergeBrowseBtn;
		private I18nControls.GridRadioButton otherApp2RB;
		private I18nControls.GridRadioButton p4Merge2RB;
		private I18nControls.GridLabel mergeLocationLbl;
		private I18nControls.GridTextBox mergeLocationTB;
		private I18nControls.GridLabel mergeAppLbl;
		private I18nControls.GridGroupBox mergeAppGB;
		private OpenFileDialog diffFileDialog1;
        private OpenFileDialog mergeFileDialog2;
		private I18nControls.GridLabel diffLbl;
		private I18nControls.GridGroupBox groupBox2;
		private I18nControls.GridGroupBox groupBox1;
		private I18nControls.GridLayoutPanel gridLayoutPanel1;
		private I18nControls.GridLayoutSubpanel gridLayoutSubpanel2;
		private I18nControls.GridLayoutSubpanel gridLayoutSubpanel1;
		private I18nControls.GridPanel gridPanel1;
		private I18nControls.GridPanel gridPanel2;
		private I18nControls.GridRadioButton p4MergeRB;
		private I18nControls.GridRadioButton otherAppRB;
		private I18nControls.GridLayoutSubpanel gridLayoutSubpanel3;
		private I18nControls.GridLayoutSubpanel gridLayoutSubpanel4;
		private I18nControls.GridPanel gridPanel3;
		private I18nControls.GridPanel gridPanel4;
        private I18nControls.GridCheckBox reviewsChkBox;
        private I18nControls.GridLayoutSubpanel gridLayoutSubpanel5;
        private I18nControls.GridLabel reviewsLbl;
        private I18nControls.GridGroupBox groupBox3;

        // The parent page, use to persist data
        private P4DiffMergePreferences _customPage;

        public P4DiffMergePreferencesControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
				GC.SuppressFinalize(this);
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(P4DiffMergePreferencesControl));
            this.diffFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.mergeFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.gridLayoutPanel1 = new Perforce.I18nControls.GridLayoutPanel();
            this.reviewsChkBox = new Perforce.I18nControls.GridCheckBox();
            this.gridLayoutSubpanel5 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.reviewsLbl = new Perforce.I18nControls.GridLabel();
            this.groupBox3 = new Perforce.I18nControls.GridGroupBox();
            this.gridLayoutSubpanel3 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.gridPanel3 = new Perforce.I18nControls.GridPanel();
            this.p4MergeRB = new Perforce.I18nControls.GridRadioButton();
            this.otherAppRB = new Perforce.I18nControls.GridRadioButton();
            this.gridPanel2 = new Perforce.I18nControls.GridPanel();
            this.gridPanel1 = new Perforce.I18nControls.GridPanel();
            this.gridLayoutSubpanel2 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.mergeAppLbl = new Perforce.I18nControls.GridLabel();
            this.groupBox1 = new Perforce.I18nControls.GridGroupBox();
            this.gridLayoutSubpanel1 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.diffLbl = new Perforce.I18nControls.GridLabel();
            this.diffAppLbl = new Perforce.I18nControls.GridLabel();
            this.groupBox2 = new Perforce.I18nControls.GridGroupBox();
            this.diffLocationLbl = new Perforce.I18nControls.GridLabel();
            this.mergeLocationTB = new Perforce.I18nControls.GridTextBox();
            this.diffArgumentsTB = new Perforce.I18nControls.GridTextBox();
            this.mergeBrowseBtn = new Perforce.I18nControls.GridButton();
            this.diffArgumentsLbl = new Perforce.I18nControls.GridLabel();
            this.mergeArgumentsTB = new Perforce.I18nControls.GridTextBox();
            this.diffBrowseBtn = new Perforce.I18nControls.GridButton();
            this.mergeArgumentsLbl = new Perforce.I18nControls.GridLabel();
            this.mergeLocationLbl = new Perforce.I18nControls.GridLabel();
            this.diffLocationTB = new Perforce.I18nControls.GridTextBox();
            this.gridLayoutSubpanel4 = new Perforce.I18nControls.GridLayoutSubpanel();
            this.gridPanel4 = new Perforce.I18nControls.GridPanel();
            this.p4Merge2RB = new Perforce.I18nControls.GridRadioButton();
            this.otherApp2RB = new Perforce.I18nControls.GridRadioButton();
            this.mergeAppGB = new Perforce.I18nControls.GridGroupBox();
            this.gridLayoutPanel1.SuspendLayout();
            this.gridLayoutSubpanel5.SuspendLayout();
            this.gridLayoutSubpanel3.SuspendLayout();
            this.gridLayoutSubpanel2.SuspendLayout();
            this.gridLayoutSubpanel1.SuspendLayout();
            this.gridLayoutSubpanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // diffFileDialog1
            // 
            this.diffFileDialog1.FileName = "openFileDialog1";
            // 
            // mergeFileDialog2
            // 
            this.mergeFileDialog2.FileName = "openFileDialog1";
            // 
            // gridLayoutPanel1
            // 
            this.gridLayoutPanel1.Controls.Add(this.reviewsChkBox);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel5);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel3);
            this.gridLayoutPanel1.Controls.Add(this.gridPanel2);
            this.gridLayoutPanel1.Controls.Add(this.gridPanel1);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel2);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel1);
            this.gridLayoutPanel1.Controls.Add(this.diffLocationLbl);
            this.gridLayoutPanel1.Controls.Add(this.mergeLocationTB);
            this.gridLayoutPanel1.Controls.Add(this.diffArgumentsTB);
            this.gridLayoutPanel1.Controls.Add(this.mergeBrowseBtn);
            this.gridLayoutPanel1.Controls.Add(this.diffArgumentsLbl);
            this.gridLayoutPanel1.Controls.Add(this.mergeArgumentsTB);
            this.gridLayoutPanel1.Controls.Add(this.diffBrowseBtn);
            this.gridLayoutPanel1.Controls.Add(this.mergeArgumentsLbl);
            this.gridLayoutPanel1.Controls.Add(this.mergeLocationLbl);
            this.gridLayoutPanel1.Controls.Add(this.diffLocationTB);
            this.gridLayoutPanel1.Controls.Add(this.gridLayoutSubpanel4);
            resources.ApplyResources(this.gridLayoutPanel1, "gridLayoutPanel1");
            this.gridLayoutPanel1.EnableDesignerGrid = false;
            this.gridLayoutPanel1.EnableDesignerLayout = true;
            this.gridLayoutPanel1.EnableParentResize = false;
            this.gridLayoutPanel1.MinimumColumnWidth = 10;
            this.gridLayoutPanel1.MinimumRowHeight = 10;
            this.gridLayoutPanel1.Name = "gridLayoutPanel1";
            // 
            // reviewsChkBox
            // 
            resources.ApplyResources(this.reviewsChkBox, "reviewsChkBox");
            this.reviewsChkBox.CellHeight = 23;
            this.reviewsChkBox.CellWidth = 440;
            this.reviewsChkBox.Column = 1;
            this.reviewsChkBox.ColumnsSpanned = 3;
            this.reviewsChkBox.Name = "reviewsChkBox";
            this.reviewsChkBox.Row = 9;
            this.reviewsChkBox.RowsSpanned = 0;
            this.reviewsChkBox.UseVisualStyleBackColor = true;
            this.reviewsChkBox.YOffset = 0;
            // 
            // gridLayoutSubpanel5
            // 
            resources.ApplyResources(this.gridLayoutSubpanel5, "gridLayoutSubpanel5");
            this.gridLayoutSubpanel5.CellHeight = 19;
            this.gridLayoutSubpanel5.CellWidth = 440;
            this.gridLayoutSubpanel5.Column = 0;
            this.gridLayoutSubpanel5.ColumnsSpanned = 4;
            this.gridLayoutSubpanel5.Controls.Add(this.reviewsLbl);
            this.gridLayoutSubpanel5.Controls.Add(this.groupBox3);
            this.gridLayoutSubpanel5.EnableDesignerGrid = false;
            this.gridLayoutSubpanel5.EnableDesignerLayout = true;
            this.gridLayoutSubpanel5.EnableParentResize = false;
            this.gridLayoutSubpanel5.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel5.MinimumRowHeight = 10;
            this.gridLayoutSubpanel5.Name = "gridLayoutSubpanel5";
            this.gridLayoutSubpanel5.Row = 8;
            this.gridLayoutSubpanel5.RowsSpanned = 0;
            this.gridLayoutSubpanel5.YOffset = 0;
            // 
            // reviewsLbl
            // 
            resources.ApplyResources(this.reviewsLbl, "reviewsLbl");
            this.reviewsLbl.CellHeight = 0;
            this.reviewsLbl.CellWidth = 0;
            this.reviewsLbl.Column = 0;
            this.reviewsLbl.ColumnsSpanned = 0;
            this.reviewsLbl.Name = "reviewsLbl";
            this.reviewsLbl.Row = 0;
            this.reviewsLbl.RowsSpanned = 0;
            this.reviewsLbl.YOffset = 0;
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.CellHeight = 11;
            this.groupBox3.CellWidth = 434;
            this.groupBox3.Column = 0;
            this.groupBox3.ColumnsSpanned = 0;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Row = 0;
            this.groupBox3.RowsSpanned = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.YOffset = 0;
            // 
            // gridLayoutSubpanel3
            // 
            resources.ApplyResources(this.gridLayoutSubpanel3, "gridLayoutSubpanel3");
            this.gridLayoutSubpanel3.CellHeight = 49;
            this.gridLayoutSubpanel3.CellWidth = 430;
            this.gridLayoutSubpanel3.Column = 1;
            this.gridLayoutSubpanel3.ColumnsSpanned = 3;
            this.gridLayoutSubpanel3.Controls.Add(this.gridPanel3);
            this.gridLayoutSubpanel3.Controls.Add(this.p4MergeRB);
            this.gridLayoutSubpanel3.Controls.Add(this.otherAppRB);
            this.gridLayoutSubpanel3.EnableDesignerGrid = false;
            this.gridLayoutSubpanel3.EnableDesignerLayout = false;
            this.gridLayoutSubpanel3.EnableParentResize = false;
            this.gridLayoutSubpanel3.MinimumColumnWidth = 0;
            this.gridLayoutSubpanel3.MinimumRowHeight = 0;
            this.gridLayoutSubpanel3.Name = "gridLayoutSubpanel3";
            this.gridLayoutSubpanel3.Row = 1;
            this.gridLayoutSubpanel3.RowsSpanned = 0;
            this.gridLayoutSubpanel3.YOffset = 0;
            // 
            // gridPanel3
            // 
            resources.ApplyResources(this.gridPanel3, "gridPanel3");
            this.gridPanel3.CellHeight = 23;
            this.gridPanel3.CellWidth = 312;
            this.gridPanel3.Column = 1;
            this.gridPanel3.ColumnsSpanned = 0;
            this.gridPanel3.Name = "gridPanel3";
            this.gridPanel3.Row = 0;
            this.gridPanel3.RowsSpanned = 0;
            this.gridPanel3.YOffset = 2;
            // 
            // p4MergeRB
            // 
            resources.ApplyResources(this.p4MergeRB, "p4MergeRB");
            this.p4MergeRB.CellHeight = 23;
            this.p4MergeRB.CellWidth = 112;
            this.p4MergeRB.Checked = true;
            this.p4MergeRB.Column = 0;
            this.p4MergeRB.ColumnsSpanned = 0;
            this.p4MergeRB.Name = "p4MergeRB";
            this.p4MergeRB.Row = 0;
            this.p4MergeRB.RowsSpanned = 0;
            this.p4MergeRB.TabStop = true;
            this.p4MergeRB.UseVisualStyleBackColor = true;
            this.p4MergeRB.YOffset = 0;
            this.p4MergeRB.Click += new System.EventHandler(this.p4MergeRB_Click);
            // 
            // otherAppRB
            // 
            resources.ApplyResources(this.otherAppRB, "otherAppRB");
            this.otherAppRB.CellHeight = 20;
            this.otherAppRB.CellWidth = 112;
            this.otherAppRB.Column = 0;
            this.otherAppRB.ColumnsSpanned = 0;
            this.otherAppRB.Name = "otherAppRB";
            this.otherAppRB.Row = 1;
            this.otherAppRB.RowsSpanned = 0;
            this.otherAppRB.UseVisualStyleBackColor = true;
            this.otherAppRB.YOffset = 0;
            this.otherAppRB.Click += new System.EventHandler(this.otherAppRB_Click);
            // 
            // gridPanel2
            // 
            this.gridPanel2.CellHeight = 31;
            this.gridPanel2.CellWidth = 16;
            this.gridPanel2.Column = 1;
            this.gridPanel2.ColumnsSpanned = 0;
            resources.ApplyResources(this.gridPanel2, "gridPanel2");
            this.gridPanel2.Name = "gridPanel2";
            this.gridPanel2.Row = 2;
            this.gridPanel2.RowsSpanned = 0;
            this.gridPanel2.YOffset = 0;
            // 
            // gridPanel1
            // 
            resources.ApplyResources(this.gridPanel1, "gridPanel1");
            this.gridPanel1.CellHeight = 69;
            this.gridPanel1.CellWidth = 252;
            this.gridPanel1.Column = 3;
            this.gridPanel1.ColumnsSpanned = 0;
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Row = 10;
            this.gridPanel1.RowsSpanned = 0;
            this.gridPanel1.YOffset = 0;
            // 
            // gridLayoutSubpanel2
            // 
            resources.ApplyResources(this.gridLayoutSubpanel2, "gridLayoutSubpanel2");
            this.gridLayoutSubpanel2.CellHeight = 19;
            this.gridLayoutSubpanel2.CellWidth = 440;
            this.gridLayoutSubpanel2.Column = 0;
            this.gridLayoutSubpanel2.ColumnsSpanned = 4;
            this.gridLayoutSubpanel2.Controls.Add(this.mergeAppLbl);
            this.gridLayoutSubpanel2.Controls.Add(this.groupBox1);
            this.gridLayoutSubpanel2.EnableDesignerGrid = false;
            this.gridLayoutSubpanel2.EnableDesignerLayout = true;
            this.gridLayoutSubpanel2.EnableParentResize = false;
            this.gridLayoutSubpanel2.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel2.MinimumRowHeight = 10;
            this.gridLayoutSubpanel2.Name = "gridLayoutSubpanel2";
            this.gridLayoutSubpanel2.Row = 4;
            this.gridLayoutSubpanel2.RowsSpanned = 0;
            this.gridLayoutSubpanel2.YOffset = 0;
            // 
            // mergeAppLbl
            // 
            resources.ApplyResources(this.mergeAppLbl, "mergeAppLbl");
            this.mergeAppLbl.CellHeight = 0;
            this.mergeAppLbl.CellWidth = 0;
            this.mergeAppLbl.Column = 0;
            this.mergeAppLbl.ColumnsSpanned = 0;
            this.mergeAppLbl.Name = "mergeAppLbl";
            this.mergeAppLbl.Row = 0;
            this.mergeAppLbl.RowsSpanned = 0;
            this.mergeAppLbl.YOffset = 0;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.CellHeight = 11;
            this.groupBox1.CellWidth = 434;
            this.groupBox1.Column = 0;
            this.groupBox1.ColumnsSpanned = 0;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Row = 0;
            this.groupBox1.RowsSpanned = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.YOffset = 0;
            // 
            // gridLayoutSubpanel1
            // 
            resources.ApplyResources(this.gridLayoutSubpanel1, "gridLayoutSubpanel1");
            this.gridLayoutSubpanel1.CellHeight = 19;
            this.gridLayoutSubpanel1.CellWidth = 440;
            this.gridLayoutSubpanel1.Column = 0;
            this.gridLayoutSubpanel1.ColumnsSpanned = 4;
            this.gridLayoutSubpanel1.Controls.Add(this.diffLbl);
            this.gridLayoutSubpanel1.Controls.Add(this.diffAppLbl);
            this.gridLayoutSubpanel1.Controls.Add(this.groupBox2);
            this.gridLayoutSubpanel1.EnableDesignerGrid = false;
            this.gridLayoutSubpanel1.EnableDesignerLayout = true;
            this.gridLayoutSubpanel1.EnableParentResize = false;
            this.gridLayoutSubpanel1.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel1.MinimumRowHeight = 10;
            this.gridLayoutSubpanel1.Name = "gridLayoutSubpanel1";
            this.gridLayoutSubpanel1.Row = 0;
            this.gridLayoutSubpanel1.RowsSpanned = 0;
            this.gridLayoutSubpanel1.YOffset = 0;
            // 
            // diffLbl
            // 
            resources.ApplyResources(this.diffLbl, "diffLbl");
            this.diffLbl.CellHeight = 0;
            this.diffLbl.CellWidth = 0;
            this.diffLbl.Column = 0;
            this.diffLbl.ColumnsSpanned = 0;
            this.diffLbl.Name = "diffLbl";
            this.diffLbl.Row = 0;
            this.diffLbl.RowsSpanned = 0;
            this.diffLbl.YOffset = 0;
            // 
            // diffAppLbl
            // 
            resources.ApplyResources(this.diffAppLbl, "diffAppLbl");
            this.diffAppLbl.CellHeight = 13;
            this.diffAppLbl.CellWidth = 118;
            this.diffAppLbl.Column = 0;
            this.diffAppLbl.ColumnsSpanned = 0;
            this.diffAppLbl.Name = "diffAppLbl";
            this.diffAppLbl.Row = 0;
            this.diffAppLbl.RowsSpanned = 0;
            this.diffAppLbl.YOffset = 0;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.CellHeight = 13;
            this.groupBox2.CellWidth = 316;
            this.groupBox2.Column = 1;
            this.groupBox2.ColumnsSpanned = 0;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Row = 0;
            this.groupBox2.RowsSpanned = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.YOffset = 5;
            // 
            // diffLocationLbl
            // 
            resources.ApplyResources(this.diffLocationLbl, "diffLocationLbl");
            this.diffLocationLbl.AutoEllipsis = true;
            this.diffLocationLbl.CellHeight = 31;
            this.diffLocationLbl.CellWidth = 66;
            this.diffLocationLbl.Column = 2;
            this.diffLocationLbl.ColumnsSpanned = 0;
            this.diffLocationLbl.Name = "diffLocationLbl";
            this.diffLocationLbl.Row = 2;
            this.diffLocationLbl.RowsSpanned = 0;
            this.diffLocationLbl.YOffset = 9;
            // 
            // mergeLocationTB
            // 
            resources.ApplyResources(this.mergeLocationTB, "mergeLocationTB");
            this.mergeLocationTB.CellHeight = 31;
            this.mergeLocationTB.CellWidth = 252;
            this.mergeLocationTB.Column = 3;
            this.mergeLocationTB.ColumnsSpanned = 0;
            this.mergeLocationTB.Name = "mergeLocationTB";
            this.mergeLocationTB.Row = 6;
            this.mergeLocationTB.RowsSpanned = 0;
            this.mergeLocationTB.YOffset = 2;
            // 
            // diffArgumentsTB
            // 
            resources.ApplyResources(this.diffArgumentsTB, "diffArgumentsTB");
            this.diffArgumentsTB.CellHeight = 33;
            this.diffArgumentsTB.CellWidth = 252;
            this.diffArgumentsTB.Column = 3;
            this.diffArgumentsTB.ColumnsSpanned = 0;
            this.diffArgumentsTB.Name = "diffArgumentsTB";
            this.diffArgumentsTB.Row = 3;
            this.diffArgumentsTB.RowsSpanned = 0;
            this.diffArgumentsTB.YOffset = 0;
            // 
            // mergeBrowseBtn
            // 
            resources.ApplyResources(this.mergeBrowseBtn, "mergeBrowseBtn");
            this.mergeBrowseBtn.CellHeight = 31;
            this.mergeBrowseBtn.CellWidth = 96;
            this.mergeBrowseBtn.Column = 4;
            this.mergeBrowseBtn.ColumnsSpanned = 0;
            this.mergeBrowseBtn.Name = "mergeBrowseBtn";
            this.mergeBrowseBtn.Row = 6;
            this.mergeBrowseBtn.RowsSpanned = 0;
            this.mergeBrowseBtn.UseVisualStyleBackColor = true;
            this.mergeBrowseBtn.YOffset = 0;
            this.mergeBrowseBtn.Click += new System.EventHandler(this.mergeBrowseBtn_Click);
            // 
            // diffArgumentsLbl
            // 
            resources.ApplyResources(this.diffArgumentsLbl, "diffArgumentsLbl");
            this.diffArgumentsLbl.AutoEllipsis = true;
            this.diffArgumentsLbl.CellHeight = 33;
            this.diffArgumentsLbl.CellWidth = 66;
            this.diffArgumentsLbl.Column = 2;
            this.diffArgumentsLbl.ColumnsSpanned = 0;
            this.diffArgumentsLbl.Name = "diffArgumentsLbl";
            this.diffArgumentsLbl.Row = 3;
            this.diffArgumentsLbl.RowsSpanned = 0;
            this.diffArgumentsLbl.YOffset = 7;
            // 
            // mergeArgumentsTB
            // 
            resources.ApplyResources(this.mergeArgumentsTB, "mergeArgumentsTB");
            this.mergeArgumentsTB.CellHeight = 26;
            this.mergeArgumentsTB.CellWidth = 252;
            this.mergeArgumentsTB.Column = 3;
            this.mergeArgumentsTB.ColumnsSpanned = 0;
            this.mergeArgumentsTB.Name = "mergeArgumentsTB";
            this.mergeArgumentsTB.Row = 7;
            this.mergeArgumentsTB.RowsSpanned = 0;
            this.mergeArgumentsTB.YOffset = 0;
            // 
            // diffBrowseBtn
            // 
            resources.ApplyResources(this.diffBrowseBtn, "diffBrowseBtn");
            this.diffBrowseBtn.CellHeight = 31;
            this.diffBrowseBtn.CellWidth = 96;
            this.diffBrowseBtn.Column = 4;
            this.diffBrowseBtn.ColumnsSpanned = 0;
            this.diffBrowseBtn.Name = "diffBrowseBtn";
            this.diffBrowseBtn.Row = 2;
            this.diffBrowseBtn.RowsSpanned = 0;
            this.diffBrowseBtn.UseVisualStyleBackColor = true;
            this.diffBrowseBtn.YOffset = 3;
            this.diffBrowseBtn.Click += new System.EventHandler(this.diffBrowseBtn_Click);
            // 
            // mergeArgumentsLbl
            // 
            resources.ApplyResources(this.mergeArgumentsLbl, "mergeArgumentsLbl");
            this.mergeArgumentsLbl.AutoEllipsis = true;
            this.mergeArgumentsLbl.CellHeight = 26;
            this.mergeArgumentsLbl.CellWidth = 66;
            this.mergeArgumentsLbl.Column = 2;
            this.mergeArgumentsLbl.ColumnsSpanned = 0;
            this.mergeArgumentsLbl.Name = "mergeArgumentsLbl";
            this.mergeArgumentsLbl.Row = 7;
            this.mergeArgumentsLbl.RowsSpanned = 0;
            this.mergeArgumentsLbl.YOffset = 3;
            // 
            // mergeLocationLbl
            // 
            resources.ApplyResources(this.mergeLocationLbl, "mergeLocationLbl");
            this.mergeLocationLbl.AutoEllipsis = true;
            this.mergeLocationLbl.CellHeight = 31;
            this.mergeLocationLbl.CellWidth = 66;
            this.mergeLocationLbl.Column = 2;
            this.mergeLocationLbl.ColumnsSpanned = 0;
            this.mergeLocationLbl.Name = "mergeLocationLbl";
            this.mergeLocationLbl.Row = 6;
            this.mergeLocationLbl.RowsSpanned = 0;
            this.mergeLocationLbl.YOffset = 6;
            // 
            // diffLocationTB
            // 
            resources.ApplyResources(this.diffLocationTB, "diffLocationTB");
            this.diffLocationTB.CellHeight = 31;
            this.diffLocationTB.CellWidth = 252;
            this.diffLocationTB.Column = 3;
            this.diffLocationTB.ColumnsSpanned = 0;
            this.diffLocationTB.Name = "diffLocationTB";
            this.diffLocationTB.Row = 2;
            this.diffLocationTB.RowsSpanned = 0;
            this.diffLocationTB.YOffset = 5;
            // 
            // gridLayoutSubpanel4
            // 
            resources.ApplyResources(this.gridLayoutSubpanel4, "gridLayoutSubpanel4");
            this.gridLayoutSubpanel4.CellHeight = 49;
            this.gridLayoutSubpanel4.CellWidth = 430;
            this.gridLayoutSubpanel4.Column = 1;
            this.gridLayoutSubpanel4.ColumnsSpanned = 3;
            this.gridLayoutSubpanel4.Controls.Add(this.gridPanel4);
            this.gridLayoutSubpanel4.Controls.Add(this.p4Merge2RB);
            this.gridLayoutSubpanel4.Controls.Add(this.otherApp2RB);
            this.gridLayoutSubpanel4.EnableDesignerGrid = false;
            this.gridLayoutSubpanel4.EnableDesignerLayout = false;
            this.gridLayoutSubpanel4.EnableParentResize = false;
            this.gridLayoutSubpanel4.MinimumColumnWidth = 10;
            this.gridLayoutSubpanel4.MinimumRowHeight = 10;
            this.gridLayoutSubpanel4.Name = "gridLayoutSubpanel4";
            this.gridLayoutSubpanel4.Row = 5;
            this.gridLayoutSubpanel4.RowsSpanned = 0;
            this.gridLayoutSubpanel4.YOffset = 0;
            // 
            // gridPanel4
            // 
            resources.ApplyResources(this.gridPanel4, "gridPanel4");
            this.gridPanel4.CellHeight = 23;
            this.gridPanel4.CellWidth = 309;
            this.gridPanel4.Column = 1;
            this.gridPanel4.ColumnsSpanned = 0;
            this.gridPanel4.Name = "gridPanel4";
            this.gridPanel4.Row = 0;
            this.gridPanel4.RowsSpanned = 0;
            this.gridPanel4.YOffset = 2;
            // 
            // p4Merge2RB
            // 
            resources.ApplyResources(this.p4Merge2RB, "p4Merge2RB");
            this.p4Merge2RB.CellHeight = 23;
            this.p4Merge2RB.CellWidth = 112;
            this.p4Merge2RB.Checked = true;
            this.p4Merge2RB.Column = 0;
            this.p4Merge2RB.ColumnsSpanned = 0;
            this.p4Merge2RB.Name = "p4Merge2RB";
            this.p4Merge2RB.Row = 0;
            this.p4Merge2RB.RowsSpanned = 0;
            this.p4Merge2RB.TabStop = true;
            this.p4Merge2RB.UseVisualStyleBackColor = true;
            this.p4Merge2RB.YOffset = 0;
            this.p4Merge2RB.Click += new System.EventHandler(this.p4Merge2RB_Click);
            // 
            // otherApp2RB
            // 
            resources.ApplyResources(this.otherApp2RB, "otherApp2RB");
            this.otherApp2RB.CellHeight = 20;
            this.otherApp2RB.CellWidth = 112;
            this.otherApp2RB.Column = 0;
            this.otherApp2RB.ColumnsSpanned = 0;
            this.otherApp2RB.Name = "otherApp2RB";
            this.otherApp2RB.Row = 1;
            this.otherApp2RB.RowsSpanned = 0;
            this.otherApp2RB.UseVisualStyleBackColor = true;
            this.otherApp2RB.YOffset = 0;
            this.otherApp2RB.Click += new System.EventHandler(this.otherApp2RB_Click);
            // 
            // mergeAppGB
            // 
            this.mergeAppGB.CellHeight = 0;
            this.mergeAppGB.CellWidth = 0;
            this.mergeAppGB.Column = 0;
            this.mergeAppGB.ColumnsSpanned = 0;
            resources.ApplyResources(this.mergeAppGB, "mergeAppGB");
            this.mergeAppGB.Name = "mergeAppGB";
            this.mergeAppGB.Row = 0;
            this.mergeAppGB.RowsSpanned = 0;
            this.mergeAppGB.TabStop = false;
            this.mergeAppGB.YOffset = 0;
            // 
            // P4DiffMergePreferencesControl
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridLayoutPanel1);
            this.Name = "P4DiffMergePreferencesControl";
            this.Load += new System.EventHandler(this.P4DiffMergePreferencesControl_Load);
            this.gridLayoutPanel1.ResumeLayout(false);
            this.gridLayoutPanel1.PerformLayout();
            this.gridLayoutSubpanel5.ResumeLayout(false);
            this.gridLayoutSubpanel5.PerformLayout();
            this.gridLayoutSubpanel3.ResumeLayout(false);
            this.gridLayoutSubpanel3.PerformLayout();
            this.gridLayoutSubpanel2.ResumeLayout(false);
            this.gridLayoutSubpanel2.PerformLayout();
            this.gridLayoutSubpanel1.ResumeLayout(false);
            this.gridLayoutSubpanel1.PerformLayout();
            this.gridLayoutSubpanel4.ResumeLayout(false);
            this.gridLayoutSubpanel4.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion
    
        public P4DiffMergePreferences OptionsPage
        {
            set
            {
                _customPage = value;
            }
        }

		private void diffBrowseBtn_Click(object sender, EventArgs e)
		{
			Stream logSelectStream;
			OpenFileDialog openFileDialog = new OpenFileDialog();

			openFileDialog.Filter = Resources.P4DiffMergePreferencesControl_openFileDialogFilter;
			string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			openFileDialog.InitialDirectory = programFiles;
			openFileDialog.FilterIndex = 1;
			openFileDialog.RestoreDirectory = true;
			openFileDialog.Title = Resources.P4DiffMergePreferencesControl_openFileDialogTitle;

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				if ((logSelectStream = openFileDialog.OpenFile()) != null)
				{
					// Code to write the stream goes here.
					diffLocationTB.Text = openFileDialog.FileName;
					logSelectStream.Close();
				}
			}

		}

		private void mergeBrowseBtn_Click(object sender, EventArgs e)
		{
			Stream logSelectStream;
			OpenFileDialog openFileDialog = new OpenFileDialog();

			openFileDialog.Filter = Resources.P4DiffMergePreferencesControl_openFileDialogFilter;
			string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			openFileDialog.InitialDirectory = programFiles;
			openFileDialog.FilterIndex = 1;
			openFileDialog.RestoreDirectory = true;
			openFileDialog.Title = Resources.P4DiffMergePreferencesControl_openFileDialogTitle;

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				if ((logSelectStream = openFileDialog.OpenFile()) != null)
				{
					// Code to write the stream goes here.
					mergeLocationTB.Text = openFileDialog.FileName;
					logSelectStream.Close();
				}
			}

		}

		private void P4DiffMergePreferencesControl_Load(object sender, EventArgs e)
		{
			if (Preferences.LocalSettings.ContainsKey("P4Diff_app"))
			{
				if ((bool)Preferences.LocalSettings["P4Diff_app"] == true)
				{
					p4MergeRB.Checked = true;
					otherAppRB.Checked = false;
					diffLocationTB.Enabled = false;
					diffBrowseBtn.Enabled = false;
					diffArgumentsTB.Enabled = false;
					if (Preferences.LocalSettings.ContainsKey("P4Diff_path"))
					{
						diffLocationTB.Text = Preferences.LocalSettings["P4Diff_path"].ToString();
					}
				}
				else
				{
					p4MergeRB.Checked = false;
					otherAppRB.Checked = true;
					diffLocationTB.Enabled = true;
					diffBrowseBtn.Enabled = true;
					diffArgumentsTB.Enabled = true;
					if (Preferences.LocalSettings.ContainsKey("P4Diff_path"))
					{
						diffLocationTB.Text = Preferences.LocalSettings["P4Diff_path"].ToString();
					}
					if (Preferences.LocalSettings.ContainsKey("Diff_args"))
					{
						diffArgumentsTB.Text = Preferences.LocalSettings["Diff_args"].ToString();
					}
				}
			}

			if (Preferences.LocalSettings.ContainsKey("P4Merge_app"))
			{
				if ((bool)Preferences.LocalSettings["P4Merge_app"] == true)
				{
					p4Merge2RB.Checked = true;
					otherApp2RB.Checked = false;
					mergeLocationTB.Enabled = false;
					mergeBrowseBtn.Enabled = false;
					mergeArgumentsTB.Enabled = false;
					if (Preferences.LocalSettings.ContainsKey("P4Merge_path"))
					{
						mergeLocationTB.Text = Preferences.LocalSettings["P4Merge_path"].ToString();
					}
				}
				else
				{
					p4Merge2RB.Checked = false;
					otherApp2RB.Checked = true;
					mergeLocationTB.Enabled = true;
					mergeBrowseBtn.Enabled = true;
					mergeArgumentsTB.Enabled = true;
					if (Preferences.LocalSettings.ContainsKey("P4Merge_path"))
					{
						mergeLocationTB.Text = Preferences.LocalSettings["P4Merge_path"].ToString();
					}
					if (Preferences.LocalSettings.ContainsKey("Merge_args"))
					{
						mergeArgumentsTB.Text = Preferences.LocalSettings["Merge_args"].ToString();
					}
				}
			}
            reviewsChkBox.Checked = Preferences.LocalSettings.GetBool("Launch_Swarm_Browser", false);
        }

		public void OnApply()
		{
			if ((p4MergeRB.Checked == true)&& (otherAppRB.Checked ==false))
			{
				Preferences.LocalSettings["P4Diff_app"] = true;
				Preferences.LocalSettings["P4Diff_path"] = diffLocationTB.Text;
			}

			if ((p4MergeRB.Checked == false) && (otherAppRB.Checked ==true))
			{
				Preferences.LocalSettings["P4Diff_app"] = false;
				Preferences.LocalSettings["P4Diff_path"] = diffLocationTB.Text;
				Preferences.LocalSettings["Diff_args"] = diffArgumentsTB.Text;
			}

			if ((p4Merge2RB.Checked == true) && (otherApp2RB.Checked == false))
			{
				Preferences.LocalSettings["P4Merge_app"] = true;
				Preferences.LocalSettings["P4Merge_path"] = mergeLocationTB.Text;
			}

			if ((p4Merge2RB.Checked == false) && (otherApp2RB.Checked == true))
			{
				Preferences.LocalSettings["P4Merge_app"] = false;
				Preferences.LocalSettings["P4Merge_path"] = mergeLocationTB.Text;
				Preferences.LocalSettings["Merge_args"] = mergeArgumentsTB.Text;
			}
            Preferences.LocalSettings["Launch_Swarm_Browser"] = reviewsChkBox.Checked;
		}

		private void otherAppRB_Click(object sender, EventArgs e)
		{
			diffLocationTB.Enabled = true;
			diffBrowseBtn.Enabled = true;
			diffArgumentsTB.Enabled = true;
		}

		private void otherApp2RB_Click(object sender, EventArgs e)
		{
			mergeLocationTB.Enabled = true;
			mergeBrowseBtn.Enabled = true;
			mergeArgumentsTB.Enabled = true;
		}

		private void p4MergeRB_Click(object sender, EventArgs e)
		{
			diffLocationTB.Enabled = false;
			diffBrowseBtn.Enabled = false;
			diffArgumentsTB.Enabled = false;
		}

		private void p4Merge2RB_Click(object sender, EventArgs e)
		{
			mergeLocationTB.Enabled = false;
			mergeBrowseBtn.Enabled = false;
			mergeArgumentsTB.Enabled = false;
		}
    }
}
